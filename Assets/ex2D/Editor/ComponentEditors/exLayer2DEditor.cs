// ======================================================================================
// File         : exLayer2DEditor.cs
// Author       : Wu Jie 
// Last Change  : 07/29/2011 | 19:15:18 PM | Friday,July
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

[CustomEditor(typeof(exLayer2D))]
public class exLayer2DEditor : Editor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exLayer2D curEdit;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( target != curEdit ) {
            curEdit = target as exLayer2D;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        bool inAnimMode = AnimationUtility.InAnimationMode();

        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        // ======================================================== 
        // layer & bias 
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        int newLayer = EditorGUILayout.IntSlider( "Layer", curEdit.layer, 0, exLayer2D.MAX_LAYER-1 );
        GUI.enabled = true;

        float newBias = EditorGUILayout.Slider( "Bias", curEdit.bias, 0.0f, 1.0f );
        if ( newLayer != curEdit.layer || newBias != curEdit.bias ) {
            curEdit.SetLayer( newLayer, newBias );
            exSpriteEditorHelper.RecursivelyUpdateLayer(curEdit.transform);
        }

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
	}
}
