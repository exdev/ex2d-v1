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
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class exTextureHelper {

    public enum RotateDirection {
        None,
        RotRight, // >>
        Flip,
        RotLeft, // <<
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

#if UNITY_EDITOR
    public static void ImportTextureForAtlas ( Texture2D _tex ) {
        string path = AssetDatabase.GetAssetPath(_tex);
        TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
        if (importer.textureType != TextureImporterType.Advanced ||
            importer.textureFormat != TextureImporterFormat.AutomaticTruecolor ||
            importer.npotScale != TextureImporterNPOTScale.None ||
            importer.isReadable != true)
        {
            importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            importer.textureType = TextureImporterType.Advanced;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
        }
    }
#endif

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Fill ( Texture2D _dest, Vector2 _pos, Texture2D _src, Rect _rect, RotateDirection _rotDir ) {
        int xDest = (int)_pos.x;
        int yDest = (int)_pos.y;
        int xSrc = (int)_rect.x;
        int ySrc = (int)_rect.y;
        int srcWidth = (int)_rect.width;
        int srcHeight = (int)_rect.height;

        if ( _rotDir == RotateDirection.None ) {

            _dest.SetPixels( xDest, yDest, srcWidth, srcHeight, 
                             _src.GetPixels( xSrc, _src.height - ySrc - srcHeight, srcWidth, srcHeight ) );
        }
        else if ( _rotDir == RotateDirection.RotRight ) {
            int destWidth = srcHeight;
            int destHeight = srcWidth;
            for ( int j = 0; j < destHeight; ++j ) {
                for ( int i = 0; i < destWidth; ++i ) {
                    _dest.SetPixel( xDest + i, yDest + j, 
                                    _src.GetPixel( xSrc + srcWidth - j, _src.height - ySrc - srcHeight + i ) );
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static Rect GetTrimTextureRect ( Texture2D _tex ) {
        Rect rect = new Rect( 0, 0, 0, 0 );
        Color[] pixels = _tex.GetPixels(0);

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

