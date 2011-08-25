// ======================================================================================
// File         : exPixelPerfect.cs
// Author       : Wu Jie 
// Last Change  : 07/24/2011 | 20:19:52 PM | Sunday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exPixelPerfect : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    [SerializeField] protected Camera camera_;
    public Camera renderCamera {
        get { return camera_; }
        set {
            if ( value == null )
                camera_ = Camera.main;
            else
                camera_ = value;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // private data
    ///////////////////////////////////////////////////////////////////////////////

    private exSpriteBase sprite;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void MakePixelPerfect ( exSpriteBase _sp, 
                                          Camera _camera, 
                                          float _screenWidth, 
                                          float _screenHeight ) {
        float s = 1.0f;
        if ( _camera.orthographic ) {
            s =  2.0f * _camera.orthographicSize / _screenHeight;
        }
        else {
            float ratio = 2.0f * Mathf.Tan(Mathf.Deg2Rad * _camera.fov * 0.5f) / _screenHeight;
            s = ratio * ( _sp.transform.position.z - _camera.transform.position.z );
        }
		_sp.scale = new Vector2( Mathf.Sign(_sp.scale.x) * s, Mathf.Sign(_sp.scale.y) * s );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        sprite = GetComponent<exSpriteBase>();
        if ( camera_ == null )
            camera_ = Camera.main;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        if ( sprite ) {
            MakePixelPerfect ( sprite, camera_, Screen.width, Screen.height );
        }
    }
}
