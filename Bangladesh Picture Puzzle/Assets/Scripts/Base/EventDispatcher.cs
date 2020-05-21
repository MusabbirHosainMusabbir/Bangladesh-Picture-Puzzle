using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EventName
{
	None,
	ShowAds,

	GameSelect,
	GameSelectImage,
	GameOver,


	ModalMessage,

	BoardCellSelect,
	BoardStartGame,

}

public class EventDispatcher
{
	public static int stack;
	public static Dictionary<EventName, List<Action>> emptyEvents;
	public static Dictionary<EventName, List<Action<object[]>>> argsEvents;
	public static Dictionary<EventName, List<Action>> tmpEmptyEvents;
	public static Dictionary<EventName, List<Action<object[]>>> tmpArgsEvents;

	public static void Add(EventName eName, Action proc)
	{
		if (emptyEvents == null)
		{
			emptyEvents = new Dictionary<EventName, List<Action>>();
			tmpEmptyEvents = new Dictionary<EventName, List<Action>>();
		}
		
		if (!emptyEvents.ContainsKey(eName))
		{
			List<Action> procs = new List<Action>();
			List<Action> tmpProcs = new List<Action>();
			emptyEvents.Add(eName, procs);
			tmpEmptyEvents.Add(eName, tmpProcs);
		}
		
		if (stack == 0)
		{
			emptyEvents [eName].Add(proc);
		}
		else
		{
			// в процессе обработки
			tmpEmptyEvents [eName].Add(proc);
		}
	}

	public static void Add(EventName eName, Action<object[]> proc)
	{
		string en = eName.ToString();

		if (argsEvents == null)
		{
			argsEvents = new Dictionary<EventName, List<Action<object[]>>>();
			tmpArgsEvents = new Dictionary<EventName, List<Action<object[]>>>();
		}

		if (!argsEvents.ContainsKey(eName))
		{
			List<Action<object[]>> procs = new List<Action<object[]>>();
			List<Action<object[]>> tmpProcs = new List<Action<object[]>>();
			argsEvents.Add(eName, procs);
			tmpArgsEvents.Add(eName, tmpProcs);
		}

		if (stack == 0)
		{
			argsEvents [eName].Add(proc);
		}
		else
		{
			// в процессе обработки
			tmpArgsEvents [eName].Add(proc);
		}
	}

	public static void Remove(EventName eName)
	{
		if (emptyEvents != null && emptyEvents.ContainsKey(eName))
		{
			emptyEvents.Remove(eName);
		}

		if (argsEvents != null && argsEvents.ContainsKey(eName))
		{
			argsEvents.Remove(eName);
		}

		if (tmpEmptyEvents != null && tmpEmptyEvents.ContainsKey(eName))
		{
			tmpEmptyEvents.Remove(eName);
		}
		
		if (tmpArgsEvents != null && tmpArgsEvents.ContainsKey(eName))
		{
			tmpArgsEvents.Remove(eName);
		}
	}

	public static void SendEvent(EventName eName, params object[] args)
	{
		stack++;
//		Debug.Log("stack++ " + stack + " " + eName.ToString());

		if (emptyEvents != null && emptyEvents.ContainsKey(eName))
		{
			foreach (Action proc in emptyEvents[eName])
			{
				if (proc.Target.ToString() != "null")
					proc();
			}
		}

		if (argsEvents != null && argsEvents.ContainsKey(eName))
		{
			foreach (Action<object[]> proc in argsEvents[eName])
			{
				if (proc.Target.ToString() != "null")
					proc(args);
			}
		}
		
		stack--;
//		Debug.Log("stack-- " + stack);

		if (stack == 0)
		{
			// добавление процедур вызванных во время обработки
			if(tmpEmptyEvents != null)
			{
				foreach (KeyValuePair<EventName,List<Action>> pair in tmpEmptyEvents)
				{
					if (!emptyEvents.ContainsKey(pair.Key))
					{
						emptyEvents.Add(pair.Key, pair.Value);
					}
					else
					{
						foreach (Action proc in tmpEmptyEvents[pair.Key])
						{
							emptyEvents [pair.Key].Add(proc);
						}
					}
					tmpEmptyEvents [pair.Key].Clear();
				}
			}

			if(tmpArgsEvents != null)
			{
				foreach (KeyValuePair<EventName,List<Action<object[]>>> pair in tmpArgsEvents)
				{
					if (!argsEvents.ContainsKey(pair.Key))
					{
						argsEvents.Add(pair.Key, pair.Value);
					}
					else
					{
						foreach (Action<object[]> proc in tmpArgsEvents[pair.Key])
						{
							argsEvents [pair.Key].Add(proc);
						}
					}
					tmpArgsEvents [pair.Key].Clear();
				}
			}

			// очистка процедур удалённых объектов
			if (emptyEvents != null)
			{
				foreach (List<Action> e in emptyEvents.Values)
				{
					e.RemoveAll(item => item.Target.ToString() == "null");
				}
			}

			if (argsEvents != null)
			{
				foreach (List<Action<object[]>> e in argsEvents.Values)
				{
					e.RemoveAll(item => item.Target.ToString() == "null");
				}
			}
		}
	}

}

