// ======================================================================================
// File         : exTileMapWizard.cs
// Author       : Wu Jie 
// Last Change  : 09/17/2011 | 21:39:00 PM | Saturday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

class exTileMapWizard : ScriptableWizard {

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    public string assetPath = "";
    public string assetName = "New TileMap";
    public int row = 20;
    public int col = 20;
    
    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D/Tile Map")]
    public static void Create () {
        ScriptableWizard.DisplayWizard<exTileMapWizard>("Create Tile Map");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSelectionChange () {
        Repaint();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        GUILayout.BeginVertical();
            assetPath = exEditorHelper.GetCurrentDirectory();
            assetPath = EditorGUILayout.TextField( "Saved Path", assetPath, GUILayout.MaxWidth(405) );

            assetName = Path.GetFileNameWithoutExtension(assetName);
            assetName = EditorGUILayout.TextField( "Asset Name", assetName, GUILayout.MaxWidth(405) );

            GUILayout.BeginHorizontal ( GUILayout.MaxWidth(405) );
                col = Mathf.Max( EditorGUILayout.IntField( "Column", col ), 1 ); 
                row = Mathf.Max( EditorGUILayout.IntField( "Row", row ), 1 ); 
            GUILayout.EndHorizontal ();

            // Create Button
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if ( GUILayout.Button( "Create...", GUILayout.MaxWidth(100) ) ) {
                    bool doCreate = true;
                    string path = Path.Combine( assetPath, assetName + ".asset" );
                    FileInfo fileInfo = new FileInfo(path);
                    if ( fileInfo.Exists ) {
                        doCreate = EditorUtility.DisplayDialog( assetName + " already exists.",
                                                                "Do you want to overwrite the old one?",
                                                                "Yes", "No" );
                    }
                    if ( doCreate ) {
                        exTileMapUtility.Create ( assetPath, assetName, row, col );
                    }
                    Close();
                }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.EndVertical();
    }
}
