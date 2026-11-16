using System;

namespace Oculus.Platform;

public static class PlatformInternal
{
	public enum MessageTypeInternal : uint
	{
		Application_ExecuteCoordinatedLaunch = 645772532u,
		Application_GetInstalledApplications = 1376744524u,
		Avatar_UpdateMetaData = 2077219214u,
		GraphAPI_Get = 822018158u,
		GraphAPI_Post = 1990567876u,
		HTTP_Get = 1874211363u,
		HTTP_GetToFile = 1317133401u,
		HTTP_Post = 1798743375u,
		Livestreaming_IsAllowedForApplication = 191729014u,
		Livestreaming_StartStream = 1343932350u,
		Livestreaming_StopStream = 1155796426u,
		Livestreaming_UpdateMicStatus = 475495815u,
		Party_Create = 450042703u,
		Party_GatherInApplication = 1921499523u,
		Party_Get = 1586058173u,
		Party_GetCurrentForUser = 1489764138u,
		Party_Invite = 901104867u,
		Party_Join = 1744993395u,
		Party_Leave = 848430801u,
		Room_CreateOrUpdateAndJoinNamed = 2089683601u,
		Room_GetNamedRooms = 125660812u,
		Room_GetSocialRooms = 1636310390u,
		SystemPermissions_GetStatus = 493497353u,
		SystemPermissions_LaunchDeeplink = 442139697u,
		User_NewEntitledTestUser = 292822787u,
		User_NewTestUser = 921194380u,
		User_NewTestUserFriends = 517416647u
	}

	public static class HTTP
	{
		public static void SetHttpTransferUpdateCallback(Message<Oculus.Platform.Models.HttpTransferUpdate>.Callback callback)
		{
			Callback.SetNotificationCallback(Message.MessageType.Notification_HTTP_Transfer, callback);
		}
	}

	public static void CrashApplication()
	{
		CAPI.ovr_CrashApplication();
	}

	internal static Message ParseMessageHandle(IntPtr messageHandle, Message.MessageType messageType)
	{
		Message result = null;
		switch ((MessageTypeInternal)messageType)
		{
		case MessageTypeInternal.Livestreaming_UpdateMicStatus:
		case MessageTypeInternal.Application_ExecuteCoordinatedLaunch:
		case MessageTypeInternal.Party_Leave:
			result = new Message(messageHandle);
			break;
		case MessageTypeInternal.Application_GetInstalledApplications:
			result = new MessageWithInstalledApplicationList(messageHandle);
			break;
		case MessageTypeInternal.Livestreaming_IsAllowedForApplication:
			result = new MessageWithLivestreamingApplicationStatus(messageHandle);
			break;
		case MessageTypeInternal.Livestreaming_StartStream:
			result = new MessageWithLivestreamingStartResult(messageHandle);
			break;
		case MessageTypeInternal.Livestreaming_StopStream:
			result = new MessageWithLivestreamingVideoStats(messageHandle);
			break;
		case MessageTypeInternal.Party_Get:
			result = new MessageWithParty(messageHandle);
			break;
		case MessageTypeInternal.Party_GetCurrentForUser:
			result = new MessageWithPartyUnderCurrentParty(messageHandle);
			break;
		case MessageTypeInternal.Party_Create:
		case MessageTypeInternal.Party_Invite:
		case MessageTypeInternal.Party_Join:
		case MessageTypeInternal.Party_GatherInApplication:
			result = new MessageWithPartyID(messageHandle);
			break;
		case MessageTypeInternal.Room_CreateOrUpdateAndJoinNamed:
			result = new MessageWithRoomUnderViewerRoom(messageHandle);
			break;
		case MessageTypeInternal.Room_GetNamedRooms:
		case MessageTypeInternal.Room_GetSocialRooms:
			result = new MessageWithRoomList(messageHandle);
			break;
		case MessageTypeInternal.User_NewEntitledTestUser:
		case MessageTypeInternal.User_NewTestUserFriends:
		case MessageTypeInternal.GraphAPI_Get:
		case MessageTypeInternal.User_NewTestUser:
		case MessageTypeInternal.HTTP_GetToFile:
		case MessageTypeInternal.HTTP_Post:
		case MessageTypeInternal.HTTP_Get:
		case MessageTypeInternal.GraphAPI_Post:
		case MessageTypeInternal.Avatar_UpdateMetaData:
			result = new MessageWithString(messageHandle);
			break;
		case MessageTypeInternal.SystemPermissions_LaunchDeeplink:
		case MessageTypeInternal.SystemPermissions_GetStatus:
			result = new MessageWithSystemPermission(messageHandle);
			break;
		}
		return result;
	}
}
