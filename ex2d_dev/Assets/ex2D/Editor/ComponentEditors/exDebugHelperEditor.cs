// ======================================================================================
// File         : exDebugHelperEditor.cs
// Author       : Wu Jie 
// Last Change  : 11/25/2011 | 23:49:23 PM | Friday,November
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

[CustomEditor(typeof(exDebugHelper))]
public class exDebugHelperEditor : Editor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exDebugHelper curEdit;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( target != curEdit ) {
            curEdit = target as exDebugHelper;
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
        // text print
        // ======================================================== 

        curEdit.txtPrint = (exSpriteFont)EditorGUILayout.ObjectField( "Text Print"
                                                                      , curEdit.txtPrint
                                                                      , typeof(exSpriteFont)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                      , false 
#endif
                                                                    );

        // ======================================================== 
        // text FPS 
        // ======================================================== 

        curEdit.txtFPS = (exSpriteFont)EditorGUILayout.ObjectField( "Text FPS"
                                                                    , curEdit.txtFPS
                                                                    , typeof(exSpriteFont)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                    , false 
#endif
                                                                  );

        // ======================================================== 
        // text Log 
        // ======================================================== 

        curEdit.txtLog = (exSpriteFont)EditorGUILayout.ObjectField( "Text Log"
                                                                    , curEdit.txtLog
                                                                    , typeof(exSpriteFont)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                    , false 
#endif
                                                                  );

        curEdit.showFps = EditorGUILayout.Toggle( "Show Fps", curEdit.showFps );
        curEdit.showScreenPrint = EditorGUILayout.Toggle( "Show Screen Print", curEdit.showScreenPrint );
        curEdit.showScreenLog = EditorGUILayout.Toggle( "Show Screen Log", curEdit.showScreenLog );

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
	}
}
