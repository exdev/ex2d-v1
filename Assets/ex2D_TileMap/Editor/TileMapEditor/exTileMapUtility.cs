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

public class exTileMapUtility : MonoBehaviour {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/TileMap Object")]
    static void CreateTileMapObject () {
        GameObject go = new GameObject("TileMapObject");
        go.AddComponent<exTileMap>();
        Selection.activeObject = go;
    }
}

