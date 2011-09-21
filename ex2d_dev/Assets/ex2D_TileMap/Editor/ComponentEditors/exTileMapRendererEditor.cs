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

    // DISABLE { 
    // private Rect tileMapRect;
    // private bool isMouseInside = false;
    // private GameObject mouseTile = null;
    // private Plane plane;
    // private Vector3 startOffset = Vector3.zero;
    // private Vector3 curPos = Vector3.zero;
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != editTileMap ) {
            editTileMap = target as exTileMapRenderer;
            editTileMap.editorShowGrid = true;
        }
        // DISABLE { 
        // InternalUpdate ();
        // } DISABLE end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDisable () {
        // DISABLE { 
        // HandleUtility.AddDefaultControl(-1);
        // editTileMap.editorShowGrid = false;
        // } DISABLE end 
    }

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // public void InternalUpdate () {
    //     float width = editTileMap.col * editTileMap.tileWidth;
    //     float height = editTileMap.row * editTileMap.tileHeight;
    //     float halfWidth = width * 0.5f;
    //     float halfHeight = height * 0.5f;
    //     float offsetX = 0.0f;
    //     float offsetY = 0.0f;

    //     //
    //     switch ( editTileMap.anchor ) {
    //     case exSpriteBase.Anchor.TopLeft:   offsetX = 0.0f;         offsetY = -height; break;
    //     case exSpriteBase.Anchor.TopCenter: offsetX = -halfWidth;   offsetY = -height; break;
    //     case exSpriteBase.Anchor.TopRight:  offsetX = -width;       offsetY = -height; break;

    //     case exSpriteBase.Anchor.MidLeft:   offsetX = 0.0f;         offsetY = -halfHeight; break;
    //     case exSpriteBase.Anchor.MidCenter: offsetX = -halfWidth;   offsetY = -halfHeight; break;
    //     case exSpriteBase.Anchor.MidRight:  offsetX = -width;       offsetY = -halfHeight; break;

    //     case exSpriteBase.Anchor.BotLeft:   offsetX = 0.0f;         offsetY = 0.0f; break;
    //     case exSpriteBase.Anchor.BotCenter: offsetX = -halfWidth;   offsetY = 0.0f; break;
    //     case exSpriteBase.Anchor.BotRight:  offsetX = -width;       offsetY = 0.0f; break;
    //     }

    //     //
    //     switch ( editTileMap.plane ) {
    //     case exPlane.Plane.XY:
    //         plane = new Plane ( Vector3.forward, editTileMap.transform.position ); 
    //         startOffset = new Vector3 ( offsetX, offsetY, 0.0f );
    //         tileMapRect = new Rect( editTileMap.transform.position.x + offsetX,
    //                                 editTileMap.transform.position.y + offsetY,
    //                                 width, 
    //                                 height );
    //         break;
    //     case exPlane.Plane.XZ:
    //         plane = new Plane ( Vector3.up, editTileMap.transform.position ); 
    //         startOffset = new Vector3 ( offsetX, 0.0f, offsetY );
    //         tileMapRect = new Rect( editTileMap.transform.position.x + offsetX,
    //                                 editTileMap.transform.position.z + offsetY,
    //                                 width, 
    //                                 height );
    //         break;
    //     case exPlane.Plane.ZY:
    //         plane = new Plane ( Vector3.right, editTileMap.transform.position ); 
    //         startOffset = new Vector3 ( 0.0f, offsetY, offsetX );
    //         tileMapRect = new Rect( editTileMap.transform.position.z + offsetX,
    //                                 editTileMap.transform.position.y + offsetY,
    //                                 width, 
    //                                 height );
    //         break;
    //     }
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	override public void OnInspectorGUI () {

        // ======================================================== 
        // exSprite Base GUI 
        // ======================================================== 

        base.OnInspectorGUI();
        bool needRebuild = false;

        // //
        // EditorGUILayout.Space ();
        // EditorGUI.indentLevel = 1;

        // EditorGUIUtility.LookLikeControls ();
        // GUILayout.BeginHorizontal ();

        //     // ======================================================== 
        //     // col 
        //     // ======================================================== 

        //     int newCol = EditorGUILayout.IntField( "Col", editTileMap.col ); 

        //     // ======================================================== 
        //     // row
        //     // ======================================================== 

        //     int newRow = EditorGUILayout.IntField( "Row", editTileMap.row ); 

        // GUILayout.EndHorizontal ();

        // if ( newRow != editTileMap.row ||
        //      newCol != editTileMap.col )
        // {
        //     editTileMap.Resize(newRow,newCol);
        //     needRebuild = true;
        // }

        // GUILayout.BeginHorizontal ();

        //     // ======================================================== 
        //     // Tile Width 
        //     // ======================================================== 

        //     editTileMap.tileWidth = EditorGUILayout.IntField( "Tile Width", editTileMap.tileWidth ); 

        //     // ======================================================== 
        //     // Tile Height 
        //     // ======================================================== 

        //     editTileMap.tileHeight = EditorGUILayout.IntField( "Tile Height", editTileMap.tileHeight ); 

        // GUILayout.EndHorizontal ();

        // GUILayout.BeginHorizontal ();

        //     // ======================================================== 
        //     // Tile Offset X 
        //     // ======================================================== 

        //     editTileMap.tileOffsetX = EditorGUILayout.IntField( "Tile Offset X", editTileMap.tileOffsetX ); 

        //     // ======================================================== 
        //     // Tile Offset Y 
        //     // ======================================================== 

        //     editTileMap.tileOffsetY = EditorGUILayout.IntField( "Tile Offset Y", editTileMap.tileOffsetY ); 

        // GUILayout.EndHorizontal ();

        // EditorGUIUtility.LookLikeInspector ();

        // // ======================================================== 
        // // color
        // // ======================================================== 

        // editTileMap.color = EditorGUILayout.ColorField ( "Color", editTileMap.color );

        // // ======================================================== 
        // // Show Grid
        // // ======================================================== 

        // editTileMap.editorShowGrid = EditorGUILayout.Toggle( "Show Grid", editTileMap.editorShowGrid ); 

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
                // Debug.Log("rebuild mesh...");
                editTileMap.Build();
            }
            else if ( GUI.changed ) {
                // Debug.Log("update mesh...");
                if ( editTileMap.meshFilter.sharedMesh != null )
                    editTileMap.UpdateMesh( editTileMap.meshFilter.sharedMesh );
                EditorUtility.SetDirty(editTileMap);
            }

            // DISABLE { 
            // // change plane if anchor or plane changes
            // if ( GUI.changed ) {
            //     InternalUpdate ();
            // }
            // } DISABLE end 
        }
    }

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void OnSceneGUI () {

    //     // ======================================================== 
    //     Event e = Event.current;
    //     // ======================================================== 

    //     int controlID = GUIUtility.GetControlID(FocusType.Passive);
    //     switch ( e.type ) {
    //     case EventType.mouseMove:
    //         HandleUtility.Repaint();

    //         Ray ray = HandleUtility.GUIPointToWorldRay( e.mousePosition );
    //         float dist;
    //         if ( plane.Raycast(ray, out dist ) ) {
    //             Vector3 pos = ray.origin +  ray.direction.normalized * dist;
    //             switch ( editTileMap.plane ) {
    //             case exPlane.Plane.XY: pos.z = editTileMap.transform.position.z; break;
    //             case exPlane.Plane.XZ: pos.y = editTileMap.transform.position.y; break;
    //             case exPlane.Plane.ZY: pos.x = editTileMap.transform.position.x; break;
    //             }
    //             curPos = SnapToGrid (pos);
    //         }
    //         break;

    //     case EventType.mouseDown:
    //         if ( isMouseInside == false ) {
    //             HandleUtility.AddDefaultControl(-1);
    //             Selection.activeObject = null;
    //         }
    //         break;

    //     case EventType.layout: 
    //         HandleUtility.AddDefaultControl(controlID);
    //         break;
    //     }

    //     // ======================================================== 
    //     // draw mouse move 
    //     // ======================================================== 

    //     if ( isMouseInside ) {
    //         DrawMouseGrid ( curPos );
    //     }
    //     else {
    //         ShowMouseTile (false);
    //     }
    // }
    // } DISABLE end 

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void ShowMouseTile ( bool _show ) {
    //     if ( _show ) {
    //         if ( mouseTile == null ) {
    //             mouseTile = new GameObject(".temp_mouse_tile");
    //             // mouseTile.hideFlags = HideFlags.HideAndDontSave;
    //             // mouseTile.hideFlags = HideFlags.HideInHierarchy;
    //             mouseTile.hideFlags = HideFlags.DontSave;
    //             // DELME { 
    //             // mouseTile.transform.parent = editTileMap.transform;
    //             // EditorUtility.SetSelectedWireframeHidden(mouseTile.renderer, true);
    //             // } DELME end 

    //             exTileMapRenderer tm = mouseTile.AddComponent<exTileMapRenderer>();
    //             tm.anchor = exPlane.Anchor.BotLeft;
    //             tm.plane  = editTileMap.plane;

    //             tm.tileSheet     = editTileMap.tileSheet;
    //             tm.tileWidth    = editTileMap.tileSheet.tileWidth;
    //             tm.tileHeight   = editTileMap.tileSheet.tileHeight;
    //             tm.tileOffsetX  = editTileMap.tileOffsetX;
    //             tm.tileOffsetY  = editTileMap.tileOffsetY;

    //             tm.Resize(1,1);
    //             tm.Build();
    //             // TODO { 
    //             if ( selectedGrids.Count > 0 )
    //                 tm.SetTile ( 0, 0, selectedGrids[0] );
    //             // } TODO end 
    //             tm.meshFilter.sharedMesh.hideFlags = HideFlags.DontSave;
    //         }
    //     }
    //     else {
    //         if ( mouseTile != null ) {
    //             Object.DestroyImmediate(mouseTile.GetComponent<MeshFilter>().sharedMesh,true); 
    //             Object.DestroyImmediate(mouseTile,true); 
    //         }
    //         mouseTile = null;
    //     }
    // }
    // } DISABLE end 

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // Vector3 SnapToGrid ( Vector3 _pos ) {
    //     Vector3 startPos = editTileMap.transform.position + startOffset;
    //     Vector3 deltaPos = _pos - startPos;
    //     Vector2 worldMousePos = Vector2.zero;

    //     //
    //     switch ( editTileMap.plane ) {
    //     case exPlane.Plane.XY:
    //         deltaPos.x = Mathf.Floor(deltaPos.x / editTileMap.tileWidth) * editTileMap.tileWidth;
    //         deltaPos.y = Mathf.Floor(deltaPos.y / editTileMap.tileHeight) * editTileMap.tileHeight;
    //         worldMousePos = new Vector2 ( _pos.x, _pos.y );
    //         break;
    //     case exPlane.Plane.XZ:
    //         deltaPos.x = Mathf.Floor(deltaPos.x / editTileMap.tileWidth) * editTileMap.tileWidth;
    //         deltaPos.z = Mathf.Floor(deltaPos.z / editTileMap.tileHeight) * editTileMap.tileHeight;
    //         worldMousePos = new Vector2 ( _pos.x, _pos.z );
    //         break;
    //     case exPlane.Plane.ZY:
    //         deltaPos.z = Mathf.Floor(deltaPos.z / editTileMap.tileWidth) * editTileMap.tileWidth;
    //         deltaPos.y = Mathf.Floor(deltaPos.y / editTileMap.tileHeight) * editTileMap.tileHeight;
    //         worldMousePos = new Vector2 ( _pos.z, _pos.y );
    //         break;
    //     }

    //     //
    //     isMouseInside = false;
    //     if ( tileMapRect.Contains(worldMousePos) ) {
    //         isMouseInside = true;
    //     }

    //     return (startPos + deltaPos);
    // }
    // } DISABLE end 

    // DISABLE { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void DrawMouseGrid ( Vector3 _pos ) {
    //     if ( editTileMap.tileSheet == null )
    //         return;

    //     if ( mouseTile != null )
    //         mouseTile.transform.position = _pos;

    //     // DELME { 
    //     // Handles.color = Color.white;
    //     // Color faceColor = new Color( 1.0f, 1.0f, 1.0f, 0.2f );
    //     // Color lineColor = new Color( 0.0f, 0.0f, 0.0f, 1.0f );
    //     // float width = editTileMap.tileSheet.tileWidth;
    //     // float height = editTileMap.tileSheet.tileHeight;

    //     // switch ( editTileMap.plane ) {
    //     // case exPlane.Plane.XY:
    //     //     Handles.DrawSolidRectangleWithOutline( new Vector3[] {
    //     //                                            _pos + new Vector3( 0.0f,   0.0f,   0.0f ), 
    //     //                                            _pos + new Vector3( 0.0f,   height, 0.0f ),
    //     //                                            _pos + new Vector3( width,  height, 0.0f ),
    //     //                                            _pos + new Vector3( width,  0.0f,   0.0f ),
    //     //                                            _pos + new Vector3( 0.0f,   0.0f,   0.0f )
    //     //                                            },
    //     //                                            faceColor, 
    //     //                                            lineColor );
    //     //     break;

    //     // case exPlane.Plane.XZ:
    //     //     Handles.DrawSolidRectangleWithOutline( new Vector3[] {
    //     //                                            _pos + new Vector3( 0.0f,   0.0f, 0.0f   ), 
    //     //                                            _pos + new Vector3( 0.0f,   0.0f, height ),
    //     //                                            _pos + new Vector3( width,  0.0f, height ),
    //     //                                            _pos + new Vector3( width,  0.0f, 0.0f   ),
    //     //                                            _pos + new Vector3( 0.0f,   0.0f, 0.0f   )
    //     //                                            },
    //     //                                            faceColor, 
    //     //                                            lineColor );
    //     //     break;

    //     // case exPlane.Plane.ZY:
    //     //     Handles.DrawSolidRectangleWithOutline( new Vector3[] {
    //     //                                            _pos + new Vector3( 0.0f,   0.0f, 0.0f  ), 
    //     //                                            _pos + new Vector3( height, 0.0f, 0.0f  ),
    //     //                                            _pos + new Vector3( height, 0.0f, width ),
    //     //                                            _pos + new Vector3( 0.0f,   0.0f, width ),
    //     //                                            _pos + new Vector3( 0.0f,   0.0f, 0.0f  )
    //     //                                            },
    //     //                                            faceColor, 
    //     //                                            lineColor );
    //     //     break;
    //     // }
    //     // } DELME end 
    // }
    // } DISABLE end 
}
