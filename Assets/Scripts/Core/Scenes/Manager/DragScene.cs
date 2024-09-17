using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragScene : MonoBehaviour
{
    public float minLeft, minRight;
    private Vector2 mouseWorldPos, mouseStartPos;
    private Vector3 cameraStartPos;
    private float distance;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseWorldPos=GetMouseWorldPos(Input.mousePosition);
        //Êó±êÖÐ¼üÍÏ×§
        if(Input.GetMouseButtonDown(2))
        {
            mouseStartPos = mouseWorldPos;
            cameraStartPos = transform.position;
        }
        if(Input.GetMouseButton(2))
        {
            distance = mouseWorldPos.x - mouseStartPos.x;
            //ÅÐ¶Ï±ß½ç
            if (transform.position.x > minLeft && transform.position.x < minRight)
                transform.position = new Vector3(cameraStartPos.x - distance, transform.position.y, transform.position.z);
        }
    }
    Vector2 GetMouseWorldPos(Vector3 mousePos)
    {
        float factor = Screen.height / cam.orthographicSize / 2;
        float x = (mousePos.x - Screen.width / 2) / factor;
        float y = (mousePos.y - Screen.height / 2) / factor;
        return new Vector2(x, y);
    }
}
