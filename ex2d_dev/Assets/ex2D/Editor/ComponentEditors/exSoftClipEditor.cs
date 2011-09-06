// ======================================================================================
// File         : exSoftClipEditor.cs
// Author       : Wu Jie 
// Last Change  : 08/25/2011 | 15:18:50 PM | Thursday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exSoftClip))]
class exSoftClipEditor : exPlaneEditor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exSoftClip curEdit;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != curEdit ) {
            curEdit = target as exSoftClip;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        base.OnInspectorGUI();
        GUILayout.Space(20);

        EditorGUIUtility.LookLikeInspector ();
        EditorGUI.indentLevel = 1;

        // DELME { 
        // // ======================================================== 
        // // center
        // // ======================================================== 

        // EditorGUIUtility.LookLikeControls ();
        // GUI.enabled = !inAnimMode;
        // curEdit.center = EditorGUILayout.Vector2Field( "Center", curEdit.center );
        // GUI.enabled = true;
        // EditorGUIUtility.LookLikeInspector ();
        // } DELME end 

        // ======================================================== 
        // width
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        curEdit.width = EditorGUILayout.FloatField( "Width", curEdit.width );
        GUI.enabled = true;

        // ======================================================== 
        // height
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        curEdit.height = EditorGUILayout.FloatField( "Height", curEdit.height );
        GUI.enabled = true;

        // ======================================================== 
        // clip objects 
        // ======================================================== 

        // label
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUILayout.Label( "Clip List" );
        GUILayout.EndHorizontal();

        // list
        EditorGUI.indentLevel = 2;
        GUI.enabled = false;
        for ( int i = 0; i < curEdit.planes.Count; ++i ) {
            exPlane p = curEdit.planes[i];
            if ( p == null ) {
                curEdit.planes.RemoveAt(i);
                --i;
            }
            else {
                EditorGUILayout.ObjectField ( p.name, p, typeof(exPlane), true );
            }
        }
        GUI.enabled = true;
        EditorGUI.indentLevel = 1;

        // update button
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
            if ( GUILayout.Button("Update", GUILayout.Width(50), GUILayout.Height(20) ) ) {
                curEdit.UpdateClipList();
                GUI.changed = true;
            }
        GUILayout.EndHorizontal();

        // ======================================================== 
        // if changes
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    // void OnSceneGUI () {
    //     //
    //     Vector3 center = curEdit.transform.position;
    //     Vector3[] vertices = new Vector3[5];
    //     float halfWidth = curEdit.size.x * 0.5f;
    //     float halfHeight = curEdit.size.y * 0.5f;

    //     //
    //     switch ( curEdit.plane ) {
    //     case exPlane.Plane.XY:
    //         center += new Vector3( curEdit.center.x, curEdit.center.y, 0.0f );
    //         vertices[0] = center + new Vector3 ( -halfWidth, -halfHeight, 0.0f ); 
    //         vertices[1] = center + new Vector3 (  halfWidth, -halfHeight, 0.0f );
    //         vertices[2] = center + new Vector3 (  halfWidth,  halfHeight, 0.0f );
    //         vertices[3] = center + new Vector3 ( -halfWidth,  halfHeight, 0.0f );
    //         vertices[4] = vertices[0];
    //         break;
    //     case exPlane.Plane.XZ:
    //         center += new Vector3( curEdit.center.x, 0.0f, curEdit.center.y );
    //         vertices[0] = center + new Vector3 ( -halfWidth, 0.0f, -halfHeight ); 
    //         vertices[1] = center + new Vector3 (  halfWidth, 0.0f, -halfHeight );
    //         vertices[2] = center + new Vector3 (  halfWidth, 0.0f,  halfHeight );
    //         vertices[3] = center + new Vector3 ( -halfWidth, 0.0f,  halfHeight );
    //         vertices[4] = vertices[0];
    //         break;
    //     case exPlane.Plane.ZY:
    //         center += new Vector3( 0.0f, curEdit.center.y, curEdit.center.x );
    //         vertices[0] = center + new Vector3 ( 0.0f, -halfHeight, -halfWidth ); 
    //         vertices[1] = center + new Vector3 ( 0.0f, -halfHeight,  halfWidth );
    //         vertices[2] = center + new Vector3 ( 0.0f,  halfHeight,  halfWidth );
    //         vertices[3] = center + new Vector3 ( 0.0f,  halfHeight, -halfWidth );
    //         vertices[4] = vertices[0];
    //         break;
    //     }

    //     Handles.color = Color.yellow;
    //     Handles.DrawPolyLine( vertices );
    // }
}
