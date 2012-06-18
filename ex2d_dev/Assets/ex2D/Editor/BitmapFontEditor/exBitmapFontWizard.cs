// ======================================================================================
// File         : exBitmapFontWizard.cs
// Author       : Wu Jie 
// Last Change  : 08/22/2011 | 17:14:50 PM | Monday,August
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

class exBitmapFontWizard : ScriptableWizard {

#if !(EX2D_EVALUATE)

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    public string assetPath = "";
    public string assetName = "New BitmapFont";
    public Object fontInfo;
    
    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D/Bitmap Font")]
    public static void Create () {
        ScriptableWizard.DisplayWizard<exBitmapFontWizard>("Create Bitmap Font");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSelectionChange () {
        Repaint();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        GUILayout.BeginVertical();
            string selPath = AssetDatabase.GetAssetPath( Selection.activeObject );
            bool isFontInfo = (Path.GetExtension(selPath) == ".txt" || Path.GetExtension(selPath) == ".fnt");

            // font info
            GUI.enabled = false;
            fontInfo = EditorGUILayout.ObjectField( "Font Info"
                                                    , isFontInfo ? Selection.activeObject : null 
                                                    , typeof(Object) 
                                                    , false 
                                                  );
            GUI.enabled = true;

            // Label
            if ( isFontInfo == false ) {
                GUIStyle style = new GUIStyle();
                // style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.red;

                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.Label ( "Please select a font info (.txt, .fnt) asset", style );
                GUILayout.EndHorizontal();
            }

            GUI.enabled = false;
            // asset path
            assetPath = exEditorHelper.GetCurrentDirectory();
            EditorGUILayout.TextField( "Saved Path", assetPath, GUILayout.MaxWidth(405) );

            // asset name
            assetName = "";
            if ( Selection.activeObject != null )
                assetName = Selection.activeObject.name;
            EditorGUILayout.TextField( "Asset Name", assetName, GUILayout.MaxWidth(405) );
            GUI.enabled = true;

            // Create Button
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUI.enabled = isFontInfo;
                if ( GUILayout.Button( "Create...", GUILayout.MaxWidth(100) ) ) {
                    bool doCreate = true;
                    string path = Path.Combine( assetPath, assetName + ".asset" );
                    FileInfo fileInfo = new FileInfo(path);
                    if ( fileInfo.Exists ) {
                        doCreate = EditorUtility.DisplayDialog( assetName + " already exists.",
                                                                "Do you want to overwrite the old one?",
                                                                "Yes", "No" );
                    }
                    if ( doCreate ) {
                        CreateNewBitmapFont ( assetPath, assetName, fontInfo );
                    }
                    Close();
                }
                GUI.enabled = true;
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.EndVertical();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void CreateNewBitmapFont ( string _path, string _name, Object _fontInfo ) {
        try {
            // create atlas info
            EditorUtility.DisplayProgressBar( "Creating BitmapFont...",
                                              "Creating BitmapFont Asset...",
                                              0.1f );    

            // check if there have 
            exBitmapFont bitmapFont = exBitmapFontUtility.Create( _path, _name );

            // check if we have the texture and textasset with the same name of bitmapfont 
            EditorUtility.DisplayProgressBar( "Creating BitmapFont...",
                                              "Check building ...",
                                              0.2f );    

            // if we have enough information, try to build the exBitmapFont asset
            bitmapFont.Build ( _fontInfo );
            EditorUtility.ClearProgressBar();    

            //
            Selection.activeObject = bitmapFont;
            EditorGUIUtility.PingObject(bitmapFont);
        } 
        catch ( System.Exception ) {
            EditorUtility.ClearProgressBar();    
            throw;
        }
    }
#endif // !(EX2D_EVALUATE)
}
