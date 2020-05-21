using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Content : MonoBehaviour
{
	public static GameType currentGame;		// current selected game
	public Dictionary<int, GameType> games;	// list of games

	void Awake()
	{
		// form a list of games
		games = new Dictionary<int, GameType>();
		foreach (Transform child in transform)
		{
			GameType game = child.GetComponent<GameType>();
			games.Add(game.ID, game);
		}

		EventDispatcher.Add(EventName.GameSelect, GameSelect);
	}

	void GameSelect(object[] args)
	{
		// select game
		int ID = (int)args [0];

		// load game menu with its data
		currentGame = games [ID];
		UIRoot.Load(WindowName.Win_GameMenu, currentGame);
	}
}
