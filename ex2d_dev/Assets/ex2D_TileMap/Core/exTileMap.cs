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

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    //
    public exTileSheet tileSheet = null;
    public Type type = Type.Rectangular;
    public int row = 50;
    public int col = 50;
    public int tileWidth = 32;
    public int tileHeight = 32;
    public int tileOffsetX = 0;
    public int tileOffsetY = 0;

    // editor properties
    public bool editorShowGrid = true;
    public Color editorColor = Color.white;

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// Clear the tilemap by sets the alpha of the color to 0.0f
    // ------------------------------------------------------------------ 

    public void Clear () {
        // TODO { 
        // int vertexCount = col_ * row_ * 4;
        // Color[] colors = new Color[vertexCount];
        // for ( int i = 0; i < vertexCount; ++i ) {
        //     colors[i] = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        // }
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void SetTile ( int _row, int _col, int _index ) {
        // TODO { 
        // if ( meshFilter == null || meshFilter.sharedMesh == null) {
        //     return;
        // }

        // Rect uv = tileSheet_.GetTileUV ( _index );
        // float xStart  = uv.x;
        // float yStart  = uv.y;
        // float xEnd    = uv.xMax;
        // float yEnd    = uv.yMax;
        // int id = _row * _col * 4; 

        // // set uv
        // Vector2[] uvs = meshFilter.sharedMesh.uv;
        // uvs[id + 0] = new Vector2 ( xStart,  yEnd );   
        // uvs[id + 1] = new Vector2 ( xEnd,    yEnd );  
        // uvs[id + 2] = new Vector2 ( xStart,  yStart );
        // uvs[id + 3] = new Vector2 ( xEnd,    yStart );
        // meshFilter.sharedMesh.uv = uvs;

        // // set color
        // Color[] colors = meshFilter.sharedMesh.colors;
        // colors[id + 0] = color_;
        // colors[id + 1] = color_;
        // colors[id + 2] = color_;
        // colors[id + 3] = color_;
        // meshFilter.sharedMesh.colors = colors;
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Resize ( int _row, int _col ) {
        // TODO { 
        // row_ = _row;
        // col_ = _col;

        // // TODO: runtime and editor { 
        // // ForceUpdateMesh( newMesh );
        // // } TODO end 
        // } TODO end 
    } 
}

