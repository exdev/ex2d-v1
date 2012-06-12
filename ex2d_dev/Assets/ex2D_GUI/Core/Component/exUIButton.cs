// ======================================================================================
// File         : exUIButton.cs
// Author       : Wu Jie 
// Last Change  : 10/30/2011 | 11:27:13 AM | Sunday,October
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

[AddComponentMenu("ex2D GUI/Button")]
public class exUIButton : exUIElement {

    // delegates
	public delegate void EventHandler ();

    // events
	public event EventHandler OnHoverIn;
	public event EventHandler OnHoverOut;
	public event EventHandler OnButtonPress;
	public event EventHandler OnButtonRelease;

    // TODO { 
    // ///////////////////////////////////////////////////////////////////////////////
    // // serializable
    // ///////////////////////////////////////////////////////////////////////////////

    // public List<MessageInfo> messageInfos = new List<MessageInfo>();
    // } TODO end 


    // ------------------------------------------------------------------ 
    [SerializeField] protected string text_ = "";
    /// the text of the button
    // ------------------------------------------------------------------ 

    public string text {
        get { return text_; }
        set {
            if ( text_ != value ) {
                text_ = value;
                font.text = text_;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    bool isPressing = false;

    public exSpriteBorder border = null;
    public exSpriteFont font = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Sync () {
        base.Sync ();

        if ( border ) {
            border.anchor = anchor;
            border.width = width;
            border.height = height;
            border.transform.localPosition = new Vector3 ( 0.0f, 0.0f, border.transform.localPosition.z );
        }

        if ( font ) {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            font.transform.localPosition 
                = new Vector3( boxCollider.center.x, boxCollider.center.y, font.transform.localPosition.z );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override bool OnEvent ( exUIEvent _e ) {
        switch ( _e.type ) {
        case exUIEvent.Type.PointerEnter: 
            if ( OnHoverIn != null ) OnHoverIn ();
            return true;

        case exUIEvent.Type.PointerExit: 
            if ( exUIMng.focus == this ) {
                isPressing = false;
                exUIMng.focus = null;
            }
            if ( OnHoverOut != null ) OnHoverOut();
            return true;

        case exUIEvent.Type.PointerPress: 
            if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
                 _e.buttons == exUIEvent.PointerButtonFlags.Touch ) {
                isPressing = true;
                exUIMng.focus = this;
                if ( OnButtonPress != null ) OnButtonPress ();
            }
            return true;

        case exUIEvent.Type.PointerRelease: 
            if ( _e.buttons == exUIEvent.PointerButtonFlags.Left ||
                 _e.buttons == exUIEvent.PointerButtonFlags.Touch ) {
                exUIMng.focus = null;
                if ( isPressing ) {
                    isPressing = false;
                    if ( OnButtonRelease != null ) OnButtonRelease ();
                    if ( OnHoverOut != null ) OnHoverOut();
                }
            }
            return true;
        }

        return false;
    }

}
