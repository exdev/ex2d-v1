// ======================================================================================
// File         : exLayerMngInspector.cs
// Author       : Wu Jie 
// Last Change  : 11/09/2011 | 11:33:13 AM | Wednesday,November
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
// exAtlasInspector
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exLayerMng))]
class exLayerMngInspector : Editor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        DrawDefaultInspector(); 

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
            if ( GUILayout.Button("Edit...", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                exLayerEditor editor = exLayerEditor.NewWindow();
                editor.Edit((target as exLayerMng).gameObject);
            }
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
    }
}
