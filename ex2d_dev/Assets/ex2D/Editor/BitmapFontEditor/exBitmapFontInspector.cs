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
class exBitmapFontInspector : Editor {

#if !(EX2D_EVALUATE)

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {
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

#else // !(EX2D_EVALUATE)

	public override void OnInspectorGUI () {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        GUILayout.Label( "Unavailable in Evaluate Version", style );
    }

#endif // !(EX2D_EVALUATE)

}
