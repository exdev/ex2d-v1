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

    public List<exSpriteAnimClip> data = new List<exSpriteAnimClip>();
    public Dictionary<string,List<exSpriteAnimClip> > 
        guidToAnimClips = new Dictionary<string,List<exSpriteAnimClip> >();

    public bool showData = true;
    public bool showTable = true;

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    static bool needSync = false;
    static exSpriteAnimationDB db;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void Sync () {
        if ( db == null )
            CreateDB ();

        needSync = true;
        SyncRoot();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void SyncRoot () {
        // clear data
        db.data.Clear();
        db.guidToAnimClips.Clear();

        EditorUtility.DisplayProgressBar( "Syncing exSpriteAnimationDB...", "Syncing...", 0.5f );    
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
            exSpriteAnimClip spAnimClip = (exSpriteAnimClip)AssetDatabase.LoadAssetAtPath( fileName, typeof(exSpriteAnimClip) );
            if ( spAnimClip ) {
                AddSpriteAnimClip(spAnimClip);
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
        // get sprite animation clip db, if not found, create one
        db = (exSpriteAnimationDB)AssetDatabase.LoadAssetAtPath( "Assets/.ex2D_SpriteAnimationDB.asset", typeof(exSpriteAnimationDB) );
        if ( db == null ) {
            db = ScriptableObject.CreateInstance<exSpriteAnimationDB>();
            AssetDatabase.CreateAsset( db, "Assets/.ex2D_SpriteAnimationDB.asset" );
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
                needSync = true;
                SyncRoot();
            }
            else {
                // create atlas element table
                for ( int i = 0; i < db.data.Count; ++i ) {
                    exSpriteAnimClip spAnimClip = db.data[i];
                    if ( spAnimClip == null ) {
                        db.data.RemoveAt(i);
                        --i;
                        continue;
                    }

                    // update sprite anim clip
                    foreach ( exSpriteAnimClip.FrameInfo fi in spAnimClip.frameInfos ) {
                        UpdateDataBase ( spAnimClip, fi );
                    }
                }

                EditorUtility.SetDirty(db);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public Dictionary<string,List<exSpriteAnimClip> > GetGUIDToAnimClips () {
        Init();

        return db.guidToAnimClips;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public List<exSpriteAnimClip> GetSpriteAnimClips ( string _textureGUID ) {
        Init();

        if ( db.guidToAnimClips.ContainsKey(_textureGUID) ) 
            return db.guidToAnimClips[_textureGUID]; 
        return null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void AddSpriteAnimClip ( exSpriteAnimClip _animClip ) {
        Init();

        if ( db.data.Contains(_animClip) == false ) {
            db.data.Add(_animClip);

            // update sprite anim clip
            foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
                UpdateDataBase ( _animClip, fi );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RemoveSpriteAnimClip ( exSpriteAnimClip _animClip ) {
        Init();

        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            if ( db.guidToAnimClips.ContainsKey(fi.textureGUID) ) {
                List<exSpriteAnimClip> animClips = db.guidToAnimClips[fi.textureGUID];
                animClips.Remove (_animClip);
            }
        }

        db.data.Remove(_animClip);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void UpdateDataBase ( exSpriteAnimClip _animClip, exSpriteAnimClip.FrameInfo _fi ) 
    {
        Init();

        List<exSpriteAnimClip> animClips;
        if ( db.guidToAnimClips.ContainsKey(_fi.textureGUID) == false ) {
            animClips = new List<exSpriteAnimClip>();
            db.guidToAnimClips[_fi.textureGUID] = animClips;
        }
        else {
            animClips = db.guidToAnimClips[_fi.textureGUID];
        }
        int idx = animClips.IndexOf(_animClip);
        if ( idx == -1 ) {
            animClips.Add (_animClip);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void OnFrameInfoRemoved ( exSpriteAnimClip _animClip, exSpriteAnimClip.FrameInfo _fi ) 
    {
        Init();

        if ( db.guidToAnimClips.ContainsKey(_fi.textureGUID) ) {
            List<exSpriteAnimClip> animClips = db.guidToAnimClips[_fi.textureGUID];
            animClips.Remove (_animClip);
        }
    }
}

