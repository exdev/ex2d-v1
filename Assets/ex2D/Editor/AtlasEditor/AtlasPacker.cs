// ======================================================================================
// File         : AtlasPacker.cs
// Author       : Wu Jie 
// Last Change  : 07/12/2011 | 09:39:32 AM | Tuesday,July
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

partial class exAtlasEditor {

    ///////////////////////////////////////////////////////////////////////////////
    // class Node
    // 
    // Purpose: 
    // 
    ///////////////////////////////////////////////////////////////////////////////

    public class Node {
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

    public void TreePack () {
        int i = 0; 
        Node root = new Node( new Rect( 0,
                                        0,
                                        curEdit.width - curEdit.padding,
                                        curEdit.height - curEdit.padding ) );
        EditorUtility.DisplayProgressBar( "Layout Elements...", "Layout Elements...", 0.5f  );    
        foreach ( exAtlasInfo.Element el in curEdit.elements ) {
            // DISABLE: it is too slow { 
            // EditorUtility.DisplayProgressBar( "Layout Elements...",
            //                                   "Layout " + el.texture.name,
            //                                   (float)i / (float)curEdit.elements.Count  );    
            // } DISABLE end 
            Node n = root.Insert (el);
            if ( n == null ) {
                Debug.LogError( "Failed to layout element " + el.texture.name );
                break;
            }
            el.coord[0] = (int)n.rect.x + curEdit.padding;
            el.coord[1] = (int)n.rect.y + curEdit.padding;
            ++i;
        }
        EditorUtility.ClearProgressBar();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void BasicPack () {
        int curX = curEdit.padding;
        int curY = curEdit.padding;
        int maxY = 0; 
        int i = 0; 

        EditorUtility.DisplayProgressBar( "Layout Elements...", "Layout Elements...", 0.5f  );    
        foreach ( exAtlasInfo.Element el in curEdit.elements ) {
            // DISABLE: it is too slow { 
            // EditorUtility.DisplayProgressBar( "Layout Elements...",
            //                                   "Layout " + el.texture.name,
            //                                   (float)i / (float)curEdit.elements.Count  );    
            // } DISABLE end 
            if ( (curX + el.Width() + curEdit.padding) >= curEdit.width ) {
                curX = curEdit.padding;
                curY = curY + maxY + curEdit.padding;
                maxY = 0;
            }
            if ( el.Height() > maxY ) {
                maxY = el.Height();
            }
            if ( (maxY + el.Height()) >= curEdit.height ) {
                Debug.LogError( "Failed to layout element " + el.texture.name );
                break;
            }
            el.coord[0] = curX;
            el.coord[1] = curY;

            curX = curX + el.Width() + curEdit.padding;
            ++i;
        }
        EditorUtility.ClearProgressBar();
    }
}


