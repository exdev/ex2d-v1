// ======================================================================================
// File         : exBitmapFontInspector.cs
// Author       : Wu Jie 
// Last Change  : 08/11/2011 | 16:09:23 PM | Thursday,August
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

[CustomEditor(typeof(exBitmapFont))]
public class exBitmapFontInspector : Editor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        DrawDefaultInspector(); 

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
            if ( GUILayout.Button("Edit...", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                exBitmapFontEditor editor = exBitmapFontEditor.NewWindow();
                editor.Edit(target);
            }
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
    }
}
