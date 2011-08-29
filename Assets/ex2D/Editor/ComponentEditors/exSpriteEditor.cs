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

        // 
        bool needRebuild = false;
        MeshFilter meshFilter = editSprite.GetComponent<MeshFilter>();

        // get ElementInfo first
        Texture2D editTexture = exEditorRuntimeHelper.LoadAssetFromGUID<Texture2D>(editSprite.textureGUID); 

        // ======================================================== 
        // Texture preview (input)
        // ======================================================== 

        bool textureChanged = false;
        EditorGUI.indentLevel = 1;

        GUI.enabled = !inAnimMode;
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
            EditorGUIUtility.LookLikeControls ();
            Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField( editTexture
                                                                           , typeof(Texture2D)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                           , false
#endif
                                                                           , GUILayout.Width(100)
                                                                           , GUILayout.Height(100) 
                                                                         );
            EditorGUIUtility.LookLikeInspector ();
            if ( newTexture != editTexture ) {
                editTexture = newTexture;
                editSprite.textureGUID = exEditorRuntimeHelper.AssetToGUID(editTexture);
                textureChanged = true;
            }
            GUILayout.Space(10);
            GUILayout.BeginVertical();
                GUILayout.Space(90);
                GUILayout.Label ( editTexture ? editTexture.name : "None" );
            GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUI.enabled = true;

        // ======================================================== 
        // get atlas element info from atlas database 
        // ======================================================== 

        exAtlas editAtlas = null; 
        int editIndex = -1; 
        exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo(editSprite.textureGUID);
        if ( elInfo != null ) {
            editAtlas = exEditorRuntimeHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
            editIndex = elInfo.indexInAtlas;
        }
        bool useAtlas = editAtlas != null && editIndex != -1; 

        // get atlas and index from textureGUID
        if ( !EditorApplication.isPlaying ) {
            // if we use atlas, check if the atlas,index changes
            if ( useAtlas ) {
                if ( editAtlas != editSprite.atlas ||
                     editIndex != editSprite.index )
                {
                    editSprite.SetSprite( editAtlas, editIndex );
                }
            }
            // if we don't use atlas and current edit target use atlas, clear it.
            else {
                if ( editSprite.useAtlas )
                    editSprite.Clear();
            }

            // check if we are first time assignment
            if ( useAtlas || editTexture != null ) {
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
        }

        // ======================================================== 
        // get trimTexture
        // ======================================================== 

        GUI.enabled = !inAnimMode && !useAtlas;
        bool newTrimTexture = EditorGUILayout.Toggle ( "Trim Texture", editSprite.trimTexture );
        if ( !useAtlas && 
             (textureChanged || newTrimTexture != editSprite.trimTexture) ) {
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
            if ( useAtlas ) {
                exAtlas.Element el = editAtlas.elements[editIndex];
                editSprite.width = el.trimRect.width;
                editSprite.height = el.trimRect.height;
            }
            else if ( editTexture ) {
                editSprite.width = editSprite.trimUV.width * editTexture.width;
                editSprite.height = editSprite.trimUV.height * editTexture.height;
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

    void AtlasTextureField ( Rect _rect, exAtlas _atlas, int _index, Texture2D _texture ) {

        exEditorHelper.DrawRect ( _rect, Color.gray, Color.black );


        GUILayoutUtility.GetRect ( _rect.width, _rect.height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void AddAnimationHelper () {
        editSprite.gameObject.AddComponent<exSpriteAnimHelper>();
    }
}
