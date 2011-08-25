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

public class exSpriteAnimationUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void Build ( exSpriteAnimClip _animClip ) {
        int i = 0;
        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            exAtlasInfo.Element el = exAtlasDB.GetElement (fi.textureGUID);
            if ( el != null ) {
                // DISABLE: it is too slow { 
                // EditorUtility.DisplayProgressBar( "Building Sprite Animation Clip...",
                //                                   "Building FrameInfo " + el.texture.name,
                //                                   (float)i/(float)_animClip.frameInfos.Count );    
                // } DISABLE end 
                fi.atlas = el.atlasInfo.atlas;
                fi.index = el.atlasInfo.elements.IndexOf(el);
            }
            else {
                string texturePath = AssetDatabase.GUIDToAssetPath(fi.textureGUID);
                Texture2D tex2D = (Texture2D)AssetDatabase.LoadAssetAtPath( texturePath, typeof(Texture2D));

                Debug.LogError ( "Failed to build sprite animation clip: can't find texture " 
                                 + tex2D.name 
                                 + "in exAtlasInfo." );
                fi.atlas = null;
                fi.index = -1;
            }
            ++i;
        }
        EditorUtility.ClearProgressBar();

        _animClip.editorNeedRebuild = false;
        EditorUtility.SetDirty(_animClip);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void BuildFromAtlasInfo ( exAtlasInfo _atlasInfo ) {
        EditorUtility.DisplayProgressBar( "Building Sprite Animation Clips...",
                                          "Building Sprite Animation Clips...",
                                          0.5f );    

        for ( int i = 0; i < _atlasInfo.rebuildSpAnimClips.Count; ++i ) {
            exSpriteAnimClip sp = _atlasInfo.rebuildSpAnimClips[i];
            if ( sp ) {
                // DISABLE: it is too slow { 
                // EditorUtility.DisplayProgressBar( "Building Sprite Animation Clips...",
                //                                   "Building Sprite Animation Clips " + sp.name,
                //                                   (float)i/(float)_atlasInfo.rebuildSpAnimClips.Count );    
                // } DISABLE end 
                Build(sp);
            }
        }
        _atlasInfo.rebuildSpAnimClips.Clear();
        EditorUtility.ClearProgressBar();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddFrame ( exSpriteAnimClip _animClip, Texture2D _tex ) {
        exSpriteAnimClip.FrameInfo frameInfo = new exSpriteAnimClip.FrameInfo ();
        frameInfo.length = 10.0f/60.0f;
        frameInfo.textureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_tex));
        exAtlasInfo.Element el = exAtlasDB.GetElement (frameInfo.textureGUID);
        if ( el != null ) {
            frameInfo.atlas = el.atlasInfo.atlas;
            frameInfo.index = el.atlasInfo.elements.IndexOf(el);
        }
        else {
            frameInfo.atlas = null;
            frameInfo.index = -1;
        }
        _animClip.frameInfos.Add(frameInfo);
        _animClip.length += frameInfo.length;

        exSpriteAnimationDB.UpdateDataBase ( _animClip, frameInfo );
        _animClip.editorNeedRebuild = true;
        EditorUtility.SetDirty(_animClip);
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveFrame ( exSpriteAnimClip _animClip, exSpriteAnimClip.FrameInfo _fi ) {
        _animClip.frameInfos.Remove(_fi);
        _animClip.length -= _fi.length;

        exSpriteAnimationDB.OnFrameInfoRemoved ( _animClip, _fi );
        _animClip.editorNeedRebuild = true;
        EditorUtility.SetDirty(_animClip);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void Update ( exSpriteAnimClip _animClip ) {
        _animClip.length = 0.0f;
        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            _animClip.length += fi.length;
        }
    }
}
