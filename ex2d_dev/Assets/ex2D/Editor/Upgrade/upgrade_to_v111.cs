// ======================================================================================
// File         : upgrade_to_v111.cs
// Author       : Wu Jie 
// Last Change  : 09/02/2011 | 15:35:17 PM | Friday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////

public static class upgrade_to_v111 {

    [MenuItem("Edit/ex2D Upgrade/upgrade to v1.1.1")]
    static void Exec () {
        EditorUtility.DisplayProgressBar( "Update Scene Sprite Layers...", 
                                          "Update Scene Sprite Layers...", 
                                          0.5f );    

        exLayer2D[] layerObjs = Resources.FindObjectsOfTypeAll(typeof(exLayer2D)) as exLayer2D[];
        for ( int i = 0; i < layerObjs.Length; ++i ) {
            exLayer2D layer2d = layerObjs[i]; 
            exPlane plane = layer2d.GetComponent<exPlane>();

            int layer = 0;
            float bias = 0.0f;
            if ( layer2d ) {
                layer = layer2d.layer; 
                bias = layer2d.bias; 
                Object.DestroyImmediate(layer2d);
            }
            switch ( plane.plane ) {
            case exPlane.Plane.XY: plane.layer2d = plane.gameObject.AddComponent<exLayerXY>(); break;
            case exPlane.Plane.XZ: plane.layer2d = plane.gameObject.AddComponent<exLayerXZ>(); break;
            case exPlane.Plane.ZY: plane.layer2d = plane.gameObject.AddComponent<exLayerZY>(); break;
            }
            plane.layer2d.SetLayer( layer, bias );
        }
        EditorUtility.ClearProgressBar();
    }
}
