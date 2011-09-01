// ======================================================================================
// File         : exTileMapUtility.cs
// Author       : Wu Jie 
// Last Change  : 08/31/2011 | 01:33:44 AM | Wednesday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public static class exTileMapUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/TileMap Object")]
    static void CreateTileMapObject () {
        GameObject go = new GameObject("TileMapObject");
        go.AddComponent<exTileMap>();
        Selection.activeObject = go;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Rebuild ( this exTileMap _tileMap ) {
        _tileMap.Build ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Build ( this exTileMap _tileMap ) {
        EditorUtility.SetDirty(_tileMap);

        // NOTE: it is possible user duplicate an GameObject, 
        //       if we directly change the mesh, the original one will changed either.
        Mesh newMesh = new Mesh();
        newMesh.Clear();

        // build vertices, normals, uvs and colors.
        MeshFilter meshFilter = _tileMap.GetComponent<MeshFilter>();
        _tileMap.ForceUpdateMesh( newMesh );
        newMesh.uv = new Vector2[_tileMap.row * _tileMap.col * 4]; 

        //
        meshFilter.sharedMesh = newMesh;
    }
}

