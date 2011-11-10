// ======================================================================================
// File         : exTileMapEditor.cs
// Author       : Wu Jie 
// Last Change  : 09/16/2011 | 11:42:35 AM | Friday,September
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
// exTileMapEditor
///////////////////////////////////////////////////////////////////////////////

partial class exTileMapEditor : EditorWindow {

    private static Texture[] toolIcons = new Texture[] { 
        (Texture)AssetDatabase.LoadAssetAtPath( "Assets/ex2D_TileMap/Editor/Resource/icon_stamp.png", typeof(Texture) ), 
        (Texture)AssetDatabase.LoadAssetAtPath( "Assets/ex2D_TileMap/Editor/Resource/icon_bucket.png", typeof(Texture) ), 
        (Texture)AssetDatabase.LoadAssetAtPath( "Assets/ex2D_TileMap/Editor/Resource/icon_area.png", typeof(Texture) ), 
    };

    private static Texture[] modeIcons = new Texture[] { 
        (Texture)AssetDatabase.LoadAssetAtPath( "Assets/ex2D_TileMap/Editor/Resource/icon_paint.png", typeof(Texture) ), 
        (Texture)AssetDatabase.LoadAssetAtPath( "Assets/ex2D_TileMap/Editor/Resource/icon_eraser.png", typeof(Texture) ), 
    };

    ///////////////////////////////////////////////////////////////////////////////
    // private data
    ///////////////////////////////////////////////////////////////////////////////

    private exTileMap curEdit;
    private Vector2 mouseDownPos = Vector2.zero;

    // DEBUG
    private int debugVisibleGrids = 0;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/TileMap Editor")]
    public static exTileMapEditor NewWindow () {
        exTileMapEditor newWindow = EditorWindow.GetWindow<exTileMapEditor>();
        return newWindow;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        name = "TileMap Editor";
        wantsMouseMove = true;
        autoRepaintOnSceneChange = true;
        // position = new Rect ( 50, 50, 800, 600 );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
        sheetInRectSelectState = false;
        sheetSelectedGrids.Clear();
        sheetCommitGrids.Clear();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Edit ( Object _obj ) {
        // check if repaint
        if ( curEdit != _obj ) {

            // check if we have atlas - editorinfo in the same directory
            Object obj = _obj; 
            if ( obj is exTileMap ) {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string dirname = Path.GetDirectoryName(assetPath);
                string filename = Path.GetFileNameWithoutExtension(assetPath);
                obj = (exTileMap)AssetDatabase.LoadAssetAtPath( Path.Combine( dirname, filename + ".asset" ),
                                                                typeof(exTileMap) );
                if ( obj == null ) {
                    obj = _obj;
                }
            }

            // if this is another atlas, swtich to it.
            if ( obj is exTileMap && obj != curEdit ) {
                curEdit = obj as exTileMap;
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
            GUILayout.Label ( "Please select a Tile Map" );
            return;
        }

        // ======================================================== 
        // 
        // ======================================================== 

        Rect lastRect = new Rect( 10, 0, 1, 1 );

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical( GUILayout.MaxWidth(300) );

            GUILayout.Space(5);

            // ======================================================== 
            // Tile Sheet Field 
            // ======================================================== 

            GUILayout.BeginHorizontal();
                exTileSheet newTileSheet = 
                    (exTileSheet)EditorGUILayout.ObjectField( "Tile Sheet"
                                                               , curEdit.tileSheet
                                                               , typeof(exTileSheet)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                               , false
#endif
                                                             );
                if ( newTileSheet != curEdit.tileSheet ) {
                    curEdit.tileSheet = newTileSheet;
                    sheetSelectedGrids.Clear();
                    sheetCommitGrids.Clear();
                }
                if ( GUILayout.Button("Edit...", GUILayout.Width(40), GUILayout.Height(15) ) ) {
                    exTileSheetEditor editor = exTileSheetEditor.NewWindow();
                    editor.Edit(curEdit.tileSheet);
                }
            GUILayout.EndHorizontal();

            // ======================================================== 
            // tile size
            // ======================================================== 

            if ( curEdit.tileSheet )
                EditorGUILayout.LabelField ( "Tile Size", curEdit.tileSheet.tileWidth + " x " + curEdit.tileSheet.tileHeight ); 
            else
                EditorGUILayout.LabelField ( "Tile Size", "0 x 0" ); 

            // ======================================================== 
            // tile sheet field 
            // ======================================================== 

            EditorGUILayout.Space ();
            lastRect = GUILayoutUtility.GetLastRect ();
            TileSheetField ( new Rect ( 15, lastRect.yMax, 300, 500 ),
                             curEdit.tileSheet );

            // ======================================================== 
            // tile map object
            // ======================================================== 

            GUILayout.Space (20);
            GUI.enabled = false;
            EditorGUILayout.ObjectField( "Tile Map"
                                         , curEdit
                                         , typeof(exTileMap)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                         , false
#endif
                                       );
            GUI.enabled = true;

            // ======================================================== 
            // col & row 
            // ======================================================== 

            GUILayout.BeginHorizontal ();
                int newCol = Mathf.Max( EditorGUILayout.IntField( "Column", curEdit.col ), 1 ); 
                int newRow = Mathf.Max( EditorGUILayout.IntField( "Row", curEdit.row ), 1 ); 
                if ( newCol != curEdit.col || newRow != curEdit.row ) {
                    curEdit.Resize ( newCol, newRow );
                }
            GUILayout.EndHorizontal ();

            // ======================================================== 
            // Tile Width & Height 
            // ======================================================== 

            GUILayout.BeginHorizontal ();
                 int newtileWidth = EditorGUILayout.IntField( "Tile Width", curEdit.tileWidth ); 
                 if ( newtileWidth != curEdit.tileWidth ) {
                     curEdit.tileWidth = newtileWidth;
                     curEdit.editorNeedRebuild = true;
                 }
                 
                 int newtileHeight = EditorGUILayout.IntField( "Tile Height", curEdit.tileHeight ); 
                 if ( newtileHeight != curEdit.tileHeight ) {
                     curEdit.tileHeight = newtileHeight;
                     curEdit.editorNeedRebuild = true;
                 }
            GUILayout.EndHorizontal ();

            // ======================================================== 
            // Tile Offset X & Y 
            // ======================================================== 

            GUILayout.BeginHorizontal ();
                 int newTileOffsetX = EditorGUILayout.IntField( "Tile Offset X", curEdit.tileOffsetX ); 
                 if ( newTileOffsetX != curEdit.tileOffsetX ) {
                     curEdit.tileOffsetX = newTileOffsetX;
                     curEdit.editorNeedRebuild = true;
                 }
                 
                 int newTileOffsetY = EditorGUILayout.IntField( "Tile Offset Y", curEdit.tileOffsetY ); 
                 if ( newTileOffsetY != curEdit.tileOffsetY ) {
                     curEdit.tileOffsetY = newTileOffsetY;
                     curEdit.editorNeedRebuild = true;
                 }
            GUILayout.EndHorizontal ();

            // DEBUG { 
            // ======================================================== 
            // Debug 
            // ======================================================== 

            GUILayout.Space (20);
            GUILayout.Label ( "Debug" );
            GUILayout.Label ( "Visible Grids " + debugVisibleGrids );
            // } DEBUG end 


        GUILayout.EndVertical();

            GUILayout.Space (10);
            lastRect = GUILayoutUtility.GetLastRect ();

            GUILayout.BeginVertical();

                // ======================================================== 
                // toolbar 
                // ======================================================== 

                GUILayout.BeginHorizontal ( EditorStyles.toolbar );

                    // ======================================================== 
                    // show grid 
                    // ======================================================== 

                    curEdit.editorShowGrid = GUILayout.Toggle( curEdit.editorShowGrid, "Show Grid", EditorStyles.toolbarButton );
                    GUILayout.Space (10);

                    // ======================================================== 
                    // edit tool 
                    // ======================================================== 

                    curEdit.editorEditTool 
                        = (exTileMap.EditTool)GUILayout.Toolbar ( (int)curEdit.editorEditTool, 
                                                                  toolIcons,
                                                                  EditorStyles.toolbarButton );  
                    GUILayout.Space (10);

                    // ======================================================== 
                    // edit mode 
                    // ======================================================== 

                    if ( sheetSelectedGrids.Count > 0 || sheetCommitGrids.Count > 0 ) {
                        curEdit.editorEditMode = exTileMap.EditMode.Paint; 
                    }
                    else {
                        curEdit.editorEditMode = exTileMap.EditMode.Erase; 
                    }

                    exTileMap.EditMode newEditMode
                        = (exTileMap.EditMode)GUILayout.Toolbar ( (int)curEdit.editorEditMode, 
                                                                  modeIcons,
                                                                  EditorStyles.toolbarButton );  
                    if ( newEditMode != curEdit.editorEditMode ) {
                        curEdit.editorEditMode = newEditMode;
                        if ( curEdit.editorEditMode == exTileMap.EditMode.Erase ) {
                            sheetSelectedGrids.Clear();
                            sheetCommitGrids.Clear();
                        }
                    }

                    GUILayout.FlexibleSpace();

                    // ======================================================== 
                    // clear
                    // ======================================================== 

                    if ( GUILayout.Button( "Clear", EditorStyles.toolbarButton ) ) {
                        curEdit.Clear();
                    }
                    GUILayout.Space (10);

                    // ======================================================== 
                    // Help
                    // ======================================================== 

                    if ( GUILayout.Button( exEditorHelper.HelpTexture(), EditorStyles.toolbarButton ) ) {
                        Help.BrowseURL("http://www.ex-dev.com/ex2d/wiki/doku.php?id=manual:tilemap_editor");
                    }

                GUILayout.EndHorizontal ();

                // ======================================================== 
                // tile map filed 
                // ======================================================== 

                float toolbarHeight = EditorStyles.toolbar.CalcHeight( new GUIContent(""), 0 );

                TileMapField ( new Rect ( lastRect.xMax,
                                          toolbarHeight, 
                                          position.width - lastRect.xMax,
                                          position.height - toolbarHeight ), 
                               curEdit );

            GUILayout.EndVertical();

            // ======================================================== 
            // draw vertical split line 
            // ======================================================== 

            exEditorHelper.DrawLine( new Vector2 ( lastRect.xMax, 0 ),
                                     new Vector2 ( lastRect.xMax, position.height ),
                                     Color.black,
                                     1.0f );

        GUILayout.EndHorizontal();

        // ======================================================== 
        // NOTE: cancle select event of tile sheet. Should after TileMapField, so we put it here 
        Event e = Event.current;
        // ======================================================== 

        if ( e.type == EventType.MouseUp && e.button == 0 ) {
            if ( sheetInRectSelectState ) {
                sheetInRectSelectState = false;
                ConfirmRectSelection();
                Repaint();

                e.Use();
            }
            else {
                sheetCommitGrids.Clear();
                sheetSelectedGrids.Clear();
                Repaint();

                e.Use();
            }
        }

        // ======================================================== 
        // check dirty
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
    }
}


