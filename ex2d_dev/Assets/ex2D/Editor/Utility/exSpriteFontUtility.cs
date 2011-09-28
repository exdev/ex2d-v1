// ======================================================================================
// File         : exSpriteFontUtility.cs
// Author       : Wu Jie 
// Last Change  : 09/01/2011 | 23:30:30 PM | Thursday,September
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
/// the sprite font utility
///
///////////////////////////////////////////////////////////////////////////////

public static class exSpriteFontUtility {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/SpriteFont Object")]
    static void CreateSpriteFontObject () {
        GameObject go = new GameObject("SpriteFontObject");
        go.AddComponent<exSpriteFont>();
        Selection.activeObject = go;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void Rebuild ( this exSpriteFont _spriteFont ) {
        _spriteFont.Build ();
    }

    // ------------------------------------------------------------------ 
    /// \param _spriteFont the sprite font
    /// build the sprite font
    // ------------------------------------------------------------------ 

    public static void Build ( this exSpriteFont _spriteFont ) {
        bool isPrefab = (EditorUtility.GetPrefabType(_spriteFont) == PrefabType.Prefab); 

        // when build, alway set dirty
        EditorUtility.SetDirty (_spriteFont);

        //
        if ( _spriteFont.fontInfo == null ) {
            GameObject.DestroyImmediate( _spriteFont.meshFilter.sharedMesh, true );
            _spriteFont.meshFilter.sharedMesh = null; 
            _spriteFont.renderer.sharedMaterial = null;
            return;
        }

        // prefab do not need rebuild mesh
        if ( isPrefab == false ) {
            //
            Mesh newMesh = new Mesh();
            newMesh.Clear();

            // update material 
            _spriteFont.renderer.sharedMaterial = _spriteFont.fontInfo.pageInfos[0].material;

            // update mesh
            _spriteFont.ForceUpdateMesh (newMesh);

            //
            GameObject.DestroyImmediate( _spriteFont.meshFilter.sharedMesh, true ); // delete old mesh (to avoid leaking)
            _spriteFont.meshFilter.sharedMesh = newMesh;
        }

        // update layer2d
        if ( _spriteFont.layer2d ) {
            _spriteFont.layer2d.RecursivelyUpdateLayer ();
        }

        // update collider
        if ( _spriteFont.collisionHelper ) {
            _spriteFont.collisionHelper.UpdateCollider ();
        }
    }
}
