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
	/// Test class for function PT_REF (delta between two points)
	/// </summary>
	[TestFixture]
	public class PtRefTest
	{
		#region methods

		private static IEnumerable CreateValidDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp1};{../Mp2};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				],
				ExpectedResult = -5.255
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp2};{../Mp1};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				],
				ExpectedResult = 5.255
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp1};{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Z" )
				],
				ExpectedResult = -5.255
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp2};{../Mp1};\"Z\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Z" )
				],
				ExpectedResult = 5.255
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp1};{../Mp2};\"N\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "N" )
				],
				ExpectedResult = -5.255
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp2};{../Mp1};\"N\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "N" )
				],
				ExpectedResult = 5.255
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp1};{../Mp2};\"M\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "M" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "M" )
				],
				ExpectedResult = -2.255
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp2};{../Mp1};\"M\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "M" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "M" )
				],
				ExpectedResult = 2.255
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp1};{../Mp5};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP5", false, "X" )
				],
				ExpectedResult = -6.3
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp5};{../Mp6};\"M\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP5", false, "M" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "M" )
				],
				ExpectedResult = -2.4
			};
		}

		private static IEnumerable CreateMissingMeasurementValueTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp1};{../Mp2};\"Y\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Y" )
				],
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp1};{../Mp3};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X" )
				],
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp3};{../Mp1};\"D\")",
				ExpectedDependentCharacteristics = [],
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_REF({../Mp1};{../Mp4};\"X\")",
				ExpectedDependentCharacteristics =
				[
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" )
				],
				ExpectedResult = null
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" ),
				new CharacteristicInfo( "M" )
			] );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" ),
				new CharacteristicInfo( "M" )
			], true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" ),
				new CharacteristicInfo( "M" )
			], true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" ),
				new CharacteristicInfo( "M" )
			], true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp5", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" ),
				new CharacteristicInfo( "M" )
			], false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp6", [
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" ),
				new CharacteristicInfo( "M" )
			], false ) );
			return characteristics;
		}

		private static Dictionary<PathInformation, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformation, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), -4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), 5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), -3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "N", true ), -2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "M", true ), 1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 1.2547 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), 2.2547 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "N", true ), 3.2547 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "M", true ), 3.2547 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "X", false ), 2.3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Y", false ), 2.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Z", false ), -2.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "N", false ), 2.1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "M", false ), -1.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "X", false ), -1.3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Y", false ), 3.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Z", false ), -5.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "N", false ), 3.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "M", false ), 1.2 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_REF({../Mp1}{../Mp2};\"X\")" )]
		[TestCase( "PT_REF({../Mp1};{../Mp2};\'X\')" )]
		[TestCase( "PT_REF(\'{../Mp1}\';{../Mp2};\"X\")" )]
		[TestCase( "PT_REF({../Mp1};{../Mp2};\"X\"" )]
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
		[TestCase( "PT_REF(\"{../Mp1}\";{../Mp2};\"X\")" )]
		[TestCase( "PT_REF({../Mp1};\"X\";{../Mp2})" )]
		[TestCase( "PT_REF({../Mp1};{../Mp2};{../Mp3})" )]
		[TestCase( "PT_REF(1.23;{../Mp2};\"X\")" )]
		[TestCase( "PT_REF({../Mp1};{../Mp2};1.23)" )]
		public void TestInvalidArguments( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test invalid count of characteristic arguments
		/// </summary>
		[Test]
		[TestCase( "PT_REF(\"X\")" )]
		[TestCase( "PT_REF({../Mp1};\"X\")" )]
		[TestCase( "PT_REF({../Mp1};{../Mp2};{../Mp3};\"X\")" )]
		public void TestInvalidCharacteristicArgumentCount( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

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
			var formula = "PT_REF({../Mp1};{../Mp2})";

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
			const string formula = "PT_REF({../Mp1};{../Mp2};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test multiple directions
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
		/// Test missing measurement value onMp2/Mp3 and non existing direction D on Mp1/Mp3
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