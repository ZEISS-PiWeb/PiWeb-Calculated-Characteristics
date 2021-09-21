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
	/// Test class for function PT_DIST_PT_2PT (shortest distance point to line)
	/// </summary>
	[TestFixture]
	public class PtDistPt2PtTest
	{
		#region methods

		private static IEnumerable CreateMissingMeasurementValueTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp5", true, "X", "Y", "Z" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp5};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp5};{../Mp2};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp2};{../Mp5};{../Mp1};\"Z\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp5};{../Mp1};{../Mp2};\"E\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp6};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp6};{../Mp2};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp2};{../Mp6};{../Mp1};\"Z\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp6};{../Mp1};{../Mp2};\"E\")",
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
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp4", true, "X", "Y", "Z" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp4};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp4};{../Mp2};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp2};{../Mp4};{../Mp1};\"Z\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp4};{../Mp1};{../Mp2};\"E\")",
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
				new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp3", true, "X", "Y", "Z" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 1.049
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};\"Y\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = -5.525
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};\"Z\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 8.811
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};\"E\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 6.803
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp7};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp7", false, "X", "Y", "Z" )
				},
				ExpectedResult = -2.233
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_DIST_PT_2PT({../Mp7};{../Mp8};{../Mp9};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp7", false, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp8", false, "X", "Y", "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp9", false, "X", "Y", "Z" )
				},
				ExpectedResult = 0.457
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( new[]
			{
				new CharacteristicInfo( "X" )
			} );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", new[]
			{
				new CharacteristicInfo( "X", 122.1 ),
				new CharacteristicInfo( "Y", -427.882 ),
				new CharacteristicInfo( "Z", 283.896 )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", new[]
			{
				new CharacteristicInfo( "X", 300.563 ),
				new CharacteristicInfo( "Y", -465 ),
				new CharacteristicInfo( "Z", 475.83 )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", new[]
			{
				new CharacteristicInfo( "X", 253.699 ),
				new CharacteristicInfo( "Y", -278 ),
				new CharacteristicInfo( "Z", 482.846 )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp4", new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp5", new[]
			{
				new CharacteristicInfo( "X", 253.699 ),
				new CharacteristicInfo( "Y", -278 ),
				new CharacteristicInfo( "Z", 482.846 )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp7", new[]
			{
				new CharacteristicInfo( "X", 135.454 ),
				new CharacteristicInfo( "Y", -32.534 ),
				new CharacteristicInfo( "Z", 53.534 )
			}, false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp8", new[]
			{
				new CharacteristicInfo( "X", 213.4 ),
				new CharacteristicInfo( "Y", -231.543 ),
				new CharacteristicInfo( "Z", 231.233 )
			}, false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp9", new[]
			{
				new CharacteristicInfo( "X", 122.5 ),
				new CharacteristicInfo( "Y", -52.122 ),
				new CharacteristicInfo( "Z", 42.12 )
			}, false ) );

			return characteristics;
		}

		private static Dictionary<PathInformationDto, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformationDto, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), -1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), 2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), -3 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), -4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Y", true ), 5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), 6 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "X", true ), 4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Y", true ), -5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Z", true ), 6 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "X", true ), 4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "Y", true ), -5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "Z", true ), 6 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "X", false ), 3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "Y", false ), -4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "Z", false ), 5 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp8", "X", false ), 2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp8", "Y", false ), -1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp8", "Z", false ), 3 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp9", "X", false ), 4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp9", "Y", false ), -2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp9", "Z", false ), 1 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_DIST_PT_2PT({../Mp1}{../Mp2};{../Mp3};\"X\")" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};\'X\')" )]
		[TestCase( "PT_DIST_PT_2PT(\'{../Mp1}\';{../Mp2};{../Mp3};\"X\")" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};\"X\"" )]
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
		[TestCase( "PT_DIST_PT_2PT(\"{../Mp1}\";{../Mp2};{../Mp3};\"X\")" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};{../Mp3})" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};\"X\";{../Mp2};{../Mp3})" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};1.23;{../Mp3};\"X\")" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};123.23)" )]
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
		[TestCase( "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3})" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};\"L\")" )]
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
			const string formula = "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test wrong count of characteristic arguments
		/// </summary>
		[Test]
		[TestCase( "PT_DIST_PT_2PT(\"X\")" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};\"X\")" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};{../Mp2};\"X\")" )]
		[TestCase( "PT_DIST_PT_2PT({../Mp1};{../Mp2};{../Mp3};{../Mp4};\"X\")" )]
		public void TestWrongCharacteristicArgumentCount( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test missing measurement value on Mp5
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
		/// Test missing characteristic on Mp6
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
		/// Test missing nominal value on Mp4
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