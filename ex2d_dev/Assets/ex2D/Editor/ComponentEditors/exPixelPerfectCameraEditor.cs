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
[CanEditMultipleObjects]
public class exPixelPerfectCameraEditor : Editor {

    SerializedProperty customResolutionProp;
    SerializedProperty widthProp;
    SerializedProperty heightProp;
    SerializedProperty fixOrthographicSizeProp;
    SerializedProperty orthographicSizeProp;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        customResolutionProp    = serializedObject.FindProperty("customResolution");
        widthProp               = serializedObject.FindProperty("width");
        heightProp              = serializedObject.FindProperty("height");
        fixOrthographicSizeProp = serializedObject.FindProperty("fixOrthographicSize");
        orthographicSizeProp    = serializedObject.FindProperty("orthographicSize");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {
        serializedObject.Update ();

            //
            EditorGUILayout.PropertyField (customResolutionProp);
            GUI.enabled = customResolutionProp.boolValue;
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField (widthProp);
                EditorGUILayout.PropertyField (heightProp);
                --EditorGUI.indentLevel;
            GUI.enabled = true;

            //
            EditorGUILayout.PropertyField (fixOrthographicSizeProp);
            GUI.enabled = fixOrthographicSizeProp.boolValue;
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField (orthographicSizeProp);
                --EditorGUI.indentLevel;
            GUI.enabled = true;

        serializedObject.ApplyModifiedProperties ();
    }
}
