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

    static string[] anchorTexts = new string[] {
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
    protected bool isPrefab = false;
    protected Transform2D trans2d = Transform2D.None;
    protected GUIStyle labelStyle = new GUIStyle();

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
                // EditorUtility.SetSelectedWireframeHidden(editPlane.renderer, false); // DEBUG
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

#if UNITY_3_4
        isPrefab = (EditorUtility.GetPrefabType(target) == PrefabType.Prefab); 
#else
        isPrefab = (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab); 
#endif

        // TEMP: not sure this is good { 
        Event e = Event.current;
        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            if ( isPrefab )
                Undo.RegisterUndo(editPlane, "editPlane");
            else
                Undo.RegisterSceneUndo("ex2D.Scene");
        }
        // } TEMP end 

        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        if ( isPrefab && editPlane.meshFilter && editPlane.meshFilter.sharedMesh ) {
            editPlane.meshFilter.sharedMesh = null;
        }

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
                Object.DestroyImmediate(screenPos,true);
            }
            exViewportPosition vpPos = editPlane.GetComponent<exViewportPosition>();
            if ( vpPos != null ) {
                Object.DestroyImmediate(vpPos,true);
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
                    Object.DestroyImmediate(compAnimHelper,true);
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
        if ( isPrefab ) {
            GUILayout.BeginHorizontal();
#if UNITY_3_4
                bool isPrefabCamera = (EditorUtility.GetPrefabType(editPlane.renderCamera) == PrefabType.Prefab);
#else
                bool isPrefabCamera = (PrefabUtility.GetPrefabType(editPlane.renderCamera) == PrefabType.Prefab);
#endif
                editPlane.renderCamera = (Camera)EditorGUILayout.ObjectField( "Camera"
                                                                              , isPrefabCamera ? editPlane.renderCamera : null 
                                                                              , typeof(Camera) 
                                                                              , false 
                                                                              , GUILayout.Width(250) );
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.normal.textColor = Color.yellow;
                GUILayout.Label( "(Prefab Only)", labelStyle );
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        else {
            editPlane.renderCamera = (Camera)EditorGUILayout.ObjectField( "Camera"
                                                                          , editPlane.renderCamera 
                                                                          , typeof(Camera) 
                                                                          , true 
                                                                          , GUILayout.Width(250) );
        }
        EditorGUIUtility.LookLikeInspector ();

        // DELME { 
        // // ======================================================== 
        // // plane type
        // // ======================================================== 

        // GUI.enabled = !inAnimMode;
        // EditorGUIUtility.LookLikeControls ();
        // editPlane.plane = (exPlane.Plane)EditorGUILayout.EnumPopup( "Plane", editPlane.plane, GUILayout.Width(165) );
        // EditorGUIUtility.LookLikeInspector ();
        // } DELME end 

        // ======================================================== 
        // anchor
        // ======================================================== 

        EditorGUILayout.LabelField ( "Anchor", "" );
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
            editPlane.anchor 
                = (exPlane.Anchor)GUILayout.SelectionGrid ( (int)editPlane.anchor, 
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

