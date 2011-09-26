// ======================================================================================
// File         : exBitmapFontEditor.cs
// Author       : Wu Jie 
// Last Change  : 07/15/2011 | 13:50:38 PM | Friday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
///
/// Bitmap font editor 
///
///////////////////////////////////////////////////////////////////////////////

partial class exBitmapFontEditor : EditorWindow {

    ///////////////////////////////////////////////////////////////////////////////
    // private variables
    ///////////////////////////////////////////////////////////////////////////////

    private exBitmapFont curEdit;
    private Object curFontInfo;

    private Vector2 scrollPos = Vector2.zero;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \return the bitmap font editor 
    /// Open the atlas editor window
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/BitmapFont Editor %&f")]
    public static exBitmapFontEditor NewWindow () {
        exBitmapFontEditor newWindow = EditorWindow.GetWindow<exBitmapFontEditor>();
        return newWindow;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        name = "BitmapFont Editor";
        wantsMouseMove = true;
        autoRepaintOnSceneChange = false;
        // position = new Rect ( 50, 50, 800, 600 );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
        curFontInfo = null;
        if ( curEdit ) {
            string path = AssetDatabase.GetAssetPath(curEdit);
            string fontInfoPath = Path.Combine ( Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".txt" ); 
            curFontInfo = AssetDatabase.LoadAssetAtPath( fontInfoPath, typeof(Object) );
            if ( curFontInfo == null ) {
                fontInfoPath = Path.Combine ( Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".fnt" ); 
                curFontInfo = AssetDatabase.LoadAssetAtPath( fontInfoPath, typeof(Object) );
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _obj
    /// Check if the object is valid bitmap font and open it in bitmap font editor.
    // ------------------------------------------------------------------ 

    public void Edit ( Object _obj ) {
        // check if repaint
        if ( curEdit != _obj ) {

            // check if we have exBitmapFont in the same directory
            Object obj = _obj; 
            if ( obj != null ) {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if ( string.IsNullOrEmpty(assetPath) == false ) {
                    string dirname = Path.GetDirectoryName(assetPath);
                    string filename = Path.GetFileNameWithoutExtension(assetPath);
                    obj = (exBitmapFont)AssetDatabase.LoadAssetAtPath( Path.Combine( dirname, filename ) + ".asset",
                                                                       typeof(exBitmapFont) );
                }
                if ( obj == null ) {
                    obj = _obj;
                }
            }

            // if this is another bitmapfont, swtich to it.
            if ( obj is exBitmapFont && obj != curEdit ) {
                curEdit = obj as exBitmapFont;
                Init();

                Repaint ();
                return;
            }
        }
    }

    // DISABLE: the focus only occur when main window lost foucs, then come in { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void OnFocus () {
    //     OnSelectionChange ();
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSelectionChange () {
        Edit (Selection.activeObject);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        EditorGUI.indentLevel = 0;

        if ( curEdit == null ) {
            GUILayout.Space(10);
            GUILayout.Label ( "Please select an BitmapFont asset" );
            return;
        }

        // ======================================================== 
        // toolbar 
        // ======================================================== 

        GUILayout.BeginHorizontal ( EditorStyles.toolbar );
            GUILayout.FlexibleSpace();

            // ======================================================== 
            // Build 
            // ======================================================== 

            GUI.enabled = curEdit.editorNeedRebuild;
            if ( GUILayout.Button( "Build", EditorStyles.toolbarButton, GUILayout.Width(80) ) ) {
                curEdit.Build( curFontInfo );
            }
            GUI.enabled = true;

            // ======================================================== 
            // Help
            // ======================================================== 

            if ( GUILayout.Button( exEditorHelper.HelpTexture(), EditorStyles.toolbarButton ) ) {
                Help.BrowseURL("http://www.ex-dev.com/ex2d/wiki/doku.php?id=manual:font_editor");
            }
        GUILayout.EndHorizontal ();

        GUILayout.Space(5);

        // ======================================================== 
        // if we have curEdit
        // ======================================================== 

        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, 
                                                      GUILayout.Width(position.width),
                                                      GUILayout.Height(position.height) );


        // ======================================================== 
        // font info 
        // ======================================================== 

        Object newFontInfo = EditorGUILayout.ObjectField( "Font Info"
                                                          , curFontInfo
                                                          , typeof(Object)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                          , false
#endif
                                                          , GUILayout.Width(300) 
                                                        );
        if ( newFontInfo != curFontInfo ) {
            curFontInfo = newFontInfo;
            curEdit.editorNeedRebuild = true;
        }

        // ======================================================== 
        // page info
        // ======================================================== 

        GUI.enabled = false;
        foreach ( exBitmapFont.PageInfo pi in curEdit.pageInfos ) {
            EditorGUILayout.ObjectField( pi.texture.name
                                         , pi.texture
                                         , typeof(Texture2D)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                         , false
#endif
                                         , GUILayout.Width(50)
                                         , GUILayout.Height(50) 
                                       );
        }
        GUI.enabled = true;

        EditorGUILayout.EndScrollView();
    }
}
