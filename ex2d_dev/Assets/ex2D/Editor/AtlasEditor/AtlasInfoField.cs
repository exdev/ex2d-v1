// ======================================================================================
// File         : AtlasInfoField.cs
// Author       : Wu Jie 
// Last Change  : 07/06/2011 | 09:54:11 AM | Wednesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// class exAtlasEditor
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

partial class exAtlasEditor : EditorWindow {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AtlasInfoField ( Rect _rect, int _borderSize, exAtlasInfo _atlasInfo ) {

        Texture2D texCheckerboard = exEditorHelper.CheckerboardTexture();
        float boxWidth = (float)_atlasInfo.width * _atlasInfo.scale + 2.0f * _borderSize; // box border
        float boxHeight = (float)_atlasInfo.height * _atlasInfo.scale + 2.0f * _borderSize; // box border
        Rect scaledRect =  new Rect( _rect.x, _rect.y, boxWidth, boxHeight ); 

        // ======================================================== 
        // draw background textures
        // ======================================================== 

        GUI.BeginGroup( new Rect(_rect.x, 
                                 _rect.y, 
                                 (float)_atlasInfo.width * _atlasInfo.scale, 
                                 (float)_atlasInfo.height * _atlasInfo.scale) );
        Color old = GUI.color;
        GUI.color = new Color ( _atlasInfo.bgColor.r, 
                                _atlasInfo.bgColor.g,
                                _atlasInfo.bgColor.b,
                                1.0f );
        if ( _atlasInfo.showCheckerboard ) {
            int col = Mathf.CeilToInt(_atlasInfo.width * _atlasInfo.scale / texCheckerboard.width);
            int row = Mathf.CeilToInt(_atlasInfo.height * _atlasInfo.scale / texCheckerboard.height);
            for ( int i = 0; i < col; ++i ) {
                for ( int j = 0; j < row; ++j ) {
                    Rect size = new Rect( i * texCheckerboard.width,
                                          j * texCheckerboard.height,
                                          texCheckerboard.width,
                                          texCheckerboard.height );
                    GUI.DrawTexture( size, texCheckerboard );
                }
            }
        }
        else {
            GUI.DrawTexture( new Rect( 0, 
                                       0, 
                                       _atlasInfo.width * _atlasInfo.scale,
                                       _atlasInfo.height * _atlasInfo.scale ), 
                             exEditorHelper.WhiteTexture() );
        }
        GUI.color = old;
        GUI.EndGroup();

        // ======================================================== 
        // draw the gui box
        // ======================================================== 

        GUIContent bgContent = new GUIContent();
        if ( _atlasInfo.elements.Count == 0 ) {
            bgContent.text = "Drag Textures On It";
            bgContent.tooltip = "Drag Textures to create atlas";
        }
        else {
            bgContent.text = "";
        }

        Color oldBGColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.black;
        GUI.Box ( new Rect( _rect.x - _borderSize, _rect.y - _borderSize, boxWidth, boxHeight), 
                  bgContent, 
                  exEditorHelper.RectBorderStyle() );
        GUI.backgroundColor = oldBGColor;

        // ======================================================== 
        // exAtlasInfo.Element
        // ======================================================== 

        List<exAtlasInfo.Element> invalidElements = new List<exAtlasInfo.Element>();
        foreach ( exAtlasInfo.Element el in _atlasInfo.elements ) {
            if ( el.texture == null ) {
                invalidElements.Add(el);
                continue;
            }

            if ( el.isFontElement &&
                 ( el.srcFontInfo == null || 
                   el.destFontInfo == null ) ) 
            {
                invalidElements.Add(el);
                continue;
            }

            AtlasElementField ( scaledRect, _atlasInfo, el );
        }
        foreach ( exAtlasInfo.Element el in invalidElements ) {
            _atlasInfo.RemoveElement(el);
        }

        // ======================================================== 
        // handle drop event 
        Event e = Event.current;
        // ======================================================== 

        if ( scaledRect.Contains(e.mousePosition) ) {
            if ( e.type == EventType.DragUpdated ) {
                // Show a copy icon on the drag
                foreach ( Object o in DragAndDrop.objectReferences ) {
                    if ( o is Texture2D || 
                         (o is exBitmapFont && (o as exBitmapFont).inAtlas == false) ||
                         exEditorHelper.IsDirectory(o) ) 
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        break;
                    }
                }
            }
            else if ( e.type == EventType.DragPerform ) {
                DragAndDrop.AcceptDrag();

                // NOTE: Unity3D have a problem in ImportTextureForAtlas, when a texture is an active selection, 
                //       no matter how you change your import settings, finally it will apply changes that in Inspector (shows when object selected)
                oldSelActiveObject = null;
                oldSelObjects.Clear();
                foreach ( Object o in Selection.objects ) {
                    oldSelObjects.Add(o);
                }
                oldSelActiveObject = Selection.activeObject;

                // NOTE: Selection.GetFiltered only affect on activeObject, but we may proceed non-active selections sometimes
                foreach ( Object o in DragAndDrop.objectReferences ) {
                    if ( exEditorHelper.IsDirectory(o) ) {
                        Selection.activeObject = o;

                        // add Texture2D objects
                        Object[] objs = Selection.GetFiltered( typeof(Texture2D), SelectionMode.DeepAssets);
                        importObjects.AddRange(objs);

                        // add exBitmapFont objects
                        objs = Selection.GetFiltered( typeof(exBitmapFont), SelectionMode.DeepAssets);
                        importObjects.AddRange(objs);
                    }
                    else if ( o is Texture2D || o is exBitmapFont ) {
                        importObjects.Add(o);
                    }
                }

                //
                Selection.activeObject = null;

                //
                doImport = true;
                Repaint();
            }
        }

        //
        GUILayoutUtility.GetRect ( boxWidth, boxHeight );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AtlasElementField ( Rect _atlasRect, exAtlasInfo _atlasInfo, exAtlasInfo.Element _el ) {
        Color oldBGColor = GUI.backgroundColor;
        Rect srcRect; 
        Rect rect = new Rect( _el.coord[0] * _atlasInfo.scale, 
                              _el.coord[1] * _atlasInfo.scale, 
                              _el.Width() * _atlasInfo.scale, 
                              _el.Height() * _atlasInfo.scale );
        bool selected = selectedElements.IndexOf(_el) != -1;

        // ======================================================== 
        // draw the sprite background 
        // ======================================================== 

        GUI.BeginGroup(_atlasRect);
            GUI.color = _el.atlasInfo.spriteBgColor;
                GUI.DrawTexture( rect, exEditorHelper.WhiteTexture() );
            GUI.color = oldBGColor;
        GUI.EndGroup();


        // ======================================================== 
        // draw the texture
        // ======================================================== 

        // process rotate
        Matrix4x4 oldMat = GUI.matrix;
        if ( _el.rotated ) {

            GUIUtility.RotateAroundPivot( 90.0f, new Vector2 ( _atlasRect.x, _atlasRect.y) );
            GUI.matrix = GUI.matrix * Matrix4x4.TRS ( new Vector3( 0.0f, -_atlasRect.width, 0.0f), Quaternion.identity, Vector3.one );

            // NOTE: clipping is done before rotating, if we rotate, we have to change the clip 
            GUI.BeginGroup( _atlasRect );
            srcRect = new Rect( _el.coord[1],
                                _atlasRect.width - _el.coord[0] - _el.trimRect.height,
                                _el.trimRect.width, 
                                _el.trimRect.height );
        }
        else {
            GUI.BeginGroup( _atlasRect );
            srcRect = new Rect( _el.coord[0],
                                _el.coord[1],
                                _el.trimRect.width, 
                                _el.trimRect.height );
        }
        srcRect = new Rect ( srcRect.x * _atlasInfo.scale,
                             srcRect.y * _atlasInfo.scale,
                             srcRect.width * _atlasInfo.scale,
                             srcRect.height * _atlasInfo.scale );

        // draw texture
        if ( _el.trim ) {
            Rect rect2 = new Rect( -_el.trimRect.x * _atlasInfo.scale,
                                   -_el.trimRect.y * _atlasInfo.scale,
                                   _el.texture.width * _atlasInfo.scale, 
                                   _el.texture.height * _atlasInfo.scale );
            GUI.BeginGroup( srcRect );
            GUI.DrawTexture( rect2, _el.texture );
            GUI.EndGroup();
        }
        else {
            GUI.DrawTexture( srcRect, _el.texture );
        }

        // recover from rotate
        if ( _el.rotated )
            GUI.matrix = oldMat;
        GUI.EndGroup();

        // ======================================================== 
        // draw selected border
        // ======================================================== 

        GUI.BeginGroup(_atlasRect);
        if ( selected ) {
            GUI.backgroundColor = _el.atlasInfo.spriteSelectColor;
                GUI.Box ( rect, GUIContent.none, exEditorHelper.RectBorderStyle() );
            GUI.backgroundColor = oldBGColor;
        }
        GUI.EndGroup();

        // ======================================================== 
        // process mouse event 
        Event e = Event.current;
        // ======================================================== 

        GUI.BeginGroup(_atlasRect);
        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            if ( rect.Contains( e.mousePosition ) ) {
                GUIUtility.keyboardControl = -1; // remove any keyboard control
                if ( e.command || e.control ) {
                    ToggleSelected(_el);
                }
                else {
                    inDraggingElementState = true;
                    if ( selected == false ) {
                        if ( e.command == false && e.control == false ) {
                            selectedElements.Clear();
                            AddSelected(_el);
                        }
                    }
                }

                e.Use();
                Repaint();
            }
        }
        GUI.EndGroup();
    }
}
