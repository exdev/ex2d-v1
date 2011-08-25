// ======================================================================================
// File         : exUIElement.cs
// Author       : Wu Jie 
// Last Change  : 07/20/2011 | 00:07:45 AM | Wednesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// Interactions Data
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class exUIElement : MonoBehaviour {

    public bool multiTouch = false; // support multi-touch or not.

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public exUIElement parent;
    [System.NonSerialized] public List<exUIElement> children;

    protected exDraggable draggable;
    // protected Droppable droppable;
    // protected Resizable resizable;
    // protected Selectable selectable;
    // protected Sortable sortable;
    // protected Vector3 focusPoint = Vector3.zero; // if multiTouch, the focusPoint will be the center of the touches

    ///////////////////////////////////////////////////////////////////////////////
    // event
    ///////////////////////////////////////////////////////////////////////////////

	public delegate void EventHandler( exUIElement _element );
	public delegate void DragMoveEventHandler( exUIElement _element, Vector2 _point, Vector2 _delta );
	public delegate void DragEventHandler( exUIElement _element, Vector2 _startPoint );

	public event EventHandler OnHoverInEvent;
	public event EventHandler OnHoverOutEvent;
	public event EventHandler OnPressDownEvent;
	public event EventHandler OnPressUpEvent;

	public event DragMoveEventHandler OnDragEvent;
	public event DragEventHandler OnDragStartEvent;
	public event DragEventHandler OnDragStopEvent;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void FindAndAddChild ( exUIElement _el ) {
        foreach ( Transform child in _el.transform ) {
            exUIElement child_el = child.GetComponent<exUIElement>();
            if ( child_el ) {
                _el.children.Add(child_el);
                child_el.parent = _el;
                exUIElement.FindAndAddChild (child_el);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected virtual void Awake () {
        draggable = GetComponent<exDraggable>();
        // droppable = GetComponent<Droppable>();
        // resizable = GetComponent<Resizable>();
        // selectable = GetComponent<Selectable>();
        // Sortable = GetComponent<Sortable>();
    }

    // DELME: confirm and delete if ProcessTouch don't need them { 
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // public bool IsDraggable () { return draggable != null; }
    // public bool IsDroppable () { return droppable != null; }
    // public bool IsResizable () { return resizable != null; }
    // public bool IsSelectable () { return selectable != null; }
    // public bool IsSortable () { return sortable != null; }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // public Draggable DraggableInfo () { return draggable; }
    // public Droppable DroppableInfo () { return droppable; }
    // public Resizable ResizableInfo () { return resizable; }
    // public Selectable SelectableInfo () { return selectable; }
    // public Sortable SortableInfo () { return sortable; }
    // } DELME end 

    ///////////////////////////////////////////////////////////////////////////////
    // touch events
    ///////////////////////////////////////////////////////////////////////////////

    // TODO: 
    // OnEvent () may be a good way. The event can be used.

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void TouchEnter () {
        if ( OnHoverInEvent != null ) 
            OnHoverInEvent ( this );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void TouchOut () {
        if ( OnHoverOutEvent != null ) 
            OnHoverOutEvent ( this );
    }

    ///////////////////////////////////////////////////////////////////////////////
    // mouse events
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void MouseEnter () {
        if ( OnHoverInEvent != null ) 
            OnHoverInEvent ( this );

        // DISABLE { 
        // // it is possible we hover in with mouse button down.
        // if ( Input.GetMouseButton(0) || 
        //      Input.GetMouseButton(1) ||
        //      Input.GetMouseButton(2) )
        // {
        //     MouseButtonDown();
        // }
        // } DISABLE end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void MouseExit () {
        if ( OnHoverOutEvent != null ) 
            OnHoverOutEvent ( this );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void MouseButtonDown () {

        if ( OnPressDownEvent != null ) 
            OnPressDownEvent ( this );

        if ( Input.GetMouseButton(0) )
            StartMouseDrag();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void MouseButtonUp () {

        //
        if ( OnPressUpEvent != null ) 
            OnPressUpEvent ( this );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool UpdateMouseDrag () {
        if ( draggable == null || draggable.IsInDraggState() == false )
            return false;

        if ( Input.GetMouseButton(0) ) {
            // update drag 
            if ( draggable.IsDragging() ) {
                Vector2 deltaPos = draggable.UpdateDrag( Input.mousePosition );
                if ( OnDragEvent != null )
                    OnDragEvent ( this, Input.mousePosition, deltaPos );
                return true;
            }

            // start drag
            if ( draggable.CheckDrag( Input.mousePosition ) ) {
                if ( OnDragStartEvent != null )
                    OnDragStartEvent ( this, Input.mousePosition );
                return true;
            }

            return false;
        }

        if ( Input.GetMouseButtonUp(0) ) {
            return StopMouseDrag();
        }

        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void StartMouseDrag () {
        if ( draggable ) {
            if ( draggable.StartDrag ( Input.mousePosition ) ) {
                if ( OnDragStartEvent != null )
                    OnDragStartEvent ( this, Input.mousePosition );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool StopMouseDrag () {
        //
        if ( draggable.IsDragging() ) {
            draggable.StopDrag();

            if ( OnDragStopEvent != null )
                OnDragStopEvent ( this, Input.mousePosition );

            return true;
        }

        //
        draggable.StopDrag();
        return false;
    }
}
