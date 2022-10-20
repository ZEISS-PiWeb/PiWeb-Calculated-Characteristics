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

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	/// <summary>
	/// Test class for function PT_POS_SQUARE (location of a point)
	/// </summary>
	[TestFixture]
	public class PtPosSquareTest
	{
		#region methods

		private static IEnumerable CreateValidDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp1};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" )
				},
				ExpectedResult = -4.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" )
				},
				ExpectedResult = 5.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp1};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" )
				},
				ExpectedResult = 3.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp1};\"XY\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "Y" )
				},
				ExpectedResult = 5.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp1};\"XZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "Z" )
				},
				ExpectedResult = 4.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp1};\"YZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y", "Z" )
				},
				ExpectedResult = 5.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp1};\"XYZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "Y", "Z" )
				},
				ExpectedResult = 5.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp1};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" )
				},
				ExpectedResult = -3.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp5};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP5", false, "X" )
				},
				ExpectedResult = 3.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp5};\"XYZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP5", false, "X", "Y", "Z" )
				},
				ExpectedResult = 5.0
			};
		}

		private static IEnumerable CreateMissingMeasurementTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp3};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp3};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "Y" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp3};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "Z" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp3};\"XY\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X", "Y" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp3};\"XZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X", "Z" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp3};\"YZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "Y", "Z" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp3};\"XYZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X", "Y", "Z" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp3};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "N" )
				},
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp2};\"X\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp2};\"Y\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp2};\"Z\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp2};\"XY\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp2};\"XZ\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp2};\"YZ\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp2};\"XYZ\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp2};\"N\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingToleranceTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp4};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X" )
				},
				ExpectedResult = -4.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp4};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Y" )
				},
				ExpectedResult = 5.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp4};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Z" )
				},
				ExpectedResult = 3.0
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp4};\"XY\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X", "Y" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp4};\"XZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X", "Z" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp4};\"YZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Y", "Z" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp4};\"XYZ\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X", "Y", "Z" )
				},
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_POS_SQUARE({../Mp4};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "N" )
				},
				ExpectedResult = -3.0
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var tol = new Tolerance( -2.0, 2.0 );
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( new[]
			{
				new CharacteristicInfo( "X", tolerance: tol ),
				new CharacteristicInfo( "Y", tolerance: tol ),
				new CharacteristicInfo( "Z", tolerance: tol ),
				new CharacteristicInfo( "N", tolerance: tol )
			} );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp5", new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			}, false ) );
			return characteristics;
		}

		private static Dictionary<PathInformation, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformation, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), -4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "N", true ), -3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), 5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), 3 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "X", true ), -4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "N", true ), -3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "Y", true ), 5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp4", "Z", true ), 3 },

				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "X", false ), 3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "N", false ), 1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Y", false ), -4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Z", false ), 5 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_POS_SQUARE({../Mp1}\"X\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\'X\')" )]
		[TestCase( "PT_POS_SQUARE(\'{../Mp1}\';\"X\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"X\"" )]
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
		[TestCase( "PT_POS_SQUARE(\"{../Mp1}\";\"X\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};{../Mp1})" )]
		[TestCase( "PT_POS_SQUARE(\"X\";{../Mp1})" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};1.23)" )]
		[TestCase( "PT_POS_SQUARE(1.23;\"X\")" )]
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
		public void TestMissingCharacteristicArguments()
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );
			var formula = "PT_POS_SQUARE(\"X\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test too many characteristic arguments
		/// </summary>
		[Test]
		public void TestTooManyCharacteristicArguments()
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );
			var formula = "PT_POS_SQUARE({../Mp1};{../Mp2};\"X\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test invalid direction arguments
		/// </summary>
		[Test]
		[TestCase( "PT_POS_SQUARE({../Mp1})" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"XN\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"YN\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"ZN\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"NX\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"NY\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"NZ\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"XYN\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"XZN\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"YZN\")" )]
		[TestCase( "PT_POS_SQUARE({../Mp1};\"F\")" )]
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
			const string formula = "PT_POS_SQUARE({../Mp1};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test all valid directions
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
		/// Test direction with missing measurement value
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingMeasurementTestCases ) )]
		public void TestMissingMeasurementValue( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test missing characteristic Mp2
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
		/// Test without tolerances (no result)
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingToleranceTestCases ) )]
		public void TestMissingTolerance( OprFunctionTestCase testCase )
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			} );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp4", new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			}, true ) );

			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		#endregion
	}
}