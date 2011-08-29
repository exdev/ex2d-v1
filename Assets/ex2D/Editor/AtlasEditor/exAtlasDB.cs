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

    public int curVersion = version;
    public List<string> atlasInfoGUIDs = new List<string>();
    public List<ElementInfo> elementInfos = new List<ElementInfo>();
    public Dictionary<string,ElementInfo> 
        texGUIDToElementInfo = new Dictionary<string,ElementInfo>();

    // editor
    public bool showData = true;
    public bool showTable = true;

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    static int version = 2;
    static bool needSync = false;
    static exAtlasDB db;
    public static string dbPath = "Assets/.ex2D_AtlasDB.asset"; 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void ForceSync () {
        if ( db == null )
            CreateDB ();

        SyncRoot();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void BuildAll () {
        if ( db == null )
            CreateDB ();

        foreach ( string guidAtlasInfo in db.atlasInfoGUIDs ) {
            exAtlasInfo atlasInfo = exEditorRuntimeHelper.LoadAssetFromGUID<exAtlasInfo>(guidAtlasInfo);
            exAtlasInfoUtility.Build ( atlasInfo );

            atlasInfo = null;
            EditorUtility.UnloadUnusedAssets();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void SyncRoot () {
        db.atlasInfoGUIDs.Clear();
        db.elementInfos.Clear();
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
        db = (exAtlasDB)AssetDatabase.LoadAssetAtPath( dbPath, typeof(exAtlasDB) );
        if ( db == null ) {
            db = ScriptableObject.CreateInstance<exAtlasDB>();
            AssetDatabase.CreateAsset( db, dbPath );
            needSync = true;
        }
        if ( version != db.curVersion ) {
            db.curVersion = version;
            needSync = true;
            EditorUtility.SetDirty(db);
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
                foreach ( ElementInfo elInfo in db.elementInfos ) {
                    AddElementInfo( elInfo.guidTexture, 
                                    elInfo.guidAtlas,
                                    elInfo.guidAtlasInfo,
                                    elInfo.indexInAtlas );
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

    static public bool HasAtlasGUID ( string _guid ) {
        Init();

        return db.atlasInfoGUIDs.IndexOf(_guid) != -1;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddAtlas ( exAtlasInfo _a ) {
        Init();

        string guid = exEditorRuntimeHelper.AssetToGUID (_a);
        if ( db.atlasInfoGUIDs.Contains(guid) == false ) {
            db.atlasInfoGUIDs.Add(guid);
            for ( int i = 0; i < _a.elements.Count; ++i ) {
                ElementInfo elInfo = AddElementInfo( _a.elements[i], i);
                if ( elInfo != null )
                    db.elementInfos.Add(elInfo);
            }
            EditorUtility.SetDirty(db);
        }
    }

    // DISABLE: no use { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // static public void RemoveAtlas ( exAtlasInfo _a ) {
    //     Init();

    //     string guid = exEditorRuntimeHelper.AssetToGUID (_a);
    //     foreach ( exAtlasInfo.Element el in _a.elements ) {
    //         RemoveElementInfo(exEditorRuntimeHelper.AssetToGUID(el.texture));
    //     }
    //     db.atlasInfoGUIDs.Remove(guid);
    //     EditorUtility.SetDirty(db);
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveAtlas ( string _atlasInfoGUID ) {
        Init();

        // get ElementInfo that have the same atlasInfo guid to remove list 
        List<string> removedItems = new List<string>();
        foreach ( KeyValuePair<string,exAtlasDB.ElementInfo> pair in db.texGUIDToElementInfo ) {
            if ( pair.Value.guidAtlasInfo == _atlasInfoGUID ) {
                removedItems.Add(pair.Key);
            }
        }

        // remove these elements
        foreach ( string textureGUID in removedItems ) {
            RemoveElementInfo(textureGUID);
        }

        // remove atlas info
        db.atlasInfoGUIDs.Remove(_atlasInfoGUID);
        EditorUtility.SetDirty(db);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static ElementInfo AddElementInfo ( exAtlasInfo.Element _el, int _index ) {
        Init();

        if ( _el.isFontElement )
            return null;

        string textureGUID = exEditorRuntimeHelper.AssetToGUID(_el.texture);
        return AddElementInfo ( textureGUID,
                                exEditorRuntimeHelper.AssetToGUID(_el.atlasInfo.atlas), 
                                exEditorRuntimeHelper.AssetToGUID(_el.atlasInfo),
                                _index );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static ElementInfo AddElementInfo ( string _textureGUID, 
                                        string _atlasGUID, 
                                        string _atlasInfoGUID,
                                        int _index ) 
    {
        Init();

        if ( db.texGUIDToElementInfo.ContainsKey(_textureGUID) ) {
            ElementInfo existsElInfo = GetElementInfo(_textureGUID);
            Debug.LogError ( "The texture: " + AssetDatabase.GUIDToAssetPath(existsElInfo.guidTexture) + 
                             " has been added in atlas: " + Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(existsElInfo.guidAtlasInfo)) 
                             + ", The new element in atlas: " + Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(_atlasInfoGUID)) 
                             + " will not be added. please delete the incorrect data." );
            return null;
        }

        ElementInfo elInfo = new ElementInfo();
        elInfo.indexInAtlas = _index;
        elInfo.indexInAtlasInfo = _index;
        elInfo.guidTexture = _textureGUID;
        elInfo.guidAtlas = _atlasGUID; 
        elInfo.guidAtlasInfo = _atlasInfoGUID;
        db.texGUIDToElementInfo[_textureGUID] = elInfo;
        return elInfo;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void UpdateElementInfo ( exAtlasInfo.Element _el, int _index ) {
        Init();

        ElementInfo elInfo = null;
        string textureGUID = exEditorRuntimeHelper.AssetToGUID(_el.texture);
        if ( db.texGUIDToElementInfo.ContainsKey(textureGUID) == false ) {
            elInfo = AddElementInfo (_el, _index);
            if ( elInfo != null )
                db.elementInfos.Add(elInfo);
        }
        else {
            elInfo = db.texGUIDToElementInfo[textureGUID];
            elInfo.indexInAtlas = _index;
            elInfo.indexInAtlasInfo = _index;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveElementInfo ( string _textureGUID ) {
        Init();

        if ( db.texGUIDToElementInfo.ContainsKey(_textureGUID) ) {
            db.elementInfos.Remove(db.texGUIDToElementInfo[_textureGUID]);
            db.texGUIDToElementInfo.Remove(_textureGUID);
        }
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

            // DELME: we don't need this anymore { 
            // // NOTE: when atlas been removed, it never notify the exAtlasDB to remove elements  
            // exAtlasInfo atlasInfo = exEditorRuntimeHelper.LoadAssetFromGUID<exAtlasInfo>(elInfo.guidAtlasInfo);
            // if ( atlasInfo == null ) {
            //     RemoveElementInfo (_textureGUID);
            //     return null;
            // }
            // } DELME end 

            return elInfo;
        }

        //
        return null;
    }
}

