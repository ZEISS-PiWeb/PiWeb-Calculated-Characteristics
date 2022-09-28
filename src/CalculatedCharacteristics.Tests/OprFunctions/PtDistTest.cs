#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
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
	using Zeiss.PiWeb.Api.Contracts;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	/// <summary>
	/// Test class for function PT_DIST (distance of two points)
	/// </summary>
	[TestFixture]
	public class PtDistTest
	{
		#region methods

		private static IEnumerable CreateValidDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp2};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				},
				ExpectedResult = 5.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp2};{../Mp1};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				},
				ExpectedResult = 5.0
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp2};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Y" )
				},
				ExpectedResult = 3.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp2};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Y" )
				},
				ExpectedResult = 3.0
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Z" )
				},
				ExpectedResult = -9.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp2};{../Mp1};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Z" )
				},
				ExpectedResult = -9.0
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp2};\"E\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X", "Y", "Z" )
				},
				ExpectedResult = -2.543
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp2};{../Mp1};\"E\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X", "Y", "Z" )
				},
				ExpectedResult = -2.543
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp5};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP5", false, "Z" )
				},
				ExpectedResult = 8.00
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp5};{../Mp6};\"E\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP5", false, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "X", "Y", "Z" )
				},
				ExpectedResult = 1.199
			};
		}

		private static IEnumerable CreateMissingMeasurementValueTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp4};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X" )
				},
				ExpectedResult = null
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp4};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Y" )
				},
				ExpectedResult = null
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp4};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Z" )
				},
				ExpectedResult = null
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp4};{../Mp1};\"E\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X", "Y", "Z" )
				},
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingNominalValuesTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp3};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X" )
				},
				ExpectedResult = null
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp3};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "Y" )
				},
				ExpectedResult = null
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp1};{../Mp3};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "Z" )
				},
				ExpectedResult = null
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST({../Mp3};{../Mp1};\"E\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X", "Y", "Z" )
				},
				ExpectedResult = null
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( new[] { new CharacteristicInfo( "X" ), new CharacteristicInfo( "Y" ), new CharacteristicInfo( "Z" ), new CharacteristicInfo( "E" ) } );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", new[] { new CharacteristicInfo( "X", 300.563 ), new CharacteristicInfo( "Y", -465 ), new CharacteristicInfo( "Z", 475.83 ) }, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", new[] { new CharacteristicInfo( "X", 304.653 ), new CharacteristicInfo( "Y", -493 ), new CharacteristicInfo( "Z", 504.937 ) }, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", new[] { new CharacteristicInfo( "X" ), new CharacteristicInfo( "Y" ), new CharacteristicInfo( "Z" ) }, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp4", new[] { new CharacteristicInfo( "X" ), new CharacteristicInfo( "Y" ), new CharacteristicInfo( "Z" ) }, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp5", new[] { new CharacteristicInfo( "X", 345.653 ), new CharacteristicInfo( "Y", 43 ), new CharacteristicInfo( "Z", 404.937 ) }, false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp6", new[] { new CharacteristicInfo( "X", 632.653 ), new CharacteristicInfo( "Y", -293 ), new CharacteristicInfo( "Z", -204.937 ) }, false ) );
			return characteristics;
		}

		private static Dictionary<PathInformation, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformation, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), -4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), 5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), 6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Y", true ), 2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), -3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "X", true ), 1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Y", true ), 2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Z", true ), -3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "X", false ), -1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Y", false ), 3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Z", false ), -2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "X", false ), 4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Y", false ), 1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Z", false ), 0 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_DIST({../Mp1}{../Mp2};\"X\")" )]
		[TestCase( "PT_DIST({../Mp1};{../Mp2};\'X\')" )]
		[TestCase( "PT_DIST(\'{../Mp1}\';{../Mp2};\"X\")" )]
		[TestCase( "PT_DIST({../Mp1};{../Mp2};\"X\"" )]
		public void TestEmptyDirection_ThrowsParserException( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.TypeOf<ParserException>() );
		}

		/// <summary>
		/// Test wrong argument types.
		/// </summary>
		[Test]
		[TestCase( "PT_DIST(\"{../Mp1}\";{../Mp2};\"X\")" )]
		[TestCase( "PT_DIST({../Mp1};\"{../Mp2}\";\"X\")" )]
		[TestCase( "PT_DIST({../Mp1};\"X\";{../Mp2})" )]
		[TestCase( "PT_DIST(\"X\";{../Mp1};{../Mp2})" )]
		[TestCase( "PT_DIST({../Mp1};{../Mp2};12.3)" )]
		[TestCase( "PT_DIST(1.23;{../Mp2};\"X\")" )]
		public void TestInvalidArguments( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test wrong characteristic argument count
		/// </summary>
		[Test]
		[TestCase( "PT_DIST(\"X\")" )]
		[TestCase( "PT_DIST({../Mp1};\"X\")" )]
		[TestCase( "PT_DIST({../Mp1};{../Mp2};{../Mp3};\"X\")" )]
		public void TestWrongCharacteristicArgumentCount( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test invalid direction argument
		/// </summary>
		[Test]
		[TestCase( "PT_DIST({../Mp1};{../Mp2})" )]
		[TestCase( "PT_DIST({../Mp1};{../Mp2};\"F\")" )]
		public void TestInvalidDirectionArgument( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

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
			const string formula = "PT_DIST({../Mp1};{../Mp2};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test euclid distance
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
		/// Test missing measurement values euclid distance on Mp4
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingMeasurementValueTestCases ) )]
		public void TestMissingMeasurementValueEuclidDistance( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test missing nominal values
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingNominalValuesTestCases ) )]
		public void TestMissingNominalValues( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		#endregion
	}
}