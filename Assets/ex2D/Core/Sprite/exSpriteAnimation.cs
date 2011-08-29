// ======================================================================================
// File         : exSpriteAnimation.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 21:32:18 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

///////////////////////////////////////////////////////////////////////////////
// exSpriteAnimState
///////////////////////////////////////////////////////////////////////////////

public enum exAnimStopAction {
    DoNothing,
    DefaultSprite,
    Hide,
    Destroy
}

[System.Serializable]
public class exSpriteAnimState {
    [System.NonSerialized] public string name;
    [System.NonSerialized] public WrapMode wrapMode;
    [System.NonSerialized] public exAnimStopAction stopAction;
    [System.NonSerialized] public exSpriteAnimClip clip;
    [System.NonSerialized] public float length;

    [System.NonSerialized] public List<float> frameTimes;
    [System.NonSerialized] public float time = 0.0f;
    // [System.NonSerialized] public float normalizedTime = 0.0f;
    [System.NonSerialized] public float speed = 1.0f;

    //
    public exSpriteAnimState ( exSpriteAnimClip _animClip ) {
        name = _animClip.name;
        wrapMode = _animClip.wrapMode;
        stopAction = _animClip.stopAction;
        clip = _animClip;
        length = _animClip.length;

        frameTimes = new List<float>(_animClip.frameInfos.Count);
        float tmp = 0.0f;
        foreach ( exSpriteAnimClip.FrameInfo fi in _animClip.frameInfos ) {
            tmp += fi.length;
            frameTimes.Add(tmp);
        }
    }
}

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

[RequireComponent (typeof(exSprite))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[AddComponentMenu("ex2D Sprite/Sprite Animation")]
public class exSpriteAnimation : MonoBehaviour {

    public List<exSpriteAnimClip> animations = new List<exSpriteAnimClip>();
    public exSpriteAnimClip defaultAnimation; // TODO: shold do something in inspector
    public bool playAutomatically = false;

    ///////////////////////////////////////////////////////////////////////////////
    // private
    ///////////////////////////////////////////////////////////////////////////////

    private bool initialized = false;
    private Dictionary<string,exSpriteAnimState> nameToState;
    private exSpriteAnimState curAnimation;
    private exSprite sprite;
    private bool playing = false;
    private bool paused = false;
    private exAtlas defaultAtlas;
    private int defaultIndex;

    ///////////////////////////////////////////////////////////////////////////////
    // static functions
    ///////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D/SpriteAnimation Object")]
    static void CreateSpriteAnimationObject () {
        GameObject go = new GameObject("SpriteAnimationObject");
        go.AddComponent<exSpriteAnimation>();
        Selection.activeObject = go;
    }
#endif

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {
        if ( initialized == false ) {
            initialized = true;

            sprite = GetComponent<exSprite>();
            defaultAtlas = sprite.atlas;
            defaultIndex = sprite.index;

            nameToState = new Dictionary<string,exSpriteAnimState> ();
            foreach ( exSpriteAnimClip clip in animations ) {
                exSpriteAnimState state = new exSpriteAnimState(clip);
                nameToState[state.name] = state;
            }

            if ( defaultAnimation != null )
                curAnimation = nameToState[defaultAnimation.name];
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Awake () {
        // DEBUG { 
        // Debug.Log("exSpriteAnimation:Awake()");
        // } DEBUG end 

        Init ();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Start () {
        if ( playAutomatically && defaultAnimation != null ) {
            Play (defaultAnimation.name);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        if ( !paused && playing && (curAnimation != null) ) {
            // advance the time and check if we trigger any animation events
            float delta = Time.deltaTime * curAnimation.speed;
            float nextTime = curAnimation.time + delta;
            curAnimation.clip.TriggerEvents( gameObject, 
                                             curAnimation.time,
                                             delta,
                                             curAnimation.wrapMode );
            curAnimation.time = nextTime;

            //
            exSpriteAnimClip.FrameInfo fi = GetCurFrameInfo();
            if ( fi != null )
                sprite.SetSprite ( fi.atlas, fi.index );

            // check if stop
            if ( ( curAnimation.wrapMode == WrapMode.Once ||
                   curAnimation.wrapMode == WrapMode.Default ) && 
                 curAnimation.time >= curAnimation.length )
            {
                Stop();
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Play ( string _name, int _index = 0 ) {
        curAnimation = GetAnimation(_name);
        if ( curAnimation != null ) {
            if ( _index >= 0 && _index < curAnimation.frameTimes.Count )
                curAnimation.time = curAnimation.frameTimes[_index];
            playing = true;
            paused = false;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void SetFrame ( string _name, int _index ) {
        curAnimation = GetAnimation(_name);
        if ( curAnimation != null &&
             _index >= 0 &&
             _index < curAnimation.clip.frameInfos.Count ) 
        {
            exSpriteAnimClip.FrameInfo fi = curAnimation.clip.frameInfos[_index]; 
            sprite.SetSprite ( fi.atlas, fi.index );
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Stop () {
        if ( curAnimation != null ) {
            //
            exAnimStopAction stopAction = curAnimation.stopAction; 

            //
            curAnimation.time = 0.0f;
            curAnimation = null;
            playing = false;
            paused = false;

            //
            switch ( stopAction ) {
            case exAnimStopAction.DoNothing:
                // Nothing todo;
                break;

            case exAnimStopAction.DefaultSprite:
                sprite.SetSprite( defaultAtlas, defaultIndex );
                break;

            case exAnimStopAction.Hide:
                sprite.enabled = false;
                break;

            case exAnimStopAction.Destroy:
                GameObject.Destroy(this);
                break;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Pause () {
        paused = true;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Resume () {
        paused = false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsPlaying ( string _name = "" ) {
        if ( string.IsNullOrEmpty(_name) )
            return playing;
        else
            return ( playing && curAnimation.name == _name );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsPaused ( string _name = "" ) {
        if ( string.IsNullOrEmpty(_name) )
            return paused;
        else
            return (paused && curAnimation.name == _name);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public exSpriteAnimState GetAnimation ( string _name ) {
        Init ();
        // DISABLE { 
        // if ( nameToState == null ) {
        //     Debug.LogError ("The exSpriteAnimation is not Awake yet. Please put it before Default Time in Menu/Edit/Project Settings/Script Execution Order");
        //     return null;
        // }
        // } DISABLE end 
        return nameToState[_name];
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public exSpriteAnimClip.FrameInfo GetCurFrameInfo () {
        if ( curAnimation != null ) {
            float wrappedTime = curAnimation.clip.WrapSeconds(curAnimation.time, curAnimation.wrapMode);
            int index = curAnimation.frameTimes.BinarySearch(wrappedTime);
            if ( index < 0 ) {
                index = ~index;
            }
            if ( index < curAnimation.clip.frameInfos.Count )
                return curAnimation.clip.frameInfos[index];
        }
        return null;
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public exSpriteAnimState AddAnimation ( exSpriteAnimClip _animClip ) {
        // if we already have the animation, just return the animation state
        if ( animations.IndexOf(_animClip) != -1 ) {
            return nameToState[_animClip.name];
        }

        //
        animations.Add (_animClip);
        exSpriteAnimState state = new exSpriteAnimState(_animClip);
        nameToState[state.name] = state;
        return state;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void RemoveAnimation ( exSpriteAnimClip _animClip ) {
        // if we already have the animation, just return the animation state
        if ( animations.IndexOf(_animClip) == -1 ) {
            return; 
        }

        //
        animations.Remove (_animClip);
        nameToState.Remove (_animClip.name);
    }
}

