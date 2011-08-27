// ======================================================================================
// File         : exSpriteEditor.cs
// Author       : Wu Jie 
// Last Change  : 06/04/2011 | 23:47:19 PM | Saturday,June
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

[CustomEditor(typeof(exSprite))]
public class exSpriteEditor : exSpriteBaseEditor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exSprite editSprite;
    private Texture2D editTexture;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static public void UpdateAtlas ( exSprite _sprite, exAtlasDB.ElementInfo _elInfo ) {
        // get atlas and index from textureGUID
        if ( _elInfo != null ) {
            if ( _elInfo.guidAtlas != exEditorRuntimeHelper.AssetToGUID(_sprite.atlas) ||
                 _elInfo.indexInAtlas != _sprite.index )
            {
                _sprite.SetSprite( exEditorRuntimeHelper.LoadAssetFromGUID<exAtlas>(_elInfo.guidAtlas), 
                                   _elInfo.indexInAtlas );
            }
        }
        else {
            if ( _sprite.atlas ) {
                _sprite.Clear();
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != editSprite ) {
            editSprite = target as exSprite;
            editTexture = exEditorRuntimeHelper.LoadAssetFromGUID<Texture2D>(editSprite.textureGUID); 
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
        // 
        // ======================================================== 

        bool needRebuild = false;
        MeshFilter meshFilter = editSprite.GetComponent<MeshFilter>();
        EditorGUIUtility.LookLikeInspector ();
        EditorGUI.indentLevel = 1;

        // check if we can build the mesh 
        if ( editSprite.GetComponent<MeshRenderer>() == null ||
             meshFilter == null ) 
        {
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.red;
            GUILayout.Label( "Can't find MeshRenderer and MeshFilter in edit editSprite", style );
            return;
        }

        // first time build.
        if ( editTexture != null ) {
            if ( editSprite.renderer.sharedMaterial == null ) {
                needRebuild = true;
            }
            else if ( meshFilter.sharedMesh == null ) {
                bool isPrefab = (EditorUtility.GetPrefabType(target) == PrefabType.Prefab); 
                if ( isPrefab == false ) {
                    needRebuild = true;
                }
            }
        }

        // ======================================================== 
        // get texture
        // ======================================================== 

        EditorGUIUtility.LookLikeControls ();
        bool textureChanged = false;
        GUI.enabled = !inAnimMode;
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField( editTexture
                                                                       , typeof(Texture2D)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                       , false
#endif
                                                                       , GUILayout.Width(100)
                                                                       , GUILayout.Height(100) 
                                                                     );
        if ( newTexture != editTexture ) {
            editSprite.textureGUID = newTexture ? exEditorRuntimeHelper.AssetToGUID(newTexture) : "";
            editTexture = newTexture;
            if ( editTexture ) {
                if ( editSprite.customSize == false ) {
                    editSprite.width = editSprite.trimUV.width * editTexture.width;
                    editSprite.height = editSprite.trimUV.height * editTexture.height;
                }
            }
            else {
                editSprite.width = 1;
                editSprite.height = 1;
            }
            textureChanged = true;
        }
        GUILayout.Space(10);
        GUILayout.BeginVertical();
            GUILayout.Space(90);
            GUILayout.Label ( newTexture ? newTexture.name : "None" );
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUI.enabled = true;
        EditorGUIUtility.LookLikeInspector ();

        exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo(editSprite.textureGUID);

        // ======================================================== 
        // get atlas and index from textureGUID
        // ======================================================== 

        if ( !EditorApplication.isPlaying ) {
            if ( elInfo != null ) {

                // ======================================================== 
                // check atlas and index  
                // ======================================================== 

                if ( elInfo.guidAtlas != exEditorRuntimeHelper.AssetToGUID(editSprite.atlas) ||
                     elInfo.indexInAtlas != editSprite.index )
                {
                    editSprite.SetSprite( exEditorRuntimeHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas), 
                                          elInfo.indexInAtlas );
                }
            }
            else {
                if ( editSprite.atlas ) {
                    editSprite.Clear();
                }
            }
        }

        // ======================================================== 
        // color
        // ======================================================== 

        editSprite.color = EditorGUILayout.ColorField ( "Color", editSprite.color );

        // ======================================================== 
        // atlas & index 
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUI.enabled = false;
        EditorGUILayout.ObjectField( "Atlas"
                                     , editSprite.atlas
                                     , typeof(exAtlas)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                     , false 
#endif
                                   );
        GUI.enabled = true;

        GUI.enabled = !inAnimMode;
        if ( GUILayout.Button("Edit...", GUILayout.Width(40), GUILayout.Height(15) ) ) {
            exAtlasEditor editor = exAtlasEditor.NewWindow();
            editor.Edit(editSprite.atlas);
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUI.enabled = false;
        EditorGUILayout.IntField( "Index", editSprite.index );
        GUI.enabled = true;

        // ======================================================== 
        // get trimTexture
        // ======================================================== 

        GUI.enabled = !inAnimMode && (editSprite.atlas == null);
        bool newTrimTexture = EditorGUILayout.Toggle ( "Trim Texture", editSprite.trimTexture );
        if ( editSprite.atlas == null && (textureChanged || newTrimTexture != editSprite.trimTexture) ) {
            editSprite.trimTexture = newTrimTexture; 

            // get trimUV
            Rect trimUV = new Rect( 0, 0, 1, 1 );
            if ( editTexture != null ) {
                
                if ( editSprite.trimTexture ) {
                    exTextureHelper.ImportTextureForAtlas(editTexture);
                    trimUV = exTextureHelper.GetTrimTextureRect(editTexture);
                    trimUV = new Rect( trimUV.x/editTexture.width,
                                       (editTexture.height - trimUV.height - trimUV.y)/editTexture.height,
                                       trimUV.width/editTexture.width,
                                       trimUV.height/editTexture.height );
                }

                if ( editSprite.customSize == false ) {
                    editSprite.width = trimUV.width * editTexture.width;
                    editSprite.height = trimUV.height * editTexture.height;
                }
            }
            editSprite.trimUV = trimUV;
            needRebuild = true;
        }
        GUI.enabled = true;

        // ======================================================== 
        // custom size
        // ======================================================== 

        GUI.enabled = !inAnimMode && !hasPixelPerfectComponent;
        editSprite.customSize = EditorGUILayout.Toggle( "Custom Size", editSprite.customSize );
        GUI.enabled = true;

        // ======================================================== 
        // width & height 
        // ======================================================== 

        EditorGUI.indentLevel = 2;
        GUI.enabled = !inAnimMode && editSprite.customSize;
            // width
            float newWidth = EditorGUILayout.FloatField( "Width", editSprite.width );
            if ( newWidth != editSprite.width ) {
                if ( newWidth < 1.0f )
                    newWidth = 1.0f;
                editSprite.width = newWidth;
            }

            // height
            float newHeight = EditorGUILayout.FloatField( "Height", editSprite.height );
            if ( newHeight != editSprite.height ) {
                if ( newHeight < 1.0f )
                    newHeight = 1.0f;
                editSprite.height = newHeight;
            }
        EditorGUI.indentLevel = 1;

        // ======================================================== 
        // Reset to original
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        if ( GUILayout.Button("Reset to original...", GUILayout.Width(150) ) ) {
            if ( elInfo != null ) {
                exAtlas.Element el = editSprite.GetCurrentElement();
                editSprite.width = el.trimRect.width;
                editSprite.height = el.trimRect.height;
            }
            else if ( newTexture ) {
                editSprite.width = editSprite.trimUV.width * newTexture.width;
                editSprite.height = editSprite.trimUV.height * newTexture.height;
            }
            GUI.changed = true;
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;

        // DISABLE { 
        // // ======================================================== 
        // // Rebuild button
        // // ======================================================== 

        // GUI.enabled = !inAnimMode; 
        // GUILayout.BeginHorizontal();
        // GUILayout.FlexibleSpace();
        // if ( GUILayout.Button("Rebuild...", GUILayout.Width(100), GUILayout.Height(25) ) ) {
        //     needRebuild = true;
        // }
        // GUILayout.Space(5);
        // GUILayout.EndHorizontal();
        // GUI.enabled = true;
        // } DISABLE end 

        // if dirty, build it.
        if ( !EditorApplication.isPlaying && !AnimationUtility.InAnimationMode() ) {
            if ( needRebuild ) {
                editSprite.Build( editTexture );
            }
            else if ( GUI.changed ) {
                if ( meshFilter.sharedMesh != null )
                    editSprite.UpdateMesh( meshFilter.sharedMesh );
                EditorUtility.SetDirty(editSprite);
            }
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSceneGUI () {
        //
        MeshFilter meshFilter = editSprite.GetComponent<MeshFilter>();
        if ( meshFilter == null || meshFilter.sharedMesh == null ) {
            return;
        }

        //
        Vector3[] vertices = meshFilter.sharedMesh.vertices;
        if ( vertices.Length > 0 ) {
            Vector3[] w_vertices = new Vector3[5];
            w_vertices[0] = editSprite.transform.position + vertices[0]; 
            w_vertices[1] = editSprite.transform.position + vertices[1]; 
            w_vertices[2] = editSprite.transform.position + vertices[3]; 
            w_vertices[3] = editSprite.transform.position + vertices[2]; 
            w_vertices[4] = w_vertices[0];
            Handles.DrawPolyLine( w_vertices );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void AddAnimationHelper () {
        editSprite.gameObject.AddComponent<exSpriteAnimHelper>();
    }
}
