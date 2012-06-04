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

#if !(EX2D_EVALUATE)

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
#if UNITY_3_4
        bool isPrefab = (EditorUtility.GetPrefabType(_spriteBorder) == PrefabType.Prefab); 
#else
        bool isPrefab = (PrefabUtility.GetPrefabType(_spriteBorder) == PrefabType.Prefab); 
#endif
        EditorUtility.SetDirty (_spriteBorder);

        //
        if ( _spriteBorder.guiBorder == null && _spriteBorder.atlas == null && _texture == null ) {
            GameObject.DestroyImmediate( _spriteBorder.meshFilter.sharedMesh, true );
            _spriteBorder.meshFilter.sharedMesh = null; 
            _spriteBorder.renderer.sharedMaterial = null;
            return;
        }

        // set a texture to it
        if ( _spriteBorder.atlas != null ) {
            _spriteBorder.renderer.sharedMaterial = _spriteBorder.atlas.material;
        }
        else if ( _texture != null ) {
            _spriteBorder.renderer.sharedMaterial = exEditorHelper.GetDefaultMaterial(_texture, _texture.name);
        }
        EditorUtility.UnloadUnusedAssets();

        // prefab do not need rebuild mesh
        if ( isPrefab == false ) {
            // NOTE: it is possible user duplicate an GameObject, 
            //       if we directly change the mesh, the original one will changed either.
            Mesh newMesh = new Mesh();
            newMesh.Clear();

            // build vertices, normals, uvs and colors.
            _spriteBorder.ForceUpdateMesh( newMesh );

            // set the new mesh in MeshFilter
            GameObject.DestroyImmediate( _spriteBorder.meshFilter.sharedMesh, true ); // delete old mesh (to avoid leaking)
            _spriteBorder.meshFilter.sharedMesh = newMesh; 
        }

        // update collider
        if ( _spriteBorder.collisionHelper ) {
            _spriteBorder.collisionHelper.UpdateCollider ();
        }
    }

#endif // !(EX2D_EVALUATE)

}

