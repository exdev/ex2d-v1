// ======================================================================================
// File         : exUIElement.cs
// Author       : Wu Jie 
// Last Change  : 07/20/2011 | 00:07:45 AM | Wednesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// Interactions Data
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

public class exUIElement : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    [System.NonSerialized] public exUIElement parent;
    [System.NonSerialized] public List<exUIElement> children;

    ///////////////////////////////////////////////////////////////////////////////
    // static functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static void FindAndAddChild ( exUIElement _el ) {
        foreach ( Transform child in _el.transform ) {
            exUIElement child_el = child.GetComponent<exUIElement>();
            if ( child_el ) {
                _el.AddChild (child_el);
                exUIElement.FindAndAddChild (child_el);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnDestroy () {
        if ( parent == null ) {
            exUIMng uiMng = exUIMng.instance;
            if ( uiMng )
                uiMng.elements.Remove(this);
        }
        else {
            parent.RemoveChild(this);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public virtual bool OnEvent ( exUIEvent _e ) {
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddChild ( exUIElement _element ) {
        _element.transform.parent = transform;
        _element.parent = this;
        children.Add(_element);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void RemoveChild ( exUIElement _element ) {
        _element.parent = null;
        _element.transform.parent = null;
        children.Remove(_element);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    exUIElement FindParent () {
        Transform tranParent = transform.parent;
        while ( tranParent != null ) {
            exUIElement el = tranParent.GetComponent<exUIElement>();
            if ( el != null )
                return el;
            tranParent = tranParent.parent;
        }
        return null;
    } 
}
