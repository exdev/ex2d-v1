// ======================================================================================
// File         : exAtlasDBEditor.cs
// Author       : Wu Jie 
// Last Change  : 06/15/2011 | 10:22:23 AM | Wednesday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exAtlasDB))]
class exAtlasDBEditor : Editor {

    private exAtlasDB curEditTarget;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        if ( target != curEditTarget ) {
            curEditTarget = target as exAtlasDB;
        }
        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space();

        // sync button
        EditorGUI.indentLevel = 1;
        if ( GUILayout.Button ("Sync", GUILayout.Width(100)) ) {
            exAtlasDB.ForceSync();
        }

        // rebuild all
        if ( GUILayout.Button ("Build All", GUILayout.Width(100)) ) {
            exAtlasDB.BuildAll();
        }

        // show version
        GUI.enabled = false;
        EditorGUILayout.LabelField( "version", curEditTarget.curVersion.ToString() );
        GUI.enabled = true;

        // show atlasInfoGUIDs
        EditorGUI.indentLevel = 0;
        curEditTarget.showData = EditorGUILayout.Foldout(curEditTarget.showData, "Atlas Asset List");
        EditorGUI.indentLevel = 2;
        if ( curEditTarget.showData ) {
            GUI.enabled = false;
            for ( int i = 0; i < curEditTarget.atlasInfoGUIDs.Count; ++i ) {
                string guid = curEditTarget.atlasInfoGUIDs[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string atlasInfoName = Path.GetFileNameWithoutExtension (path);
                EditorGUILayout.LabelField( "[" + i + "]",  atlasInfoName );
            }
            GUI.enabled = true;
        }

        // show texture to AtlasElement table
        EditorGUI.indentLevel = 0;
        curEditTarget.showTable = EditorGUILayout.Foldout(curEditTarget.showTable, "Texture To Atlas.Element Table");
        EditorGUI.indentLevel = 2;
        if ( curEditTarget.showTable ) {
            GUI.enabled = false;
            foreach ( KeyValuePair<string,exAtlasDB.ElementInfo> pair in exAtlasDB.GetTexGUIDToElementInfo() ) {
                string textureName = Path.GetFileNameWithoutExtension ( AssetDatabase.GUIDToAssetPath(pair.Key) );
                string atlasName = Path.GetFileNameWithoutExtension ( AssetDatabase.GUIDToAssetPath(pair.Value.guidAtlas) );
                EditorGUILayout.LabelField ( atlasName + "[" + pair.Value.indexInAtlas + "]", textureName );
            }
            GUI.enabled = true;
        }
        EditorGUI.indentLevel = 0;

        //
        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEditTarget);
        }
    }
}
