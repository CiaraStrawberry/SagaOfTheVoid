using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class OvrAvatarComponent : MonoBehaviour
{
	public List<OvrAvatarRenderComponent> RenderParts = new List<OvrAvatarRenderComponent>();
}
public struct ovrAvatarComponent
{
	public ovrAvatarTransform transform;

	public uint renderPartCount;

	public IntPtr renderParts;

	[MarshalAs(UnmanagedType.LPStr)]
	public string name;
}
