// ======================================================================================
// File         : exTileInfoEditor.cs
// Author       : Wu Jie 
// Last Change  : 08/20/2011 | 16:28:17 PM | Saturday,August
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
// exTileInfoEditor
///////////////////////////////////////////////////////////////////////////////

partial class exTileInfoEditor : EditorWindow {

    ///////////////////////////////////////////////////////////////////////////////
    // private data
    ///////////////////////////////////////////////////////////////////////////////

    private exTileInfo curEdit;
    private Vector2 scrollPos = Vector2.zero;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/TileInfo Editor %&t")]
    public static exTileInfoEditor NewWindow () {
        exTileInfoEditor newWindow = EditorWindow.GetWindow<exTileInfoEditor>();
        return newWindow;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        name = "TileInfo Editor";
        wantsMouseMove = true;
        autoRepaintOnSceneChange = true;
        // position = new Rect ( 50, 50, 800, 600 );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
        // TODO { 
        // selectedElements.Clear();

        // inRectSelectState = false;
        // inDraggingElementState = false;
        // accDeltaMove = Vector2.zero;

        // doImport = false;
        // importObjects.Clear();
        // oldSelActiveObject = null;
        // oldSelObjects.Clear();
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Edit ( Object _obj ) {
        // check if repaint
        if ( curEdit != _obj ) {

            // check if we have atlas - editorinfo in the same directory
            Object obj = _obj; 
            if ( obj is exTileInfo || obj is Texture2D || obj is Material ) {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string dirname = Path.GetDirectoryName(assetPath);
                string filename = Path.GetFileNameWithoutExtension(assetPath);
                obj = (exTileInfo)AssetDatabase.LoadAssetAtPath( Path.Combine( dirname, filename + ".asset" ),
                                                                typeof(exTileInfo) );
                if ( obj == null ) {
                    obj = _obj;
                }
            }

            // if this is another atlas, swtich to it.
            if ( obj is exTileInfo && obj != curEdit ) {
                curEdit = obj as exTileInfo;
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
            GUILayout.Label ( "Please select a Tile Info" );
            return;
        }

        // ======================================================== 
        // if we have curEdit
        // ======================================================== 

        Rect lastRect = new Rect( 10, 0, 1, 1 );
        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, 
                                                      GUILayout.Width(position.width),
                                                      GUILayout.Height(position.height) );

        // draw label
        GUILayout.Space(10);
        GUILayout.Label ( AssetDatabase.GetAssetPath(curEdit) );
        lastRect = GUILayoutUtility.GetLastRect ();  

        // ======================================================== 
        // settings area 
        // ======================================================== 

        GUILayout.BeginHorizontal();
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
                    string texturePath = AssetDatabase.GetAssetPath(curEdit.texture);

                    // load material from "texture_path/Materials/texture_name.mat"
                    string materialDirectory = Path.Combine( Path.GetDirectoryName(texturePath), "Materials" );
                    string materialPath = Path.Combine( materialDirectory, curEdit.texture.name + ".mat" );
                    Material newMaterial = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));

                    // if not found, load material from "texture_path/texture_name.mat"
                    if ( newMaterial == null ) {
                        newMaterial = (Material)AssetDatabase.LoadAssetAtPath( Path.Combine( Path.GetDirectoryName(texturePath), 
                                                                                             Path.GetFileNameWithoutExtension(texturePath) + ".mat" ), 
                                                                               typeof(Material) );
                    }

                    // if still not found, create it!
                    if ( newMaterial == null ) {
                        // check if directory exists, if not, create one.
                        DirectoryInfo info = new DirectoryInfo(materialDirectory);
                        if ( info.Exists == false )
                            AssetDatabase.CreateFolder ( texturePath, "Materials" );

                        // create temp materal
                        newMaterial = new Material( Shader.Find("ex2D/Alpha Blended") );
                        newMaterial.mainTexture = curEdit.texture;

                        AssetDatabase.CreateAsset(newMaterial, materialPath);
                        AssetDatabase.Refresh();
                    }

                    // assign it
                    curEdit.material = newMaterial;
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

            // TODO { 
            // an anchor editor (used to put anchor point)
            // } TODO end 

        GUILayout.EndVertical();

        // ======================================================== 
        // spac 
        // ======================================================== 

        GUILayout.Space(20);
        lastRect = GUILayoutUtility.GetLastRect ();  

        // ======================================================== 
        // tile info texture
        // ======================================================== 

        GUILayout.BeginVertical();
            PreviewTileInfo ( lastRect.xMax, lastRect.yMax, curEdit );
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // TODO: process evenet { 
        // // ======================================================== 
        // Event e = Event.current;
        // // ======================================================== 
        // } TODO end 

        //
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

        EditorGUILayout.EndScrollView();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void PreviewTileInfo ( float _x, float _y, exTileInfo _tileInfo ) {
        int uvX = _tileInfo.padding;
        int uvY = _tileInfo.padding;

        if ( _tileInfo.texture == null )
            return;

        // show texture by grids
        float curX = 0.0f;
        float curY = 0.0f;
        float interval = 2.0f;
        int borderSize = 1;
        uvX = _tileInfo.padding;
        uvY = _tileInfo.padding;

        //
        Rect filedRect = new Rect( _x, 
                                   _y,
                                   (_tileInfo.tileWidth + interval + 2 * borderSize) * _tileInfo.col - interval,
                                   (_tileInfo.tileHeight + interval + 2 * borderSize) * _tileInfo.row - interval );
        GUI.BeginGroup(filedRect);

            while ( (uvY + _tileInfo.tileHeight + _tileInfo.padding) <= _tileInfo.texture.height ) {
                while ( (uvX + _tileInfo.tileWidth + _tileInfo.padding) <= _tileInfo.texture.width ) {
                    Rect rect = new Rect( curX, 
                                          curY, 
                                          _tileInfo.tileWidth + 2 * borderSize, 
                                          _tileInfo.tileHeight + 2 * borderSize );
                    GUI.BeginGroup( new Rect ( rect.x + 1,
                                               rect.y + 1,
                                               rect.width - 2,
                                               rect.height - 2 ) );
                        Rect cellRect = new Rect( -uvX,
                                                  -uvY,
                                                  _tileInfo.texture.width, 
                                                  _tileInfo.texture.height );
                        GUI.DrawTexture( cellRect, _tileInfo.texture );
                    GUI.EndGroup();
                    exEditorHelper.DrawRect ( rect,
                                              new Color ( 1.0f, 1.0f, 1.0f, 0.0f ),
                                              Color.gray );

                    uvX = uvX + _tileInfo.tileWidth + _tileInfo.padding; 
                    curX = curX + _tileInfo.tileWidth + interval + 2 * borderSize; 
                }

                // step uv
                uvX = _tileInfo.padding;
                uvY = uvY + _tileInfo.tileHeight + _tileInfo.padding; 

                // step pos
                curX = 0.0f;
                curY = curY + _tileInfo.tileHeight + interval + 2 * borderSize; 
            }

        GUI.EndGroup();
        GUILayoutUtility.GetRect ( filedRect.width, filedRect.height );
    }
}

