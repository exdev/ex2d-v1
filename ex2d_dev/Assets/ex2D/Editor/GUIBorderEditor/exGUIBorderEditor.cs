// ======================================================================================
// File         : exGUIBorderEditor.cs
// Author       : Wu Jie 
// Last Change  : 09/26/2011 | 13:57:00 PM | Monday,September
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
/// GUIBorder editor 
///
///////////////////////////////////////////////////////////////////////////////

partial class exGUIBorderEditor : EditorWindow {

    ///////////////////////////////////////////////////////////////////////////////
    // private variables
    ///////////////////////////////////////////////////////////////////////////////

    private exGUIBorder curEdit;
    private Vector2 scrollPos = Vector2.zero;

    static int previewWidth = 200;
    static int previewHeight = 200;
    static bool showGrid = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \return the bitmap font editor 
    /// Open the atlas editor window
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/GUIBorder Editor %&g")]
    public static exGUIBorderEditor NewWindow () {
        exGUIBorderEditor newWindow = EditorWindow.GetWindow<exGUIBorderEditor>();
        return newWindow;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        name = "GUIBorder Editor";
        wantsMouseMove = true;
        autoRepaintOnSceneChange = false;
        // position = new Rect ( 50, 50, 800, 600 );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
        // TODO:
    }

    // ------------------------------------------------------------------ 
    /// \param _obj
    /// Check if the object is valid gui-border and open it in gui-border editor.
    // ------------------------------------------------------------------ 

    public void Edit ( Object _obj ) {
        // check if repaint
        if ( curEdit != _obj ) {

            // check if we have exGUIBorder in the same directory
            Object obj = _obj; 
            if ( obj != null ) {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if ( string.IsNullOrEmpty(assetPath) == false ) {
                    string dirname = Path.GetDirectoryName(assetPath);
                    string filename = Path.GetFileNameWithoutExtension(assetPath);
                    obj = (exGUIBorder)AssetDatabase.LoadAssetAtPath( Path.Combine( dirname, filename ) + ".asset",
                                                                      typeof(exGUIBorder) );
                }
                if ( obj == null ) {
                    obj = _obj;
                }
            }

            // if this is another bitmapfont, swtich to it.
            if ( obj is exGUIBorder && obj != curEdit ) {
                curEdit = obj as exGUIBorder;
                Init();

                Repaint ();
                return;
            }
        }
    }

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
            GUILayout.Label ( "Please select an GUI Border" );
            // DISABLE: this.ShowNotification( new GUIContent("Please select an Atlas Info"));
            return;
        }

        // ======================================================== 
        // 
        // ======================================================== 

        Rect lastRect = new Rect( 10, 0, 1, 1 );

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical( GUILayout.Width(350) );

            GUILayout.Space(5);

            // get ElementInfo first
            Texture2D editTexture = exEditorHelper.LoadAssetFromGUID<Texture2D>(curEdit.textureGUID); 
            lastRect = GUILayoutUtility.GetLastRect ();
            EditorGUI.indentLevel = 1;

            // ======================================================== 
            // GUI border
            // ======================================================== 

            Object newBorder = EditorGUILayout.ObjectField( "GUI Border"
                                                            , curEdit 
                                                            , typeof(exGUIBorder) 
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                            , false
#endif
                                                            , GUILayout.Width(300)
                                                          );
            if ( newBorder != curEdit )
                Selection.activeObject = newBorder;

            // ======================================================== 
            // texture field
            // ======================================================== 

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
                Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField( editTexture
                                                                               , typeof(Texture2D)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                               , false
#endif
                                                                               , GUILayout.Height(50)
                                                                               , GUILayout.Width(50)
                                                                             );
                EditorGUIUtility.LookLikeInspector ();
                if ( newTexture != editTexture ) {
                    editTexture = newTexture;
                    // DISABLE: some gui are good in bilinear filter (controls/window), 
                    //          some are good in point filter (controls/boxOver)
                    // editTexture.filterMode = FilterMode.Point;
                    curEdit.textureGUID = exEditorHelper.AssetToGUID(editTexture);
                    // curEdit.border = new RectOffset ( 0, 0, 0, 0 ); // NOTE: we have protect below
                    curEdit.editorNeedRebuild = true;
                }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField ( "Name", editTexture ?  editTexture.name : "None", GUILayout.Width(200) );
            EditorGUILayout.LabelField ( "Size", editTexture ?  editTexture.width + " x " + editTexture.height : "0 x 0 ", GUILayout.Width(200) );

            // ======================================================== 
            // texture preview
            // ======================================================== 

            GUILayout.Space(10);
            lastRect = GUILayoutUtility.GetLastRect ();
            TexturePreviewField ( new Rect( 30, lastRect.yMax, 100, 100 ), curEdit, editTexture );
            GUILayout.Space(10);

            // ======================================================== 
            // Rect Offset 
            // ======================================================== 

            GUI.enabled = editTexture != null;
            EditorGUIUtility.LookLikeControls ();
            GUILayout.BeginHorizontal();
                int newLeft = EditorGUILayout.IntField ( "Left", curEdit.border.left, GUILayout.Width(150) ); // left
                if ( editTexture ) {
                    newLeft = Mathf.Clamp( newLeft, 0, editTexture.width - curEdit.border.right );
                    if ( newLeft != curEdit.border.left ) {
                        curEdit.border.left = newLeft;
                        curEdit.editorNeedRebuild = true;
                    }
                    
                }

                int newRight = EditorGUILayout.IntField ( "Right", curEdit.border.right, GUILayout.Width(150) ); // right
                if ( editTexture ) {
                    newRight = Mathf.Clamp( newRight, 0, editTexture.width - curEdit.border.left );
                    if ( newRight != curEdit.border.right ) {
                        curEdit.border.right = newRight;
                        curEdit.editorNeedRebuild = true;
                    }
                }

                GUILayout.Label ( " = " + curEdit.border.horizontal );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
                int newTop = EditorGUILayout.IntField ( "Top", curEdit.border.top, GUILayout.Width(150) ); // top
                if ( editTexture ) {
                     newTop = Mathf.Clamp( newTop, 0, editTexture.height - curEdit.border.bottom );
                     if ( newTop != curEdit.border.top ) {
                        curEdit.border.top = newTop;
                        curEdit.editorNeedRebuild = true;
                     }
                     
                }

                int newBottom = EditorGUILayout.IntField ( "Bottom", curEdit.border.bottom, GUILayout.Width(150) ); // bottom
                if ( editTexture ) {
                    newBottom = Mathf.Clamp( newBottom, 0, editTexture.height - curEdit.border.top );
                    if ( newBottom != curEdit.border.bottom ) {
                        curEdit.border.bottom = newBottom;
                        curEdit.editorNeedRebuild = true;
                    }
                }

                GUILayout.Label ( " = " + curEdit.border.vertical );
            GUILayout.EndHorizontal();
            EditorGUIUtility.LookLikeInspector ();
            GUI.enabled = true;

            EditorGUILayout.LabelField ( "Center", editTexture ? 
                                         (editTexture.width - curEdit.border.horizontal) + " x " + (editTexture.height - curEdit.border.vertical)
                                         : "0 x 0", 
                                         GUILayout.Width(200) );

            // ======================================================== 
            // save button
            // ======================================================== 

            GUI.enabled = curEdit.editorNeedRebuild;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
                if ( GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                    AssetDatabase.SaveAssets();
                }
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUI.enabled = true;

        GUILayout.EndVertical();

            GUILayout.Space (10);
            lastRect = GUILayoutUtility.GetLastRect ();

            // ======================================================== 
            // draw vertical split line 
            // ======================================================== 

            exEditorHelper.DrawLine( new Vector2 ( lastRect.xMax, 0 ),
                                     new Vector2 ( lastRect.xMax, position.height ),
                                     Color.black,
                                     1.0f );

            GUILayout.BeginVertical();

                // ======================================================== 
                // toolbar 
                // ======================================================== 

                GUILayout.BeginHorizontal ( EditorStyles.toolbar, GUILayout.MaxWidth(position.width - 350) );

                    // ======================================================== 
                    // Show Grid 
                    // ======================================================== 

                    showGrid = GUILayout.Toggle ( showGrid, "Show Grid", EditorStyles.toolbarButton );

                    // ======================================================== 
                    // Preview Width and Height 
                    // ======================================================== 

                    EditorGUIUtility.LookLikeControls ();
                    GUILayout.BeginHorizontal();
                        previewWidth = Mathf.Max ( curEdit.border.horizontal, EditorGUILayout.IntField ( "Width", previewWidth, EditorStyles.toolbarTextField ) ); // preview width
                        previewHeight = Mathf.Max ( curEdit.border.vertical, EditorGUILayout.IntField ( "Height", previewHeight, EditorStyles.toolbarTextField ) ); // preview height
                    GUILayout.EndHorizontal();
                    EditorGUIUtility.LookLikeInspector ();

                    GUILayout.FlexibleSpace();

                    // ======================================================== 
                    // Reset Width & Height
                    // ======================================================== 

                    if ( GUILayout.Button( "Reset", EditorStyles.toolbarButton ) ) {
                        previewWidth = 200;
                        previewHeight = 200;
                    }

                    // ======================================================== 
                    // Help
                    // ======================================================== 

                    if ( GUILayout.Button( exEditorHelper.HelpTexture(), EditorStyles.toolbarButton ) ) {
                        Help.BrowseURL("http://www.ex-dev.com/ex2d/wiki/doku.php?id=manual:gui_border_editor");
                    }

                GUILayout.EndHorizontal ();

                // ======================================================== 
                // BorderPreviewField 
                // ======================================================== 

                float toolbarHeight = EditorStyles.toolbar.CalcHeight( new GUIContent(""), 0 );
                Rect previewArea = new Rect ( lastRect.xMax,
                                              toolbarHeight, 
                                              position.width - lastRect.xMax,
                                              position.height - toolbarHeight );

                //
                scrollPos = GUI.BeginScrollView(  previewArea, 
                                                  scrollPos, 
                                                  new Rect( -10.0f, 
                                                            -10.0f, 
                                                            (previewArea.width - previewWidth)/2.0f + previewWidth + 20.0f, 
                                                            (previewArea.height - previewHeight)/2.0f + previewHeight + 20.0f )  
                                               );
                    BorderPreviewField ( new Rect ( (previewArea.width - previewWidth) / 2.0f, 
                                                    (previewArea.height - previewHeight) / 2.0f, 
                                                    previewWidth,
                                                    previewHeight ), 
                                         curEdit, 
                                         editTexture );
                GUI.EndScrollView();
            GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        // ======================================================== 
        // dirty
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void TexturePreviewField ( Rect _rect, exGUIBorder _guiBorder, Texture2D _texture ) {

        if ( _texture == null )
            return;

        float widthRatio = _rect.width/_texture.width;
        float heightRatio = _rect.height/_texture.height;
        GUI.BeginGroup(_rect);
            GUI.DrawTexture( new Rect( 0, 0, _rect.width, _rect.height), _texture );
            exEditorHelper.DrawRect( new Rect( 0, 0, _rect.width, _rect.height), new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.gray );
            // top line
            exEditorHelper.DrawLine ( new Vector2( 0, _guiBorder.border.top * heightRatio ),
                                      new Vector2( _rect.width, _guiBorder.border.top * heightRatio ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );

            // bottom line
            exEditorHelper.DrawLine ( new Vector2( 0, (_texture.height - _guiBorder.border.bottom) * heightRatio ),
                                      new Vector2( _rect.width, (_texture.height - _guiBorder.border.bottom) * heightRatio ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );

            // left line
            exEditorHelper.DrawLine ( new Vector2( _guiBorder.border.left * widthRatio, 0 ),
                                      new Vector2( _guiBorder.border.left * widthRatio, _rect.height ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );

            // right line
            exEditorHelper.DrawLine ( new Vector2( (_texture.width - _guiBorder.border.right) * widthRatio, 0 ),
                                      new Vector2( (_texture.width - _guiBorder.border.right) * widthRatio, _rect.height ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );
        GUI.EndGroup();
        GUILayoutUtility.GetRect ( _rect.width, _rect.height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void BorderPreviewField ( Rect _rect, exGUIBorder _guiBorder, Texture2D _texture ) {

        if ( _texture == null )
            return;

        float curX = _rect.x;
        float curY = _rect.y;
        int texCenterWidth = _texture.width - _guiBorder.border.horizontal;
        int texCenterHeight = _texture.height - _guiBorder.border.vertical;
        int previewCenterWidth = (int)_rect.width - _guiBorder.border.horizontal;
        int previewCenterHeight = (int)_rect.height - _guiBorder.border.vertical;

        float widthRatio = (float)previewCenterWidth/(texCenterWidth != 0 ? (float)texCenterWidth : 0.1f);
        float heightRatio = (float)previewCenterHeight/(texCenterHeight != 0 ? (float)texCenterHeight : 0.1f);

        // ======================================================== 
        // top 
        // ======================================================== 

        // left-top
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.left, 
                              _guiBorder.border.top ), 
                   new Rect ( 0, 0, 1, 1 ),
                   _texture );
        
        // mid-top
        curX += _guiBorder.border.left;
        DrawGrid ( new Rect ( curX, curY, 
                              previewCenterWidth,
                              _guiBorder.border.top ), 
                   new Rect ( _guiBorder.border.left, 
                              0, 
                              widthRatio, 
                              1 ),
                   _texture );

        // right-top
        curX += previewCenterWidth;
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.right, 
                              _guiBorder.border.top ), 
                   new Rect ( _texture.width - _guiBorder.border.right, 
                              0, 
                              1, 
                              1 ),
                   _texture );

        // ======================================================== 
        curX = _rect.x;
        curY += _guiBorder.border.top;
        // center 
        // ======================================================== 

        // left-center
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.left, 
                              previewCenterHeight ), 
                   new Rect ( 0, 
                              _guiBorder.border.top, 
                              1, 
                              heightRatio ),
                   _texture );
        
        curX += _guiBorder.border.left;
        // mid-center
        DrawGrid ( new Rect ( curX, curY, 
                              previewCenterWidth, 
                              previewCenterHeight ), 
                   new Rect ( _guiBorder.border.left, 
                              _guiBorder.border.top, 
                              widthRatio, 
                              heightRatio ),
                   _texture );

        // right-center
        curX += previewCenterWidth;
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.right, 
                              previewCenterHeight ), 
                   new Rect ( _texture.width - _guiBorder.border.right, 
                              _guiBorder.border.top, 
                              1, 
                              heightRatio ),
                   _texture );

        // ======================================================== 
        curX = _rect.x;
        curY += previewCenterHeight;
        // bottom 
        // ======================================================== 

        // left-bottom
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.left, 
                              _guiBorder.border.bottom ), 
                   new Rect ( 0, 
                              _texture.height - _guiBorder.border.bottom, 
                              1, 
                              1 ),
                   _texture );
        
        // mid-bottom
        curX += _guiBorder.border.left;
        DrawGrid ( new Rect ( curX, curY, 
                              previewCenterWidth,
                              _guiBorder.border.bottom ), 
                   new Rect ( _guiBorder.border.left, 
                              _texture.height - _guiBorder.border.bottom, 
                              widthRatio, 
                              1 ),
                   _texture );

        // right-bottom
        curX += previewCenterWidth;
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.right, 
                              _guiBorder.border.bottom ), 
                   new Rect ( _texture.width - _guiBorder.border.right, 
                              _texture.height - _guiBorder.border.bottom, 
                              1, 
                              1 ),
                   _texture );

        //
        GUILayoutUtility.GetRect ( _rect.width, _rect.height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void DrawGrid ( Rect _dest, Rect _src, Texture2D _texture ) {

        GUI.BeginGroup( _dest );
            GUI.DrawTexture( new Rect( -_src.x * _src.width,
                                       -_src.y * _src.height,
                                       _texture.width  * _src.width,
                                       _texture.height * _src.height ), 
                             _texture );
        GUI.EndGroup();

        if ( showGrid ) {
            exEditorHelper.DrawRect( _dest, 
                                     new Color(0.0f, 0.0f, 0.0f, 0.0f), 
                                     new Color( 1.0f, 0.0f, 1.0f, 1.0f) );
        }

    }
}
