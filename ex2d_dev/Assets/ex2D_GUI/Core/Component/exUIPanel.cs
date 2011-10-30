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

[RequireComponent (typeof(exSpriteBorder))]
public class exUIPanel : exUIElement {

    // delegates
	public delegate void EventHandler ();

    // events
	public event EventHandler OnHoverIn;
	public event EventHandler OnHoverOut;
	public event EventHandler OnButtonPress;
	public event EventHandler OnButtonRelease;

    ///////////////////////////////////////////////////////////////////////////////
    // serialize properites 
    ///////////////////////////////////////////////////////////////////////////////

    protected exSpriteBase backgroundSP = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        backgroundSP = GetComponent<exSpriteBase> ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Reset () {
        backgroundSP = GetComponent<exSpriteBase> ();

        // add box collider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if ( boxCollider == null ) {
            boxCollider = backgroundSP.gameObject.AddComponent<BoxCollider>();
            switch ( backgroundSP.plane ) {
            case exSprite.Plane.XY:
                boxCollider.center = new Vector3( boxCollider.center.x, boxCollider.center.y, 0.2f );
                break;

            case exSprite.Plane.XZ:
                boxCollider.center = new Vector3( boxCollider.center.x, 0.2f, boxCollider.center.z );
                break;

            case exSprite.Plane.ZY:
                boxCollider.center = new Vector3( 0.2f, boxCollider.center.y, boxCollider.center.z );
                break;
            }
        }

        // add collision helper
        if ( backgroundSP.collisionHelper == null ) {
            exCollisionHelper collisionHelper = backgroundSP.gameObject.AddComponent<exCollisionHelper>();
            collisionHelper.plane = backgroundSP;
            collisionHelper.autoLength = false;
            collisionHelper.length = 0.2f;
            collisionHelper.UpdateCollider();
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
            if ( _e.buttons == exUIEvent.MouseButtonFlags.Left ) {
                if ( OnButtonPress != null )
                    OnButtonPress ();
            }
            return true;

        case exUIEvent.Type.PointerRelease: 
            if ( _e.buttons == exUIEvent.MouseButtonFlags.Left ) {
                if ( OnButtonRelease != null )
                    OnButtonRelease ();
            }
            return true;
        }

        return false;
    }
}
