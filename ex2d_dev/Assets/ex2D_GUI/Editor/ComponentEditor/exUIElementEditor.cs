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
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
// public
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exUIElement))]
public class exUIElementEditor : exPlaneEditor {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    SerializedProperty widthProp;
    SerializedProperty heightProp;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();
        widthProp = serializedObject.FindProperty ("width_");
        heightProp = serializedObject.FindProperty ("height_");
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

        // ======================================================== 
        // 
        // ======================================================== 

        serializedObject.Update ();

            EditorGUIUtility.LookLikeInspector ();

            EditorGUILayout.PropertyField( widthProp, new GUIContent("Width") );
            EditorGUILayout.PropertyField( heightProp, new GUIContent("Height") );

            // DELME { 
            // if ( EditorApplication.isPlaying == false )
            //     curEdit.Sync();
            // } DELME end 

        serializedObject.ApplyModifiedProperties ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void MessageInfoField ( string _label, SerializedProperty _infoProp ) {
        ++EditorGUI.indentLevel;
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            // label
            EditorGUILayout.LabelField(_label, GUILayout.Width(50));

            // receiver
            EditorGUILayout.PropertyField( _infoProp.FindPropertyRelative ( "receiver" ), new GUIContent("") );

            // method
            EditorGUIUtility.LookLikeControls ();
                EditorGUILayout.PropertyField ( _infoProp.FindPropertyRelative ( "method" ), new GUIContent("") );
            EditorGUIUtility.LookLikeInspector ();

            // remove button
            Color oldBGColor = GUI.backgroundColor;
            Color oldCTColor = GUI.contentColor;
            GUI.backgroundColor = Color.red;
            GUI.contentColor = Color.yellow;
            if ( GUILayout.Button( "-", GUILayout.Width(20) ) )
                _infoProp.DeleteCommand();

            GUI.backgroundColor = oldBGColor;
            GUI.contentColor = oldCTColor;
        EditorGUILayout.EndHorizontal();
        --EditorGUI.indentLevel;
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected void MessageInfoListField ( string _label, SerializedProperty _infoListProp ) {
        EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.LabelField( _label );
            GUILayout.FlexibleSpace();
            if ( GUILayout.Button( "+", GUILayout.Width(20) ) ) {
                _infoListProp.InsertArrayElementAtIndex ( _infoListProp.arraySize-1 );
                SerializedProperty msgInfoProp = _infoListProp.GetArrayElementAtIndex( _infoListProp.arraySize-1 );
                msgInfoProp.FindPropertyRelative ( "receiver" ).objectReferenceValue = (target as exUIButton).gameObject;
                msgInfoProp.FindPropertyRelative ( "method" ).stringValue = "";
            }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        //
        int i = 0;
        SerializedProperty infoProp = _infoListProp.GetArrayElementAtIndex(0);
        SerializedProperty endProperty = _infoListProp.GetEndProperty();
        while ( infoProp.NextVisible(false) && !SerializedProperty.EqualContents(infoProp, endProperty) ) {
            MessageInfoField ( "[" + i + "]", infoProp );
            ++i;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected virtual void OnSceneGUI () {
        exUIElement curEdit = target as exUIElement;

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

        // DELME { 
        // if ( EditorApplication.isPlaying == false )
        //     curEdit.Sync();
        // } DELME end 

        if ( GUI.changed )
            EditorUtility.SetDirty (curEdit);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void HandleRezie ( int _handleID, Vector3 _oldPos, Vector3 _newPos ) {
        if ( _oldPos == _newPos ) {
            return;
        }

        exUIElement curEdit = target as exUIElement;

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
}
