namespace Apex.Services;

public interface ILayerMask
{
	int value { get; set; }

	string LayerToName(int layer);

	int NameToLayer(string layerName);

	int GetMask(string[] layerNames);
}
