// ======================================================================================
// File         : Spawner.cs
// Author       : Wu Jie 
// Last Change  : 07/04/2011 | 14:26:22 PM | Monday,July
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

public class Spawner : MonoBehaviour {

    public GameObject obj;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        for ( int i = 0; i < 50; ++i ) {
            GameObject inst = GameObject.Instantiate( obj,
                                                      new Vector3( Random.Range(-400.0f, 400.0f),
                                                                   Random.Range(-400.0f, 400.0f),
                                                                   obj.transform.position.z ),
                                                      Quaternion.identity ) as GameObject;
            exSpriteAnimation spAnim = inst.GetComponent<exSpriteAnimation>();
            spAnim.GetAnimation("sheep_run").speed = Random.Range( 0.5f, 2.0f );
        }
    }
}
