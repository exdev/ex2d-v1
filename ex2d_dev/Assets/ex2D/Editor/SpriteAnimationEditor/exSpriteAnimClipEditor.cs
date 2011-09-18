// ======================================================================================
// File         : exSpriteAnimClipEditor.cs
// Author       : Wu Jie 
// Last Change  : 06/15/2011 | 01:56:25 AM | Wednesday,June
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////
///
/// exSpriteAnimClipEditor
///
///////////////////////////////////////////////////////////////////////////////

partial class exSpriteAnimClipEditor : EditorWindow {

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    private exSpriteAnimClip curEdit;

    private double lastTime = 0.0;

    private bool playing = false; 
    private bool startPlaying = false; 
    private Vector2 scrollPos = Vector2.zero;

    private List<exSpriteAnimClip.FrameInfo> selectedFrameInfos = new List<exSpriteAnimClip.FrameInfo>();
    private List<exSpriteAnimClip.EventInfo> selectedEventInfos = new List<exSpriteAnimClip.EventInfo>();
    private int selectIdx = 0;
    private float totalWidth = 0.0f;

    private Rect selectRect = new Rect( 0, 0, 1, 1 );
    private Vector2 mouseDownPos = Vector2.zero;

    private bool inRectSelectFrameState = false;
    private bool inDraggingFrameInfoState = false;
    private bool inDraggingEventInfoState = false;
    private bool inResizeFrameInfoState = false;
    private bool inDraggingNeedleState = false;
    private bool inRectSelectEventState = false;

    private exSpriteAnimClip.EventInfo activeEventInfo = null;

    private float yFrameInfoOffset = 10.0f;
    private Rect frameInfoViewRect = new Rect( 0, 0, 1, 1 );
    private Rect eventInfoViewRect = new Rect( 0, 0, 1, 1 );
    private Rect spriteAnimClipRect = new Rect( 0, 0, 1, 1 );
    private int insertAt = -1;

    private float curSeconds = 0.0f;
    private float playingSeconds = 0.0f;
    private float playingStart = 0.0f;
    private float playingEnd = 0.0f;
    private bool playingSelects = false;

    private float previewScale = 1.0f;
    private float previewSize = 256.0f;

    private exSpriteAnimClip.EventInfoComparer eventInfoSorter 
        = new exSpriteAnimClip.EventInfoComparer(); 

    // ------------------------------------------------------------------ 
    /// \return the sprite animation editor
    /// Open the sprite animation editor window
    // ------------------------------------------------------------------ 

    [MenuItem ("Window/ex2D/Sprite Animation Editor %&s")]
    public static exSpriteAnimClipEditor NewWindow () {
        exSpriteAnimClipEditor newWindow = EditorWindow.GetWindow<exSpriteAnimClipEditor>();
        return newWindow;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnEnable () {
        name = "Sprite Animation Editor";
        wantsMouseMove = true;
        autoRepaintOnSceneChange = false;
        // position = new Rect ( 50, 50, 800, 600 );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Init () {

        inRectSelectFrameState = false;
        inDraggingFrameInfoState = false;
        inDraggingEventInfoState = false;
        inResizeFrameInfoState = false;
        inDraggingNeedleState = false;
        inRectSelectEventState = false;

        selectedFrameInfos.Clear();
        selectedEventInfos.Clear();
        selectIdx = 0;
        playing = false;
        startPlaying = true;

        curSeconds = 0.0f;
        playingSeconds = 0.0f;
        playingStart = 0.0f;
        playingEnd = 0.0f;
        playingSelects = false;

        previewScale = 1.0f;
        CalculatePreviewScale();

        if ( curEdit ) {
            curEdit.UpdateLength();
        }
    }

    // DISABLE: the focus only occur when main window lost foucs, then come in { 
    // // ------------------------------------------------------------------ 
    // // Desc: 
    // // ------------------------------------------------------------------ 

    // void OnFocus () {
    //     OnSelectionChange ();
    // }
    // } DISABLE end 

    // ------------------------------------------------------------------ 
    /// \param _obj
    /// Check if the object is valid sprite animation clip and open it in sprite animation clip editor.
    // ------------------------------------------------------------------ 

    public void Edit ( Object _obj ) {
        // check if repaint
        if ( curEdit != _obj ) {
            Object obj = _obj; 

            if ( obj is GameObject ) {
                GameObject go = obj as GameObject;
                // get exSpriteAnimation from itself, children or root 
                exSpriteAnimation spAnim = go.GetComponent<exSpriteAnimation>();
                // DISABLE { 
                // if ( spAnim == null ) {
                //     spAnim = go.GetComponentInChildren<exSpriteAnimation>();
                //     if ( spAnim == null ) {
                //         spAnim = go.transform.root.GetComponentInChildren<exSpriteAnimation>();
                //     }
                // }
                // } DISABLE end 
                if ( spAnim ) {
                    int idx = spAnim.animations.IndexOf(curEdit);
                    // if curEdit is exists in the selected gameObject, don't do anything
                    if ( idx != -1 ) {
                        Repaint ();
                        return;
                    }

                    // if we have default animation, use it
                    if ( spAnim.defaultAnimation != null ) {
                        obj = spAnim.defaultAnimation;
                    }
                    // else we will check if we have animations in our list and use the first one 
                    else if ( spAnim.animations.Count > 0 ) {
                        obj = spAnim.animations[0];
                    }
                }
            }

            // if this is another anim clip, swtich to it.
            if ( obj is exSpriteAnimClip && obj != curEdit ) {
                curEdit = obj as exSpriteAnimClip;
                Init ();
                Repaint ();
                return;
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnSelectionChange () {
        Edit ( Selection.activeObject );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void Update () {
        if ( playing ) {
            float delta = (float)(EditorApplication.timeSinceStartup - lastTime);
            Step(delta);
            Repaint();
        }
        lastTime = EditorApplication.timeSinceStartup; 
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void OnGUI () {
        EditorGUI.indentLevel = 0;

        if ( curEdit == null ) {
            GUILayout.Space(10);
            GUILayout.Label ( "Please select an Sprite Animation Clip" );
            return;
        }

        // ======================================================== 
        // toolbar 
        // ======================================================== 

        GUILayout.BeginHorizontal ( EditorStyles.toolbar );

            // ======================================================== 
            // Play 
            // ======================================================== 

            playing = GUILayout.Toggle ( playing, 
                                         exEditorHelper.AnimationPlayTexture(),
                                         EditorStyles.toolbarButton );
            if ( playing == false ) {
                startPlaying = false;
                playingSeconds = 0.0f;
            }
            else if ( startPlaying == false ) {
                startPlaying = true;
                curSeconds = 0.0f;
                playingSeconds = playingSelects ? playingStart : 0.0f;
            }

            //
            if ( playing &&
                 curEdit.wrapMode == WrapMode.Once &&
                 curSeconds >= curEdit.length ) {
                playing = false;
            }

            // ======================================================== 
            // prev frame 
            // ======================================================== 

            if ( GUILayout.Button ( exEditorHelper.AnimationPrevTexture(), EditorStyles.toolbarButton ) ) {
                exSpriteAnimClip.FrameInfo fi = curEdit.GetFrameInfoBySeconds ( curSeconds, WrapMode.Once );
                int i = curEdit.frameInfos.IndexOf(fi) - 1;
                if ( i >= 0  ) {
                    curSeconds = 0.0f;
                    for ( int j = 0; j < i; ++j ) {
                        curSeconds += curEdit.frameInfos[j].length; 
                    } 
                    curSeconds += 0.1f/totalWidth * curEdit.length;
                }
            }

            // ======================================================== 
            // next frame 
            // ======================================================== 

            if ( GUILayout.Button ( exEditorHelper.AnimationNextTexture(), EditorStyles.toolbarButton ) ) {
                exSpriteAnimClip.FrameInfo fi = curEdit.GetFrameInfoBySeconds ( curSeconds, WrapMode.Once );
                int i = curEdit.frameInfos.IndexOf(fi) + 1;
                if ( i < curEdit.frameInfos.Count ) {
                    curSeconds = 0.0f;
                    for ( int j = 0; j < i; ++j ) {
                        curSeconds += curEdit.frameInfos[j].length; 
                    } 
                    curSeconds += 0.1f/totalWidth * curEdit.length;
                }
            }

            // ======================================================== 
            // frames
            // ======================================================== 

            GUILayout.Space(5);
            EditorGUILayout.SelectableLabel( curEdit.SnapToFrames(curEdit.length) + " frames | "
                                             + curEdit.length.ToString("f3") + " secs",
                                             GUILayout.Width(150), GUILayout.MaxHeight(18) );

            // ======================================================== 
            // preview speed
            // ======================================================== 

            GUILayout.Space(10);
            curEdit.editorSpeed = EditorGUILayout.FloatField( "Preview Speed", 
                                                              curEdit.editorSpeed,
                                                              EditorStyles.toolbarTextField,
                                                              GUILayout.Width(150) );

            // ======================================================== 
            // preview length
            // ======================================================== 

            GUILayout.Space(5);
            EditorGUILayout.SelectableLabel( (curEdit.length / curEdit.editorSpeed).ToString("f3") + " secs", 
                                             GUILayout.Width(200), GUILayout.MaxHeight(18) );

            GUILayout.FlexibleSpace();

            // ======================================================== 
            // Select 
            // ======================================================== 

            GUI.enabled = selectedFrameInfos.Count != 0;
            if ( GUILayout.Button("Select In Project...", EditorStyles.toolbarButton ) ) {
                List<Object> selects = new List<Object>(selectedFrameInfos.Count);
                foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
                    Texture2D texture 
                        = exEditorHelper.LoadAssetFromGUID<Texture2D>(fi.textureGUID ); 
                    selects.Add(texture);
                }

                if ( selects.Count != 0 ) {
                    selectIdx = (selectIdx + 1) % selects.Count;  
                    Selection.objects = selects.ToArray();
                    EditorGUIUtility.PingObject(Selection.objects[selectIdx]);
                }
            }
            GUI.enabled = true; 

            // ======================================================== 
            // editor scale 
            // ======================================================== 

            GUILayout.Label ("Zoom");
            GUILayout.Space(5);
            curEdit.editorScale = GUILayout.HorizontalSlider ( curEdit.editorScale, 
                                                               0.01f, 
                                                               10.0f, 
                                                               GUILayout.MaxWidth(150) );
            GUILayout.Space(5);
            curEdit.editorScale = EditorGUILayout.FloatField( curEdit.editorScale,
                                                              EditorStyles.toolbarTextField,
                                                              GUILayout.Width(50) );
            curEdit.editorScale = Mathf.Clamp( curEdit.editorScale, 0.01f, 10.0f );

            // ======================================================== 
            // Build
            // ======================================================== 

            GUI.enabled = curEdit.editorNeedRebuild; 
            if ( GUILayout.Button( "Build", EditorStyles.toolbarButton, GUILayout.Width(80) ) ) 
            {
                curEdit.Build();
            }
            GUI.enabled = true; 

            // ======================================================== 
            // Help
            // ======================================================== 

            if ( GUILayout.Button( exEditorHelper.HelpTexture(), EditorStyles.toolbarButton ) ) {
                Help.BrowseURL("http://www.ex-dev.com/ex2d/wiki/doku.php?id=manual:sprite_anim_editor_guide");
            }

        GUILayout.EndHorizontal ();

        // ======================================================== 
        // Scroll View
        // ======================================================== 

        float toolbarHeight = EditorStyles.toolbar.CalcHeight( new GUIContent(""), 0 );
        scrollPos = EditorGUILayout.BeginScrollView ( scrollPos, 
                                                      GUILayout.Width(position.width),
                                                      GUILayout.Height(position.height-toolbarHeight) );

        Rect lastRect = new Rect( 10, 0, 1, 1 );
        GUILayout.Space(5);

        // ======================================================== 
        // draw label
        // ======================================================== 

        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginVertical( GUILayout.MaxWidth(500) );

            // DISABLE { 
            // // asset path
            // EditorGUILayout.LabelField ( "Path", AssetDatabase.GetAssetPath(curEdit) );
            // } DISABLE end 

            // animclip field
            Object newClip = EditorGUILayout.ObjectField( "Sprite Animation"
                                                          , curEdit
                                                          , typeof(exSpriteAnimClip)
#if !UNITY_3_0 && !UNITY_3_1 && !UNITY_3_3
                                                          , false
#endif
                                                          , GUILayout.Width(300) 
                                                        );
            if ( newClip != curEdit )
                Selection.activeObject = newClip;

            // DELME { 
            // // length
            // float newLength = EditorGUILayout.FloatField( "Animation Length", 
            //                                               curEdit.length, 
            //                                               GUILayout.MaxWidth(200) );
            // if ( newLength != curEdit.length ) {
            //     float totalLength = 0.0f;
            //     float delta = newLength - curEdit.length;
            //     foreach ( exSpriteAnimClip.FrameInfo fi in curEdit.frameInfos) {
            //         float ratio = fi.length/curEdit.length;
            //         fi.length = Mathf.Max(1.0f/60.0f, fi.length + delta * ratio);
            //         totalLength += fi.length;
            //     }
            //     foreach ( exSpriteAnimClip.EventInfo ei in curEdit.eventInfos) {
            //         ei.time = ei.time/curEdit.length * totalLength;
            //     }
            //     curEdit.length = totalLength;
            //     GUI.changed = true;
            // }
            // } DELME end 

            // speed and length
            GUILayout.BeginHorizontal();
                curEdit.speed = EditorGUILayout.FloatField( "Speed", 
                                                            curEdit.speed, 
                                                            GUILayout.MaxWidth(200) );
                GUILayout.Space(10);
                float curLength = curEdit.length / curEdit.speed;
                float newLength = EditorGUILayout.FloatField( "Length", 
                                                              curLength, 
                                                              GUILayout.MaxWidth(200) );
                if ( curLength != newLength ) {
                    curEdit.speed = curEdit.length/newLength;
                }
                GUILayout.Label( "secs" );
            GUILayout.EndHorizontal();

            // sample rate
            curEdit.sampleRate = EditorGUILayout.FloatField( "Sample Rate", 
                                                             curEdit.sampleRate, 
                                                             GUILayout.MaxWidth(200) );

            // Wrap Mode enum popup
            curEdit.wrapMode = (WrapMode)EditorGUILayout.EnumPopup ( "Wrap Mode", 
                                                                     curEdit.wrapMode, 
                                                                     GUILayout.Width(200) );

            // Anim Stop Action 
            curEdit.stopAction = (exSpriteAnimClip.StopAction)EditorGUILayout.EnumPopup ( "Stop Action", 
                                                                                          curEdit.stopAction, 
                                                                                          GUILayout.Width(200) );
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        // ======================================================== 
        // draw timeline editor
        // ======================================================== 

        lastRect = GUILayoutUtility.GetLastRect ();
        int topHeight = 20;
        int botHeight = 20;
        int scalarHeight = 14; // 20 for scalar + label, 14 for scalar
        spriteAnimClipRect = new Rect ( lastRect.x + 40.0f, 
                                        lastRect.yMax,
                                        position.width - 80.0f, 
                                        200.0f );
        SpriteAnimClipField ( spriteAnimClipRect, topHeight, botHeight, scalarHeight, curEdit );

        // ======================================================== 
        // draw event info view
        // ======================================================== 

        Rect eventInfoViewRect2 = new Rect ( spriteAnimClipRect.x + curEdit.editorOffset, 
                                             spriteAnimClipRect.y + eventInfoViewRect.y,
                                             eventInfoViewRect.width,
                                             eventInfoViewRect.height );
        EventInfoViewField ( eventInfoViewRect2, curEdit );

        // ======================================================== 
        // draw event info dragging
        // ======================================================== 

        if ( inDraggingEventInfoState ) {
            foreach ( exSpriteAnimClip.EventInfo ei in selectedEventInfos ) {
                float lineAt = spriteAnimClipRect.x + curEdit.editorOffset + (ei.time / curEdit.length) * totalWidth;
                float yStart = spriteAnimClipRect.y + frameInfoViewRect.y;
                float height = frameInfoViewRect.height; 

                if ( lineAt >= spriteAnimClipRect.x && lineAt <= spriteAnimClipRect.xMax ) {
                    exEditorHelper.DrawLine ( new Vector2(lineAt, yStart ), 
                                            new Vector2(lineAt, yStart + height ), 
                                            Color.yellow, 
                                            1.0f );
                    GUI.Label ( new Rect( lineAt-15.0f, yStart + height, 30.0f, 20.0f ),
                                exTimeHelper.ToString_Frames(ei.time,curEdit.sampleRate)
                              );
                }
            }
        }

        // ======================================================== 
        // Needle 
        // ======================================================== 

        NeedleField ( lastRect.yMax + topHeight,
                      lastRect.yMax + topHeight + spriteAnimClipRect.height - topHeight - botHeight );

        if ( Event.current.type == EventType.MouseDown && 
             Event.current.button == 0 && 
             Event.current.clickCount == 1 ) {

            Rect needleRect = new Rect ( spriteAnimClipRect.x + curEdit.editorOffset, 
                                         spriteAnimClipRect.y - 10.0f,
                                         spriteAnimClipRect.width,
                                         30.0f );
            if ( needleRect.Contains( Event.current.mousePosition ) ) {
                inDraggingNeedleState = true;
                MoveNeedle ( Event.current.mousePosition );

                Event.current.Use();
                Repaint();
            }
        }

        // ======================================================== 
        // draw insert field 
        // ======================================================== 

        if ( inDraggingFrameInfoState )
            InsertField ( new Rect ( spriteAnimClipRect.x, 
                                     spriteAnimClipRect.y + frameInfoViewRect.y,
                                     7, 
                                     frameInfoViewRect.height ), curEdit);

        GUILayout.BeginHorizontal();
        GUILayout.Space(spriteAnimClipRect.x);

        // ======================================================== 
        // left panel
        GUILayout.BeginVertical( GUILayout.MaxWidth(300) );
        // ======================================================== 

            // ======================================================== 
            // PreviewField 
            // ======================================================== 

            GUILayout.Space(10);
            lastRect = GUILayoutUtility.GetLastRect ();  
            PreviewField ( new Rect ( spriteAnimClipRect.x, lastRect.yMax, previewSize, previewSize ) );

        GUILayout.EndVertical();

        // ======================================================== 
        GUILayout.Space(40);
        // right panel
        GUILayout.BeginVertical( GUILayout.MaxWidth( 600 ) );
        // ======================================================== 
        
            EventInfoEditField ();

            GUILayout.Space(5);
            lastRect = GUILayoutUtility.GetLastRect ();
                exEditorHelper.DrawLine( new Vector2( lastRect.xMax, lastRect.yMax), 
                                       new Vector2(position.width-40, lastRect.yMax), 
                                       Color.gray, 1.0f );
                exEditorHelper.DrawLine( new Vector2( lastRect.xMax, lastRect.yMax+1), 
                                       new Vector2(position.width-40, lastRect.yMax+1), 
                                       Color.white, 1.0f );
            GUILayout.Space(5+2);

            FrameInfoEditField ();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // ======================================================== 
        // draw select rect 
        // ======================================================== 

        if ( selectRect.width != 0.0f || selectRect.height != 0.0f ) {
            if ( inRectSelectFrameState ) {
                exEditorHelper.DrawRect( selectRect, new Color( 0.0f, 0.5f, 1.0f, 0.2f ), new Color( 0.2f, 0.5f, 1.0f, 1.0f ) );
            }
            else if ( inRectSelectEventState ) {
                exEditorHelper.DrawRect( selectRect, new Color( 1.0f, 0.0f, 0.0f, 0.2f ), new Color( 1.0f, 0.0f, 0.0f, 1.0f ) );
            }
        }

        // ======================================================== 
        Event e = Event.current;
        // ======================================================== 

        // mouse down
        if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 1 ) {
            GUIUtility.keyboardControl = -1; // remove any keyboard control
            selectedEventInfos.Clear();

            mouseDownPos = e.mousePosition;
            inRectSelectFrameState = true;
            UpdateSelectRect (e.mousePosition);
            ConfirmRectSelectFrameInfo();
            Repaint();

            e.Use();
        }

        // rect select frame state
        if ( inRectSelectFrameState ) {
            if ( e.type == EventType.MouseDrag ) {
                UpdateSelectRect (e.mousePosition);
                ConfirmRectSelectFrameInfo();

                Repaint();
                e.Use();
            }
            else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                ConfirmRectSelectFrameInfo();

                inRectSelectEventState = false;
                inRectSelectFrameState = false;
                inResizeFrameInfoState = false;
                inDraggingNeedleState = false;
                inDraggingFrameInfoState = false;
                inDraggingEventInfoState = false;
                insertAt = -1;

                Repaint();
                e.Use();
            }
        }

        // rect select event state
        if ( inRectSelectEventState ) {
            if ( e.type == EventType.MouseDrag ) {
                UpdateSelectRect (e.mousePosition);
                ConfirmRectSelectEventInfo();

                Repaint();
                e.Use();
            }
            else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                ConfirmRectSelectEventInfo();

                inRectSelectEventState = false;
                inRectSelectFrameState = false;
                inResizeFrameInfoState = false;
                inDraggingNeedleState = false;
                inDraggingFrameInfoState = false;
                inDraggingEventInfoState = false;
                insertAt = -1;

                Repaint();
                e.Use();
            }
        }

        // resize frame info state
        if ( inResizeFrameInfoState ) {
            if ( e.type == EventType.MouseDrag ) {
                ResizeSelectedFrames ( e.mousePosition );

                Repaint();
                e.Use();
            }
            else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                inRectSelectEventState = false;
                inRectSelectFrameState = false;
                inResizeFrameInfoState = false;
                inDraggingNeedleState = false;
                inDraggingFrameInfoState = false;
                inDraggingEventInfoState = false;
                insertAt = -1;

                Repaint();
                e.Use();
            }
        }

        // dragging selected frame
        if ( inDraggingFrameInfoState ) {
            if ( e.type == EventType.MouseDrag ) {
                insertAt = FindInsertPlace ();

                Repaint();
                e.Use();
            }
            else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                MoveSelectedFrameInfo ();

                inRectSelectEventState = false;
                inRectSelectFrameState = false;
                inResizeFrameInfoState = false;
                inDraggingNeedleState = false;
                inDraggingFrameInfoState = false;
                inDraggingEventInfoState = false;
                insertAt = -1;

                Repaint();
                e.Use();
            }
        }

        // dragging selected event
        if ( inDraggingEventInfoState ) {
            if ( e.type == EventType.MouseDrag ) {
                MoveSelectedEventInfo ( e.mousePosition );

                Repaint();
                e.Use();
            }
            else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                inRectSelectEventState = false;
                inRectSelectFrameState = false;
                inResizeFrameInfoState = false;
                inDraggingNeedleState = false;
                inDraggingFrameInfoState = false;
                inDraggingEventInfoState = false;
                insertAt = -1;

                Repaint();
                e.Use();
            }
        }

        // dragging needle
        if ( inDraggingNeedleState ) {
            if ( e.type == EventType.MouseDrag ) {
                MoveNeedle ( e.mousePosition );

                Repaint();
                e.Use();
            }
            else if ( e.type == EventType.MouseUp && e.button == 0 ) {
                inRectSelectEventState = false;
                inRectSelectFrameState = false;
                inResizeFrameInfoState = false;
                inDraggingNeedleState = false;
                inDraggingFrameInfoState = false;
                inDraggingEventInfoState = false;
                insertAt = -1;

                Repaint();
                e.Use();
            }
        }

        // key events 
        if ( e.isKey ) {
            if ( e.type == EventType.KeyDown ) {
                if ( e.keyCode == KeyCode.Backspace ||
                     e.keyCode == KeyCode.Delete ) 
                {
                    RemoveSelectedElements();
                    CalculatePreviewScale();
                    Repaint();
                    e.Use();
                }
            }
        }

        EditorGUILayout.EndScrollView ();

        // 
        if ( GUI.changed ) {
            EditorUtility.SetDirty(curEdit);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void CalculatePreviewScale () {
        // get the max width and max height
        float maxWidth = -1.0f;
        float maxHeight = -1.0f;
        foreach ( exSpriteAnimClip.FrameInfo frameInfo in curEdit.frameInfos ) {
            float fiWidth = 0.0f;
            float fiHeight = 0.0f;

            //
            exAtlasDB.ElementInfo elInfo = exAtlasDB.GetElementInfo (frameInfo.textureGUID);
            if ( elInfo != null ) {
                exAtlasInfo atlasInfo = exEditorHelper.LoadAssetFromGUID<exAtlasInfo>(elInfo.guidAtlasInfo);
                exAtlasInfo.Element el = atlasInfo.elements[elInfo.indexInAtlasInfo];  
                // fiWidth = el.trimRect.width;
                // fiHeight = el.trimRect.height;
                fiWidth = el.texture.width;
                fiHeight = el.texture.height;
            } else {
                string texturePath = AssetDatabase.GUIDToAssetPath(frameInfo.textureGUID);
                Texture2D tex2D = (Texture2D)AssetDatabase.LoadAssetAtPath( texturePath, typeof(Texture2D));
                fiWidth = tex2D.width;
                fiHeight = tex2D.height;
            }

            //
            if ( maxWidth <= fiWidth ) {
                maxWidth = fiWidth;
            }
            if ( maxHeight <= fiHeight ) {
                maxHeight = fiHeight;
            }
        }

        // get the preview scale
        previewScale = 1.0f;
        float viewWidth = previewSize;
        float viewHeight = previewSize;
        if ( maxWidth > viewWidth && maxHeight > viewHeight ) {
            previewScale = Mathf.Min( viewWidth / maxWidth, viewHeight / maxHeight );
        }
        else if ( maxWidth > viewWidth ) {
            previewScale = viewWidth / maxWidth;
        }
        else if ( maxHeight > viewHeight ) {
            previewScale = viewHeight / maxHeight;
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AddSelected ( exSpriteAnimClip.FrameInfo _fi ) {
        if ( selectedFrameInfos.IndexOf(_fi) == -1 ) {
            selectedFrameInfos.Add(_fi);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void AddSelected ( exSpriteAnimClip.EventInfo _ei ) {
        if ( selectedEventInfos.IndexOf(_ei) == -1 ) {
            selectedEventInfos.Add(_ei);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ToggleSelected ( exSpriteAnimClip.FrameInfo _fi ) {
        if ( selectedFrameInfos.IndexOf(_fi) == -1 ) {
            selectedFrameInfos.Add(_fi);
        }
        else {
            selectedFrameInfos.Remove(_fi);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ToggleSelected ( exSpriteAnimClip.EventInfo _ei ) {
        if ( selectedEventInfos.IndexOf(_ei) == -1 ) {
            selectedEventInfos.Add(_ei);
        }
        else {
            selectedEventInfos.Remove(_ei);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void UpdateSelectRect ( Vector2 _curMousePos ) {
        float x = 0;
        float y = 0;
        float width = 0;
        float height = 0;
        Vector2 curMousePos = _curMousePos;

        if ( mouseDownPos.x < curMousePos.x ) {
            x = mouseDownPos.x;
            width = curMousePos.x - mouseDownPos.x;
        }
        else {
            x = curMousePos.x;
            width = mouseDownPos.x - curMousePos.x;
        }
        if ( mouseDownPos.y < curMousePos.y ) {
            y = mouseDownPos.y;
            height = curMousePos.y - mouseDownPos.y;
        }
        else {
            y = curMousePos.y;
            height = mouseDownPos.y - curMousePos.y;
        }

        selectRect = new Rect( x, y, width, height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ConfirmRectSelectFrameInfo () {
        Rect mappedRect = MapToFrameInfoViewField(selectRect);

        float curX = 0.0f;
        selectedFrameInfos.Clear();
        foreach ( exSpriteAnimClip.FrameInfo fi in curEdit.frameInfos ) {
            float width = (fi.length / curEdit.length) * totalWidth;
            Rect fiRect = new Rect( curX, 10, width, frameInfoViewRect.height - 20 );
            curX += width;

            if ( exContains2D.RectRect( mappedRect, fiRect ) != 0 ||
                 exIntersection2D.RectRect( mappedRect, fiRect ) )
            {
                selectedFrameInfos.Add (fi);
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ConfirmRectSelectEventInfo () {
        Rect mappedRect = MapToEventInfoViewField(selectRect);
        float maxHeight = eventInfoViewRect.height-3;
        Rect lastRect = new Rect( -10, -10, 7, maxHeight );
        Rect curRect = new Rect( -10, -10, 7, maxHeight );

        selectedEventInfos.Clear();
        foreach ( exSpriteAnimClip.EventInfo ei in curEdit.eventInfos ) {
            float at = (ei.time / curEdit.length) * totalWidth;
            lastRect = curRect;
            curRect = new Rect( at - 4, 0, 7, maxHeight );
            if ( exIntersection2D.RectRect( lastRect, curRect ) ) {
                curRect.height = Mathf.Max( lastRect.height - 5.0f, 10.0f );
            }

            if ( exContains2D.RectRect( mappedRect, curRect ) != 0 ||
                 exIntersection2D.RectRect( mappedRect, curRect ) )
            {
                selectedEventInfos.Add (ei);
            }
        }
        selectedEventInfos.Sort(eventInfoSorter);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void RemoveSelectedElements () {
        if ( selectedEventInfos.Count != 0  ) {
            foreach ( exSpriteAnimClip.EventInfo ei in selectedEventInfos ) {
                curEdit.RemoveEvent( ei );
            }
            EditorUtility.SetDirty(curEdit);
            selectedEventInfos.Clear();
        }

        if ( selectedFrameInfos.Count != 0 ) {
            foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
                curEdit.RemoveFrame(fi);
            }
            curEdit.UpdateLength();
            selectedFrameInfos.Clear();
        }
    } 

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ResizeSelectedFrames ( Vector2 _pos ) {
        // float pos = Mathf.Clamp( _pos.x - spriteAnimClipRect.x, 0.0f, totalWidth + curEdit.editorOffset );
        float pos = Mathf.Max( _pos.x - spriteAnimClipRect.x, 0.0f );
        float expectSeconds = (pos - curEdit.editorOffset) * curEdit.length / totalWidth;
        expectSeconds = curEdit.SnapToSeconds (expectSeconds);

        // get start seconds
        float startSeconds = 0.0f;
        foreach ( exSpriteAnimClip.FrameInfo frameInfo in curEdit.frameInfos ) {
            if ( frameInfo == selectedFrameInfos[0] )
                break;
            startSeconds += frameInfo.length;
        }
        float newLength = expectSeconds - startSeconds;

        //
        ResizeSelectedFrames (newLength);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void ResizeSelectedFrames ( float _newLength ) {
        // get total length
        float unitSeconds = 1.0f/curEdit.sampleRate;
        float total = 0.0f;
        int[] frames = new int[selectedFrameInfos.Count]; 
        float minSeconds = 99999.0f;
        for ( int i = 0; i < selectedFrameInfos.Count; ++i ) {
            exSpriteAnimClip.FrameInfo frameInfo = selectedFrameInfos[i];
            total += frameInfo.length;
            frames[i] = curEdit.SnapToFrames(frameInfo.length);

            if ( frameInfo.length < minSeconds ) {
                minSeconds = frameInfo.length;
            }
        }
        int gcd = exMathHelper.GetGCD(frames);

        // get delta
        float delta = _newLength - total;
        float minTotal = total/gcd;

        // check if resize
        if ( (delta < 0.0f && gcd == 1) || 
             (Mathf.Abs(delta) <= minTotal - unitSeconds * 0.5f) )
            return;

        //
        foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos) {
            float ratio = fi.length/total;
            fi.length = curEdit.SnapToSeconds( Mathf.Max(unitSeconds, fi.length + (delta - (delta % minTotal)) * ratio) );
        }

        // re-calculate the animclip length
        curEdit.UpdateLength();

        //
        curEdit.editorNeedRebuild = true;
        EditorUtility.SetDirty(curEdit);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    int FindInsertPlace () {
        bool canInsert = true;
        int lastIdx = -1;

        // add indices
        List<int> indices = new List<int>();
        foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
            int idx = curEdit.frameInfos.IndexOf(fi);
            indices.Add(idx);
        }
        indices.Sort();
        int minIdx = indices[0];
        int maxIdx = indices[indices.Count-1] + 1;

        // check if can insert or not
        foreach ( int idx in indices ) {
            if ( lastIdx != -1 && idx - lastIdx != 1 ) {
                canInsert = false;
                break;
            }
            lastIdx = idx;
        }

        int at = -1;
        if ( canInsert ) {
            GUI.BeginGroup(frameInfoViewRect);
            Vector2 mousePos = Event.current.mousePosition;
            GUI.EndGroup();

            float curX = 0.0f;
            Rect rect = new Rect(0,0,1,1);
            for ( int i = 0; i < curEdit.frameInfos.Count; ++i ) {
                exSpriteAnimClip.FrameInfo fi = curEdit.frameInfos[i];
                float width = (fi.length / curEdit.length) * totalWidth;
                rect = new Rect( curX, yFrameInfoOffset, width, frameInfoViewRect.height - 2 * yFrameInfoOffset );
                curX += width;

                if ( i == 0 && mousePos.x <= rect.xMax ) {
                    at = i;
                    break;
                }
                else if ( mousePos.x >= rect.x && mousePos.x < rect.xMax ) {
                    at = i;
                    break;
                }
            }

            if ( mousePos.x >= rect.xMax ) {
                at = curEdit.frameInfos.Count;
            }

            if ( at != -1 && at >= minIdx && at <= maxIdx ) {
                at = -1;
            }
        }
        return at;
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void MoveSelectedFrameInfo () {
        if ( insertAt != -1 ) {
            // add indices
            List<int> indices = new List<int>();
            foreach ( exSpriteAnimClip.FrameInfo fi in selectedFrameInfos ) {
                int idx = curEdit.frameInfos.IndexOf(fi);
                indices.Add(idx);
            }
            indices.Sort();
            int minIdx = indices[0];
            int maxIdx = indices[indices.Count-1]+1;

            List<exSpriteAnimClip.FrameInfo> insertList = new List<exSpriteAnimClip.FrameInfo>();
            for ( int i = minIdx; i < maxIdx; ++i ) {
                exSpriteAnimClip.FrameInfo fi = curEdit.frameInfos[i];
                insertList.Add(fi);
            }
            curEdit.frameInfos.RemoveRange( minIdx, indices.Count );

            // 
            if ( insertAt > minIdx ) {
                insertAt -= indices.Count;
            }
            foreach ( exSpriteAnimClip.FrameInfo fi in insertList ) {
                curEdit.frameInfos.Insert(insertAt,fi);
                ++insertAt;
            }
            insertAt = -1;
            selectedFrameInfos.Clear();
            curEdit.UpdateLength();

            curEdit.editorNeedRebuild = true;
            EditorUtility.SetDirty(curEdit);
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void MoveSelectedEventInfo ( Vector2 _pos ) {
        float pos = Mathf.Clamp( _pos.x - spriteAnimClipRect.x, 0.0f, totalWidth + curEdit.editorOffset );
        float expectSeconds = (pos - curEdit.editorOffset) * curEdit.length / totalWidth;
        expectSeconds = curEdit.SnapToSeconds (expectSeconds);

        selectedEventInfos.Sort(eventInfoSorter);
        float deltaSeconds = expectSeconds - activeEventInfo.time;

        //
        if ( selectedEventInfos[0].time + deltaSeconds <= 0.0f ) 
        {
            deltaSeconds = 0.0f - selectedEventInfos[0].time;
        }
        else if ( selectedEventInfos[selectedEventInfos.Count-1].time + deltaSeconds >= curEdit.length )
        {
            deltaSeconds = curEdit.length - selectedEventInfos[selectedEventInfos.Count-1].time;
        }

        //
        foreach ( exSpriteAnimClip.EventInfo ei in selectedEventInfos ) {
            ei.time = curEdit.SnapToSeconds(ei.time + deltaSeconds);
        }
        curEdit.eventInfos.Sort(eventInfoSorter);
        EditorUtility.SetDirty(curEdit);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void MoveNeedle ( Vector2 _pos ) {
        float pos = Mathf.Clamp( _pos.x - spriteAnimClipRect.x, 0.0f, totalWidth + curEdit.editorOffset );
        curSeconds = (pos - curEdit.editorOffset) * curEdit.length / totalWidth;
        curSeconds = curEdit.SnapToSeconds (curSeconds);
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    Rect MapToFrameInfoViewField ( Rect _rect ) {
        float x = _rect.x - frameInfoViewRect.x - spriteAnimClipRect.x;
        float y = _rect.y - frameInfoViewRect.y - spriteAnimClipRect.y;
        return new Rect ( x, y, _rect.width, _rect.height );
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    Rect MapToEventInfoViewField ( Rect _rect ) {
        float x = _rect.x - eventInfoViewRect.x - spriteAnimClipRect.x;
        float y = _rect.y - eventInfoViewRect.y - spriteAnimClipRect.y;
        return new Rect ( x, y, _rect.width, _rect.height );
    }
}
