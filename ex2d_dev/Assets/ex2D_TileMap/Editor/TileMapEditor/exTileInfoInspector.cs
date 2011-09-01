// ======================================================================================
// File         : exTileInfoInspector.cs
// Author       : Wu Jie 
// Last Change  : 08/30/2011 | 14:46:17 PM | Tuesday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exTileInfo))]
public class exTileInfoInspector : Editor {

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        DrawDefaultInspector(); 

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
            if ( GUILayout.Button("Edit...", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                exTileInfoEditor editor = exTileInfoEditor.NewWindow();
                editor.Edit(target);
            }
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
    }

}
