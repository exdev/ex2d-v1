// ======================================================================================
// File         : exUILabelEditor.cs
// Author       : Wu Jie 
// Last Change  : 08/07/2012 | 01:20:24 AM | Tuesday,August
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// public
///////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(exUILabel))]
public class exUILabelEditor : exUIElementEditor {

    private static string[] textAlignStrings = new string[] { "Left", "Center", "Right" };

    SerializedProperty textProp;
    SerializedProperty fontProp;
    SerializedProperty useMultilineProp;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();

        textProp = serializedObject.FindProperty ("text_");
        fontProp = serializedObject.FindProperty ("font");
        useMultilineProp = new SerializedObject ( serializedObject.FindProperty ("font").objectReferenceValue ).FindProperty("useMultiline_");
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	public override void OnInspectorGUI () {
        exUILabel editTarget = target as exUILabel; 

        // ======================================================== 
        // Base GUI 
        // ======================================================== 

        base.OnInspectorGUI();
        GUILayout.Space(20);

        // ======================================================== 
        // 
        // ======================================================== 

        serializedObject.Update ();

            // use Multiline
            EditorGUILayout.PropertyField( useMultilineProp, new GUIContent ("Use Multiline") );
            editTarget.font.useMultiline = useMultilineProp.boolValue;

            // text
            if ( useMultilineProp.boolValue ) {
                EditorGUILayout.LabelField ( "Text" );
                textProp.stringValue = EditorGUILayout.TextArea ( textProp.stringValue, EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textArea );
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                    editTarget.font.textAlign = (exSpriteFont.TextAlign)GUILayout.Toolbar ( (int)editTarget.font.textAlign, 
                                                                                            textAlignStrings, 
                                                                                            GUILayout.Width(150) );  
                GUILayout.EndHorizontal();
            }
            else {
                EditorGUILayout.PropertyField ( textProp, new GUIContent("Text") );
            }

            // font
            EditorGUILayout.PropertyField( fontProp );

            // TODO { 
            // // anchor
            // EditorGUILayout.LabelField ( "Anchor", "" );
            // GUILayout.BeginHorizontal();
            // GUILayout.Space(30);
            //     editPlane.anchor 
            //         = (exPlane.Anchor)GUILayout.SelectionGrid ( (int)editPlane.anchor, 
            //                                                       anchorTexts, 
            //                                                       3, 
            //                                                       GUILayout.Width(80) );  
            // GUILayout.EndHorizontal();
            // } TODO end 

        serializedObject.ApplyModifiedProperties ();

    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected override void OnSceneGUI () {
        base.OnSceneGUI();

        serializedObject.Update ();
            exUILabel curEdit = target as exUILabel;

            if ( curEdit.font ) {
                if ( curEdit.font.text != textProp.stringValue ) {
                    curEdit.font.text = textProp.stringValue;
                    EditorUtility.SetDirty(curEdit.font);
                    HandleUtility.Repaint(); 
                }
            }
        serializedObject.ApplyModifiedProperties ();
    }
}
