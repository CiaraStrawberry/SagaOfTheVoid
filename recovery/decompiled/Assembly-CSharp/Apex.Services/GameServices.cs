using Apex.GameState;
using Apex.PathFinding;
using Apex.Steering;
using Apex.Steering.VectorFields;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Services;

public static class GameServices
{
	private static IMessageBus _messageBus;

	private static IPathService _pathService;

	private static HeightStrategy _heightStrategy;

	private static VectorFieldManagerComponent _vectorFieldManager;

	private static NavigationSettings _navigationSettings;

	public static GameStateManager gameStateManager { get; set; }

	public static ICellCostStrategy cellCostStrategy { get; internal set; }

	public static CellConstructionStrategy cellConstruction { get; internal set; }

	public static IMessageBus messageBus
	{
		get
		{
			if (_messageBus == null)
			{
				throw new MissingComponentException("No message bus has been initialized, please ensure that you have a Game Services Initializer component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.");
			}
			return _messageBus;
		}
		set
		{
			_messageBus = value;
		}
	}

	public static IPathService pathService
	{
		get
		{
			if (_pathService == null)
			{
				throw new MissingComponentException("No Path Service has been initialized, please ensure that you have a Path Service component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.");
			}
			return _pathService;
		}
		set
		{
			_pathService = value;
		}
	}

	public static HeightStrategy heightStrategy
	{
		get
		{
			if (_heightStrategy == null && Application.isPlaying)
			{
				throw new MissingComponentException("No Height Strategy has been initialized, please ensure that you have a Navigation Settings component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.");
			}
			return _heightStrategy;
		}
		set
		{
			_heightStrategy = value;
		}
	}

	public static VectorFieldManagerComponent vectorFieldManager
	{
		get
		{
			if (_vectorFieldManager == null)
			{
				throw new MissingComponentException("No VectorFieldManager Service has been initialized, please ensure that you have a VectorFieldManager Service component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.");
			}
			return _vectorFieldManager;
		}
		set
		{
			_vectorFieldManager = value;
		}
	}

	public static NavigationSettings navigationSettings
	{
		get
		{
			if (_navigationSettings == null)
			{
				throw new MissingComponentException("No NavigationSettings Service has been initialized, please ensure that you have a NavigationSettings Service component in the game world.\nThis may also be caused by script files having been recompiled while the scene is running, if so restart the scene.");
			}
			return _navigationSettings;
		}
		set
		{
			_navigationSettings = value;
		}
	}
}
