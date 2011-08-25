// ======================================================================================
// File         : exFastPool.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 21:41:16 PM | Saturday,August
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

[System.Serializable]
public class exFastPool {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int count = 0;
    public GameObject prefab = null;

    private GameObject[] goList;
    private int idx = -1;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Init ( bool _active = false ) {
        goList = new GameObject[count]; 
        for ( int i = 0; i < count; ++i ) {
            GameObject obj = (GameObject)GameObject.Instantiate(prefab,Vector3.zero, Quaternion.identity);
            goList[i] = obj;
            if ( _active == false ) {
                obj.SetActiveRecursively(false);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public GameObject Request ( Vector3 _pos, Quaternion _rot ) {
        GameObject result = goList[idx];
        idx = (idx + 1) % count;
        result.transform.position = _pos;
        result.transform.rotation = _rot;
        return result;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Clear () {
        for ( int i = 0; i < count; ++i ) {
            GameObject.Destroy(goList[i]);
            goList[i] = null;
        }
        idx = 0;
        count = 0;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public GameObject[] GameObjects () { return goList; }
}
