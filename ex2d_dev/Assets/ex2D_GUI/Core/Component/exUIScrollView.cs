// ======================================================================================
// File         : exUIScrollView.cs
// Author       : Wu Jie 
// Last Change  : 11/01/2011 | 15:25:52 PM | Tuesday,November
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

[ExecuteInEditMode]
[AddComponentMenu("ex2D GUI/Button")]
public class exUIScrollView : exUIElement {

    // delegates
	public delegate void EventHandler ();
	public delegate void StateUpdate ();

    // events
	public event EventHandler OnHoverIn;
	public event EventHandler OnHoverOut;

    public enum ScrollDirection {
        Vertical,
        Horizontal,
        Both
    }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    public float contentWidth = 100.0f;
    public float contentHeight = 100.0f;
    public bool bounce = true;
    public ScrollDirection scrollDirection = ScrollDirection.Both;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public exSoftClip clipRect;
    public exSpriteBorder horizontalBar;
    public exSpriteBorder horizontalSlider;
    public exSpriteBorder verticalBar;
    public exSpriteBorder verticalSlider;
    public GameObject contentAnchor;

    protected Vector2 offset = Vector2.zero;
    protected Vector2 scrollDistance = Vector2.zero;
    protected bool doDeaccelerate = false;
    protected StateUpdate stateUpdate;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        if ( verticalSlider != null ) {
            if ( height < contentHeight ) {
                verticalSlider.enabled = true;
                verticalSlider.height = height/contentHeight * height;
            }
            else {
                verticalSlider.enabled = false;
            }
        }

        if ( horizontalSlider != null ) {
            if ( width < contentWidth ) {
                horizontalSlider.enabled = true;
                horizontalSlider.width = width/contentWidth * width;
            }
            else {
                horizontalSlider.enabled = false;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override public void Sync () {
        base.Sync();

        float startX = boundingRect.xMin;
        float startY = boundingRect.yMax;
        float endX = boundingRect.xMax;
        float endY = boundingRect.yMin;

        clipRect.anchor = anchor;
        clipRect.width = width;
        clipRect.height = height;
        clipRect.transform.localPosition = Vector3.zero;

        switch ( plane ) {
        case exPlane.Plane.XY:
            if ( contentAnchor )
                contentAnchor.transform.localPosition = new Vector3 ( startX, startY, 0.0f );
            if ( horizontalBar )
                horizontalBar.transform.localPosition = new Vector3 ( startX, endY, 0.0f );
            if ( horizontalSlider )
                horizontalSlider.transform.localPosition = new Vector3 ( startX, endY, 0.0f );
            if ( verticalBar )
                verticalBar.transform.localPosition = new Vector3 ( endX, startY, 0.0f );
            if ( verticalSlider )
                verticalSlider.transform.localPosition = new Vector3 ( endX, startY, 0.0f );
            break;

        case exPlane.Plane.XZ:
            contentAnchor.transform.localPosition = new Vector3 ( startX, 0.0f, startY );
            horizontalBar.transform.localPosition = new Vector3 ( startX, 0.0f, endY );
            horizontalSlider.transform.localPosition = new Vector3 ( startX, 0.0f, endY );
            verticalBar.transform.localPosition = new Vector3 ( endX, 0.0f, startY );
            verticalSlider.transform.localPosition = new Vector3 ( endX, 0.0f, startY );
            break;

        case exPlane.Plane.ZY:
            contentAnchor.transform.localPosition = new Vector3 ( 0.0f, startY, startX );
            horizontalBar.transform.localPosition = new Vector3 ( 0.0f, endY, startX );
            horizontalSlider.transform.localPosition = new Vector3 ( 0.0f, endY, startX );
            verticalBar.transform.localPosition = new Vector3 ( 0.0f, endX, startY );
            verticalSlider.transform.localPosition = new Vector3 ( 0.0f, endX, startY );
            break;
        }

        //
        float hbarHeight = (horizontalBar && horizontalBar.guiBorder) ? horizontalBar.guiBorder.border.vertical : 0.0f;
        float vbarWidth = (verticalBar && verticalBar.guiBorder) ? verticalBar.guiBorder.border.horizontal : 0.0f;
        if ( horizontalBar ) {
            horizontalBar.width = width - vbarWidth; 
            horizontalBar.height = hbarHeight;
        }
        if ( verticalBar ) {
            verticalBar.height = height - hbarHeight;
            verticalBar.width = vbarWidth;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        switch ( _e.type ) {
        case exUIEvent.Type.HoverIn: 
            if ( OnHoverIn != null )
                OnHoverIn ();
            return true;

        case exUIEvent.Type.HoverOut: 
            if ( OnHoverOut != null )
                OnHoverOut ();
            return true;

        case exUIEvent.Type.PointerPress: 
            if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
                 _e.buttons == exUIEvent.PointerButtonFlags.Touch ) 
            {
                exUIMng.instance.activeElement = this;
                scrollDistance = Vector2.zero;
                stateUpdate = null;
            }
            return true;

        case exUIEvent.Type.PointerRelease: 
            if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
                 _e.buttons == exUIEvent.PointerButtonFlags.Touch ) 
            {
                exUIMng.instance.activeElement = null;
                stateUpdate = DeaccelerateScrolling;
                horizontalSlider.width = width/contentWidth * width;
                verticalSlider.height = height/contentHeight * height;
            }
            return true;

        case exUIEvent.Type.PointerMove: 
            if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
                 _e.buttons == exUIEvent.PointerButtonFlags.Touch ) 
            {
                float maxOffsetX = Mathf.Max(contentWidth - width, 0.0f);
                float maxOffsetY = Mathf.Max(contentHeight - height, 0.0f);
                float newX = -_e.delta.x;
                float newY = -_e.delta.y;

                //
                if ( scrollDirection == ScrollDirection.Vertical )
                    newX = 0.0f;
                else if ( scrollDirection == ScrollDirection.Horizontal )
                    newY = 0.0f;

                //
                if ( bounce ) {
                    if ( offset.x > 0.0f )
                        newX *= 0.95f / offset.x; 
                    else if (  offset.x < maxOffsetX ) 
                        newX *= 0.95f / (maxOffsetX - offset.x); 

                    float bounceY = 0.0f;
                    if ( offset.y > 0.0f ) {
                        bounceY = offset.y;
                        newY *= 0.95f / bounceY; 
                    }
                    else if (  offset.y < -maxOffsetY ) {
                        bounceY = -maxOffsetY - offset.y;
                        newY *= 0.95f / bounceY; 
                    }
                    verticalSlider.height = (height - bounceY)/contentHeight * (height - bounceY);

                    scrollDistance = new Vector2( newX, newY );
                    offset += scrollDistance;
                }
                else {
                    scrollDistance = new Vector2( newX, newY );
                    offset += scrollDistance;
                    offset = new Vector2 ( Mathf.Clamp ( offset.x, 0.0f, maxOffsetX ),
                                           Mathf.Clamp ( offset.y, -maxOffsetY, 0.0f ) );
                }
                SetOffset ( offset );
            }
            return true;
        }

        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void SetOffset ( Vector2 _offset ) {
        float maxOffsetX = Mathf.Max(contentWidth - width, 0.0f);
        float maxOffsetY = Mathf.Max(contentHeight - height, 0.0f);

        float startX = boundingRect.xMin;
        float startY = boundingRect.yMax;

        float vsliderX = verticalSlider.transform.localPosition.x;
        float vsliderY = startY + _offset.y * height/contentHeight;
        if ( _offset.y < -maxOffsetY ) {
            vsliderY = -height + verticalSlider.height;
        }
        else if ( _offset.y > 0 ) {
            vsliderY = 0.0f;
        }

        float hsliderX = startX + _offset.x * width/contentWidth;
        if ( _offset.x < -maxOffsetX ) {
            hsliderX = width - horizontalSlider.width;
        }
        else if ( _offset.y > 0 ) {
            hsliderX = 0.0f;
        }
        float hsliderY = horizontalSlider.transform.localPosition.y;

        switch ( plane ) {
        case exPlane.Plane.XY:
            contentAnchor.transform.localPosition = new Vector3( startX-_offset.x, startY-_offset.y, 0.0f );
            verticalSlider.transform.localPosition = new Vector3( vsliderX, vsliderY, 0.0f );
            horizontalSlider.transform.localPosition = new Vector3( hsliderX, hsliderY, 0.0f );
            break;

        case exPlane.Plane.XZ:
            contentAnchor.transform.localPosition = new Vector3( startX-_offset.x, 0.0f, startY-_offset.y );
            verticalSlider.transform.localPosition = new Vector3( vsliderX, 0.0f, vsliderY );
            horizontalSlider.transform.localPosition = new Vector3( hsliderX, 0.0f, hsliderY );
            break;

        case exPlane.Plane.ZY:
            contentAnchor.transform.localPosition = new Vector3( 0.0f, startY-_offset.y, startX-_offset.x );
            verticalSlider.transform.localPosition = new Vector3( 0.0f, vsliderY, vsliderX ); 
            horizontalSlider.transform.localPosition = new Vector3( 0.0f, hsliderY, hsliderX );
            break;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void DeaccelerateScrolling () {
        float maxOffsetX = Mathf.Max(contentWidth - boundingRect.width, 0.0f);
        float maxOffsetY = Mathf.Max(contentHeight - boundingRect.height, 0.0f);
        offset += scrollDistance;
        offset = new Vector2 ( Mathf.Clamp ( offset.x, 0.0f, maxOffsetX ),
                               Mathf.Clamp ( offset.y, -maxOffsetY, 0.0f ) );
        SetOffset ( offset );
        scrollDistance *= 0.95f;

        if ( (Mathf.Abs(scrollDistance.x) <= 0.01f && 
              Mathf.Abs(scrollDistance.y) <= 0.01f) ) {
            stateUpdate = null;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LateUpdate () {
        if ( stateUpdate != null ) {
            stateUpdate ();
        }
    }
}
