// ======================================================================================
// File         : exUIUtility.cs
// Author       : Wu Jie 
// Last Change  : 10/30/2011 | 14:45:40 PM | Sunday,October
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

///////////////////////////////////////////////////////////////////////////////
///
/// the ui utility
///
///////////////////////////////////////////////////////////////////////////////

public static class exUIUtility {

    ///////////////////////////////////////////////////////////////////////////////
    // Button
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void CreateButtonObject ( bool _useSprite ) {
        // create button object
        GameObject buttonGO = new GameObject("Button");
        exUIButton button = buttonGO.AddComponent<exUIButton>();

        // create Background
        if ( _useSprite ) {
            GameObject backgroundGO = new GameObject("Background");
            exSprite background = backgroundGO.AddComponent<exSprite>();
            button.background = background;
            backgroundGO.transform.parent = buttonGO.transform;
        }
        else {
            GameObject backgroundGO = new GameObject("Background");
            exSpriteBorder background = backgroundGO.AddComponent<exSpriteBorder>();
            button.background = background;
            backgroundGO.transform.parent = buttonGO.transform;
        }

        // create Font
        GameObject fontGO = new GameObject("Font");
        exSpriteFont font = fontGO.AddComponent<exSpriteFont>();
        font.text = "";
        button.font = font;
        fontGO.transform.parent = buttonGO.transform;

        //
        Selection.activeObject = buttonGO;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D_GUI/Button")]
    static void CreateButtonObject_BorderBackground () {
        CreateButtonObject (false);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D_GUI/Button (Sprite)")]
    static void CreateButtonObject_SpriteBackground () {
        CreateButtonObject (true);
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Panel
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    static void CreatePanelObject ( bool _useSprite ) {
        // create panel object
        GameObject panelGO = new GameObject("Panel");
        exUIPanel panel = panelGO.AddComponent<exUIPanel>();

        // create Background
        if ( _useSprite ) {
            GameObject backgroundGO = new GameObject("Background");
            exSprite background = backgroundGO.AddComponent<exSprite>();
            panel.background = background;
            backgroundGO.transform.parent = panelGO.transform;
        }
        else {
            GameObject backgroundGO = new GameObject("Background");
            exSpriteBorder background = backgroundGO.AddComponent<exSpriteBorder>();
            panel.background = background;
            backgroundGO.transform.parent = panelGO.transform;
        }

        //
        Selection.activeObject = panelGO;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D_GUI/Panel")]
    static void CreatePanelObject_BorderBackground () {
        CreatePanelObject (false);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D_GUI/Panel (Sprite)")]
    static void CreatePanelObject_SpriteBackground () {
        CreatePanelObject (true);
    }


    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D_GUI/ScrollView")]
    static void CreateScrollViewObject () {
        // create panel object
        GameObject scrollViewGO = new GameObject("ScrollView");

        //
        GameObject clipRectGO = new GameObject("ClipRect");
        clipRectGO.transform.parent = scrollViewGO.transform;
        exClipping clipRect = clipRectGO.AddComponent<exClipping>();

        //
        GameObject contentAnchor = new GameObject("ContentAnchor");
        contentAnchor.transform.parent = clipRect.transform;

        // //
        // GameObject horizontalBarGO = new GameObject("HorizontalBar");
        // horizontalBarGO.transform.parent = scrollViewGO.transform;
        // exSpriteBorder horizontalBar = horizontalBarGO.AddComponent<exSpriteBorder>();
        // // myBorder = AssetDatabase.LoadAssetAtPath("Assets/ex2D_GUI/Resource/GUIBorder/HorizontalScrollBar.asset", 
        // //                                          typeof(exGUIBorder)) as exGUIBorder;
        // // elInfo = exAtlasDB.GetElementInfo(myBorder.textureGUID);
        // // if ( elInfo != null ) {
        // //     atlas = exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
        // //     index = elInfo.indexInAtlas;
        // // }
        // // horizontalBar.SetBorder( myBorder, atlas, index ); 
        // // horizontalBar.Rebuild ();
        // horizontalBar.anchor = exPlane.Anchor.BotLeft;
        // horizontalBar.enabled = false;

        // //
        // GameObject horizontalSliderGO = new GameObject("HorizontalSlider");
        // horizontalSliderGO.transform.parent = scrollViewGO.transform;
        // exSpriteBorder horizontalSlider = horizontalSliderGO.AddComponent<exSpriteBorder>();
        // // myBorder = AssetDatabase.LoadAssetAtPath("Assets/ex2D_GUI/Resource/GUIBorder/HorizontalSlider.asset", 
        // //                                          typeof(exGUIBorder)) as exGUIBorder;
        // // elInfo = exAtlasDB.GetElementInfo(myBorder.textureGUID);
        // // if ( elInfo != null ) {
        // //     atlas = exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
        // //     index = elInfo.indexInAtlas;
        // // }
        // // horizontalSlider.SetBorder( myBorder, atlas, index ); 
        // horizontalSlider.anchor = exPlane.Anchor.BotLeft;
        // horizontalSlider.width = 0.0f;
        // horizontalSlider.height = 0.0f;
        // horizontalSlider.enabled = false;

        // //
        // GameObject verticalBarGO = new GameObject("VerticalBar");
        // verticalBarGO.transform.parent = scrollViewGO.transform;
        // exSpriteBorder verticalBar = verticalBarGO.AddComponent<exSpriteBorder>();
        // // myBorder = AssetDatabase.LoadAssetAtPath("Assets/ex2D_GUI/Resource/GUIBorder/VerticalScrollBar.asset", 
        // //                                           typeof(exGUIBorder)) as exGUIBorder;
        // // elInfo = exAtlasDB.GetElementInfo(myBorder.textureGUID);
        // // if ( elInfo != null ) {
        // //     atlas = exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
        // //     index = elInfo.indexInAtlas;
        // // }
        // // verticalBar.SetBorder( myBorder, atlas, index ); 
        // // verticalBar.Rebuild ();
        // verticalBar.anchor = exPlane.Anchor.TopRight;
        // verticalBar.enabled = false;

        // //
        // GameObject verticalSliderGO = new GameObject("VerticalSlider");
        // verticalSliderGO.transform.parent = scrollViewGO.transform;
        // exSpriteBorder verticalSlider = verticalSliderGO.AddComponent<exSpriteBorder>();
        // // myBorder = AssetDatabase.LoadAssetAtPath("Assets/ex2D_GUI/Resource/GUIBorder/verticalSlider.asset", 
        // //                                          typeof(exGUIBorder)) as exGUIBorder;
        // // elInfo = exAtlasDB.GetElementInfo(myBorder.textureGUID);
        // // if ( elInfo != null ) {
        // //     atlas = exEditorHelper.LoadAssetFromGUID<exAtlas>(elInfo.guidAtlas);
        // //     index = elInfo.indexInAtlas;
        // // }
        // // verticalSlider.SetBorder( myBorder, atlas, index ); 
        // verticalSlider.anchor = exPlane.Anchor.TopRight; 
        // verticalSlider.width = 0.0f;
        // verticalSlider.height = 0.0f;
        // verticalSlider.enabled = false;

        //
        exUIScrollView scrollView = scrollViewGO.AddComponent<exUIScrollView>();
        scrollView.anchor = exPlane.Anchor.TopCenter;
        // scrollView.horizontalBar = horizontalBar;
        // scrollView.horizontalSlider = horizontalSlider;
        // scrollView.verticalBar = verticalBar;
        // scrollView.verticalSlider = verticalSlider;
        scrollView.contentAnchor = contentAnchor.transform;
        scrollView.clipRect = clipRect;

        //
        Selection.activeObject = scrollViewGO;
    }
}

