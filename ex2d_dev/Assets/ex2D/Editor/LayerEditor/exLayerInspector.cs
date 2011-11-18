// ======================================================================================
// File         : exLayerInspector.cs
// Author       : Wu Jie 
// Last Change  : 11/09/2011 | 11:31:28 AM | Wednesday,November
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

[CustomEditor(typeof(exLayer))]
class exLayerInspector : Editor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        // DrawDefaultInspector(); 
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
            if ( GUILayout.Button("Edit...", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                exLayerEditor editor = exLayerEditor.NewWindow();
                editor.Edit((target as exLayer).gameObject);
            }
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
    }
}

