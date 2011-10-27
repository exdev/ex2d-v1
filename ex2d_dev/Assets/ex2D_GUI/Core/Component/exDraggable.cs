// ======================================================================================
// File         : exDraggable.cs
// Author       : Wu Jie 
// Last Change  : 06/03/2011 | 15:35:54 PM | Friday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// Interactions Data
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class exDraggable : MonoBehaviour {

    enum State {
        None,
        Detecting,
        Dragging,
    }

    public bool axisX = true; // can drag along axis-x
    public bool axisY = true; // can drag along axis-y
    public float delay = 0.0f; // delay for seconds
    public float distance = 0.0f; // the threshold of distance to start drag 
    // TODO: grid
    // TODO: handle // If specified, restricts drag start click to the specified element(s).

    // internal state
    private State state = State.None;
    private Vector2 anchor = Vector3.zero; // where mouse press start
    private Vector2 dragPoint;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsInDraggState () { return state != State.None; }
    public bool IsDetecting () { return state == State.Detecting; }
    public bool IsDragging () { return state == State.Dragging; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 UpdateDrag ( Vector2 _pos ) { 
        Vector2 delta = _pos - dragPoint;
        dragPoint = _pos;
        if ( axisX == false )
            delta.x = 0.0f;
        if ( axisY == false )
            delta.y = 0.0f;
        return delta;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool CheckDrag ( Vector2 _pos ) { 
        dragPoint = _pos; // init/update drag point

        // update checking resule
        Vector2 delta = _pos - anchor;
        if ( axisX == false )
            delta.x = 0.0f;
        if ( axisY == false )
            delta.y = 0.0f;
        if ( delta.sqrMagnitude >= distance*distance ) {
            state = State.Dragging;
            return true;
        }

        //
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool StartDrag ( Vector2 _pos ) {
        state = State.Detecting;
        anchor = _pos; // init the anchor
        if ( distance == 0.0f  ) {
            state = State.Dragging;
            return true;
        }
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void StopDrag () {
        state = State.None;
    }
}
