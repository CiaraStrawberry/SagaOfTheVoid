using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DFNetwork.Networking.PacketHandling {

    public class PacketTypeRegistry {

        internal struct PacketHandler {

            /// <summary>
            /// The command ID, easy way to determine what the command does
            /// </summary>
            public short CommandID;

            /// <summary>
            /// Type of the object
            /// </summary>
            public Type ObjectType;

            /// <summary>
            /// Instance of the object
            /// </summary>
            public PacketHandling.iPacketHandler Instance;

            /// <summary>
            /// The serialize method
            /// </summary>
            public System.Func<object[], byte[]> Serialize;

            /// <summary>
            /// The deserialize method
            /// </summary>
            public System.Func<byte[], object[]> Deserialize;

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
            public PacketHandler(short commandID, Type objectType, PacketHandling.iPacketHandler instance, System.Func<object[], byte[]> serialize, System.Func<byte[], object[]> deserialize) {
                CommandID = commandID;
                ObjectType = objectType;
                Instance = instance;
                Serialize = serialize;
                Deserialize = deserialize;
            }

        }

        /// <summary>
        /// Command Cache
        /// </summary>
        private Dictionary<string, PacketHandler> _packetHandlers = new Dictionary<string, PacketHandler>();

        /// <summary>
        /// Lock object for thread safety
        /// </summary>
        private static object _LockObject = new object();

        #region Singleton Implementation

        private static PacketTypeRegistry _instance;

        /// <summary>
        /// Constructor is private
        /// </summary>
        private PacketTypeRegistry() {}

        /// <summary>
        /// Get instance of the packet handler
        /// </summary>
        public static PacketTypeRegistry GetInstance
        {
            get
            {
                lock (_LockObject) {
                    if (_instance == null) {
                        _instance = new PacketTypeRegistry();
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

            // Clear the cache
            _packetHandlers.Clear();

            // Get the module list using reflection, does not replace existing modules
            System.Type[] types = System.Reflection.Assembly.GetAssembly(typeof(PacketHandling.iPacketHandler)).GetTypes().Where(m => typeof(PacketHandling.iPacketHandler).IsAssignableFrom(m)).ToArray();

            // Register packets
            foreach (System.Type t in types) {

                if (t.Name != "iPacketHandler") {

                    // Check if the instance exists or not
                    if (_packetHandlers.Where(x => x.Key == t.Name).Count() == 0) {

                        // Using unity
                        RegisterCommand((PacketHandling.iPacketHandler)ScriptableObject.CreateInstance(t));
                        
                        // Using reflection
                        //RegisterCommand(t.Name, (PacketHandling.iPacketHandler)System.Activator.CreateInstance(t));

                    }
                }
            }

            // Now sort the list alphabetically
            _packetHandlers = _packetHandlers.OrderBy(key => key.Key).ToDictionary(x => x.Key, y => y.Value);

        }

        /// <summary>
        /// Register a packet
        /// </summary>
        /// <param name="packet"></param>
        public void RegisterCommand(PacketHandling.iPacketHandler packet) {

            // Create Handler
            PacketHandler ph = new PacketHandler((short)_packetHandlers.Count, packet.GetType(), packet, packet.Serialize, packet.Deserialize);

            // Register
            _packetHandlers.Add(packet.CommandName, ph);


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
        internal PacketHandler GetCommandByName(string name) {

            if (!_packetHandlers.ContainsKey(name)) { throw new System.IndexOutOfRangeException("Make sure the module you are trying to access is enabled (" + name + ")"); }

            return _packetHandlers[name];

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
        public short GetCommandID(string name) {

            if (!_packetHandlers.ContainsKey(name)) { throw new System.IndexOutOfRangeException("Make sure the module you are trying to access is enabled (" + name + ")"); }

            return _packetHandlers[name].CommandID;

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
        public string GetCommandName(short id) {

            foreach (KeyValuePair<string, PacketHandler> kp in _packetHandlers) {

                if (kp.Value.CommandID == id) {
                    return kp.Key;
                }

            }

            return "";

        }

        /// <summary>
        /// Check if the command exists
        /// </summary>
        /// <returns>
        /// The exists.
        /// </returns>
        /// <param name='id'>
        /// If set to <c>true</c> identifier.
        /// </param>
        public bool CommandExists(short id) {

            bool found = false;

            foreach (KeyValuePair<string, PacketHandler> kp in _packetHandlers) {

                if (kp.Value.CommandID == id) {
                    found = true;
                }

            }

            return found;

        }

        /// <summary>
        /// Check if the command exists
        /// </summary>
        /// <returns>
        /// The exists.
        /// </returns>
        /// <param name='name'>
        /// If set to <c>true</c> name.
        /// </param>
        public bool CommandExists(string name) {

            return _packetHandlers.ContainsKey(name);

        }

        /// <summary>
        /// Returns list of commands that are registered
        /// </summary>
        /// <returns></returns>
        public List<string> GetCommandNameList {
            get
            {
                return _packetHandlers.Select(x => x.Key).ToList<string>();
            }
        }

        /// <summary>
        /// Serializes the packet using the appropriate serializer
        /// </summary>
        /// <returns>
        /// The packet.
        /// </returns>
        /// <param name='name'>
        /// Name.
        /// </param>
        public byte[] SerializePacket(PacketHandling.Packet cmd) {

            // Check if the command exists
            if (!CommandExists(cmd.CommandName)) { throw new MissingReferenceException("Packet type \"" + cmd.CommandName + "\" does not exist, please make sure the module is enabled"); }

            // Add the header to the packet
            Helpers.Serializer bs = new Helpers.Serializer();
            bs.Serialize();

            // Command Type
            bs.AddBits(cmd.PacketType);

            // Add owner
            bs.AddBits((short)cmd.Owner);

            byte[] header = bs.ToArray();
            byte[] data = _packetHandlers[cmd.CommandName].Serialize(cmd.Params);
            byte[] full = new byte[header.Length + data.Length];

            header.CopyTo(full, 0);
            data.CopyTo(full, header.Length);

            return full;

        }

        /// <summary>
        /// Deserializes the packet using the appropriate deserializer
        /// </summary>
        /// <returns>
        /// The packet.
        /// </returns>
        /// <param name='name'>
        /// Name.
        /// </param>
        /// <param name='data'>
        /// Data.
        /// </param>
        public Packet DeserializePacket(byte[] data) {

            Helpers.Deserializer bs = new Helpers.Deserializer(data);

            short id = bs.ReadNextShort();

            // Check if the command exists
            if (!CommandExists(id)) { throw new MissingReferenceException("Packet type \"" + id + "\" does not exist, please make sure the module is enabled"); }

            string name = GetCommandName(id);

            short owner = bs.ReadNextShort();

            // Pass the rest of the data to the correct command
            byte[] param = new byte[data.Length - bs.index];
            param = data.Skip(bs.index).Take(data.Length - bs.index).ToArray();
            object[] par = _packetHandlers[name].Deserialize(param);

            return new Packet(name, owner, par);

        }
    } // End PacketTypeRegistry
} // End namespace