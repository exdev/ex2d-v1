// ======================================================================================
// File         : exPool.cs
// Author       : Wu Jie 
// Last Change  : 08/30/2011 | 00:44:50 AM | Tuesday,August
// Description  : 
// ======================================================================================

#if !UNITY_FLASH

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// class exPool
// 
// Purpose: 
// 
///////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class exPool<T> where T : MonoBehaviour {

    public int size;
    public GameObject prefab;

    private T[] objects;
    private int idx = 0;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Init () {
        objects = new T[size]; 
        idx = size-1;
        Reset ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Reset () {
        if ( prefab != null ) {
            for ( int i = 0; i < size; ++i ) {
                GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                objects[i] = obj.GetComponent<T>();
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public T Request ( Vector3 _pos, Quaternion _rot )  {
        if ( idx < 0 )
            Debug.LogError ("Error: the pool do not have enough free item.");

        T result = objects[idx];
        --idx; 

        result.transform.position = _pos;
        result.transform.rotation = _rot;
        return result;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Return ( T _obj ) {
        ++idx;
        objects[idx] = _obj;
    }
}

///////////////////////////////////////////////////////////////////////////////
// class exFastPool
// 
// Purpose: 
// 
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

#endif
