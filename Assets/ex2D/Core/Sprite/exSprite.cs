// ======================================================================================
// File         : exSprite.cs
// Author       : Wu Jie 
// Last Change  : 06/04/2011 | 23:44:11 PM | Saturday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

///////////////////////////////////////////////////////////////////////////////
// defines
// NOTE: without ExecuteInEditMode, we can't not drag and create mesh in the scene 
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[AddComponentMenu("ex2D Sprite/Sprite")]
public class exSprite : exSpriteBase {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public string textureGUID = ""; 
    public bool trimTexture = true; // only affect when exSprite is not in atlas
    public Rect trimUV = new Rect(0,0,1,1); // only affect when exSprite is not in atlas

    [SerializeField] protected bool useTextureOffset_ = true;
    public bool useTextureOffset {
        get { return useTextureOffset_; }
        set {
            if ( useTextureOffset_ != value ) {
                useTextureOffset_ = value;
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

    [SerializeField] protected bool customSize_ = false;
    public bool customSize {
        get { return customSize_; }
        set {
            if ( customSize_ != value ) {
                customSize_ = value;
                if ( customSize_ == false) {
                    float newWidth = 0.0f;
                    float newHeight = 0.0f;

                    if ( useAtlas ) {
                        exAtlas.Element el = atlas_.elements[index_];
                        newWidth = el.coords.width * atlas_.texture.width;
                        newHeight = el.coords.height * atlas_.texture.height;
                        if ( el.rotated ) {
                            float tmp = newWidth;
                            newWidth = newHeight;
                            newHeight = tmp;
                        } 
                    }
                    else {
                        Texture texture = renderer.sharedMaterial.mainTexture;
                        newWidth = trimUV.width * texture.width;
                        newHeight = trimUV.height * texture.height;
                    }

                    if ( newWidth != width_ || newHeight != height_ ) {
                        width_ = newWidth;
                        height_ = newHeight;
                        updateFlags |= UpdateFlags.Vertex;
                    }
                }
            }
        }
    }

    [SerializeField] protected float width_ = 1.0f;
    public float width {
        get { return width_; }
        set {
            if ( width_ != value ) {
                width_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    [SerializeField] protected float height_ = 1.0f;
    public float height {
        get { return height_; }
        set {
            if ( height_ != value ) {
                height_ = value;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }

    [SerializeField] protected exAtlas atlas_ = null;
    public exAtlas atlas { get { return atlas_; } }

    [SerializeField] protected int index_ = -1;
    public int index { get { return index_; } }

    public bool useAtlas { 
        get { 
            return ( atlas_ != null 
                     && atlas_.elements != null
                     && index_ >= 0
                     && index_ < atlas_.elements.Length ); 
        } 
    }

    ///////////////////////////////////////////////////////////////////////////////
    // non-serialize
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public exSpriteAnimation spanim;

    ///////////////////////////////////////////////////////////////////////////////
    // mesh building functions
    ///////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [ContextMenu ("Rebuild")]
    void Rebuild () {

        Texture2D texture = null;
        if ( string.IsNullOrEmpty(textureGUID) == false ) {
            string texturePath = AssetDatabase.GUIDToAssetPath(textureGUID);
            texture = (Texture2D)AssetDatabase.LoadAssetAtPath( texturePath, typeof(Texture2D) );
        }

        this.Build (texture);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Build ( Texture2D _texture = null ) {
        EditorUtility.SetDirty (this);

        //
        if ( atlas == null && _texture == null ) {
            GetComponent<MeshFilter>().sharedMesh = null; 
            renderer.sharedMaterial = null;
            return;
        }

        // NOTE: it is possible user duplicate an GameObject, 
        //       if we directly change the mesh, the original one will changed either.
        Mesh newMesh = new Mesh();
        newMesh.Clear();

        // build vertices, normals, uvs and colors.
        ForceUpdateMesh( newMesh );

        // set the new mesh in MeshFilter
        GetComponent<MeshFilter>().sharedMesh = newMesh; 

        // if we have mesh collider, update it.
        MeshCollider meshCol = GetComponent<MeshCollider>();
        if ( meshCol )
            meshCol.sharedMesh = newMesh;

        // if we have box collider, update it.
        BoxCollider boxCol = GetComponent<BoxCollider>();
        if ( boxCol ) {
            Vector3 size = newMesh.bounds.size;
            boxCol.center = newMesh.bounds.center;

            switch ( plane ) {
            case exSprite.Plane.XY:
                boxCol.size = new Vector3( size.x, size.y, 0.2f );
                break;
            case exSprite.Plane.XZ:
                boxCol.size = new Vector3( size.x, 0.2f, size.z );
                break;
            case exSprite.Plane.ZY:
                boxCol.size = new Vector3( 0.2f, size.y, size.z );
                break;
            }
        }

        // set a texture to it
        if ( atlas != null ) {
            renderer.sharedMaterial = atlas.material;
        }
        else if ( _texture != null ) {
            string texturePath = AssetDatabase.GetAssetPath(_texture);

            // load material from "texture_path/Materials/texture_name.mat"
            string materialDirectory = Path.Combine( Path.GetDirectoryName(texturePath), "Materials" );
            string materialPath = Path.Combine( materialDirectory, _texture.name + ".mat" );
            Material newMaterial = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));

            // if not found, load material from "texture_path/texture_name.mat"
            if ( newMaterial == null ) {
                newMaterial = (Material)AssetDatabase.LoadAssetAtPath( Path.Combine( Path.GetDirectoryName(texturePath), 
                                                                                     Path.GetFileNameWithoutExtension(texturePath) + ".mat" ), 
                                                                       typeof(Material) );
            }

            if ( newMaterial == null ) {
                // check if directory exists, if not, create one.
                DirectoryInfo info = new DirectoryInfo(materialDirectory);
                if ( info.Exists == false )
                    AssetDatabase.CreateFolder ( texturePath, "Materials" );

                // create temp materal
                newMaterial = new Material( Shader.Find("ex2D/Alpha Blended") );
                newMaterial.mainTexture = _texture;

                AssetDatabase.CreateAsset(newMaterial, materialPath);
                AssetDatabase.Refresh();
            }

            // assign it
            renderer.sharedMaterial = newMaterial;
        }

        EditorUtility.UnloadUnusedAssets();
    }
#endif

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void UpdateMesh ( Mesh _mesh ) {

        exAtlas.Element el = null;
        if ( useAtlas )
            el = atlas_.elements[index_];

        // ======================================================== 
        // get clip info first
        // ======================================================== 

        float clipLeft   = 0.0f; 
        float clipRight  = 0.0f; 
        float clipTop    = 0.0f; 
        float clipBottom = 0.0f;

        if ( clipInfo_.clipped ) {
            if ( scale_.x >= 0.0f ) {
                clipLeft = clipInfo_.left;
                clipRight = clipInfo_.right;
            }
            else {
                clipLeft = clipInfo_.right;
                clipRight = clipInfo_.left;
            }

            if ( scale_.y >= 0.0f ) {
                clipTop = clipInfo_.top;
                clipBottom = clipInfo_.bottom;
            }
            else{
                clipTop = clipInfo_.bottom;
                clipBottom = clipInfo_.top;
            }
        }

        // ======================================================== 
        // Update Vertex
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Vertex) != 0 ) {

            // init
            float halfWidth = width_ * scale_.x * 0.5f;
            float halfHeight = height_ * scale_.y * 0.5f;
            float offsetX = 0.0f;
            float offsetY = 0.0f;

            Vector3[] vertices = new Vector3[4];
            // Vector3[] normals = new Vector3[4];

            // calculate anchor offset
            if ( useTextureOffset ) {
                // get original width and height
                float originalWidth = 0.0f; 
                float originalHeight = 0.0f; 
                Rect trimRect = new Rect ( 0, 0, 1, 1 );

                if ( el != null ) {
                    originalWidth   = el.originalWidth * scale_.x;
                    originalHeight  = el.originalHeight * scale_.y;
                    trimRect        = new Rect( el.trimRect.x * scale_.x, 
                                                el.trimRect.y * scale_.y, 
                                                el.trimRect.width * scale_.x, 
                                                el.trimRect.height * scale_.y );
                }
                else {
                    if ( renderer.sharedMaterial != null ) {
                        Texture texture = renderer.sharedMaterial.mainTexture;
                        originalWidth   = texture.width * scale_.x;
                        originalHeight  = texture.height * scale_.y;
                        trimRect = new Rect ( trimUV.x * originalWidth,
                                              (1.0f - trimUV.height - trimUV.y ) * originalHeight,
                                              trimUV.width * originalWidth, 
                                              trimUV.height * originalHeight );
                    }
                }

                switch ( anchor ) {
                    //
                case Anchor.TopLeft:
                    offsetX = -halfWidth - trimRect.x;
                    offsetY = -halfHeight - trimRect.y;
                    break;

                case Anchor.TopCenter:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = -halfHeight - trimRect.y;
                    break;

                case Anchor.TopRight:    
                    offsetX = halfWidth + originalWidth - trimRect.xMax;
                    offsetY = -halfHeight - trimRect.y;
                    break;
                    
                    //
                case Anchor.MidLeft:
                    offsetX = -halfWidth - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                case Anchor.MidCenter:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                case Anchor.MidRight:
                    offsetX = halfWidth + originalWidth - trimRect.xMax;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;

                    //
                case Anchor.BotLeft:
                    offsetX = -halfWidth - trimRect.x;
                    offsetY = halfHeight + originalHeight - trimRect.yMax;
                    break;

                case Anchor.BotCenter: 
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = halfHeight + originalHeight - trimRect.yMax;
                    break;

                case Anchor.BotRight:
                    offsetX = halfWidth + originalWidth - trimRect.xMax;
                    offsetY = halfHeight + originalHeight - trimRect.yMax;
                    break;

                default:
                    offsetX = (originalWidth - trimRect.width) * 0.5f - trimRect.x;
                    offsetY = (originalHeight - trimRect.height) * 0.5f - trimRect.y;
                    break;
                }
            }
            else {
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
            }

            //
            float xMinClip = scale_.x * width  * ( -0.5f + clipLeft   );
            float xMaxClip = scale_.x * width  * (  0.5f - clipRight  );
            float yMinClip = scale_.y * height * ( -0.5f + clipTop    );
            float yMaxClip = scale_.y * height * (  0.5f - clipBottom );

            // build vertices & normals
            for ( int r = 0; r < 2; ++r ) {
                for ( int c = 0; c < 2; ++c ) {
                    int i = r * 2 + c;

                    // calculate the base pos
                    float x = -halfWidth  + c * width_  * scale_.x;
                    float y =  halfHeight - r * height_ * scale_.y;

                    // do clip
                    if ( clipInfo_.clipped ) {
                        if ( x <= xMinClip ) {
                            x = xMinClip;
                        }
                        else if ( x >= xMaxClip ) {
                            x = xMaxClip;
                        }

                        if ( y <= yMinClip ) {
                            y = yMinClip;
                        }
                        else if ( y >= yMaxClip ) {
                            y = yMaxClip;
                        }
                    }

                    // calculate the pos affect by anchor
                    x -= offsetX;
                    y += offsetY;

                    // calculate the shear
                    x += y * shear_.x;
                    y += x * shear_.y;

                    // DISABLE: we use min,max clip above { 
                    // // do clip
                    // if ( clipInfo_.clipped ) {
                    //     switch (i) {
                    //     case 0: x += scale_.x * width_ * clipLeft;  y -= scale_.y * height_ * clipBottom; break; // bl
                    //     case 1: x -= scale_.x * width_ * clipRight; y -= scale_.y * height_ * clipBottom; break; // br
                    //     case 2: x += scale_.x * width_ * clipLeft;  y += scale_.y * height_ * clipTop; break; // tl
                    //     case 3: x -= scale_.x * width_ * clipRight; y += scale_.y * height_ * clipTop; break; // tr
                    //     }
                    // }
                    // } DISABLE end 

                    // build vertices, normals and uvs
                    switch ( plane ) {
                    case Plane.XY:
                        vertices[i] = new Vector3( x, y, 0.0f );
                        // normals[i] = new Vector3( 0.0f, 0.0f, -1.0f );
                        break;
                    case Plane.XZ:
                        vertices[i] = new Vector3( x, 0.0f, y );
                        // normals[i] = new Vector3( 0.0f, 1.0f, 0.0f );
                        break;
                    case Plane.ZY:
                        vertices[i] = new Vector3( 0.0f, y, x );
                        // normals[i] = new Vector3( 1.0f, 0.0f, 0.0f );
                        break;
                    }
                }
            }
            _mesh.vertices = vertices;
            // _mesh.normals = normals;
            _mesh.bounds = UpdateBounds ( offsetX, offsetY, halfWidth * 2.0f, halfHeight * 2.0f );

            // update box-collider if we have
            UpdateBoxCollider ( collider as BoxCollider, _mesh );

// #if UNITY_EDITOR
//             _mesh.RecalculateBounds();
// #endif
        }

        // ======================================================== 
        // Update UV
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.UV) != 0 ) {
            Vector2[] uvs = new Vector2[4];

            // if the sprite is in an atlas
            if ( el != null ) {
                float xStart  = el.coords.x;
                float yStart  = el.coords.y;
                float xEnd    = el.coords.xMax;
                float yEnd    = el.coords.yMax;

                // do uv clip
                if ( clipInfo_.clipped ) {
                    xStart  += el.coords.width  * clipLeft;
                    yStart  += el.coords.height * clipTop;
                    xEnd    -= el.coords.width  * clipRight;
                    yEnd    -= el.coords.height * clipBottom;
                }

                if ( el.rotated ) {
                    uvs[0] = new Vector2 ( xEnd,    yEnd );
                    uvs[1] = new Vector2 ( xEnd,    yStart );
                    uvs[2] = new Vector2 ( xStart,  yEnd );
                    uvs[3] = new Vector2 ( xStart,  yStart );
                }
                else {
                    uvs[0] = new Vector2 ( xStart,  yEnd );
                    uvs[1] = new Vector2 ( xEnd,    yEnd );
                    uvs[2] = new Vector2 ( xStart,  yStart );
                    uvs[3] = new Vector2 ( xEnd,    yStart );
                }
            }
            else {
                float xStart  = trimUV.x;
                float yStart  = trimUV.y;
                float xEnd    = trimUV.xMax;
                float yEnd    = trimUV.yMax;

                // do uv clip
                if ( clipInfo_.clipped ) {
                    xStart  += trimUV.width  * clipLeft;
                    yStart  += trimUV.height * clipTop;
                    xEnd    -= trimUV.width  * clipRight;
                    yEnd    -= trimUV.height * clipBottom;
                }

                uvs[0] = new Vector2 ( xStart,  yEnd );
                uvs[1] = new Vector2 ( xEnd,    yEnd );
                uvs[2] = new Vector2 ( xStart,  yStart );
                uvs[3] = new Vector2 ( xEnd,    yStart );
            }
            _mesh.uv = uvs;
        }

        // ======================================================== 
        // Update Color
        // ======================================================== 

        if ( (updateFlags & UpdateFlags.Color) != 0 ) {

            Color[] colors = new Color[4];
            for ( int i = 0; i < 4; ++i ) {
                colors[i] = color_;
            }
            _mesh.colors = colors;
        }

        // ======================================================== 
        // Update Index 
        // ======================================================== 

        if (  (updateFlags & UpdateFlags.Index) != 0 ) {
            int[] indices = new int[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 1;
            indices[5] = 3;
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
        updateFlags = UpdateFlags.Vertex | UpdateFlags.UV | UpdateFlags.Color | UpdateFlags.Index;
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

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnEnable () {
        base.OnEnable();

        // NOTE: though we have ExecuteInEditMode, user can Add/Remove layer2d in Editor
#if UNITY_EDITOR
        exSpriteAnimation my_spanim = spanim;
        if ( EditorApplication.isPlaying ) {
            my_spanim = GetComponent<exSpriteAnimation>(); 
        }
        if ( my_spanim ) {
            my_spanim.enabled = true;
        }
#else
        if ( spanim ) {
            spanim.enabled = true;
        }
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void OnDisable () {
        base.OnDisable();

        // NOTE: though we have ExecuteInEditMode, user can Add/Remove layer2d in Editor
#if UNITY_EDITOR
        exSpriteAnimation my_spanim = spanim;
        if ( EditorApplication.isPlaying ) {
            my_spanim = GetComponent<exSpriteAnimation>(); 
        }
        if ( my_spanim ) {
            my_spanim.enabled = false;
            my_spanim.Stop();
        }
#else
        if ( spanim ) {
            spanim.enabled = false;
            spanim.Stop();
        }
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    override protected void Awake () {
        base.Awake();

        // DELME { 
// #if UNITY_EDITOR
//         if ( EditorApplication.isPlaying == false &&
//              useAtlas &&
//              string.IsNullOrEmpty(textureGUID) == false ) 
//         {
//             exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo ( textureGUID );
//             if ( elInfo != null &&
//                  ( elInfo.indexInAtlas != index_ ||
//                    elInfo.guidAtlas != exEditorHelper.AssetToGUID(atlas_) ) ) 
//             {
//                 SetSprite( exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas),
//                            elInfo.indexInAtlas );
//             }
//         }
// #endif
        // } DELME end 

        spanim = GetComponent<exSpriteAnimation>();
        if ( atlas_ != null ) {
            renderer.sharedMaterial = atlas_.material;

            meshFilter.sharedMesh = new Mesh();
            ForceUpdateMesh( meshFilter.sharedMesh );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Clear () {
        atlas_ = null;
        index_ = -1;
        renderer.sharedMaterial = null;
#if UNITY_EDITOR
        GetComponent<MeshFilter>().sharedMesh = null;
#else
        meshFilter.sharedMesh = null;
#endif
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public exAtlas.Element GetCurrentElement () {
        if ( useAtlas )
            return atlas_.elements[index_];
        return null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void SetSprite ( exAtlas _atlas, int _index ) {
        bool checkSize = false;

        // pre-check
        if ( _atlas == null || 
             _atlas.elements == null || 
             _index < 0 || 
             _index >= _atlas.elements.Length ) 
        {
            Debug.LogWarning ( "Invalid input in SetSprite." );
            return;
        }

        //
        if ( atlas_ != _atlas ) {
            atlas_ = _atlas;
            renderer.sharedMaterial = _atlas.material;
            updateFlags |= UpdateFlags.UV;
            checkSize = true;
        }

        //
        if ( index_ != _index ) {
            index_ = _index;
            updateFlags |= UpdateFlags.UV;
            checkSize = true;
        }

        //
        if ( checkSize && !customSize_ ) {
            exAtlas.Element el = atlas_.elements[index_];

            float newWidth = el.trimRect.width;
            float newHeight = el.trimRect.height;
            // float newWidth = el.coords.width * atlas_.texture.width;
            // float newHeight = el.coords.height * atlas_.texture.height;

            if ( el.rotated ) {
                float tmp = newWidth;
                newWidth = newHeight;
                newHeight = tmp;
            } 

            if ( newWidth != width_ || newHeight != height_ ) {
                width_ = newWidth;
                height_ = newHeight;
                updateFlags |= UpdateFlags.Vertex;
            }
        }
    }
}
