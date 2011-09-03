// ======================================================================================
// File         : exSpriteBase.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 21:18:47 PM | Saturday,August
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
/// \class exSpriteBase
///
/// The base class for rendering sprite by different assets
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exSpriteBase : exPlane {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \property scale
    /// the scale of the sprite
    // ------------------------------------------------------------------ 

    [SerializeField] protected Vector2 scale_ = Vector2.one;
    public Vector2 scale {
        get { return scale_; }
        set { 
            if ( scale_ != value ) {
                scale_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \property shear
    /// the shear of the sprite
    // ------------------------------------------------------------------ 

    [SerializeField] protected Vector2 shear_ = Vector2.zero;
    public Vector2 shear {
        get { return shear_; }
        set { 
            if ( shear_ != value ) {
                shear_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \property autoResizeCollision
    /// if the value is true and we use BoxCollider in the sprite, the 
    /// width and height of the BoxCollider will be the same as the boundingRect 
    /// of the sprite, and the thick of it will fix to 0.2f.
    // ------------------------------------------------------------------ 

    [SerializeField] protected bool autoResizeCollision_ = true;
    public bool autoResizeCollision {
        get { return autoResizeCollision_; }
        set {
            if ( autoResizeCollision_ != value ) {
                autoResizeCollision_ = value;

                BoxCollider boxCol = gameObject.GetComponent<BoxCollider>();
                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                if ( meshFilter != null ) {
                    UpdateBoxCollider ( boxCol, meshFilter.sharedMesh );
                }
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \fn OnEnable
    /// OnEnable functoin inherit from exPlane
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        exPixelPerfect ppf = GetComponent<exPixelPerfect>();
        if ( ppf ) {
            ppf.enabled = true;
        }
    }

    // ------------------------------------------------------------------ 
    /// \fn OnDisable
    /// OnDisable functoin inherit from exPlane
    // ------------------------------------------------------------------ 

    override protected void OnDisable () {
        base.OnDisable();
        exPixelPerfect ppf = GetComponent<exPixelPerfect>();
        if ( ppf ) {
            ppf.enabled = false;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Reset() {
        if ( GetComponent<exLayer2D>() == null ) {
            switch ( plane ) {
            case exPlane.Plane.XY: layer2d = gameObject.AddComponent<exLayerXY>(); break;
            case exPlane.Plane.XZ: layer2d = gameObject.AddComponent<exLayerXZ>(); break;
            case exPlane.Plane.ZY: layer2d = gameObject.AddComponent<exLayerZY>(); break;
            }
            layer2d.UpdateDepth();
        }
    }

    // ------------------------------------------------------------------ 
    /// \fn AddMeshCollider
    /// add a MeshCollider component on the sprite if no collider exists 
    // ------------------------------------------------------------------ 

    public void AddMeshCollider () {
        if ( collider == null ) {
            MeshCollider meshCol = gameObject.AddComponent<MeshCollider>();
            if ( meshCol && meshFilter ) {
                meshCol.sharedMesh = meshFilter.sharedMesh;
            }
        }
    }

    // ------------------------------------------------------------------ 
    /// \fn AddBoxCollider
    /// add a BoxCollider component on the sprite if no collider exists 
    /// if the autoResizeCollision is true, it will also update the size 
    /// BoxCollider to fit the size of sprite
    // ------------------------------------------------------------------ 

    public void AddBoxCollider () {
        if ( collider == null ) {
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            UpdateBoxCollider ( boxCol, meshFilter.sharedMesh );
        }
    }

    // ------------------------------------------------------------------ 
    /// \fn UpdateBoxCollider
    /// \param _boxCol the BoxCollider of the sprite  
    /// \param _mesh the mesh of the sprite  
    /// 
    /// Update the size BoxCollider to fit the size of sprite, only affect 
    /// when autoResizeCollision is true
    // ------------------------------------------------------------------ 

    public void UpdateBoxCollider ( BoxCollider _boxCol, Mesh _mesh ) {
        if ( _boxCol == null || _mesh == null || autoResizeCollision == false )
            return;

        _boxCol.center = _mesh.bounds.center;
        _boxCol.size = _mesh.bounds.size;
    }
}

