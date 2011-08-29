// ======================================================================================
// File         : exAtlasInfoWizard.cs
// Author       : Wu Jie 
// Last Change  : 08/22/2011 | 13:58:59 PM | Monday,August
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

public class exAtlasInfoWizard : ScriptableWizard {

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    static int[] sizeList = new int[] { 
        32, 64, 128, 256, 512, 1024, 2048, 4096 
    };
    static string[] sizeTextList = new string[] { 
        "32px", "64px", "128px", "256px", "512px", "1024px", "2048px", "4096px" 
    };

    public string assetPath = "";
    public string assetName = "New Atlas";
    public int width = 512;
    public int height = 512;
    
    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D Atlas Info")]
    public static void Create () {
        ScriptableWizard.DisplayWizard<exAtlasInfoWizard>("Create Atlas Info");
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
            assetPath = exEditorRuntimeHelper.GetCurrentDirectory();
            assetPath = EditorGUILayout.TextField( "Saved Path", assetPath, GUILayout.MaxWidth(405) );

            assetName = Path.GetFileNameWithoutExtension(assetName);
            assetName = EditorGUILayout.TextField( "Asset Name", assetName, GUILayout.MaxWidth(405) );

            width = EditorGUILayout.IntPopup ( "Width", width, sizeTextList, sizeList, GUILayout.MaxWidth(200) );
            height = EditorGUILayout.IntPopup ( "Height", height, sizeTextList, sizeList, GUILayout.MaxWidth(200) );

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
                        exAtlasInfoUtility.CreateAtlasInfo ( assetPath, assetName, width, height );
                    }
                    Close();
                }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.EndVertical();
    }
}
