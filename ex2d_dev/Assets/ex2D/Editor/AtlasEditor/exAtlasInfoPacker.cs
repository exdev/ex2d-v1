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
            float areaWidth = _el.trimRect.width + _el.atlasInfo.padding;
            float areaHeight = _el.trimRect.height + _el.atlasInfo.padding;
            if ( areaWidth <= rect.width && areaHeight <= rect.height )
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
                                        width - padding,
                                        height - padding ) );
        foreach ( exAtlasInfo.Element el in elements ) {
            Node n = root.Insert (el);
            if ( n == null ) {
                Debug.LogError( "Failed to layout element " + el.texture.name );
                break;
            }
            el.coord[0] = (int)n.rect.x + padding;
            el.coord[1] = (int)n.rect.y + padding;
            ++i;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void BasicPack () {
        int curX = padding;
        int curY = padding;
        int maxY = 0; 
        int i = 0; 

        foreach ( exAtlasInfo.Element el in elements ) {
            if ( (curX + el.Width() + padding) >= width ) {
                curX = padding;
                curY = curY + maxY + padding;
                maxY = 0;
            }
            if ( el.Height() > maxY ) {
                maxY = el.Height();
            }
            if ( (maxY + el.Height()) >= height ) {
                Debug.LogError( "Failed to layout element " + el.texture.name );
                break;
            }
            el.coord[0] = curX;
            el.coord[1] = curY;

            curX = curX + el.Width() + padding;
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


