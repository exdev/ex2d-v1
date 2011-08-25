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

public class BtnHover : exUIElement {

    private exScaleTo scaleTo; 
    private exEffectToColor colorTo; 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void Awake () {
        base.Awake();

        OnHoverInEvent += OnHoverIn;
        OnHoverOutEvent += OnHoverOut;
        OnPressDownEvent += OnPressDown;
        OnPressUpEvent += OnPressUp;

        scaleTo = GetComponent<exScaleTo>();
        scaleTo.useAbsoluteValue = true;

        colorTo = GetComponent<exEffectToColor>();
        colorTo.useAbsoluteValue = true;

        renderer.material.color = Color.gray;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnHoverIn ( exUIElement _self ) {
        scaleTo.absoluteValue = new Vector3( 1.0f, 1.0f, 2.0f );
        scaleTo.Play();

        colorTo.absoluteValue = Color.white;
        colorTo.Play();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnHoverOut ( exUIElement _self ) {
        scaleTo.absoluteValue = new Vector3( 1.0f, 1.0f, 1.0f );
        scaleTo.Play();

        colorTo.absoluteValue = Color.gray;
        colorTo.Play();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnPressDown ( exUIElement _self ) {
        scaleTo.absoluteValue = new Vector3( 1.0f, 1.0f, 1.5f );
        scaleTo.Play();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void OnPressUp ( exUIElement _self ) {
        scaleTo.absoluteValue = new Vector3( 1.0f, 1.0f, 2.0f );
        scaleTo.Play();
    }
}
