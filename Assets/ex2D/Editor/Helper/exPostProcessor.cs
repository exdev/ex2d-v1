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
        foreach ( string path in _importedAssets ) {
            Object obj;
            obj = (Object)AssetDatabase.LoadAssetAtPath ( path, typeof(Object) );
            if ( obj == null )
                continue;

            // ======================================================== 
            // exAtlasInfo
            // ======================================================== 

            if ( obj is exAtlasInfo ) {
                exAtlasInfo atlasInfo = obj as exAtlasInfo;
                exAtlasDB.AddAtlas(atlasInfo);
            }

            // ======================================================== 
            // exSpriteAnimClip
            // ======================================================== 

            if ( obj is exSpriteAnimClip ) {
                exSpriteAnimClip spAnimClip = obj as exSpriteAnimClip;
                exSpriteAnimationDB.AddSpriteAnimClip(spAnimClip);
            }
        }

        //
        foreach ( string path in _deletedAssets ) {
            if ( string.Equals(path, exAtlasDB.dbPath, System.StringComparison.CurrentCultureIgnoreCase) || 
                 string.Equals(path, exSpriteAnimationDB.dbPath, System.StringComparison.CurrentCultureIgnoreCase) )
            {
                continue;
            }

            if ( Path.GetExtension(path) == ".asset" ) {
                string guid = AssetDatabase.AssetPathToGUID(path);

                // check if we have the guid in the exAtlasInfo
                if ( exAtlasDB.HasAtlasGUID( guid ) ) {
                    exAtlasDB.RemoveAtlas(guid);
                }
                // check if we have the guid in the exSpriteAnimClip
                else if ( exSpriteAnimationDB.HasSpriteAnimClipGUID( guid ) ) {
                    exSpriteAnimationDB.RemoveSpriteAnimClip(guid);
                }
            }
        }

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
        List<exAtlasInfo> rebuildAtlasInfos = new List<exAtlasInfo>();

        foreach ( string path in _paths ) {
            Object obj = (Object)AssetDatabase.LoadAssetAtPath ( path, typeof(Object) );
            if ( obj == null )
                continue;

            // ======================================================== 
            // build exAtlasInfo 
            // ======================================================== 

            if ( obj is exAtlasInfo ) {
                exAtlasInfo atlasInfo = obj as exAtlasInfo;
                if ( atlasInfo.needRebuild ) {
                    exAtlasInfoUtility.Build(atlasInfo);
                    rebuildAtlasInfos.Add(atlasInfo);

                    // build sprite animclip that used this atlasInfo
                    exSpriteAnimationUtility.BuildFromAtlasInfo(atlasInfo);
                }
            }

            // ======================================================== 
            // build exSpriteAnimClip 
            // ======================================================== 

            if ( obj is exSpriteAnimClip ) {
                exSpriteAnimClip spAnimClip = obj as exSpriteAnimClip;
                if ( spAnimClip.editorNeedRebuild )
                    exSpriteAnimationUtility.Build(spAnimClip);
            }

            // TODO { 
            // // ======================================================== 
            // // build exBitmapFont 
            // // ======================================================== 

            // if ( obj is exBitmapFont ) {
            //     exBitmapFont bitmapFont = obj as exBitmapFont;
            //     if ( bitmapFont.editorNeedRebuild )
            //         Object fontInfo = exEditorRuntimeHelper.LoadAssetFromGUID(bitmapFont.fontinfoGUID);
            //         exBitmapFontUtility.Build(bitmapFont, fontInfo );
            // }
            // } TODO end 
        }

        // ======================================================== 
        // post build 
        // ======================================================== 

        exAtlasInfoUtility.PostBuild (rebuildAtlasInfos);

        // NOTE: without this you will got leaks message
        EditorUtility.UnloadUnusedAssets();
    }
}
