// ======================================================================================
// File         : exGroupImportEditor.cs
// Author       : Wu Jie 
// Last Change  : 06/10/2011 | 11:34:47 AM | Friday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
/// 
/// Group import editor 
/// 
///////////////////////////////////////////////////////////////////////////////

class exGroupImportEditor : EditorWindow {

    private bool showApplyButton = false;
    // DISABLE { 
    // private TextureImporter myTextureImporter;
    // private AudioImporter myAudioImporter;
    // } DISABLE end 

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    /// \return the group import editor
    /// Open the group import editor window
    // ------------------------------------------------------------------ 

    // DEPRECATED { 
    // [MenuItem ("Window/ex2D/Group Import Editor")]
    // public static exGroupImportEditor NewWindow () {
    //     exGroupImportEditor newWindow = EditorWindow.GetWindow<exGroupImportEditor>();
    //     newWindow.wantsMouseMove = true;
    //     newWindow.autoRepaintOnSceneChange = true;
    //     return newWindow;
    // }
    // } DEPRECATED end 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSelectionChange () {
        // if we have more than one object selected
        if ( Selection.objects.Length > 1 ) {
            bool show = false;
            if ( Selection.activeObject is Texture2D ) {
                // DISABLE { 
                // string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                // myTextureImporter = TextureImporter.GetAtPath(path) as TextureImporter;
                // myAudioImporter = null;
                // } DISABLE end 
                show = true;
            }
            else if ( Selection.activeObject is AudioClip ) {
                // DISABLE { 
                // string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                // myTextureImporter = null;
                // myAudioImporter = AudioImporter.GetAtPath(path) as AudioImporter;
                // } DISABLE end 
                show = true;
            }

            if ( show ) {
                showApplyButton = true;
                Repaint();
                return;
            }
            Repaint();
        }
        else {
            showApplyButton = false;
            Repaint();
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        EditorGUILayout.Space ();

        //
        if ( showApplyButton ) {
            if ( GUILayout.Button( "Apply", GUILayout.Width(100) ) ) {
                ApplySettings ();
            }

            // DISABLE { 
            // if ( myTextureImporter ) {
            //     myTextureImporter.textureFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup ( "Texture Format", myTextureImporter.textureFormat );
            // }
            // else if ( myAudioImporter ) {
            // }
            // } DISABLE end 
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ApplySettings () {
        if ( Selection.activeObject is Texture2D ) {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            TextureImporter firstImporter = TextureImporter.GetAtPath(path) as TextureImporter;

            try {
                int i = 0;
                foreach ( Object o in Selection.objects ) {
                    if ( (o is Texture2D) == false )
                        continue;

                    path = AssetDatabase.GetAssetPath(o);
                    TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;

                    importer.textureFormat           = firstImporter.textureFormat;                             
                    importer.maxTextureSize          = firstImporter.maxTextureSize;         
                    importer.grayscaleToAlpha        = firstImporter.grayscaleToAlpha;       
                    importer.generateCubemap         = firstImporter.generateCubemap;
                    importer.npotScale               = firstImporter.npotScale;
                    importer.isReadable              = firstImporter.isReadable;
                    importer.mipmapEnabled           = firstImporter.mipmapEnabled;
                    importer.borderMipmap            = firstImporter.borderMipmap;
#if UNITY_3_4
                    importer.correctGamma            = firstImporter.correctGamma;
#else
                    importer.generateMipsInLinearSpace = firstImporter.generateMipsInLinearSpace;
#endif
                    importer.mipmapFilter            = firstImporter.mipmapFilter;
                    importer.fadeout                 = firstImporter.fadeout;
                    importer.mipmapFadeDistanceStart = firstImporter.mipmapFadeDistanceStart;
                    importer.mipmapFadeDistanceEnd   = firstImporter.mipmapFadeDistanceEnd;
                    importer.convertToNormalmap      = firstImporter.convertToNormalmap;
                    importer.normalmap               = firstImporter.normalmap;
                    importer.normalmapFilter         = firstImporter.normalmapFilter;
                    importer.heightmapScale          = firstImporter.heightmapScale;
                    importer.lightmap                = firstImporter.lightmap;
                    importer.anisoLevel              = firstImporter.anisoLevel;
                    importer.filterMode              = firstImporter.filterMode;
                    importer.wrapMode                = firstImporter.wrapMode;
                    importer.mipMapBias              = firstImporter.mipMapBias;
                    importer.textureType             = firstImporter.textureType;

                    EditorUtility.DisplayProgressBar( "Process Textures...",
                                                      "Process Texture " + o.name,
                                                      (float)i/(float)Selection.objects.Length );    
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
                    ++i;
                }
                EditorUtility.ClearProgressBar();
            }
            catch ( System.Exception ) {
                EditorUtility.ClearProgressBar();
                throw;
            }
        }
        else if ( Selection.activeObject is AudioClip ) {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            AudioImporter firstImporter = AudioImporter.GetAtPath(path) as AudioImporter;

            try {
                int i = 0;
                foreach ( Object o in Selection.objects ) {
                    if ( (o is AudioClip) == false  )
                        continue;

                    path = AssetDatabase.GetAssetPath(o);
                    AudioImporter importer = AudioImporter.GetAtPath(path) as AudioImporter;

                    importer.format             = firstImporter.format;                             
                    importer.compressionBitrate = firstImporter.compressionBitrate;         
                    importer.threeD             = firstImporter.threeD;       
                    importer.forceToMono        = firstImporter.forceToMono;
                    importer.hardware           = firstImporter.hardware;
                    importer.loopable           = firstImporter.loopable;

                    EditorUtility.DisplayProgressBar( "Process AudioClips...",
                                                      "Process AudioClip " + o.name,
                                                      (float)i/(float)Selection.objects.Length );    
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
                    ++i;
                }
                EditorUtility.ClearProgressBar();
            }
            catch ( System.Exception ) {
                EditorUtility.ClearProgressBar();
                throw;
            }
        }
    }
}


