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
// exSpriteAnimationDB
///////////////////////////////////////////////////////////////////////////////

public class exSpriteAnimationDB : ScriptableObject {

    [System.Serializable]
    public class GUIDInfo {
        public GUIDInfo( string _guidAnimClip, string _guidTexture ) {
            guidAnimClip = _guidAnimClip;
            guidTexture = _guidTexture;
        }
        public string guidAnimClip;
        public string guidTexture;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int curVersion = version;
    public List<string> spAnimClipGUIDs = new List<string>();
    public List<GUIDInfo> guidInfos = new List<GUIDInfo>();
    public Dictionary<string,List<string> > 
        texGuidToAnimClipGUIDs = new Dictionary<string,List<string> >();

    // editor
    public bool showData = true;
    public bool showTable = true;

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
    public static string dbPath = "Assets/.ex2D_SpriteAnimationDB.asset";

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void Sync () {
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

        foreach ( string guidAnimClip in db.spAnimClipGUIDs ) {
            exSpriteAnimClip spAnimClip = exEditorHelper.LoadAssetFromGUID<exSpriteAnimClip>(guidAnimClip);
            spAnimClip.Build ();

            spAnimClip = null;
            EditorUtility.UnloadUnusedAssets();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public bool DBExists () {
        FileInfo fileInfo = new FileInfo(dbPath);
        return fileInfo.Exists;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void SyncRoot () {
        db.spAnimClipGUIDs.Clear();
        db.guidInfos.Clear();
        db.texGuidToAnimClipGUIDs.Clear();

        EditorUtility.DisplayProgressBar( "Syncing exSpriteAnimationDB...", "Syncing...", 0.5f );    
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
            exSpriteAnimClip spAnimClip = (exSpriteAnimClip)AssetDatabase.LoadAssetAtPath( fileName, typeof(exSpriteAnimClip) );
            if ( spAnimClip ) {
                AddSpriteAnimClip(spAnimClip);

                spAnimClip = null;
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
        // get sprite animation clip db, if not found, create one
        if ( DBExists() == false ) {
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
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem("Edit/ex2D/Create Sprite Animation DB")]
    static public void Init () {
        // if db not found we need to create it and re-initliaze
        if ( db == null ) {
            CreateDB ();

            // sync
            if ( needSync ) {
                needSync = false;
                SyncRoot();
            }
            else {
                // DELME { 
                // // create atlas element table
                // for ( int i = 0; i < db.spAnimClipGUIDs.Count; ++i ) {
                //     string animClipGUID = db.spAnimClipGUIDs[i];
                //     exSpriteAnimClip spAnimClip = exEditorHelper.LoadAssetFromGUID<exSpriteAnimClip>(animClipGUID);

                //     if ( spAnimClip == null ) {
                //         db.spAnimClipGUIDs.RemoveAt(i);
                //         --i;
                //         continue;
                //     }

                //     // update sprite anim clip
                //     foreach ( exSpriteAnimClip.FrameInfo fi in spAnimClip.frameInfos ) {
                //         AddFrameInfo ( spAnimClip, fi );
                //     }

                //     spAnimClip = null;
                //     EditorUtility.UnloadUnusedAssets();
                // }
                // } DELME end 

                foreach ( GUIDInfo gi in db.guidInfos ) {
                    AddFrameInfo ( gi.guidAnimClip, gi.guidTexture, false );
                }

                EditorUtility.SetDirty(db);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public Dictionary<string,List<string> > GetTexGUIDToAnimClipGUIDs () {
        Init();

        return db.texGuidToAnimClipGUIDs;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public List<string> GetSpriteAnimClipGUIDs ( string _textureGUID ) {
        Init();

        if ( db.texGuidToAnimClipGUIDs.ContainsKey(_textureGUID) ) 
            return db.texGuidToAnimClipGUIDs[_textureGUID]; 
        return null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public bool HasSpriteAnimClipGUID ( string _guid ) {
        Init();

        return db.spAnimClipGUIDs.IndexOf(_guid) != -1;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddSpriteAnimClip ( exSpriteAnimClip _animClip ) {
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

    // static public void RemoveSpriteAnimClip ( exSpriteAnimClip _animClip ) {
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
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveSpriteAnimClip ( string _guidAnimClip ) {
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
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddFrameInfo ( exSpriteAnimClip _animClip, 
                                      exSpriteAnimClip.FrameInfo _fi ) {
        Init();

        string animClipGUID = exEditorHelper.AssetToGUID(_animClip);
        AddFrameInfo ( animClipGUID, _fi.textureGUID );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddFrameInfo ( string _guidAnimClip, 
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
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveFrameInfo ( exSpriteAnimClip _animClip, 
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
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveGUIDInfo ( string _guidAnimClip, 
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

