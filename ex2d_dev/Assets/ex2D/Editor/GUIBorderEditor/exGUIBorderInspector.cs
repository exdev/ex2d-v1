// ======================================================================================
// File         : exGUIBorderInspector.cs
// Author       : Wu Jie 
// Last Change  : 09/20/2011 | 15:14:03 PM | Tuesday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// exGUIBorderInspector
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exGUIBorder))]
class exGUIBorderInspector : Editor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exGUIBorder curEdit;
    static int previewWidth = 200;
    static int previewHeight = 200;
    static bool showGrid = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        if ( target != curEdit ) {
            curEdit = target as exGUIBorder;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {

        GUILayout.Space(10);

        // get ElementInfo first
        Texture2D editTexture = exEditorHelper.LoadAssetFromGUID<Texture2D>(curEdit.textureGUID); 
        Rect lastRect = GUILayoutUtility.GetLastRect ();
        EditorGUI.indentLevel = 1;

        // ======================================================== 
        // GUI border
        // ======================================================== 

        GUI.enabled = false;
        EditorGUILayout.ObjectField( "GUI Border"
                                     , curEdit 
                                     , typeof(exGUIBorder) 
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                     , false
#endif
                                     , GUILayout.Width(200)
                                   );
        GUI.enabled = true;

        // ======================================================== 
        // texture field
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
            Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField( editTexture
                                                                           , typeof(Texture2D)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                                           , false
#endif
                                                                           , GUILayout.Height(50)
                                                                           , GUILayout.Width(50)
                                                                         );
            EditorGUIUtility.LookLikeInspector ();
            if ( newTexture != editTexture ) {
                editTexture = newTexture;
                // DISABLE: some gui are good in bilinear filter (controls/window), 
                //          some are good in point filter (controls/boxOver)
                // editTexture.filterMode = FilterMode.Point;
                curEdit.textureGUID = exEditorHelper.AssetToGUID(editTexture);
                curEdit.border = new RectOffset ( 0, 0, 0, 0 );
            }
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField ( "Name", editTexture ?  editTexture.name : "None", GUILayout.Width(200) );
        EditorGUILayout.LabelField ( "Size", editTexture ?  editTexture.width + " x " + editTexture.height : "0 x 0 ", GUILayout.Width(200) );

        // ======================================================== 
        // texture preview
        // ======================================================== 

        GUILayout.Space(10);
        lastRect = GUILayoutUtility.GetLastRect ();
        TexturePreviewField ( new Rect( 30, lastRect.yMax, 100, 100 ), curEdit, editTexture );
        GUILayout.Space(10);

        // ======================================================== 
        // Rect Offset 
        // ======================================================== 

        GUI.enabled = editTexture != null;
        EditorGUIUtility.LookLikeControls ();
        GUILayout.BeginHorizontal();
            curEdit.border.left = EditorGUILayout.IntField ( "Left", curEdit.border.left, GUILayout.Width(150) ); // left
            if ( editTexture )
                curEdit.border.left = Mathf.Clamp( curEdit.border.left, 0, editTexture.width - curEdit.border.right );

            curEdit.border.right = EditorGUILayout.IntField ( "Right", curEdit.border.right, GUILayout.Width(150) ); // right
            if ( editTexture )
                curEdit.border.right = Mathf.Clamp( curEdit.border.right, 0, editTexture.width - curEdit.border.left );

            GUILayout.Label ( " = " + curEdit.border.horizontal );
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            curEdit.border.top = EditorGUILayout.IntField ( "Top", curEdit.border.top, GUILayout.Width(150) ); // top
            if ( editTexture )
                curEdit.border.top = Mathf.Clamp( curEdit.border.top, 0, editTexture.height - curEdit.border.bottom );

            curEdit.border.bottom = EditorGUILayout.IntField ( "Bottom", curEdit.border.bottom, GUILayout.Width(150) ); // bottom
            if ( editTexture )
                curEdit.border.bottom = Mathf.Clamp( curEdit.border.bottom, 0, editTexture.height - curEdit.border.top );

            GUILayout.Label ( " = " + curEdit.border.vertical );
        GUILayout.EndHorizontal();
        EditorGUIUtility.LookLikeInspector ();
        GUI.enabled = true;

        EditorGUILayout.LabelField ( "Center", editTexture ? 
                                     (editTexture.width - curEdit.border.horizontal) + " x " + (editTexture.height - curEdit.border.vertical)
                                     : "0 x 0", 
                                     GUILayout.Width(200) );

        // ======================================================== 
        // Preview Width and Height 
        // ======================================================== 

        GUILayout.Space(10);
        EditorGUIUtility.LookLikeControls ();
        GUILayout.BeginHorizontal();
            previewWidth = Mathf.Max ( curEdit.border.horizontal, EditorGUILayout.IntField ( "Preview Width", previewWidth, GUILayout.Width(150) ) ); // preview width
            previewHeight = Mathf.Max ( curEdit.border.vertical, EditorGUILayout.IntField ( "Preview Height", previewHeight, GUILayout.Width(150) ) ); // preview height
        GUILayout.EndHorizontal();
        EditorGUIUtility.LookLikeInspector ();

        // ======================================================== 
        // Show Grid 
        // ======================================================== 

        showGrid = EditorGUILayout.Toggle ( "Show Grid", showGrid, GUILayout.Width(150) );

        // ======================================================== 
        // BorderPreviewField 
        // ======================================================== 

        GUILayout.Space(20);
        lastRect = GUILayoutUtility.GetLastRect ();  
        BorderPreviewField ( new Rect ( 30, 
                                        lastRect.yMax, 
                                        previewWidth,
                                        previewHeight ), 
                             curEdit, 
                             editTexture );

        // ======================================================== 
        // Rebuild button
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void TexturePreviewField ( Rect _rect, exGUIBorder _guiBorder, Texture2D _texture ) {

        if ( _texture == null )
            return;

        float widthRatio = _rect.width/_texture.width;
        float heightRatio = _rect.height/_texture.height;
        GUI.BeginGroup(_rect);
            GUI.DrawTexture( new Rect( 0, 0, _rect.width, _rect.height), _texture );
            exEditorHelper.DrawRect( new Rect( 0, 0, _rect.width, _rect.height), new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.gray );
            // top line
            exEditorHelper.DrawLine ( new Vector2( 0, _guiBorder.border.top * heightRatio ),
                                      new Vector2( _rect.width, _guiBorder.border.top * heightRatio ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );

            // bottom line
            exEditorHelper.DrawLine ( new Vector2( 0, (_texture.height - _guiBorder.border.bottom) * heightRatio ),
                                      new Vector2( _rect.width, (_texture.height - _guiBorder.border.bottom) * heightRatio ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );

            // left line
            exEditorHelper.DrawLine ( new Vector2( _guiBorder.border.left * widthRatio, 0 ),
                                      new Vector2( _guiBorder.border.left * widthRatio, _rect.height ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );

            // right line
            exEditorHelper.DrawLine ( new Vector2( (_texture.width - _guiBorder.border.right) * widthRatio, 0 ),
                                      new Vector2( (_texture.width - _guiBorder.border.right) * widthRatio, _rect.height ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );
        GUI.EndGroup();
        GUILayoutUtility.GetRect ( _rect.width, _rect.height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void BorderPreviewField ( Rect _rect, exGUIBorder _guiBorder, Texture2D _texture ) {

        if ( _texture == null )
            return;

        float curX = _rect.x;
        float curY = _rect.y;
        int texCenterWidth = _texture.width - _guiBorder.border.horizontal;
        int texCenterHeight = _texture.height - _guiBorder.border.vertical;
        int previewCenterWidth = (int)_rect.width - _guiBorder.border.horizontal;
        int previewCenterHeight = (int)_rect.height - _guiBorder.border.vertical;

        float widthRatio = (float)previewCenterWidth/(texCenterWidth != 0 ? (float)texCenterWidth : 1.0f);
        float heightRatio = (float)previewCenterHeight/(texCenterHeight != 0 ? (float)texCenterHeight : 1.0f);

        // ======================================================== 
        // top 
        // ======================================================== 

        // left-top
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.left, 
                              _guiBorder.border.top ), 
                   new Rect ( 0, 0, 1, 1 ),
                   _texture );
        
        // mid-top
        curX += _guiBorder.border.left;
        DrawGrid ( new Rect ( curX, curY, 
                              previewCenterWidth,
                              _guiBorder.border.top ), 
                   new Rect ( _guiBorder.border.left, 
                              0, 
                              widthRatio, 
                              1 ),
                   _texture );

        // right-top
        curX += previewCenterWidth;
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.right, 
                              _guiBorder.border.top ), 
                   new Rect ( _texture.width - _guiBorder.border.right, 
                              0, 
                              1, 
                              1 ),
                   _texture );

        // ======================================================== 
        curX = _rect.x;
        curY += _guiBorder.border.top;
        // center 
        // ======================================================== 

        // left-center
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.left, 
                              previewCenterHeight ), 
                   new Rect ( 0, 
                              _guiBorder.border.top, 
                              1, 
                              heightRatio ),
                   _texture );
        
        curX += _guiBorder.border.left;
        // mid-center
        DrawGrid ( new Rect ( curX, curY, 
                              previewCenterWidth, 
                              previewCenterHeight ), 
                   new Rect ( _guiBorder.border.left, 
                              _guiBorder.border.top, 
                              widthRatio, 
                              heightRatio ),
                   _texture );

        // right-center
        curX += previewCenterWidth;
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.right, 
                              previewCenterHeight ), 
                   new Rect ( _texture.width - _guiBorder.border.right, 
                              _guiBorder.border.top, 
                              1, 
                              heightRatio ),
                   _texture );

        // ======================================================== 
        curX = _rect.x;
        curY += previewCenterHeight;
        // bottom 
        // ======================================================== 

        // left-bottom
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.left, 
                              _guiBorder.border.bottom ), 
                   new Rect ( 0, 
                              _texture.height - _guiBorder.border.bottom, 
                              1, 
                              1 ),
                   _texture );
        
        // mid-bottom
        curX += _guiBorder.border.left;
        DrawGrid ( new Rect ( curX, curY, 
                              previewCenterWidth,
                              _guiBorder.border.bottom ), 
                   new Rect ( _guiBorder.border.left, 
                              _texture.height - _guiBorder.border.bottom, 
                              widthRatio, 
                              1 ),
                   _texture );

        // right-bottom
        curX += previewCenterWidth;
        DrawGrid ( new Rect ( curX, curY,
                              _guiBorder.border.right, 
                              _guiBorder.border.bottom ), 
                   new Rect ( _texture.width - _guiBorder.border.right, 
                              _texture.height - _guiBorder.border.bottom, 
                              1, 
                              1 ),
                   _texture );

        //
        GUILayoutUtility.GetRect ( _rect.width, _rect.height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void DrawGrid ( Rect _dest, Rect _src, Texture2D _texture ) {

        GUI.BeginGroup( _dest );
            GUI.DrawTexture( new Rect( -_src.x * _src.width,
                                       -_src.y * _src.height,
                                       _texture.width  * _src.width,
                                       _texture.height * _src.height ), 
                             _texture );
        GUI.EndGroup();

        if ( showGrid ) {
            exEditorHelper.DrawRect( _dest, 
                                     new Color(0.0f, 0.0f, 0.0f, 0.0f), 
                                     new Color( 1.0f, 0.0f, 1.0f, 1.0f) );
        }

    }
}
