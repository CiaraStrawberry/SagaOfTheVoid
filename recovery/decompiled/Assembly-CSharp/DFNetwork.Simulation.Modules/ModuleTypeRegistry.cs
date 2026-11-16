using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DFNetwork.Networking.PacketHandling;
using UnityEngine;

namespace DFNetwork.Simulation.Modules;

public class ModuleTypeRegistry
{
	internal struct ModuleHandler(iModule module, short moduleID, Func<Packet, bool> processCommand)
	{
		public short ModuleID = moduleID;

		public iModule Module = module;

		public Func<Packet, bool> ProcessCommand = processCommand;
	}

	private Dictionary<string, ModuleHandler> _moduleHandlers = new Dictionary<string, ModuleHandler>();

	private static object _LockObject = new object();

	private static ModuleTypeRegistry _instance;

	public static ModuleTypeRegistry GetInstance
	{
		get
		{
			lock (_LockObject)
			{
				if (_instance == null)
				{
					_instance = new ModuleTypeRegistry();
				}
			}
			return _instance;
		}
	}

	public List<string> GetModuleNameList => _moduleHandlers.Select((KeyValuePair<string, ModuleHandler> x) => x.Key).ToList();

	public List<iModule> GetAllModules => _moduleHandlers.Select((KeyValuePair<string, ModuleHandler> x) => x.Value.Module).ToList();

	private ModuleTypeRegistry()
	{
	}

	public void RefreshCache()
	{
		_moduleHandlers.Clear();
		Type[] array = (from m in Assembly.GetAssembly(typeof(iModule)).GetTypes()
			where typeof(iModule).IsAssignableFrom(m)
			select m).ToArray();
		Type[] array2 = array;
		foreach (Type t in array2)
		{
			if (t.Name != "iModule" && _moduleHandlers.Where((KeyValuePair<string, ModuleHandler> x) => x.GetType().Name == t.Name).Count() == 0)
			{
				RegisterModule((iModule)ScriptableObject.CreateInstance(t));
			}
		}
		_moduleHandlers = _moduleHandlers.OrderBy((KeyValuePair<string, ModuleHandler> key) => key.Key).ToDictionary((KeyValuePair<string, ModuleHandler> x) => x.Key, (KeyValuePair<string, ModuleHandler> y) => y.Value);
	}

	public void RegisterModule(iModule module)
	{
		_moduleHandlers.Add(module.ModuleName, new ModuleHandler(module, (short)_moduleHandlers.Count, module.ProcessCommand));
	}

	internal ModuleHandler GetModuleByName(string name)
	{
		if (!_moduleHandlers.ContainsKey(name))
		{
			throw new IndexOutOfRangeException("Make sure the module you are trying to access is enabled (" + name + ")");
		}
		return _moduleHandlers[name];
	}

	public short GetModuleID(string name)
	{
		if (!_moduleHandlers.ContainsKey(name))
		{
			throw new IndexOutOfRangeException("Make sure the module you are trying to access is enabled (" + name + ")");
		}
		return _moduleHandlers[name].ModuleID;
	}

	public string GetModuleName(short id)
	{
		foreach (KeyValuePair<string, ModuleHandler> moduleHandler in _moduleHandlers)
		{
			if (moduleHandler.Value.ModuleID == id)
			{
				return moduleHandler.Key;
			}
		}
		return string.Empty;
	}
}
