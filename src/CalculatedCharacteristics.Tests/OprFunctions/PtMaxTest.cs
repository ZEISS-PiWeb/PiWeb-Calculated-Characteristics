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

	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	/// <summary>
	/// Test class for function PT_MAX (maximum of points)
	/// </summary>
	[TestFixture]
	public class PtMaxTest
	{
		#region methods

		private static IEnumerable CreateValidDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" )
				],
				ExpectedResult = 0.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};\"Y\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y" )
				],
				ExpectedResult = 1.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};\"Z\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Z" )
				],
				ExpectedResult = 2.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};\"N\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "N" )
				],
				ExpectedResult = 3.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};\"M\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = 4.2
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};{../Mp3};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X" )
				],
				ExpectedResult = 0.6
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp2};{../Mp3};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "Y" )
				],
				ExpectedResult = 1.6
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp3};{../Mp1};{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "Z" )
				],
				ExpectedResult = 2.6
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};{../Mp3};\"N\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "N" )
				],
				ExpectedResult = 3.6
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp2};{../Mp3};{../Mp1};\"M\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = 4.6
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp6};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp6", false, "X" )
				],
				ExpectedResult = 0.8
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp6};{../Mp7};\"Y\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp7", false, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp6", false, "Y" )
				],
				ExpectedResult = 2.0
			};
		}

		private static IEnumerable CreateMissingMeasurementValueTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp5", true, "X" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};{../Mp5};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 0.4
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp5};{../Mp2};{../Mp1};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 0.4
			};
		}

		private static IEnumerable CreateMissingRequiredMeasurementValueTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp5", true, "X" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};{../Mp5};\"X\";\"true\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp5};{../Mp2};{../Mp1};\"X\";\"true\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateNotExistingDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};\"D\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};\"1\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};\"?\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};\" \")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};\"\t\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp2};{../Mp4};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X" )
				],
				ExpectedResult = 0.4
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp1};{../Mp4};\"D\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MAX({../Mp4};{../Mp1};\"1\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = null
			};
		}

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
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp5", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			], true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp6", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			], false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp7", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			], false ) );
			return characteristics;
		}

		private static Dictionary<PathInformation, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformation, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), 0.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 0.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "X", true ), 0.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "X", false ), 0.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "X", false ), 1.0 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), 1.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Y", true ), 1.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Y", true ), 1.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Y", false ), 1.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "Y", false ), 2.0 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), 2.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), 2.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Z", true ), 2.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Z", false ), 2.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "Z", false ), 3.0 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "N", true ), 3.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "N", true ), 3.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "N", true ), 3.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "N", false ), 3.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "N", false ), 4.0 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "M", true ), 4.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "M", true ), 4.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "M", true ), 4.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "M", false ), 4.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "M", false ), 5.0 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_MAX({../Mp1}{../Mp2};\"X\")" )]
		[TestCase( "PT_MAX({../Mp1};{../Mp2};\'X\')" )]
		[TestCase( "PT_MAX(\'{../Mp1}\';{../Mp2};\"X\")" )]
		[TestCase( "PT_MAX({../Mp1};{../Mp2};\"X\"" )]
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
		[TestCase( "PT_MAX(\"{../Mp1}\";{../Mp2};\"X\")" )]
		[TestCase( "PT_MAX(1.23;{../Mp2};\"X\")" )]
		[TestCase( "PT_MAX({../Mp1};{../Mp2};1.23)" )]
		[TestCase( "PT_MAX({../Mp1};{../Mp2};{../Mp3})" )]
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
			var formula = "PT_MAX(\"X\")";

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
			var formula = "PT_MAX({../Mp1})";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test empty direction
		/// </summary>
		[Test]
		public void TestEmptyDirection_ThrowsArgumentException()
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );
			const string formula = "PT_MAX({../Mp1};\"\")";

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

		/// <summary>
		/// Test missing measurement values on x direction for characteristic Mp5 (not all measurement values are required)
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
		/// Test missing measurement value on x direction for characteristic Mp5 (all measurement values are required)
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
		/// Test non existing direction on Mp1/Mp2
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateNotExistingDirectionTestCases ) )]
		public void TestNonExistingDirection( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test missing characteristic Mp4
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