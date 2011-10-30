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

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

    [MenuItem ("GameObject/Create Other/ex2D_GUI/Button")]
    static void CreateSpriteObject () {
        // create button object
        GameObject buttonGO = new GameObject("Button");
        buttonGO.AddComponent<exUIButton>();

        // TODO { 
        // // create sprite button as a child
        // GameObject spriteGO = new GameObject("button_sprite");
        // exSpriteBase buttonSP = spriteGO.AddComponent<exSpriteBorder>();
        // buttonUI.buttonSP = buttonSP;
        // spriteGO.transform.parent = buttonGO.transform;

        // // add box collider
        // BoxCollider boxCollider = buttonSP.GetComponent<BoxCollider>();
        // if ( boxCollider == null ) {
        //     boxCollider = buttonSP.gameObject.AddComponent<BoxCollider>();
        //     switch ( buttonSP.plane ) {
        //     case exSprite.Plane.XY:
        //         boxCollider.center = new Vector3( boxCollider.center.x, boxCollider.center.y, 0.2f );
        //         break;

        //     case exSprite.Plane.XZ:
        //         boxCollider.center = new Vector3( boxCollider.center.x, 0.2f, boxCollider.center.z );
        //         break;

        //     case exSprite.Plane.ZY:
        //         boxCollider.center = new Vector3( 0.2f, boxCollider.center.y, boxCollider.center.z );
        //         break;
        //     }
        // }

        // // add collision helper
        // if ( buttonSP.collisionHelper == null ) {
        //     exCollisionHelper collisionHelper = buttonSP.gameObject.AddComponent<exCollisionHelper>();
        //     collisionHelper.plane = buttonSP;
        //     collisionHelper.autoLength = false;
        //     collisionHelper.length = 0.2f;
        //     collisionHelper.UpdateCollider();
        // }
        // } TODO end 

        //
        Selection.activeObject = buttonGO;
    }

}

