using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move9 : MonoBehaviour
{
    public GameObject picture9;
    public GameObject nextPicture;
    public static bool isUp9 = false;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp9)
        {
            picture9.transform.Translate(new Vector2(picture9.transform.position.x, -4.62f) * 1 * Time.deltaTime);
        }
        if (picture9.transform.position.y <= -4.62f)
        {
            isUp9 = false;
            Invoke("Load", 2.0f);
        }
    }
    void Load()
    {
        Destroy(picture9);
        nextPicture.SetActive(true);
    }
}
