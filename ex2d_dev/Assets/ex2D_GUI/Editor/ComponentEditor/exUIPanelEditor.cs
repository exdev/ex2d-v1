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

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exUIPanel editPanel;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();
        if ( target != editPanel ) {
            editPanel = target as exUIPanel;
        }
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
        // Updates 
        // ======================================================== 

        // update button
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            if ( GUILayout.Button("Update", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                editPanel.background = editPanel.transform.Find("Background").GetComponent<exSpriteBorder>();
                GUI.changed = true;
            }
        GUILayout.EndHorizontal();

        EditorGUI.indentLevel = 2;
        GUI.enabled = false;
        EditorGUILayout.ObjectField( "Background"
                                     , editPanel.background
                                     , typeof(exSpriteBorder)
                                     , false 
                                   );
        GUI.enabled = true;
        EditorGUI.indentLevel = 1;

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( EditorApplication.isPlaying == false )
            editPanel.Sync();

        if ( GUI.changed )
            EditorUtility.SetDirty (editPanel);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSceneGUI () {
        base.OnSceneGUI();

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( EditorApplication.isPlaying == false )
            editPanel.Sync();

        if ( GUI.changed )
            EditorUtility.SetDirty (editPanel);
    }
}
