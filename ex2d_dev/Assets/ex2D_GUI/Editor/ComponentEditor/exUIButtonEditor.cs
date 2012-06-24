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

    SerializedProperty hoverInSlotsProp;
    SerializedProperty hoverOutSlotsProp;
    SerializedProperty pressSlotsProp;
    SerializedProperty releaseSlotsProp;
    SerializedProperty clickSlotsProp;

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

        hoverInSlotsProp = serializedObject.FindProperty ("hoverInSlots");
        hoverOutSlotsProp = serializedObject.FindProperty ("hoverOutSlots");
        pressSlotsProp = serializedObject.FindProperty ("pressSlots");
        releaseSlotsProp = serializedObject.FindProperty ("releaseSlots");
        clickSlotsProp = serializedObject.FindProperty ("clickSlots");
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

            //
            EditorGUILayout.PropertyField( textProp, new GUIContent("Text") );
            EditorGUILayout.PropertyField( fontProp );
            EditorGUILayout.PropertyField( backgroundProp );


            // message infos
            EditorGUILayout.Space();
            MessageInfoListField ( "On Hover In", hoverInSlotsProp );

            EditorGUILayout.Space();
            MessageInfoListField ( "On Hover Out", hoverOutSlotsProp );

            EditorGUILayout.Space();
            MessageInfoListField ( "On Press", pressSlotsProp );

            EditorGUILayout.Space();
            MessageInfoListField ( "On Rlease", releaseSlotsProp );

            EditorGUILayout.Space();
            MessageInfoListField ( "On Click", clickSlotsProp );


        serializedObject.ApplyModifiedProperties ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSceneGUI () {
        base.OnSceneGUI();

        serializedObject.Update ();
            exUIButton curEdit = target as exUIButton;

            if ( curEdit.font ) {
                if ( curEdit.font.text != textProp.stringValue ) {
                    curEdit.font.text = textProp.stringValue;
                    EditorUtility.SetDirty(curEdit.font);
                    HandleUtility.Repaint(); 
                }
            }

            if ( curEdit.background ) {
                if ( curEdit.background.anchor != curEdit.anchor ) {
                    curEdit.background.anchor = curEdit.anchor;
                    EditorUtility.SetDirty(curEdit.background);
                    HandleUtility.Repaint(); 
                }
            }
        serializedObject.ApplyModifiedProperties ();
    }
}
