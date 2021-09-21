#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2018                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics.Tests.OprFunctions
{
	#region usings

	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Shared.CalculatedCharacteristics.Tests.Misc;

	#endregion

	/// <summary>
	/// Test class for function PT_DIST_PT_3PT (shortest distance point to plane)
	/// </summary>
	[TestFixture]
	public class PtDistPt3PtTest
	{
		#region methods

		private static IEnumerable CreateMissingMeasurementValueTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp6", true, "X", "Y", "Z" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp6};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp2};{../Mp3};{../Mp6};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp3};{../Mp6};{../Mp1};{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp6};{../Mp1};{../Mp2};{../Mp3};\"E\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X", "Y", "Z" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp7};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp2};{../Mp3};{../Mp7};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp3};{../Mp7};{../Mp1};{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp7};{../Mp1};{../Mp2};{../Mp3};\"E\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingNominalValueTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp5", true, "X", "Y", "Z" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp5};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp2};{../Mp3};{../Mp5};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp3};{../Mp5};{../Mp1};{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp5};{../Mp1};{../Mp2};{../Mp3};\"E\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateValidDirectionTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp4", true, "X", "Y", "Z" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 83.134
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = -12630258.786
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\"Z\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = -339.917
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\"E\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 66.069
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp8};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp8", false, "X", "Y", "Z" )
				},
				ExpectedResult = 34.035
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_3PT({../Mp8};{../Mp9};{../Mp10};{../Mp11};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp8", false, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp9", false, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp10", false, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp11", false, "X", "Y", "Z" )
				},
				ExpectedResult = 2.276
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( new[] { new CharacteristicInfo( "X" ) } );

			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", new[]
			{
				new CharacteristicInfo( "X", 130.901 ),
				new CharacteristicInfo( "Y", -229 ),
				new CharacteristicInfo( "Z", 347.468 )
			}, true ) );

			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", new[]
			{
				new CharacteristicInfo( "X", 254.918 ),
				new CharacteristicInfo( "Y", -308 ),
				new CharacteristicInfo( "Z", 491.52 )
			}, true ) );

			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", new[]
			{
				new CharacteristicInfo( "X", 254.546 ),
				new CharacteristicInfo( "Y", -248 ),
				new CharacteristicInfo( "Z", 488.87 )
			}, true ) );

			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp4", new[]
			{
				new CharacteristicInfo( "X", 253.699 ),
				new CharacteristicInfo( "Y", -278 ),
				new CharacteristicInfo( "Z", 482.846 )
			}, true ) );

			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp5", new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp6", new[]
			{
				new CharacteristicInfo( "X", 253.699 ),
				new CharacteristicInfo( "Y", -278 ),
				new CharacteristicInfo( "Z", 482.846 )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp8", new[]
			{
				new CharacteristicInfo( "X", 32.32 ),
				new CharacteristicInfo( "Y", 312.3 ),
				new CharacteristicInfo( "Z", 31.5 )
			}, false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp9", new[]
			{
				new CharacteristicInfo( "X", 445.2 ),
				new CharacteristicInfo( "Y", -21.3 ),
				new CharacteristicInfo( "Z", -43.2 )
			}, false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp10", new[]
			{
				new CharacteristicInfo( "X", 342.25 ),
				new CharacteristicInfo( "Y", -123.2 ),
				new CharacteristicInfo( "Z", 12.3 )
			}, false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp11", new[]
			{
				new CharacteristicInfo( "X", -31.32 ),
				new CharacteristicInfo( "Y", 23.123 ),
				new CharacteristicInfo( "Z", 52.2 )
			}, false ) );
			return characteristics;
		}

		private static Dictionary<PathInformationDto, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformationDto, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), 1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), -2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), 3 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 7 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Y", true ), 8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), -9 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "X", true ), -2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Y", true ), 3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Z", true ), 5 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "X", true ), 4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "Y", true ), -5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "Z", true ), 6 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "X", true ), 4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Y", true ), -5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Z", true ), 6 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp8", "X", false ), -1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp8", "Y", false ), 1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp8", "Z", false ), -3 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp9", "X", false ), 2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp9", "Y", false ), -3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp9", "Z", false ), 4 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp10", "X", false ), 2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp10", "Y", false ), 3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp10", "Z", false ), -2 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp11", "X", false ), -4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp11", "Y", false ), -5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp11", "Z", false ), -3 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_DIST_PT_3PT({../Mp1}{../Mp2};{../Mp3};{../Mp4};\"X\")" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\'X\')" )]
		[TestCase( "PT_DIST_PT_3PT(\'{../Mp1}\';{../Mp2};{../Mp3};{../Mp4};\"X\")" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\"X\"" )]
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
		[TestCase( "PT_DIST_PT_3PT(\"{../Mp1}\";{../Mp2};{../Mp3};{../Mp4};\"X\")" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};1.23;{../Mp3};{../Mp4};\"X\")" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};1.23)" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};\"X\";{../Mp3};{../Mp4};)" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};{../Mp5})" )]
		public void TestInvalidArguments( string formula )
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
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4})" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\"L\")" )]
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
			const string formula = "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test wrong count of characteristic arguments
		/// </summary>
		[Test]
		[TestCase( "PT_DIST_PT_3PT(\"X\")" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};\"X\")" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};\"X\")" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};\"X\")" )]
		[TestCase( "PT_DIST_PT_3PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};{../Mp5};\"X\")" )]
		public void TestWrongCharacteristicArgumentCount( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test missing measurement value on Mp6
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
		/// Test missing characteristic Mp7
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingCharacteristicTestCases ) )]
		public void TestMissingCharacteristic( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test missing nominal value on Mp5
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingNominalValueTestCases ) )]
		public void TestMissingNominalValue( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test directions
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateValidDirectionTestCases ) )]
		public void TestValidDirection( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		#endregion
	}
}