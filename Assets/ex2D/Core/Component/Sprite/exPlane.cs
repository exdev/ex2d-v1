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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class ClipInfo {

        static public bool operator == ( ClipInfo _a, ClipInfo _b ) { return _a.Equals(_b); }
        static public bool operator != ( ClipInfo _a, ClipInfo _b ) { return !_a.Equals(_b); }

        public bool clipped = false; 
        public float top    = 0.0f; // percentage of clipped top
        public float bottom = 0.0f; // percentage of clipped bottom
        public float left   = 0.0f; // percentage of clipped left
        public float right  = 0.0f; // percentage of clipped right

        public override int GetHashCode() { 
            return Mathf.FloorToInt(top * 10.0f) 
                ^ Mathf.FloorToInt(bottom * 10.0f) 
                ^ Mathf.FloorToInt(left * 10.0f) 
                ^ Mathf.FloorToInt(right * 10.0f) 
                ;
        }
        public override bool Equals ( object _obj ) {
            if ( !(_obj is ClipInfo) )
                return false;

            return Equals((ClipInfo)_obj);
        }
        public bool Equals ( ClipInfo _other ) {
            if ( clipped != _other.clipped ||
                 top != _other.top ||
                 bottom != _other.bottom ||
                 left != _other.left ||
                 right != _other.right )
            {
                return false;
            }
            return true;
        }
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

    ///////////////////////////////////////////////////////////////////////////////
    // Non Serialized
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public exLayer2D layer2d;
    // NOTE: I only public this for exAnimationHelper, user should not set it
	[System.NonSerialized] public UpdateFlags updateFlags = UpdateFlags.None;
    public Rect boundingRect { get; protected set; }

    protected MeshFilter meshFilter;

    protected ClipInfo clipInfo_ = new ClipInfo();
    public ClipInfo clipInfo { 
        get { return clipInfo_; }
        set {
            if ( clipInfo_ != value ) {
                clipInfo_ = value;

                if ( clipInfo_.clipped ) {
                    if ( clipInfo_.left >= 1.0f ||
                         clipInfo_.right >= 1.0f ||
                         clipInfo_.top >= 1.0f ||
                         clipInfo_.bottom >= 1.0f )
                    {
                        enabled = false; // just hide it
                    }
                    else {
                        enabled = true;
                        updateFlags |= (UpdateFlags.Vertex|UpdateFlags.UV|UpdateFlags.Text);
                    }
                }
                else {
                    enabled = true;
                    updateFlags |= (UpdateFlags.Vertex|UpdateFlags.UV|UpdateFlags.Text);
                }
            }
        } 
    }

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
        if ( updateFlags != UpdateFlags.None ) {
            InternalUpdate();
            updateFlags = UpdateFlags.None;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual protected void InternalUpdate () {
        // Debug.LogWarning ("You should not directly call this function. please override it!");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Bounds UpdateBounds ( float _offsetX, float _offsetY, float _width, float _height ) {
        //
        float sign_w = Mathf.Sign(_width);
        float sign_h = Mathf.Sign(_height);
        boundingRect = new Rect( -_offsetX - sign_w * _width * 0.5f, 
                                  _offsetY - sign_h * _height * 0.5f, 
                                  sign_w * _width, 
                                  sign_h * _height );

        //
        switch ( plane ) {
        case exSprite.Plane.XY:
            return new Bounds (  new Vector3( -_offsetX, _offsetY, 0.0f ), 
                                 new Vector3( _width, _height, 0.2f ) );
        case exSprite.Plane.XZ:
            return new Bounds (  new Vector3( -_offsetX, 0.0f, _offsetY ), 
                                 new Vector3( _width, 0.2f, _height ) );
        case exSprite.Plane.ZY:
            return new Bounds (  new Vector3( 0.0f, _offsetY, -_offsetX ), 
                                 new Vector3( 0.2f, _height, _width ) );
        default:
            return new Bounds (  new Vector3( -_offsetX, _offsetY, 0.0f ), 
                                 new Vector3( _width, _height, 0.2f ) );
        }
    } 
}
