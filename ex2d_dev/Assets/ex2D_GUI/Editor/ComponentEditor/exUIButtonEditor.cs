// ======================================================================================
// File         : exUIButtonEditor.cs
// Author       : Wu Jie 
// Last Change  : 11/03/2011 | 16:51:40 PM | Thursday,November
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

[CustomEditor(typeof(exUIButton))]
public class exUIButtonEditor : exUIElementEditor {

    SerializedProperty textProp;
    SerializedProperty fontProp;
    SerializedProperty backgroundProp;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();

        textProp = serializedObject.FindProperty ("text_");
        fontProp = serializedObject.FindProperty ("font");
        backgroundProp = serializedObject.FindProperty ("background");
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

            exUIButton curEdit = target as exUIButton;

            EditorGUILayout.PropertyField( textProp, new GUIContent("Text") );
            EditorGUILayout.PropertyField( fontProp );
            EditorGUILayout.PropertyField( backgroundProp );


            // message infos
            EditorGUILayout.Space();
            MessageInfoListField ( "On Hover In", curEdit.hoverInSlots );

            EditorGUILayout.Space();
            MessageInfoListField ( "On Hover Out", curEdit.hoverOutSlots );

            EditorGUILayout.Space();
            MessageInfoListField ( "On Press", curEdit.pressSlots );

            EditorGUILayout.Space();
            MessageInfoListField ( "On Rlease", curEdit.releaseSlots );

            EditorGUILayout.Space();
            MessageInfoListField ( "On Click", curEdit.clickSlots );


        serializedObject.ApplyModifiedProperties ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSceneGUI () {
        base.OnSceneGUI();

        serializedObject.Update ();
            exUIButton curEdit = target as exUIButton;

            if ( curEdit.font.text != textProp.stringValue ) {
                curEdit.font.text = textProp.stringValue;
                EditorUtility.SetDirty(curEdit.font);
                HandleUtility.Repaint(); 
            }

            if ( curEdit.anchor != curEdit.background.anchor ) {
                curEdit.background.anchor = curEdit.anchor;
                EditorUtility.SetDirty(curEdit.background);
                HandleUtility.Repaint(); 
            }
        serializedObject.ApplyModifiedProperties ();
    }
}
