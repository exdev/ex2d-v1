// ======================================================================================
// File         : exSpriteBorderUtility.cs
// Author       : Wu Jie 
// Last Change  : 09/21/2011 | 09:19:24 AM | Wednesday,September
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

public static class exSpriteBorderUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/SpriteBorder Object")]
    static void CreateSpriteBorderObject () {
        GameObject go = new GameObject("SpriteBorderObject");
        go.AddComponent<exSpriteBorder>();
        Selection.activeObject = go;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Rebuild ( this exSpriteBorder _spriteBorder ) {
        Texture2D texture = exEditorHelper.LoadAssetFromGUID<Texture2D>(_spriteBorder.guiBorder.textureGUID);
        _spriteBorder.Build (texture);
    }

    // ------------------------------------------------------------------ 
    /// \param _spriteBorder the sprite
    /// \param _texture the raw texture used in the sprite
    /// build the sprite by texture
    // ------------------------------------------------------------------ 

    public static void Build ( this exSpriteBorder _spriteBorder, Texture2D _texture = null ) {
        EditorUtility.SetDirty (_spriteBorder);

        //
        if ( _spriteBorder.guiBorder == null && _spriteBorder.atlas == null && _texture == null ) {
            GameObject.DestroyImmediate( _spriteBorder.meshFilter.sharedMesh, true );
            _spriteBorder.meshFilter.sharedMesh = null; 
            _spriteBorder.renderer.sharedMaterial = null;
            return;
        }

        // NOTE: it is possible user duplicate an GameObject, 
        //       if we directly change the mesh, the original one will changed either.
        Mesh newMesh = new Mesh();
        newMesh.Clear();

        // build vertices, normals, uvs and colors.
        _spriteBorder.ForceUpdateMesh( newMesh );

        // set the new mesh in MeshFilter
        GameObject.DestroyImmediate( _spriteBorder.meshFilter.sharedMesh, true ); // delete old mesh (to avoid leaking)
        _spriteBorder.meshFilter.sharedMesh = newMesh; 

        // set a texture to it
        if ( _spriteBorder.atlas != null ) {
            _spriteBorder.renderer.sharedMaterial = _spriteBorder.atlas.material;
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
            _spriteBorder.renderer.sharedMaterial = newMaterial;
        }
        EditorUtility.UnloadUnusedAssets();

        // update layer2d
        if ( _spriteBorder.layer2d ) {
            _spriteBorder.layer2d.RecursivelyUpdateLayer ();
        }

        // update collider
        if ( _spriteBorder.collisionHelper ) {
            _spriteBorder.collisionHelper.UpdateCollider ();
        }
    }

}

