// ======================================================================================
// File         : exTileSheetEditor.cs
// Author       : Wu Jie 
// Last Change  : 09/16/2011 | 10:15:36 AM | Friday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// exTileSheetEditor
///////////////////////////////////////////////////////////////////////////////

partial class exTileSheetEditor : EditorWindow {

    ///////////////////////////////////////////////////////////////////////////////
    // private data
    ///////////////////////////////////////////////////////////////////////////////

    private exTileSheet curEdit;
    private Vector2 scrollPos = Vector2.zero;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/TileSheet Editor %&t")]
    public static exTileSheetEditor NewWindow () {
        exTileSheetEditor newWindow = EditorWindow.GetWindow<exTileSheetEditor>();
        return newWindow;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        name = "TileSheet Editor";
        wantsMouseMove = true;
        autoRepaintOnSceneChange = true;
        // position = new Rect ( 50, 50, 800, 600 );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Edit ( Object _obj ) {
        // check if repaint
        if ( curEdit != _obj ) {

            // check if we have atlas - editorinfo in the same directory
            Object obj = _obj; 
            if ( obj is exTileSheet || obj is Texture2D || obj is Material ) {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string dirname = Path.GetDirectoryName(assetPath);
                string filename = Path.GetFileNameWithoutExtension(assetPath);
                obj = (exTileSheet)AssetDatabase.LoadAssetAtPath( Path.Combine( dirname, filename + ".asset" ),
                                                                typeof(exTileSheet) );
                if ( obj == null ) {
                    obj = _obj;
                }
            }

            // if this is another atlas, swtich to it.
            if ( obj is exTileSheet && obj != curEdit ) {
                curEdit = obj as exTileSheet;
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
        Edit ( Selection.activeObject );
        Repaint ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        EditorGUI.indentLevel = 0;

        if ( curEdit == null ) {
            GUILayout.Space(10);
            GUILayout.Label ( "Please select a Tile Sheet" );
            return;
        }

        // ======================================================== 
        // toolbar 
        // ======================================================== 

        GUILayout.BeginHorizontal ( EditorStyles.toolbar );
            GUILayout.FlexibleSpace();

            // ======================================================== 
            // Help
            // ======================================================== 

            if ( GUILayout.Button( exEditorHelper.HelpTexture(), EditorStyles.toolbarButton ) ) {
                Help.BrowseURL("http://www.ex-dev.com/ex2d/wiki/doku.php?id=manual:tilesheet_editor_guide");
            }

        GUILayout.EndHorizontal ();

        // ======================================================== 
        // if we have curEdit
        // ======================================================== 

        Rect lastRect = new Rect( 10, 0, 1, 1 );
        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, 
                                                      GUILayout.Width(position.width),
                                                      GUILayout.Height(position.height) );

        // draw label
        GUILayout.Space(10);
        lastRect = GUILayoutUtility.GetLastRect ();  

        // ======================================================== 
        // settings area 
        // ======================================================== 

        GUILayout.BeginVertical( GUILayout.MaxWidth(200) );

            // ======================================================== 
            // texture 
            // ======================================================== 

            Texture2D newTexture
                = (Texture2D)EditorGUILayout.ObjectField( "Texture"
                                                          , curEdit.texture
                                                          , typeof(Texture2D)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                          , false
#endif
                                                          , GUILayout.Width(100) 
                                                          , GUILayout.Height(100) 
                                                        );
            if ( curEdit.texture != newTexture ) {
                curEdit.texture = newTexture;

                // if we have texture but no material, try to find it
                if ( curEdit.texture != null ) {
                    // assign it
                    curEdit.material = exEditorHelper.GetDefaultMaterial(curEdit.texture);
                }
                // if we don't have texture, set material to null
                else {
                    curEdit.material = null;
                }
            }

            // ======================================================== 
            // material 
            // ======================================================== 

            curEdit.material 
                = (Material)EditorGUILayout.ObjectField( "Material" 
                                                         , curEdit.material
                                                         , typeof(Material)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                         , false 
#endif
                                                       );

            // ======================================================== 
            // tile width 
            // ======================================================== 

            curEdit.tileWidth = Mathf.Max ( 1, EditorGUILayout.IntField ( "Tile Width", curEdit.tileWidth ) ); 

            // ======================================================== 
            // tile height 
            // ======================================================== 

            curEdit.tileHeight = Mathf.Max( 1, EditorGUILayout.IntField ( "Tile Height", curEdit.tileHeight ) ); 

            // ======================================================== 
            // padding
            // ======================================================== 

            curEdit.padding = Mathf.Max( 0, EditorGUILayout.IntField ( "Padding", curEdit.padding ) ); 

            // ======================================================== 
            // row and col  
            // ======================================================== 

            EditorGUILayout.LabelField ( "Column x Row", curEdit.col + " x " + curEdit.row ); 

        GUILayout.EndVertical();

        // ======================================================== 
        // spac 
        // ======================================================== 

        GUILayout.Space(20);
        lastRect = GUILayoutUtility.GetLastRect ();  

        // ======================================================== 
        // tile sheet texture
        // ======================================================== 

        GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            lastRect = GUILayoutUtility.GetLastRect ();  
            PreviewTileSheet ( lastRect.xMax, lastRect.yMax, curEdit );
        GUILayout.EndHorizontal();

        // TODO: process evenet { 
        // // ======================================================== 
        // Event e = Event.current;
        // // ======================================================== 
        // } TODO end 

        EditorGUILayout.EndScrollView();

        // ======================================================== 
        // check dirty
        // ======================================================== 

        if ( GUI.changed ) {
            int col = 0;
            int row = 0;
            int uvX = curEdit.padding;
            int uvY = curEdit.padding;

            // count the col
            while ( (uvX + curEdit.tileWidth + curEdit.padding) <= curEdit.texture.width ) {
                uvX = uvX + curEdit.tileWidth + curEdit.padding; 
                ++col;
            }
            // count the row
            while ( (uvY + curEdit.tileHeight + curEdit.padding) <= curEdit.texture.height ) {
                uvY = uvY + curEdit.tileHeight + curEdit.padding; 
                ++row;
            }
            curEdit.col = col;
            curEdit.row = row;

            EditorUtility.SetDirty(curEdit);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void PreviewTileSheet ( float _x, float _y, exTileSheet _tileSheet ) {
        int uvX = _tileSheet.padding;
        int uvY = _tileSheet.padding;

        if ( _tileSheet.texture == null )
            return;

        // show texture by grids
        float curX = 0.0f;
        float curY = 0.0f;
        float interval = 2.0f;
        int borderSize = 1;
        uvX = _tileSheet.padding;
        uvY = _tileSheet.padding;

        //
        Rect filedRect = new Rect( _x, 
                                   _y,
                                   (_tileSheet.tileWidth + interval + 2 * borderSize) * _tileSheet.col - interval,
                                   (_tileSheet.tileHeight + interval + 2 * borderSize) * _tileSheet.row - interval );
        GUI.BeginGroup(filedRect);

            while ( (uvY + _tileSheet.tileHeight + _tileSheet.padding) <= _tileSheet.texture.height ) {
                while ( (uvX + _tileSheet.tileWidth + _tileSheet.padding) <= _tileSheet.texture.width ) {
                    Rect rect = new Rect( curX, 
                                          curY, 
                                          _tileSheet.tileWidth + 2 * borderSize, 
                                          _tileSheet.tileHeight + 2 * borderSize );
                    GUI.BeginGroup( new Rect ( rect.x + 1,
                                               rect.y + 1,
                                               rect.width - 2,
                                               rect.height - 2 ) );
                        Rect cellRect = new Rect( -uvX,
                                                  -uvY,
                                                  _tileSheet.texture.width, 
                                                  _tileSheet.texture.height );
                        GUI.DrawTexture( cellRect, _tileSheet.texture );
                    GUI.EndGroup();
                    exEditorHelper.DrawRect ( rect,
                                              new Color ( 1.0f, 1.0f, 1.0f, 0.0f ),
                                              Color.gray );

                    uvX = uvX + _tileSheet.tileWidth + _tileSheet.padding; 
                    curX = curX + _tileSheet.tileWidth + interval + 2 * borderSize; 
                }

                // step uv
                uvX = _tileSheet.padding;
                uvY = uvY + _tileSheet.tileHeight + _tileSheet.padding; 

                // step pos
                curX = 0.0f;
                curY = curY + _tileSheet.tileHeight + interval + 2 * borderSize; 
            }

        GUI.EndGroup();
        GUILayoutUtility.GetRect ( filedRect.width, filedRect.height );
    }
}

