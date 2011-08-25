// ======================================================================================
// File         : Order_01.cs
// Author       : Wu Jie 
// Last Change  : 07/27/2011 | 14:37:15 PM | Wednesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

public class Order_01 : Order {

    void Awake () {
        Debug.Log("order 01 awake");
    }

	// Use this for initialization
	void Start () {
        Debug.Log("order 01 start");
	}
	
	// Update is called once per frame
	void Update () {
        // Debug.Log("order 01 update");
	}

    void LateUpdate () {
        // Debug.Log("order 01 late_update");
    }
}
