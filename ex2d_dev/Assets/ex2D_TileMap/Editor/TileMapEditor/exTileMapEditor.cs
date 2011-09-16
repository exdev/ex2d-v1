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

    ///////////////////////////////////////////////////////////////////////////////
    // private data
    ///////////////////////////////////////////////////////////////////////////////

    private exTileMap curEdit;
    private Vector2 mouseDownPos = Vector2.zero;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/TileMap Editor %&t")]
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
        // toolbar 
        // ======================================================== 

        EditorGUILayout.BeginHorizontal ( EditorStyles.toolbar );
            GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal ();

        // ======================================================== 
        // scroll view
        // ======================================================== 

        float toolbarHeight = EditorStyles.toolbar.CalcHeight( new GUIContent(""), 0 );

        Rect lastRect = new Rect( 10, 0, 1, 1 );
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical( GUILayout.MaxWidth(300) );

            // ======================================================== 
            // Tile Sheet Field 
            // ======================================================== 

            GUILayout.BeginHorizontal();
                curEdit.tileSheet 
                    = (exTileSheet)EditorGUILayout.ObjectField( "Tile Sheet"
                                                               , curEdit.tileSheet
                                                               , typeof(exTileSheet)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                               , false
#endif
                                                             );
                if ( GUILayout.Button("Edit...", GUILayout.Width(40), GUILayout.Height(15) ) ) {
                    exTileSheetEditor editor = exTileSheetEditor.NewWindow();
                    editor.Edit(curEdit.tileSheet);
                }
            GUILayout.EndHorizontal();

            // ======================================================== 
            // tile size
            // ======================================================== 

            EditorGUILayout.LabelField ( "Tile Size", curEdit.tileSheet.tileWidth + " x " + curEdit.tileSheet.tileHeight ); 

            // ======================================================== 
            // tile sheet field 
            // ======================================================== 

            EditorGUILayout.Space ();
            lastRect = GUILayoutUtility.GetLastRect ();
            TileSheetField ( new Rect ( 15, lastRect.yMax, 300, 500 ),
                             curEdit.tileSheet );

            // ======================================================== 
            // separate line 
            // ======================================================== 

            GUILayout.Space (20);
            GUILayout.Label ( "Tile Map Option" ); 

            // ======================================================== 
            // col & row 
            // ======================================================== 

            GUILayout.BeginHorizontal ();
                curEdit.col = EditorGUILayout.IntField( "Col", curEdit.col ); 
                curEdit.row = EditorGUILayout.IntField( "Row", curEdit.row ); 
            GUILayout.EndHorizontal ();

            // ======================================================== 
            // Tile Width & Height 
            // ======================================================== 

            GUILayout.BeginHorizontal ();
                curEdit.tileWidth = EditorGUILayout.IntField( "Tile Width", curEdit.tileWidth ); 
                curEdit.tileHeight = EditorGUILayout.IntField( "Tile Height", curEdit.tileHeight ); 
            GUILayout.EndHorizontal ();

            // ======================================================== 
            // Tile Offset X & Y 
            // ======================================================== 

            GUILayout.BeginHorizontal ();
                curEdit.tileOffsetX = EditorGUILayout.IntField( "Tile Offset X", curEdit.tileOffsetX ); 
                curEdit.tileOffsetY = EditorGUILayout.IntField( "Tile Offset Y", curEdit.tileOffsetY ); 
            GUILayout.EndHorizontal ();

        GUILayout.EndVertical();

            GUILayout.Space (10);
            lastRect = GUILayoutUtility.GetLastRect ();

            // ======================================================== 
            // tile map filed 
            // ======================================================== 

            TileMapField ( new Rect ( lastRect.xMax,
                                      toolbarHeight, 
                                      position.width - lastRect.xMax,
                                      position.height - toolbarHeight ), 
                           curEdit );

            // ======================================================== 
            // draw vertical split line 
            // ======================================================== 

            exEditorHelper.DrawLine( new Vector2 ( lastRect.xMax, toolbarHeight-1 ),
                                     new Vector2 ( lastRect.xMax, position.height ),
                                     Color.black,
                                     1.0f );

        GUILayout.EndHorizontal();

        // ======================================================== 
        // check dirty
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
    }
}


