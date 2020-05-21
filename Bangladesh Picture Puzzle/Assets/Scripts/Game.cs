using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	public static int gameID;		// The current game ID
	public static Sprite image;		// current picture
	public static int piecesX;		// the number of pieces on the side
	public static int piecesY;		// the number of pieces on the side

	public Content content;			// object games content
	public Transform rootScene;		// stage root for loading games

	void Awake()
	{
		piecesX = 2;
		piecesY = 2;

		// subscription events
		EventDispatcher.Add(EventName.GameSelect, GameSelect);
		EventDispatcher.Add(EventName.BoardStartGame, BoardStartGame);
		EventDispatcher.Add(EventName.GameOver, GameOver);
	}

	void Start ()
	{
		// load MainMenu window
		UIRoot.Load (WindowName.Win_MainMenu);
	}

	void GameSelect(object[] args)
	{
		// select ID game
		gameID = (int)args [0];
	}

	void BoardStartGame(object[] args)
	{
		// load game
		image = (Sprite)args [0];	// picture

		Lib.RemoveObjects(rootScene);
		// load the game from the content list
		Board board = Lib.AddObject<Board>(content.games [gameID].board, rootScene);
		board.SendMessage("SetData", SendMessageOptions.DontRequireReceiver);

		// load the game interface window
		UIRoot.CloseAll();
		UIRoot.Load(WindowName.Win_Board);
	}

	void GameOver()
	{
		// GameOver show window
		UIRoot.Load(WindowName.Win_GameOver);
	}
		
}
