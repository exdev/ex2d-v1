// ======================================================================================
// File         : exSpriteScaleTo.cs
// Author       : Wu Jie 
// Last Change  : 07/20/2011 | 15:48:12 PM | Wednesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

[AddComponentMenu("ex2D Helper/EffectTo/exSprite Scale To")]
public class exSpriteScaleTo : exEffectOp {

    public Vector2 offset = Vector2.one;
    public bool useAbsoluteValue = false;
    public Vector2 absoluteValue = Vector2.one;
    public exSpriteBase[] sprites;

    private Vector2[] srcList;
    private Vector2[] destList;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected new void Awake() {
        base.Awake();
        srcList = new Vector2[sprites.Length];
        destList = new Vector2[sprites.Length];
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        // if time up, we stop it.
        if ( curve.IsTimeUp() ) {
            Stop();
            for ( int i = 0; i < sprites.Length; ++i ) {
                sprites[i].scale = new Vector2( destList[i].x, destList[i].y );
            }
        }

        // step
        float v = curve.Step();
        for ( int i = 0; i < sprites.Length; ++i ) {
            Vector2 s = exEase.Lerp ( srcList[i], destList[i], v );
            sprites[i].scale = new Vector2( s.x, s.y );
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Play () {
        enabled = true;
        for ( int i = 0; i < sprites.Length; ++i ) {
            exSpriteBase sp = sprites[i];

            // if we are playing and not use absolute value, recover it first
            if ( enabled == true && useAbsoluteValue == false ) {
                sp.scale = new Vector2( destList[i].x, destList[i].y );
            }

            //
            srcList[i] = sp.scale;
            destList[i] = useAbsoluteValue ? absoluteValue : (sp.scale + offset);
            curve.Start();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Stop () {
        if ( enabled == true ) {
            enabled = false;
            for ( int i = 0; i < sprites.Length; ++i ) {
                exSpriteBase sp = sprites[i];
                sp.scale = new Vector2( destList[i].x, destList[i].y );
            }
        }
    }
}
