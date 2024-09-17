using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2 : MonoBehaviour
{
    public GameObject picture2;
    public GameObject nextPicture;
    public static bool isUp2 = false;
    // Start is called before the first frame update
    void Start()
    {
        //picture1 = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUp2)
        {
            picture2.transform.Translate(new Vector2(picture2.transform.position.x, 4.62f) * 1 * Time.deltaTime);
        }
        if (picture2.transform.position.y >= 4.62f)
        {
            isUp2 = false;
            Invoke("Load", 2.0f);
        }
    }
    void Load()
    {
        Destroy(picture2);
        nextPicture.SetActive(true);
    }
}
