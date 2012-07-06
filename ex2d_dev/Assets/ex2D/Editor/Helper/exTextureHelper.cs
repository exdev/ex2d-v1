// ======================================================================================
// File         : exTextureHelper.cs
// Author       : Wu Jie 
// Last Change  : 06/11/2011 | 23:12:50 PM | Saturday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using System;
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
    /// \param _tex
    /// \param _isReadable
    /// change the Read/Write settings to false
    // ------------------------------------------------------------------ 

    public static void SetReadable ( Texture2D _tex, bool _isReadable ) {
        string path = AssetDatabase.GetAssetPath(_tex);
        TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
        if ( importer.isReadable != _isReadable ) {
            importer.isReadable = _isReadable;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
        }
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

    // ======================================================== 
    // X and Y offsets used in contour bleed for sampling all around each purely transparent pixel
    // ======================================================== 

    private static readonly int[] bleedXOffsets = new []{ -1,  0,  1, -1,  1, -1,  0,  1 };
    private static readonly int[] bleedYOffsets = new []{ -1, -1, -1,  0,  0,  1,  1,  1 };

    // ------------------------------------------------------------------ 
    /// \param _tex the texture in which to apply contour bleed
    /// prevents edge artifacts due to bilinear filtering
    /// Note: Some image editors like Photoshop tend to fill purely transparent pixel with
    /// white color (R=1, G=1, B=1, A=0). This is generally OK, because these white pixels
    /// are impossible to see in normal circumstances.  However, when such textures are
    /// used in 3D with bilinear filtering, the shader will sometimes sample beyond visible
    /// edges into purely transparent pixels and the white color stored there will bleed
    /// into the visible edge.  This method scans the texture to find all purely transparent
    /// pixels that have a visible neighbor pixel, and copy the color data from that neighbor
    /// into the transparent pixel, while preserving its 0 alpha value.  In order to
    /// optimize the algorithm for speed of execution, a compromise is made to use any
    /// arbitrary neighboring pixel, as this should generally lead to correct results.
    /// It also limits itself to the immediate neighbors around the edge, resulting in a
    /// a bleed of a single pixel border around the edges, which should be fine, as bilinear
    /// filtering should generally not sample beyond that one pixel range.
    // ------------------------------------------------------------------ 

    public static Texture2D ApplyContourBleed ( Texture2D _tex ) {
        // Extract pixel buffer to be modified
        Color32[] pixels = _tex.GetPixels32(0);

        for ( int x = 0; x < _tex.width; ++x ) {
            for ( int y = 0; y < _tex.height; ++y ) {
                // only try to bleed into purely transparent pixels
                if ( pixels[x + y * _tex.width].a == 0 ) {
                    // sample all around to find any non-purely transparent pixels
                    for ( int i = 0; i < bleedXOffsets.Length; i++ ) {
                        int sampleX = x + bleedXOffsets[i];
                        int sampleY = y + bleedYOffsets[i];
						// check to stay within texture bounds
                        if (sampleX >= 0 && sampleX < _tex.width && sampleY >= 0 && sampleY < _tex.height) {
                            Color32 color = pixels[sampleX + sampleY * _tex.width];
                            if (color.a != 0) {
                                // Copy RGB color channels to purely transparent pixel, but preserving its 0 alpha
                                pixels[x + y * _tex.width] = new Color32(color.r, color.g, color.b, 0);
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Copy modified pixel buffer to new texture (to preserve original element texture and allow user to uncheck the option)
        Texture2D tex = new Texture2D(_tex.width, _tex.height, _tex.format, false);
        tex.SetPixels32(pixels);
        return tex;
    }

    // ------------------------------------------------------------------ 
    /// \param _tex the texture in which to apply padding bleed
    /// \param _rect the bounds of the element around which to apply bleed
    /// prevents border artifacts due to bilinear filtering
    /// Note: Shaders with bilinear filtering will sometimes sample outside the bounds
    /// of the element, in the padding area, resulting in the padding color to bleed
    /// around the rectangular borders of the element.  This is true even when padding is
    /// purely transparent, because in that case, it is the 0 alpha that bleeds into the
    /// alpha of the outer pixels.  Such alpha bleed is especially problematic when
    /// trying to seamlessly tile multiple rectangular textures, as semi-transparent seams
    /// will sometimes appear at different scales.  This method duplicates a single row of
    /// pixels from the inner border of an element into the padding area.  This technique
    /// can be used with all kinds of textures without risk, even textures with uneven
    /// transparent edges, as it only allows the shader to sample more of the same (opaque
    /// or transparent) values when it exceeds the bounds of the element.
    // ------------------------------------------------------------------ 

    public static void ApplyPaddingBleed( Texture2D _tex, Rect _rect ) {
        
        // NOTE: Possible optimization: If Get/SetPixels32() make a copy of the data (instead
        // of just returning a reference to it, this method call might be very intensive on
        // CPU, as the *entire* atlas would be copied twice for *every* element.  A simple way
        // to optimize that would be to externalize the call to GetPixels32() out of this method
        // and out of the foreach, then call ApplyPaddingBleed() for every element, and finally
        // call SetPixel32() to copy data back into atlas.  It would require two foreach instead
        // of one, but the performance could be greatly improved.  That stands *only* if
        // Get/SetPixels32() make a copy of the data, otherwise there is no performance
        // cost to the current algorithm.  It might be worth investigating that...
        
        // Extract pixel buffer to be modified
        Color32[] pixels = _tex.GetPixels32(0);
        
        // Copy top and bottom rows of pixels
        for (int x = (int)_rect.xMin; x < (int)_rect.xMax; ++x)
        {
            int yMin = (int)_rect.yMin;
            if (yMin - 1 >= 0) // Clamp
                pixels[x + (yMin - 1) * _tex.width] = pixels[x + yMin * _tex.width];

            int yMax = (int)_rect.yMax - 1;
            if (yMax + 1 < _tex.height) // Clamp
                pixels[x + (yMax + 1) * _tex.width] = pixels[x + yMax * _tex.width];
        }

        // Copy left and right columns of pixels (plus 2 extra pixels for corners)
        int startY = Math.Max((int)_rect.yMin - 1, 0); // Clamp
        int endY = Math.Min((int)_rect.yMax + 1, _tex.width); // Clamp
        for (int y = startY; y < endY; ++y)
        {
            int xMin = (int)_rect.xMin;
            if (xMin - 1 >= 0) // Clamp
                pixels[xMin - 1 + y * _tex.width] = pixels[xMin + y * _tex.width];

            int xMax = (int)_rect.xMax - 1;
            if (xMax + 1 < _tex.width) // Clamp
                pixels[xMax + 1 + y * _tex.width] = pixels[xMax + y * _tex.width];
        }

        // Copy modified pixel buffer back to same texture (we are modifying the destination atlas anyway)
        _tex.SetPixels32(pixels);
    }
}


