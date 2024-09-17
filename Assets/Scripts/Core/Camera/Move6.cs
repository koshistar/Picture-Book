using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move6 : MonoBehaviour
{
    public GameObject picture6;
    public GameObject nextPicture;
    public static bool isUp6 = false;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp6)
        {
            picture6.transform.Translate(new Vector2(picture6.transform.position.x, -4.62f) * 1 * Time.deltaTime);
        }
        if (picture6.transform.position.y <= -4.62f)
        {
            isUp6 = false;
            Invoke("Load", 2.0f);
        }
    }
    void Load()
    {
        Destroy(picture6);
        nextPicture.SetActive(true);
    }
}
