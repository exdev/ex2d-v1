// ======================================================================================
// File         : exTileMapRendererEditor.cs
// Author       : Wu Jie 
// Last Change  : 09/16/2011 | 09:59:17 AM | Friday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// exTileMapRendererEditor
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exTileMapRenderer))]
partial class exTileMapRendererEditor : exPlaneEditor {

    private exTileMapRenderer editTileMap;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != editTileMap ) {
            editTileMap = target as exTileMapRenderer;
            editTileMap.editorShowGrid = true;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {

        // ======================================================== 
        // exSprite Base GUI 
        // ======================================================== 

        base.OnInspectorGUI();
        GUILayout.Space(20);

        // ======================================================== 
        // init values
        // ======================================================== 

        bool needRebuild = false;

        // ======================================================== 
        // tileMap
        // ======================================================== 

        exTileMap newTileMap = (exTileMap)EditorGUILayout.ObjectField( "Tile Map"
                                                                       , editTileMap.tileMap
                                                                       , typeof(exTileMap)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                       , false
#endif
                                                                     );
        if ( newTileMap != editTileMap.tileMap ) {
            editTileMap.CreateTileMap(newTileMap);
        }

        // ======================================================== 
        // color
        // ======================================================== 

        editTileMap.color = EditorGUILayout.ColorField ( "Color", editTileMap.color );

        // ======================================================== 
        // Show Grid
        // ======================================================== 

        editTileMap.editorShowGrid = EditorGUILayout.Toggle( "Show Grid", editTileMap.editorShowGrid ); 

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
                EditorUtility.ClearProgressBar();
                editTileMap.Build();
            }
            else if ( GUI.changed ) {
                if ( editTileMap.meshFilter.sharedMesh != null )
                    editTileMap.UpdateMesh( editTileMap.meshFilter.sharedMesh );
                EditorUtility.SetDirty(editTileMap);
            }
        }
    }

}
