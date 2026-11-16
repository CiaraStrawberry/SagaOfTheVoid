using System.Collections.Generic;
using UnityEngine;

public class OvrAvatarRemoteDriver : OvrAvatarDriver
{
	private bool isStreaming;

	private Queue<OvrAvatarPacket> packetQueue = new Queue<OvrAvatarPacket>();

	private OvrAvatarPacket currentPacket;

	private PoseFrame? currentPose;

	private float currentPacketTime;

	private int currentSequence = -1;

	private const int MinPacketQueue = 1;

	private const int MaxPacketQueue = 4;

	private void Update()
	{
		if (!isStreaming && packetQueue.Count > 1)
		{
			currentPacket = packetQueue.Dequeue();
			isStreaming = true;
		}
		if (!isStreaming)
		{
			return;
		}
		currentPacketTime += Time.deltaTime;
		while (currentPacketTime > currentPacket.Duration)
		{
			if (packetQueue.Count == 0)
			{
				currentPose = currentPacket.FinalFrame;
				currentPacketTime = 0f;
				currentPacket = null;
				isStreaming = false;
				return;
			}
			while (packetQueue.Count > 4)
			{
				packetQueue.Dequeue();
			}
			currentPacketTime -= currentPacket.Duration;
			currentPacket = packetQueue.Dequeue();
		}
		currentPose = currentPacket.GetPoseFrame(currentPacketTime);
	}

	public void QueuePacket(int sequence, OvrAvatarPacket packet)
	{
		if (sequence - currentSequence >= 0)
		{
			currentSequence = sequence;
			packetQueue.Enqueue(packet);
		}
	}

	public override bool GetCurrentPose(out PoseFrame pose)
	{
		if (currentPose.HasValue)
		{
			pose = currentPose.Value;
			return true;
		}
		pose = default;
		return false;
	}
}
