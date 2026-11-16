using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples.Tests;

public class VRTK_ControllerReference_Setup : VRTK_BaseTest
{
	public uint overrideActualIndex = uint.MaxValue;

	public GameObject overrideActualController;

	public GameObject overrideAliasController;

	public GameObject overrideModelController;

	public SDK_BaseController.ControllerHand actualControllerHand = SDK_BaseController.ControllerHand.Right;

	protected VRTK_ControllerReference testReference;

	protected uint actualIndex;

	protected GameObject actualController;

	protected GameObject aliasController;

	protected GameObject modelController;

	protected override void SetUp(string message)
	{
		base.SetUp(message);
		actualController = ((!(overrideActualController == null)) ? overrideActualController : VRTK_SDK_Bridge.GetControllerByHand(actualControllerHand, actual: true));
		aliasController = ((!(overrideAliasController == null)) ? overrideAliasController : VRTK_SDK_Bridge.GetControllerByHand(actualControllerHand, actual: false));
		modelController = ((!(overrideModelController == null)) ? overrideModelController : VRTK_SDK_Bridge.GetControllerModel(actualController));
		actualIndex = ((overrideActualIndex != uint.MaxValue) ? overrideActualIndex : VRTK_SDK_Bridge.GetControllerIndex(actualController));
		testReference = null;
	}

	protected override void TearDown()
	{
		base.TearDown();
		testReference = null;
		actualController = null;
		aliasController = null;
		modelController = null;
		actualIndex = uint.MaxValue;
	}

	protected override void Test()
	{
		BeginTest("VRTK_ControllerReference");
		TestNullState();
		TestStaticIndex();
		TestStaticActual();
		TestStaticAlias();
		TestStaticModel();
		TestStaticHand();
		TestStaticIsValid();
		TestStaticGetRealIndex();
	}

	protected virtual List<SDK_BaseController.ControllerHand> GetOtherHands(SDK_BaseController.ControllerHand ignore)
	{
		List<SDK_BaseController.ControllerHand> list = new List<SDK_BaseController.ControllerHand>();
		if (ignore != SDK_BaseController.ControllerHand.Left)
		{
			list.Add(SDK_BaseController.ControllerHand.Left);
		}
		if (ignore != SDK_BaseController.ControllerHand.Right)
		{
			list.Add(SDK_BaseController.ControllerHand.Right);
		}
		if (ignore != SDK_BaseController.ControllerHand.None)
		{
			list.Add(SDK_BaseController.ControllerHand.None);
		}
		return list;
	}

	protected virtual void TestNullState()
	{
		SetUp("Null State");
		Assert("testReference == null", testReference == null, "reference should be null by default", string.Empty);
		TearDown();
	}

	protected virtual void TestStaticIndex()
	{
		SetUp("Static Index");
		testReference = VRTK_ControllerReference.GetControllerReference(actualIndex);
		Assert("testReference != null", testReference != null, "reference should not be null", testReference.ToString());
		Assert("testReference.index == actualIndex", testReference.index == actualIndex, "The reference actual [" + testReference.index + "] does not match the actual [" + actualIndex + "]", string.Empty);
		Assert("testReference.actual == actualController", testReference.actual == actualController, string.Concat("The reference actual [", testReference.actual, "] does not match the actual [", actualController, "]"), string.Empty);
		Assert("testReference.scriptAlias == aliasController", testReference.scriptAlias == aliasController, string.Concat("The reference script alias [", testReference.scriptAlias, "] does not match the actual [", aliasController, "]"), string.Empty);
		Assert("testReference.model == modelController", testReference.model == modelController, string.Concat("The reference model [", testReference.model, "] does not match the model [", modelController, "]"), string.Empty);
		Assert("testReference.hand == actualControllerHand", testReference.hand == actualControllerHand, string.Concat("The reference hand [", testReference.hand, "] does not match the hand [", actualControllerHand, "]"), string.Empty);
		foreach (SDK_BaseController.ControllerHand otherHand in GetOtherHands(actualControllerHand))
		{
			Assert("testReference.hand == " + otherHand, testReference.hand != otherHand, string.Concat("The reference hand [", testReference.hand, "] should not match the hand [", otherHand, "]"), string.Empty);
		}
		TearDown();
	}

	protected virtual void TestStaticActual()
	{
		SetUp("Static Actual");
		testReference = VRTK_ControllerReference.GetControllerReference(actualController);
		Assert("testReference != null", testReference != null, "reference should not be null", testReference.ToString());
		Assert("testReference.index == actualIndex", testReference.index == actualIndex, "The reference actual [" + testReference.index + "] does not match the actual [" + actualIndex + "]", string.Empty);
		Assert("testReference.actual == actualController", testReference.actual == actualController, string.Concat("The reference actual [", testReference.actual, "] does not match the actual [", actualController, "]"), string.Empty);
		Assert("testReference.scriptAlias == aliasController", testReference.scriptAlias == aliasController, string.Concat("The reference script alias [", testReference.scriptAlias, "] does not match the actual [", aliasController, "]"), string.Empty);
		Assert("testReference.model == modelController", testReference.model == modelController, string.Concat("The reference model [", testReference.model, "] does not match the model [", modelController, "]"), string.Empty);
		Assert("testReference.hand == actualControllerHand", testReference.hand == actualControllerHand, string.Concat("The reference hand [", testReference.hand, "] does not match the hand [", actualControllerHand, "]"), string.Empty);
		foreach (SDK_BaseController.ControllerHand otherHand in GetOtherHands(actualControllerHand))
		{
			Assert("testReference.hand == " + otherHand, testReference.hand != otherHand, string.Concat("The reference hand [", testReference.hand, "] should not match the hand [", otherHand, "]"), string.Empty);
		}
		TearDown();
	}

	protected virtual void TestStaticAlias()
	{
		SetUp("Static Alias");
		testReference = VRTK_ControllerReference.GetControllerReference(aliasController);
		Assert("testReference != null", testReference != null, "reference should not be null", testReference.ToString());
		Assert("testReference.index == actualIndex", testReference.index == actualIndex, "The reference actual [" + testReference.index + "] does not match the actual [" + actualIndex + "]", string.Empty);
		Assert("testReference.actual == actualController", testReference.actual == actualController, string.Concat("The reference actual [", testReference.actual, "] does not match the actual [", actualController, "]"), string.Empty);
		Assert("testReference.scriptAlias == aliasController", testReference.scriptAlias == aliasController, string.Concat("The reference script alias [", testReference.scriptAlias, "] does not match the actual [", aliasController, "]"), string.Empty);
		Assert("testReference.model == modelController", testReference.model == modelController, string.Concat("The reference model [", testReference.model, "] does not match the model [", modelController, "]"), string.Empty);
		Assert("testReference.hand == actualControllerHand", testReference.hand == actualControllerHand, string.Concat("The reference hand [", testReference.hand, "] does not match the hand [", actualControllerHand, "]"), string.Empty);
		foreach (SDK_BaseController.ControllerHand otherHand in GetOtherHands(actualControllerHand))
		{
			Assert("testReference.hand == " + otherHand, testReference.hand != otherHand, string.Concat("The reference hand [", testReference.hand, "] should not match the hand [", otherHand, "]"), string.Empty);
		}
		TearDown();
	}

	protected virtual void TestStaticModel()
	{
		SetUp("Static Model");
		testReference = VRTK_ControllerReference.GetControllerReference(modelController);
		Assert("testReference != null", testReference != null, "reference should not be null", testReference.ToString());
		Assert("testReference.index == actualIndex", testReference.index == actualIndex, "The reference actual [" + testReference.index + "] does not match the actual [" + actualIndex + "]", string.Empty);
		Assert("testReference.actual == actualController", testReference.actual == actualController, string.Concat("The reference actual [", testReference.actual, "] does not match the actual [", actualController, "]"), string.Empty);
		Assert("testReference.scriptAlias == aliasController", testReference.scriptAlias == aliasController, string.Concat("The reference script alias [", testReference.scriptAlias, "] does not match the actual [", aliasController, "]"), string.Empty);
		Assert("testReference.model == modelController", testReference.model == modelController, string.Concat("The reference model [", testReference.model, "] does not match the model [", modelController, "]"), string.Empty);
		Assert("testReference.hand == actualControllerHand", testReference.hand == actualControllerHand, string.Concat("The reference hand [", testReference.hand, "] does not match the hand [", actualControllerHand, "]"), string.Empty);
		foreach (SDK_BaseController.ControllerHand otherHand in GetOtherHands(actualControllerHand))
		{
			Assert("testReference.hand == " + otherHand, testReference.hand != otherHand, string.Concat("The reference hand [", testReference.hand, "] should not match the hand [", otherHand, "]"), string.Empty);
		}
		TearDown();
	}

	protected virtual void TestStaticHand()
	{
		SetUp("Static Hand");
		testReference = VRTK_ControllerReference.GetControllerReference(actualControllerHand);
		Assert("testReference != null", testReference != null, "reference should not be null", testReference.ToString());
		Assert("testReference.index == actualIndex", testReference.index == actualIndex, "The reference actual [" + testReference.index + "] does not match the actual [" + actualIndex + "]", string.Empty);
		Assert("testReference.actual == actualController", testReference.actual == actualController, string.Concat("The reference actual [", testReference.actual, "] does not match the actual [", actualController, "]"), string.Empty);
		Assert("testReference.scriptAlias == aliasController", testReference.scriptAlias == aliasController, string.Concat("The reference script alias [", testReference.scriptAlias, "] does not match the actual [", aliasController, "]"), string.Empty);
		Assert("testReference.model == modelController", testReference.model == modelController, string.Concat("The reference model [", testReference.model, "] does not match the model [", modelController, "]"), string.Empty);
		Assert("testReference.hand == actualControllerHand", testReference.hand == actualControllerHand, string.Concat("The reference hand [", testReference.hand, "] does not match the hand [", actualControllerHand, "]"), string.Empty);
		foreach (SDK_BaseController.ControllerHand otherHand in GetOtherHands(actualControllerHand))
		{
			Assert("testReference.hand == " + otherHand, testReference.hand != otherHand, string.Concat("The reference hand [", testReference.hand, "] should not match the hand [", otherHand, "]"), string.Empty);
		}
		TearDown();
	}

	protected virtual void TestStaticIsValid()
	{
		SetUp("Static IsValid");
		Assert("!VRTK_ControllerReference.IsValid(null)", !VRTK_ControllerReference.IsValid(null), "null reference should not be valid", string.Empty);
		Assert("!VRTK_ControllerReference.IsValid(VRTK_ControllerReference.GetControllerReference(999))", !VRTK_ControllerReference.IsValid(VRTK_ControllerReference.GetControllerReference(999u)), "invalid reference should not be valid", string.Empty);
		Assert("!VRTK_ControllerReference.IsValid(VRTK_ControllerReference.GetControllerReference(new GameObject()))", !VRTK_ControllerReference.IsValid(VRTK_ControllerReference.GetControllerReference(new GameObject())), "invalid reference should not be valid", string.Empty);
		Assert("VRTK_ControllerReference.IsValid(VRTK_ControllerReference.GetControllerReference(actualIndex))", VRTK_ControllerReference.IsValid(VRTK_ControllerReference.GetControllerReference(actualIndex)), "valid reference should be valid", string.Empty);
		TearDown();
	}

	protected virtual void TestStaticGetRealIndex()
	{
		SetUp("Static GetRealIndex");
		Assert("VRTK_ControllerReference.GetRealIndex(null) == uint.MaxValue", VRTK_ControllerReference.GetRealIndex(null) == uint.MaxValue, "null reference should have index of uint.MaxValue", string.Empty);
		Assert("VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(999)) == uint.MaxValue", VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(999u)) == uint.MaxValue, "invalid reference should have index of uint.MaxValue", string.Empty);
		Assert("VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(actualIndex)) == actualIndex", VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(actualIndex)) == actualIndex, "valid reference should have index of actualIndex", string.Empty);
		Assert("VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(actualController)) == actualIndex", VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(actualController)) == actualIndex, "valid reference should have index of actualIndex", string.Empty);
		Assert("VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(aliasController)) == actualIndex", VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(aliasController)) == actualIndex, "valid reference should have index of actualIndex", string.Empty);
		Assert("VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(modelController)) == actualIndex", VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(modelController)) == actualIndex, "valid reference should have index of actualIndex", string.Empty);
		Assert("VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(actualControllerHand)) == actualIndex", VRTK_ControllerReference.GetRealIndex(VRTK_ControllerReference.GetControllerReference(actualControllerHand)) == actualIndex, "valid reference should have index of actualIndex", string.Empty);
		TearDown();
	}
}
