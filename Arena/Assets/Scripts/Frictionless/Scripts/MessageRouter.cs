using System;
using System.Collections.Generic;
using System.Text;

namespace Frictionless
{
	public class MessageRouter
	{
		private Dictionary<Type,List<MessageHandler>> _handlers = new Dictionary<Type, List<MessageHandler>>();
		private List<Delegate> _pendingRemovals = new List<Delegate>();
		private bool _isRaisingMessage;

		public MessageRouter()
		{
		}
		
		public void AddListener<T>(Action<T> handler, ref Action unregisterAction)
		{
			AddListener(handler);
			unregisterAction += () => { RemoveListener(handler); };
		}
		
		public void AddListener<T>(Action<T> handler)
		{
			List<MessageHandler> delegates = null;
			
			if (!_handlers.TryGetValue(typeof(T), out delegates))
			{
				delegates = new List<MessageHandler>();
				_handlers[typeof(T)] = delegates;
			}

			if (delegates.Find(x => x.Delegate == (Delegate) handler) == null)
			{
				delegates.Add(new MessageHandler() {Target = handler.Target, Delegate = handler});
			}
		}

		public void RemoveListener<T>(Action<T> handler)
		{			
			List<MessageHandler> delegates = null;
			if (!_handlers.TryGetValue(typeof(T), out delegates))
			{
				return;
			}
			
			MessageHandler existingHandler = delegates.Find(x => x.Delegate == (Delegate) handler);
			if (existingHandler == null)
			{
				return;
			}
				
			if (_isRaisingMessage)
			{
				_pendingRemovals.Add(handler);
			}
			else
			{
				delegates.Remove(existingHandler);
			}
		}

		public void Reset()
		{
			_handlers.Clear();
		}
		
		public void RaiseMessage(object msg)
		{
			try
			{
				List<MessageHandler> delegates = null;
				
				if (!_handlers.TryGetValue(msg.GetType(), out delegates))
				{
					return;
				}
				
				_isRaisingMessage = true;
				
				try
				{
					foreach (MessageHandler h in delegates)
					{
#if NETFX_CORE
						h.Delegate.DynamicInvoke(msg);
#else
						h.Delegate.Method.Invoke(h.Target, new object[] { msg });
#endif
					}
				}
				finally
				{
					_isRaisingMessage = false;
				}
				foreach (Delegate d in _pendingRemovals)
				{
					MessageHandler existingHandler = delegates.Find(x => x.Delegate == d);				
					if (existingHandler != null)
					{
						delegates.Remove(existingHandler);
					}
				}
				_pendingRemovals.Clear();
			}
			catch(Exception ex)
			{
				UnityEngine.Debug.LogError("Exception while raising message " + msg + ": " + ex);
			}
		}

		public string GetAllHandlerDebugInfo()
		{
			if (_handlers.Count == 0)
			{
				return "No message handlers registered.";
			}
			
			StringBuilder debugOutput = new StringBuilder();

			int total = 0;
			
			foreach (var handler in _handlers)
			{
				debugOutput.AppendLine($"Message {handler.Key.Name} registered with {handler.Value.Count} handlers");
				total += handler.Value.Count;
			}

			debugOutput.AppendLine("Total handlers: " + total);

			return debugOutput.ToString();
		}

		public string GetHandlerDebugInfo<T>()
		{		
			List<MessageHandler> delegates;
			if (!_handlers.TryGetValue(typeof(T), out delegates))
			{
				return $"Message {typeof(T)} is not registered.";
			}	

			return $"Message {typeof(T)} registered with {delegates.Count} handlers";
		}

		private class MessageHandler
		{
			public object Target { get; set; }
			public Delegate Delegate { get; set; }
		}
	}
}
