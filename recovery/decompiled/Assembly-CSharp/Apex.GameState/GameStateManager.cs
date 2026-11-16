using System;
using System.Collections.Generic;
using Apex.Units;
using UnityEngine;

namespace Apex.GameState;

public class GameStateManager
{
	private class UnitEqualityComparer : IEqualityComparer<GameObject>
	{
		public bool Equals(GameObject x, GameObject y)
		{
			return object.ReferenceEquals(x, y);
		}

		public int GetHashCode(GameObject obj)
		{
			return obj.GetHashCode();
		}
	}

	private Dictionary<GameObject, IUnitFacade> _units;

	private Selections _unitSelection;

	private IUnitFacadeFactory _unitFactory;

	public Selections unitSelection => _unitSelection;

	public IEnumerable<IUnitFacade> units => _units.Values;

	public GameStateManager()
		: this(null)
	{
	}

	public GameStateManager(IUnitFacadeFactory unitFactory)
	{
		_unitSelection = new Selections();
		_units = new Dictionary<GameObject, IUnitFacade>(new UnitEqualityComparer());
		_unitFactory = unitFactory;
	}

	public IUnitFacade GetUnitFacade(GameObject unitGameObject, bool createIfMissing = true)
	{
		if (!_units.TryGetValue(unitGameObject, out var value) && createIfMissing)
		{
			if (unitGameObject.Equals(null))
			{
				throw new ArgumentException("Cannot get a unit facade on a destroyed game object!", "unitGameObject");
			}
			value = ((_unitFactory == null) ? new UnitFacade() : _unitFactory.CreateUnitFacade());
			value.Initialize(unitGameObject);
			_units.Add(unitGameObject, value);
		}
		return value;
	}

	public T GetUnitFacade<T>(GameObject unitGameObject, bool createIfMissing = true) where T : class, IUnitFacade, new()
	{
		if (!_units.TryGetValue(unitGameObject, out var value) && createIfMissing)
		{
			if (unitGameObject.Equals(null))
			{
				throw new ArgumentException("Cannot get a unit facade on a destroyed game object!", "unitGameObject");
			}
			value = new T();
			value.Initialize(unitGameObject);
			_units.Add(unitGameObject, value);
		}
		T val = value as T;
		if (val == null)
		{
			val = new T();
			val.Initialize(unitGameObject);
			_units[unitGameObject] = val;
		}
		return val;
	}

	public void RegisterUnit(GameObject unitGameObject)
	{
		IUnitFacade unitFacade = GetUnitFacade(unitGameObject);
		if (unitFacade.isSelectable)
		{
			_unitSelection.RegisterSelectable(unitFacade);
		}
	}

	public void UnregisterUnit(GameObject unitGameObject)
	{
		if (_units.TryGetValue(unitGameObject, out var value))
		{
			if (value.transientGroup != null)
			{
				value.transientGroup.Remove(value);
			}
			if (value.isSelectable)
			{
				_unitSelection.UnregisterSelectable(value);
			}
			value.isAlive = false;
		}
		_units.Remove(unitGameObject);
	}
}
