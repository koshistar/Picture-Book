using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move7 : MonoBehaviour
{
    public GameObject picture7;
    public GameObject nextPicture;
    public static bool isUp7 = false;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp7)
        {
            picture7.transform.Translate(new Vector2(picture7.transform.position.x, -4.66f) * 1 * Time.deltaTime);
        }
        if (picture7.transform.position.y <= -4.66f)
        {
            isUp7 = false;
            Invoke("Load", 2.0f);
        }
    }
    void Load()
    {
        Destroy(picture7);
        nextPicture.SetActive(true);
    }
}
