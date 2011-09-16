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

    // DELME { 
    // struct GridID {
    //     public int x;
    //     public int y;

    //     public GridID ( int _x, int _y ) { x = _x; y = _y; }
    //     public static bool operator == ( GridID _a, GridID _b ) { return _a.Equals(_b); }
    //     public static bool operator != ( GridID _a, GridID _b ) { return !_a.Equals(_b); }
    //     public override int GetHashCode() { return x ^ y; }
    //     public override bool Equals ( object _obj ) {
    //         if ( !(_obj is GridID) )
    //             return false;
    //         return Equals((GridID)_obj);
    //     }
    //     public bool Equals ( GridID _other ) {
    //         if ( x != _other.x || y != _other.y ) {
    //             return false;
    //         }
    //         return true;
    //     }
    // }
    // } DELME end 

    private exTileMapRenderer editTileMap;
    private Plane plane;
    private Vector3 startOffset = Vector3.zero;
    private Vector3 curPos = Vector3.zero;
    private Rect tileMapRect;
    private bool isMouseInside = false;
    private GameObject mouseTile = null;

    private bool inRectSelectState = false;
    private List<int> selectedGrids = new List<int>();
    private List<int> rectSelectedGrids = new List<int>();
    private Rect selectRect = new Rect( 0, 0, 1, 1 );
    private Vector2 mouseDownPos = Vector2.zero;

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();
        if ( target != editTileMap ) {
            editTileMap = target as exTileMapRenderer;
            editTileMap.editorShowGrid = true;
        }
        InternalUpdate ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDisable () {
        ShowMouseTile (false);
        HandleUtility.AddDefaultControl(-1);
        editTileMap.editorShowGrid = false;
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

        //
        EditorGUILayout.Space ();
        EditorGUI.indentLevel = 1;

        EditorGUIUtility.LookLikeControls ();
        GUILayout.BeginHorizontal ();

            // ======================================================== 
            // col 
            // ======================================================== 

            int newCol = EditorGUILayout.IntField( "Col", editTileMap.col ); 

            // ======================================================== 
            // row
            // ======================================================== 

            int newRow = EditorGUILayout.IntField( "Row", editTileMap.row ); 

        GUILayout.EndHorizontal ();

        if ( newRow != editTileMap.row ||
             newCol != editTileMap.col )
        {
            editTileMap.Resize(newRow,newCol);
            needRebuild = true;
        }

        GUILayout.BeginHorizontal ();

            // ======================================================== 
            // Tile Width 
            // ======================================================== 

            editTileMap.tileWidth = EditorGUILayout.IntField( "Tile Width", editTileMap.tileWidth ); 

            // ======================================================== 
            // Tile Height 
            // ======================================================== 

            editTileMap.tileHeight = EditorGUILayout.IntField( "Tile Height", editTileMap.tileHeight ); 

        GUILayout.EndHorizontal ();

        GUILayout.BeginHorizontal ();

            // ======================================================== 
            // Tile Offset X 
            // ======================================================== 

            editTileMap.tileOffsetX = EditorGUILayout.IntField( "Tile Offset X", editTileMap.tileOffsetX ); 

            // ======================================================== 
            // Tile Offset Y 
            // ======================================================== 

            editTileMap.tileOffsetY = EditorGUILayout.IntField( "Tile Offset Y", editTileMap.tileOffsetY ); 

        GUILayout.EndHorizontal ();

        EditorGUIUtility.LookLikeInspector ();

        // ======================================================== 
        // color
        // ======================================================== 

        editTileMap.color = EditorGUILayout.ColorField ( "Color", editTileMap.color );

        // ======================================================== 
        // Show Grid
        // ======================================================== 

        editTileMap.editorShowGrid = EditorGUILayout.Toggle( "Show Grid", editTileMap.editorShowGrid ); 

        // ======================================================== 
        // TileSheet Field 
        // ======================================================== 

        TileSheetField ( editTileMap.tileSheet ); 
        if ( editTileMap.tileSheet != null && editTileMap.meshFilter.sharedMesh == null ) {
            needRebuild = true;
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
                // Debug.Log("rebuild mesh...");
                editTileMap.Build();
            }
            else if ( GUI.changed ) {
                // Debug.Log("update mesh...");
                if ( editTileMap.meshFilter.sharedMesh != null )
                    editTileMap.UpdateMesh( editTileMap.meshFilter.sharedMesh );
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

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch ( e.type ) {
        case EventType.mouseMove:
            HandleUtility.Repaint();

            Ray ray = HandleUtility.GUIPointToWorldRay( e.mousePosition );
            float dist;
            if ( plane.Raycast(ray, out dist ) ) {
                Vector3 pos = ray.origin +  ray.direction.normalized * dist;
                switch ( editTileMap.plane ) {
                case exPlane.Plane.XY: pos.z = editTileMap.transform.position.z; break;
                case exPlane.Plane.XZ: pos.y = editTileMap.transform.position.y; break;
                case exPlane.Plane.ZY: pos.x = editTileMap.transform.position.x; break;
                }
                curPos = SnapToGrid (pos);
            }
            break;

        case EventType.mouseDown:
            if ( isMouseInside == false ) {
                HandleUtility.AddDefaultControl(-1);
                Selection.activeObject = null;
            }
            break;

        case EventType.layout: 
            HandleUtility.AddDefaultControl(controlID);
            break;
        }

        // ======================================================== 
        // draw mouse move 
        // ======================================================== 

        if ( isMouseInside ) {
            DrawMouseGrid ( curPos );
        }
        else {
            ShowMouseTile (false);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ShowMouseTile ( bool _show ) {
        if ( _show ) {
            if ( mouseTile == null ) {
                mouseTile = new GameObject(".temp_mouse_tile");
                // mouseTile.hideFlags = HideFlags.HideAndDontSave;
                // mouseTile.hideFlags = HideFlags.HideInHierarchy;
                mouseTile.hideFlags = HideFlags.DontSave;
                // DELME { 
                // mouseTile.transform.parent = editTileMap.transform;
                // EditorUtility.SetSelectedWireframeHidden(mouseTile.renderer, true);
                // } DELME end 

                exTileMapRenderer tm = mouseTile.AddComponent<exTileMapRenderer>();
                tm.anchor = exPlane.Anchor.BotLeft;
                tm.plane  = editTileMap.plane;

                tm.tileSheet     = editTileMap.tileSheet;
                tm.tileWidth    = editTileMap.tileSheet.tileWidth;
                tm.tileHeight   = editTileMap.tileSheet.tileHeight;
                tm.tileOffsetX  = editTileMap.tileOffsetX;
                tm.tileOffsetY  = editTileMap.tileOffsetY;

                tm.Resize(1,1);
                tm.Build();
                // TODO { 
                if ( selectedGrids.Count > 0 )
                    tm.SetTile ( 0, 0, selectedGrids[0] );
                // } TODO end 
                tm.meshFilter.sharedMesh.hideFlags = HideFlags.DontSave;
            }
        }
        else {
            if ( mouseTile != null ) {
                Object.DestroyImmediate(mouseTile.GetComponent<MeshFilter>().sharedMesh,true); 
                Object.DestroyImmediate(mouseTile,true); 
            }
            mouseTile = null;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void TileSheetField ( exTileSheet _tileSheet ) {
        GUILayout.BeginHorizontal();
            editTileMap.tileSheet 
                = (exTileSheet)EditorGUILayout.ObjectField( "Tile Sheet"
                                                           , editTileMap.tileSheet
                                                           , typeof(exTileSheet)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                           , false
#endif
                                                         );
            if ( GUILayout.Button("Edit...", GUILayout.Width(40), GUILayout.Height(15) ) ) {
                exTileSheetEditor editor = exTileSheetEditor.NewWindow();
                editor.Edit(editTileMap.tileSheet);
            }
        GUILayout.EndHorizontal();

        if ( editTileMap.tileSheet == null )
            return;

        exTileSheet tileSheet = editTileMap.tileSheet;
        int col = 0;
        int row = 0;
        int uvX = tileSheet.padding;
        int uvY = tileSheet.padding;

        // count the col
        while ( (uvX + tileSheet.tileWidth + tileSheet.padding) <= tileSheet.texture.width ) {
            uvX = uvX + tileSheet.tileWidth + tileSheet.padding; 
            ++col;
        }

        // count the row
        while ( (uvY + tileSheet.tileHeight + tileSheet.padding) <= tileSheet.texture.height ) {
            uvY = uvY + tileSheet.tileHeight + tileSheet.padding; 
            ++row;
        }

        // show texture by grids
        float curX = 0.0f;
        float curY = 0.0f;
        float interval = 2.0f;
        int borderSize = 1;
        uvX = tileSheet.padding;
        uvY = tileSheet.padding;
        int x = 0;
        int y = 0;

        //
        EditorGUILayout.Space ();
        Rect lastRect = GUILayoutUtility.GetLastRect ();  
        Event e = Event.current;
        rectSelectedGrids.Clear();

        // ======================================================== 
        // draw field 
        // ======================================================== 

        Rect filedRect = new Rect( 30, 
                                   lastRect.yMax,
                                   (tileSheet.tileWidth + interval + 2 * borderSize) * col - interval,
                                   (tileSheet.tileHeight + interval + 2 * borderSize) * row - interval );
        GUI.BeginGroup(filedRect);
            while ( (uvY + tileSheet.tileHeight + tileSheet.padding) <= tileSheet.texture.height ) {
                while ( (uvX + tileSheet.tileWidth + tileSheet.padding) <= tileSheet.texture.width ) {

                    // ======================================================== 
                    // draw tile element 
                    // ======================================================== 

                    Rect rect = new Rect( curX, 
                                          curY, 
                                          tileSheet.tileWidth + 2 * borderSize, 
                                          tileSheet.tileHeight + 2 * borderSize );

                    // draw the texture
                    GUI.BeginGroup( new Rect ( rect.x + 1,
                                               rect.y + 1,
                                               rect.width - 2,
                                               rect.height - 2 ) );
                        Rect cellRect = new Rect( -uvX,
                                                  -uvY,
                                                  tileSheet.texture.width, 
                                                  tileSheet.texture.height );
                        GUI.DrawTexture( cellRect, tileSheet.texture );
                    GUI.EndGroup();

                    uvX = uvX + tileSheet.tileWidth + tileSheet.padding; 
                    curX = curX + tileSheet.tileWidth + interval + 2 * borderSize; 

                    // ======================================================== 
                    // handle events
                    // ======================================================== 

                    //
                    if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
                        if ( rect.Contains( e.mousePosition ) ) {
                            GUIUtility.keyboardControl = -1; // remove any keyboard control

                            inRectSelectState = true;
                            mouseDownPos = e.mousePosition;
                            UpdateSelectRect ();

                            if ( e.command == false && e.control == false ) {
                                selectedGrids.Clear();
                            }

                            e.Use();
                            Repaint();
                        }
                    }

                    //
                    bool selectInRect = false;
                    if ( inRectSelectState ) {
                        if ( exContains2D.RectRect( selectRect, rect ) != 0 ||
                             exIntersection2D.RectRect( selectRect, rect ) )
                        {
                            AddRectSelected(x,y);
                            selectInRect = true;
                        }
                    }

                    // ======================================================== 
                    // draw grid, NOTE: we should handle event first to get if the rect is slected 
                    // ======================================================== 

                    if ( HasSelected ( x, y ) || selectInRect ) {
                        exEditorHelper.DrawRect ( rect,
                                                  new Color ( 0.0f, 0.5f, 1.0f, 0.4f ),
                                                  new Color ( 0.0f, 0.5f, 1.0f, 1.0f ) );
                    }
                    else {
                        exEditorHelper.DrawRect ( rect,
                                                  new Color ( 1.0f, 1.0f, 1.0f, 0.0f ),
                                                  Color.gray );
                    }

                    ++x;
                }

                // step uv
                uvX = tileSheet.padding;
                uvY = uvY + tileSheet.tileHeight + tileSheet.padding; 

                // step pos
                curX = 0.0f;
                curY = curY + tileSheet.tileHeight + interval + 2 * borderSize; 

                x = 0;
                ++y;
            }

            // DISABLE { 
            // // ======================================================== 
            // // draw select rect 
            // // ======================================================== 

            // if ( inRectSelectState && (selectRect.width != 0.0f || selectRect.height != 0.0f) ) {
            //     exEditorHelper.DrawRect( selectRect, new Color( 0.0f, 0.5f, 1.0f, 0.2f ), new Color( 0.0f, 0.5f, 1.0f, 1.0f ) );
            // }
            // } DISABLE end 

            // ======================================================== 
            // handle rect select
            // ======================================================== 

            if ( inRectSelectState ) {
                if ( e.type == EventType.MouseDrag ) {
                    UpdateSelectRect ();
                    Repaint();

                    e.Use();
                }
            }

        GUI.EndGroup();
        GUILayoutUtility.GetRect ( filedRect.width, filedRect.height );

        // ======================================================== 
        // finally 
        // ======================================================== 

        if ( inRectSelectState ) {
            if ( e.type == EventType.MouseUp && e.button == 0 ) {
                inRectSelectState = false;
                ConfirmRectSelection();
                Repaint();

                e.Use();
            }
        }

        // DISABLE: we use eraser button { 

        // if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
        //     if ( filedRect.Contains( e.mousePosition ) == false ) {
        //         selectedGrids.Clear();
        //         Repaint();

        //         e.Use();
        //     }
        // }
        // } DISABLE end 
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
        if ( editTileMap.tileSheet == null )
            return;

        ShowMouseTile(true);
        if ( mouseTile != null )
            mouseTile.transform.position = _pos;

        // DELME { 
        // Handles.color = Color.white;
        // Color faceColor = new Color( 1.0f, 1.0f, 1.0f, 0.2f );
        // Color lineColor = new Color( 0.0f, 0.0f, 0.0f, 1.0f );
        // float width = editTileMap.tileSheet.tileWidth;
        // float height = editTileMap.tileSheet.tileHeight;

        // switch ( editTileMap.plane ) {
        // case exPlane.Plane.XY:
        //     Handles.DrawSolidRectangleWithOutline( new Vector3[] {
        //                                            _pos + new Vector3( 0.0f,   0.0f,   0.0f ), 
        //                                            _pos + new Vector3( 0.0f,   height, 0.0f ),
        //                                            _pos + new Vector3( width,  height, 0.0f ),
        //                                            _pos + new Vector3( width,  0.0f,   0.0f ),
        //                                            _pos + new Vector3( 0.0f,   0.0f,   0.0f )
        //                                            },
        //                                            faceColor, 
        //                                            lineColor );
        //     break;

        // case exPlane.Plane.XZ:
        //     Handles.DrawSolidRectangleWithOutline( new Vector3[] {
        //                                            _pos + new Vector3( 0.0f,   0.0f, 0.0f   ), 
        //                                            _pos + new Vector3( 0.0f,   0.0f, height ),
        //                                            _pos + new Vector3( width,  0.0f, height ),
        //                                            _pos + new Vector3( width,  0.0f, 0.0f   ),
        //                                            _pos + new Vector3( 0.0f,   0.0f, 0.0f   )
        //                                            },
        //                                            faceColor, 
        //                                            lineColor );
        //     break;

        // case exPlane.Plane.ZY:
        //     Handles.DrawSolidRectangleWithOutline( new Vector3[] {
        //                                            _pos + new Vector3( 0.0f,   0.0f, 0.0f  ), 
        //                                            _pos + new Vector3( height, 0.0f, 0.0f  ),
        //                                            _pos + new Vector3( height, 0.0f, width ),
        //                                            _pos + new Vector3( 0.0f,   0.0f, width ),
        //                                            _pos + new Vector3( 0.0f,   0.0f, 0.0f  )
        //                                            },
        //                                            faceColor, 
        //                                            lineColor );
        //     break;
        // }
        // } DELME end 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    bool HasSelected ( int _x, int _y ) {
        int in_id = _x + _y * editTileMap.tileSheet.col;
        foreach ( int id in selectedGrids ) {
            if ( id == in_id ) {
                return true;
            }
        }

        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AddRectSelected ( int _x, int _y ) {
        int in_id = _x + _y * editTileMap.tileSheet.col;

        bool found = false;
        foreach ( int id in rectSelectedGrids ) {
            if ( id == in_id ) {
                found = true;
                break;
            }
        }

        //
        if ( found == false ) {
            rectSelectedGrids.Add(in_id);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateSelectRect () {
        float x = 0;
        float y = 0;
        float width = 0;
        float height = 0;
        Vector2 curMousePos = Event.current.mousePosition;

        if ( mouseDownPos.x < curMousePos.x ) {
            x = mouseDownPos.x;
            width = curMousePos.x - mouseDownPos.x;
        }
        else {
            x = curMousePos.x;
            width = mouseDownPos.x - curMousePos.x;
        }
        if ( mouseDownPos.y < curMousePos.y ) {
            y = mouseDownPos.y;
            height = curMousePos.y - mouseDownPos.y;
        }
        else {
            y = curMousePos.y;
            height = mouseDownPos.y - curMousePos.y;
        }

        selectRect = new Rect( x, y, width, height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ConfirmRectSelection () {
        foreach ( int id in rectSelectedGrids ) {
            if ( selectedGrids.IndexOf(id) == -1 )
                selectedGrids.Add(id);
        }
        rectSelectedGrids.Clear();

        //
        if ( mouseTile != null ) {
            exTileMapRenderer tm = mouseTile.AddComponent<exTileMapRenderer>();
            if ( selectedGrids.Count > 0 )
                tm.SetTile ( 0, 0, selectedGrids[0] );
        }
    }
}
