// ======================================================================================
// File         : exPlane.cs
// Author       : Wu Jie 
// Last Change  : 08/25/2011 | 17:09:53 PM | Thursday,August
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
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class exPlane : MonoBehaviour {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	[System.FlagsAttribute]
	public enum UpdateFlags {
		None		= 0,
		Vertex		= 1,
		UV	        = 2,
		Color	    = 4,
		Text	    = 8,
		Index	    = 16,
	};

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public enum Plane {
        XY,
        XZ,
        ZY
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public enum Anchor {
		TopLeft = 0,
		TopCenter,
		TopRight,
		MidLeft,
		MidCenter,
		MidRight,
		BotLeft,
		BotCenter,
		BotRight,
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    [SerializeField] protected Plane plane_ = Plane.XY;
    public Plane plane {
        get { return plane_; }
        set {
            if ( plane_ != value ) {
                plane_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    [SerializeField] protected Anchor anchor_ = Anchor.MidCenter;
    public Anchor anchor {
        get { return anchor_; }
        set {
            if ( anchor_ != value ) {
                anchor_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    [SerializeField] protected float width_ = 1.0f;
    virtual public float width {
        get { return width_; }
        set { Debug.LogError("Readonly"); }
    }

    [SerializeField] protected float height_ = 1.0f;
    virtual public float height {
        get { return height_; }
        set { Debug.LogError("Readonly"); }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Non Serialized
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public exLayer2D layer2d;
    protected MeshFilter meshFilter;
	protected UpdateFlags updateFlags = UpdateFlags.None;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual protected void Awake () {
        meshFilter = GetComponent<MeshFilter>();
        layer2d = GetComponent<exLayer2D>();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual protected void OnEnable () {
        if ( renderer != null )
            renderer.enabled = true;

        // NOTE: though we have ExecuteInEditMode, user can Add/Remove layer2d in Editor
#if UNITY_EDITOR
        exLayer2D my_layer2d = layer2d;
        if ( EditorApplication.isPlaying == false ) {
            my_layer2d = GetComponent<exLayer2D>();
        }
        if ( my_layer2d ) {
            my_layer2d.enabled = true;
        }
#else
        if ( layer2d ) {
            layer2d.enabled = true;
        }
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual protected void OnDisable () {
        if ( renderer != null )
            renderer.enabled = false;

        // NOTE: though we have ExecuteInEditMode, user can Add/Remove layer2d in Editor
#if UNITY_EDITOR
        exLayer2D my_layer2d = layer2d;
        if ( EditorApplication.isPlaying == false ) {
            my_layer2d = GetComponent<exLayer2D>();
        }
        if ( my_layer2d ) {
            my_layer2d.enabled = false;
        }
#else
        if ( layer2d ) {
            layer2d.enabled = false;
        }
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        if ( meshFilter != null && 
             meshFilter.sharedMesh != null &&
             updateFlags != UpdateFlags.None ) 
        {
            UpdateMesh( meshFilter.sharedMesh );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual public void UpdateMesh ( Mesh _mesh ) {
        // Debug.LogWarning ("You should not directly call this function. please override it!");
    }
}
