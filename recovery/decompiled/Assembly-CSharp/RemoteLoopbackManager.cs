using System;
using System.IO;
using UnityEngine;

public class RemoteLoopbackManager : MonoBehaviour
{
	public OvrAvatar LocalAvatar;

	public OvrAvatar LoopbackAvatar;

	private int packetSequence;

	private void Start()
	{
		LocalAvatar.RecordPackets = true;
		OvrAvatar localAvatar = LocalAvatar;
		localAvatar.PacketRecorded = (EventHandler<OvrAvatar.PacketEventArgs>)Delegate.Combine(localAvatar.PacketRecorded, new EventHandler<OvrAvatar.PacketEventArgs>(OnLocalAvatarPacketRecorded));
	}

	private void OnLocalAvatarPacketRecorded(object sender, OvrAvatar.PacketEventArgs args)
	{
		using MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(packetSequence++);
		args.Packet.Write(memoryStream);
		SendPacketData(memoryStream.ToArray());
	}

	private void SendPacketData(byte[] data)
	{
		ReceivePacketData(data);
	}

	private void ReceivePacketData(byte[] data)
	{
		using MemoryStream memoryStream = new MemoryStream(data);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		int sequence = binaryReader.ReadInt32();
		OvrAvatarPacket packet = OvrAvatarPacket.Read(memoryStream);
		LoopbackAvatar.GetComponent<OvrAvatarRemoteDriver>().QueuePacket(sequence, packet);
	}
}
