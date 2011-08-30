// ======================================================================================
// File         : exTileInfoUtility.cs
// Author       : Wu Jie 
// Last Change  : 08/30/2011 | 10:52:26 AM | Tuesday,August
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

public class exTileInfoUtility : MonoBehaviour {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D Tile Info")]
    public static exTileInfo Create () {
        return Create ( exEditorHelper.GetCurrentDirectory(), "New TileInfo" );
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

        // TODO { 
        // exEditorHelper.RenameProjectWindowItem ( AssetDatabase.AssetPathToGUID (assetPath),
        //                                          "default asset" );
        // } TODO end 

        //
        exTileInfo newTileInfo = ScriptableObject.CreateInstance<exTileInfo>();
        AssetDatabase.CreateAsset(newTileInfo, assetPath);
        Selection.activeObject = newTileInfo;
        return newTileInfo;
    }
}
