// ======================================================================================
// File         : exUIElementEditor.cs
// Author       : Wu Jie 
// Last Change  : 11/03/2011 | 16:50:53 PM | Thursday,November
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// public
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exUIElement))]
public class exUIElementEditor : exPlaneEditor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    private exUIElement editElement;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();
        if ( target != editElement ) {
            editElement = target as exUIElement;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {

        // ======================================================== 
        // Base GUI 
        // ======================================================== 

        base.OnInspectorGUI();
        GUILayout.Space(20);

        EditorGUIUtility.LookLikeInspector ();
        EditorGUI.indentLevel = 1;

        // ======================================================== 
        // width
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        editElement.width = EditorGUILayout.FloatField( "Width", editElement.width );
        GUI.enabled = true;

        // ======================================================== 
        // height
        // ======================================================== 

        GUI.enabled = !inAnimMode;
        editElement.height = EditorGUILayout.FloatField( "Height", editElement.height );
        GUI.enabled = true;

        // ======================================================== 
        // check dirty 
        // ======================================================== 

        if ( EditorApplication.isPlaying == false )
            editElement.Sync();

        if ( GUI.changed )
            EditorUtility.SetDirty (editElement);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected virtual void OnSceneGUI () {

        //
        Vector3[] vertices = new Vector3[4]; 
        Vector3[] corners = new Vector3[5];
        Vector3[] controls = new Vector3[8];

        float halfWidthScaled = editElement.width * 0.5f;
        float halfHeightScaled = editElement.height * 0.5f;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        //
        switch ( editElement.anchor ) {
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

        // DELME { 
        // //
        // switch ( editElement.plane ) {
        // case exPlane.Plane.XY:
        //     vertices[0] = new Vector3 (-halfWidthScaled-offsetX,  halfHeightScaled+offsetY, 0.0f );
        //     vertices[1] = new Vector3 ( halfWidthScaled-offsetX,  halfHeightScaled+offsetY, 0.0f );
        //     vertices[2] = new Vector3 ( halfWidthScaled-offsetX, -halfHeightScaled+offsetY, 0.0f );
        //     vertices[3] = new Vector3 (-halfWidthScaled-offsetX, -halfHeightScaled+offsetY, 0.0f );
        //     break;

        // case exPlane.Plane.XZ:
        //     vertices[0] = new Vector3 (-halfWidthScaled-offsetX, 0.0f,  halfHeightScaled+offsetY );
        //     vertices[1] = new Vector3 ( halfWidthScaled-offsetX, 0.0f,  halfHeightScaled+offsetY );
        //     vertices[2] = new Vector3 ( halfWidthScaled-offsetX, 0.0f, -halfHeightScaled+offsetY );
        //     vertices[3] = new Vector3 (-halfWidthScaled-offsetX, 0.0f, -halfHeightScaled+offsetY );
        //     break;

        // case exPlane.Plane.ZY:
        //     vertices[0] = new Vector3 (0.0f,  halfHeightScaled+offsetY, -halfWidthScaled-offsetX );
        //     vertices[1] = new Vector3 (0.0f,  halfHeightScaled+offsetY,  halfWidthScaled-offsetX );
        //     vertices[2] = new Vector3 (0.0f, -halfHeightScaled+offsetY,  halfWidthScaled-offsetX );
        //     vertices[3] = new Vector3 (0.0f, -halfHeightScaled+offsetY, -halfWidthScaled-offsetX );
        //     break;
        // }
        // } DELME end 

        // 0 -- 1
        // |    |
        // 3 -- 2

        Transform trans = editElement.transform;
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
                                                     editElement.transform.rotation,
                                                     HandleUtility.GetHandleSize(pos) / 20.0f,
                                                     Vector3.zero,
                                                     Handles.DotCap
                                                   );
            HandleRezie ( i, pos, newPos );
        }
        // Handles.Label( editElement.transform.position + Vector3.up * 2,
        //                "Size = " + editElement.width + " x " + editElement.height );

        if ( EditorApplication.isPlaying == false )
            editElement.Sync();

        if ( GUI.changed )
            EditorUtility.SetDirty (editElement);
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

        float oldWidth = editElement.width;
        float oldHeight = editElement.height;
        float xRatio = 1.0f;
        float yRatio = 1.0f;

        // 0 -- 1 -- 2
        // |         |
        // 7         3
        // |         |
        // 6 -- 5 -- 4

        if ( _handleID == 0 ) {
            switch ( editElement.anchor ) {
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
            editElement.width -= dx; editElement.height += dy;
        }
        else if ( _handleID == 1 ) {
            switch ( editElement.anchor ) {
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
            editElement.height += dy;
        }
        else if ( _handleID == 2 ) {
            switch ( editElement.anchor ) {
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
            editElement.width += dx; editElement.height += dy;
        }
        else if ( _handleID == 3 ) {
            switch ( editElement.anchor ) {
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
            editElement.width += dx;
        }
        else if ( _handleID == 4 ) {
            switch ( editElement.anchor ) {
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
            editElement.width += dx; editElement.height -= dy;
        }
        else if ( _handleID == 5 ) {
            switch ( editElement.anchor ) {
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
            editElement.height -= dy;
        }
        else if ( _handleID == 6 ) {
            switch ( editElement.anchor ) {
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
            editElement.width -= dx; editElement.height -= dy;
        }
        else if ( _handleID == 7 ) {
            switch ( editElement.anchor ) {
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
            editElement.width -= dx;
        }

        float offsetX = (editElement.width - oldWidth) * xRatio;
        float offsetY = (editElement.height - oldHeight) * yRatio;
        editElement.Translate ( offsetX, offsetY );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void GetDelatSize ( Vector3 _deltaVec, out float _dx, out float _dy ) {
        _dx = _deltaVec.x; _dy = _deltaVec.y;

        // DELME { 
        // switch ( editElement.plane ) {
        // case exPlane.Plane.XY:
        //     _dx = _deltaVec.x; _dy = _deltaVec.y;
        //     break;

        // case exPlane.Plane.XZ:
        //     _dx = _deltaVec.x; _dy = _deltaVec.z;
        //     break;

        // case exPlane.Plane.ZY:
        //     _dx = _deltaVec.z; _dy = _deltaVec.y;
        //     break;
        // }
        // } DELME end 
    }
}
