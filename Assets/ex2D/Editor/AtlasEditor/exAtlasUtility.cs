// ======================================================================================
// File         : exAtlasUtility.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 22:14:46 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// exAtlasUtility
///////////////////////////////////////////////////////////////////////////////

public class exAtlasUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void Build ( exAtlasInfo _atlasInfo ) {

        exAtlas atlas = _atlasInfo.atlas;
        Texture2D texture = _atlasInfo.texture;
        Material material = _atlasInfo.material;

        // check if the atlas info is valid for build 
        if ( atlas == null ) {
            Debug.LogError("Failed to build atlas info, the atlas is missing!");
            return;
        }
        if ( texture == null ) {
            Debug.LogError("Failed to build atlas info, the texture is missing!");
            return;
        }
        if ( material == null ) {
            Debug.LogError("Failed to build atlas info, the material is missing!");
            return;
        }


        // create temp texture
        string path = AssetDatabase.GetAssetPath(texture);
        Texture2D tex = new Texture2D(_atlasInfo.width, _atlasInfo.height, TextureFormat.ARGB32, false);
        for ( int x = 0; x < _atlasInfo.width; ++x ) {
            for ( int y = 0; y < _atlasInfo.height; ++y ) {
                tex.SetPixel(x, y, new Color(0.0f, 0.0f, 0.0f, 0.0f) );
            }
        }

        EditorUtility.DisplayProgressBar( "Building Atlas " + _atlasInfo.name, "Building Atlas...", 0.1f );    

        // build atlas texture
        int i = 0;
        _atlasInfo.elements.Sort( exAtlasInfo.CompareByName );
        foreach ( exAtlasInfo.Element el in _atlasInfo.elements ) {
            // DISABLE: it is too slow { 
            // EditorUtility.DisplayProgressBar( "Building Atlas...",
            //                                   "Building Texture " + el.texture.name,
            //                                   (float)i/(float)_atlasInfo.elements.Count - 0.1f );    
            // } DISABLE end 

            Texture2D srcTexture = el.texture;
            if ( el.isFontElement ) {
                // build the font
                exBitmapFont.CharInfo charInfo = el.destFontInfo.GetCharInfo(el.charInfo.id);
                if ( charInfo != null ) {
                    charInfo.uv0 = new Vector2 ( (float)el.coord[0] / tex.width,
                                                 (tex.height - (float)el.coord[1] - charInfo.height) / tex.height );
                    EditorUtility.SetDirty(el.destFontInfo);
                }
            }

            // make the src texture readable
            exTextureHelper.ImportTextureForAtlas(srcTexture);

            //
            exTextureHelper.Fill( tex, 
                                new Vector2 (el.coord[0], tex.height - el.coord[1] - el.Height() ),  
                                srcTexture,
                                el.trimRect,
                                el.rotated ? exTextureHelper.RotateDirection.RotRight : exTextureHelper.RotateDirection.None ); 
            ++i;
        }
        tex.Apply(false);

        EditorUtility.DisplayProgressBar( "Building Atlas " + _atlasInfo.name,
                                          "Import Atlas",
                                          0.9f );    

        // write to disk
        byte[] pngData = tex.EncodeToPNG();
        if (pngData != null)
            File.WriteAllBytes(path, pngData);
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset( path );

        //
        atlas.elements = new exAtlas.Element[_atlasInfo.elements.Count];
        for ( i = 0; i < _atlasInfo.elements.Count; ++i ) {
            exAtlasInfo.Element el = _atlasInfo.elements[i];
            exAtlas.Element el2 = new exAtlas.Element ();

            int coord_x = el.coord[0];
            int coord_y = el.atlasInfo.height - el.coord[1] - (int)el.Height();
            float xStart  = (float)coord_x / (float)el.atlasInfo.width;
            float yStart  = (float)coord_y / (float)el.atlasInfo.height;
            float xEnd    = (float)(coord_x + el.Width()) / (float)el.atlasInfo.width;
            float yEnd    = (float)(coord_y + el.Height()) / (float)el.atlasInfo.height;
            el2.name = el.texture.name; 
            el2.coords = new Rect ( xStart, yStart, xEnd - xStart, yEnd - yStart );
            el2.rotated = el.rotated;
            el2.originalWidth = el.texture.width; 
            el2.originalHeight = el.texture.height; 
            el2.trimRect = el.trimRect;
            atlas.elements[i] = el2;

            // update the index in exAtlasDB
            if ( el.isFontElement == false ) {
                exAtlasDB.UpdateElementInfoIndex( exEditorRuntimeHelper.AssetToGUID(el.texture), i );
            }
        }
        atlas.texture = texture; 
        atlas.material = material; 
        EditorUtility.SetDirty(atlas);
        EditorUtility.ClearProgressBar();

        // save the needRebuild setting
        _atlasInfo.needRebuild = false;
        EditorUtility.SetDirty(_atlasInfo);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void PostBuild ( List<exAtlasInfo> _atlasInfos ) {
        if ( _atlasInfos.Count == 0 )
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

                if ( elInfo == null ) {
                    if ( sp.atlas != null ) {
                        needRebuild = true;
                    }
                }
                else {
                    // find if the sp's atalsInfo need rebuild
                    foreach ( exAtlasInfo atlasInfo in _atlasInfos ) {
                        if ( elInfo.guidAtlasInfo == exEditorRuntimeHelper.AssetToGUID(atlasInfo) ) {
                            needRebuild = true;
                            break;
                        }
                    }
                }

                if ( needRebuild ) {
                    exSpriteEditor.UpdateAtlas( sp, elInfo );
                    bool isPrefab = (EditorUtility.GetPrefabType(spBase) == PrefabType.Prefab); 
                    if ( isPrefab == false ) {
                        Texture2D texture = exEditorRuntimeHelper.LoadAssetFromGUID<Texture2D>(sp.textureGUID );
                        sp.Build( texture );
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
                    foreach ( exAtlasInfo atlasInfo in _atlasInfos ) {
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
