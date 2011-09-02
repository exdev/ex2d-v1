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

///////////////////////////////////////////////////////////////////////////////
// functions
///////////////////////////////////////////////////////////////////////////////

static public class exEditorHelper {

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    static Texture2D texLine;
    static Texture2D texWhite;
    static Texture2D texCheckerboard;

    static GUIStyle styleRectBorder = null;
    static GUIStyle styleRectSelectBox = null;

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
    // Desc: 
    // ------------------------------------------------------------------ 

    public static Texture2D WhiteTexture () {
        if ( texWhite == null ) {
            string path = "Assets/ex2D/Editor/Resource/pixel.png";
            texWhite = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
            if ( texWhite == null ) {
                Debug.LogError ( "can't find texture " + path );
                return null;
            }
        }
        return texWhite;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static Texture2D CheckerboardTexture () {
        if ( texCheckerboard == null ) {
            string path = "Assets/ex2D/Editor/Resource/checkerboard_64x64.png";
            texCheckerboard = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
            if ( texCheckerboard == null ) {
                Debug.LogError ( "can't find checkerboard_64x64.png" );
                return null;
            }

        }
        return texCheckerboard;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
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
    // special styles
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static GUIStyle RectBorderStyle () {
        // create sprite select box style
        if ( styleRectBorder == null ) {
            // find box texture
            string path = "Assets/ex2D/Editor/Resource/border.png";
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
            if ( tex == null ) {
                Debug.LogError ( "can't find texture " + path );
                return null;
            }

            styleRectBorder = new GUIStyle();
            styleRectBorder.normal.background = tex;
            styleRectBorder.border = new RectOffset( 2, 2, 2, 2 );
            styleRectBorder.alignment = TextAnchor.MiddleCenter;
        }
        return styleRectBorder; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static GUIStyle RectSelectBoxStyle () {
        // create rect select box style
        if ( styleRectSelectBox == null ) {
            // find box texture
            string path = "Assets/ex2D/Editor/Resource/RectSelect.png";
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
            if ( tex == null ) {
                tex = exEditorHelper.NewBoxTexture ( path, 
                                                   new Color(0.0f, 0.6f, 1.0f, 0.2f),
                                                   new Color(0.2f, 0.2f, 0.2f, 1.0f) );
            }

            styleRectSelectBox = new GUIStyle();
            styleRectSelectBox.normal.background = tex;
            styleRectSelectBox.border = new RectOffset( 2, 2, 2, 2 );
            styleRectSelectBox.alignment = TextAnchor.MiddleCenter;
        }
        return styleRectSelectBox; 
    }


    ///////////////////////////////////////////////////////////////////////////////
    // asset path
    ///////////////////////////////////////////////////////////////////////////////

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
    // GetAssetsAtPath<exAtlas> ( "Assets", ".asset", )
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
    // Desc: 
    // ------------------------------------------------------------------ 

    public static string AssetToGUID ( Object _o ) {
        if ( _o == null )
            return "";
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_o));
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static T LoadAssetFromGUID<T> ( string _guid ) {
        if ( string.IsNullOrEmpty(_guid) )
            return (T)(object)null;
        string assetPath = AssetDatabase.GUIDToAssetPath(_guid);
        return (T)(object)AssetDatabase.LoadAssetAtPath( assetPath, typeof(T) );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
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
    // object 
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static int CompareObjectByName ( Object _a, Object _b ) {
        return string.Compare( _a.name, _b.name );
    }

    ///////////////////////////////////////////////////////////////////////////////
    // editor draw
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void DrawLine ( Vector2 _a, Vector2 _b, Color _color, float _width ) {
        if ( (_b - _a).sqrMagnitude <= 0.0001f )
            return;

        _a.x = Mathf.FloorToInt(_a.x); _a.y = Mathf.FloorToInt(_a.y);
        _b.x = Mathf.FloorToInt(_b.x); _b.y = Mathf.FloorToInt(_b.y);

        Matrix4x4 matrix = GUI.matrix;
        if ( texLine == null ) { 
            string path = "Assets/ex2D/Editor/Resource/pixel.png";
            texLine = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof(Texture2D) );
            if ( texLine == null )
                Debug.LogError("Can't find pixel.png at Assets/ex2D/Editor/Resource/");
        }

        //
        Color savedColor = GUI.color;
        GUI.color = _color;

        // 
        float angle = Vector3.Angle(_b - _a, Vector2.right);
        if ( _a.y > _b.y ) { 
            angle = -angle; 
        }
        GUIUtility.ScaleAroundPivot( new Vector2((_b - _a).magnitude, _width), 
                                     new Vector2(_a.x, _a.y + 0.5f) ); // NOTE: +0.5f can let the line stay in the center
        GUIUtility.RotateAroundPivot(angle, _a);

        // 
        GUI.DrawTexture(new Rect(_a.x, _a.y, 1.0f, 1.0f), texLine);

        // 
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
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
            EditorGUILayout.BeginHorizontal();
            s_EditedValue = EditorGUILayout.TextField(new GUIContent(_label, controlID.ToString()), s_EditedValue, EditorStyles.numberField);
            EditorGUILayout.EndHorizontal();

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
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(new GUIContent(_label, controlID.ToString()), _value.ToString(), EditorStyles.numberField);
                EditorGUILayout.EndHorizontal();

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
                EditorGUILayout.BeginHorizontal();
                s_EditedValue = EditorGUILayout.TextField(new GUIContent(_label, controlID.ToString()), s_EditedValue, EditorStyles.numberField);
                EditorGUILayout.EndHorizontal();
            }
        }

        return _value;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Menu Item
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Edit/ex2D/Unload Unused Assets")]
    static void ex2D_UnloadUnusedAssets () {
        EditorUtility.UnloadUnusedAssets();
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
