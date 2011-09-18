// ======================================================================================
// File         : exTileMap.cs
// Author       : Wu Jie 
// Last Change  : 09/16/2011 | 10:29:33 AM | Friday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
///
/// Tile Map texture information
///
///////////////////////////////////////////////////////////////////////////////

public class exTileMap : ScriptableObject {

    // ------------------------------------------------------------------ 
    /// The tile map type
    // ------------------------------------------------------------------ 

    public enum Type {
        Rectangular, ///< regular
        Hexagonal,   ///< eight corner
        Isometric,   ///< 45 degrees
    }

    // ------------------------------------------------------------------ 
    /// the edit tool 
    // ------------------------------------------------------------------ 

    public enum EditTool {
        Stamp,  ///< paint select grid
        Bucket, ///< paint grids connect with same id
        Area,   ///< select a range for editing
    }

    // ------------------------------------------------------------------ 
    /// the edit mode 
    // ------------------------------------------------------------------ 

    public enum EditMode {
        Paint,  ///< paint grids
        Erase,  ///< erase grids
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    //
    public exTileSheet tileSheet = null;
    public Type type = Type.Rectangular;
    public int row = 20;
    public int col = 20;
    public int tileWidth = 32;
    public int tileHeight = 32;
    public int tileOffsetX = 0;
    public int tileOffsetY = 0;
    public int[] grids = new int[0];

    // editor properties
    public bool editorNeedRebuild = false;
    public EditTool editorEditTool = EditTool.Stamp;
    public EditMode editorEditMode = EditMode.Paint;
    public bool editorShowGrid = true;
    public Color editorColor = Color.white;

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// resize the col and row
    /// \param _col the new number of column
    /// \param _row the new number of row
    // ------------------------------------------------------------------ 

    public void Resize ( int _col, int _row ) {
        int[] newGrids = new int[_row*_col];
        for ( int r = 0; r < _row; ++r ) {
            for ( int c = 0; c < _col; ++c ) {
                if ( r < row && c < col ) {
                    newGrids[r*_col+c] = grids[r*col+c];
                }
                else {
                    newGrids[r*_col+c] = -1;
                }
            }
        }

        col = _col;
        row = _row;
        grids = newGrids;
        editorNeedRebuild = true;
    } 

    // ------------------------------------------------------------------ 
    /// Clear the tilemap by sets the alpha of the color to 0.0f
    // ------------------------------------------------------------------ 

    public void Clear () {
        for ( int i = 0; i < row * col; ++i ) {
            grids[i] = -1;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void SetTile ( int _col, int _row, int _index ) {
        if ( _col < 0 || _col >= col || _row < 0 || _row >= row )
            return;
        grids[_row*col + _col] = _index;
        editorNeedRebuild = true;
    }
}

