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

        int[] lodScales = new int[] { 5, 2, 3, 2 };
        float boxWidth = _rect.width;
        float boxHeight = _rect.height - _topHeight - _botHeight;

        // NOTE: a protection to prevent while dead loop
        _animClip.editorScale = Mathf.Clamp( _animClip.editorScale, 0.01f, 10.0f );

        // constant
        float widthToShowLabel = 40.0f;
        float minWidth = 10.0f;
        float maxWidth = 80.0f;
        float unitWidth = minWidth;
        float minUnitSecond = 1.0f/_animClip.sampleRate;

        // variable
        int curIdx = 0;
        float curCellWidth = unitWidth * _animClip.editorScale;
        float curUnitSecond = 1.0f/_animClip.sampleRate;

        // get curUnitSecond and curIdx
        if ( curCellWidth < minWidth ) {
            while ( curCellWidth < minWidth ) {
                curUnitSecond = curUnitSecond * lodScales[curIdx];
                curCellWidth = curCellWidth * lodScales[curIdx];
                curIdx = (curIdx + 1) % lodScales.Length;
            }
        }
        else if ( curCellWidth > maxWidth ) {
            while ( (curCellWidth > maxWidth) && (curUnitSecond > minUnitSecond) ) {
                curIdx = curIdx - 1;
                if ( curIdx < 0 )
                    curIdx = lodScales.Length - 1;
                curUnitSecond = curUnitSecond / lodScales[curIdx];
                curCellWidth = curCellWidth / lodScales[curIdx];
            }
        }

        // check if prev width is good to show
        if ( curUnitSecond > minUnitSecond ) {
            int prev = curIdx - 1;
            if ( prev < 0 )
                prev = lodScales.Length - 1;
            float prevCellWidth = curCellWidth / lodScales[prev];
            float prevUnitSecond = curUnitSecond / lodScales[prev];
            if ( prevCellWidth >= minWidth ) {
                curIdx = prev;
                curUnitSecond = prevUnitSecond;
                curCellWidth = prevCellWidth;
            }
        }

        // init total width and cell-count
        totalWidth = unitWidth * _animClip.editorScale * (_animClip.length / minUnitSecond);
        if ( totalWidth > boxWidth/2.0f ) {
            _animClip.editorOffset = Mathf.Clamp( _animClip.editorOffset, boxWidth - totalWidth - boxWidth/2.0f, 0 );
        }
        else {
            _animClip.editorOffset = 0;
        }

        // init lod index
        int idxFrom = 3;
        float[] lodWidthList = new float[5];
        int[] lodIntervalList = new int[5];
        lodWidthList[0] = curCellWidth;
        lodIntervalList[0] = 1;
        for ( int i = 1; i < lodScales.Length+1; ++i ) {
            lodWidthList[i] = lodWidthList[i-1] * lodScales[(curIdx+i-1)%lodScales.Length];
            lodIntervalList[i] = lodIntervalList[i-1] * lodScales[(curIdx+i-1)%lodScales.Length];
        }
        for ( int i = 0; i < lodScales.Length+1; ++i ) {
            if ( lodWidthList[i] >= maxWidth ) {
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

            while ( idx >= 0) {
                if ( i % lodIntervalList[idx] == 0 ) {
                    float heightRatio = lodWidthList[idx] / maxWidth;

                    // draw scalar
                    if ( heightRatio >= 1.0f ) {
                        exEditorHelper.DrawLine ( new Vector2(x, yStart ), 
                                                new Vector2(x, yStart - _scalarHeight), 
                                                Color.black, 
                                                2.0f );
                    }
                    else if ( heightRatio >= 0.5f ) {
                        exEditorHelper.DrawLine ( new Vector2(x, yStart ), 
                                                new Vector2(x, yStart - _scalarHeight * heightRatio ), 
                                                Color.black, 
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
                                    exTimeHelper.ToString_Seconds(i*curUnitSecond) );
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
        // GUILayout.Label ( "editorScale: " + _animClip.editorScale );
        // GUILayout.Label ( "curCellWidth: " + curCellWidth );
        // GUILayout.Label ( "curUnitSecond: " + curUnitSecond );
        // GUILayout.Label ( "totalWidth: " + totalWidth );
        // // for ( int i = 0; i < 5; ++i ) {
        // //     GUILayout.Label ( "lod width " + i + " = " + lodWidthList[i] );
        // //     GUILayout.Label ( "lod interval " + i + " = " + lodIntervalList[i] );
        // // }
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
