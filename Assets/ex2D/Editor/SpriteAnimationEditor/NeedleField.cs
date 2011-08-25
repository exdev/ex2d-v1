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
        Rect rect = new Rect ( -4, spriteAnimClipRect.y - 10.0f, 4, 30.0f );
        float offset = curSeconds * totalWidth / curEdit.length;
        float xPos = curEdit.editorOffset + offset - rect.width/2.0f;
        rect.x = xPos + xStart;
        xPos = xStart + xPos + rect.width/2.0f;

        if ( xPos >= spriteAnimClipRect.x && xPos <= spriteAnimClipRect.xMax ) {

            // rect
            exEditorHelper.DrawRect ( rect,
                                      // inDraggingNeedleState ? new Color( 1.0f, 0.0f, 0.0f, 0.6f ) : new Color( 1.0f, 0.0f, 0.0f, 0.2f ),
                                      new Color( 1.0f, 0.0f, 0.0f, 1.0f ),
                                      Color.red );

            exEditorHelper.DrawLine ( new Vector2( xPos, yStart ),
                                      new Vector2( xPos, yEnd ),
                                      new Color ( 0.8f, 0.0f, 0.0f, 1.0f ),
                                      1.0f );
            // show label
            if ( inDraggingNeedleState ) {
                GUI.Label ( new Rect( xPos - 15.0f, yEnd, 30.0f, 20.0f ),
                            // exTimeHelper.ToString_Seconds(curSeconds) );
                            curSeconds.ToString() );
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

