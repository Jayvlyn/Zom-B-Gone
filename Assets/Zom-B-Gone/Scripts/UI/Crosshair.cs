using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    //private Camera _gameCamera;
    //public Texture2D cursorTex;
    //private Vector2 hotSpotAuto;
    //private void Awake()
    //{
    //    hotSpotAuto = new Vector2(cursorTex.width * 0.5f, cursorTex.height * 0.5f);
    //    Vector2 hotSpot = hotSpotAuto;

    //    _gameCamera = FindObjectOfType<Camera>();
    //    //Cursor.SetCursor(cursorTex, hotSpot, CursorMode.ForceSoftware);
    //}

    public Transform worldPoint;
    public Transform playerT;

    void Update()
    {
		transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20);
		
        if(worldPoint)
        {
		    Vector3 mouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (mouseViewportPos.x < 0 || mouseViewportPos.x > 1 || mouseViewportPos.y < 0 || mouseViewportPos.y > 1)
		    { // mouse outside viewport
			    worldPoint.position = playerT.position;
		    }
		    else
		    { // mouse inside viewport
                worldPoint.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 20);
		    }
        }
	}
}
