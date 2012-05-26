// ======================================================================================
// File         : exEditorHelper.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 22:44:44 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

///////////////////////////////////////////////////////////////////////////////
///
/// editor helper class
///
///////////////////////////////////////////////////////////////////////////////

public static class exEditorHelper {

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    // static Texture2D texLine; // DISABLE
    static Texture2D texWhite;
    static Texture2D texCheckerboard;
    static Texture2D texHelp;
    static Texture2D texAnimPlay;
    static Texture2D texAnimNext;
    static Texture2D texAnimPrev;

    static GUIStyle styleRectBorder = null;
    // static GUIStyle styleRectSelectBox = null; // DISABLE

    // project
    static bool projectCallbackRegistered = false;
    static bool doRenameAsset = false;
    static string renameAssetGUID = "";
    static string defaultName = "Unknown";

    ///////////////////////////////////////////////////////////////////////////////
    // project rename
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc:
    // ------------------------------------------------------------------ 

    public static void RenameProjectWindowItem ( string _guid, string _defaultName ) {
        RegisterProjectOnGUICallback();

        renameAssetGUID = _guid;
        defaultName = _defaultName;
        doRenameAsset = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void RegisterProjectOnGUICallback () {
        if ( projectCallbackRegistered == false ) {
            projectCallbackRegistered = true;
            EditorApplication.projectWindowItemOnGUI = ProjectWindowItemOnGUI;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void ProjectWindowItemOnGUI ( string _guid, Rect _selectionRect ) {
        if ( doRenameAsset ) {
            if ( _guid == renameAssetGUID ) {
                // TODO { 
                // DrawRect( _selectionRect, Color.black, Color.gray );
                EditorGUI.LabelField( _selectionRect, defaultName, "" );
                // process rename
                doRenameAsset = false;
                // } TODO end 
            }
        }
    } 

    ///////////////////////////////////////////////////////////////////////////////
    // special texture
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \return the helper texture
    /// return a help texture
    // ------------------------------------------------------------------ 

    public static Texture2D HelpTexture () {
        // NOTE: hack from "unity editor resources" in Unity3D Contents
        if ( texHelp == null ) {
            texHelp = EditorGUIUtility.FindTexture("_help");
            if ( texHelp == null ) {
                Debug.LogError ( "can't find help texture" );
                return null;
            }
        }
        return texHelp;
    }

    // ------------------------------------------------------------------ 
    /// \return the animation play texture
    /// return a animation play texture
    // ------------------------------------------------------------------ 

    public static Texture2D AnimationPlayTexture () {
        // NOTE: hack from "unity editor resources" in Unity3D Contents
        if ( texAnimPlay == null ) {
            texAnimPlay = EditorGUIUtility.FindTexture("d_animation.play");
            if ( texAnimPlay == null ) {
                Debug.LogError ( "can't find animation play texture" );
                return null;
            }
        }
        return texAnimPlay;
    }

    // ------------------------------------------------------------------ 
    /// \return the animation next texture
    /// return a animation next texture
    // ------------------------------------------------------------------ 

    public static Texture2D AnimationNextTexture () {
        // NOTE: hack from "unity editor resources" in Unity3D Contents
        if ( texAnimNext == null ) {
            texAnimNext = EditorGUIUtility.FindTexture("d_animation.nextkey");
            if ( texAnimNext == null ) {
                Debug.LogError ( "can't find animation next texture" );
                return null;
            }
        }
        return texAnimNext;
    }

    // ------------------------------------------------------------------ 
    /// \return the animation prev texture
    /// return a animation prev texture
    // ------------------------------------------------------------------ 

    public static Texture2D AnimationPrevTexture () {
        if ( texAnimPrev == null ) {
            texAnimPrev = EditorGUIUtility.FindTexture("d_animation.prevkey");
            if ( texAnimPrev == null ) {
                Debug.LogError ( "can't find animation prev texture" );
                return null;
            }
        }
        return texAnimPrev;
    }

    // ------------------------------------------------------------------ 
    /// \return the white texture
    /// return a small white texture
    // ------------------------------------------------------------------ 

    public static Texture2D WhiteTexture () {
        if ( texWhite == null ) {
            string path = "Assets/ex2D/Editor/Resource/pixel.png";
            texWhite = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
            // DISABLE: found leak problem { 
            // if ( texWhite == null )
            //     texWhite = GetTextureResource ( Path.GetFileName(path) );
            // } DISABLE end 

            if ( texWhite == null ) {
                Debug.LogError ( "can't find texture " + Path.GetFileNameWithoutExtension(path) );
                return null;
            }
        }
        return texWhite;
    }

    // ------------------------------------------------------------------ 
    /// \return the checkerboard texture
    /// return a checkerboard texture
    // ------------------------------------------------------------------ 

    public static Texture2D CheckerboardTexture () {
        if ( texCheckerboard == null ) {
            string path = "Assets/ex2D/Editor/Resource/checkerboard_64x64.png";
            texCheckerboard = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
            // DISABLE: found leak problem { 
            // if ( texCheckerboard == null ) 
            //     texCheckerboard = GetTextureResource ( Path.GetFileName(path) );
            // } DISABLE end 

            if ( texCheckerboard == null ) {
                Debug.LogError ( "can't find texture " + Path.GetFileNameWithoutExtension(path) );
                return null;
            }

        }
        return texCheckerboard;
    }

    // ------------------------------------------------------------------ 
    /// \param _path the path to save the box texture
    /// \param _fillColor the fill color
    /// \param _borderColor the border color
    /// \return the box texture
    /// create a box texture
    // ------------------------------------------------------------------ 

    public static Texture2D NewBoxTexture ( string _path, Color _fillColor, Color _borderColor ) {
        Texture2D tex = null;
        int w = 4; int h = 4;
        tex = new Texture2D( w, h );
        for ( int i = 0; i < w; ++i ) {
            for ( int j = 0; j < h; ++j ) {
                tex.SetPixel(i, j, _fillColor );
            }
        }

        tex.SetPixel(0, 0, _borderColor);
        tex.SetPixel(0, 1, _borderColor);
        tex.SetPixel(1, 0, _borderColor);

        tex.SetPixel(w-2, 0, _borderColor);
        tex.SetPixel(w-1, 0, _borderColor);
        tex.SetPixel(w-1, 1, _borderColor);

        tex.SetPixel(w-2, h-1, _borderColor);
        tex.SetPixel(w-1, h-1, _borderColor);
        tex.SetPixel(w-1, h-2, _borderColor);

        tex.SetPixel(0, h-1, _borderColor);
        tex.SetPixel(1, h-1, _borderColor);
        tex.SetPixel(0, h-2, _borderColor);

        tex.Apply( false );

        //
        byte[] pngData = tex.EncodeToPNG();
        if (pngData != null)
            File.WriteAllBytes(_path, pngData);
        Object.DestroyImmediate(tex);

        // import texture
        AssetDatabase.ImportAsset( _path );
        TextureImporter importSettings = TextureImporter.GetAtPath(_path) as TextureImporter;
        importSettings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        importSettings.textureType = TextureImporterType.GUI;
        AssetDatabase.ImportAsset( _path );
        return (Texture2D)AssetDatabase.LoadAssetAtPath(_path, typeof(Texture2D));
    }

    ///////////////////////////////////////////////////////////////////////////////
    // material
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \param _texture the material for this texture
    /// \param _shaderName the name of the shader been applied in this material
    /// \return the result material
    /// Get default material from the in texture. This function will try to
    /// find a texture material in the same directory with the same name. If
    /// no material found, it will find if there have one in Material/ folder
    /// relate to texture path. If still not found, it will create a ex2D/Alpha Blended
    /// material at Material/ and give it the same name as texture and return it.
    // ------------------------------------------------------------------ 

    public static Material GetDefaultMaterial ( Texture2D _texture, 
                                                string _shaderName = "ex2D/Alpha Blended" ) 
    {
        if ( _texture == null )
            return null;

        string texturePath = AssetDatabase.GetAssetPath(_texture);

        // load material from "texture_path/Materials/texture_name.mat"
        string materialDirectory = Path.Combine( Path.GetDirectoryName(texturePath), "Materials" );
        string materialPath = Path.Combine( materialDirectory, _texture.name 
                                            + "-" 
                                            + Path.GetExtension (texturePath).Substring(1) 
                                            + ".mat" );
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
            newMaterial = new Material( Shader.Find(_shaderName) );
            newMaterial.mainTexture = _texture;

            AssetDatabase.CreateAsset(newMaterial, materialPath);
            AssetDatabase.Refresh();
        }

        return newMaterial;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // special styles
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \return the new gui style
    /// create rect border gui style
    // ------------------------------------------------------------------ 

    public static GUIStyle RectBorderStyle () {
        // create sprite select box style
        if ( styleRectBorder == null ) {
            // find box texture
            string path = "Assets/ex2D/Editor/Resource/border.png";
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
            // DISABLE: found leak problem { 
            // if ( tex == null )
            //     tex = GetTextureResource ( Path.GetFileName(path) );
            // } DISABLE end 

            if ( tex == null ) {
                Debug.LogError ( "can't find texture " + Path.GetFileNameWithoutExtension(path) );
                return null;
            }

            styleRectBorder = new GUIStyle();
            styleRectBorder.normal.background = tex;
            styleRectBorder.border = new RectOffset( 2, 2, 2, 2 );
            styleRectBorder.alignment = TextAnchor.MiddleCenter;
        }
        return styleRectBorder; 
    }

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // /// \return the new gui style
    // /// create rect select box gui style
    // /// \code
    // /// GUI.Box ( selectRect, GUIContent.none, exEditorHelper.RectSelectBoxStyle() );
    // /// \endcode
    // // ------------------------------------------------------------------ 

    // public static GUIStyle RectSelectBoxStyle () {
    //     // create rect select box style
    //     if ( styleRectSelectBox == null ) {
    //         // find box texture
    //         string path = "Assets/ex2D/Editor/Resource/rect_select.png";
    //         Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
    //         if ( tex == null )
    //             tex = GetTextureResource ( Path.GetFileName(path) );

    //         if ( tex == null ) {
    //             // DISABLE { 
    //             // tex = exEditorHelper.NewBoxTexture ( path, 
    //             //                                    new Color(0.0f, 0.6f, 1.0f, 0.2f),
    //             //                                    new Color(0.2f, 0.2f, 0.2f, 1.0f) );
    //             // } DISABLE end 
    //             Debug.LogError ( "can't find texture " + Path.GetFileNameWithoutExtension(path) );
    //             return null;
    //         }

    //         styleRectSelectBox = new GUIStyle();
    //         styleRectSelectBox.normal.background = tex;
    //         styleRectSelectBox.border = new RectOffset( 2, 2, 2, 2 );
    //         styleRectSelectBox.alignment = TextAnchor.MiddleCenter;
    //     }
    //     return styleRectSelectBox; 
    // }
    // } DISABLE end 

    ///////////////////////////////////////////////////////////////////////////////
    // Assets
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \return the result path
    /// get the current directory of the active selected object
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
    /// \param _path the save path
    /// \param _suffix the suffix you would like to search 
    /// \param _recursively if recursively search the directory
    /// \return get all assets at _path
    /// \code
    /// exAtlas[] atlasList = GetAssetsAtPath<exAtlas> ( "Assets", ".asset", true );
    /// \endcode
    // ------------------------------------------------------------------ 

    public static T[] GetAssetsAtPath<T> ( string _path, string _suffix = "", bool _recursively = true ) {
        List<T> assets = new List<T>();

        // Process the list of files found in the directory.
        string [] files = Directory.GetFiles(_path, "*" + _suffix);
        foreach ( string fileName in files ) {
            T asset = (T)(object)AssetDatabase.LoadAssetAtPath( fileName, typeof(T) );
            if ( asset != null ) {
                assets.Add(asset);
            }
        }

        // Recurse into subdirectories of this directory.
        if ( _recursively ) {
            string [] dirs = Directory.GetDirectories(_path);
            foreach( string dirName in dirs ) {
                assets.AddRange( GetAssetsAtPath<T> ( dirName, _suffix, true ) );
            }
        }

        return assets.ToArray();
    }

    // ------------------------------------------------------------------ 
    /// \param _o the asset object
    /// \return the guid
    /// get the guid of the asset path, if not found, return empty string
    // ------------------------------------------------------------------ 

    public static string AssetToGUID ( Object _o ) {
        if ( _o == null )
            return "";
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_o));
    }

    // ------------------------------------------------------------------ 
    /// \param _guid asset path guid
    /// \return the asset
    /// load the asset from path guid
    // ------------------------------------------------------------------ 

    public static T LoadAssetFromGUID<T> ( string _guid ) {
        if ( string.IsNullOrEmpty(_guid) )
            return (T)(object)null;
        string assetPath = AssetDatabase.GUIDToAssetPath(_guid);
        return (T)(object)AssetDatabase.LoadAssetAtPath( assetPath, typeof(T) );
    }

    // ------------------------------------------------------------------ 
    /// \param _o the asset object
    /// \return the result
    /// check if the asset is a directory
    // ------------------------------------------------------------------ 

    public static bool IsDirectory ( Object _o ) {
        string path = AssetDatabase.GetAssetPath(_o);
        if ( string.IsNullOrEmpty(path) == false ) {
            DirectoryInfo info = new DirectoryInfo(path);
            return info.Exists;
        }
        return false;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // load assembly resource
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static Stream GetResourceStream ( string _name, Assembly _assembly ) {

        if ( _assembly == null ) {
            _assembly = Assembly.GetExecutingAssembly ();
        }

        return _assembly.GetManifestResourceStream (_name);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static Stream GetResourceStream ( string _name ) {
        return GetResourceStream (_name, null);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static byte[] GetByteResource ( string _name, Assembly _assembly ) {

        Stream byteStream = GetResourceStream (_name, _assembly);
        byte[] buffer = new byte[byteStream.Length];
        byteStream.Read (buffer, 0, (int)byteStream.Length);
        byteStream.Close ();

        return buffer;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static byte[] GetByteResource ( string _name ) {
        return GetByteResource (_name, null);
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of assembled resource
    /// \param _assembly assembly
    /// \return loaded texture2d
    /// load texture from assembly
    // ------------------------------------------------------------------ 

    public static Texture2D GetTextureResource ( string _name, Assembly _assembly ) {
        Texture2D texture = new Texture2D (4, 4);
        texture.LoadImage (GetByteResource (_name, _assembly));

        return texture;
    }

    // ------------------------------------------------------------------ 
    /// \param _name the name of assembled resource
    /// \return loaded texture2d
    /// load texture from assembly
    // ------------------------------------------------------------------ 

    public static Texture2D GetTextureResource ( string _name ) {
        return GetTextureResource (_name, null);
    }

    ///////////////////////////////////////////////////////////////////////////////
    // object 
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \param _a object one
    /// \param _b object two
    /// \return the compare result
    /// compare object by name, 0 = equal, -1 = _a < _b, 1 = _a > _b
    // ------------------------------------------------------------------ 

    public static int CompareObjectByName ( Object _a, Object _b ) {
        return string.Compare( _a.name, _b.name );
    }

    ///////////////////////////////////////////////////////////////////////////////
    // editor draw
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \param _a the start point
    /// \param _b the end point
    /// \param _color the color of the line
    /// \param _width the width of the line
    /// draw a line in editor
    // ------------------------------------------------------------------ 

    public static void DrawLine ( Vector2 _a, Vector2 _b, Color _color, float _width ) {

        // DISABLE { 
        // if ( (_b - _a).sqrMagnitude <= 0.0001f )
        //     return;

        // _a.x = Mathf.FloorToInt(_a.x); _a.y = Mathf.FloorToInt(_a.y);
        // _b.x = Mathf.FloorToInt(_b.x); _b.y = Mathf.FloorToInt(_b.y);

        // Matrix4x4 matrix = GUI.matrix;
        // if ( texLine == null ) { 
        //     string path = "Assets/ex2D/Editor/Resource/pixel.png";
        //     texLine = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
        //     if ( texLine == null )
        //         Debug.LogError("Can't find pixel.png at Assets/ex2D/Editor/Resource/");
        // }

        // //
        // Color savedColor = GUI.color;
        // GUI.color = _color;

        // // 
        // float angle = Vector3.Angle(_b - _a, Vector2.right);
        // if ( _a.y > _b.y ) { 
        //     angle = -angle; 
        // }
        // GUIUtility.ScaleAroundPivot( new Vector2((_b - _a).magnitude, _width), 
        //                              new Vector2(_a.x, _a.y + 0.5f) ); // NOTE: +0.5f can let the line stay in the center
        // GUIUtility.RotateAroundPivot(angle, _a);

        // // 
        // GUI.DrawTexture(new Rect(_a.x, _a.y, 1.0f, 1.0f), texLine);

        // // 
        // GUI.matrix = matrix;
        // GUI.color = savedColor;
        // } DISABLE end 

        if ( (_b - _a).sqrMagnitude <= 0.0001f )
            return;

        _a.x = Mathf.FloorToInt(_a.x); _a.y = Mathf.FloorToInt(_a.y);
        _b.x = Mathf.FloorToInt(_b.x); _b.y = Mathf.FloorToInt(_b.y);

        //
        Color savedColor = Handles.color;
        Handles.color = _color;

        // 
        if ( _width > 1.0f ) {
            Handles.DrawAAPolyLine(_width, new Vector3[] { _a, _b } );
        }
        else {
            Handles.DrawLine( _a, _b );
        }

        // 
        Handles.color = savedColor;
    }

    // ------------------------------------------------------------------ 
    /// \param _rect the rect
    /// \param _backgroundColor the background color of the rect
    /// \param _borderColor the border color of the rect
    /// draw a rect in editor
    // ------------------------------------------------------------------ 

    public static void DrawRect ( Rect _rect, Color _backgroundColor, Color _borderColor ) {
        // backgroundColor
        Color old = GUI.color;
        GUI.color = _backgroundColor;
            GUI.DrawTexture( _rect, exEditorHelper.WhiteTexture() );
        GUI.color = old;

        // border
        old = GUI.backgroundColor;
        GUI.backgroundColor = _borderColor;
            GUI.Box ( _rect, GUIContent.none, exEditorHelper.RectBorderStyle() );
        GUI.backgroundColor = old;

        // Vector3[] verts = new Vector3 [] {
        //     new Vector3( _rect.x,    _rect.y,    0.0f ), 
        //     new Vector3( _rect.xMax, _rect.y,    0.0f ), 
        //     new Vector3( _rect.xMax, _rect.yMax, 0.0f ), 
        //     new Vector3( _rect.x,    _rect.yMax, 0.0f ),
        // };
        // Handles.DrawSolidRectangleWithOutline ( verts, _backgroundColor, _borderColor );
    }

    ///////////////////////////////////////////////////////////////////////////////
    // GUI enhancement
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // copy from: http://forum.unity3d.com/threads/55434-Custom-inspector-resize-array-intfield-problems
    // ------------------------------------------------------------------ 

    public static string s_EditedValue = string.Empty;
    public static string s_LastTooltip = string.Empty;
    public static int s_EditedField = 0;

    // Creates an special IntField that only chages th actual value when pressing enter or losing focus
    public static int IntField(string _label, int _value) {
        // Get current control id
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        // Assign real _value if out of focus or enter pressed, 
        // the edited _value cannot be empty and the tooltip must match to the current control
        if ( (controlID.ToString() == s_LastTooltip && s_EditedValue != string.Empty) &&
             ((Event.current.type == EventType.KeyDown && Event.current.character == '\n') || 
              (Event.current.type == EventType.MouseDown)) )
        {
            // Draw textfield, somehow this makes it work better when pressing enter
            // No idea why...
            GUILayout.BeginHorizontal();
                s_EditedValue = EditorGUILayout.TextField(new GUIContent(_label, controlID.ToString()), s_EditedValue, EditorStyles.numberField);
            GUILayout.EndHorizontal();

            // Parse number
            int number = 0;
            if (int.TryParse(s_EditedValue, out number))
            {
                _value = number;
            }

            // Reset values, the edite _value must go bak to its orginal state
            s_EditedValue = _value.ToString();
            s_EditedField = 0;
            return _value;
        }
        else
        {
            // Only draw this if the field is not being edited
            if (s_EditedField != controlID)
            {
                // Draw textfield with current original _value
                GUILayout.BeginHorizontal();
                    EditorGUILayout.TextField(new GUIContent(_label, controlID.ToString()), _value.ToString(), EditorStyles.numberField);
                GUILayout.EndHorizontal();

                // Save last tooltip if gets focus... also save control id
                if (GUI.tooltip == controlID.ToString())
                {
                    s_LastTooltip = GUI.tooltip;
                    s_EditedField = controlID;
                }
            }
            else
            {
                // Draw textfield, now with current edited _value
                GUILayout.BeginHorizontal();
                    s_EditedValue = EditorGUILayout.TextField(new GUIContent(_label, controlID.ToString()), s_EditedValue, EditorStyles.numberField);
                GUILayout.EndHorizontal();
            }
        }

        return _value;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Menu Item
    ///////////////////////////////////////////////////////////////////////////////

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // [MenuItem ("Edit/ex2D/Unload Unused Assets")]
    // static void ex2D_UnloadUnusedAssets () {
    //     EditorUtility.UnloadUnusedAssets();
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Edit/ex2D/Rebuild Prefabs")]
    static void ex2D_RebuildPrefabs () {
        try {
            EditorUtility.DisplayProgressBar( "Rebuilding Prefabs...", "Rebuilding...", 0.5f );    
            List<exSpriteBase> sprites = new List<exSpriteBase>();
            GetSpritesFromPrefabs ( "Assets", ref sprites );
            RebuildSprites(sprites.ToArray());
            sprites.Clear();
            EditorUtility.UnloadUnusedAssets();
            EditorUtility.ClearProgressBar();    
        }
        catch ( System.Exception ) {
            EditorUtility.ClearProgressBar();    
            throw;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void GetSpritesFromPrefabs ( string _path, ref List<exSpriteBase> _sprites ) {
        // Process the list of files found in the directory.
        string [] files = Directory.GetFiles(_path, "*.prefab");
        foreach ( string fileName in files ) {
            Object prefab = (Object)AssetDatabase.LoadAssetAtPath( fileName, typeof(Object) );
            if ( prefab ) {
                Object [] objs = EditorUtility.CollectDeepHierarchy ( new Object [] {prefab} );
                foreach ( Object o in objs ) {
                    GameObject go = o as GameObject;
                    if ( go != null ) {
                        exSpriteBase sp = go.GetComponent<exSpriteBase>();
                        if ( sp != null )
                            _sprites.Add(sp);
                    }
                }
            }
        }

        // Recurse into subdirectories of this directory.
        string [] dirs = Directory.GetDirectories(_path);
        foreach( string dirName in dirs ) {
            GetSpritesFromPrefabs ( dirName, ref _sprites );
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _sprites the list of sprites to rebuild
    /// rebuild the listed sprites
    // ------------------------------------------------------------------ 

    public static void RebuildSprites ( exSpriteBase[] _sprites ) {
        try {
            EditorUtility.DisplayProgressBar( "Rebuild Scene Sprites...", 
                                              "Rebuild Scene Sprites...", 
                                              0.5f );    

            for ( int i = 0; i < _sprites.Length; ++i ) {
                exSpriteBase spBase = _sprites[i]; 
                // DISABLE: it is too slow { 
                // float progress = (float)i/(float)_sprites.Length;
                // EditorUtility.DisplayProgressBar( "Rebuild Scene Sprites...", 
                //                                   "Build Sprite " + spBase.gameObject.name, progress );    
                // } DISABLE end 

                // if sprite
                if ( spBase is exSprite ) {
                    exSprite sp = spBase as exSprite;
                    exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo(sp.textureGUID);
                    exSpriteEditor.UpdateAtlas( sp, elInfo );

                    Texture2D texture = null;
                    if ( sp.useAtlas == false ) {
                        texture = exEditorHelper.LoadAssetFromGUID<Texture2D>(sp.textureGUID );
                    }
                    sp.Build(texture);
                }

#if !(EX2D_EVALUATE)
                // if sprite font
                if ( spBase is exSpriteFont ) {
                    exSpriteFont spFont = spBase as exSpriteFont;
                    spFont.Build();
                }

                // if sprite border
                if ( spBase is exSpriteBorder ) {
                    exSpriteBorder spBorder = spBase as exSpriteBorder; 

                    Texture2D texture = null;
                    if ( spBorder.guiBorder ) {
                        exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo(spBorder.guiBorder.textureGUID);
                        exSpriteBorderEditor.UpdateAtlas( spBorder, elInfo );

                        if ( spBorder.useAtlas == false ) {
                            texture = exEditorHelper.LoadAssetFromGUID<Texture2D>(spBorder.guiBorder.textureGUID );
                        }
                    }
                    spBorder.Build(texture);
                }
#endif // EX2D_EVALUATE
            }
            EditorUtility.ClearProgressBar();    
        }
        catch ( System.Exception ) {
            EditorUtility.ClearProgressBar();    
            throw;
        }
    }

    // TEMP { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // [MenuItem ("ex2D/Temp Test")]
    // static void Temp () {
    // }
    // } TEMP end 
}
