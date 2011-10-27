// ======================================================================================
// File         : exAtlasInfoUtility.cs
// Author       : Wu Jie 
// Last Change  : 08/27/2011 | 10:28:02 AM | Saturday,August
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
///
/// the atlas info utility
///
///////////////////////////////////////////////////////////////////////////////

public static partial class exAtlasInfoUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Select Atlas (ex2D)")]
    static void SelectAtlasByActiveTexture () {
        if ( Selection.activeObject is Texture2D ) {
            exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo ( Selection.activeObject as Texture2D );
            if ( elInfo != null ) {
                Selection.activeObject 
                    = exEditorHelper.LoadAssetFromGUID<exAtlasInfo>(elInfo.guidAtlasInfo); 
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _path the directory path to save the atlas
    /// \param _name the name of the atlas
    /// \return the atlas
    /// create the atlas in the _path, save it as _name.
    // ------------------------------------------------------------------ 

    public static exAtlas CreateAtlas ( string _path, string _name ) {
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
        exAtlas newAtlas = ScriptableObject.CreateInstance<exAtlas>();
        AssetDatabase.CreateAsset(newAtlas, assetPath);
        Selection.activeObject = newAtlas;
        return newAtlas;
    }

    // ------------------------------------------------------------------ 
    /// \param _path the directory path to save the atlas info
    /// \param _name the name of the atlas info
    /// \param _width the width of the atlas texture
    /// \param _height the height of the atlas texture
    /// \return the atlas info
    /// create the atlas info in the _path, save it as _name.
    // ------------------------------------------------------------------ 

    public static exAtlasInfo CreateAtlasInfo ( string _path, string _name, int _width, int _height ) {
        // create atlas info
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Creating Atlas Asset...",
                                          0.1f );    
        exAtlasInfo newAtlasInfo = exAtlasInfo.Create( _path, _name + " - EditorInfo" );
        newAtlasInfo.width = _width; 
        newAtlasInfo.height = _height; 

        // create texture
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Creating Atlas Texture...",
                                          0.2f );    
        Texture2D tex = new Texture2D( newAtlasInfo.width, 
                                       newAtlasInfo.height, 
                                       TextureFormat.ARGB32, 
                                       false );
        Color32 buildColor = new Color ( newAtlasInfo.buildColor.r,
                                       newAtlasInfo.buildColor.g,
                                       newAtlasInfo.buildColor.b,
                                       0.0f );
        Color32[] colors = new Color32[newAtlasInfo.width*newAtlasInfo.height];
        for ( int i = 0; i < newAtlasInfo.width * newAtlasInfo.height; ++i )
            colors[i] = buildColor;
        tex.SetPixels32( colors );
        tex.Apply(false);

        // save texture to png
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Saving Atlas Texture as PNG file...",
                                          0.3f );    
        string atlasTexturePath = Path.Combine( _path, _name + ".png" );
        byte[] pngData = tex.EncodeToPNG();
        if (pngData != null)
            File.WriteAllBytes(atlasTexturePath, pngData);
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset( atlasTexturePath );

        // import texture
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Import Texture " + atlasTexturePath + "...",
                                          0.5f );    
        TextureImporter importSettings = TextureImporter.GetAtPath(atlasTexturePath) as TextureImporter;
        importSettings.maxTextureSize = Mathf.Max( newAtlasInfo.width, newAtlasInfo.height );
        importSettings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        importSettings.isReadable = false;
        importSettings.mipmapEnabled = false;
        importSettings.textureType = TextureImporterType.Advanced;
        importSettings.npotScale = TextureImporterNPOTScale.None;
        AssetDatabase.ImportAsset( atlasTexturePath );

        // create default material
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Create New Material...",
                                          0.7f );    
        Material newMaterial = new Material( Shader.Find("ex2D/Alpha Blended") );
        AssetDatabase.CreateAsset( newMaterial, Path.Combine( _path, _name + ".mat" ) );

        // setup atlas info
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Setup New Atlas Asset...",
                                          0.9f );    
        newAtlasInfo.atlasName = _name;
        newAtlasInfo.texture = (Texture2D)AssetDatabase.LoadAssetAtPath( atlasTexturePath, typeof(Texture2D) );
        newAtlasInfo.material = newMaterial;
        newAtlasInfo.material.mainTexture = newAtlasInfo.texture;

        // create new atlas and setup it for both atlas info and atlas asset
        exAtlas newAtlas = CreateAtlas( _path, _name );
        newAtlas.texture = newAtlasInfo.texture;
        newAtlas.material = newAtlasInfo.material;
        newAtlasInfo.atlas = newAtlas;

        //
        EditorUtility.SetDirty(newAtlasInfo);
        EditorUtility.UnloadUnusedAssets();
        EditorUtility.ClearProgressBar();

        //
        Selection.activeObject = newAtlasInfo;
        EditorGUIUtility.PingObject(newAtlasInfo);
        return newAtlasInfo;
    }

    // ------------------------------------------------------------------ 
    /// \param _atlasInfo the atlas info
    /// \param _noImport if true, ex2D will not import the texture to fit for atlas 
    /// build the atlas info to atlas 
    // ------------------------------------------------------------------ 

    public static void Build ( exAtlasInfo _atlasInfo, bool _noImport = false ) {

        exAtlas atlas = _atlasInfo.atlas;
        Texture2D texture = _atlasInfo.texture;
        Material material = _atlasInfo.material;

        // check if the atlas info is valid for build 
        if ( atlas == null ) {
            Debug.LogError("Failed to build atlas info " + _atlasInfo.name + ", the atlas is missing!");
            return;
        }
        if ( texture == null ) {
            Debug.LogError("Failed to build atlas info "  + _atlasInfo.name + ", the texture is missing!");
            return;
        }
        if ( material == null ) {
            Debug.LogError("Failed to build atlas info "  + _atlasInfo.name + ", the material is missing!");
            return;
        }

        //
        if ( _atlasInfo.needLayout ) {
            _atlasInfo.LayoutElements();
            _atlasInfo.needLayout = false;
        }

        // create temp texture
        Color32 buildColor = new Color ( 0.0f, 0.0f, 0.0f, 0.0f );
        if ( _atlasInfo.useBuildColor )
            buildColor = new Color ( _atlasInfo.buildColor.r,
                                     _atlasInfo.buildColor.g,
                                     _atlasInfo.buildColor.b,
                                     0.0f );

        string path = AssetDatabase.GetAssetPath(texture);
        Texture2D tex = new Texture2D(_atlasInfo.width, _atlasInfo.height, TextureFormat.ARGB32, false);
        Color32[] colors = new Color32[_atlasInfo.width*_atlasInfo.height];
        for ( int i = 0; i < _atlasInfo.width * _atlasInfo.height; ++i )
            colors[i] = buildColor;
        tex.SetPixels32( colors );

        EditorUtility.DisplayProgressBar( "Building Atlas " + _atlasInfo.name, "Building Atlas...", 0.1f );    

        // build atlas texture
        _atlasInfo.elements.Sort( exAtlasInfo.CompareByName );
        FillAtlasTexture ( tex, _atlasInfo, _noImport );
        EditorUtility.DisplayProgressBar( "Building Atlas " + _atlasInfo.name,
                                          "Import Atlas",
                                          0.9f );    

        // write to disk
        byte[] pngData = tex.EncodeToPNG();
        if (pngData != null)
            File.WriteAllBytes(path, pngData);
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset( path );

        // now we finish atlas texture filling, we should turn off Read/Write settings, that will save memory a lot!
        TextureImporter importSettings = TextureImporter.GetAtPath(path) as TextureImporter;
        importSettings.isReadable = false;
        AssetDatabase.ImportAsset( path );

        //
        atlas.elements = new exAtlas.Element[_atlasInfo.elements.Count];
        for ( int i = 0; i < _atlasInfo.elements.Count; ++i ) {
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
                exAtlasDB.UpdateElementInfo( el, i );
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
    /// \param _atlasInfo the atlas info
    /// build sprite animation clips from the exAtlasInfo.rebuildAnimClipGUIDs
    // ------------------------------------------------------------------ 

    public static void BuildSpAnimClipsFromRebuildList ( exAtlasInfo _atlasInfo ) {

        EditorUtility.DisplayProgressBar( "Building Sprite Animation Clips...",
                                          "Building Sprite Animation Clips...",
                                          0.5f );    
        for ( int i = 0; i < _atlasInfo.rebuildAnimClipGUIDs.Count; ++i ) {
            string guidAnimClip = _atlasInfo.rebuildAnimClipGUIDs[i];
            exSpriteAnimClip sp = 
                exEditorHelper.LoadAssetFromGUID<exSpriteAnimClip>(guidAnimClip);
            if ( sp ) {
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

    static void FillAtlasTexture ( Texture2D _tex, exAtlasInfo _atlasInfo, bool _noImport ) {
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
                    charInfo.uv0 = new Vector2 ( (float)el.coord[0] / _tex.width,
                                                 (_tex.height - (float)el.coord[1] - charInfo.height) / _tex.height );
                    EditorUtility.SetDirty(el.destFontInfo);
                }
            }

            // make the src texture readable
            if ( exTextureHelper.IsValidForAtlas (srcTexture) == false ) {
                if ( _noImport ) {
                    Debug.LogError( "The texture import settings of [" + AssetDatabase.GetAssetPath(srcTexture) + "] is invalid for atlas build" );
                    continue;
                }
                else {
                    exTextureHelper.ImportTextureForAtlas(srcTexture);
                }
            }

            //
            exTextureHelper.Fill( _tex, 
                                  new Vector2 (el.coord[0], _tex.height - el.coord[1] - el.Height() ),  
                                  srcTexture,
                                  el.trimRect,
                                  el.rotated ? exTextureHelper.RotateDirection.RotRight : exTextureHelper.RotateDirection.None,
                                  _atlasInfo.useBuildColor,
                                  _atlasInfo.buildColor ); 
            // TODO { 
            // Color32[] colors = srcTexture.GetPixels32();
            // Color32[] colors_d = new Color32[_tex.width * _tex.height];
            // for ( int r = 0; r < srcTexture.width; ++r ) {
            //     for ( int c = 0; c < srcTexture.height; ++c ) {
            //         colors_d[r+c*_tex.width] = colors[r+c*srcTexture.width];
            //     }
            // }
            // _tex.SetPixels32( colors_d );
            // } TODO end 
        }

        // ======================================================== 
        // Add water mark
        // ======================================================== 

        // DISABLE: onlly open it in evaluate version { 
        // Make Water Make { 
        // Texture2D texWaterMark = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ex2D/Editor/Resource/water_mark.png", typeof(Texture2D));
        // exTextureHelper.ImportTextureForAtlas(texWaterMark);
        // Color[] colors = texWaterMark.GetPixels();
        // StreamWriter sw = new StreamWriter("Assets/TestFile.txt");
        // Color32[] colors32 = texWaterMark.GetPixels32();
        // for ( int r = 0; r < texWaterMark.height; ++r ) {
        //     for ( int c = 0; c < texWaterMark.width; ++c ) {
        //         Color32 cc = colors32[r*texWaterMark.width+c];
        //         sw.Write( "0x" + cc.r.ToString("X2") 
        //                   + ", 0x" + cc.g.ToString("X2")
        //                   + ", 0x" + cc.b.ToString("X2")
        //                   + ", 0x" + cc.a.ToString("X2")
        //                   + ", " );
        //     }
        //     sw.WriteLine("");
        // }
        // sw.Close();
        // } Make Water Make end 

        // int wa_width = 392;
        // int wa_height = 40;
        // Color[] colors = new Color[wa_width*wa_height];
        // for ( int r = 0; r < wa_height; ++r ) {
        //     for ( int c = 0; c < wa_width; ++c ) {
        //         colors[r*wa_width+c] = new Color ( waterMark[(r*wa_width+c)*4]/255.0f,
        //                                            waterMark[(r*wa_width+c)*4+1]/255.0f, 
        //                                            waterMark[(r*wa_width+c)*4+2]/255.0f, 
        //                                            waterMark[(r*wa_width+c)*4+3]/255.0f );
        //     }
        // }
        // Color[] colors_d = _tex.GetPixels();
        // for ( int r = 0; r < wa_height; ++r ) {
        //     for ( int c = 0; c < wa_width; ++c ) {
        //         Color color_d = colors_d[r*_tex.width+c];
        //         Color color = colors[r*wa_width+c];
        //         // colors_d[r*_tex.width+(c+_tex.height-wa_height)] = 
        //         colors_d[r*_tex.width+c] = color_d * ( 1.0f - color.a ) + color * color.a;
        //     }
        // }
        // _tex.SetPixels( colors_d );
        // } DISABLE end 

        _tex.Apply(false);
    }
}
