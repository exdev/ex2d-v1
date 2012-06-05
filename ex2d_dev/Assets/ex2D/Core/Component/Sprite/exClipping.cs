// ======================================================================================
// File         : exClipping.cs
// Author       : Wu Jie 
// Last Change  : 03/05/2012 | 19:19:47 PM | Monday,March
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

///////////////////////////////////////////////////////////////////////////////
///
/// A component handles the a list of exPlane GameObjects, clip them
/// to the boundingRect.
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Sprite/Clipping")]
public class exClipping : exPlane {

    // ------------------------------------------------------------------ 
    [SerializeField] protected float width_ = 100.0f;
    /// the width of the soft-clip
    // ------------------------------------------------------------------ 

    public float width {
        get { return width_; }
        set {
            if ( width_ != value ) {
                width_ = Mathf.Max(value, 0.0f);
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    [SerializeField] protected float height_ = 100.0f;
    /// the height of the soft-clip
    // ------------------------------------------------------------------ 

    public float height {
        get { return height_; }
        set {
            if ( height_ != value ) {
                height_ = Mathf.Max(value, 0.0f);
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// the list of the planes to clip
    // ------------------------------------------------------------------ 

    public List<exPlane> planes = new List<exPlane>(); // TODO: clipInfoList { plane, srcMaterial }
    public Dictionary<Texture2D,Material> textureToClipMaterialTable = new Dictionary<Texture2D,Material>(); 

    // ------------------------------------------------------------------ 
    /// the clipped rect, if the clipping plane is a child of another soft-clip plane
    // ------------------------------------------------------------------ 

    public Rect clippedRect { get; protected set; }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////
    
    // ------------------------------------------------------------------ 
    /// update the list of planes to clip
    // ------------------------------------------------------------------ 

    public void UpdateClipList () {
        planes.Clear();
        if ( transform.childCount > 0 )
            RecursivelyAddToClip (transform);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RecursivelyAddToClip ( Transform _t ) {
        foreach ( Transform child in _t ) {
            exPlane plane = child.GetComponent<exPlane>();
            if ( plane != null ) {
                planes.Add(plane);
                exClipping clipPlane = plane as exClipping;
                // if this is a clip plane, add child to it 
                if ( clipPlane != null ) {
                    clipPlane.UpdateClipList ();
                    continue;
                }
                else {
                    Renderer renderer = child.renderer;
                    if ( renderer != null ) {
                        renderer.material.shader = Shader.Find("ex2D/Alpha Blended (Clipping)");
                    }
                }
            }
            RecursivelyAddToClip (child);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// Awake functoin inherit from exPlane.
    // ------------------------------------------------------------------ 

    protected new void Awake () {
        base.Awake();
        updateFlags |= UpdateFlags.Vertex;
        Commit();
        CommitMaterialProperties();

        spriteMng.AddToClippingList(this);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnDestroy () {
        base.OnDestroy();

        if ( spriteMng != null ) {
            spriteMng_.RemoveFromClippingList(this);
        }
    }

    // ------------------------------------------------------------------ 
    /// OnEnable functoin inherit from exPlane.
    /// When enabled set to true, it will enable all the item in the planes
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();

        for ( int i = 0; i < planes.Count; ++i ) {
            exPlane p = planes[i];
            if ( p == null ) {
                planes.RemoveAt(i);
                --i;
                continue;
            }
            p.enabled = true;
        }
    }

    // ------------------------------------------------------------------ 
    /// OnDisable functoin inherit from exPlane.
    /// When enabled set to false, it will disable all the item in the planes
    // ------------------------------------------------------------------ 

    protected new void OnDisable () {
        base.OnDisable();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Commit () {
        if ( (updateFlags & UpdateFlags.Vertex) != 0 ) {
            //
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
            boundingRect = new Rect( -offsetX - halfWidth, 
                                      offsetY - halfHeight,
                                      width_, 
                                      height_ );

            // TODO: child clip { 
            // // do clip
            // if ( clipInfo_.clipped ) {
            //     clippedRect = new Rect( boundingRect.x + clipInfo_.left * boundingRect.width, 
            //                             boundingRect.y + clipInfo_.top * boundingRect.height, 
            //                             (1.0f - clipInfo_.left - clipInfo_.right) * boundingRect.width,
            //                             (1.0f - clipInfo_.top - clipInfo_.bottom) * boundingRect.height
            //                           ); 
            // }
            // else {
            //     clippedRect = boundingRect;
            // }
            // } TODO end 
            clippedRect = boundingRect;

            if ( collisionHelper ) 
                collisionHelper.UpdateCollider();
        }

        //
        updateFlags = UpdateFlags.None;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void CommitMaterialProperties () {
        Vector4 rect = new Vector4 ( clippedRect.center.x,
                                     clippedRect.center.y,
                                     clippedRect.width, 
                                     clippedRect.height ); 

        for ( int i = 0; i < planes.Count; ++i ) {
            exPlane p = planes[i];
            Renderer r = p.renderer;
            r.sharedMaterial.SetVector ( "_ClipRect", rect );
            r.sharedMaterial.SetMatrix ( "_ClipMatrix", transform.worldToLocalMatrix );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDrawGizmos () {
        //
        Vector3[] vertices = new Vector3[4]; 
        Vector3[] corners = new Vector3[4];

        float halfWidthScaled = width * 0.5f;
        float halfHeightScaled = height * 0.5f;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        //
        switch ( anchor ) {
        case exPlane.Anchor.TopLeft     : offsetX = -halfWidthScaled;   offsetY = -halfHeightScaled;  break;
        case exPlane.Anchor.TopCenter   : offsetX = 0.0f;               offsetY = -halfHeightScaled;  break;
        case exPlane.Anchor.TopRight    : offsetX = halfWidthScaled;    offsetY = -halfHeightScaled;  break;

        case exPlane.Anchor.MidLeft     : offsetX = -halfWidthScaled;   offsetY = 0.0f;               break;
        case exPlane.Anchor.MidCenter   : offsetX = 0.0f;               offsetY = 0.0f;               break;
        case exPlane.Anchor.MidRight    : offsetX = halfWidthScaled;    offsetY = 0.0f;               break;

        case exPlane.Anchor.BotLeft     : offsetX = -halfWidthScaled;   offsetY = halfHeightScaled;   break;
        case exPlane.Anchor.BotCenter   : offsetX = 0.0f;               offsetY = halfHeightScaled;   break;
        case exPlane.Anchor.BotRight    : offsetX = halfWidthScaled;    offsetY = halfHeightScaled;   break;

        default                         : offsetX = 0.0f;               offsetY = 0.0f;               break;
        }

        //
        vertices[0] = new Vector3 (-halfWidthScaled-offsetX,  halfHeightScaled+offsetY, 0.0f );
        vertices[1] = new Vector3 ( halfWidthScaled-offsetX,  halfHeightScaled+offsetY, 0.0f );
        vertices[2] = new Vector3 ( halfWidthScaled-offsetX, -halfHeightScaled+offsetY, 0.0f );
        vertices[3] = new Vector3 (-halfWidthScaled-offsetX, -halfHeightScaled+offsetY, 0.0f );

        // 0 -- 1
        // |    |
        // 3 -- 2

        corners[0] = transform.localToWorldMatrix * new Vector4 ( vertices[0].x, vertices[0].y, vertices[0].z, 1.0f );
        corners[1] = transform.localToWorldMatrix * new Vector4 ( vertices[1].x, vertices[1].y, vertices[1].z, 1.0f );
        corners[2] = transform.localToWorldMatrix * new Vector4 ( vertices[2].x, vertices[2].y, vertices[2].z, 1.0f );
        corners[3] = transform.localToWorldMatrix * new Vector4 ( vertices[3].x, vertices[3].y, vertices[3].z, 1.0f );

        Gizmos.color = Color.red;
        Gizmos.DrawLine ( corners[0], corners[1] );
        Gizmos.DrawLine ( corners[1], corners[2] );
        Gizmos.DrawLine ( corners[2], corners[3] );
        Gizmos.DrawLine ( corners[3], corners[0] );
    }
}

