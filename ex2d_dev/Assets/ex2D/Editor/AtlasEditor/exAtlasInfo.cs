// ======================================================================================
// File         : exAtlasInfo.cs
// Author       : Wu Jie 
// Last Change  : 07/03/2011 | 23:01:50 PM | Sunday,July
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
///
/// The atlas editor information asset
///
///////////////////////////////////////////////////////////////////////////////

public partial class exAtlasInfo : ScriptableObject {

    // ------------------------------------------------------------------ 
    /// the algorithm type of texture packer
    // ------------------------------------------------------------------ 

    public enum Algorithm {
        Basic, ///< basic algorithm, pack texture by the sort order
        // Shelf, // TODO
        Tree,  ///< Pack the textures by binary space, find the most fitable texture in it
        // MaxRect, // TODO
    }

    // ------------------------------------------------------------------ 
    /// sorting type for sort elements
    // ------------------------------------------------------------------ 

    public enum SortBy {
        UseBest, ///< use the best sorting result depends on exAtlasInfo.algorithm
        Width,   ///< sort by texture width
        Height,  ///< sort by texture height
        Area,    ///< sort by texture area 
        Name     ///< sort by texture name
    }

    // ------------------------------------------------------------------ 
    /// sorting the elements in Accending or Descending order
    // ------------------------------------------------------------------ 

    public enum SortOrder {
        UseBest,   ///< use the best order depends on the exAtlasInfo.algorithm
        Accending, ///< use accending order 
        Descending ///< use descending order
    }

    // ------------------------------------------------------------------ 
    /// the structure to store the edit information of each element in atlas 
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class Element {

        // ======================================================== 
        // internal state
        // ======================================================== 

        public bool rotated = false; ///< if rotate the texture in atlas 
        public bool trim = false;    ///< if trimmed the texture

        // ======================================================== 
        // normal element
        // ======================================================== 

        public exAtlasInfo atlasInfo; ///< the referernced atlas info 
        public Texture2D texture;     ///< the raw texture
        public int[] coord = new int[] { 0, 0 }; ///< the coordination of the element in atlas. start from top-left (0,0) to bottom-right (1,1)
        public Rect trimRect; ///< the trimed rect of the raw texture

        // ======================================================== 
        // font element field
        // ======================================================== 

        public bool isFontElement = false; ///< if is a font element import from exBitmapFont 
        public exBitmapFont srcFontInfo; ///< the source exBitmapFont
        public exBitmapFont destFontInfo; ///< the target exBitmapFont
        public exBitmapFont.CharInfo charInfo; ///< the character information in the atlas

        // ======================================================== 
        // functions
        // ======================================================== 

        // ------------------------------------------------------------------ 
        /// \return the result width
        /// the width calculated depends on the Element.rotated and Element.trim
        // ------------------------------------------------------------------ 

        public int Width () {
            if ( rotated )
                return (int)trimRect.height; 
            return (int)trimRect.width; 
        }

        // ------------------------------------------------------------------ 
        /// \return the result height
        /// the height calculated depends on the Element.rotated and Element.trim
        // ------------------------------------------------------------------ 

        public int Height () {
            if ( rotated )
                return (int)trimRect.width; 
            return (int)trimRect.height; 
        }
    }

    //
    public string atlasName = "New Atlas"; ///< the name of the atlas we expect to create 
    public int width = 512; ///< the width of the atlas texture 
    public int height = 512; ///< the height of the atlas texture
    public List<Element> elements = new List<Element>(); ///< the list of atlas info elements
    public exAtlas atlas; ///< the referenced atlas asset
    public Texture2D texture; ///< the referenced atlas texture
    public Material material; ///< the default material we used
    public Color buildColor = new Color(1.0f, 1.0f, 1.0f, 0.0f); ///< the color of transparent pixels in atlas texture

    // canvas settings
    public bool foldCanvas = true; ///< canvas fold option
    public Color bgColor = Color.white; ///< the canvas background color
    public bool showCheckerboard = true; ///< if show the checkerboard

    // layout settings
    public bool foldLayout = true; ///< layout fold option
    public Algorithm algorithm = Algorithm.Tree; ///< the algorithm used for texture packer
    public SortBy sortBy = SortBy.UseBest; ///< the method to sort the elements in atlas editor info
    public SortOrder sortOrder = SortOrder.UseBest; ///< the order to sort the elements in atlas editor info
    public int padding = 2; ///< the padding size between each element
    public bool allowRotate = false; ///< if allow texture rotated, disabled in current version 

    // element settings
    public bool foldElement = true; ///< element fold option
    public Color elementBgColor = new Color( 1.0f, 1.0f, 1.0f, 0.0f ); ///< the background color of each element
    public Color elementSelectColor = new Color( 0.0f, 0.0f, 1.0f, 1.0f ); ///< the select rect color of each element

    //
    public float scale = 1.0f; ///< the zoom value of the atlas

    // bitmap fonts
    public List<exBitmapFont> bitmapFonts = new List<exBitmapFont>(); ///< the list of bitmap fonts in the atlas

    //
    public List<string> rebuildAnimClipGUIDs = new List<string>(); ///< the sprite animation clip guid list for rebuilding
    public bool needUpdateAnimClips = false; ///< if need update anim clips
    public bool needRebuild = false; ///< if need rebuild the atlas

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \param _path the path of the directory you expect to save the atlas info
    /// \param _name the name of the atlas without extension you expect to save
    /// \return the new atlas info
    /// Create and save the atlas, atlas textures, atlas material and atlas info in the expect path then return it. 
    // ------------------------------------------------------------------ 

    public static exAtlasInfo Create ( string _path, string _name ) {
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
        exAtlasInfo newAtlas = ScriptableObject.CreateInstance<exAtlasInfo>();
        AssetDatabase.CreateAsset(newAtlas, assetPath);
        Selection.activeObject = newAtlas;
        return newAtlas;
    }

    // a > b = 1, a < b = -1, a = b = 0
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static int CompareByWidth ( Element _a, Element _b ) {
        return (int)_a.trimRect.width - (int)_b.trimRect.width;
    }
    public static int CompareByHeight ( Element _a, Element _b ) {
        return (int)_a.trimRect.height - (int)_b.trimRect.height;
    }
    public static int CompareByArea ( Element _a, Element _b ) {
        return (int)_a.trimRect.width * (int)_a.trimRect.height - 
            (int)_b.trimRect.width * (int)_b.trimRect.height;
    }
    public static int CompareByName ( Element _a, Element _b ) {
        return string.Compare( _a.texture.name, _b.texture.name );
    }

    public static int CompareByWidthRotate ( Element _a, Element _b ) {
        int a_size = (int)_a.trimRect.height;
        if ( (int)_a.trimRect.height > (int)_a.trimRect.width ) {
            a_size = (int)_a.trimRect.height;
            _a.rotated = true;
        }
        int b_size = (int)_b.trimRect.height;
        if ( (int)_b.trimRect.height > (int)_b.trimRect.width ) {
            b_size = (int)_b.trimRect.height;
            _b.rotated = true;
        }
        return a_size - b_size;
    }
    public static int CompareByHeightRotate ( Element _a, Element _b ) {
        int a_size = (int)_a.trimRect.height;
        if ( (int)_a.trimRect.width > (int)_a.trimRect.height ) {
            a_size = (int)_a.trimRect.width;
            _a.rotated = true;
        }
        int b_size = (int)_b.trimRect.height;
        if ( (int)_b.trimRect.width > (int)_b.trimRect.height ) {
            b_size = (int)_b.trimRect.width;
            _b.rotated = true;
        }
        return a_size - b_size;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \param _tex the raw texture you want to add
    /// \param _trim if trim the texture
    /// \return the new element
    /// add the element by raw texture 
    // ------------------------------------------------------------------ 

    public Element AddElement ( Texture2D _tex, bool _trim ) {
        if ( exTextureHelper.IsValidForAtlas (_tex) == false )
            exTextureHelper.ImportTextureForAtlas(_tex);

        //
        exAtlasInfo.Element el = new exAtlasInfo.Element();
        if ( _trim ) {
            el.trimRect = exTextureHelper.GetTrimTextureRect(_tex);
        }
        else {
            el.trimRect = new Rect( 0, 0, _tex.width, _tex.height );
        }

        el.rotated = false;
        el.trim = _trim;
        el.atlasInfo = this;
        el.texture = _tex;
        el.coord[0] = padding;
        el.coord[1] = padding;
        elements.Add(el);

        // get sprite animation clip by textureGUID, add them to rebuildAnimClipGUIDs
        AddSpriteAnimClipForRebuilding(el);

        //
        needRebuild = true;
        EditorUtility.SetDirty(this);

        return el;
    }

    // ------------------------------------------------------------------ 
    /// \param _fontInfo the font info you want to remove
    /// Find and remove the font info from the atlas
    // ------------------------------------------------------------------ 

    public void RemoveBitmapFont ( exBitmapFont _fontInfo ) {
        for ( int i = 0; i < elements.Count; ++i ) {
            exAtlasInfo.Element el = elements[i];
            if ( el.isFontElement == false )
                continue;

            if ( el.destFontInfo == _fontInfo ) {
                RemoveElement (el);
                --i;
            }
        }
        bitmapFonts.Remove(_fontInfo);
        EditorUtility.SetDirty(this);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected Element AddFontElement ( exBitmapFont _srcFontInfo, exBitmapFont _destFontInfo, exBitmapFont.CharInfo _charInfo ) {
        exAtlasInfo.Element el = new exAtlasInfo.Element();
        el.isFontElement = true;

        el.srcFontInfo = _srcFontInfo;
        el.destFontInfo = _destFontInfo;
        el.charInfo = _charInfo;

        el.trimRect = new Rect( _charInfo.x, _charInfo.y, _charInfo.width, _charInfo.height );
        el.rotated = false;
        el.trim = true;
        el.atlasInfo = this;
        el.texture = _srcFontInfo.pageInfos[0].texture;
        el.coord[0] = padding;
        el.coord[1] = padding;

        exBitmapFont.CharInfo destCharInfo = el.destFontInfo.GetCharInfo(el.charInfo.id);
        if ( destCharInfo != null ) {
            destCharInfo.id = el.charInfo.id;
            destCharInfo.x = el.charInfo.x;
            destCharInfo.y = el.charInfo.y;
            destCharInfo.width = el.charInfo.width;
            destCharInfo.height = el.charInfo.height;
            destCharInfo.xoffset = el.charInfo.xoffset;
            destCharInfo.yoffset = el.charInfo.yoffset;
            destCharInfo.xadvance = el.charInfo.xadvance;
            destCharInfo.page = el.charInfo.page;
            destCharInfo.uv0 = el.charInfo.uv0;
        }
        else {
            Debug.LogError ( "can't not find char info with ID " + el.charInfo.id );
        }

        elements.Add(el);

        needRebuild = true;
        EditorUtility.SetDirty(this);

        return el;
    }

    // ------------------------------------------------------------------ 
    /// \param _el the element you expect to remove
    /// remove an element from the atlas info
    // ------------------------------------------------------------------ 

    public void RemoveElement ( Element _el ) {
        int idx = elements.IndexOf(_el);
        if ( idx != -1 ) {
            RemoveElementAt (idx);
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _idx the index of the element 
    /// remove an element from the atlas info by index
    // ------------------------------------------------------------------ 

    public void RemoveElementAt ( int _idx ) {
        Element el = elements[_idx];

        // remove element in atlas DB
        exAtlasDB.RemoveElementInfo(exEditorHelper.AssetToGUID(el.texture));

        // get sprite animation clip by textureGUID, add them to rebuildAnimClipGUIDs
        AddSpriteAnimClipForRebuilding(el);

        //
        elements.RemoveAt(_idx);

        //
        needRebuild = true;
        EditorUtility.SetDirty(this);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ResetElements () {
        foreach ( Element el in elements ) {
            el.rotated = false;
        }
        needRebuild = true;
        EditorUtility.SetDirty(this);
    }

    // ------------------------------------------------------------------ 
    /// Sort the elemtns in atlas info by the exAtlasInfo.SortBy and exAtlasInfo.SortOrder 
    // ------------------------------------------------------------------ 

    public void SortElements () {
        //
        SortBy mySortBy = sortBy;
        SortOrder mySortOrder = sortOrder;
        if ( sortBy == SortBy.UseBest ) {
            switch ( algorithm ) {
            case Algorithm.Basic:
                mySortBy = SortBy.Height;
                break;
            case Algorithm.Tree:
                mySortBy = SortBy.Height;
                break;
            default:
                mySortBy = SortBy.Height;
                break;
            }
        }
        if ( sortOrder == SortOrder.UseBest ) {
            mySortOrder = SortOrder.Descending;
        }

        // sort by
        switch ( mySortBy ) {
        case SortBy.Width:
            if ( allowRotate )
                elements.Sort( CompareByWidthRotate );
            else
                elements.Sort( CompareByWidth );
            break;
        case SortBy.Height:
            if ( allowRotate )
                elements.Sort( CompareByHeightRotate );
            else
                elements.Sort( CompareByHeight );
            break;
        case SortBy.Area:
            elements.Sort( CompareByArea );
            break;
        case SortBy.Name:
            elements.Sort( CompareByName );
            break;
        }

        // sort order
        if ( mySortOrder == SortOrder.Descending ) {
            elements.Reverse();
        }
        needRebuild = true;
        EditorUtility.SetDirty(this);
    }

    // ------------------------------------------------------------------ 
    /// Clear the all pixels in atlas texture, and fill with white color
    // ------------------------------------------------------------------ 

    public void ClearAtlasTexture () {
        for ( int j = 0; j < texture.height; ++j )
            for ( int i = 0; i < texture.width; ++i )
                texture.SetPixel( i, j, new Color(1.0f, 1.0f, 1.0f, 0.0f) );
        texture.Apply(false);
    }

    // ------------------------------------------------------------------ 
    /// \param _el
    /// Add the sprite animation clip for rebuild by checking if clip contains the in element's exAtlasInfo.Element.texture
    // ------------------------------------------------------------------ 

    public void AddSpriteAnimClipForRebuilding ( Element _el ) {
        List<string> spAnimClipGUIDs 
            = exSpriteAnimationDB.GetSpriteAnimClipGUIDs ( exEditorHelper.AssetToGUID(_el.texture) );

        if ( spAnimClipGUIDs != null ) {
            foreach ( string animClipGUID in spAnimClipGUIDs ) {
                if ( rebuildAnimClipGUIDs.IndexOf(animClipGUID) == -1 ) {
                    rebuildAnimClipGUIDs.Add(animClipGUID);
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _objects 
    /// get the Texture2D and exBitmapFont from a list of objects, import them into atlas
    // ------------------------------------------------------------------ 

    public void ImportObjects ( Object[] _objects ) {
        bool dirty = false;
        foreach ( Object o in _objects ) {
            if ( o is Texture2D ) {
                Texture2D t = o as Texture2D;
                exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo(t);
                if ( elInfo == null ) {
                    AddElement( t, true );
                    dirty = true;
                }
                else {
                    Debug.LogError( "The texture [" + t.name + "]" + 
                                    " has already been added in atlas: " +
                                    AssetDatabase.GUIDToAssetPath(elInfo.guidAtlasInfo) );
                }
            }
            else if ( o is exBitmapFont ) {
                exBitmapFont f = o as exBitmapFont;
                if ( f.inAtlas ) {
                    // NOTE: it is still possible we have atlas font in the obj list since we use Selection.GetFiltered().
                    continue;
                }

                // multi-page atlas font is forbit
                if ( f.pageInfos.Count > 1 ) {
                    Debug.LogError("Can't not create atlas font from " + f.name + ", it has multiple page info.");
                    continue;
                }

                // check if we have resource in the project
                string assetPath = AssetDatabase.GetAssetPath(texture);
                string dirname = Path.GetDirectoryName(assetPath);
                string filename = Path.GetFileNameWithoutExtension(assetPath);
                string bitmapFontPath = Path.Combine( dirname, filename + " - " + f.name + ".asset" );
                exBitmapFont f2 = (exBitmapFont)AssetDatabase.LoadAssetAtPath( bitmapFontPath,
                                                                               typeof(exBitmapFont) );
                if ( f2 == null ) {
                    f2 = (exBitmapFont)ScriptableObject.CreateInstance(typeof(exBitmapFont));
                    f2.inAtlas = true;
                    f2.name = f.name;
                    f2.lineHeight = f.lineHeight;

                    // add page info
                    exBitmapFont.PageInfo pageInfo = new exBitmapFont.PageInfo();
                    pageInfo.texture = texture;
                    pageInfo.material = material;
                    f2.pageInfos.Add(pageInfo);

                    // add char info
                    foreach ( exBitmapFont.CharInfo c in f.charInfos ) {
                        f2.charInfos.Add(c);
                    }

                    // add kerning info
                    foreach ( exBitmapFont.KerningInfo k in f.kernings ) {
                        f2.kernings.Add(k);
                    }

                    AssetDatabase.CreateAsset ( f2, bitmapFontPath );

                    //
                    foreach ( exBitmapFont.CharInfo c in f2.charInfos ) {
                        if ( c.id == -1 )
                            continue;
                        AddFontElement( f, f2, c );
                    }
                }
                else {
                    Debug.LogError("You already add the BitmapFont in this Atlas");
                }

                //
                if ( bitmapFonts.IndexOf(f2) == -1 ) {
                    bitmapFonts.Add(f2);
                }

                dirty = true;
            }
            if ( dirty ) {
                EditorUtility.SetDirty(this);
            }
        }
    }
}


