using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This might be removed to favor a UI based integration

namespace DFNetwork.Simulation.Modules {

    public class ModuleTypeRegistry {

        internal struct ModuleHandler {

            /// <summary>
            /// The command ID, easy way to determine what the command does
            /// </summary>
            public short ModuleID;

            /// <summary>
            /// The module
            /// </summary>
            public iModule Module;

            /// <summary>
            /// The deserialize method
            /// </summary>
            public System.Func<Networking.PacketHandling.Packet, bool> ProcessCommand;

            /// <summary>
            /// Initializes a new instance of the <see cref="LS_NETSIM.LS_CommandRegistry.PacketHandler"/> struct.
            /// </summary>
            /// <param name='commandID'>
            /// Id assigned to the command
            /// </param>
            /// <param name='serialize'>
            /// Serializer method
            /// </param>
            /// <param name='deserialize'>
            /// Deserializer method
            /// </param>
            public ModuleHandler(iModule module, short moduleID, System.Func<Networking.PacketHandling.Packet, bool> processCommand) {
                Module = module;
                ModuleID = moduleID;
                ProcessCommand = processCommand;
            }

        }

        /// <summary>
        /// Module cache
        /// </summary>
        private Dictionary<string, ModuleHandler> _moduleHandlers = new Dictionary<string, ModuleHandler>();

        /// <summary>
        /// Lock object for thread safety
        /// </summary>
        private static object _LockObject = new object();

        #region Singleton Implementation

        private static ModuleTypeRegistry _instance;

        /// <summary>
        /// Constructor is private
        /// </summary>
        private ModuleTypeRegistry() { }

        /// <summary>
        /// Get instance of the packet handler
        /// </summary>
        public static ModuleTypeRegistry GetInstance
        {
            get
            {
                lock (_LockObject) {
                    if (_instance == null) {
                        _instance = new ModuleTypeRegistry();
                    }
                }

                return _instance;

            }
        }

        #endregion

        /// <summary>
        /// Refreshes the local cache of packet handlers
        /// </summary>
        public void RefreshCache() {


            _moduleHandlers.Clear();

            // Get the module list using reflection, does not replace existing modules
            System.Type[] types = System.Reflection.Assembly.GetAssembly(typeof(Modules.iModule)).GetTypes().Where(m => typeof(Modules.iModule).IsAssignableFrom(m)).ToArray();

            foreach (System.Type t in types) {

                if (t.Name != "iModule") {

                    // Check if the instance exists or not
                    if (_moduleHandlers.Where(x => x.GetType().Name == t.Name).Count() == 0) {


                        // Using unity
                        RegisterModule((Modules.iModule)ScriptableObject.CreateInstance(t));

                        // Using reflection
                        //RegisterModule((Modules.iModule)System.Activator.CreateInstance(t));

                    }
                }
            }

            // Now sort the list alphabetically
            _moduleHandlers = _moduleHandlers.OrderBy(key => key.Key).ToDictionary(x => x.Key, y => y.Value);

        }

        /// <summary>
        /// Registers the command.
        /// </summary>
        /// <param name='name'>
        /// Name of the command, user friendly way to determine what it is
        /// </param>
        /// <param name='priority'>
        /// Priority of the command, lower number is processed first
        /// </param>
        /// <param name='serializer'>
        /// Serializer for the command
        /// </param>
        /// <param name='deserializer'>
        /// Deserializer for the command
        /// </param>
        public void RegisterModule(iModule module) {

            _moduleHandlers.Add(module.ModuleName, new ModuleHandler(module, (short)_moduleHandlers.Count, module.ProcessCommand));

        }

        /// <summary>
        /// Gets the command by ID
        /// </summary>
        /// <returns>
        /// The command by I.
        /// </returns>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        internal ModuleHandler GetModuleByName(string name) {

            if (!_moduleHandlers.ContainsKey(name)) { throw new System.IndexOutOfRangeException("Make sure the module you are trying to access is enabled (" + name + ")"); }

            return _moduleHandlers[name];

        }

        /// <summary>
        /// Gets the ID of the command
        /// </summary>
        /// <returns>
        /// The command I.
        /// </returns>
        /// <param name='name'>
        /// Name.
        /// </param>
        public short GetModuleID(string name) {

            if (!_moduleHandlers.ContainsKey(name)) { throw new System.IndexOutOfRangeException("Make sure the module you are trying to access is enabled (" + name + ")"); }

            return _moduleHandlers[name].ModuleID;

        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <returns>
        /// The command name.
        /// </returns>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        public string GetModuleName(short id) {

            foreach (KeyValuePair<string, ModuleHandler> kp in _moduleHandlers) {

                if (kp.Value.ModuleID == id) {
                    return kp.Key;
                }

            }

            return "";

        }

        /// <summary>
        /// Returns list of module names that are registered
        /// </summary>
        /// <returns></returns>
        public List<string> GetModuleNameList
        {
            get
            {
                return _moduleHandlers.Select(x => x.Key).ToList<string>();
            }
        }

        /// <summary>
        /// Get all modules currently registered
        /// </summary>
        public List<iModule> GetAllModules {

            get
            {
                return _moduleHandlers.Select(x => x.Value.Module).ToList<iModule>();
            }

        }


    }

}