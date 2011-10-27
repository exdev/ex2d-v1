// ======================================================================================
// File         : exEffectToFloat.cs
// Author       : Wu Jie 
// Last Change  : 06/04/2011 | 12:12:27 PM | Saturday,June
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

[AddComponentMenu("ex2D Helper/EffectTo/EffectTo - Float")]
public class exEffectToFloat : exEffectOp {

    // TODO: can in exEffectToFloat Editor { 
    public float offset = 1.0f;
    public bool useAbsoluteValue = false;
    public float absoluteValue = 1.0f;
    // } TODO end 

    private float src = 0.0f;
    private float dest = 0.0f;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float Step () {
        // if time up, we stop it.
        if ( curve.IsTimeUp() ) {
            Stop();
            enabled = false;
            return dest;
        }

        //
        float v = curve.Step();
        return exEase.Lerp ( src, dest, v );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void PlayFrom ( float _from ) {
        enabled = true;
        src = _from;
        dest = useAbsoluteValue ? absoluteValue : (src + offset);
        curve.Start();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float StopAt () {
        enabled = false;
        return dest;
    }
}
