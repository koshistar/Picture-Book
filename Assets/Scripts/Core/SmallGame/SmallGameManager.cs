using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State
{
    Rock,
    Stretch,
    Vanish
}
public class SmallGameManager : MonoBehaviour
{
    public float stretchSpeed = 100f;
    public float maxStretch = 20f;
    public List<GameObject> fishPrefabs;
    public Text FishNumText;
    public Text LoadingText;
    public List<AudioSource> launchSFXs;
    private const float maxup = 2.5f;
    private const float maxdown = -4.4f;
    private const float maxleft = -8.1f;
    private const float maxright = 8.1f;
    private State state;

    private Vector3 dir;

    private Transform rope;

    private float length;

    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        state = State.Rock;
        dir = Vector3.back;
        rope=GetComponent<Transform>();
        length = rope.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Rock)
        {
            Rock();
            if (Input.GetMouseButtonDown(0))
            {
                state = State.Stretch;
                launchSFXs[UnityEngine.Random.Range(0,launchSFXs.Count-1)].Play();
            }   
        }
        else if (state == State.Stretch)
        {
            Stretch();
        }
        else if(state == State.Vanish)
        {
            Vanish();
        }

        if (GameData.hasFinished)
        {
            LoadingText.text = "生成完成！";
        }
        else
        {
            LoadingText.text = "生成中。";
        }
        FishNumText.text = "FishNum: " + GameData.fishNumber;
        timer += Time.deltaTime;
        if (timer > GameData.CD)
        {
            CreateFish();
        }
    }
    
    private void CreateFish()
    {
        float y = UnityEngine.Random.Range(maxup, maxdown);
        float x = UnityEngine.Random.Range(maxleft, maxright);
        GameObject fishPrefab = fishPrefabs[UnityEngine.Random.Range(0, fishPrefabs.Count - 1)];
        fishPrefab = GameObject.Instantiate(fishPrefab, new Vector3(x, y, rope.position.z), Quaternion.identity);
        fishPrefab.transform.SetParent(this.gameObject.transform.parent);
        timer = 0;
    }
    private void Rock()
    {
        dir = Vector3.forward;
        rope.Rotate(dir * 60 * Time.deltaTime);
    }

    private void Stretch()
    {
        if (length > maxStretch)
        {
            state = State.Vanish;
            return;
        }
        length += stretchSpeed * Time.deltaTime;
        rope.localScale = new Vector3(length, rope.localScale.y, rope.localScale.z);
    }

    private void Vanish()
    {
        length = 1f;
        rope.localScale = new Vector3(length, rope.localScale.y, rope.localScale.z);
        state=State.Rock;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            // Debug.Log("collider enter");
            state = State.Vanish;
            // StartCoroutine(MoveFish(other.transform));
            Destroy(other.gameObject);
            GameData.fishNumber++;
        }
    }

    // private IEnumerator MoveFish(Transform fish)
    // {
    //     yield return new WaitForSeconds(0.5f);
    //     if (fish.transform != rope)
    //     {
    //         fish.transform.Translate(new Vector3(rope.transform.position.x,rope.transform.position.y,rope.transform.position.z)*Time.deltaTime);
    //     }
    //     else
    //     {
    //         Destroy(fish.gameObject);
    //         yield return null;
    //     }
    // }
}
