using Apex.Steering;

namespace Apex.WorldGeometry;

public sealed class CellConstructionStrategy
{
	public bool calculateHeights { get; private set; }

	public ICellFactory cellFactory { get; private set; }

	public IHeightSettingsProvider heightSettingsProvider { get; private set; }

	public IClearanceProvider clearanceProvider { get; private set; }

	public CellConstructionStrategy(HeightStrategy heightStrategy, bool useClearance, float heightDiffThreshold)
	{
		calculateHeights = heightStrategy.heightMode != HeightSamplingMode.NoHeightSampling;
		if (!calculateHeights)
		{
			if (useClearance)
			{
				cellFactory = FlatClearanceCell.factory;
				clearanceProvider = new FlatCellClearanceProvider();
			}
			else
			{
				cellFactory = FlatCell.factory;
			}
		}
		else if (heightStrategy.useGlobalHeightNavigationSettings)
		{
			if (useClearance)
			{
				cellFactory = StandardClearanceCell.factory;
				clearanceProvider = new StandardCellClearanceProvider(heightDiffThreshold);
			}
			else
			{
				cellFactory = StandardCell.factory;
			}
			heightSettingsProvider = new StandardCellHeightSettingsProvider();
		}
		else
		{
			if (useClearance)
			{
				cellFactory = RichClearanceCell.factory;
				clearanceProvider = new StandardCellClearanceProvider(heightDiffThreshold);
			}
			else
			{
				cellFactory = RichCell.factory;
			}
			heightSettingsProvider = new RichCellHeightSettingsProvider();
		}
	}
}
