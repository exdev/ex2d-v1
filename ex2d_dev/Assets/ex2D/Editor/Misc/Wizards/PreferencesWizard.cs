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

        string atlasDBKey = "AtlasDB_Path";
        exAtlasDB.dbPath = EditorGUILayout.TextField ( "Atlas DB Path", EditorPrefs.GetString( atlasDBKey, exAtlasDB.dbPath ) );
        EditorPrefs.GetString( atlasDBKey, exAtlasDB.dbPath );

        // ======================================================== 
        // sprite animation DB 
        // ======================================================== 

        string spanimDBKey = "SpriteAnimDB_Path";
        exSpriteAnimationDB.dbPath = EditorGUILayout.TextField ( "SpriteAnimation DB Path", EditorPrefs.GetString( spanimDBKey, exSpriteAnimationDB.dbPath ) );
        EditorPrefs.GetString( spanimDBKey, exSpriteAnimationDB.dbPath );
    }
}
