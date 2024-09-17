using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move1 : MonoBehaviour
{
    public GameObject picture1;
    public GameObject nextPicture;
    public static bool isUp1 = false;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp1)
        {
            picture1.transform.Translate(new Vector2(picture1.transform.position.x, 4.7f) * 1* Time.deltaTime);
        }
        if (picture1.transform.position.y >= 4.7f)
        {
            isUp1 = false;
            Invoke("Load", 2.0f);
        }
    }
    void Load()
    {
        Destroy(picture1);
        nextPicture.SetActive(true);
    }
}
