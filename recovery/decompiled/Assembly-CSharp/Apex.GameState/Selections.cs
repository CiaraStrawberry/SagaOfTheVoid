using System;
using System.Collections.Generic;
using System.Linq;
using Apex.DataStructures;
using Apex.Services;
using Apex.Steering.Messages;
using Apex.Units;

namespace Apex.GameState;

public sealed class Selections
{
	private class EmptyGrouping : IGrouping<IUnitFacade>
	{
		public int groupCount => 0;

		public int memberCount => 0;

		public TransientGroup<IUnitFacade> this[int index] => null;

		public void Add(IUnitFacade member)
		{
			throw new InvalidOperationException("Cannot add members to this grouping, it is read-only.");
		}

		public void Remove(IUnitFacade member)
		{
			throw new InvalidOperationException("Cannot remove members from this grouping, it is read-only.");
		}
	}

	private static readonly IGrouping<IUnitFacade> _emptyGroup = new EmptyGrouping();

	private IGrouping<IUnitFacade> _selected;

	private IndexableSet<IUnitFacade> _selectableUnits;

	private IDictionary<int, IGrouping<IUnitFacade>> _groups;

	private List<int> _removalBuffer = new List<int>(2);

	public IGrouping<IUnitFacade> selected => _selected;

	public IIterable<IUnitFacade> selectableUnits => _selectableUnits;

	public Selections()
	{
		_selected = _emptyGroup;
		_selectableUnits = new IndexableSet<IUnitFacade>();
		_groups = new Dictionary<int, IGrouping<IUnitFacade>>();
	}

	public void RegisterSelectable(IUnitFacade unit)
	{
		_selectableUnits.Add(unit);
	}

	public void UnregisterSelectable(IUnitFacade unit)
	{
		unit.isSelected = false;
		_selectableUnits.Remove(unit);
		if (_groups.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<int, IGrouping<IUnitFacade>> group in _groups)
		{
			group.Value.Remove(unit);
			if (group.Value.memberCount == 0)
			{
				_removalBuffer.Add(group.Key);
			}
		}
		for (int i = 0; i < _removalBuffer.Count; i++)
		{
			_groups.Remove(_removalBuffer[i]);
		}
		_removalBuffer.Clear();
	}

	public void SelectUnitsAsPendingIn(PolygonXZ bounds, bool append)
	{
		int count = _selectableUnits.count;
		for (int i = 0; i < count; i++)
		{
			IUnitFacade unitFacade = _selectableUnits[i];
			bool pending = (append && unitFacade.isSelected) || bounds.Contains(unitFacade.position);
			unitFacade.MarkSelectPending(pending);
		}
	}

	public void SelectUnitsIn(PolygonXZ bounds, bool append)
	{
		IEnumerable<IUnitFacade> units = _selectableUnits.Where((IUnitFacade u) => bounds.Contains(u.position));
		Select(append, units);
	}

	public void Select(bool append, params IUnitFacade[] units)
	{
		Select(append, (IEnumerable<IUnitFacade>)units);
	}

	public void Select(bool append, IEnumerable<IUnitFacade> units)
	{
		IEnumerable<IUnitFacade> enumerable;
		if (_selected.memberCount == 0)
		{
			enumerable = units;
			_selected = GroupingManager.CreateGrouping(units);
		}
		else if (append)
		{
			enumerable = units.Except(_selected.All()).ToArray();
			_selected.Add(enumerable);
		}
		else
		{
			IEnumerable<IUnitFacade> source = _selected.All().Except(units);
			if (source.Any())
			{
				DeselectAll();
				enumerable = units;
				_selected = GroupingManager.CreateGrouping(units);
			}
			else
			{
				enumerable = units.Except(_selected.All()).ToArray();
				_selected.Add(enumerable);
			}
		}
		enumerable.Apply((IUnitFacade u) =>
		{
			u.isSelected = true;
		});
		PostUnitsSelectedMessage(_selected);
	}

	public IUnitFacade Select(int unitIndex, bool toogle)
	{
		if (unitIndex >= _selectableUnits.count || unitIndex < 0)
		{
			return null;
		}
		IUnitFacade unitFacade = _selectableUnits[unitIndex];
		if (toogle)
		{
			DeselectAll();
			ToggleSelected(unitFacade, append: true);
		}
		else
		{
			Select(false, unitFacade);
		}
		return unitFacade;
	}

	public void ToggleSelected(IUnitFacade unit, bool append)
	{
		if (!append || _selected == _emptyGroup)
		{
			DeselectAll();
			unit.isSelected = true;
			_selected = GroupingManager.CreateGrouping<IUnitFacade>(unit);
			return;
		}
		unit.isSelected = !unit.isSelected;
		if (unit.isSelected)
		{
			_selected.Add(unit);
		}
		else
		{
			_selected.Remove(unit);
		}
		PostUnitsSelectedMessage(_selected);
	}

	private void PostUnitsSelectedMessage(IGrouping<IUnitFacade> units)
	{
		GameServices.messageBus.Post(new UnitsSelectedMessage(units));
	}

	public void DeselectAll()
	{
		_selected.All().Apply((IUnitFacade u) =>
		{
			u.isSelected = false;
		});
		_selected = _emptyGroup;
	}

	public void AssignGroup(int groupIndex)
	{
		if (_groups.TryGetValue(groupIndex, out var value))
		{
			for (int i = 0; i < value.groupCount; i++)
			{
				value[i].keepAlive = false;
			}
		}
		_groups[groupIndex] = _selected;
		for (int j = 0; j < _selected.groupCount; j++)
		{
			_selected[j].keepAlive = true;
		}
	}

	public void SelectGroup(int groupIndex)
	{
		if (_groups.TryGetValue(groupIndex, out var value))
		{
			_selected.All().Apply((IUnitFacade u) =>
			{
				u.isSelected = false;
			});
			_selected = value;
			_selected.All().Apply((IUnitFacade u) =>
			{
				u.isSelected = true;
			});
			PostUnitsSelectedMessage(_selected);
		}
	}
}
