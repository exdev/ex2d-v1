// ======================================================================================
// File         : exVertexColorTo.cs
// Author       : Wu Jie 
// Last Change  : 06/04/2011 | 22:14:03 PM | Saturday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

[AddComponentMenu("ex2D Helper/EffectTo/VertexColor To")]
public class exVertexColorTo : exEffectToColor {

    private Mesh mesh;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void Awake () {
        base.Awake();
        mesh = GetComponent<MeshFilter>().mesh;
    }
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        Color c = Step();
        Color[] colors = new Color[mesh.colors.Length];
        for ( int i = 0; i < mesh.colors.Length; ++i ) {
            colors[i] = c;
        }
        mesh.colors = colors;
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Play () {
        PlayFrom ( (mesh.colors.Length > 0) ? mesh.colors[0] : Color.white );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public override void Stop () {
        if ( enabled == true ) {
            Color c = StopAt();
            Color[] colors = new Color[mesh.colors.Length];
            for ( int i = 0; i < mesh.colors.Length; ++i ) {
                colors[i] = c;
            }
            mesh.colors = colors;
        }
    }
}

