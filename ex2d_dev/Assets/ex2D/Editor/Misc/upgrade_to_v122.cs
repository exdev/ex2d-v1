// ======================================================================================
// File         : upgrade_to_v122.cs
// Author       : Wu Jie 
// Last Change  : 11/18/2011 | 11:20:31 AM | Friday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

public static class upgrade_to_v122 {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem("Edit/ex2D Upgrade/Use Layer Manager (v 1.2.2)")]
    static void Exec () {
        try {
            EditorUtility.DisplayProgressBar( "Update Scene Sprite Layers...", 
                                              "Update Scene Sprite Layers...", 
                                              0.5f );    

            // add layer
            Transform[] transforms = GameObject.FindObjectsOfType(typeof(Transform)) as Transform[];
            for ( int i = 0; i < transforms.Length; ++i ) {
                Transform trans = transforms[i]; 
                if ( trans.root != trans )
                    continue;

                RecursivelyAddLayer (trans);
            }

            // add layer manager
            if ( Camera.main ) {
                exLayerMng layerMng = Camera.main.gameObject.AddComponent<exLayerMng>();

                // add layers to layer manager
                for ( int i = 0; i < transforms.Length; ++i ) {
                    Transform trans = transforms[i]; 
                    if ( trans.root != trans )
                        continue;

                    exLayer layer = trans.GetComponent<exLayer>();
                    if ( layer && layer != layerMng )
                        layer.parent = layerMng;
                }
                layerMng.AddDirtyLayer(layerMng);
                EditorUtility.SetDirty(layerMng);
            }

            EditorUtility.ClearProgressBar();
        }
        catch ( System.Exception ) {
            EditorUtility.ClearProgressBar();
            throw;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void RecursivelyAddLayer ( Transform _trans ) {
        if ( _trans.camera == null ) {
            // remove layer2D
            exLayer2D oldLayer = _trans.GetComponent<exLayer2D>();
            if ( oldLayer ) {
                GameObject.DestroyImmediate (oldLayer);
            }

            // add new layer
            exLayer layer = _trans.GetComponent<exLayer>();
            if ( layer == null ) {
                layer = _trans.gameObject.AddComponent<exLayer>();
            }
            if ( _trans.parent ) {
                exLayer parentLayer = _trans.parent.GetComponent<exLayer> ();
                if ( parentLayer ) {
                    layer.parent = parentLayer;
                }
            }
            EditorUtility.SetDirty(layer);

            // update child
            foreach ( Transform child in _trans ) {
                RecursivelyAddLayer (child);
            }
        }
    }
}
