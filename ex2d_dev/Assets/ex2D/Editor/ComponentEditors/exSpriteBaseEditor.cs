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
class exSpriteBaseEditor : exPlaneEditor {

    protected enum Physics {
        None,
        Boxed,
        Mesh,
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exSpriteBase editSpriteBase;
    protected bool hasPixelPerfectComponent = false;
    protected Physics physics = Physics.None;

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

            // get physics
            if ( editSpriteBase.GetComponent<BoxCollider>() != null ) {
                physics = Physics.Boxed;
            }
            else if ( editSpriteBase.GetComponent<MeshCollider>() != null ) {
                physics = Physics.Mesh;
            }
            else {
                physics = Physics.None;
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

        GUILayout.BeginHorizontal();
        // ======================================================== 
        // Physics 
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        EditorGUIUtility.LookLikeControls ();
		Physics newPhysics = (Physics)EditorGUILayout.EnumPopup( "Physics", physics, GUILayout.Width(165) );
        EditorGUIUtility.LookLikeInspector ();
        GUI.enabled = true;

        //
        if ( newPhysics != physics ) {
            physics = newPhysics;

            Collider myCollider = editSpriteBase.GetComponent<Collider>();
            if ( myCollider != null ) {
                Object.DestroyImmediate(myCollider);
            }

            switch ( physics ) {
            case Physics.None: break;
            case Physics.Boxed: editSpriteBase.gameObject.AddComponent<BoxCollider>(); break;
            case Physics.Mesh: editSpriteBase.gameObject.AddComponent<MeshCollider>(); break;
            }
        }

        GUILayout.Space(10);

        // ======================================================== 
        // Collider Auto Resize 
        // ======================================================== 

        editSpriteBase.autoResizeCollision = GUILayout.Toggle( editSpriteBase.autoResizeCollision, "Auto Resize", GUILayout.Width(120) );
        GUILayout.EndHorizontal();

        // ======================================================== 
        // use pixel perfect
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUI.enabled = !inAnimMode;
            exPixelPerfect compPixelPerfect = editSpriteBase.GetComponent<exPixelPerfect>();
            hasPixelPerfectComponent = compPixelPerfect != null; 
            bool usePixelPerfect = GUILayout.Toggle ( hasPixelPerfectComponent, "Use Pixel Perfect" ); 
            if ( usePixelPerfect != hasPixelPerfectComponent ) {
                if ( usePixelPerfect )
                    editSpriteBase.gameObject.AddComponent<exPixelPerfect>();
                else {
                    Object.DestroyImmediate(compPixelPerfect);
                }
                GUI.changed = true;
            }
            GUI.enabled = true;
        GUILayout.EndHorizontal();

        // ======================================================== 
        // scale 
        // ======================================================== 

        GUI.enabled = !hasPixelPerfectComponent;
        EditorGUIUtility.LookLikeControls ();
        editSpriteBase.scale = EditorGUILayout.Vector2Field ( "Scale", editSpriteBase.scale );
        EditorGUIUtility.LookLikeInspector ();
        GUI.enabled = true;

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
