using UnityEngine;
using System.Collections;

/// <summary>
/// Packet_unpause, this module is used to send unpause packets to players
/// </summary>
public class packet_Unpause : UnityEngine.ScriptableObject, DFNetwork.Networking.PacketHandling.iPacketHandler {

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
	public string CommandName { get { return "Unpause"; } }
	
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
		
		// Unpause doesn't have any data
		return new byte[0];
		
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
		
		// Unpause doesn't have any data
		return new object[0];
		
	}
	
}
