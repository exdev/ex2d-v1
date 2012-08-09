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
    public ScrollDirection scrollDirection = ScrollDirection.Both;

    public float deceleration = 0.98f;


    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    protected bool isPressing = false;
    protected bool isDragging = false;
    protected Vector2 pressPoint = Vector2.zero;
    protected float pressTime = 0.0f;
    protected Vector2 velocity = Vector2.zero;
    protected bool flicking = false;

    protected Vector2 contentOffset = Vector2.zero;
    protected float contentWidth = 0.0f; 
    protected float contentHeight = 0.0f; 

    // TODO { 
    // protected Vector2 startPos;
    // protected Vector2 destPos;
    // protected float duration;
    // protected StateUpdate stateUpdate;
    // } TODO end 

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void Start () {
        UpdateLayout();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void Update () {
        if ( flicking ) {
            Vector2 delta = velocity * Time.deltaTime;
            velocity *= deceleration;
            contentOffset += delta;

            SetOffset ( contentOffset );

            if ( Mathf.Abs(velocity.magnitude) <= 0.015f ) {
                flicking = false;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        exUIMng uimng = exUIMng.instance;

        // ======================================================== 
        if ( _e.category == exUIEvent.Category.Mouse ) {
        // ======================================================== 

            if ( _e.type == exUIEvent.Type.MouseUp ) {
                if ( uimng.GetMouseFocus() == this ) {
                    isPressing = false;
                    uimng.SetMouseFocus(null);
                    if ( isDragging ) {
                        isDragging = false;

                        if ( Time.time - pressTime < 0.01f )
                            velocity = Vector2.zero;
                        else
                            velocity = (pressPoint - _e.position)/(Time.time - pressTime);

                        if ( scrollDirection == ScrollDirection.Vertical )
                            velocity.x = 0.0f;
                        else if ( scrollDirection == ScrollDirection.Horizontal )
                            velocity.y = 0.0f;

                        if ( Mathf.Abs(velocity.magnitude) > 0.015f ) {
                            flicking = true;
                        }
                    }
                }
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseDown &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left ) 
            {
                uimng.SetMouseFocus( this );
                isPressing = true;
                velocity = Vector2.zero;
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseMove &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left ) 
            {
                if ( _e.delta.magnitude > 1.0f ) {
                    if ( isDragging == false ) {
                        pressPoint = _e.position;
                        pressTime = Time.time;
                        isDragging = true;
                    }
                }
                else {
                    pressPoint = _e.position;
                    isDragging = false;
                }

                float maxOffsetX = Mathf.Max(contentWidth - width, 0.0f);
                float maxOffsetY = Mathf.Max(contentHeight - height, 0.0f);
                float newX = -_e.delta.x;
                float newY = -_e.delta.y;
                Vector2 scrollDistance = Vector2.zero;

                //
                if ( scrollDirection == ScrollDirection.Vertical )
                    newX = 0.0f;
                else if ( scrollDirection == ScrollDirection.Horizontal )
                    newY = 0.0f;

                scrollDistance = new Vector2( newX, newY );
                contentOffset += scrollDistance;
                contentOffset = new Vector2 ( Mathf.Clamp ( contentOffset.x, 0.0f, maxOffsetX ),
                                              Mathf.Clamp ( contentOffset.y, -maxOffsetY, 0.0f ) );

                SetOffset ( contentOffset );

                return true;
            }
        }

        // ======================================================== 
        else if ( _e.category == exUIEvent.Category.Touch ) {
        // ======================================================== 

            if ( _e.type == exUIEvent.Type.TouchUp ) {
                if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                    isPressing = false;
                    uimng.SetTouchFocus( _e.touchID, null );
                }
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchDown ) {
                uimng.SetTouchFocus( _e.touchID, this );
                isPressing = true;
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchMove ) {
                if ( _e.delta.magnitude > 1.0f ) {
                    if ( isDragging == false ) {
                        pressPoint = _e.position;
                        isDragging = true;
                    }
                }
                else {
                    pressPoint = _e.position;
                    isDragging = false;
                }

                float maxOffsetX = Mathf.Max(contentWidth - width, 0.0f);
                float maxOffsetY = Mathf.Max(contentHeight - height, 0.0f);
                float newX = -_e.delta.x;
                float newY = -_e.delta.y;
                Vector2 scrollDistance = Vector2.zero;

                //
                if ( scrollDirection == ScrollDirection.Vertical )
                    newX = 0.0f;
                else if ( scrollDirection == ScrollDirection.Horizontal )
                    newY = 0.0f;

                scrollDistance = new Vector2( newX, newY );
                contentOffset += scrollDistance;
                contentOffset = new Vector2 ( Mathf.Clamp ( contentOffset.x, 0.0f, maxOffsetX ),
                                              Mathf.Clamp ( contentOffset.y, -maxOffsetY, 0.0f ) );

                SetOffset ( contentOffset );

                return true;
            }
        }

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

        // TEST { 
        // UpdateLayout();
        // SetOffset ( Vector2.zero );
        // verticalSlider.height = height_/contentHeight * height_;
        // } TEST end 
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
}
