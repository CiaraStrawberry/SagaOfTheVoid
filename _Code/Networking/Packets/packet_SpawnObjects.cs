using UnityEngine;
using System.Collections;

/// <summary>
/// packet_SpawnObjects, This module is used to spawn objects in the game
/// </summary>
public class packet_SpawnObjects : UnityEngine.ScriptableObject, DFNetwork.Networking.PacketHandling.iPacketHandler {

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
	public string CommandName { get { return "SpawnObjects"; } }
	
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
		if (param.Length == 0 || !(param[0] is short) || !(param[1] is Vector3) || !(param[2] is Quaternion)) {
			throw new System.InvalidCastException("This module requires a short sent as param[0] and a vector3 sent as param[1] and a quaternion as param[2]");
		}
		
		DFNetwork.Helpers.Serializer serialize = new DFNetwork.Helpers.Serializer();
		
		serialize.AddBits( ((short)param[0]) );
		serialize.AddBits( ((Vector3)param[1]) );
		serialize.AddBits( ((Quaternion)param[2]) );
        serialize.AddBits(((int)param[3]));

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
		
		object[] ret = new object[4];
		
		DFNetwork.Helpers.Deserializer deserialize = new DFNetwork.Helpers.Deserializer(data);
		
		// Get the shorts
		ret[0] = deserialize.ReadNextShort();

		// Get the vector3's
		ret[1] = deserialize.ReadNextVector3();

		// get the quaternions
		ret[2] = deserialize.ReadNextQuaternion();

        // Get the view ID int
        ret[3] = deserialize.ReadNextInt();

		return ret;
		
	}
	
	
}
