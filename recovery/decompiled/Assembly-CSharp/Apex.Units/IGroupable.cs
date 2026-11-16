namespace Apex.Units;

public interface IGroupable<T> where T : IGroupable<T>
{
	TransientGroup<T> transientGroup { get; set; }

	int formationIndex { get; set; }
}
