using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UIRoot : MonoBehaviour
{
	public static string resPath;

	public static float size;
	public static int width;
	public static int height;

	private static UIRoot _instance;
	public static UIRoot instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<UIRoot> ();
			return _instance;
		}
	}

	public static int screenWidth;
	public static int screenHeight;

	public string pathStore;

	public bool clearOnStart;

	public float minSize = 625;
	public float maxSize = 700;

	private RectTransform rt;

	void Awake ()
	{
		Init ();
	}

	public void Init()
	{
		if (rt != null)
			return;

		rt = GetComponent<RectTransform> ();

		if (!Application.isPlaying)
			return;

		if (clearOnStart)
			Lib.RemoveObjects (transform);
	}

	public static GameObject Load(string prefName, float delay)
	{
		GameObject obj = Load (prefName);
		Show (obj, delay);
		return obj;
	}

	public static GameObject Load(WindowName prefName, params object[] args)
	{
		return Load(prefName.ToString(), args);
	}

	public static GameObject Load(string prefName, params object[] args)
	{
		GameObject obj = Load (prefName);
		Show (obj, 0);
		SetData (obj, args);
		return obj;
	}

	public static GameObject Load(WindowName prefName, float delay, params object[] args)
	{
		return Load(prefName.ToString(), delay, args);
	}

	public static GameObject Load(string prefName, float delay, params object[] args)
	{
		GameObject obj = Load (prefName);
		Show (obj, delay);
		SetData (obj, args);
		return obj;
	}
	
	public static GameObject Load(WindowName prefName, Transform root = null)
	{
		return Load(prefName.ToString(), root);
	}
	
	public static GameObject Load(string prefName, Transform root = null)
	{
		if (root == null)
			root = instance.transform;

		instance.Init ();

		GameObject obj = Resources.Load<GameObject> ("Store/" + prefName);
		if (obj == null)
		{
			Debug.Log ("Root: Не найден объект для добавления: " + prefName);
			return null;
		}

		GameObject pref = Instantiate<GameObject> (obj);
		RectTransform rt = pref.GetComponent<RectTransform>();
		Vector3 v = rt.anchoredPosition3D;
		Vector2 offMin = rt.offsetMin;
		Vector2 offMax = rt.offsetMax;

		pref.transform.SetParent(root);
		pref.transform.localScale = Vector3.one;

		if (rt.anchorMin == Vector2.zero && rt.anchorMax == Vector2.one)
		{
			rt.anchoredPosition3D = Vector3.zero;
			rt.offsetMin = offMin;
			rt.offsetMax = offMax;
		}
		else
		{
			rt.anchoredPosition3D = v;
		}
		
		pref.name = prefName;

		return pref;
	}

	public static void Show(GameObject obj, float delay = 0, Action onShow = null)
	{
		UIGroup groupMain = obj.GetComponent<UIGroup> ();
		if(groupMain != null)
			groupMain.Show (delay, new List<Action>() {onShow});

		UIGroup[] groups = obj.GetComponentsInChildren<UIGroup> (true);
		foreach (UIGroup group in groups)
		{
			if(group == groupMain)
				continue;
			group.Show (delay);
		}
	}

	public static void CloseAllExceptFor(params WindowName[] objNames)
	{
		string[] array = new string[objNames.Length];
		for(int i=0; i<objNames.Length; i++)
		{
			WindowName item = objNames[i];
			array[i] = item.ToString();
		}
		CloseAllExceptFor(array);
	}
	
	public static void CloseAllExceptFor(params string[] objNames)
	{
		List<string> list = new List<string>(objNames);

		foreach (Transform child in instance.transform)
		{
			if(!list.Contains(child.name))
			{
				Close(child.gameObject);
			}
		}
	}

	public static void Close(params WindowName[] objNames)
	{
		string[] array = new string[objNames.Length];
		for(int i=0; i<objNames.Length; i++)
		{
			WindowName item = objNames[i];
			array[i] = item.ToString();
		}
		Close(array);
	}

	public static void Close(params string[] objNames)
	{
		List<string> list = new List<string>(objNames);
		
		foreach (Transform child in instance.transform)
		{
			if(list.Contains(child.name))
			{
				Close(child.gameObject);
			}
		}
	}
	
	public static void Hide(string objName, float delay = 0, Action onHide = null)
	{
		Transform obj = instance.transform.Find(objName);
		if (obj == null)
			return;

		Hide(obj.gameObject, delay, onHide);
	}

	public static void Hide(GameObject obj, float delay = 0, Action onHide = null)
	{
		UIGroup groupMain = obj.GetComponent<UIGroup> ();
		groupMain.Hide (delay, new List<Action>() {onHide});
		
		UIGroup[] groups = obj.GetComponentsInChildren<UIGroup> ();
		foreach (UIGroup group in groups)
		{
			if(group == groupMain)
				continue;
			group.Hide (delay);
		}
	}

	public static void Close(GameObject obj, float delay = 0, Action onClose = null)
	{
		UIGroup groupMain = obj.GetComponent<UIGroup> ();
		groupMain.Hide (delay, new List<Action>() {groupMain.Remove, onClose});
	}

	public static void OnClose(GameObject obj, Action onClose)
	{
		UIGroup groupMain = obj.GetComponent<UIGroup> ();
		groupMain.AddOnClose (onClose);
	}

	public static void SetData(GameObject obj, object[] args)
	{
		obj.SendMessage ("SetData", args, SendMessageOptions.DontRequireReceiver);
	}

	public static void CloseAll()
	{
		foreach (Transform child in instance.transform)
		{
			Close(child.gameObject);
		}
	}

	public static void Clear()
	{
		Lib.RemoveObjects (instance.transform);
	}







	void Update ()
	{
		if (Screen.width != screenWidth || Screen.height != screenHeight)
		{			
			size = 1;
			width = (int)Screen.width;
			height = (int)Screen.height;

			float value = Mathf.Max(Screen.width, Screen.height);
			
			if (value < minSize)
			{
				// размер экрана меньше минимального, уменьшаем scale
				size = value / maxSize;
				width = (int)(Screen.width / size);
				height = (int)(Screen.height / size);
				return;
			}
			
			if (value > maxSize && Screen.dpi > 10)
			{
				// размер экрана больше максимального, увеличиваем scale (для телефонов)
				size = value / maxSize;
				width = (int)(Screen.width / size);
				height = (int)(Screen.height / size);
				return;
			}
		}
	}

	void LateUpdate ()
	{
		if (Screen.width != screenWidth || Screen.height != screenHeight)
		{
			screenWidth = Screen.width;
			screenHeight = Screen.height;

			if (width % 2 == 1)
				width--;
			
			if (height % 2 == 1)
				height--;
			
			rt.sizeDelta = new Vector2(width, height);
			transform.localScale = new Vector3(size, size, 1);
		}
	}
}
