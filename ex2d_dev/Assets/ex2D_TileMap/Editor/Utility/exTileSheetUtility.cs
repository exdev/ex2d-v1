// ======================================================================================
// File         : exTileSheetUtility.cs
// Author       : Wu Jie 
// Last Change  : 09/16/2011 | 10:14:36 AM | Friday,September
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

public class exTileSheetUtility : MonoBehaviour {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D/Tile Sheet")]
    public static exTileSheet Create () {
        return Create ( exEditorHelper.GetCurrentDirectory(), "New TileSheet" );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static exTileSheet Create ( string _path, string _name ) {
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
        exTileSheet newTileSheet = ScriptableObject.CreateInstance<exTileSheet>();
        AssetDatabase.CreateAsset(newTileSheet, assetPath);
        Selection.activeObject = newTileSheet;
        return newTileSheet;
    }
}
