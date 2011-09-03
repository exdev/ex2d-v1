// ======================================================================================
// File         : exSpriteAnimClipInspector.cs
// Author       : Wu Jie 
// Last Change  : 07/27/2011 | 10:05:43 AM | Wednesday,July
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

[CustomEditor(typeof(exSpriteAnimClip))]
public class exSpriteAnimClipInspector : Editor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        DrawDefaultInspector(); 

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
            if ( GUILayout.Button("Edit...", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                exSpriteAnimClipEditor editor = exSpriteAnimClipEditor.NewWindow();
                editor.Edit(target);
            }
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
    }
}
