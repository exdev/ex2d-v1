// ======================================================================================
// File         : exSpriteBorderEditor.cs
// Author       : Wu Jie 
// Last Change  : 09/21/2011 | 08:44:41 AM | Wednesday,September
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

[CustomEditor(typeof(exSpriteBorder))]
class exSpriteBorderEditor : exSpriteBaseEditor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exSpriteBorder editSpriteBorder;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void UpdateAtlas ( exSpriteBorder _sprite, exAtlasDB.ElementInfo _elInfo ) {
        // get atlas and index from textureGUID
        if ( _elInfo != null ) {
            if ( _elInfo.guidAtlas != exEditorHelper.AssetToGUID(_sprite.atlas) ||
                 _elInfo.indexInAtlas != _sprite.index )
            {
                _sprite.SetBorder( _sprite.guiBorder,
                                   exEditorHelper.LoadAssetFromGUID<exAtlas>(_elInfo.guidAtlas), 
                                   _elInfo.indexInAtlas );
            }
        }
        else {
            _sprite.Clear();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != editSpriteBorder ) {
            editSpriteBorder = target as exSpriteBorder;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {

        // ======================================================== 
        // Base GUI 
        // ======================================================== 

        base.OnInspectorGUI();
        GUILayout.Space(20);

        // ======================================================== 
        // init values
        // ======================================================== 

        // 
        bool needRebuild = false;

        // ======================================================== 
        // Texture preview (input)
        // ======================================================== 

        bool borderChanged = false;
        EditorGUI.indentLevel = 1;

        GUILayout.BeginHorizontal();
        GUI.enabled = !inAnimMode;
            exGUIBorder newGUIBorder = (exGUIBorder)EditorGUILayout.ObjectField( "GUI Border"
                                                                                 , editSpriteBorder.guiBorder
                                                                                 , typeof(exGUIBorder)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                                 , false
#endif
                                                                               );
            if ( newGUIBorder != editSpriteBorder.guiBorder ) {
                borderChanged = true;
            }

            if ( GUILayout.Button("Edit...", GUILayout.Width(40), GUILayout.Height(15) ) ) {
                Selection.activeObject = newGUIBorder;
            }
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        // get ElementInfo first
        Texture2D editTexture = null;
        if ( newGUIBorder )
            editTexture = exEditorHelper.LoadAssetFromGUID<Texture2D>(newGUIBorder.textureGUID); 

        // ======================================================== 
        // get atlas element info from atlas database 
        // ======================================================== 

        exAtlas editAtlas = null; 
        int editIndex = -1; 
        exAtlasDB.ElementInfo elInfo = null;
        if ( newGUIBorder )
            elInfo = exAtlasDB.GetElementInfo(newGUIBorder.textureGUID);

        if ( elInfo != null ) {
            editAtlas = exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
            editIndex = elInfo.indexInAtlas;
        }
        bool useAtlas = editAtlas != null && editIndex != -1; 

        // get atlas and index from textureGUID
        if ( !EditorApplication.isPlaying ) {
            // if we use atlas, check if the atlas,index changes
            if ( useAtlas ) {
                if ( editAtlas != editSpriteBorder.atlas ||
                     editIndex != editSpriteBorder.index )
                {
                    borderChanged = true;
                }
            }

            // check if we are first time assignment
            if ( useAtlas || editTexture != null ) {
                if ( editSpriteBorder.renderer.sharedMaterial == null ) {
                    needRebuild = true;
                }
                else if ( editSpriteBorder.meshFilter.sharedMesh == null ) {
                    bool isPrefab = (EditorUtility.GetPrefabType(target) == PrefabType.Prefab); 
                    if ( isPrefab == false ) {
                        needRebuild = true;
                    }
                }
            }
        }

        // set border
        if ( borderChanged ) {
            if ( newGUIBorder == null )
                editSpriteBorder.Clear();
            else
                editSpriteBorder.SetBorder( newGUIBorder, editAtlas, editIndex );
        }

        // ======================================================== 
        // color
        // ======================================================== 

        editSpriteBorder.color = EditorGUILayout.ColorField ( "Color", editSpriteBorder.color );

        // ======================================================== 
        // atlas & index 
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUI.enabled = false;
        EditorGUILayout.ObjectField( "Atlas"
                                     , editSpriteBorder.atlas
                                     , typeof(exAtlas)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                     , false 
#endif
                                   );
        GUI.enabled = true;

        GUI.enabled = !inAnimMode;
        if ( GUILayout.Button("Edit...", GUILayout.Width(40), GUILayout.Height(15) ) ) {
            exAtlasEditor editor = exAtlasEditor.NewWindow();
            editor.Edit(editSpriteBorder.atlas);
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUI.enabled = false;
        EditorGUILayout.IntField( "Index", editSpriteBorder.index );
        GUI.enabled = true;

        // ======================================================== 
        // width & height 
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        // width
        float newWidth = EditorGUILayout.FloatField( "Width", editSpriteBorder.width );
        if ( newWidth != editSpriteBorder.width ) {
            if ( newWidth < 1.0f )
                newWidth = 1.0f;
            editSpriteBorder.width = newWidth;
        }

        // height
        float newHeight = EditorGUILayout.FloatField( "Height", editSpriteBorder.height );
        if ( newHeight != editSpriteBorder.height ) {
            if ( newHeight < 1.0f )
                newHeight = 1.0f;
            editSpriteBorder.height = newHeight;
        }

        // ======================================================== 
        // Rebuild button
        // ======================================================== 

        GUI.enabled = !inAnimMode; 
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if ( GUILayout.Button("Rebuild...", GUILayout.Height(20) ) ) {
            needRebuild = true;
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;
        GUILayout.Space(5);

        // if dirty, build it.
        if ( !EditorApplication.isPlaying && !AnimationUtility.InAnimationMode() ) {
            if ( needRebuild ) {
                editSpriteBorder.Build( editTexture );
            }
            else if ( GUI.changed ) {
                if ( editSpriteBorder.meshFilter.sharedMesh != null )
                    editSpriteBorder.UpdateMesh( editSpriteBorder.meshFilter.sharedMesh );
                EditorUtility.SetDirty(editSpriteBorder);
            }
        }
	}

    // TODO { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void OnSceneGUI () {

    //     //
    //     if ( editSpriteBorder.meshFilter == null || editSpriteBorder.meshFilter.sharedMesh == null ) {
    //         return;
    //     }

    //     //
    //     Vector3[] vertices = editSpriteBorder.meshFilter.sharedMesh.vertices;
    //     if ( vertices.Length > 0 ) {
    //         Transform trans = editSpriteBorder.transform;

    //         Vector3[] w_vertices = new Vector3[5];
    //         w_vertices[0] = trans.localToWorldMatrix * new Vector4 ( vertices[0].x, vertices[0].y, vertices[0].z, 1.0f );
    //         w_vertices[1] = trans.localToWorldMatrix * new Vector4 ( vertices[1].x, vertices[1].y, vertices[1].z, 1.0f ); 
    //         w_vertices[2] = trans.localToWorldMatrix * new Vector4 ( vertices[3].x, vertices[3].y, vertices[3].z, 1.0f ); 
    //         w_vertices[3] = trans.localToWorldMatrix * new Vector4 ( vertices[2].x, vertices[2].y, vertices[2].z, 1.0f ); 
    //         w_vertices[4] = w_vertices[0];

    //         Handles.DrawPolyLine( w_vertices );
    //     }
    // }
    // } TODO end 

    // TODO { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // override protected void AddAnimationHelper () {
    //     editSpriteBorder.gameObject.AddComponent<exSpriteBorderAnimHelper>();
    // }
    // } TODO end 
}
