using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class keyE : MonoBehaviour
{
    public TMP_Text ui;

	//private void OnTriggerEnter(Collider other)
	//{	
	//	Debug.Log("��ײ��");
	//	if (other.tag=="Enemy")
	//	{
	//		ui.text =  other.name+"\n���� E ���ԶԻ�:";
	//	}			
	//}
	private void OnCollisionEnter(Collision collision)
	{

		//Debug.Log("������������" + collision.collider.name+"�ĵ���");
		ui.text = "������������" + collision.collider.name + "�ĵ���" + "\n�� E ������Ի�";
	}
	private void OnCollisionStay(Collision collision)
	{
		if (Input.GetKey(KeyCode.E))
		{
			if (collision.collider.tag == "Enemy1")
			{
				ui.text = collision.collider.name + ':' + '"' + "��Ҫ���㿪����" + '"';
				SceneManager.LoadScene(3);
				//������ʵ�ֶԻ����¼����߼�
			}
            if (collision.collider.tag == "Enemy2")
            {
                ui.text = collision.collider.name + ':' + '"' + "��Ҫ���㿪����" + '"';
                SceneManager.LoadScene(4);
                //������ʵ�ֶԻ����¼����߼�
            }
        }
	}
}
