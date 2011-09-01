// ======================================================================================
// File         : exSpriteAnimClipWizard.cs
// Author       : Wu Jie 
// Last Change  : 08/22/2011 | 15:19:58 PM | Monday,August
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

public class exSpriteAnimClipWizard : ScriptableWizard {

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    public string assetPath = "";
    public string assetName = "New Sprite Animation";
    
    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D Sprite Animation")]
    public static void Create () {
        ScriptableWizard.DisplayWizard<exSpriteAnimClipWizard>("Create Sprite Animation Clip");
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
                        exSpriteAnimClip clip = exSpriteAnimationUtility.CreateSpriteAnimClip ( assetPath, assetName );
                        EditorGUIUtility.PingObject(clip);
                    }
                    Close();
                }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.EndVertical();
    }
}
