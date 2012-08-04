// ======================================================================================
// File         : exUIScrollView.cs
// Author       : Wu Jie 
// Last Change  : 07/22/2012 | 14:22:52 PM | Sunday,July
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

    public enum ScrollDirection {
        Vertical,
        Horizontal,
        Both
    }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    public ScrollDirection scrollDirection = ScrollDirection.Both;

    // TODO { 
    // public bool bounce = true;
    // public float bounceDuration = 0.5f;
    // public float damping = 0.95f;
    // public float elasticity = 0.2f;
    // } TODO end 

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // public exUIButton btnLeft = null;
    // public exUIButton btnRight = null;
    // public exUIButton btnUp = null;
    // public exUIButton btnDown = null;

    public exClipping clipRect;
    public exSpriteBorder horizontalBar;
    public exSpriteBorder horizontalSlider;
    public exSpriteBorder verticalBar;
    public exSpriteBorder verticalSlider;
    public Transform contentAnchor;

    // TODO { 
    // protected Vector2 contentOffset = Vector2.zero;
    // protected Vector2 startPos;
    // protected Vector2 destPos;
    // protected float duration;
    // protected StateUpdate stateUpdate;
    // protected float pressTime = 0.0f;
    // protected Vector2 pressPoint = Vector2.zero;
    // protected Vector2 velocity = Vector2.zero;
    // protected bool isDragging = false;
    // } TODO end 

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSizeChanged ( float _newWidth, float _newHeight ) {
        base.OnSizeChanged( _newWidth, _newHeight );
        Commit();

        float startX = boundingRect.xMin;
        float startY = boundingRect.yMax;
        float endX = boundingRect.xMax;
        float endY = boundingRect.yMin;

        // resize clip rect
        clipRect.anchor = anchor;
        clipRect.width = _newWidth - style.padding.left - style.padding.right;
        clipRect.height = _newHeight - style.padding.top - style.padding.bottom;
        clipRect.transform.localPosition = new Vector3 ( style.padding.left,
                                                         style.padding.top,
                                                         clipRect.transform.localPosition.z );

        // TODO: horizontalBar, verticalBar
        if ( contentAnchor )
            contentAnchor.transform.localPosition = new Vector3 ( startX, startY, contentAnchor.transform.localPosition.z );

        if ( horizontalBar )
            horizontalBar.transform.localPosition = new Vector3 ( startX, endY, horizontalBar.transform.localPosition.z );

        if ( horizontalSlider )
            horizontalSlider.transform.localPosition = new Vector3 ( startX, endY, horizontalSlider.transform.localPosition.z );

        if ( verticalBar )
            verticalBar.transform.localPosition = new Vector3 ( endX, startY, verticalBar.transform.localPosition.z );

        if ( verticalSlider )
            verticalSlider.transform.localPosition = new Vector3 ( endX, startY, verticalSlider.transform.localPosition.z );

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

    protected float GetContentWidth () {
        float maxWidth = 9999.0f;
        for ( int i = 0; i < children.Count; ++i ) {
            exUIElement el = children[i];
            if ( el.boundingRect.width > maxWidth )
                maxWidth = el.boundingRect.width;
        }
        return maxWidth;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected float GetContentHeight () {
        float height = 0.0f;
        for ( int i = 0; i < children.Count; ++i ) {
            exUIElement el = children[i];
            height += el.boundingRect.height;
        }
        return height;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddElement ( exUIElement _el ) {
        AddChild(_el);
        UpdateLayout();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateLayout () {
        float y = contentAnchor.localPosition.y;

        for ( int i = 0; i < children.Count; ++i ) {
            exUIElement el = children[i];
            Vector3 pos = el.transform.localPosition; 
            pos.x = el.style.margin.left;
            pos.y = y + el.style.margin.top;
            y = pos.y +  el.boundingRect.height;
        }
    }

    // DELME { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // public override void Sync () {
    //     base.Sync();

    //     float startX = boundingRect.xMin;
    //     float startY = boundingRect.yMax;
    //     float endX = boundingRect.xMax;
    //     float endY = boundingRect.yMin;

    //     clipRect.anchor = anchor;
    //     clipRect.width = width;
    //     clipRect.height = height;
    //     clipRect.transform.localPosition = new Vector3 ( 0.0f, 0.0f, clipRect.transform.localPosition.z );

    //     if ( contentAnchor )
    //         contentAnchor.transform.localPosition = new Vector3 ( startX, startY, contentAnchor.transform.localPosition.z );
    //     if ( horizontalBar )
    //         horizontalBar.transform.localPosition = new Vector3 ( startX, endY, horizontalBar.transform.localPosition.z );
    //     if ( horizontalSlider )
    //         horizontalSlider.transform.localPosition = new Vector3 ( startX, endY, horizontalSlider.transform.localPosition.z );
    //     if ( verticalBar )
    //         verticalBar.transform.localPosition = new Vector3 ( endX, startY, verticalBar.transform.localPosition.z );
    //     if ( verticalSlider )
    //         verticalSlider.transform.localPosition = new Vector3 ( endX, startY, verticalSlider.transform.localPosition.z );

    //     // DELME { 
    //     // switch ( plane ) {
    //     // case exPlane.Plane.XY:
    //     //     if ( contentAnchor )
    //     //         contentAnchor.transform.localPosition = new Vector3 ( startX, startY, 0.0f );
    //     //     if ( horizontalBar )
    //     //         horizontalBar.transform.localPosition = new Vector3 ( startX, endY, 0.0f );
    //     //     if ( horizontalSlider )
    //     //         horizontalSlider.transform.localPosition = new Vector3 ( startX, endY, 0.0f );
    //     //     if ( verticalBar )
    //     //         verticalBar.transform.localPosition = new Vector3 ( endX, startY, 0.0f );
    //     //     if ( verticalSlider )
    //     //         verticalSlider.transform.localPosition = new Vector3 ( endX, startY, 0.0f );
    //     //     break;

    //     // case exPlane.Plane.XZ:
    //     //     if ( contentAnchor )
    //     //         contentAnchor.transform.localPosition = new Vector3 ( startX, 0.0f, startY );
    //     //     if ( horizontalBar )
    //     //         horizontalBar.transform.localPosition = new Vector3 ( startX, 0.0f, endY );
    //     //     if ( horizontalSlider )
    //     //         horizontalSlider.transform.localPosition = new Vector3 ( startX, 0.0f, endY );
    //     //     if ( verticalBar )
    //     //         verticalBar.transform.localPosition = new Vector3 ( endX, 0.0f, startY );
    //     //     if ( verticalSlider )
    //     //         verticalSlider.transform.localPosition = new Vector3 ( endX, 0.0f, startY );
    //     //     break;

    //     // case exPlane.Plane.ZY:
    //     //     if ( contentAnchor )
    //     //         contentAnchor.transform.localPosition = new Vector3 ( 0.0f, startY, startX );
    //     //     if ( horizontalBar )
    //     //         horizontalBar.transform.localPosition = new Vector3 ( 0.0f, endY, startX );
    //     //     if ( horizontalSlider )
    //     //         horizontalSlider.transform.localPosition = new Vector3 ( 0.0f, endY, startX );
    //     //     if ( verticalBar )
    //     //         verticalBar.transform.localPosition = new Vector3 ( 0.0f, endX, startY );
    //     //     if ( verticalSlider )
    //     //         verticalSlider.transform.localPosition = new Vector3 ( 0.0f, endX, startY );
    //     //     break;
    //     // }
    //     // } DELME end 

    //     //
    //     float hbarHeight = (horizontalBar && horizontalBar.guiBorder) ? horizontalBar.guiBorder.border.vertical : 0.0f;
    //     float vbarWidth = (verticalBar && verticalBar.guiBorder) ? verticalBar.guiBorder.border.horizontal : 0.0f;
    //     if ( horizontalBar ) {
    //         horizontalBar.width = width - vbarWidth; 
    //         horizontalBar.height = hbarHeight;
    //     }
    //     if ( verticalBar ) {
    //         verticalBar.height = height - hbarHeight;
    //         verticalBar.width = vbarWidth;
    //     }
    // }
    // } DELME end 

    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void SetOffset ( Vector2 _offset ) {
    //     float maxOffsetX = Mathf.Max(contentWidth - width, 0.0f);
    //     float maxOffsetY = Mathf.Max(contentHeight - height, 0.0f);

    //     float startX = boundingRect.xMin;
    //     float startY = boundingRect.yMax;

    //     float vsliderX = verticalSlider.transform.localPosition.x;
    //     float vsliderY = startY + _offset.y * height/contentHeight;
    //     if ( _offset.y < -maxOffsetY ) {
    //         vsliderY = -height + verticalSlider.height;
    //     }
    //     else if ( _offset.y > 0 ) {
    //         vsliderY = 0.0f;
    //     }

    //     float hsliderX = startX + _offset.x * width/contentWidth;
    //     if ( _offset.x < -maxOffsetX ) {
    //         hsliderX = width - horizontalSlider.width;
    //     }
    //     else if ( _offset.y > 0 ) {
    //         hsliderX = 0.0f;
    //     }
    //     float hsliderY = horizontalSlider.transform.localPosition.y;

    //     contentAnchor.transform.localPosition = new Vector3( startX-_offset.x, startY-_offset.y, contentAnchor.transform.localPosition.z );
    //     verticalSlider.transform.localPosition = new Vector3( vsliderX, vsliderY, verticalSlider.transform.localPosition.z );
    //     horizontalSlider.transform.localPosition = new Vector3( hsliderX, hsliderY, horizontalSlider.transform.localPosition.z );

    //     // DELME { 
    //     // switch ( plane ) {
    //     // case exPlane.Plane.XY:
    //     //     contentAnchor.transform.localPosition = new Vector3( startX-_offset.x, startY-_offset.y, 0.0f );
    //     //     verticalSlider.transform.localPosition = new Vector3( vsliderX, vsliderY, 0.0f );
    //     //     horizontalSlider.transform.localPosition = new Vector3( hsliderX, hsliderY, 0.0f );
    //     //     break;

    //     // case exPlane.Plane.XZ:
    //     //     contentAnchor.transform.localPosition = new Vector3( startX-_offset.x, 0.0f, startY-_offset.y );
    //     //     verticalSlider.transform.localPosition = new Vector3( vsliderX, 0.0f, vsliderY );
    //     //     horizontalSlider.transform.localPosition = new Vector3( hsliderX, 0.0f, hsliderY );
    //     //     break;

    //     // case exPlane.Plane.ZY:
    //     //     contentAnchor.transform.localPosition = new Vector3( 0.0f, startY-_offset.y, startX-_offset.x );
    //     //     verticalSlider.transform.localPosition = new Vector3( 0.0f, vsliderY, vsliderX ); 
    //     //     horizontalSlider.transform.localPosition = new Vector3( 0.0f, hsliderY, hsliderX );
    //     //     break;
    //     // }
    //     // } DELME end 
    // }

    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void DeaccelerateScrolling () {
    //     float maxOffsetX = Mathf.Max(contentWidth - boundingRect.width, 0.0f);
    //     float maxOffsetY = Mathf.Max(contentHeight - boundingRect.height, 0.0f);

    //     //
    //     bool needRelocate = false;
    //     if ( bounce ) {
    //         //
    //         float newX = velocity.x;
    //         float newY = velocity.y;
    //         float bounceX = 0.0f;
    //         if ( contentOffset.x > 0.0f ) {
    //             bounceX = contentOffset.x;
    //             newX *= elasticity;
    //             needRelocate = true;
    //         }
    //         else if (  contentOffset.x < maxOffsetX ) {
    //             bounceX = maxOffsetX - contentOffset.x;
    //             newX *= elasticity;
    //             needRelocate = true;
    //         }
    //         else {
    //             newX *= damping;
    //         }
    //         horizontalSlider.width = (width - bounceX)/contentWidth * (width - bounceX);

    //         //
    //         float bounceY = 0.0f;
    //         if ( contentOffset.y > 0.0f ) {
    //             bounceY = contentOffset.y;
    //             newY *= elasticity;
    //             needRelocate = true;
    //         }
    //         else if (  contentOffset.y < -maxOffsetY ) {
    //             bounceY = -maxOffsetY - contentOffset.y;
    //             newY *= elasticity;
    //             needRelocate = true;
    //         }
    //         else {
    //             newY *= damping;
    //         }
    //         verticalSlider.height = (height - bounceY)/contentHeight * (height - bounceY);

    //         //
    //         velocity = new Vector2( newX, newY );
    //         contentOffset += velocity * Time.deltaTime;
    //     }
    //     else {
    //         velocity *= damping;
    //         contentOffset += velocity * Time.deltaTime;
    //         contentOffset = new Vector2 ( Mathf.Clamp ( contentOffset.x, 0.0f, maxOffsetX ),
    //                                Mathf.Clamp ( contentOffset.y, -maxOffsetY, 0.0f ) );
    //     }
    //     SetOffset ( contentOffset );

    //     //
    //     if ( (Mathf.Abs(velocity.x) <= 0.1f && 
    //           Mathf.Abs(velocity.y) <= 0.1f) ) {

    //         if ( needRelocate ) {
    //             duration = 0.0f;
    //             startPos = contentOffset;
    //             destPos = new Vector2 ( Mathf.Clamp ( contentOffset.x, 0.0f, maxOffsetX ),
    //                                     Mathf.Clamp ( contentOffset.y, -maxOffsetY, 0.0f ) );
    //             stateUpdate = RelocateContent;
    //         }
    //         else {
    //             stateUpdate = null;
    //         }
    //     }
    // }

    // // ------------------------------------------------------------------ 
    // // Desc: 
    // public static float ExpoOut ( float _t ) {
    //     return (_t==1.0f) ? 1.0f : 1.001f * (-Mathf.Pow(2.0f, -10 * _t) + 1);
    // }
    // // ------------------------------------------------------------------ 

    // void RelocateContent () {
    //     duration += Time.deltaTime;
    //     float ratio = Mathf.Clamp( duration/bounceDuration, 0.0f, 1.0f );
    //     float maxOffsetX = Mathf.Max(contentWidth - boundingRect.width, 0.0f);
    //     float maxOffsetY = Mathf.Max(contentHeight - boundingRect.height, 0.0f);

    //     contentOffset = new Vector2 ( Mathf.Lerp ( startPos.x, destPos.x, ExpoOut(ratio) ),
    //                                   Mathf.Lerp ( startPos.y, destPos.y, ExpoOut(ratio) ) );

    //     //
    //     float bounceX = 0.0f;
    //     if ( contentOffset.x > 0.0f ) {
    //         bounceX = contentOffset.x;
    //     }
    //     else if (  contentOffset.x < maxOffsetX ) {
    //         bounceX = maxOffsetX - contentOffset.x;
    //     }
    //     horizontalSlider.width = (width - bounceX)/contentWidth * (width - bounceX);

    //     //
    //     float bounceY = 0.0f;
    //     if ( contentOffset.y > 0.0f ) {
    //         bounceY = contentOffset.y;
    //     }
    //     else if (  contentOffset.y < -maxOffsetY ) {
    //         bounceY = -maxOffsetY - contentOffset.y;
    //     }
    //     verticalSlider.height = (height - bounceY)/contentHeight * (height - bounceY);

    //     //
    //     SetOffset ( contentOffset );

    //     if ( duration >= bounceDuration ) {
    //         stateUpdate = null;
    //     }
    // }

    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void LateUpdate () {
    //     if ( stateUpdate != null ) {
    //         stateUpdate ();
    //     }
    // }
}
