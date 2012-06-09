// ======================================================================================
// File         : noexecute_in_editor.cs
// Author       : Wu Jie 
// Last Change  : 06/09/2012 | 13:55:53 PM | Saturday,June
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

public class noexecute_in_editor : MonoBehaviour {

	void Awake () { Debug.Log( "noexecute_in_editor: Awake"); }
	void Start () { Debug.Log( "noexecute_in_editor: Start"); }
	void OnEnable () { Debug.Log( "noexecute_in_editor: OnEnable"); } 
    void OnDisable () { Debug.Log( "noexecute_in_editor: OnDisable"); }
    void OnDestroy () { Debug.Log( "noexecute_in_editor: OnDestroy"); }
	void Update () {}
}
