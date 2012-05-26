// ======================================================================================
// File         : exSpriteAnimationUtility.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 22:22:16 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
///
/// the GUI border utility
///
///////////////////////////////////////////////////////////////////////////////

public static class exGUIBorderUtility {

#if !(EX2D_EVALUATE)

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D GUI Border")]
    static void Create () {
        string assetPath = exEditorHelper.GetCurrentDirectory();
        string assetName = "New GUIBorder";

        bool doCreate = true;
        string path = Path.Combine( assetPath, assetName + ".asset" );
        FileInfo fileInfo = new FileInfo(path);
        if ( fileInfo.Exists ) {
            doCreate = EditorUtility.DisplayDialog( assetName + " already exists.",
                                                    "Do you want to overwrite the old one?",
                                                    "Yes", "No" );
        }
        if ( doCreate ) {
            exGUIBorder border = exGUIBorderUtility.Create ( assetPath, assetName );
            Selection.activeObject = border;
            // EditorGUIUtility.PingObject(border);
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _path the directory path to save the atlas
    /// \param _name the name of the atlas
    /// \return the gui border
    /// create the gui border in the _path, save it as _name.
    // ------------------------------------------------------------------ 

    public static exGUIBorder Create ( string _path, string _name ) {
        //
        if ( new DirectoryInfo(_path).Exists == false ) {
            Debug.LogError ( "can't create asset, path not found" );
            return null;
        }
        if ( string.IsNullOrEmpty(_name) ) {
            Debug.LogError ( "can't create asset, the name is empty" );
            return null;
        }
        string assetPath = Path.Combine( _path, _name + ".asset" );

        //
        exGUIBorder newGUIBorder = ScriptableObject.CreateInstance<exGUIBorder>();
        AssetDatabase.CreateAsset(newGUIBorder, assetPath);
        Selection.activeObject = newGUIBorder;
        return newGUIBorder;
    }

#endif // !(EX2D_EVALUATE)

}
