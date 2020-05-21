using UnityEngine;
using System.Collections.Generic;

public class Win : MonoBehaviour
{
	void Start()
	{
		UIRoot.Show(gameObject);
	}

	public void Close()
	{
		UIRoot.Close(gameObject);
	}
}
