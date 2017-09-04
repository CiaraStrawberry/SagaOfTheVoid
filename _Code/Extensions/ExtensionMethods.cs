using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.Steering.Components
{

    public static class ExtensionMethods
    {

        /// <summary>
        /// Get the direction between two point in the xz plane only
        /// </summary>
        /// <param name="from">The from position.</param>
        /// <param name="to">The to position.</param>
        /// <returns>The direction vector between the two points.</returns>
        public static Vector3 DirToXYZ(this Vector3 from, Vector3 to)
        {
            return new Vector3(to.x - from.x, to.y - from.y, to.z - from.z);
        }

    }

}