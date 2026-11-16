namespace Apex.Units;

public interface IUnitGroupingStrategyFactory
{
	IGroupingStrategy<IUnitFacade> CreateStrategy();
}
