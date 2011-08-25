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

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exAtlasDB))]
public class exAtlasDBEditor : Editor {

    private exAtlasDB curEditTarget;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        if ( target != curEditTarget ) {
            curEditTarget = target as exAtlasDB;
        }
        EditorGUIUtility.LookLikeInspector ();

        // sync button
        EditorGUI.indentLevel = 1;
        if ( GUILayout.Button ("Sync", GUILayout.Width(100)) ) {
            exAtlasDB.ForceSync();
        }

        // show data
        EditorGUI.indentLevel = 0;
        curEditTarget.showData = EditorGUILayout.Foldout(curEditTarget.showData, "Atlas Asset List");
        EditorGUI.indentLevel = 2;
        if ( curEditTarget.showData ) {
            GUI.enabled = false;
            int i = 0;
            foreach ( exAtlasInfo atlas in curEditTarget.data ) {
                EditorGUILayout.ObjectField( "Atlas " + i 
                                             , atlas 
                                             , typeof(exAtlasInfo) 
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                             , false 
#endif
                                           );
                ++i;
            }
            GUI.enabled = true;
        }

        // show texture to AtlasElement table
        EditorGUI.indentLevel = 0;
        curEditTarget.showTable = EditorGUILayout.Foldout(curEditTarget.showTable, "Texture To Atlas.Element Table");
        EditorGUI.indentLevel = 2;
        if ( curEditTarget.showTable ) {
            GUI.enabled = false;
            int i = 0;
            foreach ( KeyValuePair<Texture2D,exAtlasInfo.Element> pair in exAtlasDB.GetTextureToElementTable() ) {
                if ( pair.Key != null &&
                     pair.Value != null &&
                     pair.Value.atlasInfo != null ) 
                {
                    string label = pair.Value.atlasInfo.name 
                        + " - " + pair.Value.coord[0] + ", " + pair.Value.coord[1];
                    EditorGUILayout.ObjectField( label 
                                                 , pair.Key 
                                                 , typeof(Texture2D) 
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                 , false 
#endif
                                               );
                }
                ++i;
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
