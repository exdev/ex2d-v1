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
// exAtlasInfo
///////////////////////////////////////////////////////////////////////////////

public partial class exAtlasInfo : ScriptableObject {

    public enum Algorithm {
        Basic,
        // TODO { 
        // Shelf,
        Tree,
        // MaxRect,
        // } TODO end 
    }

    public enum SortBy {
        UseBest,
        Width,
        Height,
        Area,
        Name
    }

    public enum SortOrder {
        UseBest,
        Accending,
        Descending
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class Element {
        // normal element
        public exAtlasInfo atlasInfo;
        public Texture2D texture; // the texture in atlas
        public int[] coord = new int[] { 0, 0 };
        public Rect trimRect;

        // internal state
        public bool rotated = false;
        public bool trim = false;

        public int Width () {
            if ( rotated )
                return (int)trimRect.height; 
            return (int)trimRect.width; 
        }
        public int Height () {
            if ( rotated )
                return (int)trimRect.width; 
            return (int)trimRect.height; 
        }

        // font element field
        public bool isFontElement = false;
        public exBitmapFont srcFontInfo;
        public exBitmapFont destFontInfo;
        public exBitmapFont.CharInfo charInfo;
    }

    //
    /*[HideInInspector]*/ public string atlasName = "New Atlas"; 
    /*[HideInInspector]*/ public int width = 512;
    /*[HideInInspector]*/ public int height = 512;
    /*[HideInInspector]*/ public List<Element> elements = new List<Element>(); // the atlas texture
    /*[HideInInspector]*/ public exAtlas atlas; // the exAtlas asset
    /*[HideInInspector]*/ public Texture2D texture; // the atlas texture
    /*[HideInInspector]*/ public Material material; // the atlas material

    // canvas settings
    /*[HideInInspector]*/ public bool showCanvas = true;
    /*[HideInInspector]*/ public Color bgColor = Color.white;
    /*[HideInInspector]*/ public bool showCheckerboard = true;

    // layout settings
    /*[HideInInspector]*/ public bool showLayout = true;
    /*[HideInInspector]*/ public Algorithm algorithm = Algorithm.Tree;
    /*[HideInInspector]*/ public SortBy sortBy = SortBy.UseBest;
    /*[HideInInspector]*/ public SortOrder sortOrder = SortOrder.UseBest;
    /*[HideInInspector]*/ public int padding = 2;
    /*[HideInInspector]*/ public bool allowRotate = false;

    // sprite settings
    /*[HideInInspector]*/ public bool showSprites = true;
    /*[HideInInspector]*/ public Color spriteBgColor = new Color( 1.0f, 1.0f, 1.0f, 0.0f );
    /*[HideInInspector]*/ public Color spriteSelectColor = new Color( 0.0f, 0.0f, 1.0f, 1.0f );
    // TODO: it would be nice if we have AtlasBuilder that including a "Edit" button to open exAtlasEditor

    //
    /*[HideInInspector]*/ public float scale = 1.0f;

    // bitmap fonts
    /*[HideInInspector]*/ public List<exBitmapFont> bitmapFonts = new List<exBitmapFont>(); 

    //
    /*[HideInInspector]*/ public List<exSpriteAnimClip> rebuildSpAnimClips = new List<exSpriteAnimClip>();
    /*[HideInInspector]*/ public bool needUpdateAnimClips = false;
    /*[HideInInspector]*/ public bool needRebuild = false;

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
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
    // Desc: 
    // ------------------------------------------------------------------ 

    public Element AddElement ( Texture2D _tex, bool _trim ) {
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

        // update atlas DB
        exAtlasDB.AddElementInfo(el);

        // get sprite animation clip by textureGUID, add them to rebuildSpAnimClips
        AddSpriteAnimClipForRebuilding(el);

        //
        needRebuild = true;
        EditorUtility.SetDirty(this);

        return el;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
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

    public Element AddFontElement ( exBitmapFont _srcFontInfo, exBitmapFont _destFontInfo, exBitmapFont.CharInfo _charInfo ) {
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
    // Desc: 
    // ------------------------------------------------------------------ 

    public void RemoveElement ( Element _el ) {
        int idx = elements.IndexOf(_el);
        if ( idx != -1 ) {
            RemoveElementAt (idx);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void RemoveElementAt ( int _idx ) {
        Element el = elements[_idx];
        elements.RemoveAt(_idx);

        // remove element in atlas DB
        exAtlasDB.RemoveElementInfo(exEditorRuntimeHelper.AssetToGUID(el.texture));

        // get sprite animation clip by textureGUID, add them to rebuildSpAnimClips
        AddSpriteAnimClipForRebuilding(el);

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
    // Desc: 
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
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ClearAtlasTexture () {
        for ( int j = 0; j < texture.height; ++j )
            for ( int i = 0; i < texture.width; ++i )
                texture.SetPixel( i, j, new Color(1.0f, 1.0f, 1.0f, 0.0f) );
        texture.Apply(false);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddSpriteAnimClipForRebuilding ( Element _el ) {
        List<string> spAnimClipGUIDs 
            = exSpriteAnimationDB.GetSpriteAnimClipGUIDs ( exEditorRuntimeHelper.AssetToGUID(_el.texture) );
        if ( spAnimClipGUIDs != null ) {
            foreach ( string animClipGUID in spAnimClipGUIDs ) {
                exSpriteAnimClip animClip = exEditorRuntimeHelper.LoadAssetFromGUID<exSpriteAnimClip>(animClipGUID);
                if ( animClip != null && rebuildSpAnimClips.IndexOf(animClip) == -1 ) {
                    animClip.editorNeedRebuild = true;
                    rebuildSpAnimClips.Add(animClip);
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
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
                    f2.useAtlas = true;
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


