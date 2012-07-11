// ======================================================================================
// File         : exSpriteAnimClipWizard.cs
// Author       : Wu Jie 
// Last Change  : 08/22/2011 | 15:19:58 PM | Monday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
// defines
///////////////////////////////////////////////////////////////////////////////

class AboutWizard : ScriptableWizard {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/Misc/About")]
    public static void Create () {
        ScriptableWizard.DisplayWizard<AboutWizard>("About ex2D");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        string logoPath = "Assets/ex2D/Editor/Resource/ex2d_logo.png";
        float logoWidth = 150.0f;  
        float logoHeight = 150.0f;  

        float x = position.width * 0.5f - logoWidth * 0.5f;
        GUI.DrawTexture( new Rect( x, 10.0f, logoWidth, logoHeight ), 
                         (Texture2D)AssetDatabase.LoadAssetAtPath( logoPath, typeof(Texture2D) ) );
        GUILayoutUtility.GetRect ( logoWidth, logoHeight );

        string version = "ex2D v1.2.6 (120712)";
#if EX2D_EVALUATE
        version += " Evaluate";
#endif
        GUILayout.Space (10);
        GUILayout.BeginHorizontal();
            GUILayout.Space (10);
            GUILayout.Label(version);
        GUILayout.EndHorizontal();

        string support = "";
#if UNITY_FLASH
        support += "Flash ";
#endif
#if UNITY_IPHONE
        support += "iPhone ";
#endif
        GUILayout.BeginHorizontal();
            GUILayout.Space (10);
            GUILayout.Label("Support Platform: " + support);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.Space (10);
            GUILayout.Label("Develop by: exDev Studio (www.ex-dev.com)");
        GUILayout.EndHorizontal();
    }
}
