// ======================================================================================
// File         : exTileMapEditor.cs
// Author       : Wu Jie 
// Last Change  : 08/20/2011 | 17:06:48 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// exTileMapEditor
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exTileMap))]
partial class exTileMapEditor : exPlaneEditor {

    private exTileMap editTileMap;
    private Plane plane;
    private Vector2 mousePos = Vector2.zero;
    private Vector3 startOffset = Vector3.zero;
    private Rect tileMapRect;
    private bool isMouseInside = false;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != editTileMap ) {
            editTileMap = target as exTileMap;
        }
        InternalUpdate ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void InternalUpdate () {
        float width = editTileMap.col * editTileMap.tileWidth;
        float height = editTileMap.row * editTileMap.tileHeight;
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        //
        switch ( editTileMap.anchor ) {
        case exSpriteBase.Anchor.TopLeft:   offsetX = 0.0f;         offsetY = -height; break;
        case exSpriteBase.Anchor.TopCenter: offsetX = -halfWidth;   offsetY = -height; break;
        case exSpriteBase.Anchor.TopRight:  offsetX = -width;       offsetY = -height; break;

        case exSpriteBase.Anchor.MidLeft:   offsetX = 0.0f;         offsetY = -halfHeight; break;
        case exSpriteBase.Anchor.MidCenter: offsetX = -halfWidth;   offsetY = -halfHeight; break;
        case exSpriteBase.Anchor.MidRight:  offsetX = -width;       offsetY = -halfHeight; break;

        case exSpriteBase.Anchor.BotLeft:   offsetX = 0.0f;         offsetY = 0.0f; break;
        case exSpriteBase.Anchor.BotCenter: offsetX = -halfWidth;   offsetY = 0.0f; break;
        case exSpriteBase.Anchor.BotRight:  offsetX = -width;       offsetY = 0.0f; break;
        }

        //
        switch ( editTileMap.plane ) {
        case exPlane.Plane.XY:
            plane = new Plane ( Vector3.forward, editTileMap.transform.position ); 
            startOffset = new Vector3 ( offsetX, offsetY, 0.0f );
            tileMapRect = new Rect( editTileMap.transform.position.x + offsetX,
                                    editTileMap.transform.position.y + offsetY,
                                    width, 
                                    height );
            break;
        case exPlane.Plane.XZ:
            plane = new Plane ( Vector3.up, editTileMap.transform.position ); 
            startOffset = new Vector3 ( offsetX, 0.0f, offsetY );
            tileMapRect = new Rect( editTileMap.transform.position.x + offsetX,
                                    editTileMap.transform.position.z + offsetY,
                                    width, 
                                    height );
            break;
        case exPlane.Plane.ZY:
            plane = new Plane ( Vector3.right, editTileMap.transform.position ); 
            startOffset = new Vector3 ( 0.0f, offsetY, offsetX );
            tileMapRect = new Rect( editTileMap.transform.position.z + offsetX,
                                    editTileMap.transform.position.y + offsetY,
                                    width, 
                                    height );
            break;
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

        bool needRebuild = false;
        MeshFilter meshFilter = editTileMap.GetComponent<MeshFilter>();

        //
        EditorGUIUtility.LookLikeInspector ();
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        // ======================================================== 
        // row
        // ======================================================== 

        int newRow = EditorGUILayout.IntField( "Row", editTileMap.row ); 

        // ======================================================== 
        // col 
        // ======================================================== 

        int newCol = EditorGUILayout.IntField( "Col", editTileMap.col ); 

        if ( newRow != editTileMap.row ||
             newCol != editTileMap.col )
        {
            editTileMap.Resize(newRow,newCol);
            needRebuild = true;
        }

        // ======================================================== 
        // Tile Width 
        // ======================================================== 

        editTileMap.tileWidth = EditorGUILayout.IntField( "Tile Width", editTileMap.tileWidth ); 

        // ======================================================== 
        // Tile Height 
        // ======================================================== 

        editTileMap.tileHeight = EditorGUILayout.IntField( "Tile Height", editTileMap.tileHeight ); 

        // ======================================================== 
        // color
        // ======================================================== 

        editTileMap.color = EditorGUILayout.ColorField ( "Color", editTileMap.color );

        ///////////////////////////////////////////////////////////////////////////////
        // Preview
        ///////////////////////////////////////////////////////////////////////////////

        // ======================================================== 
        // Show Grid
        // ======================================================== 

        editTileMap.editorShowGrid = EditorGUILayout.Toggle( "Show Grid", editTileMap.editorShowGrid ); 

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
                // Debug.Log("rebuild mesh...");
                editTileMap.Build();
            }
            else if ( GUI.changed ) {
                // Debug.Log("update mesh...");
                if ( meshFilter.sharedMesh != null )
                    editTileMap.UpdateMesh( meshFilter.sharedMesh );
                EditorUtility.SetDirty(editTileMap);
            }

            // change plane if anchor or plane changes
            if ( GUI.changed ) {
                InternalUpdate ();
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void OnSceneGUI () {

        // ======================================================== 
        Event e = Event.current;
        // ======================================================== 

        mousePos = e.mousePosition;
        switch ( e.type ) {
        case EventType.mouseMove:
            HandleUtility.Repaint();
            break;
        }

        // ======================================================== 
        // draw mouse move 
        // ======================================================== 

        Ray ray = HandleUtility.GUIPointToWorldRay( mousePos );
        float dist;
        if ( plane.Raycast(ray, out dist ) ) {
            Vector3 pos = ray.origin +  ray.direction.normalized * dist;
            switch ( editTileMap.plane ) {
            case exPlane.Plane.XY: pos.z = editTileMap.transform.position.z; break;
            case exPlane.Plane.XZ: pos.y = editTileMap.transform.position.y; break;
            case exPlane.Plane.ZY: pos.x = editTileMap.transform.position.x; break;
            }
            pos = SnapToGrid (pos);

            if ( isMouseInside ) {
                DrawMouseGrid ( pos );
            }
        }

    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    Vector3 SnapToGrid ( Vector3 _pos ) {
        Vector3 startPos = editTileMap.transform.position + startOffset;
        Vector3 deltaPos = _pos - startPos;
        Vector2 worldMousePos = Vector2.zero;

        //
        switch ( editTileMap.plane ) {
        case exPlane.Plane.XY:
            deltaPos.x = Mathf.Floor(deltaPos.x / editTileMap.tileWidth) * editTileMap.tileWidth;
            deltaPos.y = Mathf.Floor(deltaPos.y / editTileMap.tileHeight) * editTileMap.tileHeight;
            worldMousePos = new Vector2 ( _pos.x, _pos.y );
            break;
        case exPlane.Plane.XZ:
            deltaPos.x = Mathf.Floor(deltaPos.x / editTileMap.tileWidth) * editTileMap.tileWidth;
            deltaPos.z = Mathf.Floor(deltaPos.z / editTileMap.tileHeight) * editTileMap.tileHeight;
            worldMousePos = new Vector2 ( _pos.x, _pos.z );
            break;
        case exPlane.Plane.ZY:
            deltaPos.z = Mathf.Floor(deltaPos.z / editTileMap.tileWidth) * editTileMap.tileWidth;
            deltaPos.y = Mathf.Floor(deltaPos.y / editTileMap.tileHeight) * editTileMap.tileHeight;
            worldMousePos = new Vector2 ( _pos.z, _pos.y );
            break;
        }

        //
        isMouseInside = false;
        if ( tileMapRect.Contains(worldMousePos) ) {
            isMouseInside = true;
        }

        return (startPos + deltaPos);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void DrawMouseGrid ( Vector3 _pos ) {
        Handles.color = Color.white;
        Color faceColor = new Color( 1.0f, 1.0f, 1.0f, 0.2f );
        Color lineColor = new Color( 0.0f, 0.0f, 0.0f, 1.0f );

        switch ( editTileMap.plane ) {
        case exPlane.Plane.XY:
            Handles.DrawSolidRectangleWithOutline( new Vector3[] {
                                                   _pos + new Vector3( 0.0f,                   0.0f,                   0.0f ), 
                                                   _pos + new Vector3( 0.0f,                   editTileMap.tileHeight, 0.0f ),
                                                   _pos + new Vector3( editTileMap.tileWidth,  editTileMap.tileHeight, 0.0f ),
                                                   _pos + new Vector3( editTileMap.tileWidth,  0.0f,                   0.0f ),
                                                   _pos + new Vector3( 0.0f,                   0.0f,                   0.0f )
                                                   },
                                                   faceColor, 
                                                   lineColor );
            break;

        case exPlane.Plane.XZ:
            Handles.DrawSolidRectangleWithOutline( new Vector3[] {
                                                   _pos + new Vector3( 0.0f,                   0.0f, 0.0f ), 
                                                   _pos + new Vector3( 0.0f,                   0.0f, editTileMap.tileHeight ),
                                                   _pos + new Vector3( editTileMap.tileWidth,  0.0f, editTileMap.tileHeight ),
                                                   _pos + new Vector3( editTileMap.tileWidth,  0.0f, 0.0f ),
                                                   _pos + new Vector3( 0.0f,                   0.0f, 0.0f )
                                                   },
                                                   faceColor, 
                                                   lineColor );
            break;

        case exPlane.Plane.ZY:
            Handles.DrawSolidRectangleWithOutline( new Vector3[] {
                                                   _pos + new Vector3( 0.0f,                   0.0f, 0.0f ), 
                                                   _pos + new Vector3( editTileMap.tileHeight, 0.0f, 0.0f  ),
                                                   _pos + new Vector3( editTileMap.tileHeight, 0.0f, editTileMap.tileWidth ),
                                                   _pos + new Vector3( 0.0f,                   0.0f, editTileMap.tileWidth ),
                                                   _pos + new Vector3( 0.0f,                   0.0f, 0.0f )
                                                   },
                                                   faceColor, 
                                                   lineColor );
            break;
        }
    }
}
