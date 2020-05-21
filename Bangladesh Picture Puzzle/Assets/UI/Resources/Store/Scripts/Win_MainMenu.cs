using UnityEngine;
using System.Collections.Generic;

public class Win_MainMenu : MonoBehaviour
{
	void Start()
	{
		UIRoot.Show(gameObject);
	}

	// The game selection buttons
	public void SelectGame(int value)
	{
		EventDispatcher.SendEvent(EventName.GameSelect, value);	// value = ID game
		UIRoot.Close(gameObject);
	}
}
