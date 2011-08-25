// ======================================================================================
// File         : exSoftClip.cs
// Author       : Wu Jie 
// Last Change  : 08/25/2011 | 01:48:47 AM | Thursday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Helper/Soft Clip")]
public class exSoftClip : exPlane {

    public Vector2 center = Vector2.zero;

    override public float width {
        get { return width_; }
        set {
            if ( width_ != value ) {
                width_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    override public float height {
        get { return height_; }
        set {
            if ( height_ != value ) {
                height_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    public List<exSpriteBase> sprites = new List<exSpriteBase>();

    ///////////////////////////////////////////////////////////////////////////////
    // static functions
    ///////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D SoftClipObject")]
    static void CreateSoftClipObject () {
        GameObject go = new GameObject("SoftClipObject");
        go.AddComponent<exSoftClip>();
        Selection.activeObject = go;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [ContextMenu ("Add To Clip")]
    void AddToClip () {
        sprites.Clear();
        if ( transform.childCount > 0 )
            RecursivelyAddToClip (transform);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RecursivelyAddToClip ( Transform _t ) {
        foreach ( Transform child in _t ) {
            exSpriteBase spriteBase = child.GetComponent<exSpriteBase>();
            if ( spriteBase != null ) {
                sprites.Add(spriteBase);
                if ( spriteBase.transform.childCount > 0 )
                    RecursivelyAddToClip (spriteBase.transform);
            }

            // exSpriteBase spriteBase = child.GetComponent<exSoftClip>();
        }
    }
#endif

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDrawGizmos () {
        //
        Vector3 center_v3 = transform.position;
        Vector3 size_v3 = Vector3.zero;
        float halfWidth = width_ * 0.5f;
        float halfHeight = height_ * 0.5f;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        //
        switch ( anchor ) {
        case Anchor.TopLeft     : offsetX = -halfWidth;   offsetY = -halfHeight;  break;
        case Anchor.TopCenter   : offsetX = 0.0f;         offsetY = -halfHeight;  break;
        case Anchor.TopRight    : offsetX = halfWidth;    offsetY = -halfHeight;  break;

        case Anchor.MidLeft     : offsetX = -halfWidth;   offsetY = 0.0f;         break;
        case Anchor.MidCenter   : offsetX = 0.0f;         offsetY = 0.0f;         break;
        case Anchor.MidRight    : offsetX = halfWidth;    offsetY = 0.0f;         break;

        case Anchor.BotLeft     : offsetX = -halfWidth;   offsetY = halfHeight;   break;
        case Anchor.BotCenter   : offsetX = 0.0f;         offsetY = halfHeight;   break;
        case Anchor.BotRight    : offsetX = halfWidth;    offsetY = halfHeight;   break;

        default                 : offsetX = 0.0f;         offsetY = 0.0f;         break;
        }

        //
        float x = center.x - offsetX;
        float y = center.y + offsetY;
        switch ( plane ) {
        case exPlane.Plane.XY:
            center_v3 += new Vector3( x, y, 0.0f );
            size_v3 = new Vector3 ( width_, height_, 0.0f );
            break;
        case exPlane.Plane.XZ:
            center_v3 += new Vector3( x, 0.0f, y );
            size_v3 = new Vector3 ( width_, 0.0f, height_ );
            break;
        case exPlane.Plane.ZY:
            center_v3 += new Vector3( 0.0f, y, x );
            size_v3 = new Vector3 ( 0.0f, height_, width_ );
            break;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube ( center_v3, size_v3 );
        // Gizmos.color = new Color ( 1.0f, 1.0f, 0.0f, 0.0001f ); // this is very hack
        // Gizmos.DrawCube ( center_v3, new Vector3 ( size.x, size.y, 0.0f ) );
    }
#endif
}

