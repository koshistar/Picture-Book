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

		// 确保在效果结束时恢复原始大小
		parentObject.transform.localScale = originalScale;
	}

	private void Update()
	{
		//用射线检测判断是否单击物体
		//单击鼠标左键
		if (Input.GetMouseButtonDown(0))
		{
			//获取鼠标指针位置的射线
			Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
			//碰撞信息
			RaycastHit hit;
			//如果发生碰撞
			if(Physics.Raycast(ray, out hit))
			{
				Debug.Log("yes,it hits");
				audio.Play();

				Transform parentTransform=hit.collider.transform.parent;
				if (parentTransform != null)
				{
					GameObject parentObject = parentTransform.gameObject;
					Debug.Log("Parent object: " + parentObject.name);

					//实现点击物体后，物体的大小变化
					StartCoroutine(ScaleEffect(parentObject));



					/*
					 在这里添加更换场景的代码
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
