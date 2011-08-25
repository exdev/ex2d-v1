// ======================================================================================
// File         : BtnHover.cs
// Author       : Wu Jie 
// Last Change  : 06/03/2011 | 19:16:40 PM | Friday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class Layer : exUIElement {

    public TextMesh text;
    public exUIElement[] buttons;

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void Awake () {
        base.Awake();
        foreach ( exUIElement e in buttons ) {
            e.OnHoverInEvent += ShowHoverIn;
            e.OnHoverOutEvent += ShowHoverOut;
            e.OnDragEvent += ShowDrag;
            e.OnDragStopEvent += ShowDragStop;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ShowHoverIn ( exUIElement _e ) {
        text.text = _e.gameObject.name + "\nHover In";
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ShowHoverOut ( exUIElement _e ) {
        text.text = _e.gameObject.name + "\nHover Out";
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ShowDrag ( exUIElement _e, Vector2 _point, Vector2 _delta ) {
        text.text = _e.gameObject.name + "\nDrag Moving " + _point;
        Vector2 delta = _delta * 0.01f;
        _e.transform.Translate( delta.x, delta.y, 0.0f );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ShowDragStop ( exUIElement _e, Vector2 _point ) {
        text.text = _e.gameObject.name + "\nDrag Stop " + _point;
    }
}
