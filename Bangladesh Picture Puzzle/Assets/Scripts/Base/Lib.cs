using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;

public class Lib : MonoBehaviour
{
	public static void SetLayer(GameObject obj, int layer, bool includeChildren = true)
	{
		obj.layer = layer;
		if (includeChildren)
		{
			foreach (Transform trans in obj.transform.GetComponentsInChildren<Transform>(true))
			{
				trans.gameObject.layer = layer;
			}
		}
	}

	public static int ConvertToInt(string value)
	{
		try
		{
			return Convert.ToInt32(value);
		} catch (Exception e)
		{
			return 0;
		}
	}

	public static GameObject AddObject(GameObject obj, Transform parent) {
		
		GameObject itemObj = (GameObject)Instantiate(obj);
		itemObj.transform.SetParent(parent);
		itemObj.transform.localPosition = Vector3.zero;
		itemObj.transform.localRotation = Quaternion.identity;
		itemObj.transform.localScale    = Vector3.one;
		
		RectTransform rt = itemObj.GetComponent<RectTransform>();
		if (rt != null) {
			rt.anchoredPosition3D = Vector3.zero;
		}
		
		return itemObj;
	}
	
	public static T AddObject<T>(T obj, Transform parent) where T : Component
	{
		if (obj == null)
		{
			GameObject newObj = new GameObject();
			newObj.AddComponent<T>();
			obj = newObj.GetComponent<T>();
		}
		
		return AddObject<T>(obj.gameObject, parent, false);
	}
	
	public static T AddObject<T>(T obj, Transform parent, bool active) where T : Component
	{
		if (obj == null)
		{
			GameObject newObj = new GameObject();
			newObj.AddComponent<T>();
			obj = newObj.GetComponent<T>();
		}
		
		return AddObject<T>(obj.gameObject, parent, active);
	}
	
	public static T AddObject<T>(GameObject obj, Transform parent, bool active) where T : Component
	{
		GameObject itemObj = (GameObject)Instantiate(obj);
		itemObj.transform.SetParent(parent);
		itemObj.transform.localPosition = Vector3.zero;
		itemObj.transform.localRotation = Quaternion.identity;
		itemObj.transform.localScale = Vector3.one;
		if (active)
			itemObj.gameObject.SetActive(true);
		
		return itemObj.GetComponent<T>();
	}
	
	public static void RemoveObjects(Transform root)
	{
		foreach (Transform child in root)
		{
			child.gameObject.SetActive(false);
			Destroy(child.gameObject);
		}
	}

	public static string[] ParseStr(string data, string split)
	{
		if (data == null)
			data = "";
		string[] array = data.Split(new string[]{split}, System.StringSplitOptions.None);
		return array;
	}
}

