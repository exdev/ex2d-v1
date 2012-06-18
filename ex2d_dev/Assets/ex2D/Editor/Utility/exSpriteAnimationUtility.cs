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
/// the sprite animation utility
///
///////////////////////////////////////////////////////////////////////////////

public static class exSpriteAnimationUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/SpriteAnimation Object")]
    static void CreateSpriteAnimationObject () {
        GameObject go = new GameObject("SpriteAnimationObject");
        go.AddComponent<exSpriteAnimation>();
        Selection.activeObject = go;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D/Sprite Animation")]
    static void Create () {
        string assetPath = exEditorHelper.GetCurrentDirectory();
        string assetName = "New SpriteAnimation";

        bool doCreate = true;
        string path = Path.Combine( assetPath, assetName + ".asset" );
        FileInfo fileInfo = new FileInfo(path);
        if ( fileInfo.Exists ) {
            doCreate = EditorUtility.DisplayDialog( assetName + " already exists.",
                                                    "Do you want to overwrite the old one?",
                                                    "Yes", "No" );
        }
        if ( doCreate ) {
            exSpriteAnimClip clip = exSpriteAnimationUtility.CreateSpriteAnimClip ( assetPath, assetName );
            Selection.activeObject = clip;
            // EditorGUIUtility.PingObject(border);
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _path the directory path to save the atlas
    /// \param _name the name of the atlas
    /// \return the sprite animation clip asset
    /// create the sprite animation clip in the _path, save it as _name.
    // ------------------------------------------------------------------ 

    public static exSpriteAnimClip CreateSpriteAnimClip ( string _path, string _name ) {
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
        exSpriteAnimClip newAnimClip = ScriptableObject.CreateInstance<exSpriteAnimClip>();
        AssetDatabase.CreateAsset(newAnimClip, assetPath);
        Selection.activeObject = newAnimClip;
        return newAnimClip;
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animatoin clip
    /// build the sprite animation clip
    // ------------------------------------------------------------------ 

    public static void Build ( this exSpriteAnimClip _animClip ) {
        try {
            EditorUtility.DisplayProgressBar( "Building Sprite Animation Clip " + _animClip.name,
                                              "Building Frames...",
                                              0.1f );

            bool hasError = false;
            foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
                exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo (fi.textureGUID);
                if ( elInfo != null ) {
                    fi.atlas = exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
                    fi.index = elInfo.indexInAtlas;
                }
                else {
                    string texturePath = AssetDatabase.GUIDToAssetPath(fi.textureGUID);
                    Texture2D tex2D = (Texture2D)AssetDatabase.LoadAssetAtPath( texturePath, typeof(Texture2D));
                    Debug.LogError ( "Failed to build sprite animation clip: " + _animClip.name + ", can't find texture " 
                                     + ((tex2D != null) ? tex2D.name : "null") 
                                     + " in exAtlasInfo." );
                    fi.atlas = null;
                    fi.index = -1;
                    hasError = true;
                }
            }
            EditorUtility.ClearProgressBar();

            _animClip.editorNeedRebuild = hasError;
            EditorUtility.SetDirty(_animClip);
        }
        catch ( System.Exception ) {
            EditorUtility.ClearProgressBar();
            throw;
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animatoin clip
    /// \param _objects the texture objects
    /// add textures to sprite animation clip as new frames
    // ------------------------------------------------------------------ 

    public static void AddFrames ( this exSpriteAnimClip _animClip, Object[] _objects ) {
        foreach ( Object o in _objects ) {
            if ( o is Texture2D ) {
                Texture2D t = o as Texture2D;
                _animClip.AddFrame(t); // NOTE: it will SetDirty here
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animatoin clip
    /// \param _tex the texture
    /// add texture to sprite animation clip as new frame
    // ------------------------------------------------------------------ 

    public static void AddFrame ( this exSpriteAnimClip _animClip, Texture2D _tex ) {
        exSpriteAnimClip.FrameInfo frameInfo = new exSpriteAnimClip.FrameInfo ();
        frameInfo.length = 10.0f/60.0f;
        frameInfo.textureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_tex));
        exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo (frameInfo.textureGUID);
        if ( elInfo != null ) {
            frameInfo.atlas = exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
            frameInfo.index = elInfo.indexInAtlas;
        }
        else {
            frameInfo.atlas = null;
            frameInfo.index = -1;
        }
        _animClip.frameInfos.Add(frameInfo);
        _animClip.length += frameInfo.length;

        exSpriteAnimationDB.AddFrameInfo ( _animClip, frameInfo );
        _animClip.editorNeedRebuild = true;
        EditorUtility.SetDirty(_animClip);
    } 

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animatoin clip
    /// \param _fi frame info
    /// remove frame info from sprite animation clip
    // ------------------------------------------------------------------ 

    public static void RemoveFrame ( this exSpriteAnimClip _animClip, exSpriteAnimClip.FrameInfo _fi ) {
        _animClip.frameInfos.Remove(_fi);
        _animClip.length -= _fi.length;

        exSpriteAnimationDB.RemoveFrameInfo ( _animClip, _fi );
        _animClip.editorNeedRebuild = true;
        EditorUtility.SetDirty(_animClip);
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animatoin clip
    /// update the length of the sprite animation clip by frameinfo list
    // ------------------------------------------------------------------ 

    public static void UpdateLength ( this exSpriteAnimClip _animClip ) {
        float totalLength = 0.0f;
        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            totalLength += fi.length;
        }
        _animClip.length = totalLength;
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animatoin clip
    /// \param _seconds the expect seconds
    /// \param _wrapMode the wrap mode
    /// get frame info in _animClip by the input seconds
    // ------------------------------------------------------------------ 

    public static exSpriteAnimClip.FrameInfo GetFrameInfoBySeconds ( this exSpriteAnimClip _animClip, 
                                                                     float _seconds, 
                                                                     WrapMode _wrapMode ) {
        float t = _animClip.WrapSeconds(_seconds, _wrapMode);

        //
        float totalTime = 0.0f;
        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            totalTime += fi.length;
            if ( t <= totalTime )
                return fi;
        }
        return null;
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animation clip
    /// \param _seconds the expect seconds
    /// \return the snapped seconds
    // ------------------------------------------------------------------ 

    public static float SnapToSeconds ( this exSpriteAnimClip _animClip, 
                                        float _seconds ) {

        float unitSeconds = 1.0f/_animClip.sampleRate;
        float fraction = _seconds % unitSeconds;
        if ( fraction > 0.5f * unitSeconds )
            _seconds = _seconds - fraction + unitSeconds;
        else
            _seconds = _seconds - fraction; 
        return _seconds;
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animation clip
    /// \param _seconds the expect seconds
    /// \return the snapped frames
    // ------------------------------------------------------------------ 

    public static int SnapToFrames ( this exSpriteAnimClip _animClip, 
                                     float _seconds ) {

        float unitSeconds = 1.0f/_animClip.sampleRate;
        return Mathf.RoundToInt(_seconds/unitSeconds);
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animation clip
    /// \param _frames the frames
    /// \return the seconds
    // ------------------------------------------------------------------ 

    public static float FrameToSeconds ( this exSpriteAnimClip _animClip, int _frames ) {
        return _frames / _animClip.sampleRate;
    } 
}
