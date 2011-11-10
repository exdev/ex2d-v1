// ======================================================================================
// File         : exAtlasEditor.cs
// Author       : Wu Jie 
// Last Change  : 06/07/2011 | 17:05:17 PM | Tuesday,June
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
/// the atlas editor
///
///////////////////////////////////////////////////////////////////////////////

partial class exAtlasEditor : EditorWindow {

    ///////////////////////////////////////////////////////////////////////////////
    // static 
    ///////////////////////////////////////////////////////////////////////////////

    static int[] sizeList = new int[] { 
        32, 64, 128, 256, 512, 1024, 2048, 4096 
    };
    static string[] sizeTextList = new string[] { 
        "32px", "64px", "128px", "256px", "512px", "1024px", "2048px", "4096px" 
    };

    ///////////////////////////////////////////////////////////////////////////////
    // private variables
    ///////////////////////////////////////////////////////////////////////////////

    private exAtlasInfo curEdit;

    private int selectIdx = 0;
    private Rect atlasInfoRect;

    private Vector2 scrollPos = Vector2.zero;
    private List<exAtlasInfo.Element> selectedElements = new List<exAtlasInfo.Element>();

    private Rect selectRect = new Rect( 0, 0, 1, 1 );
    private Vector2 mouseDownPos = Vector2.zero;
    private Vector2 accDeltaMove = Vector2.zero;
    private bool inRectSelectState = false;
    private bool inDraggingElementState = false;

    private bool doImport = false;
    private List<Object> importObjects = new List<Object>();
    private Object oldSelActiveObject;
    private List<Object> oldSelObjects = new List<Object>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \return the editor
    /// Open the atlas editor window
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/Atlas Editor")]
    public static exAtlasEditor NewWindow () {
        exAtlasEditor newWindow = EditorWindow.GetWindow<exAtlasEditor>();
        return newWindow;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        name = "Atlas Editor";
        wantsMouseMove = true;
        autoRepaintOnSceneChange = false;
        // position = new Rect ( 50, 50, 800, 600 );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
        selectedElements.Clear();

        inRectSelectState = false;
        inDraggingElementState = false;
        accDeltaMove = Vector2.zero;

        doImport = false;
        importObjects.Clear();
        oldSelActiveObject = null;
        oldSelObjects.Clear();
    }

    // ------------------------------------------------------------------ 
    /// \param _obj
    /// Check if the object is valid atlas and open it in atlas editor.
    // ------------------------------------------------------------------ 

    public void Edit ( Object _obj ) {
        // check if repaint
        if ( curEdit != _obj ) {

            // check if we have atlas - editorinfo in the same directory
            Object obj = _obj; 
            if ( obj is exAtlas || obj is Texture2D || obj is Material ) {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                string dirname = Path.GetDirectoryName(assetPath);
                string filename = Path.GetFileNameWithoutExtension(assetPath);
                obj = (exAtlasInfo)AssetDatabase.LoadAssetAtPath( Path.Combine( dirname, filename + " - EditorInfo.asset" ),
                                                                typeof(exAtlasInfo) );
                if ( obj == null ) {
                    obj = _obj;
                }
            }

            // if this is another atlas, swtich to it.
            if ( obj is exAtlasInfo && obj != curEdit ) {
                curEdit = obj as exAtlasInfo;
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
    //     OnSelectionChange
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSelectionChange () {
        Edit ( Selection.activeObject );

        if ( curEdit != null ) {
            selectedElements.Clear();
            foreach ( Object o in Selection.objects ) {
                if ( o is Texture2D ) {
                    foreach ( exAtlasInfo.Element el in curEdit.elements ) {
                        if ( el.texture == o ) {
                            AddSelected(el);
                            break;
                        }
                    }
                }
            }
        }

        Repaint ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        EditorGUI.indentLevel = 0;

        if ( curEdit == null ) {
            GUILayout.Space(10);
            GUILayout.Label ( "Please select an Atlas Info" );
            // DISABLE: this.ShowNotification( new GUIContent("Please select an Atlas Info"));
            return;
        }

        // ======================================================== 
        // toolbar 
        // ======================================================== 

        GUILayout.BeginHorizontal ( EditorStyles.toolbar );

            GUILayout.FlexibleSpace();

            // ======================================================== 
            // Select 
            // ======================================================== 

            GUI.enabled = selectedElements.Count != 0;
            if ( GUILayout.Button( "Select In Project...", EditorStyles.toolbarButton ) ) {
                List<Object> selects = new List<Object>(curEdit.elements.Count);
                foreach ( exAtlasInfo.Element el in selectedElements ) {
                    if ( selects.IndexOf(el.texture) == -1 ) {
                        selects.Add(el.texture);
                    }
                }

                if ( selects.Count != 0 ) {
                    selectIdx = (selectIdx + 1) % selects.Count;  
                    Selection.objects = selects.ToArray();
                    EditorGUIUtility.PingObject(Selection.objects[selectIdx]);
                }
            }
            GUI.enabled = true;
            GUILayout.Space(5);

            // ======================================================== 
            // zoom in/out slider 
            // ======================================================== 

            GUILayout.Label ("Zoom");
            GUILayout.Space(5);
            curEdit.scale = GUILayout.HorizontalSlider ( curEdit.scale, 
                                                         0.1f, 
                                                         2.0f, 
                                                         GUILayout.MaxWidth(150) );
            GUILayout.Space(5);
            curEdit.scale = EditorGUILayout.FloatField( curEdit.scale,
                                                        EditorStyles.toolbarTextField,
                                                        GUILayout.Width(50) );
            curEdit.scale = Mathf.Clamp( curEdit.scale, 0.1f, 2.0f );

            // ======================================================== 
            // Build 
            // ======================================================== 

            GUI.enabled = curEdit.needRebuild;
            if ( GUILayout.Button( "Build", EditorStyles.toolbarButton, GUILayout.Width(80) ) ) {
                // build atlas info to atals
                exAtlasInfoUtility.Build(curEdit);

                // build sprite animclip that used this atlasInfo
                exAtlasInfoUtility.BuildSpAnimClipsFromRebuildList(curEdit);

                // update scene sprites
                List<string> rebuildAtlasInfos = new List<string>();
                rebuildAtlasInfos.Add(exEditorHelper.AssetToGUID(curEdit));
                exSceneHelper.UpdateSprites (rebuildAtlasInfos);

                // NOTE: without this you will got leaks message
                EditorUtility.UnloadUnusedAssets();
            }
            GUI.enabled = true;

            // ======================================================== 
            // Help
            // ======================================================== 

            if ( GUILayout.Button( exEditorHelper.HelpTexture(), EditorStyles.toolbarButton ) ) {
                Help.BrowseURL("http://www.ex-dev.com/ex2d/wiki/doku.php?id=manual:atlas_editor");
            }

        GUILayout.EndHorizontal ();

        // ======================================================== 
        // scroll view
        // ======================================================== 

        float toolbarHeight = EditorStyles.toolbar.CalcHeight( new GUIContent(""), 0 );
        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, 
                                                      GUILayout.Width(position.width),
                                                      GUILayout.Height(position.height-toolbarHeight) );

        Rect lastRect = new Rect( 10, 0, 1, 1 );
        GUILayout.Space(5);

        // DISABLE { 
        // // draw label
        // GUILayout.Label ( AssetDatabase.GetAssetPath(curEdit) );
        // } DISABLE end 

        // ======================================================== 
        // atlas info
        // ======================================================== 

        Object newAtlasInfo = EditorGUILayout.ObjectField( "Atlas Info"
                                                           , curEdit
                                                           , typeof(exAtlasInfo)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                           , false 
#endif
                                                           , GUILayout.Width(300)
                                                         );
        if ( newAtlasInfo != curEdit ) 
            Selection.activeObject = newAtlasInfo;

        // ======================================================== 
        // settings area 
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical( GUILayout.MaxWidth(200) );

            // ======================================================== 
            // canvas
            // ======================================================== 

            curEdit.foldCanvas = EditorGUILayout.Foldout(curEdit.foldCanvas, "Canvas");
            if ( curEdit.foldCanvas ) {

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();

                // width and height
                int width = curEdit.width;
                int height = curEdit.height;
                WidthAndHeightField( ref width, ref height );
                // Check if we need to Reset width & height
                if ( width != curEdit.width ||
                     height != curEdit.height )
                {
                    curEdit.width = width;
                    curEdit.height = height;

                    if ( curEdit.texture ) {
                        // NOTE: if we don't write data to disk, all changes will go back.
                        string path = AssetDatabase.GetAssetPath(curEdit.texture);
                        exTextureHelper.SetReadable ( curEdit.texture, true );
                        curEdit.texture.Resize( width, height );
                        curEdit.ClearAtlasTexture();

                        // NOTE: we can not write back directly since the texture format problem  
                        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
                        for ( int i = 0; i < width; ++i ) {
                            for ( int j = 0; j < height; ++j ) {
                                tex.SetPixel(i, j, new Color(1.0f, 1.0f, 1.0f, 0.0f) );
                            }
                        }
                        tex.Apply(false);

                        byte[] pngData = tex.EncodeToPNG();
                        if (pngData != null)
                            File.WriteAllBytes(path, pngData);
                        Object.DestroyImmediate(tex);

                        TextureImporter importSettings = TextureImporter.GetAtPath(path) as TextureImporter;
                        importSettings.maxTextureSize = Mathf.Max( width, height );
                        importSettings.isReadable = false;
                        AssetDatabase.ImportAsset( path );
                    }

                    curEdit.needRebuild = true;
                }
                curEdit.bgColor = EditorGUILayout.ColorField( "Bg Color", curEdit.bgColor );
                curEdit.showCheckerboard = EditorGUILayout.Toggle ( "Show Checkerboard", curEdit.showCheckerboard );

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(20);

            // ======================================================== 
            // layout
            // ======================================================== 

            curEdit.foldLayout = EditorGUILayout.Foldout(curEdit.foldLayout, "Layout");
            if ( curEdit.foldLayout ) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();

                // algorithm
                curEdit.algorithm = (exAtlasInfo.Algorithm)EditorGUILayout.EnumPopup ( "Algorithm", curEdit.algorithm );

                // sortBy
                curEdit.sortBy = (exAtlasInfo.SortBy)EditorGUILayout.EnumPopup ( "Sort By", curEdit.sortBy );

                // sortOrder
                curEdit.sortOrder = (exAtlasInfo.SortOrder)EditorGUILayout.EnumPopup ( "Sort Order", curEdit.sortOrder );

                // padding
                curEdit.padding = EditorGUILayout.IntField( "Padding", curEdit.padding );

                // TODO: have bug, just disable it { 
                // allow rotate
                GUI.enabled = false;
                curEdit.allowRotate = false;
                curEdit.allowRotate = EditorGUILayout.Toggle ( "Allow Rotate", curEdit.allowRotate );
                GUI.enabled = true;
                // } TODO end 

                if ( GUILayout.Button ( "Apply" ) ) {
                    EditorUtility.DisplayProgressBar( "Layout Elements...", "Layout Elements...", 0.5f  );    
                    // register undo
                    Undo.RegisterUndo ( curEdit, "Apply.LayoutElements" );
                    curEdit.LayoutElements ();
                    EditorUtility.ClearProgressBar();
                }
                // GUI.enabled = true;

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(20);

            // ======================================================== 
            // Element
            // ======================================================== 

            curEdit.foldElement = EditorGUILayout.Foldout(curEdit.foldElement, "Element");
            if ( curEdit.foldElement ) {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();

                // sprite background color
                curEdit.elementBgColor = EditorGUILayout.ColorField( "Bg Color", curEdit.elementBgColor );
                curEdit.elementSelectColor = EditorGUILayout.ColorField( "Select Color", curEdit.elementSelectColor );

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(40);

            // ======================================================== 
            // atlas texture and material 
            // ======================================================== 

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
                GUI.enabled = false;
                string assetPath = AssetDatabase.GetAssetPath(curEdit);
                string dir = Path.GetDirectoryName(assetPath); 

                // ======================================================== 
                // texture
                // ======================================================== 

                if ( curEdit.texture ) {
                    curEdit.texture 
                        = (Texture2D)EditorGUILayout.ObjectField( "Texture"
                                                                  , curEdit.texture
                                                                  , typeof(Texture2D)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                  , false
#endif
                                                                  , GUILayout.Width(100) 
                                                                  , GUILayout.Height(100) 
                                                                );
                }
                else {
                    GUI.enabled = true;
                    if ( GUILayout.Button ( "Create Texture..." ) ) {
                        // create texture
                        Texture2D tex = new Texture2D( curEdit.width, 
                                                       curEdit.height, 
                                                       TextureFormat.ARGB32, 
                                                       false );
                        for ( int i = 0; i < curEdit.width; ++i ) {
                            for ( int j = 0; j < curEdit.height; ++j ) {
                                tex.SetPixel(i, j, new Color(1.0f, 1.0f, 1.0f, 0.0f) );
                            }
                        }
                        tex.Apply(false);

                        // save texture to png
                        string atlasTexturePath = Path.Combine( dir, curEdit.atlasName + ".png" );
                        byte[] pngData = tex.EncodeToPNG();
                        if (pngData != null)
                            File.WriteAllBytes(atlasTexturePath, pngData);
                        Object.DestroyImmediate(tex);

                        // import texture
                        AssetDatabase.ImportAsset( atlasTexturePath );
                        TextureImporter importSettings = TextureImporter.GetAtPath(atlasTexturePath) as TextureImporter;
                        importSettings.maxTextureSize = Mathf.Max( curEdit.width, curEdit.height );
                        importSettings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                        importSettings.isReadable = true;
                        importSettings.mipmapEnabled = false;
                        importSettings.textureType = TextureImporterType.Advanced;
                        importSettings.npotScale = TextureImporterNPOTScale.None;
                        AssetDatabase.ImportAsset( atlasTexturePath );

                        curEdit.texture = (Texture2D)AssetDatabase.LoadAssetAtPath( atlasTexturePath, typeof(Texture2D) );
                        if ( curEdit.material )
                            curEdit.material.mainTexture = curEdit.texture;
                        EditorUtility.SetDirty(curEdit);
                    }
                    GUI.enabled = false;
                }
                // GUILayout.Space(5);

                // ======================================================== 
                // material
                // ======================================================== 

                if ( curEdit.material ) {
                    curEdit.material 
                        = (Material)EditorGUILayout.ObjectField( "Material" 
                                                                 , curEdit.material
                                                                 , typeof(Material)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                 , false 
#endif
                                                               );
                }
                else {
                    GUI.enabled = true;
                    if ( GUILayout.Button ( "Create Material..." ) ) {
                        Material newMaterial = new Material( Shader.Find("ex2D/Alpha Blended") );
                        AssetDatabase.CreateAsset( newMaterial, Path.Combine( dir, curEdit.atlasName + ".mat" ) );

                        curEdit.material = newMaterial;
                        curEdit.material.mainTexture = curEdit.texture;
                        EditorUtility.SetDirty(curEdit);
                    }
                    GUI.enabled = false;
                }
                // GUILayout.Space(5);

                // ======================================================== 
                // atlas
                // ======================================================== 

                if ( curEdit.atlas ) {
                    curEdit.atlas 
                        = (exAtlas)EditorGUILayout.ObjectField( "Atlas"
                                                              , curEdit.atlas
                                                              , typeof(exAtlas)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                              , false 
#endif
                                                            );
                }
                else {
                    GUI.enabled = true;
                    if ( GUILayout.Button ( "Create Atlas..." ) ) {
                        exAtlas newAtlas = exAtlasInfoUtility.CreateAtlas( dir, curEdit.atlasName );
                        curEdit.atlas = newAtlas;
                        EditorUtility.SetDirty(curEdit);
                    }
                    GUI.enabled = false;
                }
                GUI.enabled = true;
                // GUILayout.Space(5);

                // ======================================================== 
                // build color 
                // ======================================================== 

                GUILayout.BeginHorizontal();
                    bool newUseBuildColor = GUILayout.Toggle ( curEdit.useBuildColor, "Use Build Color" ); 
                    if ( newUseBuildColor != curEdit.useBuildColor ) {
                        curEdit.useBuildColor = newUseBuildColor;
                        curEdit.needRebuild = true;
                        GUI.changed = true;
                    }

                    GUI.enabled = curEdit.useBuildColor;
                        Color newBuildColor = EditorGUILayout.ColorField( curEdit.buildColor );
                        if ( newBuildColor != curEdit.buildColor ) {
                            curEdit.buildColor = newBuildColor;
                            curEdit.needRebuild = true;
                            GUI.changed = true;
                        }
                    GUI.enabled = true;
                GUILayout.EndHorizontal();

                // ======================================================== 
                // bitmap fonts 
                // ======================================================== 

                GUILayout.Space(20);
                GUILayout.Label ( "Atlas Fonts" );
                for ( int i = 0; i < curEdit.bitmapFonts.Count; ++i ) {
                    GUILayout.BeginHorizontal();
                        exBitmapFont bmfont = curEdit.bitmapFonts[i];
                        EditorGUILayout.ObjectField( bmfont 
                                                     , typeof(exBitmapFont) 
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                     , false 
#endif
                                                   );
                        if ( GUILayout.Button("Delete", GUILayout.MaxWidth(80) ) ) {
                            curEdit.RemoveBitmapFont(bmfont);
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(bmfont));
                            --i;
                        }
                    GUILayout.EndHorizontal();
                }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        // ======================================================== 
        // space 
        // ======================================================== 

        GUILayout.Space(40);
        lastRect = GUILayoutUtility.GetLastRect ();  

        // ======================================================== 
        // atlas area 
        // ======================================================== 

        GUILayout.BeginVertical();
        GUILayout.Space(10);

            // exAtlas Border and Background
            lastRect = GUILayoutUtility.GetLastRect ();  
            int borderSize = 1;
            atlasInfoRect = new Rect( lastRect.xMax + borderSize, 
                                      lastRect.yMax + borderSize, 
                                      curEdit.width * curEdit.scale, 
                                      curEdit.height * curEdit.scale );
            AtlasInfoField ( atlasInfoRect, borderSize, curEdit );
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // ======================================================== 
        // draw select rect 
        // ======================================================== 

        if ( inRectSelectState && (selectRect.width != 0.0f || selectRect.height != 0.0f) ) {
            exEditorHelper.DrawRect( selectRect, new Color( 0.0f, 0.5f, 1.0f, 0.2f ), new Color( 0.0f, 0.5f, 1.0f, 1.0f ) );
        }

        // ======================================================== 
        Event e = Event.current;
        // ======================================================== 

        // mouse down
        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            GUIUtility.keyboardControl = -1; // remove any keyboard control

            mouseDownPos = e.mousePosition;
            inRectSelectState = true;
            UpdateSelectRect ();
            ConfirmRectSelection();
            Repaint();

            e.Use();
        }

        // rect select
        if ( inRectSelectState ) {
            if ( e.type == EventType.MouseDrag ) {
                UpdateSelectRect ();
                ConfirmRectSelection();
                Repaint();

                e.Use();
            }
            else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                inRectSelectState = false;
                ConfirmRectSelection();
                Repaint();

                e.Use();
            }
        }

        // dragging selected
        if ( inDraggingElementState ) {
            if ( e.type == EventType.MouseDrag ) {
                MoveSelections ( e.delta / curEdit.scale );
                Repaint();

                e.Use();
            }
            else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                if ( curEdit.needUpdateAnimClips ) {
                    foreach ( exAtlasInfo.Element el in selectedElements ) {
                        curEdit.AddSpriteAnimClipForRebuilding(el);
                    }
                    curEdit.needUpdateAnimClips = false;
                }
                inDraggingElementState = false;
                accDeltaMove = Vector2.zero;

                e.Use();
            }
        }

        // key events 
        if ( e.isKey ) {
            if ( e.type == EventType.KeyDown ) {
                if ( e.keyCode == KeyCode.Backspace ||
                     e.keyCode == KeyCode.Delete ) 
                {
                    RemoveSelectedElements();
                    Repaint();
                    e.Use();
                }
            }
        }

        EditorGUILayout.EndScrollView();

        // ======================================================== 
        // do imports 
        // ======================================================== 

        if ( doImport ) {
            doImport = false;
            ImportObjects();

            Selection.activeObject = oldSelActiveObject;
            Selection.objects = oldSelObjects.ToArray();
            oldSelObjects.Clear();
            oldSelActiveObject = null;

            Repaint();
        }

        //
        if ( GUI.changed )
            EditorUtility.SetDirty(curEdit);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ImportObjects () {
        EditorUtility.DisplayProgressBar( "Adding Textures...", "Start adding ", 0.2f );
        curEdit.ImportObjects ( importObjects.ToArray() );
        importObjects.Clear();
        EditorUtility.ClearProgressBar();    
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void WidthAndHeightField ( ref int _width, ref int _height ) {
        _width = EditorGUILayout.IntPopup ( "Width", _width, sizeTextList, sizeList );
        _height = EditorGUILayout.IntPopup ( "Height", _height, sizeTextList, sizeList );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AddSelected ( exAtlasInfo.Element _el ) {
        if ( selectedElements.IndexOf(_el) == -1 ) {
            selectedElements.Add(_el);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ToggleSelected ( exAtlasInfo.Element _el ) {
        int i = selectedElements.IndexOf(_el);
        if ( i != -1 ) {
            selectedElements.RemoveAt(i);
        }
        else {
            selectedElements.Add(_el);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateSelectRect () {
        float x = 0;
        float y = 0;
        float width = 0;
        float height = 0;
        Vector2 curMousePos = Event.current.mousePosition;

        if ( mouseDownPos.x < curMousePos.x ) {
            x = mouseDownPos.x;
            width = curMousePos.x - mouseDownPos.x;
        }
        else {
            x = curMousePos.x;
            width = mouseDownPos.x - curMousePos.x;
        }
        if ( mouseDownPos.y < curMousePos.y ) {
            y = mouseDownPos.y;
            height = curMousePos.y - mouseDownPos.y;
        }
        else {
            y = curMousePos.y;
            height = mouseDownPos.y - curMousePos.y;
        }

        selectRect = new Rect( x, y, width, height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ConfirmRectSelection () {
        selectedElements.Clear();
        Rect mappedRect = MapToAtalsInfoField(selectRect);

        foreach ( exAtlasInfo.Element el in curEdit.elements ) {
            Rect elRect = new Rect( el.coord[0] * curEdit.scale,
                                    el.coord[1] * curEdit.scale,
                                    el.trimRect.width * curEdit.scale, 
                                    el.trimRect.height * curEdit.scale );
            if ( exContains2D.RectRect( mappedRect, elRect ) != 0 ||
                 exIntersection2D.RectRect( mappedRect, elRect ) )
            {
                selectedElements.Add (el);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void MoveSelections ( Vector2 _delta ) {
        int x_offset = 0;
        int y_offset = 0;
        curEdit.needUpdateAnimClips = true;
        curEdit.needRebuild = true;

        // register undo
        Undo.RegisterUndo ( curEdit, "MoveSelections" );

        //
        accDeltaMove += _delta;

        // check x
        if ( accDeltaMove.x > 1.0f ) {
            x_offset = Mathf.FloorToInt(accDeltaMove.x);
        }
        else if ( accDeltaMove.x < -1.0f ) {
            x_offset = Mathf.CeilToInt(accDeltaMove.x);
        }

        // check y
        if ( accDeltaMove.y > 1.0f ) {
            y_offset = Mathf.FloorToInt(accDeltaMove.y);
        }
        else if ( accDeltaMove.y < -1.0f ) {
            y_offset = Mathf.CeilToInt(accDeltaMove.y);
        }
        accDeltaMove -= new Vector2 ( x_offset, y_offset );

        // update elements
        foreach ( exAtlasInfo.Element el in selectedElements ) {
            el.coord[0] += x_offset;
            el.coord[1] += y_offset;
        }

        //
        EditorUtility.SetDirty(curEdit);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RemoveSelectedElements () {
        foreach ( exAtlasInfo.Element el in selectedElements ) {
            curEdit.RemoveElement(el);
        }
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    Rect MapToAtalsInfoField ( Rect _rect ) {
        float x = _rect.x - atlasInfoRect.x;
        float y = _rect.y - atlasInfoRect.y;
        return new Rect ( x, y, _rect.width, _rect.height );
    }
}

