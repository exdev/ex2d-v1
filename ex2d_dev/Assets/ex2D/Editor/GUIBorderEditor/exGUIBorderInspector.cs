// ======================================================================================
// File         : exGUIBorderInspector.cs
// Author       : Wu Jie 
// Last Change  : 09/20/2011 | 15:14:03 PM | Tuesday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// exGUIBorderInspector
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exGUIBorder))]
class exGUIBorderInspector : Editor {

#if !(EX2D_EVALUATE)

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {
        DrawDefaultInspector(); 

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
            if ( GUILayout.Button("Edit...", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                exGUIBorderEditor editor = exGUIBorderEditor.NewWindow();
                editor.Edit(target);
            }
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
    }

#else // !(EX2D_EVALUATE)

	public override void OnInspectorGUI () {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        GUILayout.Label( "Unavailable in Evaluate Version", style );
    }

#endif // !(EX2D_EVALUATE)

}
