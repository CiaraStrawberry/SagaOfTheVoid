using System.IO;
using UnityEngine;

internal static class BinaryreaderExtensions
{
	public static OvrAvatarDriver.PoseFrame ReadPoseFrame(this BinaryReader reader)
	{
		return new OvrAvatarDriver.PoseFrame
		{
			headPosition = reader.ReadVector3(),
			headRotation = reader.ReadQuaternion(),
			handLeftPosition = reader.ReadVector3(),
			handLeftRotation = reader.ReadQuaternion(),
			handRightPosition = reader.ReadVector3(),
			handRightRotation = reader.ReadQuaternion(),
			voiceAmplitude = reader.ReadSingle(),
			controllerLeftPose = reader.ReadControllerPose(),
			controllerRightPose = reader.ReadControllerPose()
		};
	}

	public static Vector2 ReadVector2(this BinaryReader reader)
	{
		return new Vector2
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle()
		};
	}

	public static Vector3 ReadVector3(this BinaryReader reader)
	{
		return new Vector3
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle(),
			z = reader.ReadSingle()
		};
	}

	public static Quaternion ReadQuaternion(this BinaryReader reader)
	{
		return new Quaternion
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle(),
			z = reader.ReadSingle(),
			w = reader.ReadSingle()
		};
	}

	public static OvrAvatarDriver.ControllerPose ReadControllerPose(this BinaryReader reader)
	{
		return new OvrAvatarDriver.ControllerPose
		{
			buttons = (ovrAvatarButton)reader.ReadUInt32(),
			touches = (ovrAvatarTouch)reader.ReadUInt32(),
			joystickPosition = reader.ReadVector2(),
			indexTrigger = reader.ReadSingle(),
			handTrigger = reader.ReadSingle(),
			isActive = reader.ReadBoolean()
		};
	}
}
