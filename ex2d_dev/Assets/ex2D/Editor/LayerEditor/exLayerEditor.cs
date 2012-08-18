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

#if !(EX2D_EVALUATE)

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
    protected bool doDrag = false;
    protected bool doShiftSelect = false;
    protected Vector2 scrollPos = Vector2.zero;
    protected bool sortSelection = false;
    protected exLayer shiftClickStart = null;
    protected exLayer curShiftClick = null;
    protected exLayer lastShiftClick = null;

    protected List<exLayer> layers = new List<exLayer>();
    protected List<exLayer> selectedLayers = new List<exLayer>();
    protected exLayer layerCursorIn = null;

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
    /// \param _go The GameObject to edit
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

            // ======================================================== 
            // update button
            // ======================================================== 

            GUILayout.FlexibleSpace();
            exLayerMng layerMng = curLayer as exLayerMng;
            GUI.enabled = (layerMng != null);
            if ( GUILayout.Button( "Update", EditorStyles.toolbarButton ) ) {
                layerMng.AddDirtyLayer(layerMng);
                EditorUtility.SetDirty(layerMng);
            }
            GUI.enabled = true;
            GUILayout.Space(5);

            // ======================================================== 
            // lock button
            // ======================================================== 

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
        GUILayout.Space(5);

        // ======================================================== 
        // Clear All Layer Button 
        // ======================================================== 

        Color oldBGColor = GUI.backgroundColor;
        Color oldCTColor = GUI.contentColor;
        GUI.backgroundColor = Color.red;
        GUI.contentColor = Color.yellow;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
                if ( GUILayout.Button("Remove All Layers", GUILayout.Width(120) ) ) {
                    bool doRemove = EditorUtility.DisplayDialog( "Warning!",
                                                                 "This operation will remove all exLayer Component in the editor, do you want to continue operation",
                                                                 "Yes", "No" );
                    if ( doRemove ) {
                        RecursivelyDestroyLayer (curLayer);
                    }
                }
            GUILayout.EndHorizontal();
        GUI.backgroundColor = oldBGColor;
        GUI.contentColor = oldCTColor;

        // ======================================================== 
        // process mouse event 
        Event e = Event.current;
        // ======================================================== 

        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            selectedLayers.Clear();

            e.Use();
            Repaint();
        }
        else if ( e.type == EventType.MouseUp ) {
            inDragState = false;

            e.Use();
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
            try {
                EditorUtility.DisplayProgressBar( "Add new layer items", "Add new layer itmes...", 0.5f  );    

                //
                Object oldSelActiveObject = null;
                List<Object> oldSelObjects = new List<Object>();
                foreach ( Object o in Selection.objects ) {
                    oldSelObjects.Add(o);
                }
                oldSelActiveObject = Selection.activeObject;

                //
                List<exLayer> addedLayers = new List<exLayer>(); 
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
                                newLayers.Add(layer);
                            }
                            if ( addedLayers.IndexOf(layer) == -1 ) {
                                addedLayers.Add(layer);
                            }
                        }
                    }
                }

                // sync new layer's parent
                foreach ( exLayer layer in newLayers ) {
                    Transform parent = layer.transform.parent;
                    while ( parent != null ) {
                        exLayer parentLayer = parent.GetComponent<exLayer>();
                        if ( parentLayer ) {
                            layer.parent = parentLayer;
                            break;
                        }
                        parent = parent.parent;
                    }
                }

                //
                List<exLayer> parentOfNewLayers = new List<exLayer>(); 
                foreach ( exLayer li in addedLayers ) {
                    if ( addedLayers.IndexOf(li.parent) == -1 ) {
                        parentOfNewLayers.Add(li);
                    }
                }
                foreach ( exLayer li in parentOfNewLayers ) {
                    if ( li.parent != null )
                        EditorUtility.SetDirty(li.parent);
                    li.ForceSetParent(curLayer);
                    EditorUtility.SetDirty(li);
                }
                EditorUtility.SetDirty(curLayer);

                //
                Selection.objects = oldSelObjects.ToArray();
                Selection.activeObject = oldSelActiveObject;
                EditorUtility.ClearProgressBar();
            }
            catch ( System.Exception ) {
                EditorUtility.ClearProgressBar();
                throw;
            }

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
        Rect layerFieldRect = new Rect ( _x, _y, position.width - 2.0f * _x, layerFieldHeight );

        // DISABLE { 
        // float totalLayerHeight = LayerField ( layerFieldRect, 0, _layer ); 
        // GUILayoutUtility.GetRect ( layerFieldRect.width, totalLayerHeight );
        LayerField ( layerFieldRect, 0, _layer ); 
        // } DISABLE end 

        // ======================================================== 
        // process drag 
        // ======================================================== 

        if ( inDragState && layerCursorIn != null ) {
            bool insertLayerSelected = selectedLayers.IndexOf(layerCursorIn) != -1;
            int index = 0;
            index = layers.IndexOf(layerCursorIn);
            Rect rect = new Rect( _x,
                                  _y + index * (layerFieldHeight) - index,
                                  position.width - 2.0f * _x,
                                  layerFieldHeight );
            int hlIndex = -1;

            //
            if ( Event.current.mousePosition.y <= rect.y + rect.height * 0.5f ) {
                index = layers.IndexOf(layerCursorIn);
                exLayer prevLayer = (index-1 < 0) ? null : layers[index-1];

                // up
                if ( prevLayer != null && 
                     ( selectedLayers.IndexOf(prevLayer) == -1 || 
                       insertLayerSelected == false ) )
                {
                    hlIndex = index-1;
                    exEditorHelper.DrawRect( new Rect( rect.x, rect.y+1 - rect.height * 0.1f, rect.width, rect.height * 0.2f ), 
                                             new Color( 1.0f, 1.0f, 0.0f, 1.0f ), 
                                             new Color( 1.0f, 1.0f, 0.0f, 1.0f ) );
                }
            }
            else if ( Event.current.mousePosition.y > rect.y + rect.height * 0.5f ) {
                index = layers.IndexOf(layerCursorIn);
                exLayer nextLayer = (index > layers.Count-1) ? null : layers[index];
                int nextIndexInSelection = selectedLayers.IndexOf(nextLayer);

                // down
                if ( nextLayer != null && 
                     ( nextIndexInSelection == -1 || 
                       nextIndexInSelection == selectedLayers.Count-1 ||
                       insertLayerSelected == false ) )
                {
                    hlIndex = index;
                    exEditorHelper.DrawRect( new Rect( rect.x, rect.y + rect.height * 0.9f, rect.width, rect.height * 0.2f ), 
                                             new Color( 1.0f, 1.0f, 0.0f, 1.0f ), 
                                             new Color( 1.0f, 1.0f, 0.0f, 1.0f ) );
                }
            }

            //
            exLayer hlLayer = null;
            exLayer insertLayer = null;
            exLayer insertLayerNext = null;

            // highlight the layer for insert 
            if ( hlIndex != -1 ) {
                insertLayer = layers[hlIndex];
                insertLayerNext = (hlIndex == layers.Count-1) ? null : layers[hlIndex+1];

                hlLayer = layers[hlIndex];
                float indentX = _x + hlLayer.indentLevel * 15.0f + 10.0f;
                if ( hlLayer.children.Count == 0 ) {
                    indentX += 15.0f;
                }
                while ( selectedLayers.IndexOf(hlLayer) != -1 || Event.current.mousePosition.x <= indentX ) {
                    exLayer hlParent = hlLayer.parent;

                    if ( hlParent == null )
                        break;

                    if ( insertLayerNext != null &&
                         (insertLayerNext.indentLevel - hlParent.indentLevel) >= 2 )
                    {
                        break;
                    }

                    //
                    hlLayer = hlParent;
                    indentX = _x + hlLayer.indentLevel * 15.0f + 10.0f;
                    if ( hlLayer.children.Count == 0 ) {
                        indentX += 15.0f;
                    }
                }
                hlIndex = layers.IndexOf(hlLayer);
            }

            // again
            if ( hlIndex != -1 ) {
                exEditorHelper.DrawRect( new Rect( _x,
                                                   _y + hlIndex * (layerFieldHeight) - hlIndex,
                                                   position.width - 2.0f * _x,
                                                   layerFieldHeight ), 
                                         new Color( 0.0f, 1.0f, 0.0f, 0.2f ), 
                                         new Color( 0.0f, 0.0f, 0.0f, 0.0f ) );
            }

            //
            if ( (Event.current.type == EventType.MouseUp && Event.current.button == 0) ||
                 (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape) ) {

                inDragState = false;

                if ( hlIndex != -1 && Event.current.type == EventType.MouseUp ) {
                    //
                    if ( sortSelection ) {
                        SortSelection();
                        sortSelection = false;
                    }
                    List<exLayer> insertList = new List<exLayer>();
                    foreach ( exLayer layer in selectedLayers ) {
                        if ( selectedLayers.IndexOf(layer.parent) == -1 ) {
                            insertList.Add(layer);
                        }
                    }

                    // get exact insert layer
                    while ( (insertLayer.indentLevel - hlLayer.indentLevel) > 1 ) {
                        insertLayer = insertLayer.parent;
                    }
                    if ( hlLayer.children.Count == 0 ) {
                        foreach ( exLayer layer in insertList ) {
                            if ( layer.parent != null )
                                EditorUtility.SetDirty(layer.parent);
                            layer.parent = hlLayer;
                            EditorUtility.SetDirty(layer);
                        }
                    }
                    else {
                        for ( int i = insertList.Count-1; i >=0; --i ) {
                            int insertIdx = hlLayer.children.IndexOf(insertLayer) + 1; 
                            if ( insertList[i].parent != null )
                                EditorUtility.SetDirty(insertList[i].parent);
                            hlLayer.InsertAt ( insertIdx, insertList[i] );
                            EditorUtility.SetDirty(insertList[i]); 
                        }
                    }
                    EditorUtility.SetDirty(hlLayer);
                }

                //
                EditorUtility.SetDirty(curLayer);
                Event.current.Use();
                Repaint();
            }
        }

        // ======================================================== 
        // do shift select 
        // ======================================================== 

        if ( doShiftSelect ) {
            doShiftSelect = false;

            //
            int idx1 = layers.IndexOf(shiftClickStart);
            int idx2 = layers.IndexOf(lastShiftClick);
            if ( idx1 != -1 && idx2 != -1 ) {
                if ( idx1 > idx2 ) {
                    int tmp = idx1;
                    idx1 = idx2;
                    idx2 = tmp;
                }
                for ( int i = idx1; i <= idx2; ++i ) {
                    RemoveSelected(layers[i]);
                }
            }

            //
            idx1 = layers.IndexOf(shiftClickStart);
            idx2 = layers.IndexOf(curShiftClick);
            if ( idx1 != -1 && idx2 != -1 ) {
                if ( idx1 > idx2 ) {
                    int tmp = idx1;
                    idx1 = idx2;
                    idx2 = tmp;
                }
                for ( int i = idx1; i <= idx2; ++i ) {
                    AddSelected(layers[i]);
                }
            }
        }

        // ======================================================== 
        // process delete 
        // ======================================================== 

        if ( Event.current.type == EventType.KeyDown && 
             ( Event.current.keyCode == KeyCode.Delete ||
               Event.current.keyCode == KeyCode.Backspace ) ) 
        {
            List<exLayer> deleteList = new List<exLayer>();
            foreach ( exLayer layer in selectedLayers ) {
                if ( selectedLayers.IndexOf(layer.parent) == -1 ) {
                    deleteList.Add(layer);
                }
            }
            selectedLayers.Clear();
            foreach ( exLayer layer in deleteList ) {
                layer.parent = null;
                EditorUtility.SetDirty(layer);
            }

            //
            EditorUtility.SetDirty(curLayer);
            Event.current.Use();
            Repaint();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    float LayerField ( Rect _rect, int _indentLevel, exLayer _layer ) {
        if ( _layer == null )
            return _rect.y;

        // get selected
        bool selected = selectedLayers.IndexOf(_layer) != -1;

        // ======================================================== 
        // draw field 
        // ======================================================== 

        // draw rect
        exEditorHelper.DrawRect( _rect, 
                                 selected ? new Color( 0.0f, 0.4f, 0.8f, 1.0f ) : new Color( 0.25f, 0.25f, 0.25f, 1.0f ), 
                                 new Color( 0.0f, 0.0f, 0.0f, 1.0f ) );
        float curX = _rect.x;
        float curY = _rect.y;


        GUI.BeginGroup( _rect );
            // draw foldout
            curX += _indentLevel * 15.0f + 10.0f;
            if ( _layer.children.Count > 0 ) {
                _layer.foldout = EditorGUI.Foldout ( new Rect ( curX, 2, 10, _rect.height-2 ), _layer.foldout, "" );
            }
            curX += 15;
            _layer.indentLevel = _indentLevel;

            // draw label
            GUI.Label ( new Rect ( curX, 1, _rect.width - curX, _rect.height - 2 ), _layer.name );

            // draw enum
            EditorGUIUtility.LookLikeInspector ();
            Color oldColor = GUI.contentColor;
            Color oldBGColor = GUI.backgroundColor;
                switch ( _layer.type ) {
                case exLayer.Type.Dynamic: 
                    GUI.backgroundColor = Color.green; 
                    GUI.contentColor = Color.green; 
                    break;
                case exLayer.Type.Abstract: 
                    GUI.backgroundColor = Color.yellow; 
                    GUI.contentColor = Color.yellow; 
                    break;
                }
                _layer.type = (exLayer.Type)EditorGUI.EnumPopup ( new Rect ( _rect.width - 60.0f * 2.0f - 20.0f, 2, 70.0f, _rect.height - 4 ), _layer.type );
            GUI.backgroundColor = oldBGColor;
            GUI.contentColor = oldColor;
            EditorGUIUtility.LookLikeControls ();

            // draw range
            if (_layer.type == exLayer.Type.Dynamic) {
                _layer.range = EditorGUI.IntField ( new Rect ( _rect.width - 60.0f * 1.0f - 5.0f, 2, 60.0f, _rect.height - 4 ), _layer.range );
            }
        GUI.EndGroup();

        //
        if ( GUI.changed ) {
            EditorUtility.SetDirty(_layer);
        }

        //
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
                    else if ( e.shift ) {
                        doShiftSelect = true;
                        lastShiftClick = curShiftClick;
                        curShiftClick = _layer;
                    }
                    else {
                        if ( selected == false ) {
                            selectedLayers.Clear();
                            AddSelected(_layer);
                            shiftClickStart = _layer;
                        }
                    }
                    layerCursorIn = null;
                    e.Use();
                    Repaint();
                }
                doDrag = true;
            }
            else if ( inDragState == false && 
                      e.type == EventType.MouseUp && 
                      e.button == 0 && 
                      e.clickCount == 1 &&
                      e.command == false &&
                      e.control == false &&
                      e.shift == false ) 
            {
                if ( _layer != curLayer ) {
                    selectedLayers.Clear();
                    AddSelected(_layer);

                    layerCursorIn = null;
                    e.Use();
                    Repaint();
                }
            }
            else if ( e.type == EventType.MouseDrag ) {
                if ( inDragState ) {
                    if ( _layer != curLayer ) {
                        layerCursorIn = _layer;
                        Repaint();
                    }
                }
                else if ( selected && doDrag ) {
                    inDragState = true;
                    doDrag = false;
                }
                e.Use();
                Repaint();
            }
        }

        GUILayoutUtility.GetRect ( _rect.width, _rect.height );

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

    // DISABLE: this will help update layer in scene, but cause a lot { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void Update () {
    //     SceneView.RepaintAll();
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AddSelected ( exLayer _l ) {
        if ( _l == null )
            return;

        if ( selectedLayers.IndexOf(_l) == -1 ) {
            selectedLayers.Add(_l);
            foreach ( exLayer l in _l.children ) {
                AddSelected (l);
            }
        }

        sortSelection = true;
        List<GameObject> goList = new List<GameObject>();
        foreach ( exLayer layer in selectedLayers ) {
            goList.Add(layer.gameObject);
        }
        Selection.objects = goList.ToArray();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RemoveSelected ( exLayer _l ) {
        if ( _l == null )
            return;

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
        List<GameObject> goList = new List<GameObject>();
        foreach ( exLayer layer in selectedLayers ) {
            goList.Add(layer.gameObject);
        }
        Selection.objects = goList.ToArray();
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
            GetExactLayerIndex( ref index, curLayer, l );
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

    bool GetExactLayerIndex ( ref int _index, exLayer _curLayer, exLayer _l ) {
        if ( _curLayer == _l )
            return true;

        for ( int i = 0; i < _curLayer.children.Count; ++i ) {
            _index += 1;
            exLayer childLayer = _curLayer.children[i]; 
            if ( childLayer == _l ) {
                return true;
            }
            bool found = GetExactLayerIndex ( ref _index, childLayer, _l );
            if ( found )
                return true;
        }
        return false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RecursivelyDestroyLayer ( exLayer _layer ) {
        for ( int i = _layer.children.Count-1; i >= 0; --i ) {
            exLayer childLayer = _layer.children[i];
            RecursivelyDestroyLayer (childLayer);
            GameObject.DestroyImmediate(childLayer);
        }
        if ( (_layer is exLayerMng) == false )
            GameObject.DestroyImmediate(_layer);
    } 

#endif // !(EX2D_EVALUATE)

}
