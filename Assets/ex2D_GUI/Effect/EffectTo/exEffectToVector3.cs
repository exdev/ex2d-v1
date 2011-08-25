// ======================================================================================
// File         : exEffectToVector3.cs
// Author       : Wu Jie 
// Last Change  : 08/13/2011 | 09:54:29 AM | Saturday,August
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

[AddComponentMenu("ex2D Helper/EffectTo/EffectTo - Vector3")]
public class exEffectToVector3 : exEffectOp {

    // TODO: can in exEffectToFloat Editor { 
    public Vector3 offset = Vector3.one;
    public bool useAbsoluteValue = false;
    public Vector3 absoluteValue = Vector3.one;
    // } TODO end 

    private Vector3 src = Vector3.zero;
    private Vector3 dest = Vector3.zero;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 Step () {
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

    public void PlayFrom ( Vector3 _from ) {
        enabled = true;
        src = _from;
        dest = useAbsoluteValue ? absoluteValue : (src + offset);
        curve.Start();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Vector3 StopAt () {
        enabled = false;
        return dest;
    }
}

