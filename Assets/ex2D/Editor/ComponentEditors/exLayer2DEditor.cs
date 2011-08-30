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

        // NOTE: limit float use only .00, otherwise the newBias will always different and
        //       the scene will keep dirty.
        float newBias = EditorGUILayout.Slider( "Bias", curEdit.bias, 0.0f, 1.0f );
        int bias = Mathf.FloorToInt(newBias * 100.0f);
        newBias = (float)bias/100.0f;
        if ( newLayer != curEdit.layer || newBias != curEdit.bias ) {
            curEdit.SetLayer( newLayer, newBias );
            exSpriteUtility.RecursivelyUpdateLayer(curEdit.transform);
        }

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
	}
}
