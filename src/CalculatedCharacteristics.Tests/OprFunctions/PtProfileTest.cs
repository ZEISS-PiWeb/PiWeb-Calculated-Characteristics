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
	/// Test class for function PT_PROFILE (profile tolerance)
	/// </summary>
	[TestFixture]
	public class PtProfileTest
	{
		#region methods

		private static IEnumerable CreateMissingMeasurementValueTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1};{../Mp3};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" )
				},
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1};{../Mp5};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
				},
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingLowerAndUpperToleranceTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1};{../Mp3};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X" )
				},
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingLowerToleranceTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1};{../Mp4};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "X" )
				},
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingUpperToleranceTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1};{../Mp4};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP4", true, "Y" )
				},
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateValidDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1},{../Mp2};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp2},{../Mp1};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1},{../Mp2};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Y" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp2},{../Mp1};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Y" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1},{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Z" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp2},{../Mp1};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Z" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1},{../Mp2};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "N" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp2},{../Mp1};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "N" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1},{../Mp2};\"M\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "M" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "M" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp2},{../Mp1};\"M\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "M" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "M" )
				},
				ExpectedResult = 0.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp1},{../Mp6};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "X" )
				},
				ExpectedResult = 1.9
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_PROFILE({../Mp6},{../Mp7};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP7", false, "Z" )
				},
				ExpectedResult = 4.7
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( new[]
			{
				new CharacteristicInfo( "X" ),
				new CharacteristicInfo( "Y" ),
				new CharacteristicInfo( "Z" ),
				new CharacteristicInfo( "N" ),
				new CharacteristicInfo( "M" )
			} );
			var tol = new Tolerance( -1.0, 2.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol ),
				new CharacteristicInfo( "M", null, tol )
			}, true ) );

			tol = new Tolerance( -2.0, 2.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol ),
				new CharacteristicInfo( "M", null, tol )
			}, true ) );

			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", new[]
			{
				new CharacteristicInfo( "X" )
			}, true ) );

			var onlyLowerTol = new Tolerance( -2.0, null );
			var onlyUpperTol = new Tolerance( null, 2.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp4", new[]
			{
				new CharacteristicInfo( "X", null, onlyLowerTol ),
				new CharacteristicInfo( "Y", null, onlyUpperTol )
			}, true ) );

			tol = new Tolerance( -1.0, 3.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp6", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol ),
				new CharacteristicInfo( "M", null, tol )
			}, false ) );
			tol = new Tolerance( -2.0, 1.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp7", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol ),
				new CharacteristicInfo( "M", null, tol )
			}, false ) );
			return characteristics;
		}

		private static Dictionary<PathInformation, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformation, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), 1.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), 1.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), 1.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "N", true ), 1.7 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "M", true ), 1.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 1.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Y", true ), 1.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), 1.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "N", true ), 1.7 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "M", true ), 1.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp3", "X", true ), 1.7 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "X", false ), -0.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Y", false ), -0.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Z", false ), -0.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "N", false ), -0.7 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "M", false ), -0.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "X", false ), 2.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "Y", false ), 2.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "Z", false ), 2.6 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "N", false ), 2.7 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp7", "M", false ), 2.8 },
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_PROFILE({../Mp1}{../Mp2};\"X\")" )]
		[TestCase( "PT_PROFILE({../Mp1};{../Mp2};\'X\')" )]
		[TestCase( "PT_PROFILE(\'{../Mp1}\';{../Mp2};\"X\")" )]
		[TestCase( "PT_PROFILE({../Mp1};{../Mp2};\"X\"" )]
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
		[TestCase( "PT_PROFILE(\"{../Mp1}\";{../Mp2};\"X\")" )]
		[TestCase( "PT_PROFILE({../Mp1};{../Mp2};{../Mp3})" )]
		[TestCase( "PT_PROFILE(1.23;{../Mp2};\"X\")" )]
		[TestCase( "PT_PROFILE({../Mp1};{../Mp2};1.23)" )]
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
			var formula = "PT_PROFILE(\"X\")";

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
			const string formula = "PT_PROFILE({../Mp1};{../Mp2};{../Mp3})";

			Assert.That( operationTestEnvironment.GetDependentCharacteristics( formula ), Is.Empty );
			Assert.Throws<ArgumentException>( () => operationTestEnvironment.GetResult( formula, values ) );
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
			const string formula = "PT_PROFILE({../Mp1};{../Mp2};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test a missing measurement value
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
		/// Test a missing characteristic
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
		/// Test multiple characteristics with missing tolerances on Mp3
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingLowerAndUpperToleranceTestCases ) )]
		public void TestMissingLowerAndUpperTolerance( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test multiple characteristics with missing lower tolerance on Mp4
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingLowerToleranceTestCases ) )]
		public void TestMissingLowerTolerance( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test multiple characteristics with missing upper tolerance on Mp4
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateMissingUpperToleranceTestCases ) )]
		public void TestMissingUpperTolerance( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
		}

		/// <summary>
		/// Test a direction
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