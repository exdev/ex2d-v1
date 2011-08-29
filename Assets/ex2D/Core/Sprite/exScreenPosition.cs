// ======================================================================================
// File         : exScreenPosition.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 10:15:15 AM | Saturday,August
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
[AddComponentMenu("ex2D Helper/Screen Position")]
public class exScreenPosition : MonoBehaviour {

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

    [SerializeField] protected float x_;
    public float x {
        get { return x_; }
        set {
            if ( value != x_ )
                x_ = value;
        }
    }

    [SerializeField] protected float y_;
    public float y {
        get { return y_; }
        set {
            if ( value != y_ )
                y_ = value;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    exPlane plane;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // //  example: CalculateWorldPosition(Screen.width, Screen.height) 
    // // ------------------------------------------------------------------ 

    // Vector3 CalculateWorldPosition ( float _screenWidth, float _screenHeight ) {
    //     float s = 1.0f;
    //     if ( camera_.orthographic ) {
    //         s =  2.0f * camera_.orthographicSize / _screenHeight;
    //     }
    //     else {
    //         float ratio = 2.0f * Mathf.Tan(Mathf.Deg2Rad * camera_.fov * 0.5f) / _screenHeight;
    //         s = ratio * ( transform.position.z - camera_.transform.position.z );
    //     }

    //     return new Vector3( (x_ - 0.5f * _screenWidth) * s,
    //                         (y_ - 0.5f * _screenHeight) * s,
    //                         transform.position.z );
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        if ( camera_ == null )
            camera_ = Camera.main;
        plane = GetComponent<exPlane>();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        //
        Vector3 newPos = Vector3.zero;

        //
        if ( plane )
            newPos = plane.ScreenToWorldPoint ( camera_, x_, y_ );
        else 
            newPos = camera_.ScreenToWorldPoint( new Vector3(x_, y_, transform.position.z) );
        newPos.z = transform.position.z;

        //
        if ( newPos != transform.position ) {
            transform.position = newPos;
        }
    }
}
