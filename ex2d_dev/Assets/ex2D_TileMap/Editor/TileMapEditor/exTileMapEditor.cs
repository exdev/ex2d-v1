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
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// exTileMapEditor
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exTileMap))]
partial class exTileMapEditor : exPlaneEditor {

    struct GridID {
        public int x;
        public int y;

        public GridID ( int _x, int _y ) { x = _x; y = _y; }
        public static bool operator == ( GridID _a, GridID _b ) { return _a.Equals(_b); }
        public static bool operator != ( GridID _a, GridID _b ) { return !_a.Equals(_b); }
        public override int GetHashCode() { return x ^ y; }
        public override bool Equals ( object _obj ) {
            if ( !(_obj is GridID) )
                return false;
            return Equals((GridID)_obj);
        }
        public bool Equals ( GridID _other ) {
            if ( x != _other.x || y != _other.y ) {
                return false;
            }
            return true;
        }
    }

    private exTileMap editTileMap;
    private Plane plane;
    private Vector2 mousePos = Vector2.zero;
    private Vector3 startOffset = Vector3.zero;
    private Rect tileMapRect;
    private bool isMouseInside = false;
    private GameObject mouseTile = null;

    private bool inRectSelectState = false;
    private List<GridID> selectedGrids = new List<GridID>();
    private List<GridID> rectSelectedGrids = new List<GridID>();
    private Rect selectRect = new Rect( 0, 0, 1, 1 );
    private Vector2 mouseDownPos = Vector2.zero;

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

    void OnDisable () {
        if ( mouseTile != null ) {
            Object.DestroyImmediate(mouseTile.GetComponent<MeshFilter>().sharedMesh,true); 
            Object.DestroyImmediate(mouseTile,true); 
        }
        mouseTile = null;
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
        editTileMap.meshFilter = editTileMap.GetComponent<MeshFilter>();

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
        // TileInfo Field 
        // ======================================================== 

        TileInfoField ( editTileMap.tileInfo ); 
        if ( editTileMap.tileInfo != null && editTileMap.meshFilter.sharedMesh == null ) {
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

        mousePos = e.mousePosition;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch ( e.type ) {
        case EventType.mouseMove:
            HandleUtility.Repaint();
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

    void TileInfoField ( exTileInfo _tileInfo ) {
        GUILayout.BeginHorizontal();
            editTileMap.tileInfo 
                = (exTileInfo)EditorGUILayout.ObjectField( "Tile Info"
                                                           , editTileMap.tileInfo
                                                           , typeof(exTileInfo)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                           , false
#endif
                                                         );
            if ( GUILayout.Button("Edit...", GUILayout.Width(40), GUILayout.Height(15) ) ) {
                exTileInfoEditor editor = exTileInfoEditor.NewWindow();
                editor.Edit(editTileMap.tileInfo);
            }
        GUILayout.EndHorizontal();

        if ( editTileMap.tileInfo == null )
            return;

        exTileInfo tileInfo = editTileMap.tileInfo;
        int col = 0;
        int row = 0;
        int uvX = tileInfo.padding;
        int uvY = tileInfo.padding;

        // count the col
        while ( (uvX + tileInfo.tileWidth + tileInfo.padding) <= tileInfo.texture.width ) {
            uvX = uvX + tileInfo.tileWidth + tileInfo.padding; 
            ++col;
        }

        // count the row
        while ( (uvY + tileInfo.tileHeight + tileInfo.padding) <= tileInfo.texture.height ) {
            uvY = uvY + tileInfo.tileHeight + tileInfo.padding; 
            ++row;
        }

        // show texture by grids
        float curX = 0.0f;
        float curY = 0.0f;
        float interval = 2.0f;
        int borderSize = 1;
        uvX = tileInfo.padding;
        uvY = tileInfo.padding;
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
                                   (tileInfo.tileWidth + interval + 2 * borderSize) * col - interval,
                                   (tileInfo.tileHeight + interval + 2 * borderSize) * row - interval );
        GUI.BeginGroup(filedRect);
            while ( (uvY + tileInfo.tileHeight + tileInfo.padding) <= tileInfo.texture.height ) {
                while ( (uvX + tileInfo.tileWidth + tileInfo.padding) <= tileInfo.texture.width ) {

                    // ======================================================== 
                    // draw tile element 
                    // ======================================================== 

                    Rect rect = new Rect( curX, 
                                          curY, 
                                          tileInfo.tileWidth + 2 * borderSize, 
                                          tileInfo.tileHeight + 2 * borderSize );

                    // draw the texture
                    GUI.BeginGroup( new Rect ( rect.x + 1,
                                               rect.y + 1,
                                               rect.width - 2,
                                               rect.height - 2 ) );
                        Rect cellRect = new Rect( -uvX,
                                                  -uvY,
                                                  tileInfo.texture.width, 
                                                  tileInfo.texture.height );
                        GUI.DrawTexture( cellRect, tileInfo.texture );
                    GUI.EndGroup();

                    uvX = uvX + tileInfo.tileWidth + tileInfo.padding; 
                    curX = curX + tileInfo.tileWidth + interval + 2 * borderSize; 

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
                                                  new Color ( 1.0f, 1.0f, 1.0f, 0.2f ),
                                                  new Color ( 0.0f, 1.0f, 0.5f, 1.0f ) );
                    }
                    else {
                        exEditorHelper.DrawRect ( rect,
                                                  new Color ( 1.0f, 1.0f, 1.0f, 0.0f ),
                                                  Color.gray );
                    }

                    ++x;
                }

                // step uv
                uvX = tileInfo.padding;
                uvY = uvY + tileInfo.tileHeight + tileInfo.padding; 

                // step pos
                curX = 0.0f;
                curY = curY + tileInfo.tileHeight + interval + 2 * borderSize; 

                x = 0;
                ++y;
            }

            // ======================================================== 
            // draw select rect 
            // ======================================================== 

            if ( inRectSelectState && (selectRect.width != 0.0f || selectRect.height != 0.0f) ) {
                exEditorHelper.DrawRect( selectRect, new Color( 0.0f, 0.5f, 1.0f, 0.2f ), new Color( 0.0f, 0.5f, 1.0f, 1.0f ) );
            }

            // ======================================================== 
            // handle rect select
            // ======================================================== 

            if ( inRectSelectState ) {
                if ( e.type == EventType.MouseDrag ) {
                    UpdateSelectRect ();
                    Repaint();

                    e.Use();
                }
                else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                    inRectSelectState = false;
                    ConfirmRectSelection();
                    Repaint();

                    e.Use();
                }
            }

        GUI.EndGroup();
        GUILayoutUtility.GetRect ( filedRect.width, filedRect.height );

        // DISABLE: we use eraser button { 
        // // ======================================================== 
        // // finally 
        // // ======================================================== 

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
        if ( editTileMap.tileInfo == null )
            return;

        if ( mouseTile == null ) {
            mouseTile = new GameObject(".temp_mouse_tile");
            // mouseTile.hideFlags = HideFlags.HideAndDontSave;
            // mouseTile.hideFlags = HideFlags.HideInHierarchy;
            mouseTile.hideFlags = HideFlags.DontSave;
            // DELME { 
            // mouseTile.transform.parent = editTileMap.transform;
            // EditorUtility.SetSelectedWireframeHidden(mouseTile.renderer, true);
            // } DELME end 

            exTileMap tm = mouseTile.AddComponent<exTileMap>();
            tm.tileInfo = editTileMap.tileInfo;
            tm.tileWidth = editTileMap.tileInfo.tileWidth;
            tm.tileHeight = editTileMap.tileInfo.tileHeight;
            tm.Resize(1,1);
            tm.Build();
            tm.meshFilter.sharedMesh.hideFlags = HideFlags.DontSave;
        }

        if ( mouseTile != null )
            mouseTile.transform.position = _pos;

        // DELME { 
        // Handles.color = Color.white;
        // Color faceColor = new Color( 1.0f, 1.0f, 1.0f, 0.2f );
        // Color lineColor = new Color( 0.0f, 0.0f, 0.0f, 1.0f );
        // float width = editTileMap.tileInfo.tileWidth;
        // float height = editTileMap.tileInfo.tileHeight;

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
        foreach ( GridID gid in selectedGrids ) {
            if ( gid.x == _x && gid.y == _y ) {
                return true;
            }
        }

        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AddRectSelected ( int _x, int _y ) {
        bool found = false;
        foreach ( GridID gid in rectSelectedGrids ) {
            if ( gid.x == _x && gid.y == _y ) {
                found = true;
                break;
            }
        }

        //
        if ( found == false ) {
            rectSelectedGrids.Add( new GridID(_x,_y) );
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
        foreach ( GridID gid in rectSelectedGrids ) {
            if ( HasSelected( gid.x, gid.y ) == false )
                selectedGrids.Add(gid);
        }
        rectSelectedGrids.Clear();
    }
}
