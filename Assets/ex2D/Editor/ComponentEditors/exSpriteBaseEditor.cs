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

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exSpriteBase editSpriteBase;
    protected bool hasPixelPerfectComponent = false;

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
        // Collider Auto Resize 
        // ======================================================== 

        editSpriteBase.autoResizeCollision = EditorGUILayout.Toggle( "Auto Resize", editSpriteBase.autoResizeCollision );

        // ======================================================== 
        // add mesh collider button
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUI.enabled = !inAnimMode && (editSpriteBase.GetComponent<Collider>() == null);
            if ( GUILayout.Button("Add Mesh Collider", GUILayout.Width(120) ) ) {
                editSpriteBase.AddMeshCollider();
                GUI.changed = true;
            }
            GUI.enabled = true;

        // ======================================================== 
        // add box collider button
        // ======================================================== 

            GUI.enabled = !inAnimMode && (editSpriteBase.GetComponent<Collider>() == null);
            if ( GUILayout.Button("Add Box Collider", GUILayout.Width(120) ) ) {
                editSpriteBase.AddBoxCollider();
                GUI.changed = true;
            }
            GUI.enabled = true;

        GUILayout.EndHorizontal();

        // ======================================================== 
        // add pixel perfect button
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUI.enabled = !inAnimMode;
            exPixelPerfect compPixelPerfect = editSpriteBase.GetComponent<exPixelPerfect>();
            hasPixelPerfectComponent = compPixelPerfect != null; 
            bool usePixelPerfect = GUILayout.Toggle ( hasPixelPerfectComponent, "Use PixelPerfect", GUI.skin.button, GUILayout.Width(120) ); 
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
            //     exPixelPerfect.MakePixelPerfect ( editSpriteBase, 
            //                                     Camera.main,
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
	}
}
