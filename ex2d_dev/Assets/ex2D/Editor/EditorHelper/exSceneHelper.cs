// ======================================================================================
// File         : exSceneHelper.cs
// Author       : Wu Jie 
// Last Change  : 08/31/2011 | 17:54:44 PM | Wednesday,August
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

public static class exSceneHelper {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Edit/ex2D/Update Scene Sprite Layers %&l")]
    static void UpdateSceneSpriteLayers () {
        EditorUtility.DisplayProgressBar( "Update Scene Sprite Layers...", 
                                          "Update Scene Sprite Layers...", 
                                          0.5f );    

        exLayer2D[] layerObjs = GameObject.FindObjectsOfType(typeof(exLayer2D)) as exLayer2D[];
        for ( int i = 0; i < layerObjs.Length; ++i ) {
            exLayer2D layerObj = layerObjs[i]; 
            // DISABLE: it is too slow { 
            // float progress = (float)i/(float)layerObjs.Length;
            // EditorUtility.DisplayProgressBar( "Update Scene Sprite Layers...", 
            //                                   "Update Sprite Layer " + layerObj.gameObject.name, progress );    
            // } DISABLE end 

            layerObj.UpdateDepth();
            RecursivelyUpdateLayer(layerObj.transform);
        }
        EditorUtility.ClearProgressBar();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Edit/ex2D/Rebuild Scene Sprites %&b")]
    static void RebuildSceneSprites () {
        exSpriteBase[] sprites = GameObject.FindObjectsOfType(typeof(exSpriteBase)) as exSpriteBase[];
        RebuildSprites (sprites);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void RebuildSprites ( exSpriteBase[] _sprites ) {
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
            if ( spBase is exSpriteFont ) {
                exSpriteFont spFont = spBase as exSpriteFont;
                spFont.Build();
            }

            // update layer
            exLayer2D layer2d = spBase.GetComponent<exLayer2D>();
            if ( layer2d ) {
                layer2d.UpdateDepth();
            }
        }
        EditorUtility.UnloadUnusedAssets(); // NOTE: without this you will got leaks message
        EditorUtility.ClearProgressBar();    
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RecursivelyUpdateLayer ( Transform _trans ) {
        foreach ( Transform child in _trans ) {
            exLayer2D layer = child.GetComponent<exLayer2D>();
            if ( layer ) {
                layer.UpdateDepth();
                RecursivelyUpdateLayer ( layer.transform );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void UpdateSceneSprites ( List<string> _atlasInfoGUIDs ) {
        if ( _atlasInfoGUIDs.Count == 0 )
            return;

        EditorUtility.DisplayProgressBar( "Update Scene Sprites...", "Scanning...", 0.0f );    
        // exSpriteBase[] sprites = GameObject.FindObjectsOfType(typeof(exSpriteBase)) as exSpriteBase[];
        exSpriteBase[] sprites = Resources.FindObjectsOfTypeAll(typeof(exSpriteBase)) as exSpriteBase[];
        for ( int i = 0; i < sprites.Length; ++i ) {
            exSpriteBase spBase = sprites[i]; 

            // ======================================================== 
            // exSprite
            // ======================================================== 

            if ( spBase is exSprite ) {
                exSprite sp = spBase as exSprite;
                exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo(sp.textureGUID);
                bool needRebuild = false;

                // NOTE: we test sp.index is -1 or not instead of test atlas is null, because it is possible we delete an atlas and it will always be null
                if ( elInfo != null ) {
                    if ( sp.index == -1 ) {
                        needRebuild = true;
                    }
                    else {
                        // find if the sp's atalsInfo need rebuild
                        foreach ( string guidAtlasInfo in _atlasInfoGUIDs ) {
                            if ( elInfo.guidAtlasInfo == guidAtlasInfo ) {
                                needRebuild = true;
                                break;
                            }
                        }
                    }

                }
                else {
                    if ( sp.index != -1 ) {
                        needRebuild = true;
                    }
                }

                //
                if ( needRebuild ) {
                    exSpriteEditor.UpdateAtlas( sp, elInfo );
                    bool isPrefab = (EditorUtility.GetPrefabType(spBase) == PrefabType.Prefab); 
                    if ( isPrefab == false ) {
                        Texture2D texture = null;
                        if ( sp.useAtlas == false ) {
                            texture = exEditorHelper.LoadAssetFromGUID<Texture2D>(sp.textureGUID );
                        }
                        sp.Build(texture);
                    }
                    EditorUtility.SetDirty(sp);
                }
            }

            // ======================================================== 
            // exSpriteFont
            // ======================================================== 

            if ( spBase is exSpriteFont ) {
                exSpriteFont spFont = spBase as exSpriteFont;

                //
                bool needRebuild = false;
                if ( spFont.fontInfo == null ) {
                    needRebuild = true;
                }
                else {
                    foreach ( string guidAtlasInfo in _atlasInfoGUIDs ) {
                        exAtlasInfo atlasInfo = exEditorHelper.LoadAssetFromGUID<exAtlasInfo>(guidAtlasInfo);
                        // NOTE: it is possible we process this in delete stage
                        if ( atlasInfo == null )
                            continue;

                        foreach ( exBitmapFont bmfont in atlasInfo.bitmapFonts ) {
                            if ( spFont.fontInfo == bmfont ) {
                                needRebuild = true;
                                break;
                            }
                        }
                    }
                }

                //
                if ( needRebuild ) {
                    spFont.Build();
                }
            }

            // DISABLE: it is too slow { 
            // float progress = (float)i/(float)sprites.Length;
            // EditorUtility.DisplayProgressBar( "Update Scene Sprites...", 
            //                                   "Update Sprite " + spBase.gameObject.name, progress );    
            // } DISABLE end 
        }
        EditorUtility.ClearProgressBar();    
    }
}
