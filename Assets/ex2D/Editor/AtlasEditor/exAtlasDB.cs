// ======================================================================================
// File         : exAtlasDB.cs
// Author       : Wu Jie 
// Last Change  : 06/14/2011 | 23:28:22 PM | Tuesday,June
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
// exAtlasDB
///////////////////////////////////////////////////////////////////////////////

public class exAtlasDB : ScriptableObject {

    // [HideInInspector]
    public List<exAtlasInfo> data = new List<exAtlasInfo>();
    public Dictionary<Texture2D,exAtlasInfo.Element> 
        textureToElement = new Dictionary<Texture2D,exAtlasInfo.Element>();

    // editor
    public bool showData = true;
    public bool showTable = true;

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    static bool needSync = false;
    static exAtlasDB db;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void ForceSync () {
        if ( db == null )
            CreateDB ();

        needSync = true;
        SyncRoot();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void SyncRoot () {
        db.data.Clear();
        db.textureToElement.Clear();

        EditorUtility.DisplayProgressBar( "Syncing exAtlasDB...", "Syncing...", 0.5f );    
        SyncDirectory ("Assets");
        EditorUtility.ClearProgressBar();    
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void SyncDirectory ( string _path ) {
        // Process the list of files found in the directory.
        string [] files = Directory.GetFiles(_path, "*.asset");
        foreach ( string fileName in files ) {
            exAtlasInfo atlas = (exAtlasInfo)AssetDatabase.LoadAssetAtPath( fileName, typeof(exAtlasInfo) );
            if ( atlas ) {
                AddAtlas(atlas);
            }
        }

        // Recurse into subdirectories of this directory.
        string [] dirs = Directory.GetDirectories(_path);
        foreach( string dirName in dirs ) {
            SyncDirectory ( dirName);
        }

        //
        EditorUtility.SetDirty(db);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void CreateDB () {
        // get atlas db, if not found, create one
        db = (exAtlasDB)AssetDatabase.LoadAssetAtPath( "Assets/.ex2D_AtlasDB.asset", typeof(exAtlasDB) );
        if ( db == null ) {
            db = ScriptableObject.CreateInstance<exAtlasDB>();
            AssetDatabase.CreateAsset( db, "Assets/.ex2D_AtlasDB.asset" );
            needSync = true;
        } 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void Init () {
        // if db not found we need to create it and re-initliaze
        if ( db == null ) {
            CreateDB ();

            // sync
            if ( needSync ) {
                needSync = false;
                SyncRoot();
            }
            // update atlas elements in db.
            else {
                // create atlas element table
                for ( int i = 0; i < db.data.Count; ++i ) {
                    exAtlasInfo atlas = db.data[i];
                    if ( atlas == null ) {
                        db.data.RemoveAt(i);
                        --i;
                        continue;
                    }

                    foreach ( exAtlasInfo.Element el in atlas.elements ) {
                        UpdateElement(el);
                    }
                }

                EditorUtility.SetDirty(db);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public Dictionary<Texture2D,exAtlasInfo.Element> GetTextureToElementTable () {
        Init();

        return db.textureToElement; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddAtlas ( exAtlasInfo _a ) {
        Init();

        if ( db.data.Contains(_a) == false ) {
            db.data.Add(_a);
            foreach ( exAtlasInfo.Element el in _a.elements ) {
                UpdateElement(el);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveAtlas ( exAtlasInfo _a ) {
        Init();

        foreach ( exAtlasInfo.Element el in _a.elements ) {
            RemoveElement(el);
        }
        db.data.Remove(_a);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void UpdateElement ( exAtlasInfo.Element _el ) {
        Init();

        if ( _el.isFontElement == false && db.textureToElement.ContainsKey(_el.texture) ) {
            exAtlasInfo.Element exists_el = db.textureToElement[_el.texture];
            Debug.LogError ( "The texture: [" + exists_el.texture.name + "]" +
                             "has been added in atlas: " + AssetDatabase.GetAssetPath(exists_el.atlasInfo) 
                             + ", The new element in atlas: " + AssetDatabase.GetAssetPath(exists_el.atlasInfo) 
                             + " will not be added. please delete the incorrect data by yourself." );
            return;
        }
        db.textureToElement[_el.texture] = _el;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveElement ( exAtlasInfo.Element _el ) {
        Init();

        db.textureToElement.Remove(_el.texture);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public exAtlasInfo.Element GetElement ( string _guid ) {
        Init();

        //
        string texturePath = AssetDatabase.GUIDToAssetPath(_guid);
        Texture2D tex2D = (Texture2D)AssetDatabase.LoadAssetAtPath( texturePath, typeof(Texture2D));
        return GetElement(tex2D);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public exAtlasInfo.Element GetElement ( Texture2D _tex ) {
        Init();

        if ( _tex == null )
            return null;

        //
        if ( db.textureToElement.ContainsKey(_tex) ) {
            exAtlasInfo.Element el = db.textureToElement[_tex]; 
            // NOTE: when atlas been removed, it never notify the exAtlasDB to remove elements  
            if ( el.atlasInfo == null ) {
                RemoveElement (el);
                return null;
            }
            return el;
        }

        //
        return null;
    }
}

