// ======================================================================================
// File         : exTileInfo.cs
// Author       : Wu Jie 
// Last Change  : 08/20/2011 | 15:11:00 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class exTileInfo : ScriptableObject {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Texture texture;
    public Material material;
    public int tileWidth = 32;
    public int tileHeight = 32;
    public int padding = 1;
    public Vector2 anchor = Vector2.zero;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class Element {
        public GameObject prefab;
    }
    public List<Element> elements = new List<Element>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D Tile Info")]
    public static exTileInfo Create () {
        return Create ( exEditorRuntimeHelper.GetCurrentDirectory(), "New TileInfo.asset" );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static exTileInfo Create ( string _path, string _name ) {
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

        //
        exTileInfo newTileInfo = ScriptableObject.CreateInstance<exTileInfo>();
        AssetDatabase.CreateAsset(newTileInfo, assetPath);
        Selection.activeObject = newTileInfo;
        return newTileInfo;
    }
#endif
}

