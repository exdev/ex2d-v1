// ======================================================================================
// File         : TileSheetField.cs
// Author       : Wu Jie 
// Last Change  : 09/16/2011 | 15:42:43 PM | Friday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
//
// exTileMapEditor
//
///////////////////////////////////////////////////////////////////////////////

partial class exTileMapEditor : EditorWindow {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private bool sheetInRectSelectState = false;
    private Rect sheetSelectRect = new Rect( 0, 0, 1, 1 );
    private List<int> sheetSelectedGrids = new List<int>();
    private List<int> sheetCommitGrids = new List<int>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void TileSheetField ( Rect _rect, exTileSheet _tileSheet ) {
        if ( _tileSheet == null )
            return;

        int col = 0;
        int row = 0;
        int uvX = _tileSheet.padding;
        int uvY = _tileSheet.padding;

        // count the col
        while ( (uvX + _tileSheet.tileWidth + _tileSheet.padding) <= _tileSheet.texture.width ) {
            uvX = uvX + _tileSheet.tileWidth + _tileSheet.padding; 
            ++col;
        }

        // count the row
        while ( (uvY + _tileSheet.tileHeight + _tileSheet.padding) <= _tileSheet.texture.height ) {
            uvY = uvY + _tileSheet.tileHeight + _tileSheet.padding; 
            ++row;
        }

        // show texture by grids
        float curX = 0.0f;
        float curY = 0.0f;
        float interval = 2.0f;
        int borderSize = 1;
        uvX = _tileSheet.padding;
        uvY = _tileSheet.padding;
        int x = 0;
        int y = 0;

        //
        Event e = Event.current;
        sheetCommitGrids.Clear();

        // ======================================================== 
        // draw field 
        // ======================================================== 

        // DISABLE { 
        // Rect filedRect = new Rect( 15, 
        //                            lastRect.yMax,
        //                            (_tileSheet.tileWidth + interval + 2 * borderSize) * col - interval,
        //                            (_tileSheet.tileHeight + interval + 2 * borderSize) * row - interval );
        // } DISABLE end 
        float tileSheetHeight = (_tileSheet.tileHeight + interval + 2 * borderSize) * row - interval;
        Rect filedRect = new Rect( _rect.x,
                                   _rect.y,
                                   _rect.width,
                                   (tileSheetHeight < _rect.height) ? tileSheetHeight : _rect.height );

        GUI.BeginGroup(filedRect);
            while ( (uvY + _tileSheet.tileHeight + _tileSheet.padding) <= _tileSheet.texture.height ) {
                while ( (uvX + _tileSheet.tileWidth + _tileSheet.padding) <= _tileSheet.texture.width ) {

                    // ======================================================== 
                    // draw tile element 
                    // ======================================================== 

                    Rect rect = new Rect( curX, 
                                          curY, 
                                          _tileSheet.tileWidth + 2 * borderSize, 
                                          _tileSheet.tileHeight + 2 * borderSize );

                    // draw the texture
                    GUI.BeginGroup( new Rect ( rect.x + 1,
                                               rect.y + 1,
                                               rect.width - 2,
                                               rect.height - 2 ) );
                        Rect cellRect = new Rect( -uvX,
                                                  -uvY,
                                                  _tileSheet.texture.width, 
                                                  _tileSheet.texture.height );
                        GUI.DrawTexture( cellRect, _tileSheet.texture );
                    GUI.EndGroup();

                    uvX = uvX + _tileSheet.tileWidth + _tileSheet.padding; 
                    curX = curX + _tileSheet.tileWidth + interval + 2 * borderSize; 

                    // ======================================================== 
                    // handle events
                    // ======================================================== 

                    //
                    if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
                        if ( rect.Contains( e.mousePosition ) ) {
                            GUIUtility.keyboardControl = -1; // remove any keyboard control

                            sheetInRectSelectState = true;
                            mouseDownPos = e.mousePosition;
                            UpdateSelectRect ();

                            if ( e.command == false && e.control == false ) {
                                sheetSelectedGrids.Clear();
                            }

                            e.Use();
                            Repaint();
                        }
                    }

                    //
                    bool selectInRect = false;
                    if ( sheetInRectSelectState ) {
                        if ( exContains2D.RectRect( sheetSelectRect, rect ) != 0 ||
                             exIntersection2D.RectRect( sheetSelectRect, rect ) )
                        {
                            AddRectSelected( _tileSheet, x, y );
                            selectInRect = true;
                        }
                    }

                    // ======================================================== 
                    // draw grid, NOTE: we should handle event first to get if the rect is slected 
                    // ======================================================== 

                    if ( HasSelected ( _tileSheet, x, y ) || selectInRect ) {
                        exEditorHelper.DrawRect ( rect,
                                                  new Color ( 0.0f, 0.5f, 1.0f, 0.4f ),
                                                  new Color ( 0.0f, 0.5f, 1.0f, 1.0f ) );
                    }
                    else {
                        exEditorHelper.DrawRect ( rect,
                                                  new Color ( 1.0f, 1.0f, 1.0f, 0.0f ),
                                                  Color.gray );
                    }

                    ++x;
                }

                // step uv
                uvX = _tileSheet.padding;
                uvY = uvY + _tileSheet.tileHeight + _tileSheet.padding; 

                // step pos
                curX = 0.0f;
                curY = curY + _tileSheet.tileHeight + interval + 2 * borderSize; 

                x = 0;
                ++y;
            }

            // DISABLE { 
            // // ======================================================== 
            // // draw select rect 
            // // ======================================================== 

            // if ( sheetInRectSelectState && (sheetSelectRect.width != 0.0f || sheetSelectRect.height != 0.0f) ) {
            //     exEditorHelper.DrawRect( sheetSelectRect, new Color( 0.0f, 0.5f, 1.0f, 0.2f ), new Color( 0.0f, 0.5f, 1.0f, 1.0f ) );
            // }
            // } DISABLE end 

            // ======================================================== 
            // handle rect select
            // ======================================================== 

            if ( sheetInRectSelectState ) {
                if ( e.type == EventType.MouseDrag ) {
                    UpdateSelectRect ();
                    Repaint();

                    e.Use();
                }
            }

        GUI.EndGroup();
        GUILayoutUtility.GetRect ( filedRect.width, filedRect.height );

        // ======================================================== 
        // finally 
        // ======================================================== 

        if ( sheetInRectSelectState ) {
            if ( e.type == EventType.MouseUp && e.button == 0 ) {
                sheetInRectSelectState = false;
                ConfirmRectSelection();
                Repaint();

                e.Use();
            }
        }

        // DISABLE: we use eraser button { 

        // if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
        //     if ( filedRect.Contains( e.mousePosition ) == false ) {
        //         sheetSelectedGrids.Clear();
        //         Repaint();

        //         e.Use();
        //     }
        // }
        // } DISABLE end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    bool HasSelected ( exTileSheet _tileSheet, int _x, int _y ) {
        int in_id = _x + _y * _tileSheet.col;
        foreach ( int id in sheetSelectedGrids ) {
            if ( id == in_id ) {
                return true;
            }
        }

        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AddRectSelected ( exTileSheet _tileSheet, int _x, int _y ) {
        int in_id = _x + _y * _tileSheet.col;

        bool found = false;
        foreach ( int id in sheetCommitGrids ) {
            if ( id == in_id ) {
                found = true;
                break;
            }
        }

        //
        if ( found == false ) {
            sheetCommitGrids.Add(in_id);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateSelectRect () {
        float x = 0;
        float y = 0;
        float width = 0;
        float height = 0;
        Vector2 curMousePos = Event.current.mousePosition;

        if ( mouseDownPos.x < curMousePos.x ) {
            x = mouseDownPos.x;
            width = curMousePos.x - mouseDownPos.x;
        }
        else {
            x = curMousePos.x;
            width = mouseDownPos.x - curMousePos.x;
        }
        if ( mouseDownPos.y < curMousePos.y ) {
            y = mouseDownPos.y;
            height = curMousePos.y - mouseDownPos.y;
        }
        else {
            y = curMousePos.y;
            height = mouseDownPos.y - curMousePos.y;
        }

        sheetSelectRect = new Rect( x, y, width, height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ConfirmRectSelection () {
        foreach ( int id in sheetCommitGrids ) {
            if ( sheetSelectedGrids.IndexOf(id) == -1 )
                sheetSelectedGrids.Add(id);
        }
        sheetCommitGrids.Clear();

        // TODO { 
        // if ( mouseTile != null ) {
        //     exTileMapRenderer tm = mouseTile.AddComponent<exTileMapRenderer>();
        //     if ( sheetSelectedGrids.Count > 0 )
        //         tm.SetTile ( 0, 0, sheetSelectedGrids[0] );
        // }
        // } TODO end 
    }
}
