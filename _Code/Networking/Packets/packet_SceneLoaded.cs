using UnityEngine;
using System.Collections;

/// <summary>
/// Packet_StateVerification, this module is used to do a state hash which is verified on all clients to ensure
/// simulation sync
/// </summary>
public class packet_SceneLoaded : UnityEngine.ScriptableObject, DFNetwork.Networking.PacketHandling.iPacketHandler {

    [SerializeField]
    private short id;
	/// <summary>
	/// Gets or sets the ID
	/// </summary>
	/// <value>
	/// The ID
	/// </value>
	public short ID { get { return id; } set { id = value; } }
	
	/// <summary>
	/// Gets the name of the command.
	/// </summary>
	/// <value>
	/// The name of the command.
	/// </value>
	public string CommandName { get { return "SceneLoaded"; } }
	
	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize ()
	{
	}
	
	/// <summary>
	/// Raises the editor GU event.
	/// </summary>
	public void OnEditorGUI ()
	{
	}
	
	/// <summary>
	/// Serialize the specified commandName and param.
	/// </summary>
	/// <param name='commandName'>
	/// Command name.
	/// </param>
	/// <param name='param'>
	/// Parameter.
	/// </param>
	public byte[] Serialize (params object[] param)
	{
		
		// validate that the packet has an array of shorts, and a vector3
		if (param.Length == 0 || !(param[0] is string)) {
			throw new System.InvalidCastException("This module requires a string sent as param[0]");
		}
		
		DFNetwork.Helpers.Serializer serialize = new DFNetwork.Helpers.Serializer();
		
		serialize.AddBits( ((string)param[0]) );
		
		return serialize.ToArray();
		
	}
	
	/// <summary>
	/// Deserialize the specified commandName and data.
	/// </summary>
	/// <param name='commandName'>
	/// Command name.
	/// </param>
	/// <param name='data'>
	/// Data.
	/// </param>
	public object[] Deserialize (byte[] data)
	{
		
		object[] ret = new object[1];
		
		DFNetwork.Helpers.Deserializer deserialize = new DFNetwork.Helpers.Deserializer(data);
		
		// Get the array of shorts
		ret[0] = deserialize.ReadNextString();
		
		return ret;
		
	}

	
}
