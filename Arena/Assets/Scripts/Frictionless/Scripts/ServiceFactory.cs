using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// A simple, *single-threaded*, service locator appropriate for use with Unity.
/// </summary>

namespace Frictionless
{
	public class ServiceFactory
	{
		private static ServiceFactory _instance;

		private Dictionary<Type,Type> _singletons = new Dictionary<Type, Type>();
		private Dictionary<Type,Type> _transients = new Dictionary<Type, Type>();
		private Dictionary<Type,object> _singletonInstances = new Dictionary<Type, object>();

		static ServiceFactory()
		{
			_instance = new ServiceFactory();
		}

		protected ServiceFactory()
		{
		}

		public static ServiceFactory Instance
		{
			get { return _instance; }
		}

		public bool IsEmpty
		{
			get { return _singletons.Count == 0 && _transients.Count == 0; }
		}

		public void HandleNewSceneLoaded()
		{
			List<IMultiSceneSingleton> multis = new List<IMultiSceneSingleton>();
			foreach(KeyValuePair<Type,object> pair in _singletonInstances)
			{
				IMultiSceneSingleton multi = pair.Value as IMultiSceneSingleton;
				if (multi != null)
				{
					multis.Add (multi);
				}
			}
			foreach(var multi in multis)
			{
				MonoBehaviour behavior = multi as MonoBehaviour;
				if (behavior != null)
				{
					behavior.StartCoroutine(multi.HandleNewSceneLoaded());
				}
			}
		}

		public void Reset()
		{
			List<Type> survivorRegisteredTypes = new List<Type>();
			List<object> survivors = new List<object>();
			foreach(KeyValuePair<Type,object> pair in _singletonInstances)
			{
				if (pair.Value is IMultiSceneSingleton)
				{
					survivors.Add(pair.Value);
					survivorRegisteredTypes.Add(pair.Key);
				}
			}
			_singletons.Clear();
			_transients.Clear();
			_singletonInstances.Clear();

			for (int i = 0; i < survivors.Count; i++)
			{
				_singletonInstances[survivorRegisteredTypes[i]] = survivors[i];
				_singletons[survivorRegisteredTypes[i]] = survivors[i].GetType();
			}
		}

		public void RegisterSingleton<TConcrete>()
		{
			_singletons[typeof(TConcrete)] = typeof(TConcrete);
		}

		public void RegisterSingleton<TAbstract,TConcrete>()
		{
			_singletons[typeof(TAbstract)] = typeof(TConcrete);
		}
		
		public void RegisterSingleton<TConcrete>(TConcrete instance)
		{
			_singletons[typeof(TConcrete)] = typeof(TConcrete);
			_singletonInstances[typeof(TConcrete)] = instance;
		}

		public void RegisterTransient<TAbstract,TConcrete>()
		{
			_transients[typeof(TAbstract)] = typeof(TConcrete);
		}

		public T GetService<T>() where T : class
		{
			return GetService<T>(false);
		}

		public T GetService<T>(bool onlyExisting) where T : class
		{
			T result = default(T);
			Type concreteType = null;
			if (_singletons.TryGetValue(typeof(T), out concreteType))
			{
				object r = null;
				if (!_singletonInstances.TryGetValue(typeof(T), out r) && !onlyExisting)
				{
	#if NETFX_CORE
					if (concreteType.GetTypeInfo().IsSubclassOf(typeof(MonoBehaviour)))
	#else
					if (concreteType.IsSubclassOf(typeof(MonoBehaviour)))
	#endif
					{
						GameObject singletonGameObject = new GameObject();
						r = singletonGameObject.AddComponent(concreteType);
						singletonGameObject.name = typeof(T).ToString() + " (singleton)";
					}
					else
					{
						r = Activator.CreateInstance(concreteType);
					}
					
					_singletonInstances[typeof(T)] = r;

					if (r is IMultiSceneSingleton multi)
					{
						multi.HandleNewSceneLoaded();
					}
				}
				result = (T)r;
			}
			else if (_transients.TryGetValue(typeof(T), out concreteType))
			{
				object r = Activator.CreateInstance(concreteType);
				result = (T)r;
			}
			
			return result;
		}

		public string GetDebugInfo()
		{
			StringBuilder debugOutput = new StringBuilder();

			debugOutput.AppendLine($"Singletons registered: {_singletons.Count}");
			
			foreach(var registeredSingleton in _singletons)
			{
				debugOutput.AppendLine($"{registeredSingleton.Key.Name}, {registeredSingleton.Value.Name}");
			}

			debugOutput.AppendLine();
			
			debugOutput.AppendLine($"Singleton instances: {_singletonInstances.Count}");
			
			foreach(var singletonInstance in _singletonInstances)
			{
				debugOutput.AppendLine($"{singletonInstance.Key.Name}, {singletonInstance.Value.GetType().Name}");
			}
			
			debugOutput.AppendLine();
			
			debugOutput.AppendLine($"Transients registered: {_transients.Count}");

			foreach(var registeredTransients in _transients)
			{
				debugOutput.AppendLine($"{registeredTransients.Key.Name}, {registeredTransients.Value.Name}");
			}
			
			return debugOutput.ToString();
		}
	}
}
