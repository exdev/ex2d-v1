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

    public static void UpdateAtlas ( exSpriteBorder _spriteBorder, 
                                     exAtlasDB.ElementInfo _elInfo ) {
        // get atlas and index from textureGUID
        if ( _elInfo != null ) {
            if ( _elInfo.guidAtlas != exEditorHelper.AssetToGUID(_spriteBorder.atlas) ||
                 _elInfo.indexInAtlas != _spriteBorder.index )
            {
                _spriteBorder.SetBorder( _spriteBorder.guiBorder,
                                   exEditorHelper.LoadAssetFromGUID<exAtlas>(_elInfo.guidAtlas), 
                                   _elInfo.indexInAtlas );
            }
        }
        else {
            exGUIBorder guiBorder = _spriteBorder.guiBorder;
            _spriteBorder.Clear();
            _spriteBorder.SetBorder( guiBorder, null, -1 );
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
        // exGUIBorder object filed
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
                exGUIBorderEditor editor = exGUIBorderEditor.NewWindow();
                editor.Edit(editSpriteBorder.guiBorder);
            }
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        // get edit texture
        Texture2D editTexture = null;
        if ( newGUIBorder ) {
            editTexture = exEditorHelper.LoadAssetFromGUID<Texture2D>(newGUIBorder.textureGUID); 

            // ======================================================== 
            // border preview
            // ======================================================== 

            Rect lastRect = GUILayoutUtility.GetLastRect ();  
            GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                lastRect = GUILayoutUtility.GetLastRect ();  
                Rect previewRect = new Rect ( lastRect.xMax, 
                                              lastRect.yMax, 
                                              Mathf.Max(100,newGUIBorder.border.horizontal), 
                                              Mathf.Max(100,newGUIBorder.border.vertical) );
                exGUIBorderEditor.TexturePreviewField ( previewRect, newGUIBorder, editTexture );

                GUILayout.Space(10);
                lastRect = GUILayoutUtility.GetLastRect ();  
                previewRect = new Rect ( lastRect.x,
                                         lastRect.yMax, 
                                         Mathf.Max(100,newGUIBorder.border.horizontal), 
                                         Mathf.Max(100,newGUIBorder.border.vertical) );
                exGUIBorderEditor.BorderPreviewField( previewRect, newGUIBorder, editTexture );

                if ( GUILayout.Button("Select...", GUILayout.Width(60), GUILayout.Height(15) ) ) {
                    EditorGUIUtility.PingObject(editTexture);
                }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

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
            // if we don't use atlas and current edit target use atlas, clear it.
            else {
                if ( editSpriteBorder.useAtlas ) {
                    borderChanged = true;
                }
            }

            // check if we are first time assignment
            if ( useAtlas || editTexture != null ) {
                if ( isPrefab == false && editSpriteBorder.meshFilter.sharedMesh == null ) {
                    needRebuild = true;
                }
            }
        }

        // set border
        if ( borderChanged ) {
            //
            if ( newGUIBorder == null ) {
                editSpriteBorder.Clear();
            }
            else {
                editSpriteBorder.SetBorder( newGUIBorder, editAtlas, editIndex );
                if ( editSpriteBorder.useAtlas == false ) {
                    editSpriteBorder.renderer.sharedMaterial = exEditorHelper.GetDefaultMaterial(editTexture);
                    editSpriteBorder.updateFlags |= exPlane.UpdateFlags.UV;
                }
            }

            GUI.changed = true;
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
                editSpriteBorder.Build( editTexture );
            }
            else if ( GUI.changed ) {
                if ( editSpriteBorder.meshFilter.sharedMesh != null )
                    editSpriteBorder.UpdateMesh( editSpriteBorder.meshFilter.sharedMesh );
                EditorUtility.SetDirty(editSpriteBorder);
            }
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSceneGUI () {

        //
        if ( editSpriteBorder.meshFilter == null || editSpriteBorder.meshFilter.sharedMesh == null ) {
            return;
        }

        //
        Vector3[] vertices = new Vector3[4]; 
        Vector3[] corners = new Vector3[5];
        Vector3[] controls = new Vector3[8];

        float halfWidthScaled = editSpriteBorder.width * editSpriteBorder.scale.x * 0.5f;
        float halfHeightScaled = editSpriteBorder.height * editSpriteBorder.scale.y * 0.5f;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        //
        switch ( editSpriteBorder.anchor ) {
        case exPlane.Anchor.TopLeft     : offsetX = -halfWidthScaled;   offsetY = -halfHeightScaled;  break;
        case exPlane.Anchor.TopCenter   : offsetX = 0.0f;               offsetY = -halfHeightScaled;  break;
        case exPlane.Anchor.TopRight    : offsetX = halfWidthScaled;    offsetY = -halfHeightScaled;  break;

        case exPlane.Anchor.MidLeft     : offsetX = -halfWidthScaled;   offsetY = 0.0f;               break;
        case exPlane.Anchor.MidCenter   : offsetX = 0.0f;               offsetY = 0.0f;               break;
        case exPlane.Anchor.MidRight    : offsetX = halfWidthScaled;    offsetY = 0.0f;               break;

        case exPlane.Anchor.BotLeft     : offsetX = -halfWidthScaled;   offsetY = halfHeightScaled;   break;
        case exPlane.Anchor.BotCenter   : offsetX = 0.0f;               offsetY = halfHeightScaled;   break;
        case exPlane.Anchor.BotRight    : offsetX = halfWidthScaled;    offsetY = halfHeightScaled;   break;

        default                         : offsetX = 0.0f;               offsetY = 0.0f;               break;
        }

        //
        switch ( editSpriteBorder.plane ) {
        case exPlane.Plane.XY:
            vertices[0] = new Vector3 (-halfWidthScaled+offsetX,  halfHeightScaled+offsetY, 0.0f );
            vertices[1] = new Vector3 ( halfWidthScaled+offsetX,  halfHeightScaled+offsetY, 0.0f );
            vertices[2] = new Vector3 ( halfWidthScaled+offsetX, -halfHeightScaled+offsetY, 0.0f );
            vertices[3] = new Vector3 (-halfWidthScaled+offsetX, -halfHeightScaled+offsetY, 0.0f );
            break;

        case exPlane.Plane.XZ:
            vertices[0] = new Vector3 (-halfWidthScaled+offsetX, 0.0f,  halfHeightScaled+offsetY );
            vertices[1] = new Vector3 ( halfWidthScaled+offsetX, 0.0f,  halfHeightScaled+offsetY );
            vertices[2] = new Vector3 ( halfWidthScaled+offsetX, 0.0f, -halfHeightScaled+offsetY );
            vertices[3] = new Vector3 (-halfWidthScaled+offsetX, 0.0f, -halfHeightScaled+offsetY );
            break;

        case exPlane.Plane.ZY:
            vertices[0] = new Vector3 (0.0f,  halfHeightScaled+offsetY, -halfWidthScaled+offsetX );
            vertices[1] = new Vector3 (0.0f,  halfHeightScaled+offsetY,  halfWidthScaled+offsetX );
            vertices[2] = new Vector3 (0.0f, -halfHeightScaled+offsetY,  halfWidthScaled+offsetX );
            vertices[3] = new Vector3 (0.0f, -halfHeightScaled+offsetY, -halfWidthScaled+offsetX );
            break;
        }

        // 0 -- 1
        // |    |
        // 3 -- 2

        Transform trans = editSpriteBorder.transform;
        corners[0] = trans.localToWorldMatrix * new Vector4 ( vertices[0].x, vertices[0].y, vertices[0].z, 1.0f );
        corners[1] = trans.localToWorldMatrix * new Vector4 ( vertices[1].x, vertices[1].y, vertices[1].z, 1.0f );
        corners[2] = trans.localToWorldMatrix * new Vector4 ( vertices[2].x, vertices[2].y, vertices[2].z, 1.0f );
        corners[3] = trans.localToWorldMatrix * new Vector4 ( vertices[3].x, vertices[3].y, vertices[3].z, 1.0f );
        corners[4] = corners[0];
        Handles.DrawPolyLine( corners );

        // 0 -- 1 -- 2
        // |         |
        // 7         3
        // |         |
        // 6 -- 5 -- 4

        controls[0] = corners[0];
        controls[1] = (corners[0] + corners[1])/2.0f;
        controls[2] = corners[1];
        controls[3] = (corners[1] + corners[2])/2.0f;
        controls[4] = corners[2];
        controls[5] = (corners[2] + corners[3])/2.0f;
        controls[6] = corners[3];
        controls[7] = (corners[3] + corners[0])/2.0f;

        // TEMP: not sure this is good { 
        Event e = Event.current;
        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            Undo.RegisterSceneUndo("ex2D.Scene");
        }
        // } TEMP end 

        //
        for ( int i = 0; i < controls.Length; ++i ) {
            Vector3 pos = controls[i];
            Handles.color = new Color( 1.0f, 1.0f, 0.0f, 0.5f );

            Vector3 newPos = Handles.FreeMoveHandle( pos,
                                                     editSpriteBorder.transform.rotation,
                                                     HandleUtility.GetHandleSize(pos) / 10.0f,
                                                     Vector3.zero,
                                                     Handles.DotCap
                                                   );
            HandleRezie ( i, pos, newPos );
        }
        // Handles.Label( editSpriteBorder.transform.position + Vector3.up * 2,
        //                "Size = " + editSpriteBorder.width + " x " + editSpriteBorder.height );

        if (GUI.changed)
            EditorUtility.SetDirty (editSpriteBorder);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void AddAnimationHelper () {
        editSpriteBorder.gameObject.AddComponent<exSpriteBorderAnimHelper>();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleRezie ( int _handleID, Vector3 _oldPos, Vector3 _newPos ) {
        if ( _oldPos == _newPos ) {
            return;
        }

        //
        float dx, dy;
        GetDelatSize ( _newPos - _oldPos, out dx, out dy );

        float oldWidth = editSpriteBorder.width;
        float oldHeight = editSpriteBorder.height;
        float xRatio = 1.0f;
        float yRatio = 1.0f;

        // 0 -- 1 -- 2
        // |         |
        // 7         3
        // |         |
        // 6 -- 5 -- 4

        if ( _handleID == 0 ) {
            switch ( editSpriteBorder.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = -1.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = -0.5f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = -0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = -1.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = -0.5f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = -0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = -1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = -0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = -0.0f; yRatio = 0.0f; break;
            }
            editSpriteBorder.width -= dx; editSpriteBorder.height += dy;
        }
        else if ( _handleID == 1 ) {
            switch ( editSpriteBorder.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = 0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 0.0f; yRatio = 0.0f; break;
            }
            editSpriteBorder.height += dy;
        }
        else if ( _handleID == 2 ) {
            switch ( editSpriteBorder.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.5f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 1.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.5f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = 1.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 1.0f; yRatio = 0.0f; break;
            }
            editSpriteBorder.width += dx; editSpriteBorder.height += dy;
        }
        else if ( _handleID == 3 ) {
            switch ( editSpriteBorder.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidRight:    xRatio = 1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 1.0f; yRatio = 0.0f; break;
            }
            editSpriteBorder.width += dx;
        }
        else if ( _handleID == 4 ) {
            switch ( editSpriteBorder.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.5f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 1.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.5f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = 1.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.5f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 1.0f; yRatio = -1.0f; break;
            }
            editSpriteBorder.width += dx; editSpriteBorder.height -= dy;
        }
        else if ( _handleID == 5 ) {
            switch ( editSpriteBorder.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = 0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.0f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 0.0f; yRatio = -1.0f; break;
            }
            editSpriteBorder.height -= dy;
        }
        else if ( _handleID == 6 ) {
            switch ( editSpriteBorder.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = -1.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = -0.5f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = -0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = -1.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = -0.5f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = -0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = -1.0f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = -0.5f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = -0.0f; yRatio = -1.0f; break;
            }
            editSpriteBorder.width -= dx; editSpriteBorder.height -= dy;
        }
        else if ( _handleID == 7 ) {
            switch ( editSpriteBorder.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = -1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = -0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = -0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = -1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidCenter:   xRatio = -0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidRight:    xRatio = -0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotLeft:     xRatio = -1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = -0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = -0.0f; yRatio = 0.0f; break;
            }
            editSpriteBorder.width -= dx;
        }

        float offsetX = (editSpriteBorder.width - oldWidth) * xRatio * editSpriteBorder.scale.x;
        float offsetY = (editSpriteBorder.height - oldHeight) * yRatio * editSpriteBorder.scale.y;
        editSpriteBorder.Translate ( offsetX, offsetY );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void GetDelatSize ( Vector3 _deltaVec, out float _dx, out float _dy ) {
        _dx = 0.0f; _dy = 0.0f;

        switch ( editSpriteBorder.plane ) {
        case exPlane.Plane.XY:
            _dx = _deltaVec.x / editSpriteBorder.scale.x; _dy = _deltaVec.y / editSpriteBorder.scale.y;
            break;

        case exPlane.Plane.XZ:
            _dx = _deltaVec.x / editSpriteBorder.scale.x; _dy = _deltaVec.z / editSpriteBorder.scale.y;
            break;

        case exPlane.Plane.ZY:
            _dx = _deltaVec.z / editSpriteBorder.scale.x; _dy = _deltaVec.y / editSpriteBorder.scale.y;
            break;
        }
    }
}
