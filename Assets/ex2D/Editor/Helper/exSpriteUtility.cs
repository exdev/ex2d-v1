// ======================================================================================
// File         : exSpriteUtility.cs
// Author       : Wu Jie 
// Last Change  : 08/30/2011 | 11:00:17 AM | Tuesday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// functions
///////////////////////////////////////////////////////////////////////////////

public static class exSpriteUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/Sprite Object")]
    static void CreateSpriteObject () {
        GameObject go = new GameObject("SpriteObject");
        go.AddComponent<exSprite>();
        Selection.activeObject = go;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Edit/ex2D/Update Scene Sprite Layers %&l")]
    static void UpdateSceneSpriteLayers () {
        EditorUtility.DisplayProgressBar( "Update Scene Sprite Layers...", 
                                          "Update Scene Sprite Layers...", 
                                          0.5f );    

        exLayer2D[] layerObjs = GameObject.FindObjectsOfType(typeof(exLayer2D)) as exLayer2D[];
        for ( int i = 0; i < layerObjs.Length; ++i ) {
            exLayer2D layerObj = layerObjs[i]; 
            // DISABLE: it is too slow { 
            // float progress = (float)i/(float)layerObjs.Length;
            // EditorUtility.DisplayProgressBar( "Update Scene Sprite Layers...", 
            //                                   "Update Sprite Layer " + layerObj.gameObject.name, progress );    
            // } DISABLE end 

            layerObj.UpdateLayer();
            exSpriteUtility.RecursivelyUpdateLayer(layerObj.transform);
        }
        EditorUtility.ClearProgressBar();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Edit/ex2D/Rebuild Scene Sprites %&b")]
    static void RebuildSceneSprites () {
        exSpriteBase[] sprites = GameObject.FindObjectsOfType(typeof(exSpriteBase)) as exSpriteBase[];
        RebuildSprites (sprites);
    }

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // [MenuItem ("Edit/ex2D/Rebuild Scene Sprites")]
    // static void RebuildSelectedSpriteObjects () {
    //     List<GameObject> gameObjects = new List<GameObject>();
    //     foreach ( Object obj in Selection.objects ) {
    //         if ( (obj is GameObject) == false )
    //             continue;

    //         GameObject go = obj as GameObject;
    //         GameObject root_go = go.transform.root.gameObject;
    //         if ( gameObjects.IndexOf(root_go) == -1 ) {
    //             gameObjects.Add (root_go);
    //         }
    //     }
    //     GameObject[] gos = gameObjects.ToArray();
    //     RebuildGameObjects (gos);
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void RebuildSprites ( exSpriteBase[] _sprites ) {
        EditorUtility.DisplayProgressBar( "Rebuild Scene Sprites...", 
                                          "Rebuild Scene Sprites...", 
                                          0.5f );    

        for ( int i = 0; i < _sprites.Length; ++i ) {
            exSpriteBase spBase = _sprites[i]; 
            // DISABLE: it is too slow { 
            // float progress = (float)i/(float)_sprites.Length;
            // EditorUtility.DisplayProgressBar( "Rebuild Scene Sprites...", 
            //                                   "Build Sprite " + spBase.gameObject.name, progress );    
            // } DISABLE end 
            if ( spBase is exSprite ) {
                exSprite sp = spBase as exSprite;
                Texture2D texture = exEditorHelper.LoadAssetFromGUID<Texture2D>(sp.textureGUID); 
                exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo(texture);
                exSpriteEditor.UpdateAtlas( sp, elInfo );
                sp.Build( texture );
            }
            if ( spBase is exSpriteFont ) {
                exSpriteFont spFont = spBase as exSpriteFont;
                spFont.Build();
            }

            // update layer
            exLayer2D layer2d = spBase.GetComponent<exLayer2D>();
            if ( layer2d ) {
                layer2d.UpdateLayer();
            }
        }
        EditorUtility.UnloadUnusedAssets(); // NOTE: without this you will got leaks message
        EditorUtility.ClearProgressBar();    
    }

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // static void RebuildGameObjects ( GameObject[] _objs ) {
    //     for ( int i = 0; i < _objs.Length; ++i ) {
    //         GameObject go = _objs[i];
    //     }
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void RecursivelyUpdateLayer ( Transform _trans ) {
        foreach ( Transform child in _trans ) {
            exLayer2D layer = child.GetComponent<exLayer2D>();
            if ( layer ) {
                layer.UpdateLayer();
                exSpriteUtility.RecursivelyUpdateLayer ( layer.transform );
            }
        }
    }
}
