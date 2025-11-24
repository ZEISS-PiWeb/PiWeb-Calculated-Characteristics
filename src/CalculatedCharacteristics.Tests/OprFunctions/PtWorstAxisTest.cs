#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2022                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.OprFunctions;

#region usings

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Zeiss.PiWeb.Api.Core;
using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

#endregion

[TestFixture]
public class PtWorstAxisTest
{
	#region methods

	private static InspectionPlanCollection CreateCharacteristics()
	{
		var tol = new Tolerance( -1.0, 2.0 );
		var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( [
			new CharacteristicInfo( "X", null, tol ),
			new CharacteristicInfo( "Y", null, tol ),
			new CharacteristicInfo( "Z", null, tol ),
			new CharacteristicInfo( "N", null, tol )
		] );
		characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", [
			new CharacteristicInfo( "X", null, tol ),
			new CharacteristicInfo( "Y", null, tol ),
			new CharacteristicInfo( "Z", null, tol ),
			new CharacteristicInfo( "N", null, tol )
		], true ) );
		characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", [
			new CharacteristicInfo( "X", 0, tol ),
			new CharacteristicInfo( "Y", 0, tol ),
			new CharacteristicInfo( "Z", 0, tol ),
			new CharacteristicInfo( "N", 0, tol )
		], true ) );
		characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", [
			new CharacteristicInfo( "X", 1, tol ),
			new CharacteristicInfo( "Y", 1, tol ),
			new CharacteristicInfo( "N", 1, tol )
		], true ) );
		return characteristics;
	}

	private static Dictionary<PathInformation, double> CreateMeasurementValues()
	{
		var values = new Dictionary<PathInformation, double>
		{
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), 0.2 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), -1.2 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), 2.2 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "N", true ), 3.2 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "M", true ), 4.2 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 0.4 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Y", true ), 1.4 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), -2.4 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "N", true ), 3.4 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "M", true ), 4.4 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "X", true ), 0.4 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Y", true ), 1.4 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "N", true ), 1.5 },
			{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "M", true ), 1.6 }
		};
		return values;
	}

	/// <summary>
	/// Test invalid formula
	/// </summary>
	[Test]
	[TestCase( "PT_WORST_AXIS({../Mp1};\'X\')" )]
	[TestCase( "PT_WORST_AXIS(\'{../Mp1}\';\"X\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"X\"" )]
	public void TestInvalidFormula_ThrowsParseException( string formula )
	{
		var characteristics = CreateCharacteristics();
		var values = CreateMeasurementValues();
		var operationTestEnvironment = new OperationTestEnvironment( characteristics );

		Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.TypeOf<ParserException>() );
	}

	/// <summary>
	/// Test wrong arguments.
	/// </summary>
	[Test]
	[TestCase( "PT_WORST_AXIS({../Mp1};{../Mp2};\"X\")" )]
	[TestCase( "PT_WORST_AXIS(\"{../Mp1}\";{../Mp2};\"X\")" )]
	[TestCase( "PT_WORST_AXIS(1.23;{../Mp2};\"X\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};{../Mp2};1.23)" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};{../Mp2};{../Mp3})" )]
	public void TestInvalidArguments( string formula )
	{
		var characteristics = CreateCharacteristics();
		var values = CreateMeasurementValues();
		var operationTestEnvironment = new OperationTestEnvironment( characteristics );

		Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
		Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
	}

	/// <summary>
	/// Test missing characteristic argument
	/// </summary>
	[Test]
	public void TestMissingCharacteristicArgument()
	{
		var characteristics = CreateCharacteristics();
		var values = CreateMeasurementValues();
		var operationTestEnvironment = new OperationTestEnvironment( characteristics );
		const string formula = "PT_WORST_AXIS(\"X\")";

		Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
		Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
	}

	/// <summary>
	/// Test missing direction argument
	/// </summary>
	[Test]
	public void TestMissingDirectionArgument()
	{
		var characteristics = CreateCharacteristics();
		var values = CreateMeasurementValues();
		var operationTestEnvironment = new OperationTestEnvironment( characteristics );
		const string formula = "PT_WORST_AXIS({../Mp1})";

		Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
		Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
	}

	/// <summary>
	/// Test invalid direction arguments
	/// </summary>
	[Test]
	[TestCase( "PT_WORST_AXIS({../Mp1})" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"N\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"E\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"XN\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"YN\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"ZN\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"NX\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"NY\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"NZ\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"XYN\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"XZN\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"YZN\")" )]
	[TestCase( "PT_WORST_AXIS({../Mp1};\"F\")" )]
	public void TestInvalidDirectionArgument( string formula )
	{
		var characteristics = CreateCharacteristics();
		var values = CreateMeasurementValues();
		var operationTestEnvironment = new OperationTestEnvironment( characteristics );

		Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
		Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
	}

	/// <summary>
	/// Test for different directions
	/// </summary>
	[Test]
	[TestCaseSource( nameof( CreateValidDirectionTestCases ) )]
	public void TestValidDirection( OprFunctionTestCase testCase )
	{
		var characteristics = CreateCharacteristics();
		var values = CreateMeasurementValues();

		testCase.AssertTestCase( characteristics, values );
	}

	private static IEnumerable CreateValidDirectionTestCases()
	{
		yield return new OprFunctionTestCase(
			"PT_WORST_AXIS({../Mp1};\"X\")",
			0.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ) );
		yield return new OprFunctionTestCase(
			"PT_WORST_AXIS({../Mp1};\"Y\")",
			-1.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y" ) );
		yield return new OprFunctionTestCase(
			"PT_WORST_AXIS({../Mp1};\"Z\")",
			2.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_WORST_AXIS({../Mp1};\"XY\")",
			-1.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y" ) );
		yield return new OprFunctionTestCase(
			"PT_WORST_AXIS({../Mp1};\"XYZ\")",
			2.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_WORST_AXIS({../Mp3};\"XZY\")",
			1.4,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "Y" ) );
	}

	#endregion
}