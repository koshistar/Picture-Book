using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move5 : MonoBehaviour
{
    public GameObject picture5;
    public GameObject nextPicture;
    public static bool isUp5 = false;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp5)
        {
            picture5.transform.Translate(new Vector2(picture5.transform.position.x, -4.69f) * 1 * Time.deltaTime);
        }
        if (picture5.transform.position.y <= -4.69f)
        {
            isUp5 = false;
            Invoke("Load", 2.0f);
        }
    }
    void Load()
    {
        Destroy(picture5);
        nextPicture.SetActive(true);
    }
}
