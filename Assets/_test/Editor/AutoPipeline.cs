// ======================================================================================
// File         : AutoPipeline.cs
// Author       : Wu Jie 
// Last Change  : 08/27/2011 | 10:13:54 AM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// AutoPipeline
///////////////////////////////////////////////////////////////////////////////

public class AutoPipeline {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem("Test/Do Auto Pipeline") ]
    static void DoAutoPipeline () {
        string path = "Assets/_test/PipelineTest";
        CreateAtlas (path);
        CreateAnimClips (path);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void CreateAtlas ( string _path ) {
        // check if we have the atlas info
        DirectoryInfo dirInfo = new DirectoryInfo( Path.Combine(_path,"atlas") );
        if ( dirInfo.Exists == false ) {
            AssetDatabase.CreateFolder ( _path, "atlas" );
        }

        // create the atlas info
        exAtlasInfo atlasInfo = exAtlasInfoUtility.CreateAtlasInfo ( Path.Combine(_path,"atlas"),
                                                                     "foobar_01",
                                                                     256,
                                                                     256 );

        // import objects
        Object[] objects = new Object[] {
            // jump
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_01.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_02.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_03.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_04.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_05.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_06.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_07.png" ), typeof(Texture2D) ),
            // run
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_01.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_02.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_03.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_04.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_05.png" ), typeof(Texture2D) ),
        };
        atlasInfo.ImportObjects ( objects );

        // layout elements
        atlasInfo.allowRotate = false; // I haven't support it
        atlasInfo.algorithm = exAtlasInfo.Algorithm.Tree; // choose tree layout algorithm
        atlasInfo.LayoutElements ();

        // build the altas
        exAtlasInfoUtility.Build ( atlasInfo );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void CreateAnimClips ( string _path ) {
        // check if we have the atlas info
        DirectoryInfo dirInfo = new DirectoryInfo( Path.Combine(_path,"anim") );
        if ( dirInfo.Exists == false ) {
            AssetDatabase.CreateFolder ( _path, "anim" );
        }
        exSpriteAnimClip clip = null;
        Object[] objects = new Object[0];

        // create jump animclips
        clip = exSpriteAnimClip.Create ( Path.Combine(_path,"anim"), "jump" );
        // add jump frames
        objects = new Object[] {
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_01.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_02.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_03.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_04.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_05.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_06.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "j_07.png" ), typeof(Texture2D) ),
        };
        exSpriteAnimationUtility.AddFrames( clip, objects );
        exSpriteAnimationUtility.Build(clip);

        // create run animclips
        clip = exSpriteAnimClip.Create ( Path.Combine(_path,"anim"), "run" );
        // add run frames
        objects = new Object[] {
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_01.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_02.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_03.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_04.png" ), typeof(Texture2D) ),
            AssetDatabase.LoadAssetAtPath( Path.Combine( _path, "r_05.png" ), typeof(Texture2D) ),
        };
        exSpriteAnimationUtility.AddFrames( clip, objects );
        exSpriteAnimationUtility.Build(clip);
    }
}
