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

    public int curVersion = version;
    public List<string> spAnimClipGUIDs = new List<string>();
    public Dictionary<string,List<string> > 
        texGuidToAnimClipGUIDs = new Dictionary<string,List<string> >();

    // editor
    public bool showData = true;
    public bool showTable = true;

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
            exSpriteAnimClip spAnimClip = exEditorRuntimeHelper.LoadAssetFromGUID<exSpriteAnimClip>(guidAnimClip);
            exSpriteAnimationUtility.Build ( spAnimClip );

            spAnimClip = null;
            EditorUtility.UnloadUnusedAssets();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void SyncRoot () {
        db.spAnimClipGUIDs.Clear();
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
        db = (exSpriteAnimationDB)AssetDatabase.LoadAssetAtPath( dbPath, typeof(exSpriteAnimationDB) );
        if ( db == null ) {
            db = ScriptableObject.CreateInstance<exSpriteAnimationDB>();
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
            else {
                // TODO: learn from AtlasDB { 
                // create atlas element table
                for ( int i = 0; i < db.spAnimClipGUIDs.Count; ++i ) {
                    string animClipGUID = db.spAnimClipGUIDs[i];
                    exSpriteAnimClip spAnimClip = exEditorRuntimeHelper.LoadAssetFromGUID<exSpriteAnimClip>(animClipGUID);

                    if ( spAnimClip == null ) {
                        db.spAnimClipGUIDs.RemoveAt(i);
                        --i;
                        continue;
                    }

                    // update sprite anim clip
                    foreach ( exSpriteAnimClip.FrameInfo fi in spAnimClip.frameInfos ) {
                        UpdateDataBase ( spAnimClip, fi );
                    }

                    spAnimClip = null;
                    EditorUtility.UnloadUnusedAssets();
                }
                // } TODO end 

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

        string guidAnimClip = exEditorRuntimeHelper.AssetToGUID (_animClip);
        if ( db.spAnimClipGUIDs.Contains(guidAnimClip) == false ) {
            db.spAnimClipGUIDs.Add(guidAnimClip);

            // update sprite anim clip
            foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
                UpdateDataBase ( _animClip, fi );
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

    //     string animClipGUID = exEditorRuntimeHelper.AssetToGUID(_animClip);
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

    static public void RemoveSpriteAnimClip ( string _guid ) {
        Init();

        // get ElementInfo that have the same atlasInfo guid to remove list 
        foreach ( KeyValuePair<string,List<string> > pair in db.texGuidToAnimClipGUIDs ) {
            List<string> animClipGUIDs = pair.Value;

            for ( int i = 0; i < animClipGUIDs.Count; ++i ) {
                if ( animClipGUIDs[i] == _guid ) {
                    animClipGUIDs.RemoveAt(i);
                    --i;
                    continue;
                }
            }
        }

        //
        db.spAnimClipGUIDs.Remove(_guid);
        EditorUtility.SetDirty(db);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void UpdateDataBase ( exSpriteAnimClip _animClip, exSpriteAnimClip.FrameInfo _fi ) 
    {
        Init();

        string animClipGUID = exEditorRuntimeHelper.AssetToGUID(_animClip);

        List<string> animClipGUIDs;
        if ( db.texGuidToAnimClipGUIDs.ContainsKey(_fi.textureGUID) == false ) {
            animClipGUIDs = new List<string>();
            db.texGuidToAnimClipGUIDs[_fi.textureGUID] = animClipGUIDs;
        }
        else {
            animClipGUIDs = db.texGuidToAnimClipGUIDs[_fi.textureGUID];
        }

        int idx = animClipGUIDs.IndexOf(animClipGUID);
        if ( idx == -1 ) {
            animClipGUIDs.Add (animClipGUID);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void OnFrameInfoRemoved ( exSpriteAnimClip _animClip, exSpriteAnimClip.FrameInfo _fi ) 
    {
        Init();

        if ( db.texGuidToAnimClipGUIDs.ContainsKey(_fi.textureGUID) ) {
            List<string> animClips = db.texGuidToAnimClipGUIDs[_fi.textureGUID];
            animClips.Remove ( exEditorRuntimeHelper.AssetToGUID(_animClip) );
        }
    }
}

