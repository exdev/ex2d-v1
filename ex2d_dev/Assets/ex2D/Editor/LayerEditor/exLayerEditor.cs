// ======================================================================================
// File         : exLayerEditor.cs
// Author       : Wu Jie 
// Last Change  : 11/08/2011 | 11:50:16 AM | Tuesday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
///
/// the layer mng editor
///
///////////////////////////////////////////////////////////////////////////////

public class exLayerEditor : EditorWindow {

    static int layerFieldHeight = 20;

    protected class LayerForSort {
        public int index = 0; 
        public exLayer layer = null; 
    }
    static int CompareByIndex ( LayerForSort _a, LayerForSort _b ) {
        return _a.index - _b.index;
    }

    ///////////////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////////////

    public bool lockSelection = true;

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    protected GameObject curEditGO = null;
    protected exLayer curLayer = null;
    protected float totalInfoFieldHeight = 0.0f;

    protected bool inDragState = false;
    protected Vector2 scrollPos = Vector2.zero;
    protected bool sortSelection = false;

    protected List<exLayer> layers = new List<exLayer>();
    protected List<exLayer> selectedLayers = new List<exLayer>();
    protected exLayer insertLayer = null;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \return the editor
    /// Open the layer mng editor window
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/Layer Editor")]
    public static exLayerEditor NewWindow () {
        exLayerEditor newWindow = EditorWindow.GetWindow<exLayerEditor>();
        return newWindow;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        name = "Layer Info Editor";
        wantsMouseMove = true;
        autoRepaintOnSceneChange = false;
        // position = new Rect ( 50, 50, 800, 600 );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
    }

    // ------------------------------------------------------------------ 
    /// \param _obj
    /// Check if the object is valid atlas and open it in atlas editor.
    // ------------------------------------------------------------------ 

    public void Edit ( GameObject _go ) {
        // check if repaint
        if ( curEditGO != _go ) {
            curEditGO = _go;
            Init();

            Repaint ();
        }
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSelectionChange () {
        // check if we have atlas - editorinfo in the same directory
        if ( lockSelection == false ) {
            GameObject go = Selection.activeGameObject;
            if ( go ) {
                Edit (go);
            }
        }

        Repaint ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        EditorGUI.indentLevel = 0;

        // ======================================================== 
        // check if selection valid 
        // ======================================================== 

        //
        if ( curEditGO == null ) {
            GUILayout.Space(10);
            GUILayout.Label ( "Please select a GameObject for editing" );
            return;
        }

        //
        if ( curEditGO.GetComponent<Camera>() ) {
            curLayer = curEditGO.GetComponent<exLayerMng>();
            if ( curLayer == null ) {
                if ( GUILayout.Button ( "Add Layer Manager", GUILayout.Width(200) ) ) {
                    curLayer = curEditGO.AddComponent<exLayerMng> ();
                }
            }
        }
        else {
            //
            curLayer = curEditGO.GetComponent<exLayer>();
            if ( curLayer == null ) {
                GUILayout.Space(10);
                GUILayout.Label ( "There is no edit layer in this GameObject" );
                return;
            }
        }

        //
        if ( curLayer == null ) {
            GUILayout.Label ( "Invalid GameObject" );
            return;
        }

        // ======================================================== 
        // toolbar 
        // ======================================================== 

        GUILayout.BeginHorizontal ( EditorStyles.toolbar );
            GUILayout.FlexibleSpace();
            lockSelection = GUILayout.Toggle ( lockSelection, "Lock", EditorStyles.toolbarButton );
        GUILayout.EndHorizontal ();

        // ======================================================== 
        // scroll view
        // ======================================================== 

        float toolbarHeight = EditorStyles.toolbar.CalcHeight( new GUIContent(""), 0 );
        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, 
                                                      GUILayout.Width(position.width),
                                                      GUILayout.Height(position.height-toolbarHeight) );

        GUILayout.Space(10);

        // ======================================================== 
        // draw the tree 
        // ======================================================== 

        //
        float space = 5.0f;
        Rect lastRect = GUILayoutUtility.GetLastRect ();  
        LayerTreeField ( space, lastRect.yMax, curLayer );

        // ======================================================== 
        // process mouse event 
        Event e = Event.current;
        // ======================================================== 

        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            selectedLayers.Clear();
            Repaint();
        }
        else if ( e.type == EventType.MouseUp ) {
            inDragState = false;
            Repaint();
        }
        // do add GameObject to layerMng 
        else if ( e.type == EventType.DragUpdated ) {
            // Show a copy icon on the drag
            foreach ( Object o in DragAndDrop.objectReferences ) {
                if ( o is GameObject ) {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;
                }
            }
        }
        else if ( e.type == EventType.DragPerform ) {
            DragAndDrop.AcceptDrag();
            EditorUtility.DisplayProgressBar( "Add new layer items", "Add new layer itmes...", 0.5f  );    

            //
            Object oldSelActiveObject = null;
            List<Object> oldSelObjects = new List<Object>();
            foreach ( Object o in Selection.objects ) {
                oldSelObjects.Add(o);
            }
            oldSelActiveObject = Selection.activeObject;

            //
            List<exLayer> newLayers = new List<exLayer>(); 
            foreach ( Object o in DragAndDrop.objectReferences ) {
                // never add self, it cause dead loop
                if ( o == curEditGO )
                    continue;

                //
                Selection.activeObject = o;
                Object[] objects = Selection.GetFiltered( typeof(GameObject), SelectionMode.Deep);
                foreach ( Object obj in objects ) {
                    if ( obj ) {
                        GameObject go = obj as GameObject;
                        exLayer layer = go.GetComponent<exLayer>();
                        if ( layer == null ) {
                            layer = go.AddComponent<exLayer>();
                        }
                        if ( newLayers.IndexOf(layer) == -1 ) {
                            newLayers.Add(layer);
                        }
                    }
                }
            }

            //
            foreach ( exLayer li in newLayers ) {
                li.parent = curLayer;
                EditorUtility.SetDirty(li);
            }
            EditorUtility.SetDirty(curLayer);

            //
            Selection.objects = oldSelObjects.ToArray();
            Selection.activeObject = oldSelActiveObject;
            EditorUtility.ClearProgressBar();

            Repaint();
        }

        EditorGUILayout.EndScrollView();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void LayerTreeField ( float _x, float _y, exLayer _layer ) {
        //
        layers.Clear();
        LayerField ( new Rect ( _x, _y, position.width - 2.0f * _x, layerFieldHeight ),
                     0,
                     _layer );

        //
        if ( sortSelection ) {
            SortSelection();
            sortSelection = false;
        }

        //
        if ( inDragState && insertLayer != null ) {
            bool insertLayerSelected = selectedLayers.IndexOf(insertLayer) != -1;
            int index = 0;
            GetLayerIndex ( ref index, curLayer, insertLayer ); 
            Rect rect = new Rect( _x,
                                  _y + index * (layerFieldHeight) - index,
                                  position.width - 2.0f * _x,
                                  layerFieldHeight );

            if ( Event.current.mousePosition.y <= rect.y + rect.height * 0.5f ) {
                index = layers.IndexOf(insertLayer);
                exLayer prevLayer = (index-1 < 0 ) ? null : layers[index-1];

                // up
                if ( prevLayer != null && 
                     ( selectedLayers.IndexOf(prevLayer) == -1 || insertLayerSelected == false ) )
                {
                    exEditorHelper.DrawRect( new Rect( rect.x, rect.y+1 - rect.height * 0.1f, rect.width, rect.height * 0.2f ), 
                                             new Color( 1.0f, 1.0f, 0.0f, 1.0f ), 
                                             new Color( 1.0f, 1.0f, 0.0f, 1.0f ) );
                }
            }
            else if ( Event.current.mousePosition.y > rect.y + rect.height * 0.5f ) {
                index = layers.IndexOf(insertLayer);
                exLayer nextLayer = (index+1 > layers.Count-1 ) ? null : layers[index+1];

                // down
                if ( nextLayer != null && 
                     ( selectedLayers.IndexOf(nextLayer) == -1 || insertLayerSelected == false ) )
                {
                    exEditorHelper.DrawRect( new Rect( rect.x, rect.y + rect.height * 0.9f, rect.width, rect.height * 0.2f ), 
                                             new Color( 1.0f, 1.0f, 0.0f, 1.0f ), 
                                             new Color( 1.0f, 1.0f, 0.0f, 1.0f ) );
                }
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    float LayerField ( Rect _rect, int _indentLevel, exLayer _layer ) {
        // get selected
        bool selected = selectedLayers.IndexOf(_layer) != -1;

        // ======================================================== 
        // draw field 
        // ======================================================== 

        exEditorHelper.DrawRect( _rect, 
                                 selected ? new Color( 0.0f, 0.4f, 0.8f, 1.0f ) : new Color( 0.25f, 0.25f, 0.25f, 1.0f ), 
                                 new Color( 0.0f, 0.0f, 0.0f, 1.0f ) );
        float curX = _rect.x;
        float curY = _rect.y;

        curX += _indentLevel * 15.0f + 10.0f;
        if ( _layer.children.Count > 0 ) {
            _layer.foldout = EditorGUI.Foldout ( new Rect ( curX, curY + 2, 10, _rect.height-2 ), _layer.foldout, "" );
            curX += 15;
        }
        else {
            curX += 10;
        }

        GUI.Label ( new Rect ( curX, curY + 1, _rect.width - curX, _rect.height ), _layer.name );
        layers.Add(_layer);

        // ======================================================== 
        // process mouse event 
        Event e = Event.current;
        // ======================================================== 

        if ( _rect.Contains( e.mousePosition ) ) {
            if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
                if ( _layer != curLayer ) {
                    if ( e.command || e.control ) {
                        ToggleSelected(_layer);
                    }
                    else {
                        if ( selected == false ) {
                            selectedLayers.Clear();
                            AddSelected(_layer);
                        }
                    }
                    insertLayer = null;
                    e.Use();
                    Repaint();
                }
            }
            else if ( inDragState == false && 
                      e.type == EventType.MouseUp && 
                      e.button == 0 && 
                      e.clickCount == 1 &&
                      e.command == false &&
                      e.control == false ) 
            {
                if ( _layer != curLayer ) {
                    selectedLayers.Clear();
                    AddSelected(_layer);

                    insertLayer = null;
                    e.Use();
                    Repaint();
                }
            }
            else if ( e.type == EventType.MouseDrag ) {
                if ( inDragState ) {
                    if ( _layer != curLayer ) {
                        insertLayer = _layer;
                        Repaint();
                    }
                }
                else {
                    inDragState = true;
                }
                e.Use();
                Repaint();
            }
        }

        // ======================================================== 
        // draw the child
        // ======================================================== 

        curY += layerFieldHeight-1;
        if ( _layer.foldout ) {
            foreach ( exLayer child in _layer.children ) {
                curY = LayerField ( new Rect ( _rect.x, curY, _rect.width, layerFieldHeight ),
                                    _indentLevel + 1,
                                    child );
            }
        }

        return curY;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AddSelected ( exLayer _l ) {
        if ( selectedLayers.IndexOf(_l) == -1 ) {
            selectedLayers.Add(_l);
            foreach ( exLayer l in _l.children ) {
                AddSelected (l);
            }
        }
        sortSelection = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RemoveSelected ( exLayer _l ) {
        bool parentInSelect = false;
        exLayer parent = _l.parent;
        while ( parent != null ) {
            if ( selectedLayers.IndexOf(parent) != -1 ) {
                parentInSelect = true;
                break;
            }
            parent = parent.parent;
        }

        //
        if ( parentInSelect == false ) {
            selectedLayers.Remove(_l);
            foreach ( exLayer l in _l.children ) {
                RemoveSelected (l);
            }
        }

        sortSelection = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ToggleSelected ( exLayer _l ) {
        int i = selectedLayers.IndexOf(_l);
        if ( i != -1 ) {
            RemoveSelected(_l);
        }
        else {
            AddSelected (_l);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void SortSelection () {
        List<LayerForSort> layerForSorts = new List<LayerForSort>(); 
        foreach ( exLayer l in selectedLayers ) {
            int index = 0;
            GetLayerIndex( ref index, curLayer, l );
            LayerForSort lfs = new LayerForSort();
            lfs.index = index;
            lfs.layer = l;
            layerForSorts.Add(lfs);
        }
        layerForSorts.Sort( CompareByIndex );
        selectedLayers.Clear();
        foreach ( LayerForSort lfs in layerForSorts ) {
            selectedLayers.Add(lfs.layer);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    bool GetLayerIndex ( ref int _index, exLayer _curLayer, exLayer _l ) {
        if ( _curLayer == _l )
            return true;

        for ( int i = 0; i < _curLayer.children.Count; ++i ) {
            _index += 1;
            exLayer childLayer = _curLayer.children[i]; 
            if ( childLayer == _l ) {
                return true;
            }
            bool found = GetLayerIndex ( ref _index, childLayer, _l );
            if ( found )
                return true;
        }
        return false;
    }
}
