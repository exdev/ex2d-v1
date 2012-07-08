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
using UnityEditor;

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
        public Node rightChild;
        public Node bottomChild;

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public Node ( Rect _rect ) {
            rect = _rect;
        }

        // ------------------------------------------------------------------ 
        // Desc: 
        // ------------------------------------------------------------------ 

        public Vector2? Insert ( Element _el ) {
            // when this node is already occupied (when it has children),
            // forward to child nodes recursively
            if (rightChild != null) {
                Vector2? pos = rightChild.Insert(_el);
                if (pos != null)
                    return pos;
                return bottomChild.Insert(_el);
            }

            // determine trimmed and padded sizes
            float trimmedWidth = _el.trimRect.width;
            float trimmedHeight = _el.trimRect.height;
            float paddedWidth = trimmedWidth + _el.atlasInfo.actualPadding;
            float paddedHeight = trimmedHeight + _el.atlasInfo.actualPadding;

            // trimmed element size must fit within current node rect
            if (trimmedWidth > rect.width || trimmedHeight > rect.height)
                return null;

            // create first child node in remaining space to the right, using trimmedHeight
            // so that only other elements with the same height or less can be added there
            // (we do not use paddedHeight, because the padding area is reserved and should
            // never be occupied)
            rightChild = new Node( new Rect ( rect.x + paddedWidth, 
                                              rect.y,
                                              rect.width - paddedWidth, 
                                              trimmedHeight ) );

            // create second child node in remaining space at the bottom, occupying the entire width
            bottomChild = new Node( new Rect ( rect.x,
                                               rect.y + paddedHeight,
                                               rect.width, 
                                               rect.height - paddedHeight ) );

            // return position where to put element
            return new Vector2( rect.x, rect.y );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void TreePack () {
        Node root = new Node( new Rect( 0,
                                        0,
                                        width,
                                        height ) );
        foreach ( Element el in elements ) {
            Vector2? pos = root.Insert (el);
            if (pos != null) {
                el.coord[0] = (int)pos.Value.x;
                el.coord[1] = (int)pos.Value.y;
            }
            else {
                // log error but continue processing other elements
                Debug.LogError("Failed to layout element " + el.texture.name);
            }
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


