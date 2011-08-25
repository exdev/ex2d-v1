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

        // sync button
        EditorGUILayout.Space();
        if ( GUILayout.Button ("Sync", GUILayout.Width(100)) ) {
            exSpriteAnimationDB.Sync();
        }

        // show data
        EditorGUI.indentLevel = 0;
        curEditTarget.showData = EditorGUILayout.Foldout(curEditTarget.showData, "SpriteAnimClip Asset List");
        if ( curEditTarget.showData ) {
            GUI.enabled = false;
            EditorGUI.indentLevel = 2;
            for ( int i = 0; i < curEditTarget.data.Count; ++i ) {
                exSpriteAnimClip animClip = curEditTarget.data[i];
                EditorGUILayout.ObjectField( "[" + i + "]"
                                             , animClip
                                             , typeof(exSpriteAnimClip)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                             , false 
#endif
                                           );
            }
            GUI.enabled = true;
        }

        // show texture to AtlasElement table
        EditorGUI.indentLevel = 0;
        curEditTarget.showTable = EditorGUILayout.Foldout(curEditTarget.showTable, "exAtlasInfo <-> SpriteAnimClip List");
        if ( curEditTarget.showTable ) {
            GUI.enabled = false;
            foreach ( KeyValuePair<string,List<exSpriteAnimClip> > pair in exSpriteAnimationDB.GetGUIDToAnimClips() ) {
                // texture
                Texture2D texture = (Texture2D)exEditorRuntimeHelper.LoadAssetFromGUID(pair.Key, typeof(Texture2D));
                EditorGUI.indentLevel = 2;
                EditorGUILayout.ObjectField( "Texture"
                                             , texture
                                             , typeof(Texture2D)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                             , false 
#endif
                                           );

                // SpriteAnimClips
                EditorGUI.indentLevel = 3;
                List<exSpriteAnimClip> spAnimClips = exSpriteAnimationDB.GetSpriteAnimClips(pair.Key);
                for ( int i = 0; i < spAnimClips.Count; ++i ) {
                    exSpriteAnimClip spAnimClip =  spAnimClips[i]; 
                    EditorGUILayout.ObjectField( "[" + i + "]"
                                                 , spAnimClip
                                                 , typeof(exSpriteAnimClip)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                 , false 
#endif
                                               );
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
