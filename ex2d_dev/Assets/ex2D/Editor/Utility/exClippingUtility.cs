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

    // // ------------------------------------------------------------------ 
    // /// \param _clipping the clipping plane
    // /// update clip list in editor
    // // ------------------------------------------------------------------ 

    // public static void UpdateClipListInEditor ( this exClipping _clipping ) {
    //     _clipping.planes.Clear();
    //     if ( _clipping.transform.childCount > 0 )
    //         _clipping.RecursivelyAddToClipInEditor (_clipping.transform);
    // }

    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // static void RecursivelyAddToClipInEditor ( this exClipping _clipping, Transform _t ) {
    //     foreach ( Transform child in _t ) {
    //         exPlane childPlane = child.GetComponent<exPlane>();
    //         if ( childPlane != null ) {
    //             AddPlaneInEditor ( _clipping, childPlane );

    //             exClipping clipPlane = childPlane as exClipping;
    //             // if this is a clip plane, add child to it 
    //             if ( clipPlane != null ) {
    //                 clipPlane.UpdateClipListInEditor ();
    //                 continue;
    //             }
    //         }
    //         _clipping.RecursivelyAddToClipInEditor (child);
    //     }
    // }

    // ------------------------------------------------------------------ 
    /// \param _clipping the clipping plane
    /// \param _plane the plane to add to clip
    /// add plane to clipping list
    // ------------------------------------------------------------------ 

    public static void InsertPlaneInEditor ( this exClipping _clipping, int _idx, exPlane _plane ) {
        _clipping.InsertPlane (_idx, _plane);

        exClipping clipPlane = _plane as exClipping;
        // if this is not a clip plane
        if ( clipPlane == null ) {
            ApplyClipMaterialInEditor ( _clipping, _plane );
        }
    }

    // ------------------------------------------------------------------ 
    /// \param _clipping the clipping plane
    /// \param _plane the plane to add to clip
    /// add plane to clipping list
    // ------------------------------------------------------------------ 

    public static void AddPlaneInEditor ( this exClipping _clipping, exPlane _plane ) {
        _clipping.AddPlane (_plane);

        exClipping clipPlane = _plane as exClipping;
        // if this is not a clip plane
        if ( clipPlane == null ) {
            ApplyClipMaterialInEditor ( _clipping, _plane );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void RemovePlaneInEditor ( this exClipping _clipping, exPlane _plane ) {
        _clipping.RemovePlane (_plane);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void ApplyClipMaterialInEditor ( this exClipping _clipping, exPlane _plane ) {
        Renderer r = _plane.renderer;
        if ( r != null && r.sharedMaterial != null ) {
            Texture2D texture = r.sharedMaterial.mainTexture as Texture2D;
            if ( _clipping.textureToClipMaterialTable.ContainsKey(texture) == false ) {
                r.sharedMaterial = exEditorHelper.GetDefaultMaterial ( texture, 
                                                                       texture.name 
                                                                       + "-clipping-" + Mathf.Abs(_clipping.GetInstanceID()),
                                                                       "ex2D/Alpha Blended (Clipping)" );
                _clipping.AddClipMaterial ( texture, r.sharedMaterial );
            }
            else {
                r.sharedMaterial = _clipping.textureToClipMaterialTable[texture];
            }
        }
    }

#endif // !(EX2D_EVALUATE)

}
