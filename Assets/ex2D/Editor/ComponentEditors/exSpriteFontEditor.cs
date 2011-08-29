// ======================================================================================
// File         : exSpriteFontEditor.cs
// Author       : Wu Jie 
// Last Change  : 07/17/2011 | 13:50:19 PM | Sunday,July
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

[CustomEditor(typeof(exSpriteFont))]
public class exSpriteFontEditor : exSpriteBaseEditor {

    private static string[] textAlignStrings = new string[] { "Left", "Center", "Right" };

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exSpriteFont editSpriteFont;
    private float textAreaHeight = 0.0f; 

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != editSpriteFont ) {
            editSpriteFont = target as exSpriteFont;
            long lines = exStringHelper.CountLinesInString(editSpriteFont.text);
            textAreaHeight = lines * EditorStyles.textField.lineHeight;
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
        MeshFilter meshFilter = editSpriteFont.GetComponent<MeshFilter>();

        EditorGUIUtility.LookLikeInspector ();
        EditorGUI.indentLevel = 1;

        // ======================================================== 
        // pt size
        // ======================================================== 

        GUI.enabled = false;
        int pt_size = 0;
        if ( editSpriteFont.fontInfo != null )
            pt_size = editSpriteFont.fontInfo.size;
        EditorGUILayout.IntField( "Pt Size", pt_size );
        GUI.enabled = true;

        // ======================================================== 
        // Use multiline 
        // ======================================================== 

        editSpriteFont.useMultiline = EditorGUILayout.Toggle( "Use Multi-Line", editSpriteFont.useMultiline );

        // ======================================================== 
        // text 
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        if ( editSpriteFont.useMultiline == false ) {
            editSpriteFont.text = EditorGUILayout.TextField ( "Text", editSpriteFont.text );
        }
        else {
            EditorGUIUtility.LookLikeControls ();
                EditorGUILayout.LabelField ( "Text", "" );
                GUILayout.BeginHorizontal();
                GUILayout.Space (30);
                    if ( Event.current.Equals ( Event.KeyboardEvent ("^return") ) ||
                         Event.current.Equals ( Event.KeyboardEvent ("%return") ) ) 
                    {
                        GUIUtility.keyboardControl = -1; // remove any keyboard control
                        Repaint();
                    }
                    string newText = EditorGUILayout.TextArea ( editSpriteFont.text, GUILayout.Height(textAreaHeight + 3) );
                    if ( newText != editSpriteFont.text ) {
                        editSpriteFont.text = newText;
                        long lines = exStringHelper.CountLinesInString(editSpriteFont.text);
                        textAreaHeight = lines * EditorStyles.textField.lineHeight;
                    }
                GUILayout.Space (10);
                GUILayout.EndHorizontal();
            EditorGUIUtility.LookLikeInspector ();
        }
        GUI.enabled = true;

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        if ( editSpriteFont.useMultiline ) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            editSpriteFont.textAlign 
                = (exSpriteFont.TextAlign)GUILayout.Toolbar ( (int)editSpriteFont.textAlign, 
                                                              textAlignStrings, 
                                                              GUILayout.Width(150) );  
            GUILayout.EndHorizontal();
        }

        // ======================================================== 
        // font info 
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        GUILayout.BeginHorizontal();
        editSpriteFont.fontInfo = (exBitmapFont)EditorGUILayout.ObjectField( "Font Info"
                                                                           , editSpriteFont.fontInfo
                                                                           , typeof(exBitmapFont)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                           , false 
#endif
                                                                         );
        if ( GUILayout.Button("Edit...", GUILayout.Width(40), GUILayout.Height(15) ) ) {
            exBitmapFontEditor editor = exBitmapFontEditor.NewWindow();
            editor.Edit(editSpriteFont.fontInfo);
        }
        GUILayout.EndHorizontal();
        GUI.enabled = true;

        // check if fontInfo change to null
        if ( editSpriteFont.fontInfo == null ) 
        {
            if ( meshFilter.sharedMesh != null || 
                 editSpriteFont.renderer.sharedMaterial != null )
            {
                needRebuild = true;
            }
        }
        else if ( meshFilter.sharedMesh == null || 
                  editSpriteFont.renderer.sharedMaterial == null ) 
        {
            needRebuild = true;
        }

        // ======================================================== 
        // use kerning
        // ======================================================== 

        bool haveKerning = (editSpriteFont.fontInfo != null) && (editSpriteFont.fontInfo.kernings.Count != 0);
        GUI.enabled = !inAnimMode && haveKerning;
        editSpriteFont.useKerning = EditorGUILayout.Toggle( "Use Kerning", editSpriteFont.useKerning );
        GUI.enabled = true;

        // ======================================================== 
        // tracking 
        // ======================================================== 

        editSpriteFont.tracking = EditorGUILayout.FloatField( "Tracking", editSpriteFont.tracking );

        // ======================================================== 
        // line spacing 
        // ======================================================== 

        editSpriteFont.lineSpacing = EditorGUILayout.FloatField( "Line Spacing", editSpriteFont.lineSpacing );

        ///////////////////////////////////////////////////////////////////////////////
        // normal color option
        ///////////////////////////////////////////////////////////////////////////////

        // ======================================================== 
        // top color
        // ======================================================== 

        editSpriteFont.topColor = EditorGUILayout.ColorField ( "Top Color", editSpriteFont.topColor );

        // ======================================================== 
        // bot color
        // ======================================================== 

        editSpriteFont.botColor = EditorGUILayout.ColorField ( "Bot Color", editSpriteFont.botColor );

        ///////////////////////////////////////////////////////////////////////////////
        // outline option
        ///////////////////////////////////////////////////////////////////////////////

        // ======================================================== 
        // use outline 
        // ======================================================== 

        editSpriteFont.useOutline = EditorGUILayout.Toggle ( "Use Outline", editSpriteFont.useOutline );

        GUI.enabled = editSpriteFont.useOutline;
        EditorGUI.indentLevel = 2;

        // ======================================================== 
        // Outline Width 
        // ======================================================== 

        editSpriteFont.outlineWidth = EditorGUILayout.FloatField ( "Outline Width", editSpriteFont.outlineWidth );

        // ======================================================== 
        // Outline Color 
        // ======================================================== 

        editSpriteFont.outlineColor = EditorGUILayout.ColorField ( "Outline Color", editSpriteFont.outlineColor );

        EditorGUI.indentLevel = 1;
        GUI.enabled = true;

        ///////////////////////////////////////////////////////////////////////////////
        // shadow option
        ///////////////////////////////////////////////////////////////////////////////

        editSpriteFont.useShadow = EditorGUILayout.Toggle ( "Use Shadow", editSpriteFont.useShadow );

        GUI.enabled = editSpriteFont.useShadow;
        EditorGUI.indentLevel = 2;

        // ======================================================== 
        // Shadow Bias 
        // ======================================================== 

        EditorGUILayout.LabelField ( "Shadow Bias", "" );
        EditorGUI.indentLevel = 3;
        float newShadowBiasX = EditorGUILayout.FloatField ( "X", editSpriteFont.shadowBias.x );
        float newShadowBiasY = EditorGUILayout.FloatField ( "Y", editSpriteFont.shadowBias.y );
        if ( newShadowBiasX != editSpriteFont.shadowBias.x ||
             newShadowBiasY != editSpriteFont.shadowBias.y ) 
        {
            editSpriteFont.shadowBias = new Vector2(newShadowBiasX, newShadowBiasY);
        }
        EditorGUI.indentLevel = 2;

        // ======================================================== 
        // Shadow Color 
        // ======================================================== 

        editSpriteFont.shadowColor = EditorGUILayout.ColorField ( "Shadow Color", editSpriteFont.shadowColor );

        EditorGUI.indentLevel = 1;
        GUI.enabled = true;

        // DISABLE { 
        // // ======================================================== 
        // // Rebuild 
        // // ======================================================== 

        // GUI.enabled = !inAnimMode; 
        // GUILayout.BeginHorizontal();
        // GUILayout.FlexibleSpace();
        // if ( GUILayout.Button("Rebuild...", GUILayout.Width(100), GUILayout.Height(25) ) ) {
        //     needRebuild = true;
        // }
        // GUILayout.EndHorizontal();
        // GUI.enabled = true;
        // } DISABLE end 

        // if dirty, build it.
        if ( !EditorApplication.isPlaying && !AnimationUtility.InAnimationMode() ) {
            if ( needRebuild ) {
                // Debug.Log("rebuilding..."); // TODO: a rebuilding label ??
                editSpriteFont.Build();
            }
            else if ( GUI.changed ) {
                if ( meshFilter.sharedMesh != null )
                    editSpriteFont.UpdateMesh( meshFilter.sharedMesh );
                EditorUtility.SetDirty(editSpriteFont);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSceneGUI () {
        //
        MeshFilter meshFilter = editSpriteFont.GetComponent<MeshFilter>();
        if ( meshFilter == null || meshFilter.sharedMesh == null ) {
            return;
        }

        // get the vertex start pos
        int numVerts = editSpriteFont.text.Length * 4;
        int vertexCount = 0;
        if ( editSpriteFont.useShadow ) {
            vertexCount += numVerts;
        }
        if ( editSpriteFont.useOutline ) {
            vertexCount += 8 * numVerts;
        }
        int vertexStartAt = vertexCount;

        //
        Vector3[] vertices = meshFilter.sharedMesh.vertices;
        Vector3[] w_vertices = new Vector3[5];
        if ( vertices.Length > 0 ) {
            for ( int i = 0; i < editSpriteFont.text.Length; ++i ) {
                int vert_id = vertexStartAt + 4 * i;
                w_vertices[0] = editSpriteFont.transform.position + vertices[vert_id+0]; 
                w_vertices[1] = editSpriteFont.transform.position + vertices[vert_id+1]; 
                w_vertices[2] = editSpriteFont.transform.position + vertices[vert_id+3]; 
                w_vertices[3] = editSpriteFont.transform.position + vertices[vert_id+2]; 
                w_vertices[4] = w_vertices[0];
                Handles.DrawPolyLine( w_vertices );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void AddAnimationHelper () {
        editSpriteFont.gameObject.AddComponent<exSpriteFontAnimHelper>();
    }
}


