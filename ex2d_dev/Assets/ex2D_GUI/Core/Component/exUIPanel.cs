// ======================================================================================
// File         : exUIPanel.cs
// Author       : Wu Jie 
// Last Change  : 10/30/2011 | 15:43:39 PM | Sunday,October
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

[AddComponentMenu("ex2D GUI/Panel")]
public class exUIPanel : exUIElement {

    // delegates
	public delegate void EventHandler ();

    // events
	public event EventHandler OnHoverIn;
	public event EventHandler OnHoverOut;
	public event EventHandler OnButtonPress;
	public event EventHandler OnButtonRelease;
	public event EventHandler OnPointerMove;

    public exSpriteBorder background = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override public void Sync () {
        base.Sync ();

        if ( background ) {
            background.anchor = anchor;
            background.width = width;
            background.height = height;
            background.transform.localPosition = Vector3.zero;
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
            exUIMng.instance.activeElement = this;
            if ( OnButtonPress != null )
                OnButtonPress ();
            return true;

        case exUIEvent.Type.PointerRelease: 
            exUIMng.instance.activeElement = null;
            if ( OnButtonRelease != null )
                OnButtonRelease ();
            return true;

        case exUIEvent.Type.PointerMove: 
            if ( OnPointerMove != null )
                OnPointerMove ();
            return true;
        }

        return false;
    }
}
