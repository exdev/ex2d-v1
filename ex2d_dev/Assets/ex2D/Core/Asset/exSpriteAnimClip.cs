// ======================================================================================
// File         : exSpriteAnimClip.cs
// Author       : Wu Jie 
// Last Change  : 09/03/2011 | 18:29:15 PM | Saturday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
/// \class exSpriteAnimClip
///
/// The sprite animation clip asset used in exSpriteAnimation component. 
///////////////////////////////////////////////////////////////////////////////

public class exSpriteAnimClip : ScriptableObject {

    public class EventInfoComparer: IComparer<EventInfo> {
        public int Compare( EventInfo _x, EventInfo _y ) {
            if ( _x.time > _y.time )
                return 1;
            else if ( _x.time == _y.time  )
                return 0;
            else
                return -1;
        }
    }

    // ------------------------------------------------------------------ 
    /// \enum StopAction
    /// The action type used when animation stpped
    // ------------------------------------------------------------------ 

    public enum StopAction {
        DoNothing,      ///< do nothing
        DefaultSprite,  ///< set to default sprite when the sprite animation stopped
        Hide,           ///< hide the sprite when the sprite animation stopped
        Destroy         ///< destroy the GameObject the sprite belongs to when the sprite animation stopped
    }

    // ------------------------------------------------------------------ 
    /// \class FrameInfo
    /// The structure to descrip a frame in the sprite animation clip
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class FrameInfo {
        public string textureGUID = ""; ///< the guid of the referenced texture
        public float length = 0.0f;     ///< the length of the frame in seconds
        public exAtlas atlas;           ///< the atlas used in this frame
        public int index;               ///< the index of the atlas used in this frame
    }

    // ------------------------------------------------------------------ 
    /// \class FrameInfo
    /// The structure to descrip an event in the sprite animation clip
    // ------------------------------------------------------------------ 

    [System.Serializable]
    public class EventInfo {
        public enum ParamType {
            NONE,
            STRING,
            FLOAT,
            INT,
            BOOL,
            OBJECT
        }

        public float time = 0.0f;
        public string methodName = "";
        public ParamType paramType = ParamType.NONE;
        public string stringParam = "";
        public float floatParam = 0.0f;
        public int intParam = -1;
        public bool boolParam = false;
        public Object objectParam = null;
        public SendMessageOptions msgOptions = SendMessageOptions.RequireReceiver;
    }

    public WrapMode wrapMode = WrapMode.Once;
    public StopAction stopAction = StopAction.DoNothing;
    public float length = 1.0f;
    public List<FrameInfo> frameInfos = new List<FrameInfo>();
    public List<EventInfo> eventInfos = new List<EventInfo>();

    // editor only
    public float editorScale = 1.0f;
    public float editorOffset = 0.0f;
    public float editorSpeed = 1.0f;
    public bool editorNeedRebuild = false;

    private EventInfoComparer eventInfoComparer = new EventInfoComparer();
    private EventInfo tmpEventInfo = new EventInfo();

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void AddEvent ( EventInfo _e ) {
        //
        int index = eventInfos.BinarySearch( _e, eventInfoComparer );
        if ( index < 0 ) {
            index = ~index;
        }

        eventInfos.Insert( index, _e );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void RemoveEvent ( EventInfo _e ) {
        eventInfos.Remove( _e );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float WrapSeconds ( float _seconds, WrapMode _wrapMode ) {
        float t = Mathf.Abs(_seconds);
        if ( _wrapMode == WrapMode.Loop ) {
            t %= length;
        }
        else if ( _wrapMode == WrapMode.PingPong ) {
            int cnt = (int)(t/length);
            t %= length;
            if ( cnt % 2 == 1 ) {
                t = length - t;
            }
        }
        else {
            t = Mathf.Clamp( t, 0.0f, length );
        }
        return t;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void TriggerEvents ( GameObject _gameObject, 
                                float _start, 
                                float _delta, 
                                WrapMode _wrapMode ) 
    {
        if ( eventInfos.Count == 0 )
            return;
        if ( _delta == 0.0f )
            return;

        int index = 0;

        // WrapSeconds
        float t = WrapSeconds(_start,_wrapMode); 

        // start index
        tmpEventInfo.time = t;
        index = eventInfos.BinarySearch( tmpEventInfo, eventInfoComparer );
        if ( index < 0 ) {
            index = ~index;
        }

        // forward
        if ( _delta > 0.0f ) {
            if ( t + _delta > length ) {
                float rest = t + _delta - length;
                if ( _wrapMode == WrapMode.Loop ) {
                    ForwardTriggerEvents ( _gameObject, index, t, length, false );
                    ForwardTriggerEvents ( _gameObject, index, 0.0f, rest, true );
                }
                else if ( _wrapMode == WrapMode.PingPong ) {
                    ForwardTriggerEvents ( _gameObject, index, t, length, false );
                    BackwardTriggerEvents ( _gameObject, index, length, length - rest, false );
                }
                else {
                    ForwardTriggerEvents ( _gameObject, index, t, length, false );
                }
            }
            else {
                ForwardTriggerEvents ( _gameObject, index, t, t + _delta, false );
            }
        }
        // backward
        else {
            if ( t + _delta < 0.0f ) {
                float rest = 0.0f - (t + _delta);
                if ( _wrapMode == WrapMode.Loop ) {
                    BackwardTriggerEvents ( _gameObject, index, t, 0.0f, false );
                    BackwardTriggerEvents ( _gameObject, index, length, length - rest, true );
                }
                else if ( _wrapMode == WrapMode.PingPong ) {
                    BackwardTriggerEvents ( _gameObject, index, t, 0.0f, false );
                    ForwardTriggerEvents ( _gameObject, index, 0.0f, rest, false );
                }
                else {
                    BackwardTriggerEvents ( _gameObject, index, t, 0.0f, false );
                }
            }
            else {
                BackwardTriggerEvents ( _gameObject, index, t, t + _delta, false );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void ForwardTriggerEvents ( GameObject _gameObject, 
                                       int _index, 
                                       float _start, 
                                       float _end,
                                       bool _includeStart ) 
    {
        for ( int i = _index; i < eventInfos.Count; ++i ) {
            EventInfo ei = eventInfos[i];
            if ( ei.time == _start && _includeStart == false )
                continue;

            if ( ei.time <= _end ) {
                Trigger ( _gameObject, ei );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void BackwardTriggerEvents ( GameObject _gameObject, 
                                       int _index, 
                                       float _start, 
                                       float _end,
                                       bool _includeStart )
    {
        for ( int i = _index; i > eventInfos.Count; --i ) {
            EventInfo ei = eventInfos[i];
            if ( ei.time == _start && _includeStart == false )
                continue;

            if ( ei.time <= _end ) {
                Trigger ( _gameObject, ei );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Trigger ( GameObject _gameObject, EventInfo _eventInfo ) {
        if ( _eventInfo.methodName == "" )
            return;

        switch ( _eventInfo.paramType ) {
        case EventInfo.ParamType.NONE:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.STRING:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.stringParam, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.FLOAT:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.floatParam, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.INT:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.intParam, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.BOOL:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.boolParam, _eventInfo.msgOptions );
            break;

        case EventInfo.ParamType.OBJECT:
            _gameObject.SendMessage ( _eventInfo.methodName, _eventInfo.objectParam, _eventInfo.msgOptions );
            break;
        }
    }
}

