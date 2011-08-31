// ======================================================================================
// File         : exLayer2D.cs
// Author       : Wu Jie 
// Last Change  : 07/29/2011 | 18:51:20 PM | Friday,July
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

    protected float depth = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        depth = exLayer2D.CalcDepth( Camera.main, layer_, bias_ );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static float CalcDepth ( Camera _cam, int _layer, float _bias ) {
        if ( _cam == null )
            return 0.0f;
        float dist = _cam.farClipPlane - _cam.nearClipPlane;
        float unitLayer = dist/MAX_LAYER;
        return ((float)_layer + _bias) * unitLayer + _cam.transform.position.z + _cam.nearClipPlane;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
#if UNITY_EDITOR
        if ( AnimationUtility.InAnimationMode() == false ) {
            if ( EditorApplication.isPlaying ) {
                if ( Mathf.Approximately(depth, transform.position.z) == false ) {
                    transform.position = new Vector3( transform.position.x,
                                                      transform.position.y,
                                                      depth );
                }
            }
            else {
                float newDepth = exLayer2D.CalcDepth( Camera.main, layer_, bias_ );
                if ( Mathf.Approximately(newDepth,depth) == false || Mathf.Approximately(newDepth,transform.position.z) == false ) {
                    depth = newDepth;
                    transform.position = new Vector3( transform.position.x,
                                                      transform.position.y,
                                                      depth );
                }
            }
        }
#else
        if ( Mathf.Approximately(depth, transform.position.z) == false ) {
            transform.position = new Vector3( transform.position.x,
                                              transform.position.y,
                                              depth );
        }
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

            depth = exLayer2D.CalcDepth( Camera.main, layer_, bias_ );
            transform.position = new Vector3( transform.position.x,
                                              transform.position.y,
                                              depth );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateLayer () {
        SetLayer ( layer_, bias_ );
    }
}
