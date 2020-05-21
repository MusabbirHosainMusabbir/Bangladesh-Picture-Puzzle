using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageItem : MonoBehaviour
{
	public Image image;

	public void SetData(Sprite sprite)
	{
		image.sprite = sprite;
		image.preserveAspect = true;
	}

	public void SelectImage()
	{
		EventDispatcher.SendEvent(EventName.GameSelectImage, image.sprite);
	}
}
