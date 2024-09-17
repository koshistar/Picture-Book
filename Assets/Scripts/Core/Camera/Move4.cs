using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move4 : MonoBehaviour
{
    public GameObject picture4;
    public GameObject nextPicture;
    public static bool isUp4 = false;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp4)
        {
            picture4.transform.Translate(new Vector2(picture4.transform.position.x, -9.9f) * 1 * Time.deltaTime);
        }
        if (picture4.transform.position.y <= -9.9f)
        {
            isUp4 = false;
            Invoke("Load", 2.0f);
        }
    }
    void Load()
    {
        Destroy(picture4);
        nextPicture.SetActive(true);
    }
}
