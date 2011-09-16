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

        float width = _tileMap.col * _tileMap.tileWidth;
        float height = _tileMap.row * _tileMap.tileHeight;
        Event e = Event.current;
        mapScrollPos = GUI.BeginScrollView( _rect, mapScrollPos, new Rect( -30, -30, width + 60, height + 60 )  );

            // ======================================================== 
            // draw grid lines
            // ======================================================== 

            for ( int i = 0; i <= _tileMap.col; ++i ) {
                float x = i * _tileMap.tileWidth;
                exEditorHelper.DrawLine ( new Vector2( x, 0.0f ), 
                                          new Vector2( x, height ),
                                          Color.gray,
                                          1.0f );
            }
            for ( int i = 0; i <= _tileMap.row; ++i ) {
                float y = i * _tileMap.tileHeight;
                exEditorHelper.DrawLine ( new Vector2( 0.0f,  y ), 
                                          new Vector2( width, y ),
                                          Color.gray,
                                          1.0f );
            }

            // ======================================================== 
            // draw mouse 
            // ======================================================== 

            if ( curGridX >= 0 && curGridX < _tileMap.col * _tileMap.tileWidth &&
                 curGridY >= 0 && curGridY < _tileMap.row * _tileMap.tileHeight )
            {
                exEditorHelper.DrawRect( new Rect( curGridX, 
                                                   curGridY+1, 
                                                   _tileMap.tileWidth-1, 
                                                   _tileMap.tileHeight-1 ),
                                         new Color( 1.0f, 1.0f, 1.0f, 0.2f ),
                                         new Color( 0.0f, 0.0f, 0.0f, 0.0f )  );
            }

            // ======================================================== 
            // handle mouse event 
            // ======================================================== 

            if ( e.type == EventType.MouseMove ) {
                int grid_x; int grid_y;
                SnapToGrid ( _tileMap, e.mousePosition, out grid_x, out grid_y );
                if ( curGridX != grid_x || curGridY != grid_y ) {
                    curGridX = grid_x;
                    curGridY = grid_y;
                    Repaint();
                }
            }

        GUI.EndScrollView();
        GUILayoutUtility.GetRect ( width, height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void SnapToGrid ( exTileMap _tileMap, Vector2 _pos, out int _x, out int _y ) {
        Vector2 startPos = new Vector2( 30, 30 ); 
        Vector2 deltaPos = _pos - startPos;

        _x = Mathf.CeilToInt(deltaPos.x / _tileMap.tileWidth) * _tileMap.tileWidth;
        _y = Mathf.CeilToInt(deltaPos.y / _tileMap.tileHeight) * _tileMap.tileHeight;
    }
}
