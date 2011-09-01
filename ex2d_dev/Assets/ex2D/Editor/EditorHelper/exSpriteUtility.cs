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
// exSpriteUtility
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
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Build ( this exSprite _sprite, Texture2D _texture = null ) {
        EditorUtility.SetDirty (_sprite);

        //
        if ( _sprite.atlas == null && _texture == null ) {
            _sprite.GetComponent<MeshFilter>().sharedMesh = null; 
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
        _sprite.GetComponent<MeshFilter>().sharedMesh = newMesh; 

        // if we have mesh collider, update it.
        MeshCollider meshCol = _sprite.GetComponent<MeshCollider>();
        if ( meshCol )
            meshCol.sharedMesh = newMesh;

        // if we have box collider, update it.
        BoxCollider boxCol = _sprite.GetComponent<BoxCollider>();
        if ( boxCol ) {
            Vector3 size = newMesh.bounds.size;
            boxCol.center = newMesh.bounds.center;

            switch ( _sprite.plane ) {
            case exSprite.Plane.XY:
                boxCol.size = new Vector3( size.x, size.y, 0.2f );
                break;
            case exSprite.Plane.XZ:
                boxCol.size = new Vector3( size.x, 0.2f, size.z );
                break;
            case exSprite.Plane.ZY:
                boxCol.size = new Vector3( 0.2f, size.y, size.z );
                break;
            }
        }

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
    }

}

