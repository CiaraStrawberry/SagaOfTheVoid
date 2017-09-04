namespace DFNetwork.Networking.PacketHandling {
	
	/// <summary>
	/// Packet handler interface.  This is used to define a handler for one specific type of packet
	/// </summary>
	public interface iPacketHandler {
		
		/// <summary>
		/// Gets or sets the ID
		/// </summary>
		/// <value>
		/// The ID
		/// </value>
		short ID { get; set; }
			
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		/// <value>
		/// The name of the command.
		/// </value>
		string CommandName { get; }
			
		/// <summary>
		/// Instruct the module to initialize
		/// </summary>
		void Initialize ();
	
		/// <summary>
		/// Implement the editor gui properties
		/// </summary>
		void OnEditorGUI();
		
		/// <summary>
		/// Serialize the specified param.
		/// </summary>
		/// <param name='param'>
		/// Parameter.
		/// </param>
		byte[] Serialize(params object[] param);
		
		/// <summary>
		/// Deserialize the specified data.
		/// </summary>
		/// <param name='data'>
		/// Data.
		/// </param>
		object[] Deserialize(byte[] data);
			
	}
	
}