// ======================================================================================
// File         : exClippingEditor.cs
// Author       : Wu Jie 
// Last Change  : 06/03/2012 | 00:36:23 AM | Sunday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exClipping))]
public class exClippingEditor : exPlaneEditor {

#if !(EX2D_EVALUATE)

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exClipping curEdit;
    private bool showClipList = true;
    private bool showMaterialList = true;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();
        if ( target != curEdit ) {
            curEdit = target as exClipping;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {
        base.OnInspectorGUI();
        GUILayout.Space(20);

        EditorGUIUtility.LookLikeInspector ();
        EditorGUI.indentLevel = 1;

        // DELME { 
        // // ======================================================== 
        // // center
        // // ======================================================== 

        // EditorGUIUtility.LookLikeControls ();
        // GUI.enabled = !inAnimMode;
        // curEdit.center = EditorGUILayout.Vector2Field( "Center", curEdit.center );
        // GUI.enabled = true;
        // EditorGUIUtility.LookLikeInspector ();
        // } DELME end 

        // ======================================================== 
        // width
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        curEdit.width = EditorGUILayout.FloatField( "Width", curEdit.width );
        GUI.enabled = true;

        // ======================================================== 
        // height
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        curEdit.height = EditorGUILayout.FloatField( "Height", curEdit.height );
        GUI.enabled = true;

        // ======================================================== 
        // 
        // ======================================================== 

        curEdit.isDyanmic = EditorGUILayout.Toggle( "Is Dyanmic", curEdit.isDyanmic );

        // ======================================================== 
        // clip material list 
        // ======================================================== 

        EditorGUI.indentLevel = 0;
        showMaterialList = EditorGUILayout.Foldout(showMaterialList, "Clip Materials");
        if ( showMaterialList ) {
            EditorGUI.indentLevel = 2;
            GUI.enabled = false;
            for ( int i = 0; i < curEdit.clipMaterialList.Count; ++i ) {
                Material clipMat = curEdit.clipMaterialList[i];
                EditorGUILayout.ObjectField( "[" + i + "]"
                                             , clipMat 
                                             , typeof(Material)
                                             , true 
                                           );
            }
            GUI.enabled = true;
        }

        // ======================================================== 
        // clip list
        // ======================================================== 

        Rect lastRect = new Rect( 0, 0, 1, 1 );
        Rect dropRect = new Rect( 0, 0, 1, 1 );

        EditorGUI.indentLevel = 0;
        showClipList = EditorGUILayout.Foldout(showClipList, "Clip Objects");
        if ( showClipList ) {
            EditorGUI.indentLevel = 2;
            int idxRemoved = -1;
            for ( int i = 0; i < curEdit.planeInfoList.Count; ++i ) {
                GUILayout.BeginHorizontal();
                exPlane curPlane = curEdit.planeInfoList[i].plane;
                exPlane newPlane = (exPlane)EditorGUILayout.ObjectField( "[" + i + "]"
                                                                         , curPlane 
                                                                         , typeof(exPlane)
                                                                         , true 
                                                                       );
                if ( newPlane != curPlane &&
                     curEdit.HasPlaneInfo(newPlane) == false ) 
                {
                    curEdit.RemovePlaneInEditor(curPlane);
                    curEdit.InsertPlaneInEditor( i, newPlane );
                }

                if ( GUILayout.Button("-", GUILayout.Width(15), GUILayout.Height(15) ) ) {
                    idxRemoved = i;
                }
                GUILayout.EndHorizontal();
            }

            // if we have item to remove
            if ( idxRemoved != -1 ) {
                curEdit.RemovePlaneInEditor(curEdit.planeInfoList[idxRemoved].plane);
            }

            EditorGUI.indentLevel = 1;
            EditorGUILayout.Space ();

            lastRect = GUILayoutUtility.GetLastRect ();  
            dropRect.x = lastRect.x + 30;
            dropRect.y = lastRect.yMax;
            dropRect.width = lastRect.xMax - 30 - 4;
            dropRect.height = 20;

            exEditorHelper.DrawRect( dropRect, new Color( 0.2f, 0.2f, 0.2f, 1.0f ), new Color( 0.5f, 0.5f, 0.5f, 1.0f ) );
            GUILayout.Space (20);

            // ======================================================== 
            // drag and drop 
            // ======================================================== 

            if ( dropRect.Contains(Event.current.mousePosition) ) {
                if ( Event.current.type == EventType.DragUpdated ) {
                    // Show a copy icon on the drag
                    foreach ( Object o in DragAndDrop.objectReferences ) {
                        if ( o is GameObject ) {
                            if ( (o as GameObject).GetComponent<exPlane>() != null ) {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                break;
                            }
                        }
                    }
                }
                else if ( Event.current.type == EventType.DragPerform ) {
                    DragAndDrop.AcceptDrag();
                    foreach ( Object o in DragAndDrop.objectReferences ) {
                        if ( o is GameObject ) {
                            exPlane plane = (o as GameObject).GetComponent<exPlane>();
                            if ( plane )
                                curEdit.AddPlaneInEditor(plane);
                        }
                    }
                    GUI.changed = true;
                }
            }
        }
        EditorGUILayout.Space ();

        // ======================================================== 
        // if changes
        // ======================================================== 

        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
	}

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSceneGUI () {

        //
        Vector3[] vertices = new Vector3[4]; 
        Vector3[] corners = new Vector3[5];
        Vector3[] controls = new Vector3[8];

        float halfWidthScaled = curEdit.width * 0.5f;
        float halfHeightScaled = curEdit.height * 0.5f;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        //
        switch ( curEdit.anchor ) {
        case exPlane.Anchor.TopLeft     : offsetX = -halfWidthScaled;   offsetY = -halfHeightScaled;  break;
        case exPlane.Anchor.TopCenter   : offsetX = 0.0f;               offsetY = -halfHeightScaled;  break;
        case exPlane.Anchor.TopRight    : offsetX = halfWidthScaled;    offsetY = -halfHeightScaled;  break;

        case exPlane.Anchor.MidLeft     : offsetX = -halfWidthScaled;   offsetY = 0.0f;               break;
        case exPlane.Anchor.MidCenter   : offsetX = 0.0f;               offsetY = 0.0f;               break;
        case exPlane.Anchor.MidRight    : offsetX = halfWidthScaled;    offsetY = 0.0f;               break;

        case exPlane.Anchor.BotLeft     : offsetX = -halfWidthScaled;   offsetY = halfHeightScaled;   break;
        case exPlane.Anchor.BotCenter   : offsetX = 0.0f;               offsetY = halfHeightScaled;   break;
        case exPlane.Anchor.BotRight    : offsetX = halfWidthScaled;    offsetY = halfHeightScaled;   break;

        default                         : offsetX = 0.0f;               offsetY = 0.0f;               break;
        }

        //
        vertices[0] = new Vector3 (-halfWidthScaled-offsetX,  halfHeightScaled+offsetY, 0.0f );
        vertices[1] = new Vector3 ( halfWidthScaled-offsetX,  halfHeightScaled+offsetY, 0.0f );
        vertices[2] = new Vector3 ( halfWidthScaled-offsetX, -halfHeightScaled+offsetY, 0.0f );
        vertices[3] = new Vector3 (-halfWidthScaled-offsetX, -halfHeightScaled+offsetY, 0.0f );

        // 0 -- 1
        // |    |
        // 3 -- 2

        Transform trans = curEdit.transform;
        corners[0] = trans.localToWorldMatrix * new Vector4 ( vertices[0].x, vertices[0].y, vertices[0].z, 1.0f );
        corners[1] = trans.localToWorldMatrix * new Vector4 ( vertices[1].x, vertices[1].y, vertices[1].z, 1.0f );
        corners[2] = trans.localToWorldMatrix * new Vector4 ( vertices[2].x, vertices[2].y, vertices[2].z, 1.0f );
        corners[3] = trans.localToWorldMatrix * new Vector4 ( vertices[3].x, vertices[3].y, vertices[3].z, 1.0f );
        corners[4] = corners[0];
        Handles.DrawPolyLine( corners );

        // 0 -- 1 -- 2
        // |         |
        // 7         3
        // |         |
        // 6 -- 5 -- 4

        controls[0] = corners[0];
        controls[1] = (corners[0] + corners[1])/2.0f;
        controls[2] = corners[1];
        controls[3] = (corners[1] + corners[2])/2.0f;
        controls[4] = corners[2];
        controls[5] = (corners[2] + corners[3])/2.0f;
        controls[6] = corners[3];
        controls[7] = (corners[3] + corners[0])/2.0f;

        // TEMP: not sure this is good { 
        Event e = Event.current;
        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            Undo.RegisterSceneUndo("ex2D.Scene");
        }
        // } TEMP end 

        //
        for ( int i = 0; i < controls.Length; ++i ) {
            Vector3 pos = controls[i];
            Handles.color = new Color( 1.0f, 1.0f, 0.0f, 0.5f );

            Vector3 newPos = Handles.FreeMoveHandle( pos,
                                                     curEdit.transform.rotation,
                                                     HandleUtility.GetHandleSize(pos) / 20.0f,
                                                     Vector3.zero,
                                                     Handles.DotCap
                                                   );
            HandleRezie ( i, pos, newPos );
        }
        // Handles.Label( curEdit.transform.position + Vector3.up * 2,
        //                "Size = " + curEdit.width + " x " + curEdit.height );

        if (GUI.changed)
            EditorUtility.SetDirty (curEdit);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void AddAnimationHelper () {
        curEdit.gameObject.AddComponent<exClippingAnimHelper>();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleRezie ( int _handleID, Vector3 _oldPos, Vector3 _newPos ) {
        if ( _oldPos == _newPos ) {
            return;
        }

        //
        float dx, dy;
        GetDelatSize ( _newPos - _oldPos, out dx, out dy );

        float oldWidth = curEdit.width;
        float oldHeight = curEdit.height;
        float xRatio = 1.0f;
        float yRatio = 1.0f;

        // 0 -- 1 -- 2
        // |         |
        // 7         3
        // |         |
        // 6 -- 5 -- 4

        if ( _handleID == 0 ) {
            switch ( curEdit.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = -1.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = -0.5f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = -0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = -1.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = -0.5f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = -0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = -1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = -0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = -0.0f; yRatio = 0.0f; break;
            }
            curEdit.width -= dx; curEdit.height += dy;
        }
        else if ( _handleID == 1 ) {
            switch ( curEdit.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = 0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 0.0f; yRatio = 0.0f; break;
            }
            curEdit.height += dy;
        }
        else if ( _handleID == 2 ) {
            switch ( curEdit.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.5f; yRatio = 1.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 1.0f; yRatio = 1.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.5f; yRatio = 0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = 1.0f; yRatio = 0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 1.0f; yRatio = 0.0f; break;
            }
            curEdit.width += dx; curEdit.height += dy;
        }
        else if ( _handleID == 3 ) {
            switch ( curEdit.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidRight:    xRatio = 1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 1.0f; yRatio = 0.0f; break;
            }
            curEdit.width += dx;
        }
        else if ( _handleID == 4 ) {
            switch ( curEdit.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.5f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 1.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.5f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = 1.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.5f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 1.0f; yRatio = -1.0f; break;
            }
            curEdit.width += dx; curEdit.height -= dy;
        }
        else if ( _handleID == 5 ) {
            switch ( curEdit.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = 0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = 0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = 0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = 0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = 0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = 0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = 0.0f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = 0.0f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = 0.0f; yRatio = -1.0f; break;
            }
            curEdit.height -= dy;
        }
        else if ( _handleID == 6 ) {
            switch ( curEdit.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = -1.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = -0.5f; yRatio = -0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = -0.0f; yRatio = -0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = -1.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidCenter:   xRatio = -0.5f; yRatio = -0.5f; break;
            case exPlane.Anchor.MidRight:    xRatio = -0.0f; yRatio = -0.5f; break;
            case exPlane.Anchor.BotLeft:     xRatio = -1.0f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = -0.5f; yRatio = -1.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = -0.0f; yRatio = -1.0f; break;
            }
            curEdit.width -= dx; curEdit.height -= dy;
        }
        else if ( _handleID == 7 ) {
            switch ( curEdit.anchor ) {
            case exPlane.Anchor.TopLeft:     xRatio = -1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.TopCenter:   xRatio = -0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.TopRight:    xRatio = -0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidLeft:     xRatio = -1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidCenter:   xRatio = -0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.MidRight:    xRatio = -0.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotLeft:     xRatio = -1.0f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotCenter:   xRatio = -0.5f; yRatio = 0.0f; break;
            case exPlane.Anchor.BotRight:    xRatio = -0.0f; yRatio = 0.0f; break;
            }
            curEdit.width -= dx;
        }

        float offsetX = (curEdit.width - oldWidth) * xRatio;
        float offsetY = (curEdit.height - oldHeight) * yRatio;
        curEdit.Translate ( offsetX, offsetY );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void GetDelatSize ( Vector3 _deltaVec, out float _dx, out float _dy ) {
        _dx = _deltaVec.x; _dy = _deltaVec.y;
    }

#else // !(EX2D_EVALUATE)

	public override void OnInspectorGUI () {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        GUILayout.Label( "Unavailable in Evaluate Version", style );
	}

#endif // !(EX2D_EVALUATE)

}
