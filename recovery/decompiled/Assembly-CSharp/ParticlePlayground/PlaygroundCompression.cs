using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace ParticlePlayground;

public class PlaygroundCompression
{
	public static byte[] SerializeAndCompress(object obj)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (GZipStream serializationStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(serializationStream, obj);
		}
		return memoryStream.ToArray();
	}

	public static T DecompressAndDeserialize<T>(byte[] data)
	{
		using MemoryStream compressedStream = new MemoryStream(data);
		using GZipStream serializationStream = new GZipStream(compressedStream, CompressionMode.Decompress, leaveOpen: true);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		return (T)binaryFormatter.Deserialize(serializationStream);
	}
}
