// ======================================================================================
// File         : FrameInfoField.cs
// Author       : Wu Jie 
// Last Change  : 07/06/2011 | 14:27:42 PM | Wednesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

partial class exSpriteAnimClipEditor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void FrameInfoViewField ( Rect _rect, exSpriteAnimClip _animClip ) {
        float curX = 0.0f;
        int oldDepth = GUI.depth;
        GUI.BeginGroup(_rect);

            // ======================================================== 
            // draw none selected object first
            // ======================================================== 

            List<exSpriteAnimClip.FrameInfo> invalidFrames = new List<exSpriteAnimClip.FrameInfo>();
            foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
                float width = (fi.length / _animClip.length) * totalWidth;
                Texture2D tex2D = exEditorRuntimeHelper.LoadAssetFromGUID<Texture2D>(fi.textureGUID);
                if ( tex2D == null ) {
                    invalidFrames.Add(fi);
                    continue;
                }
                FrameInfoField ( new Rect( curX, yFrameInfoOffset, width, _rect.height - 2 * yFrameInfoOffset ), 
                                 fi );
                curX += width;
            }
            foreach ( exSpriteAnimClip.FrameInfo fi in invalidFrames ) {
                exSpriteAnimationUtility.RemoveFrame( curEdit, fi );
                selectedFrameInfos.Remove(fi); // unselect it if we have
            }

            // ======================================================== 
            // draw resize field
            // ======================================================== 

            playingSelects = false;
            if ( selectedFrameInfos.Count > 0 )
                ResizeField (_animClip);

            // ======================================================== 
            // process mouse event 
            Event e = Event.current;
            curX = 0.0f;
            // ======================================================== 

            foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
                bool selected = selectedFrameInfos.IndexOf(fi) != -1;
                float width = (fi.length / _animClip.length) * totalWidth;
                Rect rect = new Rect( curX, yFrameInfoOffset, width, _rect.height - 2 * yFrameInfoOffset );

                if ( e.type == EventType.MouseDown && e.button == 0 ) {
                    if ( rect.Contains( e.mousePosition ) ) {
                        curSeconds = (curX+0.1f)/totalWidth * _animClip.length;
                        GUIUtility.keyboardControl = -1; // remove any keyboard control
                        selectedEventInfos.Clear(); // unselect all events

                        if ( e.command || e.control ) {
                            ToggleSelected(fi);
                        }
                        else {
                            inDraggingFrameInfoState = true;
                            if ( selected == false ) {
                                if ( e.command == false && e.control == false ) {
                                    selectedFrameInfos.Clear();
                                    AddSelected(fi);
                                }
                            }
                        }

                        e.Use();
                        Repaint();
                    }
                }
                curX += width;
            }
        GUI.EndGroup();
        GUI.depth = oldDepth;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void FrameInfoField ( Rect _rect, exSpriteAnimClip.FrameInfo _fi ) {
        bool selected = selectedFrameInfos.IndexOf(_fi) != -1;
        exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo (_fi.textureGUID);

        // ======================================================== 
        // draw background
        // ======================================================== 

        Color old = GUI.color;
        if ( selected ) {
            GUI.color = new Color( 0.2f, 0.85f, 0.0f, 0.2f ); 
        }
        else if ( elInfo == null ) {
            GUI.color = new Color( 1.0f, 0.0f, 0.0f, 0.2f );
        }
        else {
            GUI.color = new Color( 1.0f, 0.0f, 0.85f, 0.2f );
        }
        GUI.DrawTexture( _rect, exEditorHelper.WhiteTexture() );
        GUI.color = old;

        // ======================================================== 
        // draw texture
        // ======================================================== 

        if ( elInfo != null ) {
            exAtlasInfo atlasInfo = exEditorRuntimeHelper.LoadAssetFromGUID<exAtlasInfo>(elInfo.guidAtlasInfo);
            exAtlasInfo.Element el = atlasInfo.elements[elInfo.indexInAtlasInfo];  

            if ( el.texture != null ) {
                float width = el.texture.width;
                float height = el.texture.height;

                // get the scale
                float scale = 1.0f;
                if ( width > _rect.width && height > _rect.height ) {
                    scale = Mathf.Min( _rect.width / width, 
                                       _rect.height / height );
                }
                else if ( width > _rect.width ) {
                    scale = _rect.width / width;
                }
                else if ( height > _rect.height ) {
                    scale = _rect.height / height;
                }

                // draw
                Rect size = new Rect( -el.trimRect.x * scale, 
                                      -el.trimRect.y * scale, 
                                      width * scale, 
                                      height * scale );
                GUI.BeginGroup( _rect );
                    GUI.BeginGroup( new Rect( (_rect.width - el.trimRect.width * scale) * 0.5f,
                                              (_rect.height - el.trimRect.height * scale) * 0.5f,
                                              el.trimRect.width * scale, 
                                              el.trimRect.height * scale ) );
                        GUI.DrawTexture( size, el.texture );
                    GUI.EndGroup();
                GUI.EndGroup();
            }
        }
        else {
            Texture2D tex2D = exEditorRuntimeHelper.LoadAssetFromGUID<Texture2D>(_fi.textureGUID);
            if ( tex2D != null ) {
                float width = tex2D.width;
                float height = tex2D.height;

                // get the scale
                float scale = 1.0f;
                if ( width > _rect.width && height > _rect.height ) {
                    scale = Mathf.Min( _rect.width / width, 
                                       _rect.height / height );
                }
                else if ( width > _rect.width ) {
                    scale = _rect.width / width;
                }
                else if ( height > _rect.height ) {
                    scale = _rect.height / height;
                }

                //
                Rect size = new Rect( 0.0f, 0.0f, width * scale, height * scale );
                Rect rect2 = new Rect ( (_rect.width - size.width) * 0.5f,
                                        (_rect.height - size.height) * 0.5f,
                                        size.width, 
                                        size.height );

                //
                GUI.BeginGroup( _rect );
                    GUI.BeginGroup( rect2 );
                        GUI.DrawTexture( size, tex2D );
                    GUI.EndGroup();
                GUI.EndGroup();
            }
        }

        // ======================================================== 
        // draw border
        // ======================================================== 

        Color oldBGColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.black;
            GUI.Box ( _rect, GUIContent.none, exEditorHelper.RectBorderStyle() );
        GUI.backgroundColor = oldBGColor;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ResizeField ( exSpriteAnimClip _animClip ) {
        float frameInfoHeight = frameInfoViewRect.height - 2.0f * yFrameInfoOffset;

        // add indices
        List<int> indices = new List<int>();
        foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
            int idx = _animClip.frameInfos.IndexOf(fi);
            indices.Add(idx);
        }
        indices.Sort();

        // check if resizable
        int lastIdx = -1;
        bool canResize = true;
        foreach ( int idx in indices ) {
            if ( lastIdx != -1 && idx - lastIdx != 1 ) {
                canResize = false;
                break;
            }
            lastIdx = idx;
        }

        //
        if ( canResize ) {
            float borderWidth = 0.0f;
            float curX = 0.0f;

            for ( int i = 0; i < indices[0]; ++i ) {
                exSpriteAnimClip.FrameInfo fi = _animClip.frameInfos[i];
                float width = (fi.length / _animClip.length) * totalWidth;
                curX += width;
            } 

            foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
                float width = (fi.length / _animClip.length) * totalWidth;
                borderWidth += width;
            }

            float xStart = curX;
            float yStart = yFrameInfoOffset;
            Color oldBGColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            Rect rect = new Rect ( xStart, yStart, borderWidth, frameInfoHeight );
                GUI.Box ( rect, GUIContent.none, exEditorHelper.RectBorderStyle() );
            GUI.backgroundColor = oldBGColor;

            //
            if ( selectedFrameInfos.Count > 1 ) {
                playingSelects = true;
                playingStart = (xStart+0.1f)/totalWidth * _animClip.length;
                playingEnd = (xStart + borderWidth)/totalWidth * _animClip.length;
            }

            // draw resize handle
            ResizeHandleField ( new Rect( xStart + borderWidth - 5.0f, 
                                          yStart,
                                          10.0f, 
                                          frameInfoHeight ) );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ResizeHandleField ( Rect _rect ) {
        GUI.EndGroup ();
        if ( inResizeFrameInfoState ) {
            float curSeconds = 0.0f;
            exSpriteAnimClip.FrameInfo lastFrame = curEdit.frameInfos[curEdit.frameInfos.Count-1];
            foreach ( exSpriteAnimClip.FrameInfo fi in curEdit.frameInfos ) {
                curSeconds += fi.length;
                if ( fi == lastFrame )
                    break;
            }
            float lineAt = _rect.x + _rect.width/2 + curEdit.editorOffset;
            float yStart = eventInfoViewRect.y;
            float height = eventInfoViewRect.height + frameInfoViewRect.height; 
            exEditorHelper.DrawLine ( new Vector2(lineAt, yStart ), 
                                    new Vector2(lineAt, yStart + height ), 
                                    Color.yellow, 
                                    1.0f );
            GUI.Label ( new Rect( lineAt-15.0f, yStart + height, 30.0f, 20.0f ),
                        exTimeHelper.ToString_Seconds(curSeconds) );
        }

        Rect rect = new Rect ( _rect.x + curEdit.editorOffset, _rect.y + frameInfoViewRect.y, _rect.width, _rect.height );
        exEditorHelper.DrawRect( rect, 
                               new Color( 0.0f, 0.5f, 1.0f, 1.0f ),
                               new Color ( 1.0f, 1.0f, 1.0f, 1.0f ) );

        // ======================================================== 
        Event e = Event.current;
        // ======================================================== 

        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            if ( rect.Contains ( e.mousePosition ) ) {
                GUIUtility.keyboardControl = -1; // remove any keyboard control
                inResizeFrameInfoState = true;
                Repaint();

                e.Use();
            }
        }
        GUI.BeginGroup (frameInfoViewRect);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void InsertField ( Rect _rect, exSpriteAnimClip _animClip ) {
        if ( insertAt != -1 ) {
            Rect rect = _rect;
            if ( insertAt == _animClip.frameInfos.Count ) {
                rect.x = rect.x + totalWidth - rect.width/2.0f;
            }
            else {
                float curX = 0.0f;
                for ( int i = 0; i < insertAt; ++i ) {
                    exSpriteAnimClip.FrameInfo fi = _animClip.frameInfos[i];
                    float width = (fi.length / _animClip.length) * totalWidth;
                    curX += width;
                }
                rect.x = rect.x + curX - rect.width/2.0f;
            }
            
            //
            Color old = GUI.color;
            GUI.color = new Color( 1.0f, 1.0f, 0.0f, 1.0f );
            GUI.DrawTexture( rect, exEditorHelper.WhiteTexture() );
            GUI.color = old;

            Color oldBGColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color ( 0.0f, 0.0f, 0.0f, 1.0f );
            GUI.Box ( rect, GUIContent.none, exEditorHelper.RectBorderStyle() );
            GUI.backgroundColor = oldBGColor;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void FrameInfoEditField () {

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        GUILayout.Label( "FrameInfo Inspector", style );

        float newLength = 0.0f;
        bool needUpdate = false;
        bool hasSelects = selectedFrameInfos.Count != 0;

        // ======================================================== 
        // each frame length
        // ======================================================== 

        GUI.enabled = hasSelects; 
        float length = hasSelects ? selectedFrameInfos[0].length : -1.0f;
        newLength = EditorGUILayout.FloatField("Each Frame Length", length );
        if ( newLength != length ) {
            foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
                fi.length = newLength;
            }
            needUpdate = true;
        }

        // ======================================================== 
        // total frame length
        // ======================================================== 

        GUI.enabled = hasSelects; 
        float total = 0.0f;
        foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
            total += fi.length;
        }
        newLength = EditorGUILayout.FloatField("Total Frame Length", total );
        if ( newLength != total ) {
            float delta = newLength - total;
            foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos) {
                float ratio = fi.length/total;
                fi.length = Mathf.Max(1.0f/60.0f, fi.length + delta * ratio);
            }
            needUpdate = true;
        }

        // ======================================================== 
        // Select 
        // ======================================================== 

        if ( GUILayout.Button("Select In Project...", GUILayout.MaxWidth(150) ) ) {
            List<Object> selects = new List<Object>(selectedFrameInfos.Count);
            foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
                Texture2D texture 
                    = exEditorRuntimeHelper.LoadAssetFromGUID<Texture2D>(fi.textureGUID ); 
                selects.Add(texture);
            }

            if ( selects.Count != 0 ) {
                selectIdx = (selectIdx + 1) % selects.Count;  
                Selection.objects = selects.ToArray();
                EditorGUIUtility.PingObject(Selection.objects[selectIdx]);
            }
        }
        GUI.enabled = true; 
        GUILayout.Space(10);

        // ======================================================== 
        // frame info each 
        // ======================================================== 

        for ( int i = 0; i < curEdit.frameInfos.Count; ++i ) {
            exSpriteAnimClip.FrameInfo fi = curEdit.frameInfos[i];

            GUILayout.BeginHorizontal();
                GUI.enabled = false; 
                Texture2D tex = exEditorRuntimeHelper.LoadAssetFromGUID<Texture2D>(fi.textureGUID);
                EditorGUILayout.ObjectField( "Frame["+i+"]"
                                             , tex
                                             , typeof(Object)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                             , false 
#endif
                                           );
                GUI.enabled = true; 

                GUI.enabled = selectedFrameInfos.IndexOf(fi) != -1;
                newLength = EditorGUILayout.FloatField("Length", fi.length );
                if ( newLength != fi.length ) {
                    fi.length = newLength;
                    needUpdate = true;
                }
                GUI.enabled = true; 
            GUILayout.EndHorizontal();
        }

        //
        if ( needUpdate ) {
            exSpriteAnimationUtility.Update(curEdit);
            curEdit.editorNeedRebuild = true;
            EditorUtility.SetDirty(curEdit);
        }
    }
}

