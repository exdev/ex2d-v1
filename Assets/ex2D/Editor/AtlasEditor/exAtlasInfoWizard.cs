// ======================================================================================
// File         : exAtlasInfoWizard.cs
// Author       : Wu Jie 
// Last Change  : 08/22/2011 | 13:58:59 PM | Monday,August
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

public class exAtlasInfoWizard : ScriptableWizard {

    ///////////////////////////////////////////////////////////////////////////////
    // defines
    ///////////////////////////////////////////////////////////////////////////////

    static int[] sizeList = new int[] { 
        32, 64, 128, 256, 512, 1024, 2048, 4096 
    };
    static string[] sizeTextList = new string[] { 
        "32px", "64px", "128px", "256px", "512px", "1024px", "2048px", "4096px" 
    };

    public string assetPath = "";
    public string assetName = "New Atlas";
    public int width = 512;
    public int height = 512;
    
    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("Assets/Create/ex2D Atlas Info")]
    public static void Create () {
        ScriptableWizard.DisplayWizard<exAtlasInfoWizard>("Create Atlas Info");
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
            assetPath = exEditorRuntimeHelper.GetCurrentDirectory();
            assetPath = EditorGUILayout.TextField( "Saved Path", assetPath, GUILayout.MaxWidth(405) );

            assetName = Path.GetFileNameWithoutExtension(assetName);
            assetName = EditorGUILayout.TextField( "Asset Name", assetName, GUILayout.MaxWidth(405) );

            width = EditorGUILayout.IntPopup ( "Width", width, sizeTextList, sizeList, GUILayout.MaxWidth(200) );
            height = EditorGUILayout.IntPopup ( "Height", height, sizeTextList, sizeList, GUILayout.MaxWidth(200) );

            // Create Button
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
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
                        CreateAtlasInfo ( assetPath, assetName, width, height );
                    }
                    Close();
                }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.EndVertical();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void CreateAtlasInfo ( string _path, string _name, int _width, int _height ) {
        // create atlas info
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Creating Atlas Asset...",
                                          0.1f );    
        exAtlasInfo newAtlasInfo = exAtlasInfo.Create( _path, _name + " - EditorInfo" );
        newAtlasInfo.width = _width; 
        newAtlasInfo.height = _height; 

        // create texture
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Creating Atlas Texture...",
                                          0.2f );    
        Texture2D tex = new Texture2D( newAtlasInfo.width, 
                                       newAtlasInfo.height, 
                                       TextureFormat.ARGB32, 
                                       false );
        for ( int i = 0; i < newAtlasInfo.width; ++i ) {
            for ( int j = 0; j < newAtlasInfo.height; ++j ) {
                tex.SetPixel(i, j, new Color(1.0f, 1.0f, 1.0f, 0.0f) );
            }
        }
        tex.Apply(false);

        // save texture to png
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Saving Atlas Texture as PNG file...",
                                          0.3f );    
        string atlasTexturePath = Path.Combine( _path, _name + ".png" );
        byte[] pngData = tex.EncodeToPNG();
        if (pngData != null)
            File.WriteAllBytes(atlasTexturePath, pngData);
        Object.DestroyImmediate(tex);

        // import texture
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Import Texture " + atlasTexturePath + "...",
                                          0.5f );    
        AssetDatabase.ImportAsset( atlasTexturePath );
        TextureImporter importSettings = TextureImporter.GetAtPath(atlasTexturePath) as TextureImporter;
        importSettings.maxTextureSize = Mathf.Max( newAtlasInfo.width, newAtlasInfo.height );
        importSettings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        importSettings.isReadable = true;
        importSettings.mipmapEnabled = false;
        importSettings.textureType = TextureImporterType.Advanced;
        importSettings.npotScale = TextureImporterNPOTScale.None;
        AssetDatabase.ImportAsset( atlasTexturePath );

        // create default material
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Create New Material...",
                                          0.7f );    
        Material newMaterial = new Material( Shader.Find("ex2D/Alpha Blended") );
        AssetDatabase.CreateAsset( newMaterial, Path.Combine( _path, _name + ".mat" ) );

        // setup atlas info
        EditorUtility.DisplayProgressBar( "Creating Atlas...",
                                          "Setup New Atlas Asset...",
                                          0.9f );    
        newAtlasInfo.atlasName = _name;
        newAtlasInfo.texture = (Texture2D)AssetDatabase.LoadAssetAtPath( atlasTexturePath, typeof(Texture2D) );
        newAtlasInfo.material = newMaterial;
        newAtlasInfo.material.mainTexture = newAtlasInfo.texture;

        // create new atlas and setup it for both atlas info and atlas asset
        exAtlas newAtlas = exAtlas.Create( _path, _name );
        newAtlas.texture = newAtlasInfo.texture;
        newAtlas.material = newAtlasInfo.material;
        newAtlasInfo.atlas = newAtlas;

        //
        EditorUtility.SetDirty(newAtlasInfo);
        EditorUtility.UnloadUnusedAssets();
        EditorUtility.ClearProgressBar();

        //
        Selection.activeObject = newAtlasInfo;
        EditorGUIUtility.PingObject(newAtlasInfo);
    }
}
