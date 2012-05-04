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

    static string[] anchorTexts = new string[] {
        "", "", "", 
        "", "", "", 
        "", "", "", 
    };

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    exScreenPosition curEdit;

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

	public override void OnInspectorGUI () {

        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        curEdit.plane = curEdit.GetComponent<exPlane>();

        // ======================================================== 
        // screen x 
        // ======================================================== 

        curEdit.x = EditorGUILayout.FloatField ( "Screen X", curEdit.x );

        // ======================================================== 
        // screen y 
        // ======================================================== 

        curEdit.y = EditorGUILayout.FloatField ( "Screen Y", curEdit.y );

        // ======================================================== 
        // anchor
        // ======================================================== 

        EditorGUILayout.LabelField ( "Anchor", "" );
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
            curEdit.anchor 
                = (exPlane.Anchor)GUILayout.SelectionGrid ( (int)curEdit.anchor, 
                                                            anchorTexts, 
                                                            3, 
                                                            GUILayout.Width(80) );  
        GUILayout.EndHorizontal();

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
	}
}
