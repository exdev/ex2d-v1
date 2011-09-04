// ======================================================================================
// File         : exLayer2D.cs
// Author       : Wu Jie 
// Last Change  : 09/01/2011 | 14:35:44 PM | Thursday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
///
/// The base layer class
///
///////////////////////////////////////////////////////////////////////////////

[AddComponentMenu("ex2D Helper/2D Layer")]
public class exLayer2D : MonoBehaviour {

    // ------------------------------------------------------------------ 
    /// \memberof MAX_LAYER
    // ------------------------------------------------------------------ 

    public static int MAX_LAYER = 32;

    // ------------------------------------------------------------------ 
    [SerializeField] protected int layer_ = 0;
    /// layer is a value from 0 to exLayer2D.MAX_LAYER
    // ------------------------------------------------------------------ 

    public int layer { get { return layer_; } }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float bias_ = 0.0f;
    /// bias is a value from 0.0f to 1.0f
    // ------------------------------------------------------------------ 

    public float bias { get { return bias_; } }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    protected float depth_ = 0.0f;
    protected float depth { 
        get { return depth_; } 
        set {
            if ( Mathf.Approximately(depth_,value) == false ) {
                depth_ = value;
                UpdateTransformDepth ();
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    virtual protected float CalculateDepth ( Camera _cam ) { return 0.0f; }
    virtual protected void UpdateTransformDepth () {}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        UpdateDepth ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
// DELME: we will use OnSceneGUI solve this { 
// #if UNITY_EDITOR
//         if ( AnimationUtility.InAnimationMode() == false ) {
//             if ( EditorApplication.isPlaying ) {
//                 UpdateTransformDepth ();
//             }
//             else {
//                 depth = CalculateDepth( Camera.main );
//                 UpdateTransformDepth ();
//             }
//         }
// #else
// } DELME end 
        UpdateTransformDepth ();
    }

    // ------------------------------------------------------------------ 
    /// \param _layer the expect layer
    /// \param _bias the expect bias
    /// set the layer and bias of the sprite
    // ------------------------------------------------------------------ 

    public void SetLayer ( int _layer, float _bias ) {
        int newLayer = Mathf.Clamp( _layer, 0, MAX_LAYER-1 );

        if ( layer_ != newLayer || Mathf.Approximately(bias_, _bias) == false ) {
            layer_ = newLayer;
            bias_ = _bias;

            UpdateDepth ();
        }
    }

    // ------------------------------------------------------------------ 
    /// Calculate and update the depth manually, useful in editor
    // ------------------------------------------------------------------ 

    public void UpdateDepth () {
        depth = CalculateDepth( Camera.main );
    }

}
