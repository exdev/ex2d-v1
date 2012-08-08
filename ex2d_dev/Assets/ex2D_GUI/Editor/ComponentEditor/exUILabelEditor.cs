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

    // private static string[] textAlignStrings = new string[] { "Left", "Center", "Right" };

    SerializedProperty fontProp;
    SerializedProperty autoSizeProp;
    SerializedProperty useMultilineProp;
    SerializedProperty textProp;
    SerializedProperty alignmentProp;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    protected new void OnEnable () {
        base.OnEnable();

        fontProp = serializedObject.FindProperty ("font");
        autoSizeProp = serializedObject.FindProperty ("autoSize_");
        if ( fontProp.objectReferenceValue )
            useMultilineProp = new SerializedObject ( fontProp.objectReferenceValue ).FindProperty("useMultiline_");
        textProp = serializedObject.FindProperty ("text_");
        alignmentProp = serializedObject.FindProperty ("alignment_");
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

            // font
            EditorGUILayout.PropertyField( fontProp );

            // autoSize
            EditorGUILayout.PropertyField( autoSizeProp, new GUIContent ( "Auto Size" ) );
            editTarget.autoSize = autoSizeProp.boolValue;

            // use Multiline
            if ( useMultilineProp != null ) {
                EditorGUILayout.PropertyField( useMultilineProp, new GUIContent ("Use Multiline") );
                if ( editTarget.font ) editTarget.font.useMultiline = useMultilineProp.boolValue;
            }

            // text
            if ( useMultilineProp != null && useMultilineProp.boolValue ) {
                EditorGUILayout.LabelField ( "Text" );
                textProp.stringValue = EditorGUILayout.TextArea ( textProp.stringValue, EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector ).textArea );
            }
            else {
                EditorGUILayout.PropertyField ( textProp, new GUIContent("Text") );
            }
            editTarget.text = textProp.stringValue;
            if ( editTarget.autoSize ) {
                if ( editTarget.font ) {
                    editTarget.font.Commit();
                    widthProp.floatValue = editTarget.font.boundingRect.width;
                    heightProp.floatValue = editTarget.font.boundingRect.height;
                }
            }

            // alignment
            EditorGUILayout.LabelField ( "Alignment", "" );
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
                alignmentProp.enumValueIndex 
                    = GUILayout.SelectionGrid ( alignmentProp.enumValueIndex, 
                                                anchorTexts, 
                                                3, 
                                                GUILayout.Width(80) );  
                editTarget.alignment = (exPlane.Anchor)alignmentProp.enumValueIndex;
            GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties ();

    }
}
