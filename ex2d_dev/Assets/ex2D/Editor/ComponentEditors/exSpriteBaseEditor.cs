// ======================================================================================
// File         : exSpriteBaseEditor.cs
// Author       : Wu Jie 
// Last Change  : 07/24/2011 | 23:21:51 PM | Sunday,July
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

[CustomEditor(typeof(exSpriteBase))]
public class exSpriteBaseEditor : exPlaneEditor {

    protected enum CollisionType {
        None,
        Boxed,
        Mesh,
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exSpriteBase editSpriteBase;
    protected CollisionType collisionType = CollisionType.None;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != editSpriteBase ) {
            editSpriteBase = target as exSpriteBase;

            // get collision type 
            if ( editSpriteBase.GetComponent<BoxCollider>() != null ) {
                collisionType = CollisionType.Boxed;
            }
            else if ( editSpriteBase.GetComponent<MeshCollider>() != null ) {
                collisionType = CollisionType.Mesh;
            }
            else {
                collisionType = CollisionType.None;
            }

        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {

        // ======================================================== 
        // exPlane GUI 
        // ======================================================== 

        base.OnInspectorGUI();
        GUILayout.Space(20);

        EditorGUIUtility.LookLikeInspector ();
        EditorGUI.indentLevel = 1;

            // ======================================================== 
            // Collision Type 
            // ======================================================== 

            GUI.enabled = !inAnimMode;
            EditorGUIUtility.LookLikeControls ();
            CollisionType newCollisionType = (CollisionType)EditorGUILayout.EnumPopup( "Collision Type", collisionType, GUILayout.Width(200) );
            EditorGUIUtility.LookLikeInspector ();
            GUI.enabled = true;

            //
            if ( newCollisionType != collisionType ) {
                collisionType = newCollisionType;

                Collider myCollider = editSpriteBase.GetComponent<Collider>();
                if ( myCollider != null ) {
                    if ( myCollider is MeshCollider )
                        Object.DestroyImmediate((myCollider as MeshCollider).sharedMesh,true);
                    Object.DestroyImmediate(myCollider,true);
                }

                switch ( collisionType ) {
                case CollisionType.None: break;
                case CollisionType.Boxed: editSpriteBase.gameObject.AddComponent<BoxCollider>(); break;
                case CollisionType.Mesh: editSpriteBase.gameObject.AddComponent<MeshCollider>(); break;
                }
                if ( editSpriteBase.collisionHelper )
                    editSpriteBase.collisionHelper.UpdateCollider();
            }

        GUILayout.BeginHorizontal();

            // ======================================================== 
            // use collision helper
            // ======================================================== 

            GUILayout.Space(15);
            GUI.enabled = !inAnimMode;
            exCollisionHelper compCollisionHelper = editSpriteBase.collisionHelper;
            bool hasCollisionHelperComp = compCollisionHelper != null; 
            bool useCollisionHelper = GUILayout.Toggle ( hasCollisionHelperComp, "Use Collision Helper" ); 
            if ( useCollisionHelper != hasCollisionHelperComp ) {
                if ( useCollisionHelper ) {
                    compCollisionHelper = editSpriteBase.gameObject.AddComponent<exCollisionHelper>();
                    compCollisionHelper.plane = editSpriteBase;
                    compCollisionHelper.UpdateCollider();
                }
                else {
                    Object.DestroyImmediate(compCollisionHelper,true);
                }
                GUI.changed = true;
            }
            GUI.enabled = true;

            // ======================================================== 
            // sync button
            // ======================================================== 

            GUILayout.FlexibleSpace();
            GUI.enabled = (isPrefab == false) && (useCollisionHelper == false);
            if ( GUILayout.Button( "Sync" ) ) {
                editSpriteBase.UpdateColliderSize(0.2f);
            }
            GUI.enabled = true;
        GUILayout.EndHorizontal();

        // ======================================================== 
        // use pixel perfect
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUI.enabled = !inAnimMode;
            exPixelPerfect compPixelPerfect = editSpriteBase.GetComponent<exPixelPerfect>();
            bool hasPixelPerfectComponent = compPixelPerfect != null; 
            bool usePixelPerfect = GUILayout.Toggle ( hasPixelPerfectComponent, "Use Pixel Perfect" ); 
            if ( usePixelPerfect != hasPixelPerfectComponent ) {
                if ( usePixelPerfect )
                    editSpriteBase.gameObject.AddComponent<exPixelPerfect>();
                else
                    Object.DestroyImmediate(compPixelPerfect,true);
                GUI.changed = true;
            }
            GUI.enabled = true;
        GUILayout.EndHorizontal();

        // ======================================================== 
        // scale 
        // ======================================================== 

        EditorGUIUtility.LookLikeControls ();
        editSpriteBase.scale = EditorGUILayout.Vector2Field ( "Scale", editSpriteBase.scale );
        EditorGUIUtility.LookLikeInspector ();

        // ======================================================== 
        // HFlip, VFlip, Reset to Pixel Perfect
        // ======================================================== 

        bool flip = false;
        bool newflip = false;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
            // DISABLE: the current version not allow I do this, just click "Use PixelPerfect" and re-click for the feature { 
            // // reset to pixel perfect
            // if ( GUILayout.Button( "Reset to pixel perfect..." ) ) {
            //     if ( editSprite != null ) {
            //         editSprite.customSize = false;
            //     }
            //     editSpriteBase.MakePixelPerfect ( Camera.main,
            //                                     PlayerSettings.defaultScreenWidth,
            //                                     PlayerSettings.defaultScreenHeight );
            //     GUI.changed = true;
            // }
            // } DISABLE end 
            // h-flip
            flip = Mathf.Sign ( editSpriteBase.scale.x ) < 0.0f;
            newflip = GUILayout.Toggle ( flip, "H-Flip", GUI.skin.button ); 
            if ( newflip != flip ) {
                float s = newflip ? -1.0f : 1.0f;
                editSpriteBase.scale = new Vector2( s * Mathf.Abs(editSpriteBase.scale.x), 
                                                   editSpriteBase.scale.y );
                GUI.changed = true;
            }

            // v-flip
            flip = Mathf.Sign ( editSpriteBase.scale.y ) < 0.0f;
            newflip = GUILayout.Toggle ( flip, "V-Flip", GUI.skin.button ); 
            if ( newflip != flip ) {
                float s = newflip ? -1.0f : 1.0f;
                editSpriteBase.scale = new Vector2( editSpriteBase.scale.x, 
                                                   s * Mathf.Abs(editSpriteBase.scale.y) );
                GUI.changed = true;
            }
        GUILayout.EndHorizontal();

        // ======================================================== 
        // shear 
        // ======================================================== 

        EditorGUIUtility.LookLikeControls ();
        editSpriteBase.shear = EditorGUILayout.Vector2Field ( "Shear", editSpriteBase.shear );
        EditorGUIUtility.LookLikeInspector ();

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(editSpriteBase);
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void AddAnimationHelper () {
        editSpriteBase.gameObject.AddComponent<exSpriteBaseAnimHelper>();
    }
}
