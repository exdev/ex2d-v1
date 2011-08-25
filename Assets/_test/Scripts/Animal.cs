// ======================================================================================
// File         : Animal.cs
// Author       : Wu Jie 
// Last Change  : 08/01/2011 | 09:54:56 AM | Monday,August
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

public class Animal : MonoBehaviour {

    public exSprite animal;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

    void PlaySpAnim ( string _name ) {
        animal.spanim.Play(_name);
    }
}
