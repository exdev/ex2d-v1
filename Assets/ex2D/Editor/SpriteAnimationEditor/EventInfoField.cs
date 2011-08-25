// ======================================================================================
// File         : EventInfoField.cs
// Author       : Wu Jie 
// Last Change  : 07/06/2011 | 12:30:53 PM | Wednesday,July
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

partial class exSpriteAnimClipEditor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void EventInfoViewField ( Rect _rect, exSpriteAnimClip _animClip ) {
        float maxHeight = _rect.height-3;
        Rect lastRect = new Rect( -100, _rect.y, 7, maxHeight );
        Rect curRect = new Rect( -100, _rect.y, 7, maxHeight );
        exSpriteAnimClip.EventInfo lastClicked = null;

        foreach ( exSpriteAnimClip.EventInfo eventInfo in _animClip.eventInfos ) {
            float at = _rect.x + (eventInfo.time / _animClip.length) * totalWidth;
            lastRect = curRect;
            curRect = new Rect( at - 4, _rect.y, 7, maxHeight );
            if ( exIntersection2D.RectRect( lastRect, curRect ) ) {
                curRect.height = Mathf.Max( lastRect.height - 5.0f, 10.0f );
            }
            exSpriteAnimClip.EventInfo clicked_ei = null;
            
            if ( at >= spriteAnimClipRect.x && at <= spriteAnimClipRect.xMax )
                clicked_ei = EventInfoField ( curRect, eventInfo );
            if ( clicked_ei != null )
                lastClicked = clicked_ei;
        }

        // ======================================================== 
        Event e = Event.current;
        // ======================================================== 

        if ( lastClicked != null ) {
            if ( e.command || e.control ) {
                ToggleSelected(lastClicked);
            }
            else {
                inDraggingEventInfoState = true;
                bool selected = selectedEventInfos.IndexOf(lastClicked) != -1;
                if ( selected == false ) {
                    if ( e.command == false && e.control == false ) {
                        selectedEventInfos.Clear();
                        AddSelected(lastClicked);
                    }
                }
            }

            e.Use();
            Repaint();
        }

        // ======================================================== 
        e = Event.current;
        // ======================================================== 

        if ( e.isMouse ) {
            if ( _rect.Contains(e.mousePosition) ) {
                // mouse down
                if ( e.type == EventType.MouseDown 
                     && e.button == 0 
                     && e.clickCount == 1 ) 
                {
                    GUIUtility.keyboardControl = -1; // remove any keyboard control
                    selectedFrameInfos.Clear();

                    mouseDownPos = e.mousePosition;
                    inRectSelectEventState = true;
                    UpdateSelectRect (mouseDownPos);
                    ConfirmRectSelectEventInfo();
                    Repaint();

                    e.Use();
                }

                // double click mouse down
                if ( e.type == EventType.MouseDown 
                     && e.clickCount == 2
                     && e.command == false ) 
                {
                    exSpriteAnimClip.EventInfo eventInfo = new exSpriteAnimClip.EventInfo();
                    eventInfo.time = ((e.mousePosition.x - _rect.x) * _animClip.length) / totalWidth; 
                    _animClip.AddEvent (eventInfo);
                    EditorUtility.SetDirty(_animClip);
                    e.Use();
                }
            } 
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    exSpriteAnimClip.EventInfo EventInfoField ( Rect _rect, exSpriteAnimClip.EventInfo _eventInfo ) {
        bool selected = selectedEventInfos.IndexOf(_eventInfo) != -1;

        //
        if ( selected ) {
            exEditorHelper.DrawRect ( _rect,
                                    new Color ( 0.0f, 0.3f, 1.0f, 1.0f ),
                                    new Color ( 1.0f, 1.0f, 1.0f, 1.0f ) );
        }
        else if ( _eventInfo.methodName == "" ) {
            exEditorHelper.DrawRect ( _rect,
                                    new Color ( 1.0f, 0.0f, 0.0f, 0.5f ),
                                    new Color ( 0.2f, 0.2f, 0.2f, 1.0f ) );
        }
        else {
            exEditorHelper.DrawRect ( _rect,
                                    new Color ( 0.0f, 1.0f, 0.0f, 1.0f ),
                                    new Color ( 0.2f, 0.2f, 0.2f, 1.0f ) );
        }

        //
        float at = _rect.x + Mathf.Round(_rect.width/2.0f);
        Vector2 start = new Vector2( at, _rect.yMax );
        Vector2 end = new Vector2( at, _rect.y + eventInfoViewRect.height );
        exEditorHelper.DrawLine ( start, end, Color.black, 1.0f ); 

        // ======================================================== 
        Event e = Event.current;
        // ======================================================== 

        if ( e.isMouse ) {
            if ( e.type == EventType.MouseDown && e.button == 0 ) {
                if ( _rect.Contains(e.mousePosition) ) {
                    GUIUtility.keyboardControl = -1; // remove any keyboard control
                    selectedFrameInfos.Clear();

                    return _eventInfo;
                }
            }
        }
        return null;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void EventInfoEditField () {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        GUILayout.Label( "EventInfo Inspector", style );

        if ( selectedEventInfos.Count == 1 ) {
            exSpriteAnimClip.EventInfo ei = selectedEventInfos[0];
            float newTime = EditorGUILayout.FloatField( "Time", ei.time, GUILayout.Width(200) );
            ei.time = Mathf.Clamp( newTime, 0.0f, curEdit.length );
            // TODO: we can use MethodInfo in C# when select a GameObject with exSpriteAnimation.cs in it { 
            ei.methodName = EditorGUILayout.TextField ( "Method Name", ei.methodName, GUILayout.Width(300) );
            ei.paramType = (exSpriteAnimClip.EventInfo.ParamType)EditorGUILayout.EnumPopup ( "Param Type", 
                                                                                           ei.paramType,
                                                                                           GUILayout.Width(200) );
            switch ( ei.paramType ) {
            case exSpriteAnimClip.EventInfo.ParamType.STRING: 
                ei.stringParam = EditorGUILayout.TextField ( "Parameter", ei.stringParam, GUILayout.Width(200) );
                break;
            case exSpriteAnimClip.EventInfo.ParamType.FLOAT: 
                ei.floatParam = EditorGUILayout.FloatField ( "Parameter", ei.floatParam, GUILayout.Width(200) );
                break;
            case exSpriteAnimClip.EventInfo.ParamType.INT: 
                ei.intParam = EditorGUILayout.IntField ( "Parameter", ei.intParam, GUILayout.Width(200)  );
                break;
            case exSpriteAnimClip.EventInfo.ParamType.BOOL: 
                ei.boolParam = EditorGUILayout.Toggle ( "Parameter", ei.boolParam, GUILayout.Width(200)  );
                break;
            case exSpriteAnimClip.EventInfo.ParamType.OBJECT: 
                ei.objectParam = EditorGUILayout.ObjectField ( "Parameter"
                                                               , ei.objectParam
                                                               , typeof(Object)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                               , true
#endif
                                                               , GUILayout.Width(200) 
                                                             );
                break;
            }
            // } TODO end 
            ei.msgOptions = (SendMessageOptions)EditorGUILayout.EnumPopup ( "Send Message Options", 
                                                                            ei.msgOptions,
                                                                            GUILayout.Width(200) );
        }
        else {
            for ( int i = 0; i < selectedEventInfos.Count; ++i ) {
                exSpriteAnimClip.EventInfo ei = selectedEventInfos[i];
                string text = "Time [" + i + "]"; 
                float newTime = EditorGUILayout.FloatField( text, ei.time, GUILayout.Width(200) );
                ei.time = Mathf.Clamp( newTime, 0.0f, curEdit.length );
            }
        }
    }
}

