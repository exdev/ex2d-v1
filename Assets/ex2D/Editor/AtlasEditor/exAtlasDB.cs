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

    [System.Serializable]
    public class ElementInfo {
        public int indexInAtlas;
        public int indexInAtlasInfo;
        public string guidTexture;
        public string guidAtlas;
        public string guidAtlasInfo;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public List<string> atlasInfoGUIDs = new List<string>();
    public Dictionary<string,ElementInfo> 
        texGUIDToElementInfo = new Dictionary<string,ElementInfo>();

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
        db.atlasInfoGUIDs.Clear();
        db.texGUIDToElementInfo.Clear();

        EditorUtility.DisplayProgressBar( "Syncing exAtlasDB...", "Syncing...", 0.5f );    
        SyncDirectory ("Assets");
        EditorUtility.UnloadUnusedAssets();
        EditorUtility.ClearProgressBar();    

        EditorUtility.SetDirty(db);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void SyncDirectory ( string _path ) {
        // Process the list of files found in the directory.
        string [] files = Directory.GetFiles(_path, "*.asset");
        foreach ( string fileName in files ) {
            exAtlasInfo atlasInfo = (exAtlasInfo)AssetDatabase.LoadAssetAtPath( fileName, typeof(exAtlasInfo) );
            if ( atlasInfo ) {
                AddAtlas(atlasInfo);

                atlasInfo = null;
                EditorUtility.UnloadUnusedAssets();
            }
        }

        // Recurse into subdirectories of this directory.
        string [] dirs = Directory.GetDirectories(_path);
        foreach( string dirName in dirs ) {
            SyncDirectory ( dirName);
        }
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
                for ( int i = 0; i < db.atlasInfoGUIDs.Count; ++i ) {
                    exAtlasInfo atlasInfo = exEditorRuntimeHelper.LoadAssetFromGUID<exAtlasInfo>(db.atlasInfoGUIDs[i]);

                    if ( atlasInfo == null ) {
                        db.atlasInfoGUIDs.RemoveAt(i);
                        --i;
                        continue;
                    }

                    foreach ( exAtlasInfo.Element el in atlasInfo.elements ) {
                        AddElementInfo(el);
                    }

                    atlasInfo = null;
                    EditorUtility.UnloadUnusedAssets();
                }

                EditorUtility.SetDirty(db);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public Dictionary<string,ElementInfo> GetTexGUIDToElementInfo () {
        Init();

        return db.texGUIDToElementInfo; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddAtlas ( exAtlasInfo _a ) {
        Init();

        string guid = exEditorRuntimeHelper.AssetToGUID (_a);
        if ( db.atlasInfoGUIDs.Contains(guid) == false ) {
            db.atlasInfoGUIDs.Add(guid);
            foreach ( exAtlasInfo.Element el in _a.elements ) {
                AddElementInfo(el);
            }
            EditorUtility.SetDirty(db);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveAtlas ( exAtlasInfo _a ) {
        Init();

        string guid = exEditorRuntimeHelper.AssetToGUID (_a);
        foreach ( exAtlasInfo.Element el in _a.elements ) {
            RemoveElementInfo(el);
        }
        db.atlasInfoGUIDs.Remove(guid);
        EditorUtility.SetDirty(db);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddElementInfo ( exAtlasInfo.Element _el ) {
        Init();

        if ( _el.isFontElement )
            return;

        string textureGUID = exEditorRuntimeHelper.AssetToGUID(_el.texture);
        AddElementInfo ( textureGUID,
                        exEditorRuntimeHelper.AssetToGUID(_el.atlasInfo.atlas), 
                        exEditorRuntimeHelper.AssetToGUID(_el.atlasInfo),
                        _el.atlasInfo.elements.IndexOf(_el) );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddElementInfo ( string _textureGUID, 
                                        string _atlasGUID, 
                                        string _atlasInfoGUID,
                                        int _atlasInfoIndex ) 
    {
        Init();

        if ( db.texGUIDToElementInfo.ContainsKey(_textureGUID) ) {
            ElementInfo existsElInfo = GetElementInfo(_textureGUID);
            Debug.LogError ( "The texture: " + AssetDatabase.GUIDToAssetPath(existsElInfo.guidTexture) + 
                             "has been added in atlas: " + Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(existsElInfo.guidAtlasInfo)) 
                             + ", The new element in atlas: " + Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(_atlasInfoGUID)) 
                             + " will not be added. please delete the incorrect data." );
            return;
        }

        ElementInfo elInfo = new ElementInfo();
        elInfo.indexInAtlas = _atlasInfoIndex;
        elInfo.indexInAtlasInfo = _atlasInfoIndex;
        elInfo.guidTexture = _textureGUID;
        elInfo.guidAtlas = _atlasGUID; 
        elInfo.guidAtlasInfo = _atlasInfoGUID;
        db.texGUIDToElementInfo[_textureGUID] = elInfo;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void UpdateElementInfoIndex ( string _textureGUID, int _index ) {
        Init();

        ElementInfo elInfo = db.texGUIDToElementInfo[_textureGUID];
        elInfo.indexInAtlas = _index;
        elInfo.indexInAtlasInfo = _index;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveElementInfo ( exAtlasInfo.Element _el ) {
        Init();

        string textureGUID = exEditorRuntimeHelper.AssetToGUID(_el.texture);
        RemoveElementInfo(textureGUID);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveElementInfo ( string _textureGUID ) {
        Init();

        db.texGUIDToElementInfo.Remove(_textureGUID);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public ElementInfo GetElementInfo ( Texture2D _tex ) {
        Init();

        if ( _tex == null )
            return null;

        return GetElementInfo( exEditorRuntimeHelper.AssetToGUID(_tex) );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public ElementInfo GetElementInfo ( string _textureGUID ) {
        Init();

        //
        if ( db.texGUIDToElementInfo.ContainsKey(_textureGUID) ) {
            ElementInfo elInfo = db.texGUIDToElementInfo[_textureGUID]; 

            // NOTE: when atlas been removed, it never notify the exAtlasDB to remove elements  
            exAtlasInfo atlasInfo = exEditorRuntimeHelper.LoadAssetFromGUID<exAtlasInfo>(elInfo.guidAtlasInfo);
            if ( atlasInfo == null ) {
                RemoveElementInfo (_textureGUID);
                return null;
            }
            return elInfo;
        }

        //
        return null;
    }
}

