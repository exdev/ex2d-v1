// ======================================================================================
// File         : exLayer.cs
// Author       : Wu Jie 
// Last Change  : 11/07/2011 | 14:50:39 PM | Monday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
///
/// The layer classes
///
///////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
[AddComponentMenu("ex2D Helper/Layer 2D")]
public class exLayer : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    [SerializeField] protected exLayer parent_ = null;
    public exLayer parent {
        set {
            // already the parent
            if ( parent_ == value ) {
                return;
            }

            if ( value == this ) {
                Debug.LogWarning("can't add self as parent");
                return;
            }

            //
            if ( parent_ ) {
                parent_.children_.Remove(this);
            }
            if ( value ) {
                value.children_.Add(this);
            }
            parent_ = value;
        }
        get {
            return parent_;
        }
    }

    [SerializeField] protected List<exLayer> children_ = new List<exLayer>();
    public List<exLayer> children {
        get {
            return children_;
        }
    }

    // editor only data
    public bool foldout = true;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected virtual void Awake () {
        // relink layers
        // NOTE: this happends when we clone a GameObject

        if ( parent_ && parent_.children.IndexOf(this) == -1 ) {
            parent_.children_.Add(this);
        }

        for ( int i = 0; i < children_.Count; ++i ) {
            exLayer childLayer = children_[i];
            if ( childLayer.parent != this ) {
                children_.RemoveAt(i);
                --i;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDestroy () {
        parent = null;
    }
}
