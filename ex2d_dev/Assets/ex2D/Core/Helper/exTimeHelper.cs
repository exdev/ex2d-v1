// ======================================================================================
// File         : exTimeHelper.cs
// Author       : Wu Jie 
// Last Change  : 06/16/2011 | 10:09:06 AM | Thursday,June
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

public static class exTimeHelper {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static int GetMinutes ( float _seconds ) {
        return Mathf.FloorToInt(_seconds / 60.0f);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static string ToString_Minutes ( float _seconds ) {
        int min = Mathf.FloorToInt(_seconds / 60.0f);
        int sec = Mathf.FloorToInt(_seconds % 60.0f);
        return min + ":" + sec.ToString("d2");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static string ToString_Seconds ( float _seconds ) {
        int sec1 = Mathf.FloorToInt(_seconds);
        int sec2 = Mathf.FloorToInt((_seconds - sec1) * 60.0f % 60.0f);
        return sec1 + ":" + sec2.ToString("d2");
    }
}
