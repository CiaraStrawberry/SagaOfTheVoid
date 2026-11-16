using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon;

internal class SocketUdp : IPhotonSocket, IDisposable
{
	private Socket sock;

	private readonly object syncer = new object();

	public SocketUdp(PeerBase npeer)
		: base(npeer)
	{
		if (ReportDebugOfLevel(DebugLevel.ALL))
		{
			Listener.DebugReturn(DebugLevel.ALL, "CSharpSocket: UDP, Unity3d.");
		}
		Protocol = ConnectionProtocol.Udp;
		PollReceive = false;
	}

	public void Dispose()
	{
		State = PhotonSocketState.Disconnecting;
		if (sock != null)
		{
			try
			{
				if (sock.Connected)
				{
					sock.Close();
				}
			}
			catch (Exception ex)
			{
				EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + ex);
			}
		}
		sock = null;
		State = PhotonSocketState.Disconnected;
	}

	public override bool Connect()
	{
		lock (syncer)
		{
			if (!base.Connect())
			{
				return false;
			}
			State = PhotonSocketState.Connecting;
			Thread thread = new Thread(DnsAndConnect);
			thread.Name = "photon dns thread";
			thread.IsBackground = true;
			thread.Start();
			return true;
		}
	}

	public override bool Disconnect()
	{
		if (ReportDebugOfLevel(DebugLevel.INFO))
		{
			EnqueueDebugReturn(DebugLevel.INFO, "CSharpSocket.Disconnect()");
		}
		State = PhotonSocketState.Disconnecting;
		lock (syncer)
		{
			if (sock != null)
			{
				try
				{
					sock.Close();
				}
				catch (Exception ex)
				{
					EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + ex);
				}
				sock = null;
			}
		}
		State = PhotonSocketState.Disconnected;
		return true;
	}

	public override PhotonSocketError Send(byte[] data, int length)
	{
		lock (syncer)
		{
			if (sock == null || !sock.Connected)
			{
				return PhotonSocketError.Skipped;
			}
			try
			{
				sock.Send(data, 0, length, SocketFlags.None);
			}
			catch (Exception ex)
			{
				if (ReportDebugOfLevel(DebugLevel.ERROR))
				{
					EnqueueDebugReturn(DebugLevel.ERROR, "Cannot send to: " + ServerAddress + ". " + ex.Message);
				}
				return PhotonSocketError.Exception;
			}
		}
		return PhotonSocketError.Success;
	}

	public override PhotonSocketError Receive(out byte[] data)
	{
		data = null;
		return PhotonSocketError.NoData;
	}

	internal void DnsAndConnect()
	{
		IPAddress iPAddress = null;
		try
		{
			lock (syncer)
			{
				iPAddress = IPhotonSocket.GetIpAddress(ServerAddress);
				if (iPAddress == null)
				{
					throw new ArgumentException("Invalid IPAddress. Address: " + ServerAddress);
				}
				if (iPAddress.AddressFamily != AddressFamily.InterNetwork && iPAddress.AddressFamily != AddressFamily.InterNetworkV6)
				{
					throw new ArgumentException(string.Concat("AddressFamily '", iPAddress.AddressFamily, "' not supported. Address: ", ServerAddress));
				}
				sock = new Socket(iPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
				sock.Connect(iPAddress, ServerPort);
				AddressResolvedAsIpv6 = iPAddress.AddressFamily == AddressFamily.InterNetworkV6;
				State = PhotonSocketState.Connected;
				peerBase.OnConnect();
			}
		}
		catch (SecurityException ex)
		{
			if (ReportDebugOfLevel(DebugLevel.ERROR))
			{
				Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + ServerAddress + "' (" + ((iPAddress != null) ? iPAddress.AddressFamily.ToString() : string.Empty) + ") failed: " + ex.ToString());
			}
			HandleException(StatusCode.SecurityExceptionOnConnect);
			return;
		}
		catch (Exception ex2)
		{
			if (ReportDebugOfLevel(DebugLevel.ERROR))
			{
				Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + ServerAddress + "' (" + ((iPAddress != null) ? iPAddress.AddressFamily.ToString() : string.Empty) + ") failed: " + ex2.ToString());
			}
			HandleException(StatusCode.ExceptionOnConnect);
			return;
		}
		Thread thread = new Thread(ReceiveLoop);
		thread.Name = "photon receive thread";
		thread.IsBackground = true;
		thread.Start();
	}

	public void ReceiveLoop()
	{
		byte[] array = new byte[MTU];
		while (State == PhotonSocketState.Connected)
		{
			try
			{
				int length = sock.Receive(array);
				HandleReceivedDatagram(array, length, willBeReused: true);
			}
			catch (Exception ex)
			{
				if (State != PhotonSocketState.Disconnecting && State != PhotonSocketState.Disconnected)
				{
					if (ReportDebugOfLevel(DebugLevel.ERROR))
					{
						EnqueueDebugReturn(DebugLevel.ERROR, string.Concat("Receive issue. State: ", State, ". Server: '", ServerAddress, "' Exception: ", ex));
					}
					HandleException(StatusCode.ExceptionOnReceive);
				}
			}
		}
		Disconnect();
	}
}
