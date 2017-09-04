using UnityEngine;
using System.Collections;

namespace DFNetwork.Networking.PacketHandling {

	/// <summary>
	/// NetPacket, the network packet that sends data across the internet
	/// </summary>
	public class Packet {
		
		/*
		public enum CommandType {
			Ready = 0,
			FinishedInit = 1,
			Start = 2,
			Pause = 3,
			FinishedTurn = 4,
			Intent = 5,
			Move = 6,
			Attack = 7,
			DesyncCheckRequest = 8,
			CheckForDesync = 9,
			DesyncNotDetected = 10,
			DesyncDetected = 11,
			Message = 12,
			Spawn = 13,
			PingRequest = 14,
			PingReturn = 15,
			NetModify = 16
		} */
		public readonly string CommandName;
		
		public readonly short PacketType;
		
		public readonly short Priority;
		
		public readonly short Owner;
		
		public object[] Params;
		
		public Packet(string commandName, short owner, params object[] param) {
			
			// Get the packet type
			PacketType = PacketTypeRegistry.GetInstance.GetCommandID(commandName);
			
			// set the packet name
			CommandName = commandName;
			
			// Set the owner
			Owner = owner;
			
			// Set the params
			Params = param;
			
		}
		
	}
	
}