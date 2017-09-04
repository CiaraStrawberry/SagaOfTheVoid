using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace DFNetwork.Simulation.Resources {

    /// <summary>
    /// This class is used to track prefabs that contain classes we consider loadable at runtime
    /// </summary>
    public class ResourceList {

        public static List<ResourceItem> GetAllResources {

            get
            {

                List<ResourceItem> CachedResources = new List<ResourceItem>();

#if UNITY_EDITOR

                string[] tmp = AssetDatabase.GetAllAssetPaths();

                int cnt = 0;

                for (short a = 0; a < tmp.Length; a++) {

                    if (tmp[a].EndsWith(".prefab")) {

                        GameObject go = (GameObject)AssetDatabase.LoadMainAssetAtPath(tmp[a]);

                        // need to check if the object uses the "aObject" class
                        if (go.GetComponent<_Moveable>() != null
                            || go.GetComponent<_Ship>() != null
                            || go.GetComponent<_Station>() != null
                            || go.GetComponent<_Stationary>() != null
                            || go.GetComponent<_Weapon>() != null) {

                            string t = tmp[a].Split('/')[tmp[a].Split('/').Length - 1].Replace(".prefab", "");

                            CachedResources.Add(new ResourceItem() {
                                PrefabID = (short)cnt,
                                PrefabName = t
                            });

                            cnt++;

                        }

                    }
                }

#endif

                return CachedResources;

            }

        }

    }

}
