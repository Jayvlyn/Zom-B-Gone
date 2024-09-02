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

    void Update()
    {
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20);
        //transform.position = new Vector3(_gameCamera.ScreenToWorldPoint(Input.mousePosition).x, _gameCamera.ScreenToWorldPoint(Input.mousePosition).y, 20);
    }
}
