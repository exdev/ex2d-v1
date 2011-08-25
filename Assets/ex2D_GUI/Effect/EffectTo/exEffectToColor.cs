// ======================================================================================
// File         : exEffectToColor.cs
// Author       : Wu Jie 
// Last Change  : 06/04/2011 | 14:40:11 PM | Saturday,June
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

[AddComponentMenu("ex2D Helper/EffectTo/EffectTo - Color")]
public class exEffectToColor : exEffectOp {

    // TODO: can in exEffectToFloat Editor { 
    public Color offset = Color.white;
    public bool useAbsoluteValue = false;
    public Color absoluteValue = Color.white;
    // } TODO end 

    private Color src = Color.black;
    private Color dest = Color.black;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Color Step () {
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

    public void PlayFrom ( Color _from ) {
        enabled = true;
        src = _from;
        dest = useAbsoluteValue ? absoluteValue : (src + offset);
        curve.Start();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public Color StopAt () {
        enabled = false;
        return dest;
    }
}

