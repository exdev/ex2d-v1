// ======================================================================================
// File         : exLayerMng.cs
// Author       : Wu Jie 
// Last Change  : 11/06/2011 | 17:23:35 PM | Sunday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
/// 
/// A component to manage draw order
/// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Helper/Layer Manager")]
public class exLayerMng : exLayer {

    bool needsUpdate = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void Awake () {
        base.Awake();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnPreRender () {
        if ( needsUpdate ) {
            needsUpdate = false;

            List<exLayer> layerList = new List<exLayer>();
            RecursivelyAddLayer ( ref layerList, this );

            float dist = camera.farClipPlane - camera.nearClipPlane;
            float unitLayer = dist/layerList.Count;
            for ( int i = 0; i < layerList.Count; ++i ) {
                exLayer layer = layerList[i];
                Transform trans = layer.transform;
                trans.position = new Vector3( trans.position.x,
                                              trans.position.y,
                                              unitLayer * i );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RecursivelyAddLayer ( ref List<exLayer> _layerList, exLayer _curLayer ) {
        _layerList.Add ( _curLayer );
        foreach ( exLayer childLayer in _curLayer.children ) {
            RecursivelyAddLayer ( ref _layerList, childLayer );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateLayer () {
        needsUpdate = true;
    }
}
