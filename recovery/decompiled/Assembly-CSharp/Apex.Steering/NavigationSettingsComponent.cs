using Apex.Services;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

[ExecuteInEditMode]
[AddComponentMenu("Apex/Game World/Navigation Settings", 1037)]
[ApexComponent("Game World")]
public class NavigationSettingsComponent : MonoBehaviour
{
	[Label("Mode", "The way by which height sampling and height navigation is performed.")]
	public HeightSamplingMode heightSampling = HeightSamplingMode.HeightMap;

	[Label("Height Map Detail", "The detail level of the height map, Normal is recommended. High is more accurate but somewhat slower to generate.")]
	public HeightMapDetailLevel heightMapDetail;

	[MinCheck(0.05f, label = "Granularity", tooltip = "The distance between height samples.")]
	public float heightSamplingGranularity = 0.1f;

	[RangeX(0f, 89f, label = "Ledge Threshold", tooltip = "The max angle at which a piece of geometry is considered a ledge. A climb or drop is defined as movement from one ledge to another.")]
	public float ledgeThreshold = 10f;

	[Tooltip("Controls whether units define their own height navigation capabilities or use a global setting.")]
	public bool useGlobalHeightNavigationSettings = true;

	public HeightNavigationCapabilities unitsHeightNavigationCapability = new HeightNavigationCapabilities
	{
		maxClimbHeight = 0.5f,
		maxDropHeight = 1f,
		maxSlopeAngle = 30f
	};

	[Tooltip("Controls whether a clearance map is generated to allow for differently sized units.")]
	public bool useClearance;

	[Tooltip("Represents the maximum height difference between adjacent cells before they will see each other as blocked with regards to clearance.")]
	public float heightDiffThreshold = 0.5f;

	[MinCheck(0.01f, label = "Group Update Interval", tooltip = "The interval used for the load balanced updating of the default transient steering group")]
	public float groupUpdateInterval = 0.1f;

	private void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		GameServices.heightStrategy = new HeightStrategy(heightSampling, heightSamplingGranularity, ledgeThreshold, useGlobalHeightNavigationSettings, unitsHeightNavigationCapability, heightMapDetail);
		GameServices.cellConstruction = new CellConstructionStrategy(GameServices.heightStrategy, useClearance, heightDiffThreshold);
		RefreshPartial();
	}

	private void RefreshPartial()
	{
		GameServices.navigationSettings = new NavigationSettings(groupUpdateInterval);
	}
}
