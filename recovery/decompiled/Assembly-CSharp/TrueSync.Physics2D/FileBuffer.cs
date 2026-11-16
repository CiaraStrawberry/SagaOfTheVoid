using System.IO;

namespace TrueSync.Physics2D;

internal class FileBuffer
{
	public string Buffer { get; set; }

	public int Position { get; set; }

	private int Length => Buffer.Length;

	public char Next
	{
		get
		{
			char result = Buffer[Position];
			Position++;
			return result;
		}
	}

	public bool EndOfBuffer => Position == Length;

	public FileBuffer(Stream stream)
	{
		using (StreamReader streamReader = new StreamReader(stream))
		{
			Buffer = streamReader.ReadToEnd();
		}
		Position = 0;
	}
}
