using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class keyE : MonoBehaviour
{
    public TMP_Text ui;

	//private void OnTriggerEnter(Collider other)
	//{	
	//	Debug.Log("碰撞了");
	//	if (other.tag=="Enemy")
	//	{
	//		ui.text =  other.name+"\n按下 E 键以对话:";
	//	}			
	//}
	private void OnCollisionEnter(Collision collision)
	{

		//Debug.Log("你走入了神人" + collision.collider.name+"的地盘");
		ui.text = "你走入了神人" + collision.collider.name + "的地盘" + "\n按 E 键开启对话";
	}
	private void OnCollisionStay(Collision collision)
	{
		if (Input.GetKey(KeyCode.E))
		if (collision.collider.tag == "Enemy")
		{
			ui.text = collision.collider.name + ':'+'"'+"我要对你开大啦" +'"';
			//在这里实现对话等事件的逻辑
		}
	}
}
