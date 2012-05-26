// ======================================================================================
// File         : exSoftClipUtility.cs
// Author       : Wu Jie 
// Last Change  : 09/01/2011 | 23:40:08 PM | Thursday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// exSoftClipUtility
///////////////////////////////////////////////////////////////////////////////

public static class exSoftClipUtility {

#if !(EX2D_EVALUATE)

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/SoftClip Object")]
    static void CreateSoftClipObject () {
        GameObject go = new GameObject("SoftClipObject");
        go.AddComponent<exSoftClip>();
        Selection.activeObject = go;
    }

#endif // !(EX2D_EVALUATE)

}
