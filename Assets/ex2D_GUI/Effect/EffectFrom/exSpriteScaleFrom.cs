// ======================================================================================
// File         : exSpriteScaleFrom.cs
// Author       : Wu Jie 
// Last Change  : 08/13/2011 | 09:52:14 AM | Saturday,August
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

[AddComponentMenu("ex2D Helper/EffectFrom/exSprite Scale From")]
public class exSpriteScaleFrom : exEffectOp {

    public Vector2 offset = Vector2.one;
    public bool useAbsoluteValue = false;
    public Vector2 absoluteValue = Vector2.one;
    public exSpriteBase[] sprites;

    private Vector2[] srcList;
    private Vector2[] destList;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	protected override void Awake() {
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
        for ( int i = 0; i < sprites.Length; ++i ) {
            exSpriteBase sp = sprites[i];

            // if we are playing and not use absolute value, recover it first
            if ( enabled == true && useAbsoluteValue == false ) {
                sp.scale = new Vector2( destList[i].x, destList[i].y );
            }

            //
            srcList[i] = useAbsoluteValue ? absoluteValue : (sp.scale + offset);
            destList[i] = sp.scale;
            curve.Start();
        }
        enabled = true;
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
