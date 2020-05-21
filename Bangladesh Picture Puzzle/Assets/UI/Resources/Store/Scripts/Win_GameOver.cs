using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Win_GameOver : MonoBehaviour
{
	void Awake()
	{
	}

	void Start()
	{
		UIRoot.Show(gameObject);
	}

	public void BackMainMenu()
	{
		UIRoot.CloseAll();
		UIRoot.Load(WindowName.Win_MainMenu);
	}

	public void Replay()
	{
		UIRoot.Close(gameObject);
		EventDispatcher.SendEvent(EventName.BoardStartGame, Game.image);
	}

	public void NextPic()
	{
		UIRoot.Close(gameObject);
		Game.image = Content.currentGame.GetNextPic(Game.image);
		EventDispatcher.SendEvent(EventName.BoardStartGame, Game.image);
	}
	
	public void BackGameMenu()
	{
		EventDispatcher.SendEvent(EventName.GameSelect, Game.gameID);
	}
}
