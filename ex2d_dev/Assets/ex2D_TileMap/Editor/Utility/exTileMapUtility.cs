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
        go.AddComponent<exTileMapRenderer>();
        Selection.activeObject = go;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static exTileMap Create ( string _path, string _name, int _row, int _col ) {
        //
        if ( new DirectoryInfo(_path).Exists == false ) {
            Debug.LogError ( "can't create asset, path not found" );
            return null;
        }
        if ( string.IsNullOrEmpty(_name) ) {
            Debug.LogError ( "can't create asset, the name is empty" );
            return null;
        }
        string assetPath = Path.Combine( _path, _name + ".asset" );

        // TODO { 
        // exEditorHelper.RenameProjectWindowItem ( AssetDatabase.AssetPathToGUID (assetPath),
        //                                          "default asset" );
        // } TODO end 

        //
        exTileMap newTileMap = ScriptableObject.CreateInstance<exTileMap>();
        newTileMap.row = _row;
        newTileMap.col = _col;
        newTileMap.grids = new int[newTileMap.row * newTileMap.col];
        newTileMap.Clear();

        AssetDatabase.CreateAsset(newTileMap, assetPath);
        Selection.activeObject = newTileMap;
        return newTileMap;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Rebuild ( this exTileMapRenderer _tileMapRenderer ) {
        _tileMapRenderer.Build ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Build ( this exTileMapRenderer _tileMapRenderer ) {
        EditorUtility.SetDirty(_tileMapRenderer);

        // NOTE: it is possible user duplicate an GameObject, 
        //       if we directly change the mesh, the original one will changed either.
        Mesh newMesh = new Mesh();
        newMesh.Clear();

        // build vertices, normals, uvs and colors.
        _tileMapRenderer.ForceUpdateMesh( newMesh );

        //
        GameObject.DestroyImmediate( _tileMapRenderer.meshFilter.sharedMesh, true ); // delete old mesh (to avoid leaking)
        _tileMapRenderer.meshFilter.sharedMesh = newMesh;
    }
}

