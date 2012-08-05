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

    public bool showSliderOnDragging = false;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    protected float contentWidth = 0.0f; 
    protected float contentHeight = 0.0f; 

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

        // horizontalBar, verticalBar
        if ( contentAnchor )
            contentAnchor.transform.localPosition = new Vector3 ( 0.0f, 0.0f, contentAnchor.transform.localPosition.z );

        if ( showSliderOnDragging == false ) {
            if ( horizontalBar )
                horizontalBar.transform.localPosition = new Vector3 ( startX, endY, horizontalBar.transform.localPosition.z );

            if ( verticalBar )
                verticalBar.transform.localPosition = new Vector3 ( endX, startY, verticalBar.transform.localPosition.z );
        }

        if ( horizontalSlider )
            horizontalSlider.transform.localPosition = new Vector3 ( horizontalBar.transform.localPosition.x, 
                                                                     horizontalBar.transform.localPosition.y, 
                                                                     horizontalSlider.transform.localPosition.z );

        if ( verticalSlider )
            verticalSlider.transform.localPosition = new Vector3 ( verticalBar.transform.localPosition.x, 
                                                                   verticalBar.transform.localPosition.y, 
                                                                   verticalSlider.transform.localPosition.z );

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

        // resize clip rect
        clipRect.anchor = Anchor.TopLeft;
        clipRect.width = _newWidth - style.padding.left - style.padding.right - vbarWidth;
        clipRect.height = _newHeight - style.padding.top - style.padding.bottom - hbarHeight;
        clipRect.transform.localPosition = new Vector3 ( -_newWidth * 0.5f + style.padding.left,
                                                         style.padding.top,
                                                         clipRect.transform.localPosition.z );

        // 
        UpdateLayout(); // TODO TEMP
        SetOffset ( Vector2.zero ); // TODO TEMP
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
        float x = 0.0f;

        for ( int i = 0; i < children.Count; ++i ) {
            exUIElement el = children[i];
            Vector3 pos = el.transform.localPosition; 
            pos.x = el.style.margin.left;
            pos.y = y + el.style.margin.top;
            y = pos.y + el.boundingRect.height;
            if ( el.boundingRect.width > x )
                x = el.boundingRect.width; 
        }

        contentHeight = (y == 0.0f) ? 1.0f : y;
        contentWidth = (x == 0.0f) ? 1.0f : x;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void SetOffset ( Vector2 _offset ) {
        float maxOffsetX = Mathf.Max(contentWidth - width, 0.0f);
        float maxOffsetY = Mathf.Max(contentHeight - height, 0.0f);

        float startX = boundingRect.xMin;
        float startY = boundingRect.yMax;

        // vslider
        if ( verticalSlider != null && verticalSlider.enabled ) {
            float vsliderX = verticalSlider.transform.localPosition.x;
            float vsliderY = startY + _offset.y * height/contentHeight;
            if ( _offset.y < -maxOffsetY ) {
                vsliderY = -height + verticalSlider.height;
            }
            else if ( _offset.y > 0 ) {
                vsliderY = 0.0f;
            }
            verticalSlider.transform.localPosition = new Vector3( vsliderX, vsliderY, verticalSlider.transform.localPosition.z );
        }

        // hslider
        if ( horizontalSlider != null && horizontalSlider.enabled ) {
            float hsliderX = startX + _offset.x * width/contentWidth;
            if ( _offset.x < -maxOffsetX ) {
                hsliderX = width - horizontalSlider.width;
            }
            else if ( _offset.y > 0 ) {
                hsliderX = 0.0f;
            }
            float hsliderY = horizontalSlider.transform.localPosition.y;
            horizontalSlider.transform.localPosition = new Vector3( hsliderX, hsliderY, horizontalSlider.transform.localPosition.z );
        }

        contentAnchor.transform.localPosition = new Vector3 ( -_offset.x, -_offset.y, contentAnchor.transform.localPosition.z );
    }

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
