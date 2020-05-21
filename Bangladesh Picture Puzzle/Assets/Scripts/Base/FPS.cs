using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FPS : MonoBehaviour
{
	public string ver = "ver 226r";
	public Text label;
	
	int counter;
	float timer;
	
	void Start()
	{
		label.text = "0";
		counter = 0;
		timer = 0;
	}

	void Update()
	{
/*
		if (UserData.GetAccess(LevelAccess.ROOT) || UserData.GetAccess(LevelAccess.DEVELOPER))
		{
			label.enabled = true;
		}
		else
		{
			label.enabled = false;
			return;
		}
*/
		counter++;
		timer += Time.deltaTime;
		
		if(timer > 1) {
			timer -= 1;
			label.text = "FPS: " + counter.ToString() + "\n" + 
				Screen.width + "x" + Screen.height + " (" + Screen.dpi + ")\n" + 
					ver;
			counter = 0;
		}
	}
}


