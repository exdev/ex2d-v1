// ======================================================================================
// File         : ScrollView.cs
// Author       : Wu Jie 
// Last Change  : 09/15/2011 | 10:38:43 AM | Thursday,September
// Description  : 
// ======================================================================================

///////////////////////////////////////////////////////////////////////////////
// usings
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////
// 
// ScrollView 
// 
///////////////////////////////////////////////////////////////////////////////

public class ScrollView : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////////
    // properties
    ///////////////////////////////////////////////////////////////////////////////

    public exPlane scrollButton;
    public exPlane scrollPage;

    exPlane plane;
    Vector2 scrollPos = Vector2.zero;
    bool dragging = false;
    Vector3 lastMouse = Vector2.zero;
    float yStart = 0.0f;

    ///////////////////////////////////////////////////////////////////////////////
    // functions
    ///////////////////////////////////////////////////////////////////////////////

    void Awake () {
        plane = GetComponent<exPlane>();
    }

    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Start () {
        yStart = transform.position.y - plane.boundingRect.y;

        Vector3 pos = scrollPage.transform.position;
        scrollPage.transform.position = new Vector3 ( pos.x, yStart, pos.z );

        pos = scrollButton.transform.position;
        scrollButton.transform.position = new Vector3 ( pos.x, yStart, pos.z );
	}
	
    // ------------------------------------------------------------------ 
    // Desc: 
    // ------------------------------------------------------------------ 

	void Update () {
        if ( Input.GetMouseButtonDown(0) ) {
            dragging = true;
            lastMouse = Input.mousePosition;
        }
        else if ( Input.GetMouseButtonUp(0) ) {
            dragging = false;
        }

        if ( dragging ) {
            Vector3 delta = Input.mousePosition - lastMouse;
            lastMouse = Input.mousePosition;

            scrollPos.y += delta.y;
            if ( scrollPos.y < 0 ) {
                scrollPos.y = 0.0f;

                scrollButton.transform.position 
                    = new Vector3 ( scrollButton.transform.position.x, 
                                    yStart - scrollPos.y,
                                    scrollButton.transform.position.z );
                scrollPage.transform.position 
                    = new Vector3 ( scrollPage.transform.position.x, 
                                    yStart + scrollPos.y,
                                    scrollPage.transform.position.z );
                return;
            }

            if ( (scrollPos.y + scrollButton.boundingRect.height) > plane.boundingRect.height ) {
                scrollPos.y = plane.boundingRect.height - scrollButton.boundingRect.height;

                scrollButton.transform.position 
                    = new Vector3 ( scrollButton.transform.position.x, 
                                    yStart - scrollPos.y,
                                    scrollButton.transform.position.z );
                scrollPage.transform.position 
                    = new Vector3 ( scrollPage.transform.position.x, 
                                    yStart + scrollPos.y,
                                    scrollPage.transform.position.z );

                return;
            }

            scrollButton.transform.Translate ( 0.0f, -delta.y, 0.0f );
            scrollPage.transform.Translate ( 0.0f, delta.y, 0.0f );
        }
	}
}
