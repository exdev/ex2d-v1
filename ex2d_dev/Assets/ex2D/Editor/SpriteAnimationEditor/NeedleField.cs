// ======================================================================================
// File         : NeedleField.cs
// Author       : Wu Jie 
// Last Change  : 07/06/2011 | 23:33:04 PM | Wednesday,July
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
//
///////////////////////////////////////////////////////////////////////////////

partial class exSpriteAnimClipEditor {

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    void NeedleField ( float yStart, float yEnd ) {
        float xStart = spriteAnimClipRect.x;
        // float width = 4.0f;
        float offset = curSeconds * totalWidth / curEdit.length;
        // float xPos = curEdit.editorOffset + offset - width * 0.5f;
        // xPos = xStart + xPos + width * 0.5f + 1;
        float xPos = curEdit.editorOffset + offset;
        xPos = xStart + xPos + 1;

        if ( xPos >= spriteAnimClipRect.x && xPos <= spriteAnimClipRect.xMax ) {

            // rect
            exEditorHelper.DrawLine ( new Vector2( xPos, spriteAnimClipRect.y - 10.0f ),
                                      new Vector2( xPos, spriteAnimClipRect.y + 20.0f ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );
            exEditorHelper.DrawLine ( new Vector2( xPos+1, spriteAnimClipRect.y - 10.0f ),
                                      new Vector2( xPos+1, spriteAnimClipRect.y + 20.0f ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );

            exEditorHelper.DrawLine ( new Vector2( xPos, yStart ),
                                      new Vector2( xPos, yEnd ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 0.5f ),
                                      1.0f );
            exEditorHelper.DrawLine ( new Vector2( xPos+1, yStart ),
                                      new Vector2( xPos+1, yEnd ),
                                      new Color ( 1.0f, 0.0f, 0.0f, 0.5f ),
                                      1.0f );
            // show label
            if ( inDraggingNeedleState ) {
                GUI.Label ( new Rect( xPos - 15.0f, yEnd, 30.0f, 20.0f ),
                            exTimeHelper.ToString_Frames(curSeconds,curEdit.sampleRate) );
            }
        }
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    public void Step ( float _delta ) {
        if ( playingSelects ) {
            playingSeconds += _delta * curEdit.editorSpeed;
            float wrapTime = (playingSeconds - playingStart) % (playingEnd - playingStart);
            curSeconds = wrapTime + playingStart;
        }
        else {
            playingSeconds += _delta * curEdit.editorSpeed;
            curSeconds = curEdit.WrapSeconds(playingSeconds,curEdit.wrapMode);
        }
    }
}

