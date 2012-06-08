// ======================================================================================
// File         : exClippingUtility.cs
// Author       : Wu Jie 
// Last Change  : 06/05/2012 | 00:03:54 AM | Tuesday,June
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
// exClippingUtility
///////////////////////////////////////////////////////////////////////////////

public static class exClippingUtility {

#if !(EX2D_EVALUATE)

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/Clipping Object")]
    static void CreateClippingObject () {
        GameObject go = new GameObject("ClippingObject");
        go.AddComponent<exClipping>();
        Selection.activeObject = go;
    }

    // ------------------------------------------------------------------ 
    /// \param _sprite the sprite
    /// \param _texture the raw texture used in the sprite
    /// build the sprite by texture
    // ------------------------------------------------------------------ 

    public static void UpdateClipListInEditor ( this exClipping _clipping ) {
        _clipping.planes.Clear();
        if ( _clipping.transform.childCount > 0 )
            _clipping.RecursivelyAddToClipInEditor (_clipping.transform);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void RecursivelyAddToClipInEditor ( this exClipping _clipping, Transform _t ) {
        foreach ( Transform child in _t ) {
            exPlane childPlane = child.GetComponent<exPlane>();
            if ( childPlane != null ) {
                AddToClipInEditor ( _clipping, childPlane );

                exClipping clipPlane = childPlane as exClipping;
                // if this is a clip plane, add child to it 
                if ( clipPlane != null ) {
                    clipPlane.UpdateClipListInEditor ();
                    continue;
                }
            }
            _clipping.RecursivelyAddToClipInEditor (child);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void AddToClipInEditor ( this exClipping _clipping, exPlane _plane ) {
        // we already have this in clipping list
        if ( _clipping.planes.IndexOf(_plane) != -1 )
            return;

        _clipping.planes.Add(_plane);
        exClipping clipPlane = _plane as exClipping;
        // if this is not a clip plane
        if ( clipPlane == null ) {
            Renderer r = _plane.renderer;
            if ( r != null ) {
                Texture2D texture = r.sharedMaterial.mainTexture as Texture2D;
                if ( _clipping.textureToClipMaterialTable.ContainsKey(texture) == false ) {
                    r.sharedMaterial = exEditorHelper.GetDefaultMaterial ( texture, 
                                                                           texture.name 
                                                                           + "_clipping_" + _clipping.GetInstanceID(),
                                                                           "ex2D/Alpha Blended (Clipping)" );
                    _clipping.AddClipMaterial ( texture, r.sharedMaterial );
                }
                else {
                    r.sharedMaterial = _clipping.textureToClipMaterialTable[texture];
                }
            }
        }
    }

#endif // !(EX2D_EVALUATE)

}
