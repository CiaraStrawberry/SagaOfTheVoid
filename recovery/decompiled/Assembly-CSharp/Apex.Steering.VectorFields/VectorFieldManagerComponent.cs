using Apex.PathFinding;
using Apex.Services;
using Apex.Units;
using UnityEngine;

namespace Apex.Steering.VectorFields;

[AddComponentMenu("Apex/Game World/Vector Field Manager", 1044)]
[ApexComponent("Steering")]
public class VectorFieldManagerComponent : ExtendedMonoBehaviour
{
	public VectorFieldOptions vectorFieldOptions;

	private void Awake()
	{
		GameServices.vectorFieldManager = this;
	}

	public IVectorField CreateVectorField(TransientGroup<IUnitFacade> group, Path path)
	{
		IVectorField vectorField = null;
		vectorField = vectorFieldOptions.vectorFieldType switch
		{
			VectorFieldType.FullGridField => (IVectorField)new FullGridVectorField(group, path, vectorFieldOptions), 
			VectorFieldType.ProgressiveField => new ProgressiveVectorField(group, path, vectorFieldOptions), 
			VectorFieldType.CrossGridField => new CrossGridVectorField(group, path, vectorFieldOptions), 
			_ => new FunnelVectorField(group, path, vectorFieldOptions), 
		};
		if (vectorField == null)
		{
			return null;
		}
		vectorField.Initialize();
		return vectorField;
	}
}
