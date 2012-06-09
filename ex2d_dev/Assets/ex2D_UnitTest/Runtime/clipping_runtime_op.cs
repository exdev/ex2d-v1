// ======================================================================================
// File         : clipping_runtime_op.cs
// Author       : Wu Jie 
// Last Change  : 06/09/2012 | 09:27:04 AM | Saturday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// \class 
// 
// \brief 
// 
///////////////////////////////////////////////////////////////////////////////

public class clipping_runtime_op : MonoBehaviour {

    public exClipping clipPlane;
    public List<exPlane> planes = new List<exPlane>();

    public exSpriteFont seconds;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    IEnumerator Start () {
        foreach ( exPlane plane in planes ) {
            clipPlane.AddPlane (plane);
        }

        yield return new WaitForSeconds ( 2.0f ); 

        foreach ( exPlane plane in planes ) {
            clipPlane.RemovePlane (plane);
        }

        yield return new WaitForSeconds ( 2.0f ); 

        foreach ( exPlane plane in planes ) {
            clipPlane.AddPlane (plane);
        }
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        seconds.text = Time.time.ToString("f2");
	}
}
