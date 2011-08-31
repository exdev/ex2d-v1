// ======================================================================================
// File         : exTileInfo.cs
// Author       : Wu Jie 
// Last Change  : 08/20/2011 | 15:11:00 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class exTileInfo : ScriptableObject {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public Texture2D texture;     // tile-sheet build from atlas editor or photoshop
    public Material material;   // default material
    public int tileWidth = 32;
    public int tileHeight = 32;
    public int padding = 1;
    public Vector2 anchor = Vector2.zero; // the anchor point of each tile-texture, anchor is start from left-bottom 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class Element {
        public GameObject prefab;
    }
    public List<Element> elements = new List<Element>();
}

