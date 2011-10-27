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

[System.Serializable]
public class exUIEvent {
    public enum Type {
    }
}

// ------------------------------------------------------------------ 
// Desc: 
// ------------------------------------------------------------------ 

public class exUIMng : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Camera uiCamera;
    // public exUIElement[] elements;

    //
    private List<exUIElement> elements = new List<exUIElement>();
    private RaycastSorter raycastSorter = new RaycastSorter();

    // internal ui status
    private Vector2 curMousePos = Vector2.zero;
    // private exUIElement curDragElement = null;
    private exUIElement curElement = null;
    private RaycastHit[] sortedHits;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

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

    void Awake () {
        //
        if ( uiCamera == null ) {
            Debug.LogWarning ( "Please specifiy an Ui Camera in Inspector, use MainCamera as default." );
            uiCamera = Camera.main;
        }

        // recursively add ui-tree
        foreach ( Transform child in transform ) {
            exUIElement child_el = child.GetComponent<exUIElement>();
            if ( child_el ) {
                elements.Add(child_el);
                exUIElement.FindAndAddChild (child_el);
            }
        }
        // foreach ( exUIElement el in elements ) {
        //     exUIElement.FindAndAddChild(el);
        // }
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        ProcessInput();
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessInput () {
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
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ProcessMouse () {
        // TODO { 
        // check mouse drag, mouse move and barabara
        // } TODO end 

        curMousePos = Input.mousePosition;
        exUIElement lastElement = curElement;
        bool eventHandled = false;

        // DELME { 
        // Vector2 lastMousePos = curMousePos;
        // Vector2 deltaPos = curMousePos - lastMousePos;
        // } DELME end 

        // handle mouse dragging
        if ( curElement ) {
            eventHandled = curElement.UpdateMouseDrag();
        }

        // if the mouse dragging event not processed
        // remain to normal event process
        if ( !eventHandled ) {
            curElement = PickTopElement(curMousePos);

            // process hover in/out
            if ( lastElement != curElement ) {
                if ( lastElement )
                    lastElement.MouseExit();
                if ( curElement )
                    curElement.MouseEnter();
            }

            // process mouse down/up
            exUIElement process_el = curElement;
            if ( process_el == null ) {
                foreach ( exUIElement el in elements ) {
                    if ( el.enabled ) {
                        process_el = el;
                        break;
                    }
                }
            }
            if ( process_el ) {
                if ( Input.GetMouseButtonDown(0) || 
                     Input.GetMouseButtonDown(1) ||
                     Input.GetMouseButtonDown(2) )
                {
                    process_el.MouseButtonDown();
                }
                if ( Input.GetMouseButtonUp(0) || 
                     Input.GetMouseButtonUp(1) ||
                     Input.GetMouseButtonUp(2) )
                {
                    process_el.MouseButtonUp();
                }
            }
        }

        // DEBUG { 
        // Debug.Log ( "curElement = " + curElement );
        // } DEBUG end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    exUIElement PickTopElement ( Vector2 _pos ) {
        Ray ray = uiCamera.ScreenPointToRay ( _pos );
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
