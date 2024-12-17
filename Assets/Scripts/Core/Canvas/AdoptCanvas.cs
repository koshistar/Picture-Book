using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdoptCanvas : MonoBehaviour
{
    public float widthRoatio = 16f / 9f;

    public float heightRoatio = 9 / 16f;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Adopt();
    }

    void Adopt()
    {
        float screenRatio = Screen.width / (float)Screen.height;
        if (screenRatio > widthRoatio)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y * screenRatio, rectTransform.sizeDelta.y);
        }
        else if (screenRatio < heightRoatio)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x / screenRatio);
        }
    }
}
