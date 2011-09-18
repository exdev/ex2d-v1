// ======================================================================================
// File         : TileMapField.cs
// Author       : Wu Jie 
// Last Change  : 09/16/2011 | 18:08:04 PM | Friday,September
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

    private Vector2 mapScrollPos = Vector2.zero;
    private int curGridX = -1;
    private int curGridY = -1;
    private int margin = 30;

    // private bool mapInRectSelectState = false;
    // private Rect mapSelectRect = new Rect( 0, 0, 1, 1 );
    // private List<int> mapSelectedGrids = new List<int>();
    // private List<int> mapCommitGrids = new List<int>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void TileMapField ( Rect _rect, exTileMap _tileMap ) {

        //
        if ( _tileMap == null )
            return;

        Color eraseColor = new Color( 1.0f, 0.0f, 0.0f, 0.5f );
        float width = _tileMap.col * _tileMap.tileWidth;
        float height = _tileMap.row * _tileMap.tileHeight;

        // step 1: find the center of selection
        int minX = 9999; 
        int minY = 9999;
        int maxX = -1; 
        int maxY = -1;
        foreach ( int idx in sheetSelectedGrids ) {
            int id_x, id_y;
            _tileMap.tileSheet.GetColRow( idx, out id_x, out id_y );

            // check min max x
            if ( id_x < minX )
                minX = id_x; 
            if ( id_x > maxX )
                maxX = id_x;

            // check min max y
            if ( id_y < minY )
                minY = id_y;
            if ( id_y > maxY )
                maxY = id_y;
        }
        int centerX = Mathf.CeilToInt((maxX-minX)/2.0f) + minX; 
        int centerY = Mathf.CeilToInt((maxY-minY)/2.0f) + minY;

        Event e = Event.current;
        mapScrollPos = GUI.BeginScrollView( _rect, mapScrollPos, new Rect( -margin, -margin, width + margin * 2, height + margin * 2 )  );

            if ( e.type == EventType.Repaint ) {

                // ======================================================== 
                // draw tile 
                // ======================================================== 

                debugVisibleGrids = 0;
                Color textureColor = Color.white;
                Color oldColor = GUI.color;
                Rect viewPort = new Rect ( mapScrollPos.x-margin, mapScrollPos.y-margin, _rect.width, _rect.height );
                for ( int r = 0; r < _tileMap.row; ++r ) {
                    for ( int c = 0; c < _tileMap.col; ++c ) {
                        int curX = c * _tileMap.tileWidth;
                        int curY = r * _tileMap.tileHeight;
                        int sheetID = _tileMap.grids[r*_tileMap.col+c];

                        if ( sheetID == -1 )
                            continue;

                        Rect gridRect = new Rect( curX - _tileMap.tileOffsetX,
                                                  curY - (_tileMap.tileSheet.tileHeight - _tileMap.tileHeight) + _tileMap.tileOffsetY,
                                                  _tileMap.tileSheet.tileWidth, 
                                                  _tileMap.tileSheet.tileHeight );

                        // check if we render this rect
                        if ( exContains2D.RectRect ( viewPort, gridRect ) == 0 &&
                             exIntersection2D.RectRect ( viewPort, gridRect ) == false )
                            continue;

                        //
                        if ( _tileMap.editorEditMode == exTileMap.EditMode.Erase &&
                             curX == curGridX &&
                             curY == curGridY )
                        {
                            textureColor = eraseColor;
                        }
                        else {
                            textureColor = _tileMap.editorColor;
                        }
                        GUI.color = textureColor; 

                        //
                        Rect uv = _tileMap.tileSheet.GetTileUV (sheetID);

                        // DISABLE { 
                        // GUI.BeginGroup( gridRect );
                        //     GUI.DrawTexture( new Rect( -uv.x * _tileMap.tileSheet.texture.width, 
                        //                                -(1.0f - uv.y) * _tileMap.tileSheet.texture.height + _tileMap.tileSheet.tileHeight, 
                        //                                _tileMap.tileSheet.texture.width, 
                        //                                _tileMap.tileSheet.texture.height), 
                        //                      _tileMap.tileSheet.texture );
                        // GUI.EndGroup();
                        // } DISABLE end 

                        // DISABLE: NOTE: this is pro only { 
                        textureColor /= 2.0f;
                        Graphics.DrawTexture ( gridRect, 
                                               _tileMap.tileSheet.texture,  
                                               uv,
                                               0, 0, 0, 0,
                                               textureColor );
                        // } DISABLE end 
                        ++debugVisibleGrids;
                    }
                }
                GUI.color = oldColor;

                // ======================================================== 
                // draw mouse 
                // ======================================================== 

                if ( curGridX >= 0 && curGridX < _tileMap.col * _tileMap.tileWidth &&
                     curGridY >= 0 && curGridY < _tileMap.row * _tileMap.tileHeight )
                {
                    // draw selected grids 
                    if ( _tileMap.editorEditMode == exTileMap.EditMode.Paint ) {
                        Rect uv = new Rect( 0, 0, 1, 1 );

                        if ( _tileMap.tileSheet != null ) {

                            // step 2: draw mouse grid
                            foreach ( int idx in sheetSelectedGrids ) {
                                int id_x, id_y;
                                _tileMap.tileSheet.GetColRow( idx, out id_x, out id_y );

                                int deltaX = id_x - centerX;
                                int deltaY = id_y - centerY;
                                int curX = curGridX + deltaX * _tileMap.tileWidth;
                                int curY = curGridY + deltaY * _tileMap.tileHeight;

                                if ( curX < 0 || curX >= _tileMap.col * _tileMap.tileWidth ||
                                     curY < 0 || curY >= _tileMap.row * _tileMap.tileHeight )
                                    continue;

                                exEditorHelper.DrawRect( new Rect( curX,
                                                                   curY,
                                                                   _tileMap.tileWidth, 
                                                                   _tileMap.tileHeight ),
                                                         new Color( 1.0f, 1.0f, 1.0f, 0.2f ),
                                                         new Color( 0.0f, 0.0f, 0.0f, 0.0f )  );
                            }

                            // step 3: draw mouse texture
                            oldColor = GUI.color;
                            GUI.color = new Color( 1.0f, 1.0f, 1.0f, 0.6f );
                            foreach ( int idx in sheetSelectedGrids ) {
                                int id_x, id_y;
                                _tileMap.tileSheet.GetColRow( idx, out id_x, out id_y );

                                int deltaX = id_x - centerX;
                                int deltaY = id_y - centerY;
                                int curX = curGridX + deltaX * _tileMap.tileWidth;
                                int curY = curGridY + deltaY * _tileMap.tileHeight;

                                if ( curX < 0 || curX >= _tileMap.col * _tileMap.tileWidth ||
                                     curY < 0 || curY >= _tileMap.row * _tileMap.tileHeight )
                                    continue;

                                uv = _tileMap.tileSheet.GetTileUV (idx);
                                Rect gridRect = new Rect( curX - _tileMap.tileOffsetX,
                                                          curY - (_tileMap.tileSheet.tileHeight - _tileMap.tileHeight) + _tileMap.tileOffsetY,
                                                          _tileMap.tileSheet.tileWidth, 
                                                          _tileMap.tileSheet.tileHeight );

                                GUI.BeginGroup( gridRect );
                                    GUI.DrawTexture( new Rect( -uv.x * _tileMap.tileSheet.texture.width, 
                                                               -(1.0f - uv.y) * _tileMap.tileSheet.texture.height + _tileMap.tileSheet.tileHeight, 
                                                               _tileMap.tileSheet.texture.width, 
                                                               _tileMap.tileSheet.texture.height), 
                                                     _tileMap.tileSheet.texture );
                                GUI.EndGroup();
                            }
                            GUI.color = oldColor;
                        }
                        else {
                            exEditorHelper.DrawRect( new Rect( curGridX, 
                                                               curGridY, 
                                                               _tileMap.tileWidth, 
                                                               _tileMap.tileHeight ),
                                                     new Color( 1.0f, 1.0f, 1.0f, 0.2f ),
                                                     new Color( 0.0f, 0.0f, 0.0f, 0.0f )  );
                        }
                    }
                    else {
                        exEditorHelper.DrawRect( new Rect( curGridX,
                                                           curGridY,
                                                           _tileMap.tileWidth, 
                                                           _tileMap.tileHeight ),
                                                 new Color( 1.0f, 1.0f, 1.0f, 0.2f ),
                                                 new Color( 0.0f, 0.0f, 0.0f, 0.0f )  );
                    }
                }

                // ======================================================== 
                // draw grid lines
                // ======================================================== 

                if ( _tileMap.editorShowGrid ) {
                    for ( int i = 0; i <= _tileMap.col; ++i ) {
                        float x = i * _tileMap.tileWidth;
                        exEditorHelper.DrawLine ( new Vector2( x, 0.0f ), 
                                                  new Vector2( x, height ),
                                                  new Color( 0.5f, 0.5f, 0.5f, 1.0f ),
                                                  1.0f );
                    }
                    for ( int i = 0; i <= _tileMap.row; ++i ) {
                        float y = i * _tileMap.tileHeight;
                        exEditorHelper.DrawLine ( new Vector2( 0.0f,  y ), 
                                                  new Vector2( width, y ),
                                                  new Color( 0.5f, 0.5f, 0.5f, 1.0f ),
                                                  1.0f );
                    }
                }
            }

            // ======================================================== 
            // handle mouse event 
            // ======================================================== 

            if ( new Rect( mapScrollPos.x - margin, 
                           mapScrollPos.y - margin, 
                           _rect.width, 
                           _rect.height ).Contains(e.mousePosition) ) 
            {
                //
                if ( e.type == EventType.MouseMove || e.type == EventType.MouseDrag ) {
                    int grid_x; int grid_y;
                    SnapToGrid ( _tileMap, e.mousePosition, out grid_x, out grid_y );
                    if ( curGridX != grid_x || curGridY != grid_y ) {
                        curGridX = grid_x;
                        curGridY = grid_y;
                        Repaint();

                        e.Use();
                    }
                }

                //
                if ( e.type == EventType.MouseDrag || 
                     (e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1) ) 
                {
                    if ( _tileMap.editorEditMode == exTileMap.EditMode.Erase ) {
                        _tileMap.SetTile( curGridX/_tileMap.tileWidth,
                                          curGridY/_tileMap.tileHeight,
                                          -1 );
                    }
                    else {
                        foreach ( int idx in sheetSelectedGrids ) {
                            int id_x, id_y;
                            _tileMap.tileSheet.GetColRow( idx, out id_x, out id_y );

                            int deltaX = id_x - centerX;
                            int deltaY = id_y - centerY;
                            int curX = curGridX + deltaX * _tileMap.tileWidth;
                            int curY = curGridY + deltaY * _tileMap.tileHeight;

                            if ( curX < 0 || curX >= _tileMap.col * _tileMap.tileWidth ||
                                 curY < 0 || curY >= _tileMap.row * _tileMap.tileHeight )
                                continue;

                            _tileMap.SetTile( curX/_tileMap.tileWidth,
                                              curY/_tileMap.tileHeight,
                                              idx );

                            EditorUtility.SetDirty(_tileMap);
                        }
                    }

                    e.Use();
                }

                if ( e.isMouse )
                    e.Use();
            }
        GUI.EndScrollView();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void SnapToGrid ( exTileMap _tileMap, Vector2 _pos, out int _x, out int _y ) {
        _x = Mathf.FloorToInt(_pos.x / _tileMap.tileWidth) * _tileMap.tileWidth;
        _y = Mathf.FloorToInt(_pos.y / _tileMap.tileHeight) * _tileMap.tileHeight;
    }
}
