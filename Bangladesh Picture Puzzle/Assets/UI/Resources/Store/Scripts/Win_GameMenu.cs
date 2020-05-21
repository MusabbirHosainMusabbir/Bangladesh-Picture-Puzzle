using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Win_GameMenu : MonoBehaviour
{
	public static int resID;	// ID current size (0 = 2x2, 1=3x3 ...)

	public Transform rootImages;	
	public UIImageItem prefImage;
	public Text labelTitle;
	public Text labelSizeX;
	public Text labelSizeY;

	void Awake()
	{
		// display the current size
		labelSizeX.text = Game.piecesX.ToString();
		labelSizeY.text = Game.piecesY.ToString();

		// subscribe to image selection Event
		EventDispatcher.Add(EventName.GameSelectImage, GameSelectImage);
	}

	void Start()
	{
		UIRoot.Show(gameObject);
	}

	// set game data when loading windows
	void SetData(object[] args)
	{
		GameType game = (GameType)args [0];

		labelTitle.text = game.title;	// название

		// картинки
		foreach (Sprite image in game.images)
		{
			UIImageItem item = Lib.AddObject<UIImageItem>(prefImage, rootImages);
			item.SetData(image);
		}
	}

	// button MainMenu
	public void BackMainMenu()
	{
		UIRoot.Close(gameObject);
		UIRoot.Load(WindowName.Win_MainMenu);
	}

	// button Prev X
	public void PrevSizeX()
	{
		Game.piecesX--;
		if (Game.piecesX < 2)
			Game.piecesX = 2;

		labelSizeX.text = Game.piecesX.ToString();
	}

	// button Next X
	public void NextSizeX()
	{
		Game.piecesX++;
		if (Game.piecesX > 18)
			Game.piecesX = 18;
		
		labelSizeX.text = Game.piecesX.ToString();
	}

	// button Prev Y
	public void PrevSizeY()
	{
		Game.piecesY--;
		if (Game.piecesY < 2)
			Game.piecesY = 2;
		
		labelSizeY.text = Game.piecesY.ToString();
	}
	
	// button Next Y
	public void NextSizeY()
	{
		Game.piecesY++;
		if (Game.piecesY > 18)
			Game.piecesY = 18;
		
		labelSizeY.text = Game.piecesY.ToString();
	}
	
	// Event selection of pictures
	void GameSelectImage(object[] args)
	{
		Sprite image = (Sprite)args [0];

		// load the game with the selected picture
		EventDispatcher.SendEvent(EventName.BoardStartGame, image);
	}

}
