using UnityEngine;

namespace EasyEditor;

public class ImageAttribute : PropertyAttribute
{
	public ImageAlignement alignement = ImageAlignement.Center;

	public float size;
}
