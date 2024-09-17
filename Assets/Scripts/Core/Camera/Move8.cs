using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move8 : MonoBehaviour
{
    public GameObject picture8;
    public static bool isUp8 = false;
    public GameObject nextPicture;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp8)
        {
            picture8.transform.Translate(new Vector2(picture8.transform.position.x, 4.04f) * 1 * Time.deltaTime);
        }
        if (picture8.transform.position.y >= 4.04f)
        {
            isUp8 = false;
            Invoke("Load", 1.0f);
        }
    }
    void Load()
    {
        nextPicture.SetActive(true);
    }
}
