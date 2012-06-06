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
                _clipping.planes.Add(childPlane);
                exClipping clipPlane = childPlane as exClipping;
                // if this is a clip plane, add child to it 
                if ( clipPlane != null ) {
                    clipPlane.UpdateClipListInEditor ();
                    continue;
                }
                else {
                    Renderer childRenderer = child.renderer;
                    if ( childRenderer != null ) {
                        Texture2D texture = childRenderer.sharedMaterial.mainTexture as Texture2D;
                        if ( _clipping.textureToClipMaterialTable.ContainsKey(texture) == false ) {
                            childRenderer.sharedMaterial = exEditorHelper.GetDefaultMaterial ( texture, 
                                                                                               texture.name 
                                                                                               + "_clipping_" + _clipping.GetInstanceID(),
                                                                                               "ex2D/Alpha Blended (Clipping)" );
                            _clipping.AddClipMaterial ( texture, childRenderer.sharedMaterial );
                        }
                        else {
                            childRenderer.sharedMaterial = _clipping.textureToClipMaterialTable[texture];
                        }
                    }
                }
            }
            _clipping.RecursivelyAddToClipInEditor (child);
        }
    }

#endif // !(EX2D_EVALUATE)

}
