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

    public static void UpdateAtlas ( exSprite _sprite, 
                                     exAtlasDB.ElementInfo _elInfo ) {
        // get atlas and index from textureGUID
        if ( _elInfo != null ) {
            if ( _elInfo.guidAtlas != exEditorHelper.AssetToGUID(_sprite.atlas) ||
                 _elInfo.indexInAtlas != _sprite.index )
            {
                _sprite.SetSprite( exEditorHelper.LoadAssetFromGUID<exAtlas>(_elInfo.guidAtlas), 
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

    protected new void OnEnable () {
        base.OnEnable();
        if ( target != editSprite ) {
            editSprite = target as exSprite;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {

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
        editSprite.spanim = editSprite.GetComponent<exSpriteAnimation>();

        // get ElementInfo first
        Texture2D editTexture = exEditorHelper.LoadAssetFromGUID<Texture2D>(editSprite.textureGUID); 

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
                                                                           , false
                                                                           , GUILayout.Width(100)
                                                                           , GUILayout.Height(100) 
                                                                         );
            EditorGUIUtility.LookLikeInspector ();
            if ( newTexture != editTexture ) {
                editTexture = newTexture;
                editSprite.textureGUID = exEditorHelper.AssetToGUID(editTexture);
                textureChanged = true;
                GUI.changed = true;
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
            editAtlas = exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
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
                    GUI.changed = true;
                }
            }
            // if we don't use atlas and current edit target use atlas, clear it.
            else {
                if ( editSprite.useAtlas )
                    editSprite.Clear();
            }

            // check if we are first time assignment
            if ( useAtlas || editTexture != null ) {
                if ( isPrefab == false && editSprite.meshFilter.sharedMesh == null ) {
                    needRebuild = true;
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
            editSprite.renderer.sharedMaterial = exEditorHelper.GetDefaultMaterial(editTexture);
            editSprite.trimTexture = newTrimTexture; 

            // get trimUV
            Rect trimUV = new Rect( 0, 0, 1, 1 );
            if ( editTexture != null ) {
                
                if ( editSprite.trimTexture ) {
                    if ( exTextureHelper.IsValidForAtlas (editTexture) == false )
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
            editSprite.updateFlags |= exPlane.UpdateFlags.UV;
            editSprite.updateFlags |= exPlane.UpdateFlags.Vertex;
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
                                     , false 
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

        GUI.enabled = !inAnimMode;
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
        if ( GUILayout.Button("Reset", GUILayout.Width(50) ) ) {
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
                editSprite.Build( editTexture );
            }
            else if ( GUI.changed ) {
                if ( editSprite.meshFilter.sharedMesh != null )
                    editSprite.UpdateMesh( editSprite.meshFilter.sharedMesh );
                EditorUtility.SetDirty(editSprite);
            }
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSceneGUI () {
        //
        if ( editSprite.meshFilter == null || editSprite.meshFilter.sharedMesh == null ) {
            return;
        }

        //
        Vector3[] vertices = editSprite.meshFilter.sharedMesh.vertices;
        if ( vertices.Length > 0 ) {
            Transform trans = editSprite.transform;

            Vector3[] w_vertices = new Vector3[5];
            w_vertices[0] = trans.localToWorldMatrix * new Vector4 ( vertices[0].x, vertices[0].y, vertices[0].z, 1.0f );
            w_vertices[1] = trans.localToWorldMatrix * new Vector4 ( vertices[1].x, vertices[1].y, vertices[1].z, 1.0f ); 
            w_vertices[2] = trans.localToWorldMatrix * new Vector4 ( vertices[3].x, vertices[3].y, vertices[3].z, 1.0f ); 
            w_vertices[3] = trans.localToWorldMatrix * new Vector4 ( vertices[2].x, vertices[2].y, vertices[2].z, 1.0f ); 
            w_vertices[4] = w_vertices[0];

            Handles.DrawPolyLine( w_vertices );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void AddAnimationHelper () {
        editSprite.gameObject.AddComponent<exSpriteAnimHelper>();
    }
}
