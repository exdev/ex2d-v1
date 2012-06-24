// ======================================================================================
// File         : exUIPanelEditor.cs
// Author       : Wu Jie 
// Last Change  : 11/03/2011 | 17:53:14 PM | Thursday,November
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

[CustomEditor(typeof(exUIPanel))]
public class exUIPanelEditor : exUIElementEditor {

    SerializedProperty backgroundProp;

    SerializedProperty hoverInSlotsProp;
    SerializedProperty hoverOutSlotsProp;
    SerializedProperty pressSlotsProp;
    SerializedProperty releaseSlotsProp;
    SerializedProperty moveSlotsProp;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();

        backgroundProp = serializedObject.FindProperty ("background");

        hoverInSlotsProp = serializedObject.FindProperty ("hoverInSlots");
        hoverOutSlotsProp = serializedObject.FindProperty ("hoverOutSlots");
        pressSlotsProp = serializedObject.FindProperty ("pressSlots");
        releaseSlotsProp = serializedObject.FindProperty ("releaseSlots");
        moveSlotsProp = serializedObject.FindProperty ("moveSlots");
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
            MessageInfoListField ( "On Pointer Move", moveSlotsProp );


        serializedObject.ApplyModifiedProperties ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSceneGUI () {
        base.OnSceneGUI();

        serializedObject.Update ();
            exUIPanel curEdit = target as exUIPanel;

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
