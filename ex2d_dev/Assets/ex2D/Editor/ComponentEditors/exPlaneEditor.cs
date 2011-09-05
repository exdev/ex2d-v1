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

    protected enum Transform2D {
        None,
        Screen,
        Viewport,
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exPlane editPlane;
    protected bool inAnimMode = false;
    protected Transform2D trans2d = Transform2D.None;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual protected void OnEnable () {
        if ( target != editPlane ) {
            editPlane = target as exPlane;

            if ( editPlane.renderer != null ) {
                EditorUtility.SetSelectedWireframeHidden(editPlane.renderer, true);
            }

            // get trans2d
            if ( editPlane.GetComponent<exScreenPosition>() != null ) {
                trans2d = Transform2D.Screen;
            }
            else if ( editPlane.GetComponent<exViewportPosition>() != null ) {
                trans2d = Transform2D.Viewport;
            }
            else {
                trans2d = Transform2D.None;
            }
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

        editPlane.layer2d = editPlane.GetComponent<exLayer2D>();
        editPlane.meshFilter = editPlane.GetComponent<MeshFilter>();

        // TODO: I do not know how to do it. { 
        // // ======================================================== 
        // // Script 
        // // ======================================================== 

        // MonoScript script = (MonoScript)AssetDatabase.LoadAssetAtPath( AssetDatabase.GetAssetPath (target), typeof(MonoScript) );
        // script = (MonoScript)EditorGUILayout.ObjectField( "Script", script, typeof(MonoScript) );
        // } TODO end 

        // ======================================================== 
        // trans2d 
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        EditorGUIUtility.LookLikeControls ();
		Transform2D newTrans2D = (Transform2D)EditorGUILayout.EnumPopup( "Transform 2D", trans2d, GUILayout.Width(165) );
        EditorGUIUtility.LookLikeInspector ();
        GUI.enabled = true;

        //
        if ( newTrans2D != trans2d ) {
            trans2d = newTrans2D;

            exScreenPosition screenPos = editPlane.GetComponent<exScreenPosition>();
            if ( screenPos != null ) {
                Object.DestroyImmediate(screenPos);
            }
            exViewportPosition vpPos = editPlane.GetComponent<exViewportPosition>();
            if ( vpPos != null ) {
                Object.DestroyImmediate(vpPos);
            }

            switch ( trans2d ) {
            case Transform2D.None: 
                break;

            case Transform2D.Screen:
                editPlane.gameObject.AddComponent<exScreenPosition>(); 
                break;

            case Transform2D.Viewport: 
                editPlane.gameObject.AddComponent<exViewportPosition>(); 
                break;
            }
        }

        // ======================================================== 
        // use layer 2D
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUI.enabled = !inAnimMode;
            bool hasLayer2D = editPlane.layer2d != null; 
            bool useLayer2D = GUILayout.Toggle ( hasLayer2D, "Use Layer 2D" ); 
            if ( useLayer2D != hasLayer2D ) {
                if ( useLayer2D ) {
                    switch ( editPlane.plane ) {
                    case exPlane.Plane.XY: editPlane.layer2d = editPlane.gameObject.AddComponent<exLayerXY>(); break;
                    case exPlane.Plane.XZ: editPlane.layer2d = editPlane.gameObject.AddComponent<exLayerXZ>(); break;
                    case exPlane.Plane.ZY: editPlane.layer2d = editPlane.gameObject.AddComponent<exLayerZY>(); break;
                    }
                    editPlane.layer2d.plane = editPlane;
                    editPlane.layer2d.UpdateDepth();
                }
                else {
                    Object.DestroyImmediate(editPlane.layer2d);
                    editPlane.layer2d = null;
                }
                GUI.changed = true;
            }
            GUI.enabled = true;
        GUILayout.EndHorizontal();

        // ======================================================== 
        // use animation helper 
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            GUI.enabled = !inAnimMode;
            exAnimationHelper compAnimHelper = editPlane.GetComponent<exAnimationHelper>();
            bool hasAnimHelper = compAnimHelper != null; 
            bool useAnimHelper = GUILayout.Toggle ( hasAnimHelper, "Use Animation Helper" ); 
            if ( useAnimHelper != hasAnimHelper ) {
                if ( useAnimHelper )
                    AddAnimationHelper();
                else {
                    Object.DestroyImmediate(compAnimHelper);
                }
                GUI.changed = true;
            }
            GUI.enabled = true;
        GUILayout.EndHorizontal();

        // ======================================================== 
        // camera type 
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        EditorGUIUtility.LookLikeControls ();
        editPlane.renderCamera = (Camera)EditorGUILayout.ObjectField( "Camera"
                                                                      , editPlane.renderCamera 
                                                                      , typeof(Camera) 
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                      , true 
#endif
                                                                      , GUILayout.Width(300) );
        EditorGUIUtility.LookLikeInspector ();

        // ======================================================== 
        // plane type
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        EditorGUIUtility.LookLikeControls ();
        editPlane.plane = (exPlane.Plane)EditorGUILayout.EnumPopup( "Plane", editPlane.plane, GUILayout.Width(165) );
        EditorGUIUtility.LookLikeInspector ();

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
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
                editSprite.useTextureOffset = GUILayout.Toggle ( editSprite.useTextureOffset, "Use Texture Offset" ); 
            GUILayout.EndHorizontal();
        }
        GUI.enabled = true;

        // ======================================================== 
        // offset 
        // ======================================================== 

        EditorGUIUtility.LookLikeControls ();
        editPlane.offset = EditorGUILayout.Vector2Field ( "Offset", editPlane.offset );
        EditorGUIUtility.LookLikeInspector ();

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(editPlane);
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    virtual protected void AddAnimationHelper () {
        editPlane.gameObject.AddComponent<exAnimationHelper>();
    }
}

