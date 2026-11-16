using System;
using Apex.Common;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class GridPortal
{
	private string _name;

	private PortalType _type;

	private PortalDirection _direction;

	private IGrid _gridOne;

	private IGrid _gridTwo;

	private PortalCell _cellOne;

	private PortalCell _cellTwo;

	private Bounds _portalOneBounds;

	private Bounds _portalTwoBounds;

	private AttributeMask _exclusiveTo;

	private bool _enabled;

	public string name => _name;

	public PortalType type => _type;

	public PortalDirection direction => _direction;

	public IGrid gridOne => _gridOne;

	public IGrid gridTwo => _gridTwo;

	public AttributeMask exclusiveTo
	{
		get
		{
			return _exclusiveTo;
		}
		set
		{
			_exclusiveTo = value;
		}
	}

	public bool enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			if (_enabled != value)
			{
				_enabled = value;
				if (_enabled)
				{
					_cellOne.Activate();
					_cellTwo.Activate();
					_gridOne.TouchSections(_portalOneBounds);
					_gridTwo.TouchSections(_portalTwoBounds);
				}
				else
				{
					_cellOne.Deactivate();
					_cellTwo.Deactivate();
					_gridOne.TouchSections(_portalOneBounds);
					_gridTwo.TouchSections(_portalTwoBounds);
				}
			}
		}
	}

	private GridPortal()
	{
	}

	public static GridPortal Create(string name, PortalType type, Bounds portalOne, Bounds portalTwo, AttributeMask exclusiveTo, IPortalAction action)
	{
		return Create(name, type, PortalDirection.Twoway, portalOne, portalTwo, exclusiveTo, action);
	}

	public static GridPortal Create(string name, PortalType type, PortalDirection direction, Bounds portalOne, Bounds portalTwo, AttributeMask exclusiveTo, IPortalAction action)
	{
		GridPortal gridPortal = new GridPortal();
		gridPortal.Initialize(name, type, direction, portalOne, portalTwo, exclusiveTo, action);
		return gridPortal;
	}

	internal bool IsUsableBy(AttributeMask requesterAttributes)
	{
		if (_exclusiveTo == AttributeMask.None)
		{
			return true;
		}
		return ((int)_exclusiveTo & (int)requesterAttributes) > 0;
	}

	internal IGrid GetGridFor(PortalCell c)
	{
		if (c == _cellOne)
		{
			return _gridOne;
		}
		return _gridTwo;
	}

	private void Initialize(string name, PortalType type, PortalDirection direction, Bounds portalOne, Bounds portalTwo, AttributeMask exclusiveTo, IPortalAction action)
	{
		_type = type;
		_direction = direction;
		_exclusiveTo = exclusiveTo;
		_portalOneBounds = portalOne;
		_portalTwoBounds = portalTwo;
		_cellOne = new PortalCell(this, oneWay: false, action);
		_cellTwo = new PortalCell(this, _direction == PortalDirection.Oneway, action);
		_gridOne = _cellOne.Initialize(_cellTwo, portalOne);
		_gridTwo = _cellTwo.Initialize(_cellOne, portalTwo);
		if (_gridOne == null || _gridTwo == null)
		{
			throw new ArgumentException("A grid portal has been placed with one or more of its portals outside a grid. Portals must be on a grid.");
		}
		_name = GridManager.instance.RegisterPortal(name, this);
	}
}
