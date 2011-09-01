// ======================================================================================
// File         : exTileMap.cs
// Author       : Wu Jie 
// Last Change  : 08/20/2011 | 14:54:57 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[AddComponentMenu("ex2D TileMap/Tile Map")]
public class exTileMap : exPlane {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    [SerializeField] protected exTileInfo tileInfo_;
    public exTileInfo tileInfo { 
        get { return tileInfo_; } 
        set { 
            if ( tileInfo_ != value ) {
                tileInfo_ = value;
                // renderer
                if ( renderer ) {
                    if ( tileInfo_ ) {
                        renderer.sharedMaterial = tileInfo_.material;
                    }
                    else {
                        renderer.sharedMaterial = null;
                        GetComponent<MeshFilter>().sharedMesh = null; 
                    }
                }
            }
        }
    }

    [SerializeField] protected int row_ = 50;
    public int row { 
        get { return row_; } 
        // NOTE: we not allow run time changes
    }

    [SerializeField] protected int col_ = 50;
    public int col { 
        get { return col_; } 
        // NOTE: we not allow run time changes
    }

    [SerializeField] protected int tileWidth_ = 32;
    public int tileWidth { 
        get { return tileWidth_; } 
        set { 
            if ( tileWidth_ != value ) {
                tileWidth_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    [SerializeField] protected int tileHeight_ = 32;
    public int tileHeight { 
        get { return tileHeight_; } 
        set { 
            if ( tileHeight_ != value ) {
                tileHeight_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    [SerializeField] protected Color color_ = Color.white;
    public Color color { 
        get { return color_; } 
        set {
            if ( color_ != value ) {
                color_ = value;
                updateFlags |= UpdateFlags.Color;
            }
        }
    }

    // editor properties
    public bool editorShowGrid = true;

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Resize ( int _row, int _col ) {
        row_ = _row;
        col_ = _col;

        // TODO: runtime and editor { 
        // ForceUpdateMesh( newMesh );
        // } TODO end 
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateMesh ( Mesh _mesh ) {

        // ======================================================== 
        // Update Vertex
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Vertex) != 0 ) {
            int vertexCount = col_ * row_ * 4;
            Vector3[] vertices  = new Vector3[vertexCount];

            // init
            float width = col_ * tileWidth_;
            float height = row_ * tileHeight_;
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            float offsetX = 0.0f;
            float offsetY = 0.0f;
            float curX = 0.0f;
            float curY = 0.0f;

            //
            switch ( anchor ) {
            case Anchor.TopLeft     : offsetX = -halfWidth;   offsetY = -halfHeight;  break;
            case Anchor.TopCenter   : offsetX = 0.0f;         offsetY = -halfHeight;  break;
            case Anchor.TopRight    : offsetX = halfWidth;    offsetY = -halfHeight;  break;

            case Anchor.MidLeft     : offsetX = -halfWidth;   offsetY = 0.0f;         break;
            case Anchor.MidCenter   : offsetX = 0.0f;         offsetY = 0.0f;         break;
            case Anchor.MidRight    : offsetX = halfWidth;    offsetY = 0.0f;         break;

            case Anchor.BotLeft     : offsetX = -halfWidth;   offsetY = halfHeight;   break;
            case Anchor.BotCenter   : offsetX = 0.0f;         offsetY = halfHeight;   break;
            case Anchor.BotRight    : offsetX = halfWidth;    offsetY = halfHeight;   break;

            default                 : offsetX = 0.0f;         offsetY = 0.0f;         break;
            }

            //
            for ( int rr = 0; rr < row_; ++rr ) {
                for ( int cc = 0; cc < col_; ++cc ) {
                    int i = rr * col_ + cc;
                    int vert_id = 4 * i;

                    // build vertices & normals
                    for ( int r = 0; r < 2; ++r ) {
                        for ( int c = 0; c < 2; ++c ) {
                            int j = r * 2 + c;
                            float x = curX - halfWidth + c * tileInfo_.tileWidth;
                            float y = -curY + halfHeight - r * tileInfo_.tileHeight;

                            // build vertices and normals
                            switch ( plane ) {
                            case Plane.XY:
                                vertices[vert_id+j] = new Vector3( x - offsetX, y + offsetY, 0.0f );
                                break;
                            case Plane.XZ:
                                vertices[vert_id+j] = new Vector3( x - offsetX, 0.0f, y + offsetY );
                                break;
                            case Plane.ZY:
                                vertices[vert_id+j] = new Vector3( 0.0f, y + offsetY, x - offsetX );
                                break;
                            }
                        }
                    }
                    curX = curX + tileWidth_;
                }
                curX = 0.0f;
                curY = curY + tileHeight_;
            }

            //
            _mesh.vertices = vertices;
        }

        // ======================================================== 
        // Update Color
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Color) != 0 ) {
            int vertexCount = col_ * row_ * 4;
            Color[] colors = new Color[vertexCount];
            for ( int i = 0; i < vertexCount; ++i ) {
                colors[i] = color_;
            }
            _mesh.colors = colors;
        }

        // ======================================================== 
        // Update Index
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Index) != 0 ) {
            int indexCount = col_ * row_ * 6;
            int[] indices = new int[indexCount];

            for ( int cc = 0; cc < col_; ++cc ) {
                for ( int rr = 0; rr < row_; ++rr ) {
                    int i = rr * col_ + cc;
                    int vert_id = 4 * i;
                    int idx_id = 6 * i;

                    // build indices
                    indices[idx_id + 0] = vert_id + 0;
                    indices[idx_id + 1] = vert_id + 1;
                    indices[idx_id + 2] = vert_id + 2;
                    indices[idx_id + 3] = vert_id + 2;
                    indices[idx_id + 4] = vert_id + 1;
                    indices[idx_id + 5] = vert_id + 3;
                }
            }

            //
            _mesh.triangles = indices; 
        }

        // NOTE: though we set updateFlags to None at exPlane::LateUpdate, 
        //       the Editor still need this or it will caused editor keep dirty
        updateFlags = UpdateFlags.None;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ForceUpdateMesh ( Mesh _mesh ) {
        // pre check mesh
        if ( _mesh == null )
            return;

        _mesh.Clear();
        updateFlags = UpdateFlags.Index | UpdateFlags.Vertex | UpdateFlags.Color;
        UpdateMesh( _mesh );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void InternalUpdate () {
        if ( meshFilter != null && 
             meshFilter.sharedMesh != null ) 
        {
            UpdateMesh (meshFilter.sharedMesh);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDrawGizmos () {
        if ( editorShowGrid == false )
            return;

        // init
        float width = col_ * tileWidth_;
        float height = row_ * tileHeight_;
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        // get offset by anchor
        switch ( anchor ) {
        case Anchor.TopLeft     : offsetX = -halfWidth;   offsetY = -halfHeight;  break;
        case Anchor.TopCenter   : offsetX = 0.0f;         offsetY = -halfHeight;  break;
        case Anchor.TopRight    : offsetX = halfWidth;    offsetY = -halfHeight;  break;

        case Anchor.MidLeft     : offsetX = -halfWidth;   offsetY = 0.0f;         break;
        case Anchor.MidCenter   : offsetX = 0.0f;         offsetY = 0.0f;         break;
        case Anchor.MidRight    : offsetX = halfWidth;    offsetY = 0.0f;         break;

        case Anchor.BotLeft     : offsetX = -halfWidth;   offsetY = halfHeight;   break;
        case Anchor.BotCenter   : offsetX = 0.0f;         offsetY = halfHeight;   break;
        case Anchor.BotRight    : offsetX = halfWidth;    offsetY = halfHeight;   break;

        default                 : offsetX = 0.0f;         offsetY = 0.0f;         break;
        }

        // draw grid by plane
        Color old = Gizmos.color;
        Gizmos.color = Color.gray;
        switch ( plane ) {
        case Plane.XY:
            for ( int i = 0; i <= col_; ++i ) {
                float x = -halfWidth + i * tileWidth_ - offsetX;
                Gizmos.DrawLine ( transform.position + new Vector3( x, -halfHeight + offsetY, 0.0f ) , 
                                  transform.position + new Vector3( x,  halfHeight + offsetY, 0.0f ) );
            }
            for ( int i = 0; i <= row_; ++i ) {
                float y = halfHeight - i * tileHeight_ + offsetY;
                Gizmos.DrawLine ( transform.position + new Vector3( -halfWidth - offsetX, y, 0.0f ) , 
                                  transform.position + new Vector3(  halfWidth - offsetX, y, 0.0f ) );
            }
            break;

        case Plane.XZ:
            for ( int i = 0; i <= col_; ++i ) {
                float x = -halfWidth + i * tileWidth_ - offsetX;
                Gizmos.DrawLine ( transform.position + new Vector3( x, 0.0f, -halfHeight + offsetY ) , 
                                  transform.position + new Vector3( x, 0.0f,  halfHeight + offsetY ) );
            }
            for ( int i = 0; i <= row_; ++i ) {
                float y = halfHeight - i * tileHeight_ + offsetY;
                Gizmos.DrawLine ( transform.position + new Vector3( -halfWidth - offsetX, 0.0f, y ) , 
                                  transform.position + new Vector3(  halfWidth - offsetX, 0.0f, y ) );
            }
            break;

        case Plane.ZY:
            for ( int i = 0; i <= col_; ++i ) {
                float x = -halfWidth + i * tileWidth_ - offsetX;
                Gizmos.DrawLine ( transform.position + new Vector3( 0.0f, -halfHeight + offsetY, x ) , 
                                  transform.position + new Vector3( 0.0f,  halfHeight + offsetY, x ) );
            }
            for ( int i = 0; i <= row_; ++i ) {
                float y = halfHeight - i * tileHeight_ + offsetY;
                Gizmos.DrawLine ( transform.position + new Vector3( 0.0f, y, -halfWidth - offsetX ) , 
                                  transform.position + new Vector3( 0.0f, y,  halfWidth - offsetX ) );
            }
            break;
        }
        Gizmos.color = old;
    }
}
