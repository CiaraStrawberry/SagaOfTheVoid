using UnityEngine;
using EasyEditor;

#if UNITY_EDITOR

using UnityEditor;

#endif

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DFNetwork.Simulation {
	
	/// <summary>
	/// This class will run the simulation of the game
	/// </summary>
    [System.Serializable()]
	public class SimulationLayer : MonoBehaviour {
		
		#region Simulation Events
		
		/// <summary>
		/// The on simulation start.
		/// </summary>
		public System.Action OnSimulationStart;
		
		/// <summary>
		/// The on simulation stop.
		/// </summary>
		public System.Action OnSimulationStop;

        #endregion

        #region Properties

        [Inspector(displayHeader = true, foldable = true, group = "Simulation Settings", groupDescription = "Simulation settings for the system")]
        [SerializeField]
		private bool simdebugging;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LS_Simulator"/> is debugging.
		/// </summary>
		/// <value>
		/// <c>true</c> if debugging; otherwise, <c>false</c>.
		/// </value>
		public bool Debugging { 
			get { return simdebugging; } 
			set { simdebugging = value; } 
		}

        [Inspector(displayHeader = true, foldable = true, group = "Simulation Settings")]
        [SerializeField]
		private bool showLogToConsole;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="LS_Simulator"/> will show log info to console.
		/// </summary>
		/// <value>
		/// <c>true</c> if showing; otherwise, <c>false</c>.
		/// </value>
		public bool ShowLogToConsole { 
			get { return showLogToConsole; } 
			set { showLogToConsole = value; } 
		}

        //[SerializeField]
        //private ResourceItem[] ResourcesArray;

        [HideInInspector]
        [SerializeField]
        private List<ResourceItem> _Resources = new List<ResourceItem>();
        /// <summary>
        /// Cache of resources
        /// </summary>
        /// 
        
        public List<ResourceItem> Resources 
        {
            get { return _Resources; }
            set { _Resources = value; }
        }
        

		/// <summary>
		/// The packet handling graph.
		/// </summary>
		private Dictionary<string, List<string>> PacketHandlingGraph = new Dictionary<string, List<string>>();

		/// <summary>
		/// The net layer.
		/// </summary>
		private DFNetwork.Networking.aNetworkLayer netLayer;

        #endregion

        /// <summary>
        /// Local instance of the module handler
        /// </summary>
		public Modules.ModuleTypeRegistry ModuleRegistry
        {
            get
            {
                return Modules.ModuleTypeRegistry.GetInstance;
            }
        }

        void Awake() {
            ModuleRegistry.RefreshCache();
        }

        void Start() {
			
			// Get the network layer
			if (gameObject.GetComponent<DFNetwork.Networking.aNetworkLayer>() == null) { throw new MissingComponentException("The game object \"" + gameObject.name + "\" needs to have an implementation of aNetworkLayer"); }
			
			netLayer = gameObject.GetComponent<DFNetwork.Networking.aNetworkLayer>();
			
			// Capture the "Packet Arrived" event
			netLayer.OnPacketArrived += PacketArrived;

            // Get the latest modules
            ModuleRegistry.RefreshCache();

            // Register all the modules
            ModuleRegistry.GetAllModules.ForEach( delegate(Modules.iModule n) {
				
				DFNetwork.Debugging.Logging.AddLogItem("Sim Layer", "Registering module handler " + n.ModuleName);
				
				// Instruct the module to initialize
				n.Initialize(netLayer, this);

				// Get the packets that it will handle
				foreach (string s in n.UsesPackets) {
					
					// Check if the packet has a handler
					if (!PacketHandlingGraph.ContainsKey(s)) {
						PacketHandlingGraph.Add(s, new List<string>());
					}
					
					// Add the item to the graph
					DFNetwork.Debugging.Logging.AddLogItem("Sim Layer", "Adding handler for packet: " + s + " and module: " + n.ModuleName);
					PacketHandlingGraph[s].Add(n.ModuleName);
					
				}
				
			});

		}

        /// <summary>
        /// Check if a resource is available in the resource system
        /// </summary>
        /// <param name="prefab">Prefab name</param>
        /// <returns></returns>
        public bool ContainsResource(string prefab) {
            return _Resources.Count(x => x.PrefabName == prefab) > 0;
        }

        /// <summary>
        /// Check if a resource is available in the resource system
        /// </summary>
        /// <param name="prefabID">ID of the prefab</param>
        /// <returns></returns>
        public bool ContainsResource(short prefabID) {
            return _Resources.Count(x => x.PrefabID == prefabID) > 0;
        }

        /// <summary>
        /// Gets the resource ID
        /// </summary>
        /// <returns>The resource I.</returns>
        /// <param name="prefab">Prefab.</param>
        public short GetResourceID(string prefab) {
            
            if (_Resources.Count(x => x.PrefabName == prefab) == 0) {
                Debug.Log(prefab);
                throw new KeyNotFoundException("Prefab does not exist in the list, refresh the list?");
            } else {
                return _Resources.Where(x => x.PrefabName == prefab).First().PrefabID;
            }

        }

        /// <summary>
        /// Gets the resource prefab.
        /// </summary>
        /// <returns>The resource prefab.</returns>
        /// <param name="id">Identifier.</param>
        public string GetResourcePrefab(short id) {

            if (_Resources.Count(x => x.PrefabID == id) == 0) {
                throw new KeyNotFoundException("Prefab does not exist in the list, refresh the list?");
            } else {
                return _Resources.Where(x => x.PrefabID == id).First().PrefabName;
            }

        }

        /// <summary>
        /// Packet has arrived event
        /// </summary>
        /// <param name='packet'>
        /// Packet.
        /// </param>
        private void PacketArrived(DFNetwork.Networking.PacketHandling.Packet packet) {
			
			// Log the packet if debugging is enabled
			if (this.simdebugging) {
				
				DFNetwork.Debugging.Logging.AddLogItem("Sim Layer|Packet Arrived", "Owner: " + packet.Owner + ", command: " + packet.CommandName);
				
			}

            // Find the modules that handles it, if anything handles it
            if (PacketHandlingGraph.ContainsKey(packet.CommandName)) {

                foreach (string s in PacketHandlingGraph[packet.CommandName]) {

                    // Send the packet to the module
                    ModuleRegistry.GetModuleByName(s).ProcessCommand(packet);

                }

            }
			
		}
		
		/// <summary>
		/// Raises the application quit event.
		/// </summary>
		void OnApplicationQuit() {
			
			DFNetwork.Debugging.Logging.AddLogItem("Simulation", "Ending Simulation");
			
		}
		
	}
	
}