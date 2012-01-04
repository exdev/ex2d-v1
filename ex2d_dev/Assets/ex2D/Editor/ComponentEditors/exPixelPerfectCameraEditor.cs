// ======================================================================================
// File         : exPixelPerfectCameraEditor.cs
// Author       : Wu Jie 
// Last Change  : 10/05/2011 | 17:39:20 PM | Wednesday,October
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

[CustomEditor(typeof(exPixelPerfectCamera))]
public class exPixelPerfectCameraEditor : Editor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exPixelPerfectCamera curEdit;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( target != curEdit ) {
            curEdit = target as exPixelPerfectCamera;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {

        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        // customResolution
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            curEdit.customResolution = GUILayout.Toggle ( curEdit.customResolution, "Custom Resolution" ); 
        GUILayout.EndHorizontal();

        GUI.enabled = curEdit.customResolution;
        EditorGUI.indentLevel = 2;
            // width
            curEdit.width = EditorGUILayout.IntField ( "Width", curEdit.width );

            // height
            curEdit.height = EditorGUILayout.IntField ( "Height", curEdit.height );
        EditorGUI.indentLevel = 1;
        GUI.enabled = true;

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
    }
}
