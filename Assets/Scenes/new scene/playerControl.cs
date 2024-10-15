using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControl : MonoBehaviour
{
    private Animator ani;
    private Rigidbody rb;

    void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float Horizontal = 0;
        float Vertical = 0;
        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(Horizontal, 0, Vertical);

        if (Horizontal !=0)
        {
            ani.SetFloat("Horizontal", Horizontal);
            ani.SetFloat("Vertical", 0);
        }
        if (Vertical !=0)
        {
			ani.SetFloat("Horizontal", 0);
			ani.SetFloat("Vertical", Vertical);
		}

        ani.SetFloat("Speed", dir.magnitude);


		rb.velocity = dir;
	}
}
