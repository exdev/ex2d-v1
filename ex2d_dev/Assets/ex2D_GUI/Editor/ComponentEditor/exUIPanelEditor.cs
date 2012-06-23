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

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();

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

            exUIPanel curEdit = target as exUIPanel;

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
            MessageInfoListField ( "On Pointer Move", curEdit.moveSlots );


        serializedObject.ApplyModifiedProperties ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSceneGUI () {
        base.OnSceneGUI();

        serializedObject.Update ();
            exUIPanel curEdit = target as exUIPanel;

            if ( curEdit.anchor != curEdit.background.anchor ) {
                curEdit.background.anchor = curEdit.anchor;
                EditorUtility.SetDirty(curEdit.background);
                HandleUtility.Repaint(); 
            }
        serializedObject.ApplyModifiedProperties ();
    }
}
