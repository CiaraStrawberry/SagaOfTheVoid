using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding;

public delegate Path ReplanCallback(GameObject unit, IPositioned currentDestination, Path path);
