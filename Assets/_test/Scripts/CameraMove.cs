// ======================================================================================
// File         : CameraMove.cs
// Author       : Wu Jie 
// Last Change  : 08/10/2011 | 11:44:46 AM | Wednesday,August
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

public class CameraMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(
                                         transform.position.x,
                                         transform.position.y,
                                         Mathf.Cos( Time.time ) * 100.0f
                                        );
	}
}
