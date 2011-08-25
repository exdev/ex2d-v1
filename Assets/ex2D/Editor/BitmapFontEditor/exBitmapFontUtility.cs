// ======================================================================================
// File         : exBitmapFontUtility.cs
// Author       : Wu Jie 
// Last Change  : 07/15/2011 | 15:35:49 PM | Friday,July
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
// exBitmapFontUtility
///////////////////////////////////////////////////////////////////////////////

public class exBitmapFontUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void Build ( exBitmapFont _bitmapFont, Object _fontInfo ) {

        EditorUtility.DisplayProgressBar( "Building BitmapFont...",
                                          "Build BitmapFont " + _bitmapFont.name, 
                                          0.0f );    
        // 
        _bitmapFont.pageInfos.Clear();
        _bitmapFont.charInfos.Clear();
        _bitmapFont.kernings.Clear();
        // TODO { 
        // _bitmapFont.fontInfoGUIDs.Clear();
        // } TODO end 
        ParseFontInfo ( _bitmapFont, _fontInfo ); 
        EditorUtility.ClearProgressBar();    

        //
        _bitmapFont.editorNeedRebuild = false;
        EditorUtility.SetDirty(_bitmapFont);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void ParseFontInfo ( exBitmapFont _bitmapFont, Object _fontInfo ) {

        EditorUtility.DisplayProgressBar( "Building BitmapFont...",
                                          "Parsing font info...",
                                          0.1f );    

        string fontInfoPath = AssetDatabase.GetAssetPath(_fontInfo);
        string dirname = Path.GetDirectoryName(fontInfoPath);

        // TODO { 
        // _bitmapFont.fontInfoGUIDs.Add(exEditorHelper.GetPathGUID(_fontInfo));
        // } TODO end 

        // DELME { 
        // string[] lines = _textAsset.text.Split ('\n');
        // foreach ( string line in lines ) {
        // } DELME end 
		string line;
        FileInfo fileInfo = new FileInfo(fontInfoPath);
		StreamReader reader = fileInfo.OpenText();
		while ( (line = reader.ReadLine()) != null ) {

            // DISABLE: it is too slow { 
            // EditorUtility.DisplayProgressBar( "Building BitmapFont...",
            //                                   "Parsing line " + i,
            //                                   (float)i/(float)lines.Length );    
            // } DISABLE end 
            string[] words = line.Split(' ');
            if ( words[0] == "info" ) {
                _bitmapFont.size = int.Parse ( ParseValue( words, "size" ) ); 
            }
            else if ( words[0] == "common" ) {
                _bitmapFont.lineHeight = int.Parse ( ParseValue( words, "lineHeight" ) ); 
                // _bitmapFont.width = int.Parse ( ParseValue( words, "scaleW" ) ); 
                // _bitmapFont.height = int.Parse ( ParseValue( words, "scaleH" ) ); 
				int pages = int.Parse( ParseValue( words, "pages" ) );
                _bitmapFont.pageInfos = new List<exBitmapFont.PageInfo>(pages);
                for ( int i = 0; i < pages; ++i ) {
                    _bitmapFont.pageInfos.Add(new exBitmapFont.PageInfo());
                }
                // DISABLE { 
                // if ( pages != 1 ) {
                //     Debug.LogError ( "Parse Error: only support one page" );
                //     return;
                // }
                // } DISABLE end 
            }
            else if ( words[0] == "page" ) {
                // check if id is valid
                int id = int.Parse ( ParseValue( words, "id" ) ); 
                if ( id >= _bitmapFont.pageInfos.Count ) {
                    Debug.LogError("Parse Failed: The page id is exceed the page number");
                    return;
                }

                // load texture from file
                string filename = ParseValue( words, "file" );
                filename = filename.Substring( 1, filename.Length-2 ); // remove the "" in "foobar.png"
                string texturePath = Path.Combine( dirname, filename );
                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath( texturePath, typeof(Texture2D) );
                if ( texture == null ) {
                    Debug.LogError("Parse Failed: The texture " + filename + " not found.");
                    return;
                }

                // load material, if not exists, create a new one.
                string filenameNoExt = Path.GetFileNameWithoutExtension(texturePath);
                string materialPath = Path.Combine( dirname, filenameNoExt ) + ".mat";
                Material material = (Material)AssetDatabase.LoadAssetAtPath( materialPath, typeof(Material) );
                if ( material == null ) {
                    material = new Material( Shader.Find("ex2D/Alpha Blended") );
                    material.mainTexture = texture;
                    AssetDatabase.CreateAsset( material, materialPath );
                }

                // add page info 
                _bitmapFont.pageInfos[id].texture = texture;
                _bitmapFont.pageInfos[id].material = material;
            }
            else if ( words[0] == "char" ) {
                exBitmapFont.CharInfo charInfo = new exBitmapFont.CharInfo(); 
                charInfo.id = int.Parse ( ParseValue( words, "id" ) );
                charInfo.x = int.Parse ( ParseValue( words, "x" ) );
                charInfo.y = int.Parse ( ParseValue( words, "y" ) );
                charInfo.width = int.Parse ( ParseValue( words, "width" ) );
                charInfo.height = int.Parse ( ParseValue( words, "height" ) );
                charInfo.xoffset = int.Parse ( ParseValue( words, "xoffset" ) );
                charInfo.yoffset = int.Parse ( ParseValue( words, "yoffset" ) );
                charInfo.xadvance = int.Parse ( ParseValue( words, "xadvance" ) );
                charInfo.page = int.Parse ( ParseValue( words, "page" ) );

                exBitmapFont.PageInfo pageInfo = _bitmapFont.pageInfos[charInfo.page];
                charInfo.uv0 = new Vector2 ( (float)charInfo.x / pageInfo.texture.width,
                                             (pageInfo.texture.height - (float)charInfo.y - charInfo.height) / pageInfo.texture.height );

                _bitmapFont.charInfos.Add(charInfo);
            }
            else if ( words[0] == "kerning" ) {
                exBitmapFont.KerningInfo kerningInfo = new exBitmapFont.KerningInfo();
                kerningInfo.first = int.Parse ( ParseValue( words, "first" ) );
                kerningInfo.second = int.Parse ( ParseValue( words, "second" ) );
                kerningInfo.amount = int.Parse ( ParseValue( words, "amount" ) );
                _bitmapFont.kernings.Add(kerningInfo);
            }
        }
        _bitmapFont.RebuildIdToCharInfoTable();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static string ParseValue ( string[] _words, string _key ) {
        string mykey = _key + "="; 
        foreach ( string word in _words ) {
            if ( word.Length > mykey.Length &&
                 word.Substring(0,mykey.Length) == mykey )
            {
                return word.Substring(mykey.Length);
            }
        }
        return "";
    }
}

