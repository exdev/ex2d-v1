// ======================================================================================
// File         : exAtlasInfoPacker.cs
// Author       : Wu Jie 
// Last Change  : 08/27/2011 | 10:35:11 AM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// AtlasPacker
///////////////////////////////////////////////////////////////////////////////

partial class exAtlasInfo {

    ///////////////////////////////////////////////////////////////////////////////
    // class Node
    // 
    // Purpose: 
    // 
    ///////////////////////////////////////////////////////////////////////////////

    class Node {
        public Rect rect;
        public Node[] child;

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public Node ( Rect _rect ) {
            rect = _rect;
        }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public Node Insert ( exAtlasInfo.Element _el ) {
            Node node = null;

            //
            if ( child != null ) {
                node = child[0].Insert(_el);
                if ( node == null ) {
                    return child[1].Insert(_el);
                }
                else {
                    return node;
                }
            }

            //
            float areaWidth = _el.trimRect.width + _el.atlasInfo.actualPadding;
            float areaHeight = _el.trimRect.height + _el.atlasInfo.actualPadding;
            if (_el.trimRect.width <= rect.width && _el.trimRect.height <= rect.height)
            {
                child = new Node[2];
                child[0] = new Node( new Rect ( rect.x + areaWidth, 
                                                rect.y,
                                                rect.width - areaWidth, 
                                                areaHeight ) );
                child[1] = new Node( new Rect ( rect.x,
                                                rect.y + areaHeight,
                                                rect.width, 
                                                rect.height - areaHeight ) );
                node = new Node( new Rect ( rect.x, 
                                            rect.y, 
                                            areaWidth,
                                            areaHeight ) );
            }
            return node;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void TreePack () {
        int i = 0; 
        Node root = new Node( new Rect( 0,
                                        0,
                                        width,
                                        height ) );
        foreach ( exAtlasInfo.Element el in elements ) {
            Node n = root.Insert (el);
            if ( n == null ) {
                Debug.LogError( "Failed to layout element " + el.texture.name );
                break;
            }
            el.coord[0] = (int)n.rect.x;
            el.coord[1] = (int)n.rect.y;
            ++i;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void BasicPack () {
        int curX = 0;
        int curY = 0;
        int maxY = 0; 
        int i = 0; 

        foreach ( Element el in elements ) {
            if ( (curX + el.Width()) > width ) {
                curX = 0;
                curY = curY + maxY + actualPadding;
                maxY = 0;
            }
            if ( (curY + el.Height()) > height ) {
                Debug.LogError( "Failed to layout element " + el.texture.name );
                break;
            }
            el.coord[0] = curX;
            el.coord[1] = curY;

            curX = curX + el.Width() + actualPadding;
            if (el.Height() > maxY) {
                maxY = el.Height();
            }
            ++i;
        }
    }

    // ------------------------------------------------------------------ 
    /// Layout elements by the exAtlasInfo.algorithm
    // ------------------------------------------------------------------ 

    public void LayoutElements () {
        ResetElements();
        SortElements();

        // this is very basic algorithm
        if ( algorithm == exAtlasInfo.Algorithm.Basic ) {
            BasicPack ();
        }
        else if ( algorithm == exAtlasInfo.Algorithm.Tree ) {
            TreePack ();
        }
        EditorUtility.SetDirty(this);

        //
        foreach ( exAtlasInfo.Element el in elements ) {
            AddSpriteAnimClipForRebuilding(el);
        }

        needLayout = false;
    }
}


