// ======================================================================================
// File         : exTextureHelper.cs
// Author       : Wu Jie 
// Last Change  : 06/11/2011 | 23:12:50 PM | Saturday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
/// 
/// the texture helper class
/// 
///////////////////////////////////////////////////////////////////////////////

public static class exTextureHelper {

    // ------------------------------------------------------------------ 
    /// the direction of rotation
    // ------------------------------------------------------------------ 

    public enum RotateDirection {
        None, ///< none
        RotRight, ///< rotate 90 degrees to right (clock-wise) >>
        Flip, ///< rotate 180 degrees
        RotLeft, ///< rotate 90 degrees to left (clock-wise) <<
    }

    // ------------------------------------------------------------------ 
    /// \param _tex
    /// \return is valid or not
    /// check if the texture settings is valid for atlas build
    // ------------------------------------------------------------------ 

    public static bool IsValidForAtlas ( Texture2D _tex ) {
        string path = AssetDatabase.GetAssetPath(_tex);
        TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
        if ( importer.textureType != TextureImporterType.Advanced ||
             importer.textureFormat != TextureImporterFormat.AutomaticTruecolor ||
             importer.npotScale != TextureImporterNPOTScale.None ||
             importer.isReadable != true ||
             importer.mipmapEnabled != false )
        {
            return false;
        }
        return true;
    }

    // ------------------------------------------------------------------ 
    /// \param _tex
    /// change the import texture settings to make it fit for atlas 
    // ------------------------------------------------------------------ 

    public static void ImportTextureForAtlas ( Texture2D _tex ) {
        string path = AssetDatabase.GetAssetPath(_tex);
        TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;

        importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        importer.textureType = TextureImporterType.Advanced;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.isReadable = true;
        importer.mipmapEnabled = false;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
    }

    // ------------------------------------------------------------------ 
    /// \param _dest the target texture
    /// \param _pos the fill start position in target texture
    /// \param _src the src texture
    /// \param _rect the rect to fill
    /// \param _rotDir rotation direction
    /// \param _useBgColor use the background color
    /// \param _bgColor the background color, null if no use
    /// fill the source texture to target texture
    // ------------------------------------------------------------------ 

    public static void Fill ( Texture2D _dest, 
                              Vector2 _pos, 
                              Texture2D _src, 
                              Rect _rect, 
                              RotateDirection _rotDir,
                              bool _useBgColor,
                              Color _bgColor ) {
        int xDest = (int)_pos.x;
        int yDest = (int)_pos.y;
        int xSrc = (int)_rect.x;
        int ySrc = (int)_rect.y;
        int srcWidth = (int)_rect.width;
        int srcHeight = (int)_rect.height;

        if ( _rotDir == RotateDirection.None ) {
            if ( _useBgColor ) {
                for ( int j = 0; j < srcHeight; ++j ) {
                    for ( int i = 0; i < srcWidth; ++i ) {
                        Color c = _src.GetPixel( xSrc + i, _src.height - ySrc - srcHeight + j );
                        if ( c.a == 0.0f ) {
                            c = _bgColor;
                            c.a = 0.0f;
                        }
                        _dest.SetPixel( xDest + i, yDest + j, c );
                    }
                }
            }
            else {
                _dest.SetPixels( xDest, yDest, srcWidth, srcHeight, 
                                 _src.GetPixels( xSrc, _src.height - ySrc - srcHeight, srcWidth, srcHeight ) );
            }
        }
        else if ( _rotDir == RotateDirection.RotRight ) {
            int destWidth = srcHeight;
            int destHeight = srcWidth;

            if ( _useBgColor ) {
                for ( int j = 0; j < destHeight; ++j ) {
                    for ( int i = 0; i < destWidth; ++i ) {
                        Color c = _src.GetPixel( xSrc + srcWidth - j, _src.height - ySrc - srcHeight + i );
                        if ( c.a == 0.0f ) {
                            c = _bgColor;
                            c.a = 0.0f;
                        }
                        _dest.SetPixel( xDest + i, yDest + j, c );
                    }
                }
            }
            else {
                for ( int j = 0; j < destHeight; ++j ) {
                    for ( int i = 0; i < destWidth; ++i ) {
                        Color c = _src.GetPixel( xSrc + srcWidth - j, _src.height - ySrc - srcHeight + i );
                        _dest.SetPixel( xDest + i, yDest + j, c ); 
                    }
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _tex the texture to trim
    /// \return the trimmed rect
    /// get the trimmed texture rect 
    // ------------------------------------------------------------------ 

    public static Rect GetTrimTextureRect ( Texture2D _tex ) {
        Rect rect = new Rect( 0, 0, 0, 0 );
        Color32[] pixels = _tex.GetPixels32(0);

        for ( int x = 0; x < _tex.width; ++x ) {
            for ( int y = 0; y < _tex.height; ++y ) {
                if ( pixels[x+y*_tex.width].a != 0 ) {
                    rect.x = x;
                    x = _tex.width;
                    break;
                }
            }
        }

        for ( int x = _tex.width-1; x >= 0; --x ) {
            for ( int y = 0; y < _tex.height; ++y ) {
                if ( pixels[x+y*_tex.width].a != 0 ) {
                    rect.xMax = x+1;
                    x = 0;
                    break;
                }
            }
        }

        for ( int y = 0; y < _tex.height; ++y ) {
            for ( int x = 0; x < _tex.width; ++x ) {
                if ( pixels[x+y*_tex.width].a != 0 ) {
                    rect.y = y;
                    y = _tex.height;
                    break;
                }
            }
        }

        for ( int y = _tex.height-1; y >= 0; --y ) {
            for ( int x = 0; x < _tex.width; ++x ) {
                if ( pixels[x+y*_tex.width].a != 0 ) {
                    rect.yMax = y+1;
                    y = 0;
                    break;
                }
            }
        }

        rect.y = _tex.height - rect.yMax;
        return rect;
    }
}

