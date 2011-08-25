// ======================================================================================
// File         : exPlaneEditor.cs
// Author       : Wu Jie 
// Last Change  : 08/25/2011 | 17:38:28 PM | Thursday,August
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

[CustomEditor(typeof(exPlane))]
public class exPlaneEditor : Editor {

    protected static string[] anchorTexts = new string[] {
        "", "", "", 
        "", "", "", 
        "", "", "", 
    };

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exPlane editPlane;
    protected bool inAnimMode = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual protected void OnEnable () {
        if ( target != editPlane ) {
            editPlane = target as exPlane;
            if ( editPlane.renderer != null )
                EditorUtility.SetSelectedWireframeHidden(editPlane.renderer, true);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {
        exSprite editSprite = target as exSprite;
        inAnimMode = AnimationUtility.InAnimationMode();

        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        // TODO: I do not know how to do it. { 
        // // ======================================================== 
        // // Script 
        // // ======================================================== 

        // MonoScript script = (MonoScript)AssetDatabase.LoadAssetAtPath( AssetDatabase.GetAssetPath (target), typeof(MonoScript) );
        // script = (MonoScript)EditorGUILayout.ObjectField( "Script", script, typeof(MonoScript) );
        // } TODO end 

        // ======================================================== 
        // use screen position
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUI.enabled = !inAnimMode 
                && (editPlane.GetComponent<exViewportPosition>() == null)
                ;
            exScreenPosition compScreenPosition = editPlane.GetComponent<exScreenPosition>();
            bool hasScreenPosition = compScreenPosition != null; 
            bool useScreenPosition = GUILayout.Toggle ( hasScreenPosition, "Use Screen Position", GUI.skin.button, GUILayout.Width(120) ); 
            if ( useScreenPosition != hasScreenPosition ) {
                if ( useScreenPosition )
                    editPlane.gameObject.AddComponent<exScreenPosition>();
                else {
                    Object.DestroyImmediate(compScreenPosition);
                }
                GUI.changed = true;
            }
            GUI.enabled = true;

        // ======================================================== 
        // use viewport position
        // ======================================================== 

            GUI.enabled = !inAnimMode 
                && (editPlane.GetComponent<exScreenPosition>() == null)
                ;
            exViewportPosition compViewportPosition = editPlane.GetComponent<exViewportPosition>();
            bool hasViewportPosition = compViewportPosition != null; 
            bool useViewportPosition = GUILayout.Toggle ( hasViewportPosition, "Use Viewport Position", GUI.skin.button, GUILayout.Width(120) ); 
            if ( useViewportPosition != hasViewportPosition ) {
                if ( useViewportPosition )
                    editPlane.gameObject.AddComponent<exViewportPosition>();
                else {
                    Object.DestroyImmediate(compViewportPosition);
                }
                GUI.changed = true;
            }
            GUI.enabled = true;
        GUILayout.EndHorizontal();

        // ======================================================== 
        // add layer 2d button 
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUI.enabled = !inAnimMode;
            exLayer2D compLayer2D = editPlane.GetComponent<exLayer2D>();
            bool hasLayer2D = compLayer2D != null; 
            bool useLayer2D = GUILayout.Toggle ( hasLayer2D, "Use Layer2D", GUI.skin.button, GUILayout.Width(120) ); 
            if ( useLayer2D != hasLayer2D ) {
                if ( useLayer2D )
                    editPlane.gameObject.AddComponent<exLayer2D>();
                else {
                    Object.DestroyImmediate(compLayer2D);
                }
                GUI.changed = true;
            }
            GUI.enabled = true;
        GUILayout.EndHorizontal();

        // ======================================================== 
        // plane type
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        editPlane.plane = (exPlane.Plane)EditorGUILayout.EnumPopup( "Plane", editPlane.plane );

        // ======================================================== 
        // anchor
        // ======================================================== 

        EditorGUILayout.LabelField ( "Anchor", "" );
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
            editPlane.anchor 
                = (exSpriteBase.Anchor)GUILayout.SelectionGrid ( (int)editPlane.anchor, 
                                                                 anchorTexts, 
                                                                 3, 
                                                                 GUILayout.Width(80) );  
        GUILayout.EndHorizontal();

        // ======================================================== 
        // use texture offset 
        // ======================================================== 

        if ( editSprite != null ) {
            EditorGUI.indentLevel = 2;
            editSprite.useTextureOffset = EditorGUILayout.Toggle ( "Use Texture Offset", 
                                                                   editSprite.useTextureOffset ); 
            EditorGUI.indentLevel = 1;
        }
        GUI.enabled = true;
	}
}

