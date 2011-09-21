// ======================================================================================
// File         : exSpriteUtility.cs
// Author       : Wu Jie 
// Last Change  : 09/01/2011 | 23:11:18 PM | Thursday,September
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
/// the sprite utility
///
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

    public static void Rebuild ( this exSprite _sprite ) {
        Texture2D texture = exEditorHelper.LoadAssetFromGUID<Texture2D>(_sprite.textureGUID);
        _sprite.Build (texture);
    }

    // ------------------------------------------------------------------ 
    /// \param _sprite the sprite
    /// \param _texture the raw texture used in the sprite
    /// build the sprite by texture
    // ------------------------------------------------------------------ 

    public static void Build ( this exSprite _sprite, Texture2D _texture = null ) {
        EditorUtility.SetDirty (_sprite);

        //
        if ( _sprite.atlas == null && _texture == null ) {
            GameObject.DestroyImmediate( _sprite.meshFilter.sharedMesh, true );
            _sprite.meshFilter.sharedMesh = null; 
            _sprite.renderer.sharedMaterial = null;
            return;
        }

        //
        if ( _sprite.useAtlas == false && _sprite.customSize == false && _sprite.trimTexture == false ) {
            _sprite.width = _texture.width;
            _sprite.height = _texture.height;
        }

        // NOTE: it is possible user duplicate an GameObject, 
        //       if we directly change the mesh, the original one will changed either.
        Mesh newMesh = new Mesh();
        newMesh.Clear();

        // build vertices, normals, uvs and colors.
        _sprite.ForceUpdateMesh( newMesh );

        // set the new mesh in MeshFilter
        GameObject.DestroyImmediate( _sprite.meshFilter.sharedMesh, true ); // delete old mesh (to avoid leaking)
        _sprite.meshFilter.sharedMesh = newMesh; 

        // set a texture to it
        if ( _sprite.atlas != null ) {
            _sprite.renderer.sharedMaterial = _sprite.atlas.material;
        }
        else if ( _texture != null ) {
            string texturePath = AssetDatabase.GetAssetPath(_texture);

            // load material from "texture_path/Materials/texture_name.mat"
            string materialDirectory = Path.Combine( Path.GetDirectoryName(texturePath), "Materials" );
            string materialPath = Path.Combine( materialDirectory, _texture.name + ".mat" );
            Material newMaterial = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));

            // if not found, load material from "texture_path/texture_name.mat"
            if ( newMaterial == null ) {
                newMaterial = (Material)AssetDatabase.LoadAssetAtPath( Path.Combine( Path.GetDirectoryName(texturePath), 
                                                                                     Path.GetFileNameWithoutExtension(texturePath) + ".mat" ), 
                                                                       typeof(Material) );
            }

            if ( newMaterial == null ) {
                // check if directory exists, if not, create one.
                DirectoryInfo info = new DirectoryInfo(materialDirectory);
                if ( info.Exists == false )
                    AssetDatabase.CreateFolder ( texturePath, "Materials" );

                // create temp materal
                newMaterial = new Material( Shader.Find("ex2D/Alpha Blended") );
                newMaterial.mainTexture = _texture;

                AssetDatabase.CreateAsset(newMaterial, materialPath);
                AssetDatabase.Refresh();
            }

            // assign it
            _sprite.renderer.sharedMaterial = newMaterial;
        }
        EditorUtility.UnloadUnusedAssets();

        // update layer2d
        if ( _sprite.layer2d ) {
            _sprite.layer2d.RecursivelyUpdateLayer ();
        }

        // update collider
        if ( _sprite.collisionHelper ) {
            _sprite.collisionHelper.UpdateCollider ();
        }
    }

}

