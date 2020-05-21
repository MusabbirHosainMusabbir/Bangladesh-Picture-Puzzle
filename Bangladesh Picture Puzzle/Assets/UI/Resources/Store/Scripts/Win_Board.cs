using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Win_Board : MonoBehaviour
{
	void Start()
	{
		UIRoot.Show(gameObject);
	}

	public void BackGameMenu()
	{
		EventDispatcher.SendEvent(EventName.GameSelect, Game.gameID);
	}

}
