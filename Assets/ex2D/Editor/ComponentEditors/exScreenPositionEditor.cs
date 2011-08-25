// ======================================================================================
// File         : exScreenPositionEditor.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 12:01:10 PM | Saturday,August
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

[CustomEditor(typeof(exScreenPosition))]
public class exScreenPositionEditor : Editor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exScreenPosition curEdit;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( target != curEdit ) {
            curEdit = target as exScreenPosition;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {

        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        // ======================================================== 
        // Camera
        // ======================================================== 

        curEdit.renderCamera = (Camera)EditorGUILayout.ObjectField( "Render Camera"
                                                                    , curEdit.renderCamera
                                                                    , typeof(Camera)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                    , true 
#endif
                                                                  );

        // ======================================================== 
        // screen x 
        // ======================================================== 

        curEdit.x = EditorGUILayout.FloatField ( "Screen X", curEdit.x );

        // ======================================================== 
        // screen y 
        // ======================================================== 

        curEdit.y = EditorGUILayout.FloatField ( "Screen Y", curEdit.y );

	}
}
