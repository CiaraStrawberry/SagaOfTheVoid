using System;

namespace Apex.WorldGeometry;

[Serializable]
[Flags]
public enum NeighbourPosition
{
	None = 0,
	BottomLeft = 1,
	Bottom = 2,
	BottomRight = 4,
	Left = 8,
	Self = 0x10,
	Right = 0x20,
	TopLeft = 0x40,
	Top = 0x80,
	TopRight = 0x100
}
