using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIGroup : MonoBehaviour
{
	private CanvasGroup cg;
	private Animator anim;

	private List<Action> onShow;
	private List<Action> onHide;

	private bool show;

	void Awake()
	{
		cg = GetComponent<CanvasGroup> ();
		cg.alpha = 0;
		cg.blocksRaycasts = false;

		anim = GetComponent<Animator> ();
	}

	public void AddOnClose(Action callBack)
	{
		if (onHide == null)
			onHide = new List<Action>();
		onHide.Add(callBack);
	}

	public void Show(params object[] args)
	{
		if (show)
			return;

		show = true;

		float delay = 0;
		if(args.Length > 0)
			delay = (float)args [0];
		if (args.Length > 1)
			onShow = (List<Action>)args [1];

		StartCoroutine (IShow (delay));
	}

	IEnumerator IShow(float delay)
	{
		yield return new WaitForSeconds (delay);

		if (show)
		{
			cg.blocksRaycasts = true;

			if (anim == null)
			{
				cg.alpha = 1;
			}
			else
			{
				anim.SetBool("show", true);
				anim.SetBool("hide", false);
			}

			if (onShow != null)
			{
				foreach (Action callBack in onShow)
				{
					if (callBack == null)
						continue;

					callBack();
				}
			}
		}
	}
	
	public void Hide(params object[] args)
	{
		if (!show)
			return;

		show = false;

		float delay = 0;
		if(args.Length > 0)
			delay = (float)args [0];
		if (args.Length > 1)
		{
			if(onHide == null)
				onHide = (List<Action>)args [1];
			else
				onHide.AddRange((List<Action>)args [1]);
		}
		
		StartCoroutine (IHide (delay));
	}
	
	IEnumerator IHide(float delay)
	{
		yield return new WaitForSeconds (delay);

		if (!show)
		{
			cg.blocksRaycasts = false;
			
			if (anim == null)
			{
				cg.alpha = 0;
			}
			else
			{
				anim.SetBool("show", false);
				anim.SetBool("hide", true);
			}

			if (onHide != null)
			{
				foreach (Action callBack in onHide)
				{
					if (callBack == null)
						continue;
				
					callBack();
				}
			}
		}
	}

	public void Remove()
	{
		Destroy(gameObject);
	}
}
