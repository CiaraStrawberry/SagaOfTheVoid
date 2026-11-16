using Apex.GameState;
using Apex.PathFinding;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Services;

[AddComponentMenu("Apex/Game World/Game Services Initializer", 1032)]
[ApexComponent("Game World")]
public class GameServicesInitializerComponent : SingleInstanceComponent<GameServicesInitializerComponent>
{
	protected virtual void InitializeServices()
	{
		IMessageBusFactory messageBusFactory = this.As<IMessageBusFactory>();
		if (messageBusFactory == null)
		{
			GameServices.messageBus = new BasicMessageBus();
		}
		else
		{
			GameServices.messageBus = messageBusFactory.CreateMessageBus();
		}
		GameServices.gameStateManager = new GameStateManager(this.As<IUnitFacadeFactory>());
		IUnitGroupingStrategyFactory unitGroupingStrategyFactory = this.As<IUnitGroupingStrategyFactory>();
		if (unitGroupingStrategyFactory != null)
		{
			GroupingManager.RegisterGroupingStrategy(unitGroupingStrategyFactory.CreateStrategy());
		}
		else
		{
			GroupingManager.RegisterGroupingStrategy(new DefaultUnitGroupingStrategy());
		}
		ICellCostStrategyFactory cellCostStrategyFactory = this.As<ICellCostStrategyFactory>();
		if (cellCostStrategyFactory == null)
		{
			GameServices.cellCostStrategy = new DefaultCellCostStrategy();
		}
		else
		{
			GameServices.cellCostStrategy = cellCostStrategyFactory.CreateCostStrategy();
		}
	}

	protected virtual void InitializeAutoComponents()
	{
		TerrainHeightMap[] array = Object.FindObjectsOfType<TerrainHeightMap>();
		if (array.Length == 0)
		{
			Terrain[] array2 = Object.FindObjectsOfType<Terrain>();
			Terrain[] array3 = array2;
			foreach (Terrain terrain in array3)
			{
				TerrainHeightMap terrainHeightMap = gameObject.AddComponent<TerrainHeightMap>();
				terrainHeightMap.terrain = terrain;
			}
		}
	}

	protected sealed override void OnAwake()
	{
		InitializeServices();
		InitializeAutoComponents();
	}
}
