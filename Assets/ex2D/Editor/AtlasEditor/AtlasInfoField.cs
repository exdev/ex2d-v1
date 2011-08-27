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
        foreach ( exAtlasInfo.Element el in curEdit.elements ) {
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

            AtlasElementField ( scaledRect, el );
        }
        foreach ( exAtlasInfo.Element el in invalidElements ) {
            curEdit.RemoveElement(el);
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
                         (o is exBitmapFont && (o as exBitmapFont).useAtlas == false) ) 
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        break;
                    }
                }
            }
            else if ( e.type == EventType.DragPerform ) {
                // NOTE: Unity3D have a problem in ImportTextureForAtlas, when a texture is an active selection, 
                //       no matter how you change your import settings, finally it will apply changes that in Inspector (shows when object selected)
                oldSelActiveObject = null;
                oldSelObjects.Clear();
                foreach ( Object o in Selection.objects ) {
                    oldSelObjects.Add(o);
                }
                oldSelActiveObject = Selection.activeObject;
                Selection.activeObject = null;

                //
                DragAndDrop.AcceptDrag();
                foreach ( Object o in DragAndDrop.objectReferences ) {
                    importObjects.Add(o);
                }

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

    void ImportObjects () {
        EditorUtility.DisplayProgressBar( "Adding Textures...", "Start adding ", 0.2f );
        curEdit.ImportObjects ( importObjects.ToArray() );
        importObjects.Clear();
        EditorUtility.ClearProgressBar();    
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AtlasElementField ( Rect _atlasRect, exAtlasInfo.Element _el ) {
        Color oldBGColor = GUI.backgroundColor;
        Rect srcRect; 
        Rect rect = new Rect( _el.coord[0] * curEdit.scale, 
                              _el.coord[1] * curEdit.scale, 
                              _el.Width() * curEdit.scale, 
                              _el.Height() * curEdit.scale );
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
        srcRect = new Rect ( srcRect.x * curEdit.scale,
                             srcRect.y * curEdit.scale,
                             srcRect.width * curEdit.scale,
                             srcRect.height * curEdit.scale );

        // draw texture
        if ( _el.trim ) {
            Rect rect2 = new Rect( -_el.trimRect.x * curEdit.scale,
                                   -_el.trimRect.y * curEdit.scale,
                                   _el.texture.width * curEdit.scale, 
                                   _el.texture.height * curEdit.scale );
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
