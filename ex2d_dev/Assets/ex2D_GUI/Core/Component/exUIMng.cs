// ======================================================================================
// File         : exUIMng.cs
// Author       : Wu Jie 
// Last Change  : 08/13/2011 | 09:52:10 AM | Saturday,August
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

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class RaycastSorter : IComparer {
    int IComparer.Compare ( object _a, object _b ) {
        if ( !(_a is RaycastHit) || !(_b is RaycastHit) ) 
            return 0;

        RaycastHit raycastHitA = (RaycastHit)_a;
        RaycastHit raycastHitB = (RaycastHit)_b;

        return raycastHitA.distance.CompareTo(raycastHitB.distance);
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class exUIEvent {

    ///////////////////////////////////////////////////////////////////////////////
    // structures
    ///////////////////////////////////////////////////////////////////////////////

    public enum Type {
        Unknown = -1,
        PointerPress = 0,
        PointerRelease,
        PointerMove,
        HoverIn,
        HoverOut,
        KeyPress,
        KeyRelease,
    }

	[System.FlagsAttribute]
    public enum MouseButtonFlags {
        None    = 0,
        Left    = 1,
        Middle  = 2,
        Right   = 4
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public exUIElement target = null;
    public Type type = Type.Unknown;
    public Vector2 position = Vector2.zero;
    public Vector2 delta = Vector2.zero;
    public MouseButtonFlags buttons = 0; // the pressed buttons
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class exUIMng : MonoBehaviour {

    protected static exUIMng instance_ = null; 
    public static exUIMng instance {
        get {
            if ( instance_ == null ) {
                instance_ = FindObjectOfType ( typeof(exUIMng) ) as exUIMng;
                // if ( instance_ == null && Application.isEditor )
                //     Debug.LogError ("Can't find exUIMng in the scene, please create one first.");
                if ( instance_ != null )
                    instance_.Init();
            }
            return instance_;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public List<exUIElement> elements = new List<exUIElement>();

    //
    private RaycastSorter raycastSorter = new RaycastSorter();
    private RaycastHit[] sortedHits;

    // internal ui status
    private bool initialized = false;
    private int touchID = -1;
    private Vector2 curPointerPos = Vector2.zero;
    private exUIEvent.MouseButtonFlags curPointerPressed = 0;
    private exUIElement hotElement = null;
    // private exUIElement activeElement = null; // TODO?

    //
    private List<exUIEvent> eventList = new List<exUIEvent>();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Init () {
        if ( initialized )
            return;

        //
        if ( camera == null ) {
            Debug.LogError ( "The exUIMng should attach to a camera" );
            return;
        }

        // recursively add ui-tree
        foreach ( Transform child in transform ) {
            exUIElement child_el = child.GetComponent<exUIElement>();
            if ( child_el ) {
                elements.Add(child_el);
                exUIElement.FindAndAddChild (child_el);
            }
        }

        initialized = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        Init ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
#if UNITY_IPHONE
        if ( Application.isEditor == false ) {
            touchID = -1;
        } else {
#endif
            curPointerPos = Input.mousePosition;
#if UNITY_IPHONE
        }
#endif
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        QueueEvents ();
        DispatchEvents ();
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public RaycastHit[] GetLastHits () { return sortedHits; }
    public bool GetLastHit ( out RaycastHit _r, GameObject _go ) { 
        foreach ( RaycastHit r in sortedHits ) {
            if ( r.collider.gameObject == _go ) {
                _r = r;
                return true;
            }
        }
        _r = new RaycastHit();
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void QueueEvents () {
        ProcessPointer ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void DispatchEvents () {
        foreach ( exUIEvent e in eventList ) {
            bool used = e.target.OnEvent(e);
            while ( used == false ) {
                exUIElement uiParent = e.target.parent;
                if ( uiParent == null )
                    break;
                used = uiParent.OnEvent(e);
            }
        }
        eventList.Clear();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessPointer () {
#if UNITY_IPHONE
        if ( Application.isEditor == false ) {
            ProcessTouch();
        } else {
#endif
            ProcessMouse();
#if UNITY_IPHONE
        }
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessTouch () {
        // TODO { 
        foreach ( Touch touch in Input.touches ) {
            if ( touch.phase == TouchPhase.Began ) {
                touchID = touch.fingerId;
                break;
            }
        }
        if ( touchID != -1 ) {
            return;
        }
        // } TODO end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMouse () {
        //
        Vector2 lastPointerPos = curPointerPos;

        // get current position
        curPointerPos = Input.mousePosition;
        Vector2 deltaPos = curPointerPos - lastPointerPos;

        // get current mouse button
        exUIEvent.MouseButtonFlags lastPointerPressed = curPointerPressed;
        exUIEvent.MouseButtonFlags buttonDown = exUIEvent.MouseButtonFlags.None;
        exUIEvent.MouseButtonFlags buttonUp = exUIEvent.MouseButtonFlags.None;

        // handle pressed
        curPointerPressed = 0;
        if ( Input.anyKey ) {
            if ( Input.GetMouseButton(0) )
                curPointerPressed |= exUIEvent.MouseButtonFlags.Left;
            if ( Input.GetMouseButton(1) )
                curPointerPressed |= exUIEvent.MouseButtonFlags.Right;
            if ( Input.GetMouseButton(2) )
                curPointerPressed |= exUIEvent.MouseButtonFlags.Middle;
        }

        // handle press
        if ( Input.anyKeyDown ) {
            if ( Input.GetMouseButtonDown(0) )
                buttonDown = exUIEvent.MouseButtonFlags.Left;
            else if ( Input.GetMouseButton(1) )
                buttonDown = exUIEvent.MouseButtonFlags.Right;
            else if ( Input.GetMouseButton(2) )
                buttonDown = exUIEvent.MouseButtonFlags.Middle;
        }

        // handle release
        if ( lastPointerPressed != curPointerPressed ) {
            if ( Input.GetMouseButtonUp(0) )
                buttonUp = exUIEvent.MouseButtonFlags.Left;
            else if ( Input.GetMouseButtonUp(1) )
                buttonUp = exUIEvent.MouseButtonFlags.Right;
            else if ( Input.GetMouseButtonUp(2) )
                buttonUp = exUIEvent.MouseButtonFlags.Middle;
        }
        
        // get hot element
        exUIElement lastHotElement = hotElement;
        hotElement = PickTopElement(curPointerPos);

        // process hover event
        if ( lastHotElement != hotElement ) {
            // add hover-in event
            if ( hotElement != null ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.HoverIn;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = hotElement;
                e.buttons = curPointerPressed;
                eventList.Add(e);
            }

            // add hover-out event
            if ( lastHotElement != null ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.HoverOut;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = lastHotElement;
                e.buttons = curPointerPressed;
                eventList.Add(e);
            }
        }

        //
        if ( hotElement != null ) {
            // add pointer-move event
            if ( deltaPos != Vector2.zero ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.PointerMove;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = hotElement;
                e.buttons = curPointerPressed;
                eventList.Add(e);
            }

            // add pointer-press event
            if ( buttonDown != exUIEvent.MouseButtonFlags.None ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.PointerPress;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = hotElement;
                e.buttons = buttonDown;
                eventList.Add(e);
            }

            // add pointer-press event
            if ( buttonUp != exUIEvent.MouseButtonFlags.None ) {
                exUIEvent e = new exUIEvent(); 
                e.type =  exUIEvent.Type.PointerRelease;
                e.position = curPointerPos;
                e.delta = deltaPos;
                e.target = hotElement;
                e.buttons = buttonUp;
                eventList.Add(e);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    exUIElement PickTopElement ( Vector2 _pos ) {
        Ray ray = camera.ScreenPointToRay ( _pos );
        ray.origin = new Vector3 ( ray.origin.x, ray.origin.y, camera.transform.position.z );
        sortedHits = Physics.RaycastAll(ray);
        System.Array.Sort(sortedHits, raycastSorter);
        if ( sortedHits.Length > 0 ) {
            for ( int i = 0; i < sortedHits.Length; ++i ) {
                RaycastHit hit = sortedHits[i];
                GameObject go = hit.collider.gameObject;
                exUIElement el = go.GetComponent<exUIElement>();
                if ( el && el.enabled ) {
                    return el;
                }
            }
        }
        return null;
    }
}
