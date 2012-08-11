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

    SerializedProperty showSliderOnDraggingProp;
    SerializedProperty scrollDirectionProp;
    SerializedProperty decelerationProp;
    SerializedProperty bounceProp;
    SerializedProperty bounceBackDurationProp;

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

        showSliderOnDraggingProp = serializedObject.FindProperty ("showSliderOnDragging");
        scrollDirectionProp = serializedObject.FindProperty ("scrollDirection");
        decelerationProp = serializedObject.FindProperty ("deceleration");
        bounceProp = serializedObject.FindProperty ("bounce");
        bounceBackDurationProp = serializedObject.FindProperty ("bounceBackDuration");
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

            EditorGUILayout.PropertyField( showSliderOnDraggingProp );
            EditorGUILayout.PropertyField( scrollDirectionProp );
            EditorGUILayout.PropertyField( decelerationProp );
            EditorGUILayout.PropertyField( bounceProp );
            EditorGUILayout.PropertyField( bounceBackDurationProp );

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
