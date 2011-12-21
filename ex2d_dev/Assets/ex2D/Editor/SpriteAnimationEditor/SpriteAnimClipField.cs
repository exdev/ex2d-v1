// ======================================================================================
// File         : SpriteAnimClipField.cs
// Author       : Wu Jie 
// Last Change  : 07/06/2011 | 12:15:11 PM | Wednesday,July
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

    void SpriteAnimClipField ( Rect _rect, int _topHeight, int _botHeight, int _scalarHeight,  exSpriteAnimClip _animClip ) {

        // ======================================================== 
        // init varaible
        // ======================================================== 

        float boxWidth = _rect.width;
        float boxHeight = _rect.height - _topHeight - _botHeight;

        // NOTE: a protection to prevent while dead loop
        _animClip.editorScale = Mathf.Clamp( _animClip.editorScale, 0.01f, 10.0f );

        // constant
        float widthToShowLabel = 60.0f;
        float minWidth = 10.0f;
        float maxWidth = 80.0f;
        float minUnitSecond = 1.0f/_animClip.sampleRate;

        // variable
        // int[] lodScales = new int[] { 5, 2, 3, 2 };
        List<int> lodScales = new List<int>();
        int tmpSampleRate = (int)_animClip.sampleRate;
        while ( true ) {
            int div = 0;
            if ( tmpSampleRate == 30 ) {
                div = 3;
            }
            else if ( tmpSampleRate % 2 == 0 ) {
                div = 2;
            }
            else if ( tmpSampleRate % 5 == 0 ) {
                div = 5;
            }
            else if ( tmpSampleRate % 3 == 0 ) {
                div = 3;
            }
            else {
                break;
            }
            tmpSampleRate /= div;
            lodScales.Insert(0,div);
        }
        int curIdx = lodScales.Count;
        lodScales.AddRange( new int[] { 
                            5, 2, 3, 2,
                            5, 2, 3, 2,
                            } );

        //
        float unitWidth = 1000.0f; // width for 1 second
        float curUnitSecond = 1.0f;
        float curCellWidth = unitWidth * _animClip.editorScale;

        // get curUnitSecond and curIdx
        if ( curCellWidth < minWidth ) {
            while ( curCellWidth < minWidth ) {
                curUnitSecond = curUnitSecond * lodScales[curIdx];
                curCellWidth = curCellWidth * lodScales[curIdx];

                curIdx += 1;
                if ( curIdx >= lodScales.Count ) {
                    curIdx = lodScales.Count - 1;
                    break;
                }
            }
        }
        else if ( curCellWidth > maxWidth ) {
            while ( (curCellWidth > maxWidth) && 
                    (curUnitSecond > minUnitSecond) ) {
                curIdx -= 1;
                if ( curIdx < 0 ) {
                    curIdx = 0;
                    break;
                }

                curUnitSecond = curUnitSecond / lodScales[curIdx];
                curCellWidth = curCellWidth / lodScales[curIdx];
            }
        }

        // check if prev width is good to show
        if ( curUnitSecond > minUnitSecond ) {
            int prev = curIdx - 1;
            if ( prev < 0 )
                prev = 0;
            float prevCellWidth = curCellWidth / lodScales[prev];
            float prevUnitSecond = curUnitSecond / lodScales[prev];
            if ( prevCellWidth >= minWidth ) {
                curIdx = prev;
                curUnitSecond = prevUnitSecond;
                curCellWidth = prevCellWidth;
            }
        }

        // init total width and cell-count
        totalWidth = _animClip.editorScale * _animClip.length * unitWidth;
        if ( totalWidth > boxWidth/2.0f ) {
            _animClip.editorOffset = Mathf.Clamp( _animClip.editorOffset, boxWidth - totalWidth - boxWidth/2.0f, 0 );
        }
        else {
            _animClip.editorOffset = 0;
        }

        // get lod interval list
        int[] lodIntervalList = new int[lodScales.Count+1];
        lodIntervalList[curIdx] = 1;
        for ( int i = curIdx-1; i >= 0; --i ) {
            lodIntervalList[i] = lodIntervalList[i+1] / lodScales[i];
        }
        for ( int i = curIdx+1; i < lodScales.Count+1; ++i ) {
            lodIntervalList[i] = lodIntervalList[i-1] * lodScales[i-1];
        }

        // get lod width list
        float[] lodWidthList = new float[lodScales.Count+1];
        lodWidthList[curIdx] = curCellWidth;
        for ( int i = curIdx-1; i >= 0; --i ) {
            lodWidthList[i] = lodWidthList[i+1] / lodScales[i];
        }
        for ( int i = curIdx+1; i < lodScales.Count+1; ++i ) {
            lodWidthList[i] = lodWidthList[i-1] * lodScales[i-1];
        }

        // get idx from
        int idxFrom = curIdx;
        for ( int i = 0; i < lodScales.Count+1; ++i ) {
            if ( lodWidthList[i] > maxWidth ) {
                idxFrom = i;
                break;
            }
        }

        // ======================================================== 
        // draw the scalar
        GUI.BeginGroup( _rect );
        // ======================================================== 

        //
        float xStart = 0.0f;
        float yStart = _topHeight;
        // NOTE: +50 here can avoid us clip text so early 
        int iStartFrom = Mathf.CeilToInt( -(_animClip.editorOffset + 50.0f)/curCellWidth );
        int cellCount = Mathf.CeilToInt( (boxWidth - _animClip.editorOffset)/curCellWidth );
        for ( int i = iStartFrom; i < cellCount; ++i ) {
            float x = xStart + _animClip.editorOffset + i * curCellWidth + 1;
            int idx = idxFrom;

            while ( idx >= 0 ) {
                if ( i % lodIntervalList[idx] == 0 ) {
                    float heightRatio = lodWidthList[idx] / maxWidth;

                    // draw scalar
                    if ( heightRatio >= 1.0f ) {
                        exEditorHelper.DrawLine ( new Vector2(x, yStart ), 
                                                new Vector2(x, yStart - _scalarHeight), 
                                                Color.gray, 
                                                1.0f );
                        exEditorHelper.DrawLine ( new Vector2(x, yStart ), 
                                                new Vector2(x+1, yStart - _scalarHeight), 
                                                Color.gray, 
                                                1.0f );
                    }
                    else if ( heightRatio >= 0.5f ) {
                        exEditorHelper.DrawLine ( new Vector2(x, yStart ), 
                                                new Vector2(x, yStart - _scalarHeight * heightRatio ), 
                                                Color.gray, 
                                                1.0f );
                    }
                    else {
                        exEditorHelper.DrawLine ( new Vector2(x, yStart ), 
                                                new Vector2(x, yStart - _scalarHeight * heightRatio ), 
                                                Color.gray, 
                                                1.0f );
                    }

                    // draw lable
                    if ( lodWidthList[idx] >= widthToShowLabel ) {
                        GUI.Label ( new Rect( x + 4.0f, yStart - 22, 50, 20 ), 
                                    exTimeHelper.ToString_Frames(i*curUnitSecond,_animClip.sampleRate) );
                    }

                    //
                    break;
                }
                --idx;
            }
        }

        // ======================================================== 
        // draw background
        // ======================================================== 

        Color old = GUI.color;
        GUI.color = Color.gray;
            GUI.DrawTexture( new Rect ( 0, yStart, boxWidth, boxHeight ), exEditorHelper.WhiteTexture() );
        GUI.color = old;

        // ======================================================== 
        // draw event info view background (before in-box scalar) 
        // ======================================================== 

        int eventViewHeight = 25;
        eventInfoViewRect = new Rect( _animClip.editorOffset, 
                                      yStart, 
                                      totalWidth, 
                                      eventViewHeight );
        old = GUI.color;
        GUI.color = new Color ( 0.65f, 0.65f, 0.65f, 1.0f );
            GUI.DrawTexture( eventInfoViewRect, exEditorHelper.WhiteTexture() );
        GUI.color = old;

        // ======================================================== 
        // draw in-box scalar
        // ======================================================== 

        for ( int i = iStartFrom; i < cellCount; ++i ) {
            float x = _animClip.editorOffset + i * curCellWidth + 1;
            int idx = idxFrom;

            while ( idx >= 0) {
                if ( i % lodIntervalList[idx] == 0 ) {
                    float ratio = lodWidthList[idx] / maxWidth;
                    exEditorHelper.DrawLine ( new Vector2(x, yStart), 
                                            new Vector2(x, yStart + boxHeight), 
                                            new Color( 0.4f, 0.4f, 0.4f, ratio - 0.3f ),
                                            1.0f );
                    break;
                }
                --idx;
            }
        }

        // ======================================================== 
        // draw unused block
        // ======================================================== 

        exEditorHelper.DrawLine ( new Vector2( 0, yStart + eventViewHeight ), 
                                  new Vector2( boxWidth, yStart + eventViewHeight ), 
                                  new Color( 0.8f, 0.8f, 0.8f, 1.0f ),
                                  1.0f );

        Color oldBGColor;
        if ( boxWidth > _animClip.editorOffset + totalWidth ) {
            Rect unusedBlockRect = new Rect ( _animClip.editorOffset + totalWidth + 1,
                                              yStart + 1.0f,
                                              boxWidth - (_animClip.editorOffset + totalWidth + 2),
                                              boxHeight - 2.0f ); 
            exEditorHelper.DrawRect( unusedBlockRect,
                                   new Color( 0.7f, 0.7f, 0.7f, 1.0f ),
                                   new Color(0.8f, 0.8f, 0.8f, 1.0f) );
        }

        // ======================================================== 
        // draw frame info view
        // ======================================================== 

        frameInfoViewRect = new Rect( _animClip.editorOffset, 
                                      yStart + eventViewHeight,
                                      totalWidth, 
                                      boxHeight - eventViewHeight );
        FrameInfoViewField ( frameInfoViewRect, _animClip );

        // ======================================================== 
        // draw border
        // ======================================================== 

        oldBGColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.black;
            GUI.Box ( new Rect( 0, yStart, boxWidth, boxHeight ), 
                      GUIContent.none, 
                      exEditorHelper.RectBorderStyle() );
        GUI.backgroundColor = oldBGColor;

        // ======================================================== 
        // DEBUG { 
        // Color tmpclr = GUI.backgroundColor;
        // GUI.backgroundColor = Color.red;
        //     GUI.Box ( new Rect( 0, 0, _rect.width, _rect.height ), 
        //               GUIContent.none, 
        //               exEditorHelper.RectBorderStyle() );
        // GUI.backgroundColor = tmpclr;
        // } DEBUG end 
        GUI.EndGroup();
        // draw needle
        // ======================================================== 

        //
        GUILayoutUtility.GetRect ( _rect.width, _rect.height + _scalarHeight );

        // DEBUG { 
        // GUILayout.BeginHorizontal();
        // GUILayout.Space(30);
        //     GUILayout.BeginVertical();
        //         GUILayout.Label ( "curIdx: " + curIdx );
        //         GUILayout.Label ( "idxFrom: " + idxFrom );
        //         GUILayout.Label ( "curCellWidth: " + curCellWidth );
        //         GUILayout.Label ( "curUnitSecond: " + curUnitSecond );
        //         GUILayout.Label ( "totalWidth: " + totalWidth );
        //         for ( int i = 0; i < lodScales.Count; ++i ) {
        //             GUILayout.BeginHorizontal( GUILayout.MaxWidth(800) );
        //                 GUILayout.Label ( "lod scales " + i + " = " + lodScales[i] );
        //                 GUILayout.Label ( "lod width " + i + " = " + lodWidthList[i] );
        //                 GUILayout.Label ( "lod interval " + i + " = " + lodIntervalList[i] );
        //             GUILayout.EndHorizontal();
        //         }
        //     GUILayout.EndVertical();
        // GUILayout.EndHorizontal();
        // } DEBUG end 

        // ======================================================== 
        Event e = Event.current;
        // ======================================================== 

        if ( _rect.Contains(e.mousePosition) ) {
            if ( e.type == EventType.ScrollWheel ) {
                float s = 1000.0f;
                while ( (_animClip.editorScale/s) < 1.0f || (_animClip.editorScale/s) > 10.0f ) {
                    s /= 10.0f;
                }
                _animClip.editorScale -= e.delta.y * s * 0.05f;
                _animClip.editorScale = Mathf.Clamp( _animClip.editorScale, 0.001f, 100.0f );
                Repaint();

                e.Use();
            }
            else if ( e.type == EventType.MouseDrag ) {
                if ( e.button == 1 ) {
                    _animClip.editorOffset += e.delta.x;
                    Repaint();

                    e.Use();
                }
            }
            else if ( e.type == EventType.DragUpdated ) {
                // Show a copy icon on the drag
                foreach ( Object o in DragAndDrop.objectReferences ) {
                    if ( o is Texture2D ||
                         exEditorHelper.IsDirectory(o) ) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        break;
                    }
                }
                e.Use();
            }
            else if ( e.type == EventType.DragPerform ) {
                DragAndDrop.AcceptDrag();

                // store old selection state
                List<Object> oldSelObjects = new List<Object>();
                foreach ( Object o in Selection.objects ) {
                    oldSelObjects.Add(o);
                }
                Object oldSelActiveObject = Selection.activeObject;

                //
                try {
                    EditorUtility.DisplayProgressBar( "Adding Textures...",
                                                      "Start adding ",
                                                      0.5f );    
                    // sort
                    List<Object> objList = new List<Object>(DragAndDrop.objectReferences.Length);
                    foreach ( Object o in DragAndDrop.objectReferences ) {
                        if ( exEditorHelper.IsDirectory(o) ) {
                            Selection.activeObject = o;
                            Object[] objs = Selection.GetFiltered( typeof(Texture2D), SelectionMode.DeepAssets);
                            objList.AddRange(objs);
                        }
                        else if ( o is Texture2D ) {
                            objList.Add(o);
                        }
                    }
                    objList.Sort(exEditorHelper.CompareObjectByName);

                    // DELME { 
                    // // sort
                    // Object[] objList = Selection.GetFiltered( typeof(Texture2D), SelectionMode.DeepAssets);
                    // System.Array.Sort( objList, exEditorHelper.CompareObjectByName );
                    // } DELME end 

                    // add objects as frames
                    _animClip.AddFrames( objList.ToArray() );
                    EditorUtility.ClearProgressBar();    
                }
                catch ( System.Exception ) {
                    EditorUtility.ClearProgressBar();    
                    throw;
                }

                //
                CalculatePreviewScale();
                Repaint();

                // recover selections
                Selection.activeObject = oldSelActiveObject;
                Selection.objects = oldSelObjects.ToArray();

                e.Use();
            }
        }
    }
}
