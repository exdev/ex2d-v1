// ======================================================================================
// File         : exSpriteAnimationDBEditor.cs
// Author       : Wu Jie 
// Last Change  : 07/07/2011 | 15:59:47 PM | Thursday,July
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

[CustomEditor(typeof(exSpriteAnimationDB))]
public class exSpriteAnimationDBEditor : Editor {

    private exSpriteAnimationDB curEditTarget;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        if ( target != curEditTarget ) {
            curEditTarget = target as exSpriteAnimationDB;
        }
        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space();

        // sync button
        EditorGUI.indentLevel = 1;
        if ( GUILayout.Button ("Sync", GUILayout.Width(100)) ) {
            exSpriteAnimationDB.ForceSync();
        }

        // rebuild all
        if ( GUILayout.Button ("Build All", GUILayout.Width(100)) ) {
            exSpriteAnimationDB.BuildAll();
        }

        // show version
        GUI.enabled = false;
        EditorGUILayout.LabelField( "version", curEditTarget.curVersion.ToString() );
        GUI.enabled = true;

        // show spAnimClipGUIDs
        EditorGUI.indentLevel = 0;
        curEditTarget.showData = EditorGUILayout.Foldout(curEditTarget.showData, "SpriteAnimClip Asset List");
        if ( curEditTarget.showData ) {
            GUI.enabled = false;
            EditorGUI.indentLevel = 2;
            for ( int i = 0; i < curEditTarget.spAnimClipGUIDs.Count; ++i ) {
                string guid = curEditTarget.spAnimClipGUIDs[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string spAnimClipName = Path.GetFileNameWithoutExtension (path);
                EditorGUILayout.LabelField( "[" + i + "]",  spAnimClipName );
            }
            GUI.enabled = true;
        }

        // show texture to AtlasElement table
        EditorGUI.indentLevel = 0;
        curEditTarget.showTable = EditorGUILayout.Foldout(curEditTarget.showTable, "exAtlasInfo <-> SpriteAnimClip List");
        if ( curEditTarget.showTable ) {
            GUI.enabled = false;
            foreach ( KeyValuePair<string,List<string> > pair in exSpriteAnimationDB.GetTexGUIDToAnimClipGUIDs() ) {
                string textureName = Path.GetFileNameWithoutExtension ( AssetDatabase.GUIDToAssetPath(pair.Key) );
                EditorGUI.indentLevel = 2;
                EditorGUILayout.LabelField ( textureName, "" );

                // SpriteAnimClips
                EditorGUI.indentLevel = 3;
                List<string> spAnimClipGUIDs = pair.Value;
                for ( int i = 0; i < spAnimClipGUIDs.Count; ++i ) {
                    string spAnimClipName = Path.GetFileNameWithoutExtension ( AssetDatabase.GUIDToAssetPath(spAnimClipGUIDs[i]) );
                    EditorGUILayout.LabelField ( "[" + i + "]", spAnimClipName );
                }
                EditorGUILayout.Space();
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
