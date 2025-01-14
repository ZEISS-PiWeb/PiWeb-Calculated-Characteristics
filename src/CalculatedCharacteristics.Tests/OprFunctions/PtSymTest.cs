#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2018                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.OprFunctions
{
	#region usings

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	/// <summary>
	/// Test class for function PT_REF (delta between two points)
	/// </summary>
	[TestFixture]
	public class PtSymTest
	{
		#region methods

		private static IEnumerable CreateValidDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X" )
				],
				ExpectedResult = 1.1
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp4};{../Mp1};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X" )
				],
				ExpectedResult = 1.1
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\"Z\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Z" )
				],
				ExpectedResult = 4.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\"N\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "N" )
				],
				ExpectedResult = 2.6
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp6};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "X" )
				],
				ExpectedResult = 0.16
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp6};{../Mp7};\"N\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP7", false, "N" )
				],
				ExpectedResult = 0.415
			};
		}

		private static IEnumerable CreateNotExistingDirectionTestCases()
		{
			var expectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>();

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\"M\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\"?\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\" \")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\"\t\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingMeasurementValueTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Y" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 3.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp4};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 3.0
			};
		}

		private static IEnumerable CreateMissingRequiredMeasurementValueTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Y" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp4};\"Y\";\"true\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp4};{../Mp1};\"Y\";\"true\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			var expectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>();

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp1};{../Mp5};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_SYM({../Mp5};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( [
				new CharacteristicInfo( "X" )
			] );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "N" )
			], true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", [new CharacteristicInfo( "X" )], true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", [new CharacteristicInfo( "X" )], true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp4", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			], true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp6", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "N" )
			], false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp7", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "N" )
			], false ) );
			return characteristics;
		}

		private static Dictionary<PathInformation, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformation, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), 0.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "N", true ), 0.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 1.54 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "X", true ), 5.2689 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "X", true ), 2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "Y", true ), 3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "Z", true ), 4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "N", true ), 5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "X", false ), 0.12 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "N", false ), 0.23 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "X", false ), 0.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "N", false ), 0.6 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_SYM({../Mp1}{../Mp3};\"X\")" )]
		[TestCase( "PT_SYM({../Mp1};{../Mp3};\'X\')" )]
		[TestCase( "PT_SYM(\'{../Mp1}\';{../Mp3};\"X\")" )]
		[TestCase( "PT_SYM({../Mp1};{../Mp3};\"X\"" )]
		public void TestInvalidFormula_ThrowsParseException( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.TypeOf<ParserException>() );
		}

		/// <summary>
		/// Test invalid arguments.
		/// </summary>
		[Test]
		[TestCase( "PT_SYM(\"{../Mp1}\";{../Mp3};\"X\")" )]
		[TestCase( "PT_SYM({../Mp1};{../Mp3};{../Mp2})" )]
		[TestCase( "PT_SYM(1.23;{../Mp3};\"X\")" )]
		[TestCase( "PT_SYM({../Mp1};{../Mp3};1.23)" )]
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
			const string formula = "PT_SYM(\"X\")";

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
			const string formula = "PT_SYM({../Mp1};{../Mp4})";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test existing direction
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateValidDirectionTestCases ) )]
		public void TestExistingDirection( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test non existing direction
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateNotExistingDirectionTestCases ) )]
		public void TestNotExistingDirection( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test empty direction
		/// </summary>
		[Test]
		public void TestEmptyDirection_ThrowsParserException()
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );
			const string formula = "PT_SYM({../Mp4};{../Mp1};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test missing measurement value on Mp1 (not all measurement values are required)
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingMeasurementValueTestCases ) )]
		public void TestMissingMeasurementValue( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test missing measurement value on Mp1 (all measurement values are required)
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingRequiredMeasurementValueTestCases ) )]
		public void TestMissingRequiredMeasurementValue( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test missing characteristic Mp5
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingCharacteristicTestCases ) )]
		public void TestMissingCharacteristic( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		#endregion
	}
}