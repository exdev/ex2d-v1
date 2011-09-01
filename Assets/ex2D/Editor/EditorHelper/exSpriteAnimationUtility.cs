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
// exSpriteAnimationUtility
///////////////////////////////////////////////////////////////////////////////

public static class exSpriteAnimationUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
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
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Build ( this exSpriteAnimClip _animClip ) {
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

                Debug.LogError ( "Failed to build sprite animation clip: can't find texture " 
                                 + tex2D.name 
                                 + "in exAtlasInfo." );
                fi.atlas = null;
                fi.index = -1;
                hasError = true;
            }
        }
        EditorUtility.ClearProgressBar();

        _animClip.editorNeedRebuild = hasError;
        EditorUtility.SetDirty(_animClip);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void BuildFromAtlasInfo ( exAtlasInfo _atlasInfo ) {
        EditorUtility.DisplayProgressBar( "Building Sprite Animation Clips...",
                                          "Building Sprite Animation Clips...",
                                          0.5f );    


        for ( int i = 0; i < _atlasInfo.rebuildAnimClipGUIDs.Count; ++i ) {
            string guidAnimClip = _atlasInfo.rebuildAnimClipGUIDs[i];
            exSpriteAnimClip sp = 
                exEditorHelper.LoadAssetFromGUID<exSpriteAnimClip>(guidAnimClip);
            if ( sp ) {
                // DISABLE: it is too slow { 
                // EditorUtility.DisplayProgressBar( "Building Sprite Animation Clips...",
                //                                   "Building Sprite Animation Clips " + sp.name,
                //                                   (float)i/(float)_atlasInfo.rebuildAnimClipGUIDs.Count );    
                // } DISABLE end 
                sp.editorNeedRebuild = true;
                sp.Build();
            }
        }
        _atlasInfo.rebuildAnimClipGUIDs.Clear();
        EditorUtility.ClearProgressBar();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
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
    // Desc: 
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
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void RemoveFrame ( this exSpriteAnimClip _animClip, exSpriteAnimClip.FrameInfo _fi ) {
        _animClip.frameInfos.Remove(_fi);
        _animClip.length -= _fi.length;

        exSpriteAnimationDB.RemoveFrameInfo ( _animClip, _fi );
        _animClip.editorNeedRebuild = true;
        EditorUtility.SetDirty(_animClip);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void UpdateLength ( this exSpriteAnimClip _animClip ) {
        _animClip.length = 0.0f;
        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            _animClip.length += fi.length;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
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
}
