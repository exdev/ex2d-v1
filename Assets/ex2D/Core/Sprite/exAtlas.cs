// ======================================================================================
// File         : exAtlas.cs
// Author       : Wu Jie 
// Last Change  : 07/03/2011 | 23:09:42 PM | Sunday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

///////////////////////////////////////////////////////////////////////////////
// exAtlas
///////////////////////////////////////////////////////////////////////////////

public class exAtlas : ScriptableObject {
    [System.Serializable]
    public class Element {
        public string name = "";
        public int originalWidth = 0;  // the original width of the texture 
        public int originalHeight = 0; // the original height of the texture
        public Rect trimRect = new Rect( 0, 0, 1, 1 );  // the trimmed rect
        public Rect coords = new Rect( 0, 0, 1, 1 ); // (xStart, yStart, xEnd, yEnd)
        public bool rotated = false;
    }

    public Element[] elements;
    public Material material;
    public Texture2D texture;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public int GetIndexByName ( string _name ) {
        for ( int i = 0; i < elements.Length; ++i ) {
            if ( elements[i].name == _name )
                return i;
        }
        return -1;
    }

    ///////////////////////////////////////////////////////////////////////////////
    // static
    ///////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static exAtlas Create ( string _path, string _name ) {
        //
        if ( new DirectoryInfo(_path).Exists == false ) {
            Debug.LogError ( "can't create asset, path not found" );
            return null;
        }
        if ( string.IsNullOrEmpty(_name) ) {
            Debug.LogError ( "can't create asset, the name is empty" );
            return null;
        }
        string assetPath = Path.Combine( _path, _name + ".asset" );

        //
        exAtlas newAtlas = ScriptableObject.CreateInstance<exAtlas>();
        AssetDatabase.CreateAsset(newAtlas, assetPath);
        Selection.activeObject = newAtlas;
        return newAtlas;
    }
#endif
}
