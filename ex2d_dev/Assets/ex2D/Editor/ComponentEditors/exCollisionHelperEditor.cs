// ======================================================================================
// File         : exCollisionHelperEditor.cs
// Author       : Wu Jie 
// Last Change  : 09/19/2011 | 18:52:28 PM | Monday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exCollisionHelper))]
public class exCollisionHelperEditor : Editor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exCollisionHelper curEdit;
    protected exCollisionHelper.CollisionType collisionType = exCollisionHelper.CollisionType.None;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( target != curEdit ) {
            curEdit = target as exCollisionHelper;

            // get collision type 
            if ( curEdit.GetComponent<BoxCollider>() != null ) {
                collisionType = exCollisionHelper.CollisionType.Boxed;
            }
            else if ( curEdit.GetComponent<MeshCollider>() != null ) {
                collisionType = exCollisionHelper.CollisionType.Mesh;
            }
            else {
                collisionType = exCollisionHelper.CollisionType.None;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        bool inAnimMode = AnimationUtility.InAnimationMode();

        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        curEdit.plane = curEdit.GetComponent<exPlane>();

        // ======================================================== 
        // Collision Type 
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        EditorGUIUtility.LookLikeControls ();
		exCollisionHelper.CollisionType newCollisionType 
            = (exCollisionHelper.CollisionType)EditorGUILayout.EnumPopup( "Collision Type", collisionType, GUILayout.Width(165) );
        EditorGUIUtility.LookLikeInspector ();
        GUI.enabled = true;

        //
        if ( newCollisionType != collisionType ) {
            collisionType = newCollisionType;

            Collider myCollider = curEdit.GetComponent<Collider>();
            if ( myCollider != null ) {
                if ( myCollider is MeshCollider )
                    Object.DestroyImmediate((myCollider as MeshCollider).sharedMesh,true);
                Object.DestroyImmediate(myCollider,true);
            }

            switch ( collisionType ) {
            case exCollisionHelper.CollisionType.None   : break;
            case exCollisionHelper.CollisionType.Boxed  : curEdit.gameObject.AddComponent<BoxCollider>(); break;
            case exCollisionHelper.CollisionType.Mesh   : curEdit.gameObject.AddComponent<MeshCollider>(); break;
            }
            curEdit.UpdateCollider();
        }

        // ======================================================== 
        // Collider Auto Resize 
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            curEdit.autoResizeCollision = GUILayout.Toggle( curEdit.autoResizeCollision, "Auto Resize", GUILayout.Width(120) );
            EditorGUIUtility.LookLikeControls ();
            GUI.enabled = curEdit.autoResizeCollision && !curEdit.autoLength; 
                curEdit.length = EditorGUILayout.FloatField ( "Length", curEdit.length );
            GUI.enabled = true;
            EditorGUIUtility.LookLikeInspector ();
        GUILayout.EndHorizontal();

        // ======================================================== 
        // Collider Auto Length 
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            curEdit.autoLength = GUILayout.Toggle( curEdit.autoLength, "Auto Length", GUILayout.Width(120) );
        GUILayout.EndHorizontal();

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
	}
}
