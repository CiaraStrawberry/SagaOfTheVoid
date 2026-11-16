using Apex.DataStructures;
using Apex.Services;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Input;

public class InputController
{
	public void SelectUnitRange(Vector3 startScreen, Vector3 endScreen, bool append)
	{
		PolygonXZ boundingPoly = GetBoundingPoly(startScreen, endScreen);
		GameServices.gameStateManager.unitSelection.SelectUnitsIn(boundingPoly, append);
	}

	public void SelectUnitRangeTentative(Vector3 startScreen, Vector3 endScreen, bool append)
	{
		PolygonXZ boundingPoly = GetBoundingPoly(startScreen, endScreen);
		GameServices.gameStateManager.unitSelection.SelectUnitsAsPendingIn(boundingPoly, append);
	}

	public void SelectUnit(Vector3 screenPos, bool append)
	{
		IUnitFacade unitFacade = null;
		Collider colliderAtPosition = UnityServices.mainCamera.GetColliderAtPosition(screenPos, Layers.units);
		if (colliderAtPosition != null)
		{
			unitFacade = colliderAtPosition.GetUnitFacade();
		}
		if (unitFacade != null && unitFacade.isSelectable)
		{
			GameServices.gameStateManager.unitSelection.ToggleSelected(unitFacade, append);
		}
		else if (!append)
		{
			GameServices.gameStateManager.unitSelection.DeselectAll();
		}
	}

	public void SelectUnit(int unitIndex, bool toggle)
	{
		GameServices.gameStateManager.unitSelection.Select(unitIndex, toggle);
	}

	public void SelectGroup(int groupIndex)
	{
		GameServices.gameStateManager.unitSelection.SelectGroup(groupIndex);
	}

	public void AssignGroup(int groupIndex)
	{
		GameServices.gameStateManager.unitSelection.AssignGroup(groupIndex);
	}

	public virtual void SetDestination(Vector3 destination, bool append)
	{
		if (UnityServices.mainCamera.ScreenToLayerHit(destination, Layers.terrain, 1000f, out var hit))
		{
			InvalidDestinationComponent component = hit.collider.GetComponent<InvalidDestinationComponent>();
			if (!(component != null) || (!component.entireTransform && !component.onlySubArea.Contains(hit.point)))
			{
				IGrouping<IUnitFacade> selected = GameServices.gameStateManager.unitSelection.selected;
				selected.MoveTo(hit.point, append);
			}
		}
	}

	private PolygonXZ GetBoundingPoly(Vector3 startScreen, Vector3 endScreen)
	{
		Camera mainCamera = UnityServices.mainCamera;
		Vector3 vector = new Vector3(endScreen.x, startScreen.y);
		Vector3 vector2 = new Vector3(startScreen.x, endScreen.y);
		Vector3 vector3 = startScreen + (endScreen - startScreen) / 2f;
		float? firstTerrainHeight = GetFirstTerrainHeight(vector3, startScreen, endScreen, vector, vector2);
		if (!firstTerrainHeight.HasValue)
		{
			return PolygonXZ.empty;
		}
		Vector3 vector4 = mainCamera.ScreenToGroundPoint(startScreen, firstTerrainHeight.Value);
		Vector3 vector5 = mainCamera.ScreenToGroundPoint(vector, firstTerrainHeight.Value);
		Vector3 vector6 = mainCamera.ScreenToGroundPoint(endScreen, firstTerrainHeight.Value);
		Vector3 vector7 = mainCamera.ScreenToGroundPoint(vector2, firstTerrainHeight.Value);
		return new PolygonXZ(vector4, vector5, vector6, vector7);
	}

	private float? GetFirstTerrainHeight(params Vector3[] pokes)
	{
		for (int i = 0; i < pokes.Length; i++)
		{
			if (UnityServices.mainCamera.ScreenToLayerHit(pokes[i], Layers.terrain, 10000f, out var hit))
			{
				return hit.point.y;
			}
		}
		return null;
	}
}
