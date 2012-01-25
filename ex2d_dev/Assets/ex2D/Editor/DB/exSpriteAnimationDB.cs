// ======================================================================================
// File         : exSpriteAnimationDB.cs
// Author       : Wu Jie 
// Last Change  : 06/15/2011 | 01:58:00 AM | Wednesday,June
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
/// sprite animation clip database
///
///////////////////////////////////////////////////////////////////////////////

public class exSpriteAnimationDB : ScriptableObject {

    // ------------------------------------------------------------------ 
    /// the guid information
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class GUIDInfo {
        /// \param _guidAnimClip the sprite animation clip guid
        /// \param _guidTexture the raw texture guid
        /// constructor
        public GUIDInfo( string _guidAnimClip, string _guidTexture ) {
            guidAnimClip = _guidAnimClip;
            guidTexture = _guidTexture;
        }
        public string guidAnimClip; ///< the sprite animation clip guid
        public string guidTexture; ///< the raw texture guid
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int curVersion = version; ///< the current version of the sprite animation db
    public List<string> spAnimClipGUIDs = new List<string>(); ///< the guid list of sprite animation clip
    public List<GUIDInfo> guidInfos = new List<GUIDInfo>(); ///< the list of guid information 
    public Dictionary<string,List<string> > 
        texGuidToAnimClipGUIDs = new Dictionary<string,List<string> >(); ///< the texture guid to sprite animation clip guid list table

    // editor
    public bool showData = true; ///< fold data option in Inspector
    public bool showTable = true; ///< fold table option in Inspector

    // FIXME: conflict with CreateDB I doubt. when I delete DB and create it, crash with this { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void OnEnable () {
    //     Init();
    // }
    // } FIXME end 

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    static int version = 1;
    static bool needSync = false;
    static exSpriteAnimationDB db;
    public static string dbPath = "Assets/_ex2D_SpriteAnimationDB.asset"; ///< the fullpath of the sprite animation clip db we use

    // ------------------------------------------------------------------ 
    /// force sync the sprite animation db
    // ------------------------------------------------------------------ 

    public static void ForceSync () {
        if ( db == null )
            CreateDB ();

        SyncRoot();
    }

    // ------------------------------------------------------------------ 
    /// rebuild all sprite animations in the project
    // ------------------------------------------------------------------ 

    public static void BuildAll () {
        if ( db == null )
            CreateDB ();

        foreach ( string guidAnimClip in db.spAnimClipGUIDs ) {
            exSpriteAnimClip spAnimClip = exEditorHelper.LoadAssetFromGUID<exSpriteAnimClip>(guidAnimClip);
            spAnimClip.Build ();

            spAnimClip = null;
            EditorUtility.UnloadUnusedAssetsIgnoreManagedReferences();
        }
    }

    // ------------------------------------------------------------------ 
    /// check if the file in exSpriteAnimationDB.dbPath exists 
    // ------------------------------------------------------------------ 

    public static bool DBExists () {
        FileInfo fileInfo = new FileInfo(dbPath);
        return fileInfo.Exists;
    }

    // ------------------------------------------------------------------ 
    /// sync the sprite animation db file from the Assets directory
    // ------------------------------------------------------------------ 

    static void SyncRoot () {
        db.spAnimClipGUIDs.Clear();
        db.guidInfos.Clear();
        db.texGuidToAnimClipGUIDs.Clear();

        try {
            EditorUtility.DisplayProgressBar( "Syncing exSpriteAnimationDB...", "Syncing...", 0.5f );    
            SyncDirectory ("Assets");
            EditorUtility.UnloadUnusedAssetsIgnoreManagedReferences();
            EditorUtility.ClearProgressBar();    
        }
        catch ( System.Exception ) {
            EditorUtility.ClearProgressBar();    
            throw;
        }

        EditorUtility.SetDirty(db);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void SyncDirectory ( string _path ) {
        // Process the list of files found in the directory.
        string [] files = Directory.GetFiles(_path, "*.asset");
        foreach ( string fileName in files ) {
            exSpriteAnimClip spAnimClip = (exSpriteAnimClip)AssetDatabase.LoadAssetAtPath( fileName, typeof(exSpriteAnimClip) );
            if ( spAnimClip ) {
                AddSpriteAnimClip(spAnimClip);

                spAnimClip = null;
                EditorUtility.UnloadUnusedAssetsIgnoreManagedReferences();
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
        // first create db directory if not exists
        string dbDir = Path.GetDirectoryName(dbPath);
        if ( new DirectoryInfo(dbDir).Exists == false ) {
            Directory.CreateDirectory (dbDir);
        }

        // get sprite animation clip db, if not found, create one
        db = (exSpriteAnimationDB)AssetDatabase.LoadAssetAtPath( dbPath, typeof(exSpriteAnimationDB) );
        if ( db == null ) {
            db = ScriptableObject.CreateInstance<exSpriteAnimationDB>();
            AssetDatabase.CreateAsset( db, dbPath );
            needSync = true;
        }
        else {
            db = (exSpriteAnimationDB)AssetDatabase.LoadAssetAtPath( dbPath, typeof(exSpriteAnimationDB) );
        }

        //
        if ( version != db.curVersion ) {
            db.curVersion = version;
            needSync = true;
            EditorUtility.SetDirty(db);
        }
    }

    // ------------------------------------------------------------------ 
    /// init the sprite animation db file, if it doesn't exists, create it.
    // ------------------------------------------------------------------ 

    [MenuItem("Edit/ex2D/Create Sprite Animation DB")]
    public static void Init () {
        // if db not found we need to create it and re-initliaze
        if ( db == null ) {
            CreateDB ();

            // sync
            if ( needSync ) {
                needSync = false;
                SyncRoot();
            }
            // update guid info in db.
            else {
                db.texGuidToAnimClipGUIDs.Clear();

                foreach ( GUIDInfo gi in db.guidInfos ) {
                    AddFrameInfo ( gi.guidAnimClip, gi.guidTexture, false );
                }

                EditorUtility.SetDirty(db);
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// get texture guid to sprite animation guid list
    // ------------------------------------------------------------------ 

    public static Dictionary<string,List<string> > GetTexGUIDToAnimClipGUIDs () {
        Init();

        return db.texGuidToAnimClipGUIDs;
    }

    // ------------------------------------------------------------------ 
    /// \param _textureGUID the texture guid key
    /// \return the list of sprite animation guid
    /// sprite animations contains the texture by its guid, return the guid of animation
    // ------------------------------------------------------------------ 

    public static List<string> GetSpriteAnimClipGUIDs ( string _textureGUID ) {
        Init();

        if ( db.texGuidToAnimClipGUIDs.ContainsKey(_textureGUID) ) 
            return db.texGuidToAnimClipGUIDs[_textureGUID]; 
        return null;
    }

    // ------------------------------------------------------------------ 
    /// \param _guid the sprite animation clip guid
    /// \return the result
    /// check if the db have the _guid in its exSpriteAnimationDB.spAnimClipGUIDs list
    // ------------------------------------------------------------------ 

    public static bool HasSpriteAnimClipGUID ( string _guid ) {
        Init();

        return db.spAnimClipGUIDs.IndexOf(_guid) != -1;
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the sprite animation clip
    /// add the sprite animation clip to the sprite animation db
    // ------------------------------------------------------------------ 

    public static void AddSpriteAnimClip ( exSpriteAnimClip _animClip ) {
        Init();

        string guidAnimClip = exEditorHelper.AssetToGUID (_animClip);
        if ( db.spAnimClipGUIDs.Contains(guidAnimClip) == false ) {
            db.spAnimClipGUIDs.Add(guidAnimClip);

            // update sprite anim clip
            foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
                AddFrameInfo ( _animClip, fi );
            }
            EditorUtility.SetDirty(db);
        }
    }

    // DISABLE: no use { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // public static void RemoveSpriteAnimClip ( exSpriteAnimClip _animClip ) {
    //     Init();

    //     string animClipGUID = exEditorHelper.AssetToGUID(_animClip);
    //     foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
    //         if ( db.texGuidToAnimClipGUIDs.ContainsKey(fi.textureGUID) ) {
    //             List<string> animClipGUIDs = db.texGuidToAnimClipGUIDs[fi.textureGUID];
    //             animClipGUIDs.Remove (animClipGUID);
    //         }
    //     }

    //     db.spAnimClipGUIDs.Remove(animClipGUID);
    //     EditorUtility.SetDirty(db);
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    /// \param _guidAnimClip the sprite animation clip guid
    /// remove the sprite animation clip in the db by its guid
    // ------------------------------------------------------------------ 

    public static void RemoveSpriteAnimClip ( string _guidAnimClip ) {
        Init();

        // get ElementInfo that have the same atlasInfo guid to remove list 
        foreach ( KeyValuePair<string,List<string> > pair in db.texGuidToAnimClipGUIDs ) {
            List<string> animClipGUIDs = pair.Value;

            for ( int i = 0; i < animClipGUIDs.Count; ++i ) {
                if ( animClipGUIDs[i] == _guidAnimClip ) {
                    // find and remove GUIDInfo
                    RemoveGUIDInfo ( _guidAnimClip, pair.Key );

                    // remove animClipGUID from animClipGUIDs
                    animClipGUIDs.RemoveAt(i);
                    --i;
                    continue;
                }
            }
        }

        //
        db.spAnimClipGUIDs.Remove(_guidAnimClip);
        EditorUtility.SetDirty(db);
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the animation clip
    /// \param _fi the frame info in the animation clip
    /// add frame info by animation clip and frame info to sprite animation clip db
    // ------------------------------------------------------------------ 

    public static void AddFrameInfo ( exSpriteAnimClip _animClip, 
                                      exSpriteAnimClip.FrameInfo _fi ) {
        Init();

        string animClipGUID = exEditorHelper.AssetToGUID(_animClip);
        AddFrameInfo ( animClipGUID, _fi.textureGUID );
    }

    // ------------------------------------------------------------------ 
    /// \param _guidAnimClip the sprite animation clip guid
    /// \param _guidTexture the texture guid
    /// \param _addGUIDInfo if add guid info
    /// add frame info by animation clip and frame info to sprite animation clip db
    // ------------------------------------------------------------------ 

    public static void AddFrameInfo ( string _guidAnimClip, 
                                      string _guidTexture,
                                      bool _addGUIDInfo = true ) {

        Init();

        // get animClip guid list by textureGUID in FrameInfo, if not found, create an empty list
        List<string> animClipGUIDs;
        if ( db.texGuidToAnimClipGUIDs.ContainsKey(_guidTexture) == false ) {
            animClipGUIDs = new List<string>();
            db.texGuidToAnimClipGUIDs[_guidTexture] = animClipGUIDs;
        }
        else {
            animClipGUIDs = db.texGuidToAnimClipGUIDs[_guidTexture];
        }

        // find if the guid of the in animClip is already in the animClip guid list
        int idx = animClipGUIDs.IndexOf(_guidAnimClip);
        if ( idx == -1 ) {
            animClipGUIDs.Add (_guidAnimClip);

            // NOTE: we will not add guid info when syncing
            if ( _addGUIDInfo ) {
                // add new GUIDInfo
                db.guidInfos.Add( new GUIDInfo(_guidAnimClip,_guidTexture) );
                EditorUtility.SetDirty(db);
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _animClip the animation clip
    /// \param _fi the frame info in the _animClip
    /// remove frame info from sprite animation cip db
    // ------------------------------------------------------------------ 

    public static void RemoveFrameInfo ( exSpriteAnimClip _animClip, 
                                         exSpriteAnimClip.FrameInfo _fi ) {
        Init();

        // first, we need to check if the textureGUID of this frame is used in another frame of the same clip
        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            if ( fi.textureGUID == _fi.textureGUID )
                return;
        }

        // if we are the only textureGUID used in this clip, remove the clip
        if ( db.texGuidToAnimClipGUIDs.ContainsKey(_fi.textureGUID) ) {
            List<string> animClips = db.texGuidToAnimClipGUIDs[_fi.textureGUID];
            string guidAnimClip = exEditorHelper.AssetToGUID(_animClip);
            animClips.Remove (guidAnimClip);

            // find and remove GUIDInfo
            RemoveGUIDInfo ( guidAnimClip, _fi.textureGUID );
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _guidAnimClip the animation clip guid
    /// \param _guidTexture the texture guid
    /// remove guid info from sprite animation db
    // ------------------------------------------------------------------ 

    public static void RemoveGUIDInfo ( string _guidAnimClip, 
                                        string _guidTexture ) {
        Init ();


        // find and remove GUIDInfo
        for ( int i = 0; i < db.guidInfos.Count; ++i ) {
            GUIDInfo gi = db.guidInfos[i];
            if ( gi.guidAnimClip == _guidAnimClip &&
                 gi.guidTexture == _guidTexture ) {
                db.guidInfos.RemoveAt(i);
                EditorUtility.SetDirty(db);
                break;
            }
        }
    }
}

