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

#if UNITY_EDITOR
using UnityEditor;
#endif

///////////////////////////////////////////////////////////////////////////////
// defines
// NOTE: without ExecuteInEditMode, we can't not drag and create mesh in the scene 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Helper/2D Layer")]
public class exLayer2D : MonoBehaviour {

    public static int MAX_LAYER = 32;

    [SerializeField] protected int layer_ = 0; // there have 32 layer
    public int layer { get { return layer_; } }

    [SerializeField] protected float bias_ = 0.0f; // bias is a value from 0.0f to 1.0f
    public float bias { get { return bias_; } }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    protected float depth_ = 0.0f;
    public float depth { 
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

    virtual public float CalculateDepth ( Camera _cam ) { return 0.0f; }
    virtual public void UpdateTransformDepth () {}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        depth = CalculateDepth( Camera.main );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
#if UNITY_EDITOR
        if ( AnimationUtility.InAnimationMode() == false ) {
            if ( EditorApplication.isPlaying ) {
                UpdateTransformDepth ();
            }
            else {
                depth = CalculateDepth( Camera.main );
                UpdateTransformDepth ();
            }
        }
#else
        UpdateTransformDepth ();
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void SetLayer ( int _layer, float _bias ) {
        int newLayer = Mathf.Clamp( _layer, 0, MAX_LAYER-1 );

        if ( layer_ != newLayer || Mathf.Approximately(bias_, _bias) == false ) {
            layer_ = newLayer;
            bias_ = _bias;

            depth = CalculateDepth( Camera.main );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateLayer () {
        SetLayer ( layer_, bias_ );
    }
}
