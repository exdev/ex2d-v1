// ======================================================================================
// File         : exBitmapFont.cs
// Author       : Wu Jie 
// Last Change  : 07/15/2011 | 13:52:28 PM | Friday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// exBitmapFont
///////////////////////////////////////////////////////////////////////////////

public class exBitmapFont : ScriptableObject {

    ///////////////////////////////////////////////////////////////////////////////
    // class defines
    ///////////////////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class CharInfo {
        public int id = -1;
        public int x = -1;
        public int y = -1;
        public int width = -1;
        public int height = -1;
        public int xoffset = -1;
        public int yoffset = -1;
        public int xadvance = -1;
        public int page = -1;
        public Vector2 uv0 = Vector2.zero;
    }

    [System.Serializable]
    public class KerningInfo {
        public int first = -1;
        public int second = -1;
        public int amount = -1;
    }

    [System.Serializable]
    public class PageInfo {
        public Texture2D texture;
        public Material material;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public int lineHeight;
    public int size;

    public List<PageInfo> pageInfos = new List<PageInfo>(); 
    public List<CharInfo> charInfos = new List<CharInfo>();
    public List<KerningInfo> kernings = new List<KerningInfo>();
    public List<string> fontInfoGUIDs = new List<string>();

    public bool useAtlas = false;
    public bool editorNeedRebuild = false;

    protected Dictionary<int,CharInfo> idToCharInfo = null;

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void RebuildIdToCharInfoTable () {
        if ( idToCharInfo == null ) {
            idToCharInfo = new Dictionary<int,CharInfo>();
        }
        idToCharInfo.Clear();
        foreach ( CharInfo c in charInfos ) {
            idToCharInfo[c.id] = c;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public CharInfo GetCharInfo ( int _id ) {
        // create and build idToCharInfo table if null
        if ( idToCharInfo == null ) {
            idToCharInfo = new Dictionary<int,CharInfo>();
            foreach ( CharInfo c in charInfos ) {
                idToCharInfo[c.id] = c;
            }
        }

        //
        if ( idToCharInfo.ContainsKey (_id) )
            return idToCharInfo[_id];
        return null;
    }
}

