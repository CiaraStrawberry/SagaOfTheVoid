using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DFNetwork.Networking {
	
	/// <summary>
	/// Network layer.  This class is inherited by the network implementation
	/// </summary>
	public abstract class aNetworkLayer : MonoBehaviour {
		
		#region Must define methods
		
		/// <summary>
		/// Dos the network connect.
		/// </summary>
		public abstract void DoNetworkConnect();
		
		/// <summary>
		/// Dos the network disconnect.
		/// </summary>
		public abstract void DoNetworkDisconnect();
		
		/// <summary>
		/// Determines whether this instance is host.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is host; otherwise, <c>false</c>.
		/// </returns>
		public abstract bool IsHost ();
		
		/// <summary>
		/// Gets the player ID
		/// </summary>
		/// <returns>
		/// The player ID
		/// </returns>
		/// <param name='name'>
		/// Name.
		/// </param>
		public abstract short GetPlayerID (string name);
		
		/// <summary>
		/// Gets the player list.
		/// </summary>
		/// <returns>
		/// The player list.
		/// </returns>
		public abstract short[] GetPlayerList ();
		
		/// <summary>
		/// Gets the name of the player.
		/// </summary>
		/// <returns>
		/// The player name.
		/// </returns>
		/// <param name='ID'>
		/// ID
		/// </param>
		public abstract string GetPlayerName (short ID);
		
		/// <summary>
		/// Gets the current player Id
		/// </summary>
		/// <value>
		/// The current player Id
		/// </value>
		public abstract short GetCurrentPlayerID { get; }
		
		/// <summary>
		/// Generates a packet by the given command ID
		/// </summary>
		/// <returns>
		/// The packet.
		/// </returns>
		/// <param name='CommandID'>
		/// Command ID
		/// </param>
		/// <param name='param'>
		/// Parameter.
		/// </param>
		public abstract PacketHandling.Packet GeneratePacket(short CommandID, params object[] param);
		
		/// <summary>
		/// Generates a packet by the given command name
		/// </summary>
		/// <returns>
		/// The packet.
		/// </returns>
		/// <param name='CommandName'>
		/// Command name.
		/// </param>
		/// <param name='param'>
		/// Parameter.
		/// </param>
		public abstract PacketHandling.Packet GeneratePacket(string CommandName, params object[] param);
		
		public abstract void SendPacket(PacketHandling.Packet packet);
		
		#endregion
		
		#region Events and handlers
		
		/// <summary>
		/// Occurs when on packet sent.
		/// </summary>
		public event System.Action<PacketHandling.Packet> OnPacketSent;
		
		/// <summary>
		/// Occurs when a packet arrives
		/// </summary>
		public event System.Action<PacketHandling.Packet> OnPacketArrived;
		
		/// <summary>
		/// Occurs on network connect.
		/// </summary>
		public event System.Action OnNetworkConnect;
		
		/// <summary>
		/// Raises the on network connect event
		/// </summary>
		public void RaiseOnNetworkConnect() { if (OnNetworkConnect != null) { isConnected = true;  OnNetworkConnect(); } }
		
		/// <summary>
		/// Occurs on network disconnect.
		/// </summary>
		public event System.Action OnNetworkDisconnect;
		
		/// <summary>
		/// Raises the on network disconnect event
		/// </summary>
		public void RaiseOnNetworkDisconnect() { if (OnNetworkDisconnect != null) { isConnected = false; OnNetworkDisconnect(); } }
		
		/// <summary>
		/// Occurs when on player connects
		/// </summary>
		/// <remarks>Sends the players ID</remarks>
		public event System.Action<short> OnPlayerConnected;
		
		/// <summary>
		/// Raises the on player connected event
		/// </summary>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		public void RaiseOnPlayerConnected(short id) { if (OnPlayerConnected != null) { OnPlayerConnected(id); } }
		
		/// <summary>
		/// Occurs when on player disconnected.
		/// </summary>
		/// <remarks>Sends the players ID</remarks>
		public event System.Action<short> OnPlayerDisconnected;

		/// <summary>
		/// Raises the on player disconnected event
		/// </summary>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		public void RaiseOnPlayerDisconnected(short id) { if (OnPlayerDisconnected != null) { OnPlayerDisconnected(id); } }
		
		/// <summary>
		/// Occurs when on room joined.
		/// </summary>
		public event System.Action OnRoomJoined;
		
		/// <summary>
		/// Raises the on room joined event
		/// </summary>
		public void RaiseOnRoomJoined() { if (OnRoomJoined != null) { OnRoomJoined(); } }
		
		/// <summary>
		/// Occurs when on room left.
		/// </summary>
		public event System.Action OnRoomLeft;
		
		/// <summary>
		/// Raises the on room left event
		/// </summary>
		public void RaiseOnRoomLeft() { if (OnRoomLeft != null) { OnRoomLeft(); } }
		
		// Wondering if I should add more events, like force a room type system / lobby type system?
		// Make the users implement that
		
		#endregion
		
		private bool isConnected = false;
		/// <summary>
		/// Gets a value indicating whether this instance is connected.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
		/// </value>
		public bool IsConnected {
			get { return isConnected; }
		}

        /// <summary>
        /// Local instance of the packet handler
        /// </summary>
		public PacketHandling.PacketTypeRegistry PacketRegistry {
            get
            {
                return PacketHandling.PacketTypeRegistry.GetInstance;
            }
        }


        /// <summary>
        /// Awake this instance
        /// </summary>
        public virtual void Awake() {

            // Get the packet handler instance
            PacketRegistry.RefreshCache();

        }

        /// <summary>
        /// Start this instance.
        /// </summary>
        public virtual void Start() {

            // Get the packet handler instance
            PacketRegistry.RefreshCache();

        }

        /// <summary>
        /// Processes the outbound packet.
        /// </summary>
        /// <param name='packet'>
        /// Packet.
        /// </param>
        public byte[] ProcessOutboundPacket(PacketHandling.Packet packet) {
			
			// Let the engine know a packet is being sent
			if (OnPacketSent != null) { OnPacketSent(packet); }
			
			// Get the packet data
			return PacketRegistry.SerializePacket(packet);
			
		}

		/// <summary>
		/// Processes the incomming packet.
		/// </summary>
		/// <param name='data'>
		/// Data.
		/// </param>
		public void ProcessIncommingPacket(byte[] data) {
			
			// Get the packet
			PacketHandling.Packet packet = PacketRegistry.DeserializePacket(data);

			// Let the engine know a new one arrived
			if (OnPacketArrived != null) { OnPacketArrived(packet); }
			
		}
		
	}
}