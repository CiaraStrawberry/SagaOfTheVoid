using System;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundAxisConstraintsC
{
	public bool x;

	public bool y;

	public bool z;

	public bool HasConstraints()
	{
		return x || y || z;
	}

	public PlaygroundAxisConstraintsC Clone()
	{
		PlaygroundAxisConstraintsC playgroundAxisConstraintsC = new PlaygroundAxisConstraintsC();
		playgroundAxisConstraintsC.x = x;
		playgroundAxisConstraintsC.y = y;
		playgroundAxisConstraintsC.z = z;
		return playgroundAxisConstraintsC;
	}
}
