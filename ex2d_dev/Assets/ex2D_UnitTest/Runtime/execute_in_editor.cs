// ======================================================================================
// File         : execute_in_editor.cs
// Author       : Wu Jie 
// Last Change  : 06/09/2012 | 13:54:29 PM | Saturday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// \class 
// 
// \brief 
// 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class execute_in_editor : MonoBehaviour {

    bool inited = false;

	void Awake () { Debug.Log( "execute_in_editor: Awake " + inited); inited = true; }
	void Start () { Debug.Log( "execute_in_editor: Start " + inited); }
	void OnEnable () { Debug.Log( "execute_in_editor: OnEnable " + inited); } 
    void OnDisable () { Debug.Log( "execute_in_editor: OnDisable " + inited); }
    void OnDestroy () { Debug.Log( "execute_in_editor: OnDestroy " + inited); }
	void Update () {}
}
