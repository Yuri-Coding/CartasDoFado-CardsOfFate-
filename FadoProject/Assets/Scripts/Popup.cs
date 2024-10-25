using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour
{
	private Animation anim;

	public TMP_Text contentObject;
    private bool isWaitingForInput = true;

    void Start()
	{
		anim = gameObject.GetComponent<Animation>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.D))
		{
			Popdown();
		}
	}

	public void PopupMessage(string content)
	{
		anim.Play("fadein");
		contentObject.text = content;

        StartCoroutine(AutoHidePopup(7f));
    }

	void Popdown()
	{
		anim.Play("fadeout");
        isWaitingForInput = true;
        GameManager.Instance.OnInitPopdown();
	}

    
    

	private IEnumerator AutoHidePopup(float duration)
	{
		float elapsedTime = 0f;

		while (isWaitingForInput && elapsedTime < duration) {
			if (Input.anyKeyDown) {
				isWaitingForInput = false;
				break;
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		Popdown();
	}
}