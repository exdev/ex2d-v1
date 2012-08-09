// ======================================================================================
// File         : exUIScrollViewEditor.cs
// Author       : Wu Jie 
// Last Change  : 07/23/2012 | 22:20:54 PM | Monday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// public
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exUIScrollView))]
public class exUIScrollViewEditor : exUIElementEditor {

    SerializedProperty horizontalBarProp;
    SerializedProperty horizontalSliderProp;
    SerializedProperty verticalBarProp;
    SerializedProperty verticalSliderProp;

    SerializedProperty scrollDirectionProp;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();

        horizontalBarProp = serializedObject.FindProperty ("horizontalBar");
        horizontalSliderProp = serializedObject.FindProperty ("horizontalSlider");
        verticalBarProp = serializedObject.FindProperty ("verticalBar");
        verticalSliderProp = serializedObject.FindProperty ("verticalSlider");

        scrollDirectionProp = serializedObject.FindProperty ("scrollDirection");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {

        // ======================================================== 
        // Base GUI 
        // ======================================================== 

        base.OnInspectorGUI();
        GUILayout.Space(20);

        // ======================================================== 
        // 
        // ======================================================== 

        serializedObject.Update ();

            EditorGUILayout.PropertyField( horizontalBarProp );
            EditorGUILayout.PropertyField( horizontalSliderProp );
            EditorGUILayout.PropertyField( verticalBarProp );
            EditorGUILayout.PropertyField( verticalSliderProp );

            EditorGUILayout.PropertyField( scrollDirectionProp );

        serializedObject.ApplyModifiedProperties ();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if ( GUILayout.Button("Update...", GUILayout.Height(20) ) ) {
            exUIElement.FindAndAddChild(target as exUIScrollView);
            EditorUtility.SetDirty(this);
        }
        GUILayout.EndHorizontal();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSceneGUI () {
        base.OnSceneGUI();
    }
}
