using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseInteract : MonoBehaviour
{
	public AnimationCurve scaleCurve;
	public float curveDuration;
	private Vector3 originalScale= Vector3.one;

	public AudioSource audio;

	IEnumerator ScaleEffect(GameObject parentObject)
	{
		float elapsedTime = 0f;
		//Transform parentTransform = transform.parent;

		while (elapsedTime < curveDuration)
		{
			elapsedTime += Time.deltaTime;
			float curveValue = scaleCurve.Evaluate(elapsedTime / curveDuration);

			parentObject.transform.localScale *= curveValue;

			yield return null;
		}

		// ȷ����Ч������ʱ�ָ�ԭʼ��С
		parentObject.transform.localScale = originalScale;
	}

	private void Update()
	{
		//�����߼���ж��Ƿ񵥻�����
		//����������
		if (Input.GetMouseButtonDown(0))
		{
			//��ȡ���ָ��λ�õ�����
			Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
			//��ײ��Ϣ
			RaycastHit hit;
			//���������ײ
			if(Physics.Raycast(ray, out hit))
			{
				Debug.Log("yes,it hits");
				audio.Play();

				Transform parentTransform=hit.collider.transform.parent;
				if (parentTransform != null)
				{
					GameObject parentObject = parentTransform.gameObject;
					Debug.Log("Parent object: " + parentObject.name);

					//ʵ�ֵ�����������Ĵ�С�仯
					StartCoroutine(ScaleEffect(parentObject));



					/*
					 ��������Ӹ��������Ĵ���
					 */



				}
				else
				{
					Debug.Log("This object has no parent.");
				}
			}
			else
			{
				Debug.Log("no,no hit");
			}
		}
		
	}
}
