// ======================================================================================
// File         : exTimebasedCurveInfo.cs
// Author       : Wu Jie 
// Last Change  : 08/06/2011 | 21:39:08 PM | Saturday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

///////////////////////////////////////////////////////////////////////////////
// exTimebasedCurveInfo
///////////////////////////////////////////////////////////////////////////////

public class exTimebasedCurveInfo : ScriptableObject {

    public enum WrapMode {
        Once,
        Loop,
        PingPong,
    } 

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public WrapMode wrapMode = WrapMode.Once;
    public float time = 1.0f;
    public bool useRealTime = false;
    public bool useEaseCurve = true;
    public exEase.Type easeCurveType = exEase.Type.Linear;
    public AnimationCurve animationCurve = AnimationCurve.Linear( 0.0f, 0.0f, 1.0f, 1.0f );

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D Curve Info (Timebased)")]
    public static void Create () {
        exTimebasedCurveInfo newCurve 
            = exTimebasedCurveInfo.Create ( exEditorRuntimeHelper.GetCurrentDirectory(), "New CurveInfo" );
        EditorGUIUtility.PingObject(newCurve);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public static exTimebasedCurveInfo Create ( string _path, string _name ) {
        //
        if ( new DirectoryInfo(_path).Exists == false ) {
            Debug.LogError ( "can't create asset, path not found" );
            return null;
        }
        if ( string.IsNullOrEmpty(_name) ) {
            Debug.LogError ( "can't create asset, the name is empty" );
            return null;
        }
        string assetPath = Path.Combine( _path, _name + ".asset" );

        // check if create the asset
        FileInfo fileInfo = new FileInfo(assetPath);
        if ( fileInfo.Exists ) {
            if ( EditorUtility.DisplayDialog( _name + " already exists.",
                                              "Do you want to overwrite the old one?",
                                              "Yes", 
                                              "No" ) == false )
            {
                return null;
            }
        }

        //
        exTimebasedCurveInfo newCurve = ScriptableObject.CreateInstance<exTimebasedCurveInfo>();
        AssetDatabase.CreateAsset(newCurve, assetPath);
        Selection.activeObject = newCurve;

        return newCurve;
    }
#endif
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

[System.Serializable]
public class exTimebasedCurve {

    public exTimebasedCurveInfo data;

    private exEase.easeCallback callback;
    private float startTime = 0.0f; 
    private bool reverse = false;
    private bool timeup = false;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Start () {
        callback = data.useEaseCurve ? exEase.TypeToFunction(data.easeCurveType) : data.animationCurve.Evaluate;
        startTime = data.useRealTime ? Time.realtimeSinceStartup : Time.time;
        reverse = false;
        timeup = false;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public bool IsTimeUp () {
        return timeup;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public float Step () {
        float timespan = (data.useRealTime ? Time.realtimeSinceStartup : Time.time) - startTime;

        //
        if ( timespan >= data.time ) {
            if ( data.wrapMode == exTimebasedCurveInfo.WrapMode.Once ) {
                timeup = true;
                return 1.0f;
            }
            else if ( data.wrapMode == exTimebasedCurveInfo.WrapMode.Loop ) {
                startTime += data.time;
                timespan = timespan % data.time;
            }
            else if ( data.wrapMode == exTimebasedCurveInfo.WrapMode.PingPong ) {
                startTime += data.time;
                timespan = timespan % data.time;
                reverse = !reverse;
            }
        }

        //
        if ( reverse )
            timespan = data.time - timespan;

        float ratio = Mathf.Clamp ( timespan/data.time, 0.0f, 1.0f );
        return callback(ratio);
    }
}
