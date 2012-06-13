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
        exUIMng uimng = exUIMng.instance;

        if ( _e.category == exUIEvent.Category.Mouse ) {
            if ( _e.type == exUIEvent.Type.MouseEnter ) {
                OnHoverIn (_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseExit ) {
                if ( uimng.GetMouseFocus() == this ) {
                    isPressing = false;
                    uimng.SetMouseFocus(null);
                }
                OnHoverOut(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseDown &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left ) 
            {
                uimng.SetMouseFocus( this );
                isPressing = true;
                OnPress(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.MouseUp &&
                      _e.buttons == exUIEvent.MouseButtonFlags.Left )
            {
                if ( isPressing ) {
                    uimng.SetMouseFocus( null );
                    isPressing = false;
                    OnClick(_e);
                }
                OnRelease(_e);
                OnHoverOut(_e);
                return true;
            }
        }
        else if ( _e.category == exUIEvent.Category.Touch ) {
            if ( _e.type == exUIEvent.Type.TouchEnter ) {
                OnHoverIn (_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchExit ) {
                if ( uimng.GetTouchFocus(_e.touchID) == this ) {
                    isPressing = false;
                    uimng.SetTouchFocus( _e.touchID, null );
                }
                OnHoverOut(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchDown ) {
                uimng.SetTouchFocus( _e.touchID, this );
                isPressing = true;
                OnPress(_e);
                return true;
            }
            else if ( _e.type == exUIEvent.Type.TouchUp ) {
                if ( isPressing ) {
                    uimng.SetTouchFocus( _e.touchID, null );
                    isPressing = false;
                    OnClick(_e);
                }
                OnRelease(_e);
                OnHoverOut(_e);
                return true;
            }
        }

        //
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnHoverIn ( exUIEvent _e ) {
        // TODO: message info send to the right game object
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnHoverOut ( exUIEvent _e ) {
        // TODO: message info send to the right game object
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnPress ( exUIEvent _e ) {
        // TODO: message info send to the right game object
        Debug.Log("OnPress");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnRelease ( exUIEvent _e ) {
        // TODO: message info send to the right game object
        Debug.Log("OnRelease");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public virtual void OnClick ( exUIEvent _e ) {
        // TODO: message info send to the right game object
    }

}
