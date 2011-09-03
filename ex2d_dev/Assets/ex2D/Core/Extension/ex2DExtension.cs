// ======================================================================================
// File         : ex2DExtension.cs
// Author       : Wu Jie 
// Last Change  : 08/29/2011 | 11:55:07 AM | Monday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
/// \class ex2DExtension
/// 
/// ex2D Extensions
/// 
///////////////////////////////////////////////////////////////////////////////

public static class ex2DExtension {

    // ------------------------------------------------------------------ 
    /// \fn MakePixelPerfect
    /// \param _sp the base sprite
    /// \param _camera the camera
    /// \param _screenWidth the screen width
    /// \param _screenHeight the screen height
    /// Scale the exSpriteBase.scale to make it have renderer in perfect pixel in _camera
    // ------------------------------------------------------------------ 

    public static void MakePixelPerfect ( this exSpriteBase _sp, 
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
    /// \fn ScreenToWorldPoint
    /// \param _plane the in plane
    /// \param _camera the in camera
    /// \param _screen_x the screen x position
    /// \param _screen_y the screen y position
    /// \return the world position
    /// Convert the screen position to world position in _camera depends on exPlane.plane
    // ------------------------------------------------------------------ 

    public static Vector3 ScreenToWorldPoint ( this exPlane _plane, 
                                               Camera _camera,
                                               float _screen_x,
                                               float _screen_y ) 
    {
        switch ( _plane.plane ) {
        case exPlane.Plane.XY:
            return _camera.ScreenToWorldPoint( new Vector3(_screen_x, _screen_y, _plane.transform.position.z) );

        case exPlane.Plane.XZ:
            return _camera.ScreenToWorldPoint( new Vector3(_screen_x, _plane.transform.position.y, _screen_y) );

        case exPlane.Plane.ZY:
            return _camera.ScreenToWorldPoint( new Vector3(_plane.transform.position.x, _screen_y, _screen_x) );

        default:
            return _camera.ScreenToWorldPoint( new Vector3(_screen_x, _screen_y, _plane.transform.position.z) );
        }
    }

    // ------------------------------------------------------------------ 
    /// \fn ScreenToWorldPoint
    /// \param _plane the in plane
    /// \param _camera the in camera
    /// \param _viewport_x the viewport x position
    /// \param _viewport_y the viewport y position
    /// \return the world position
    /// Convert the viewport position to world position in _camera depends on exPlane.plane
    // ------------------------------------------------------------------ 

    public static Vector3 ViewportToWorldPoint ( this exPlane _plane, 
                                                 Camera _camera,
                                                 float _viewport_x,
                                                 float _viewport_y ) 
    {
        switch ( _plane.plane ) {
        case exPlane.Plane.XY:
            return _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _viewport_y, _plane.transform.position.z) );

        case exPlane.Plane.XZ:
            return _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _plane.transform.position.y, _viewport_y) );

        case exPlane.Plane.ZY:
            return _camera.ViewportToWorldPoint( new Vector3(_plane.transform.position.x, _viewport_y, _viewport_x) );

        default:
            return _camera.ViewportToWorldPoint( new Vector3(_viewport_x, _viewport_y, _plane.transform.position.z) );
        }
    }
}
