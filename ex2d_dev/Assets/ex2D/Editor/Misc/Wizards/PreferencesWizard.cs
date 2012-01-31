// ======================================================================================
// File         : PreferencesWizard.cs
// Author       : Wu Jie 
// Last Change  : 01/25/2012 | 00:00:30 AM | Wednesday,January
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

class PreferencesWizard : ScriptableWizard {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/Preferences")]
    public static void Create () {
        ScriptableWizard.DisplayWizard<PreferencesWizard>("ex2D Preferences");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {

        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.gray;
        GUILayout.Label( "General", style );

        GUILayout.Space (10);

        // ======================================================== 
        // atlas DB 
        // ======================================================== 

        exAtlasDB.dbPath = EditorGUILayout.TextField ( "Atlas DB Path", EditorPrefs.GetString( exAtlasDB.dbKey, exAtlasDB.dbPath ) );
        EditorPrefs.SetString( exAtlasDB.dbKey, exAtlasDB.dbPath );

        // ======================================================== 
        // sprite animation DB 
        // ======================================================== 

        exSpriteAnimationDB.dbPath = EditorGUILayout.TextField ( "SpriteAnimation DB Path", 
                                                                 EditorPrefs.GetString( exSpriteAnimationDB.dbKey, exSpriteAnimationDB.dbPath ) );
        EditorPrefs.SetString( exSpriteAnimationDB.dbKey, exSpriteAnimationDB.dbPath );
    }
}
