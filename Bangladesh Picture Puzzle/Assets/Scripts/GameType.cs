using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameType : MonoBehaviour
{
	public int ID;					// ID game
	public string title;			// game name
	public Board board;				// board object
	public List<Sprite> images;		// pictures

	public Sprite GetNextPic(Sprite curImage)
	{
		for (int i=0; i<images.Count; i++)
		{
			if(images[i] == curImage)
			{
				if(i < images.Count - 1)
				{
					// get next image
					return images[i + 1];
				}
				else
				{
					// last image, exit, get first
					break;
				}
			}
		}

		return images[0];
	}
}
