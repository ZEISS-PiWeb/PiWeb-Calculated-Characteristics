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
public class PtLenTest
{
	#region methods

	private static InspectionPlanCollection CreateCharacteristics()
	{
		var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( [
			new CharacteristicInfo( "X" ),
			new CharacteristicInfo( "Y" ),
			new CharacteristicInfo( "Z" ),
			new CharacteristicInfo( "N" )
		] );
		characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", [
			new CharacteristicInfo( "X" ),
			new CharacteristicInfo( "Y" ),
			new CharacteristicInfo( "Z" ),
			new CharacteristicInfo( "N" )
		], true ) );
		characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", [
			new CharacteristicInfo( "X" ),
			new CharacteristicInfo( "Y" ),
			new CharacteristicInfo( "Z" ),
			new CharacteristicInfo( "N" )
		], true ) );
		characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", [
			new CharacteristicInfo( "X" ),
			new CharacteristicInfo( "Y" ),
			new CharacteristicInfo( "Z" ),
			new CharacteristicInfo( "N" )
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
		};
		return values;
	}

	/// <summary>
	/// Test invalid formula
	/// </summary>
	[Test]
	[TestCase( "PT_LEN({../Mp1}{../Mp2};\"X\")" )]
	[TestCase( "PT_LEN({../Mp1};{../Mp2};\'X\')" )]
	[TestCase( "PT_LEN(\'{../Mp1}\';{../Mp2};\"X\")" )]
	[TestCase( "PT_LEN({../Mp1};{../Mp2};\"X\"" )]
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
	[TestCase( "PT_LEN(\"{../Mp1}\";{../Mp2};\"X\")" )]
	[TestCase( "PT_LEN(1.23;{../Mp2};\"X\")" )]
	[TestCase( "PT_LEN({../Mp1};{../Mp2};1.23)" )]
	[TestCase( "PT_LEN({../Mp1};{../Mp2};{../Mp3})" )]
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
		var formula = "PT_LEN(\"X\")";

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
		var formula = "PT_LEN({../Mp1})";

		Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
		Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
	}

	/// <summary>
	/// Test invalid direction arguments
	/// </summary>
	[Test]
	[TestCase( "PT_LEN({../Mp1})" )]
	[TestCase( "PT_LEN({../Mp1};\"N\")" )]
	[TestCase( "PT_LEN({../Mp1};\"E\")" )]
	[TestCase( "PT_LEN({../Mp1};\"XN\")" )]
	[TestCase( "PT_LEN({../Mp1};\"YN\")" )]
	[TestCase( "PT_LEN({../Mp1};\"ZN\")" )]
	[TestCase( "PT_LEN({../Mp1};\"NX\")" )]
	[TestCase( "PT_LEN({../Mp1};\"NY\")" )]
	[TestCase( "PT_LEN({../Mp1};\"NZ\")" )]
	[TestCase( "PT_LEN({../Mp1};\"XYN\")" )]
	[TestCase( "PT_LEN({../Mp1};\"XZN\")" )]
	[TestCase( "PT_LEN({../Mp1};\"YZN\")" )]
	[TestCase( "PT_LEN({../Mp1};\"F\")" )]
	public void TestInvalidDirectionArgument( string formula )
	{
		var characteristics = CreateCharacteristics();
		var values = CreateMeasurementValues();
		var operationTestEnvironment = new OperationTestEnvironment( characteristics );

		Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
		Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
	}

	/// <summary>
	/// Test maximum with single and multiple characteristics for different directions
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
			"PT_LEN({../Mp1};\"X\")",
			0.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};\"Y\")",
			1.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};\"Z\")",
			2.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};{../Mp2};\"X\")",
			0.2,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};{../Mp2};\"Y\")",
			2.6,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "Y" ) )
		{
			Tolerance = 1e-15
		};
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};{../Mp2};\"Z\")",
			4.6,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Z" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};\"XY\")",
			1.216552506059644,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y" ) )
		{
			Tolerance = 1e-15
		};
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};\"YZ\")",
			2.505992817228334,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y", "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};\"ZX\")",
			2.209072203437452,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};{../Mp2};\"XY\")",
			2.607680962081059,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};{../Mp2};\"YZ\")",
			5.283937925449163,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y", "Z" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "Y", "Z" ) )
		{
			Tolerance = 1e-14
		};
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};{../Mp2};\"ZX\")",
			4.604345773288535,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Z" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};\"XYZ\")",
			2.513961017995307,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp1};{../Mp2};\"XYZ\")",
			5.287721626560914,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp3};\"X\")",
			null,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp2},{../Mp3};\"X\")",
			null,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp2},{../Mp3};\"XY\")",
			null,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y" ),
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X", "Y" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp4};\"X\")",
			null );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp2},{../Mp4};\"X\")",
			null,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X" ) );
		yield return new OprFunctionTestCase(
			"PT_LEN({../Mp2},{../Mp4};\"XY\")",
			null,
			new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y" ) );
	}

	#endregion
}