// ======================================================================================
// File         : exTileMapRenderer.cs
// Author       : Wu Jie 
// Last Change  : 09/16/2011 | 09:57:14 AM | Friday,September
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
[AddComponentMenu("ex2D TileMap/Tile Map Renderer")]
public class exTileMapRenderer : exPlane {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    [SerializeField] protected exTileMap tileMap_ = null;
    /// the tilemap assets referenced in this component.
    /// 
    /// \sa exTileMapRenderer.CreateTileMap( exTileMap )
    // ------------------------------------------------------------------ 

    public exTileMap tileMap { get { return tileMap_; } }

    // ------------------------------------------------------------------ 
    [SerializeField] protected Color color_ = Color.white;
    /// the vertex color of the tilemap
    // ------------------------------------------------------------------ 

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

    override protected void Awake () {
        base.Awake();

        if ( tileMap_ != null ||
             ( renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null ) ) 
        {
            meshFilter.mesh = new Mesh();
            ForceUpdateMesh( meshFilter.sharedMesh );
        }
    }

    // ------------------------------------------------------------------ 
    /// Clear the tilemap
    // ------------------------------------------------------------------ 

    public void Clear () {
        tileMap_ = null;

        if ( renderer != null )
            renderer.sharedMaterial = null;

        if ( meshFilter ) {
            DestroyImmediate( meshFilter_.sharedMesh, true );
            meshFilter_.sharedMesh = null;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateMesh ( Mesh _mesh ) {
        int gridCount = 0;
        if ( updateFlags != UpdateFlags.None ) {
            foreach ( int id in tileMap_.grids ) {
                if ( id != -1 ) {
                    ++gridCount;
                }
            }
        }

        // ======================================================== 
        // Update Vertex
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Vertex) != 0 ) {
            int vertexCount = gridCount * 4;
            Vector3[] vertices  = new Vector3[vertexCount];

            // init
            float width = tileMap_.col * tileMap_.tileWidth;
            float height = tileMap_.row * tileMap_.tileHeight;
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            float offsetX = 0.0f;
            float offsetY = 0.0f;

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
            int ii = 0;
            for ( int rr = 0; rr < tileMap_.row; ++rr ) {
                for ( int cc = 0; cc < tileMap_.col; ++cc ) {
                    //
                    int i = rr * tileMap_.col + cc;
                    if ( tileMap_.grids[i] == -1 )
                        continue;

                    //
                    float curX = cc * tileMap_.tileWidth;
                    float curY = rr * tileMap_.tileHeight;

                    // build vertices & normals
                    for ( int r = 0; r < 2; ++r ) {
                        for ( int c = 0; c < 2; ++c ) {
                            int j = r * 2 + c;
                            float x = curX - halfWidth + c * tileMap_.tileSheet.tileWidth - tileMap_.tileOffsetX;
                            float y = -curY + halfHeight - r * tileMap_.tileSheet.tileHeight + (tileMap_.tileSheet.tileHeight - tileMap_.tileHeight) + tileMap_.tileOffsetY; // last thing adjust mesh start from left-bottom

                            // build vertices and normals
                            switch ( plane ) {
                            case Plane.XY:
                                vertices[4*ii+j] = new Vector3( x - offsetX, y + offsetY, 0.0f );
                                break;
                            case Plane.XZ:
                                vertices[4*ii+j] = new Vector3( x - offsetX, 0.0f, y + offsetY );
                                break;
                            case Plane.ZY:
                                vertices[4*ii+j] = new Vector3( 0.0f, y + offsetY, x - offsetX );
                                break;
                            }
                        }
                    }
                    ++ii;
                }
            }

            //
            _mesh.vertices = vertices;
        }

        // ======================================================== 
        // Update UV 
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.UV) != 0 ) {
            int vertexCount = gridCount * 4;
            Vector2[] uvs  = new Vector2[vertexCount];
            int ii = 0;

            for ( int i = 0; i < tileMap_.grids.Length; ++i ) {
                if ( tileMap_.grids[i] == -1 )
                    continue;

                // get uv
                int sheetID = tileMap_.grids[i];
                Rect uv = tileMap_.tileSheet.GetTileUV (sheetID);
                float xStart  = uv.x;
                float yStart  = uv.y;
                float xEnd    = uv.xMax;
                float yEnd    = uv.yMax;

                //
                uvs[4*ii+0] = new Vector2 ( xStart,  yEnd );
                uvs[4*ii+1] = new Vector2 ( xEnd,    yEnd );
                uvs[4*ii+2] = new Vector2 ( xStart,  yStart );
                uvs[4*ii+3] = new Vector2 ( xEnd,    yStart );
                ++ii;
            }
            _mesh.uv = uvs;
        }

        // ======================================================== 
        // Update Color
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Color) != 0 ) {
            //
            int vertexCount = gridCount * 4;
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
            //
            int indexCount = gridCount * 6;
            int[] indices = new int[indexCount];

            //
            for ( int i = 0; i < gridCount; ++i ) {
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

            //
            _mesh.triangles = indices; 
        }

        // // TODO: update bounding rect ( for seeing )

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
        updateFlags = UpdateFlags.Index | UpdateFlags.Vertex | UpdateFlags.Color | UpdateFlags.UV;
        UpdateMesh( _mesh );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void InternalUpdate () {
        if ( meshFilter ) {
            if ( meshFilter_.sharedMesh != null ) {
                UpdateMesh (meshFilter_.sharedMesh);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void CreateTileMap ( exTileMap _tileMap ) {
        Clear ();
        tileMap_ = _tileMap;

        // if we have tilemap
        if ( tileMap_ != null ) {
            if ( tileMap_.tileSheet ) {
                renderer.sharedMaterial = tileMap_.tileSheet.material;
            }

            meshFilter_.mesh = new Mesh();
            ForceUpdateMesh( meshFilter_.sharedMesh );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDrawGizmos () {
        if ( tileMap_ == null || editorShowGrid == false )
            return;

        // init
        float width = tileMap_.col * tileMap_.tileWidth;
        float height = tileMap_.row * tileMap_.tileHeight;
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
            for ( int i = 0; i <= tileMap_.col; ++i ) {
                float x = -halfWidth + i * tileMap_.tileWidth - offsetX;
                Gizmos.DrawLine ( transform.position + new Vector3( x, -halfHeight + offsetY, 0.0f ) , 
                                  transform.position + new Vector3( x,  halfHeight + offsetY, 0.0f ) );
            }
            for ( int i = 0; i <= tileMap_.row; ++i ) {
                float y = halfHeight - i * tileMap_.tileHeight + offsetY;
                Gizmos.DrawLine ( transform.position + new Vector3( -halfWidth - offsetX, y, 0.0f ) , 
                                  transform.position + new Vector3(  halfWidth - offsetX, y, 0.0f ) );
            }
            break;

        case Plane.XZ:
            for ( int i = 0; i <= tileMap_.col; ++i ) {
                float x = -halfWidth + i * tileMap_.tileWidth - offsetX;
                Gizmos.DrawLine ( transform.position + new Vector3( x, 0.0f, -halfHeight + offsetY ) , 
                                  transform.position + new Vector3( x, 0.0f,  halfHeight + offsetY ) );
            }
            for ( int i = 0; i <= tileMap_.row; ++i ) {
                float y = halfHeight - i * tileMap_.tileHeight + offsetY;
                Gizmos.DrawLine ( transform.position + new Vector3( -halfWidth - offsetX, 0.0f, y ) , 
                                  transform.position + new Vector3(  halfWidth - offsetX, 0.0f, y ) );
            }
            break;

        case Plane.ZY:
            for ( int i = 0; i <= tileMap_.col; ++i ) {
                float x = -halfWidth + i * tileMap_.tileWidth - offsetX;
                Gizmos.DrawLine ( transform.position + new Vector3( 0.0f, -halfHeight + offsetY, x ) , 
                                  transform.position + new Vector3( 0.0f,  halfHeight + offsetY, x ) );
            }
            for ( int i = 0; i <= tileMap_.row; ++i ) {
                float y = halfHeight - i * tileMap_.tileHeight + offsetY;
                Gizmos.DrawLine ( transform.position + new Vector3( 0.0f, y, -halfWidth - offsetX ) , 
                                  transform.position + new Vector3( 0.0f, y,  halfWidth - offsetX ) );
            }
            break;
        }
        Gizmos.color = old;
    }
}
