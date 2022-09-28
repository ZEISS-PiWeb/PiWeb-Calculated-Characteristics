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
	using Zeiss.PiWeb.Api.Contracts;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	/// <summary>
	/// Test class for function PT_MIN (minimum of points)
	/// </summary>
	[TestFixture]
	public class PtMinTest
	{
		#region methods

		private static IEnumerable CreateValidDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" )
				},
				ExpectedResult = 5.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" )
				},
				ExpectedResult = 4.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" )
				},
				ExpectedResult = 3.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" )
				},
				ExpectedResult = 2.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};\"M\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = 1.2
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};{../Mp3};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X" )
				},
				ExpectedResult = 1.2
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp2};{../Mp3};{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "Y" )
				},
				ExpectedResult = 2.2
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp3};{../Mp1};{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "Z" )
				},
				ExpectedResult = 3.2
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};{../Mp3};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "N" )
				},
				ExpectedResult = 2.2
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp2};{../Mp3};{../Mp1};\"M\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = 1.2
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp6};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "X" )
				},
				ExpectedResult = 4.6
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp6};{../Mp7};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP7", false, "Y" )
				},
				ExpectedResult = 2.7
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
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};{../Mp5};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 1.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp5};{../Mp2};{../Mp1};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = 1.5
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
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};{../Mp5};\"X\";\"true\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp5};{../Mp2};{../Mp1};\"X\";\"true\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateNotExistingDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};\"D\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};\"1\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};\"?\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};\" \")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};\"\t\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp2};{../Mp4};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "Mp2", true, "X" )
				},
				ExpectedResult = 1.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp4};{../Mp1};\"D\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_MIN({../Mp1};{../Mp4};\"1\")",
				ExpectedDependentCharacteristics = Array.Empty<OprFunctionTestCase.ExpectedMeasurementPoint>(),
				ExpectedResult = null
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			} );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", new[]
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
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp6", new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" )
			}, false ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp7", new[]
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
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), 5.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), 4.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), 3.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "N", true ), 2.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "M", true ), 1.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 1.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Y", true ), 2.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), 3.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "N", true ), 4.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "M", true ), 4.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "X", true ), 1.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Y", true ), 2.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "Z", true ), 3.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "N", true ), 4.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "M", true ), 5.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "X", false ), 4.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Y", false ), 3.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "X", false ), 3.7 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "Y", false ), 2.7 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_MIN({../Mp1}{../Mp2};\"X\")" )]
		[TestCase( "PT_MIN({../Mp1};{../Mp2};\'X\')" )]
		[TestCase( "PT_MIN(\'{../Mp1}\';{../Mp2};\"X\")" )]
		[TestCase( "PT_MIN({../Mp1};{../Mp2};\"X\"" )]
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
		[TestCase( "PT_MIN(\"{../Mp1}\";{../Mp2};\"X\")" )]
		[TestCase( "PT_MIN({../Mp1};{../Mp2};{../Mp3})" )]
		[TestCase( "PT_MIN(12.3;{../Mp2};\"X\")" )]
		[TestCase( "PT_MIN({../Mp1};{../Mp2};1.23)" )]
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
			const string formula = "PT_MIN(\"X\")";

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
			const string formula = "PT_MIN({../Mp1})";

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
			const string formula = "PT_MIN({../Mp1};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test minimum with single and multiple characteristics for different directions
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