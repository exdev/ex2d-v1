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
    SerializedProperty borderProp;
    SerializedProperty fontProp;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();

        textProp = serializedObject.FindProperty ("text_");
        borderProp = serializedObject.FindProperty ("border");
        fontProp = serializedObject.FindProperty ("font");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {
        exUIButton curEdit = target as exUIButton;

        // ======================================================== 
        // Base GUI 
        // ======================================================== 

        base.OnInspectorGUI();
        GUILayout.Space(20);

        EditorGUILayout.PropertyField( textProp, new GUIContent("Text") );
        curEdit.text = textProp.stringValue;

        // // ======================================================== 
        // // Updates 
        // // ======================================================== 

        // // update button
        // GUILayout.BeginHorizontal();
        // GUILayout.Space(15);
        //     if ( GUILayout.Button("Update", GUILayout.Width(50), GUILayout.Height(20) ) ) {
        //         editButton.border = editButton.transform.Find("Border").GetComponent<exSpriteBorder>();
        //         editButton.font = editButton.transform.Find("Border/Text").GetComponent<exSpriteFont>();
        //         GUI.changed = true;
        //     }
        // GUILayout.EndHorizontal();

        // EditorGUI.indentLevel = 2;
        // GUI.enabled = false;
        // EditorGUILayout.ObjectField( "Border"
        //                              , editButton.border
        //                              , typeof(exSpriteBorder)
        //                              , false 
        //                            );
        // EditorGUILayout.ObjectField( "Font"
        //                              , editButton.font
        //                              , typeof(exSpriteFont)
        //                              , false 
        //                            );
        // GUI.enabled = true;
        // EditorGUI.indentLevel = 1;

        // // ======================================================== 
        // // check dirty 
        // // ======================================================== 

        // if ( EditorApplication.isPlaying == false )
        //     editButton.Sync();

        // if ( GUI.changed )
        //     EditorUtility.SetDirty (editButton);

        serializedObject.ApplyModifiedProperties ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSceneGUI () {
        base.OnSceneGUI();

        // // ======================================================== 
        // // check dirty 
        // // ======================================================== 

        // if ( EditorApplication.isPlaying == false )
        //     editButton.Sync();

        // if ( GUI.changed )
        //     EditorUtility.SetDirty (editButton);
    }
}
