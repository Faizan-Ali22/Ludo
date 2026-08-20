using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudienceNetwork
{
	public class AdHandler : MonoBehaviour
	{
		private static readonly Queue<Action> executeOnMainThreadQueue = new Queue<Action>();

		public void ExecuteOnMainThread(Action action)
		{
			lock (executeOnMainThreadQueue)
			{
				executeOnMainThreadQueue.Enqueue(action);
			}
		}

		private void Update()
		{
			while (executeOnMainThreadQueue.Count > 0)
			{
				Action action = null;
				lock (executeOnMainThreadQueue)
				{
					try
					{
						action = executeOnMainThreadQueue.Dequeue();
					}
					catch (Exception e)
					{
						DConsole.LogException(e);
					}
				}
				action?.Invoke();
			}
		}

		public void RemoveFromParent()
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
