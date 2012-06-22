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
using System.Collections.Generic;

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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void MessageInfoListField ( string _label, List<exUIElement.MessageInfo> _infoList ) {
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.LabelField( _label );
            GUILayout.FlexibleSpace();
            if ( GUILayout.Button( "+", GUILayout.Width(20) ) ) {
                exUIElement.MessageInfo msgInfo = new exUIElement.MessageInfo();
                msgInfo.receiver = (target as exUIButton).gameObject;
                _infoList.Add ( msgInfo );
            }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        for ( int i = 0; i < _infoList.Count; ++i ) {
            if ( MessageInfoField ( "[" + i + "]", _infoList[i] ) ) {
                _infoList.RemoveAt(i);
                --i;
            }
        }
    }
}
