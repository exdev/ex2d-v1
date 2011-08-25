// ======================================================================================
// File         : exEditorRuntimeHelper.cs
// Author       : Wu Jie 
// Last Change  : 08/22/2011 | 12:30:34 PM | Monday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class exEditorRuntimeHelper {

#if UNITY_EDITOR
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static string GetCurrentDirectory () {
        if ( Selection.activeObject ) {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if ( Path.GetExtension(path) != "" ) {
                path = Path.GetDirectoryName(path);
            }
            return path;
        }
        return "Assets";
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static string GetPathGUID ( Object _o ) {
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_o));
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static Object LoadAssetFromGUID ( string _guid, System.Type _type ) {
        string texturePath = AssetDatabase.GUIDToAssetPath(_guid);
        return AssetDatabase.LoadAssetAtPath( texturePath, _type);
    }
#endif
}
