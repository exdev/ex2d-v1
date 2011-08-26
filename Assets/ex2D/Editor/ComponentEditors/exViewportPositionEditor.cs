// ======================================================================================
// File         : exViewportPositionEditor.cs
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

[CustomEditor(typeof(exViewportPosition))]
public class exViewportPositionEditor : Editor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exViewportPosition curEdit;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( target != curEdit ) {
            curEdit = target as exViewportPosition;
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
        // viewport x 
        // ======================================================== 

        curEdit.x = EditorGUILayout.FloatField ( "Viewport X", curEdit.x );

        // ======================================================== 
        // viewport y 
        // ======================================================== 

        curEdit.y = EditorGUILayout.FloatField ( "Viewport Y", curEdit.y );

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
	}
}
