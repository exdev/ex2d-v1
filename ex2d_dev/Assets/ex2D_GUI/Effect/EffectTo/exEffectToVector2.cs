// ======================================================================================
// File         : exEffectToVector2.cs
// Author       : Wu Jie 
// Last Change  : 07/20/2011 | 15:48:31 PM | Wednesday,July
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

[AddComponentMenu("ex2D Helper/EffectTo/EffectTo - Vector2")]
public class exEffectToVector2 : exEffectOp {

    // TODO: can in exEffectToFloat Editor { 
    public Vector2 offset = Vector2.one;
    public bool useAbsoluteValue = false;
    public Vector2 absoluteValue = Vector2.one;
    // } TODO end 

    private Vector2 src = Vector2.zero;
    private Vector2 dest = Vector2.zero;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 Step () {
        // if time up, we stop it.
        if ( curve.IsTimeUp() ) {
            Stop();
            return dest;
        }

        //
        float v = curve.Step();
        return exEase.Lerp ( src, dest, v );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void PlayFrom ( Vector2 _from ) {
        enabled = true;
        src = _from;
        dest = useAbsoluteValue ? absoluteValue : (src + offset);
        curve.Start();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector2 StopAt () {
        enabled = false;
        return dest;
    }
}

