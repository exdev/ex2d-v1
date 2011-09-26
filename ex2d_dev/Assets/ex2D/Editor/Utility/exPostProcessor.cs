// ======================================================================================
// File         : exPostProcessor.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 22:07:41 PM | Saturday,August
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
// class ex2D_PostProcessor 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class ex2D_PostProcessor : AssetPostprocessor {

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void OnPostprocessAllAssets ( string[] _importedAssets,
                                         string[] _deletedAssets,
                                         string[] _movedAssets,
                                         string[] _movedFromAssetPaths ) 
    {
        List<exAtlasInfo> updateAtlasInfos = new List<exAtlasInfo>();

        // ======================================================== 
        // import assets 
        // ======================================================== 

        foreach ( string path in _importedAssets ) {
            // check if we are .ex2D_AtlasDB or .ex2D_SpriteAnimationDB
            if ( string.Equals(path, exAtlasDB.dbPath, System.StringComparison.CurrentCultureIgnoreCase) || 
                 string.Equals(path, exSpriteAnimationDB.dbPath, System.StringComparison.CurrentCultureIgnoreCase) )
            {
                continue;
            }

            // check if we are asset
            string ext = Path.GetExtension(path);
            if ( ext != ".asset" &&
                 ext != ".png" &&
                 ext != ".jpg" &&
                 ext != ".tga" )
                continue;

            //
            Object obj = (Object)AssetDatabase.LoadAssetAtPath ( path, typeof(Object) );
            if ( obj == null )
                continue;

            // ======================================================== 
            // Texture2D 
            // NOTE: if we are OnWillSaveAssets, it is possible we reimport 
            //       textures if they are not valid for atlas. and when import it will
            //       trigger things here which will lead to infinite cycle.
            // ======================================================== 

            if ( ex.onSavingAssets == false && obj is Texture2D ) {
                Texture2D tex2d = obj as Texture2D;
                exAtlasDB.ElementInfo ei = exAtlasDB.GetElementInfo(tex2d);
                if ( ei != null ) {
                    exAtlasInfo atlasInfo = exEditorHelper.LoadAssetFromGUID<exAtlasInfo>(ei.guidAtlasInfo);
                    if ( atlasInfo && updateAtlasInfos.IndexOf(atlasInfo) == -1 ) {
                        updateAtlasInfos.Add(atlasInfo);
                    }
                }
                continue;
            }

            // ======================================================== 
            // exAtlasInfo
            // ======================================================== 

            if ( obj is exAtlasInfo ) {
                exAtlasInfo atlasInfo = obj as exAtlasInfo;
                exAtlasDB.AddAtlasInfo(atlasInfo);
                // Debug.Log( "add atlas " + path ); // DEBUG
            }

            // ======================================================== 
            // exSpriteAnimClip
            // ======================================================== 

            if ( obj is exSpriteAnimClip ) {
                exSpriteAnimClip spAnimClip = obj as exSpriteAnimClip;
                exSpriteAnimationDB.AddSpriteAnimClip(spAnimClip);
                // Debug.Log( "add sprite anim clip " + path ); // DEBUG
            }
        }

        // build exAtlasInfo first
        foreach ( exAtlasInfo atlasInfo in updateAtlasInfos ) {
            //  NOTE: Build atlas without import texture, without this, we will crash when changing import settings of a texture and rebuild it.
            exAtlasInfoUtility.Build(atlasInfo,true);
            // NOTE: no need to update scene sprite and sprite animation clip, because we didn't change index
        }

        // ======================================================== 
        // deleted assets
        // ======================================================== 

        List<string> atlasInfoGUIDs = new List<string>();
        foreach ( string path in _deletedAssets ) {
            // check if we are .ex2D_AtlasDB or .ex2D_SpriteAnimationDB
            if ( string.Equals(path, exAtlasDB.dbPath, System.StringComparison.CurrentCultureIgnoreCase) || 
                 string.Equals(path, exSpriteAnimationDB.dbPath, System.StringComparison.CurrentCultureIgnoreCase) )
            {
                continue;
            }

            // check if we are asset
            if ( Path.GetExtension(path) != ".asset" )
                continue;

            // 
            string guid = AssetDatabase.AssetPathToGUID(path);

            // check if we have the guid in the exAtlasInfo
            if ( exAtlasDB.HasAtlasInfoGUID( guid ) ) {
                exAtlasDB.RemoveAtlasInfo(guid);
                atlasInfoGUIDs.Add(guid);
                // Debug.Log( "remove atlas " + path ); // DEBUG
            }
            // check if we have the guid in the exSpriteAnimClip
            else if ( exSpriteAnimationDB.HasSpriteAnimClipGUID( guid ) ) {
                exSpriteAnimationDB.RemoveSpriteAnimClip(guid);
                // Debug.Log( "remove sprite anim clip " + path ); // DEBUG
            }
        }
        exSceneHelper.UpdateSceneSprites(atlasInfoGUIDs);

        // DISABLE { 
        // for ( int i = 0; i < _movedAssets.Length; ++i )
        //     Debug.Log("Moved Asset: " + _movedAssets[i] + " from: " + _movedFromAssetPaths[i]);
        // } DISABLE end 
    }
}

///////////////////////////////////////////////////////////////////////////////
// class ex2D_SaveAssetsProcessor 
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

class ex2D_SaveAssetsProcessor : SaveAssetsProcessor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void OnWillSaveAssets ( string[] _paths ) {
        ex.onSavingAssets = true;
        List<exAtlasInfo> rebuildAtlasInfos = new List<exAtlasInfo>();
        List<exSpriteAnimClip> rebuildSpriteAnimClips = new List<exSpriteAnimClip>();
        List<exGUIBorder> rebuildGuiBorders = new List<exGUIBorder>();

        //
        foreach ( string path in _paths ) {
            // check if we are .ex2D_AtlasDB or .ex2D_SpriteAnimationDB
            if ( string.Equals(path, exAtlasDB.dbPath, System.StringComparison.CurrentCultureIgnoreCase) || 
                 string.Equals(path, exSpriteAnimationDB.dbPath, System.StringComparison.CurrentCultureIgnoreCase) )
            {
                continue;
            }

            // check if we are asset
            if ( Path.GetExtension(path) != ".asset" )
                continue;

            Object obj = (Object)AssetDatabase.LoadAssetAtPath ( path, typeof(Object) );
            if ( obj == null )
                continue;

            // ======================================================== 
            // build exAtlasInfo 
            // ======================================================== 

            if ( obj is exAtlasInfo ) {
                exAtlasInfo atlasInfo = obj as exAtlasInfo;
                if ( atlasInfo.needRebuild &&
                     rebuildAtlasInfos.IndexOf(atlasInfo) == -1 ) 
                {
                    rebuildAtlasInfos.Add(atlasInfo);
                }
            }

            // ======================================================== 
            // build exSpriteAnimClip 
            // ======================================================== 

            if ( obj is exSpriteAnimClip ) {
                exSpriteAnimClip spAnimClip = obj as exSpriteAnimClip;
                if ( spAnimClip.editorNeedRebuild )
                    rebuildSpriteAnimClips.Add(spAnimClip);
            }

            // TODO { 
            // // ======================================================== 
            // // build exBitmapFont 
            // // ======================================================== 

            // if ( obj is exBitmapFont ) {
            //     exBitmapFont bitmapFont = obj as exBitmapFont;
            //     if ( bitmapFont.editorNeedRebuild )
            //         Object fontInfo = exEditorHelper.LoadAssetFromGUID(bitmapFont.fontinfoGUID);
            //         bitmapFont.Build( fontInfo );
            // }
            // } TODO end 

            // ======================================================== 
            // build exGUIBorder 
            // ======================================================== 

            if ( obj is exGUIBorder ) {
                exGUIBorder guiBorder = obj as exGUIBorder;
                if ( guiBorder.editorNeedRebuild &&  
                     rebuildGuiBorders.IndexOf(guiBorder) == -1 ) {
                    guiBorder.editorNeedRebuild = false;
                    EditorUtility.SetDirty(guiBorder);
                    rebuildGuiBorders.Add(guiBorder);
                }
            }
        }

        // NOTE: we need to make sure exAtlasInfo build before exSpriteAnimClip,
        //       because during build, exAtlasDB will update ElementInfo, and exSpriteAnimClip need this for checking error. 

        // ======================================================== 
        // build exAtlasInfo first
        // ======================================================== 

        foreach ( exAtlasInfo atlasInfo in rebuildAtlasInfos ) {
            exAtlasInfoUtility.Build(atlasInfo);
            // build sprite animclip that used this atlasInfo
            exAtlasInfoUtility.BuildSpAnimClipsFromRebuildList(atlasInfo);
        }

        // ======================================================== 
        // build exSpriteAnimClip 
        // ======================================================== 

        foreach ( exSpriteAnimClip spAnimClip in rebuildSpriteAnimClips ) {
            // NOTE: it could be built in BuildSpAnimClipsFromRebuildList above
            if ( spAnimClip.editorNeedRebuild )
                spAnimClip.Build();
        }

        // ======================================================== 
        // post build 
        // ======================================================== 

        // update scene sprites with rebuild atlasInfo list
        List<string> rebuildAtlasInfoGUIDs = new List<string>();
        foreach ( exAtlasInfo atlasInfo in rebuildAtlasInfos ) {
            rebuildAtlasInfoGUIDs.Add( exEditorHelper.AssetToGUID(atlasInfo) );
        }
        exSceneHelper.UpdateSceneSprites (rebuildAtlasInfoGUIDs);

        // update scene sprites with rebuild guiBorder list
        exSceneHelper.UpdateSceneSprites (rebuildGuiBorders);

        // NOTE: without this you will got leaks message
        EditorUtility.UnloadUnusedAssets();
        ex.onSavingAssets = false;
    }
}
