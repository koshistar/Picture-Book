using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move3 : MonoBehaviour
{
    public GameObject picture3;
    public GameObject nextPicture;
    public static bool isUp3 = false;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp3)
        {
            picture3.transform.Rotate(new Vector3(0, 0, 1), 1);
            count++;
        }
        if (count>=360)
        {
            isUp3 = false;
            Invoke("Load", 2.0f);
        }
    }
    void Load()
    {
        Destroy(picture3);
        nextPicture.SetActive(true);
    }
}
