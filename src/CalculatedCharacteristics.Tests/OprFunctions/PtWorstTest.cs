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
	/// Test class for function PT_WORST (worst value in a set of points)
	/// </summary>
	[TestFixture]
	public class PtWorstTest
	{
		#region methods

		private static IEnumerable CreateMissingMeasurementTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1};{../Mp3};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp3};{../Mp1};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateInvalidFormulaTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
				new OprFunctionTestCase.ExpectedMeasurementPoint( "MP3", true, "X" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1},{../Mp3},\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1};{../Mp3};\"X\";)",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "pt_worst({../Mp1};{../Mp3};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateMissingCharacteristicTestCases()
		{
			var expectedDependentCharacteristics = new[]
			{
				new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" )
			};

			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1},{../Mp4};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp4},{../Mp1};\"X\")",
				ExpectedDependentCharacteristics = expectedDependentCharacteristics,
				ExpectedResult = null
			};
		}

		private static IEnumerable CreateOnlyDocumentationRequiredCharacteristicsTestCases()
		{
			yield return new OprFunctionTestCase //Mp1 und Mp2 --> Y und Z nicht dokumentationspflichtig
			{
				GivenFormula = "PT_WORST({../Mp1},{../Mp2};\"P\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				},
				ExpectedResult = -1.9
			};
			yield return new OprFunctionTestCase //Mp1 --> Y und Z nicht dokumentationspflichtig
			{
				GivenFormula = "PT_WORST({../Mp1};\"P\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X", "N" )
				},
				ExpectedResult = -1.9
			};
			yield return new OprFunctionTestCase //Mp2 --> Y und Z nicht dokumentationspflichtig
			{
				GivenFormula = "PT_WORST({../Mp2};\"P\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				},
				ExpectedResult = 1.3
			};
		}

		private static IEnumerable CreateValidDirectionTestCases()
		{
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1};{../Mp2};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				},
				ExpectedResult = 1.3
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp2};{../Mp1};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "X" )
				},
				ExpectedResult = 1.3
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1};{../Mp2};\"Y\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Y" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Y" )
				},
				ExpectedResult = 1.5
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1};{../Mp2};\"Z\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "Z" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "Z" )
				},
				ExpectedResult = -2.3
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1};{../Mp2};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP2", true, "N" )
				},
				ExpectedResult = -1.9
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp1};{../Mp5};\"X\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP1", true, "X" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP5", false, "X" )
				},
				ExpectedResult = 0.7
			};
			yield return new OprFunctionTestCase
			{
				GivenFormula = "PT_WORST({../Mp5};{../Mp6};\"N\")",
				ExpectedDependentCharacteristics = new[]
				{
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP5", false, "N" ),
					new OprFunctionTestCase.ExpectedMeasurementPoint( "MP6", false, "N" )
				},
				ExpectedResult = 1.9
			};
		}

		private static InspectionPlanCollection CreateCharacteristics()
		{
			var characteristics = OprFunctionsTestHelper.SetupInspectionPlanWithOprFunctionPoint( new[]
			{
				new CharacteristicInfo( "X" )
			} );
			var tol = new Tolerance( -1.0, 2.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp1", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol, true )
			}, true ) );

			tol = new Tolerance( -2.0, 2.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp2", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol )
			}, true ) );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp3", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol )
			}, true ) );
			tol = new Tolerance( -2.0, 1.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp5", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol, true )
			}, false ) );
			tol = new Tolerance( -2.0, 3.0 );
			characteristics.AddRange( OprFunctionsTestHelper.CreateMeasurementPoint( "Mp6", new[]
			{
				new CharacteristicInfo( "X", null, tol, true ),
				new CharacteristicInfo( "Y", null, tol ),
				new CharacteristicInfo( "Z", null, tol ),
				new CharacteristicInfo( "N", null, tol, true )
			}, false ) );
			return characteristics;
		}

		private static Dictionary<PathInformationDto, double> CreateMeasurementValues()
		{
			var values = new Dictionary<PathInformationDto, double>
			{
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "X", true ), 1.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Y", true ), 1.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "Z", true ), -1.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp1", "N", true ), -1.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "X", true ), 1.3 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Y", true ), 1.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "Z", true ), 1.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp2", "N", true ), -1.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "X", false ), -0.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Y", false ), -0.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "Z", false ), 0.1 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp5", "N", false ), 1.4 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "X", false ), 2.2 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Y", false ), 2.5 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "Z", false ), -0.8 },
				{ OprFunctionsTestHelper.GetDirectionPath( "Mp6", "N", false ), -0.4 }
			};
			return values;
		}

		/// <summary>
		/// Test invalid formula
		/// </summary>
		[Test]
		[TestCase( "PT_WORST({../Mp1}{../Mp3};\"X\")" )]
		[TestCase( "PT_WORST({../Mp1};{../Mp3};\'X\')" )]
		[TestCase( "PT_WORST(\'{../Mp1}\';{../Mp3};\"X\")" )]
		[TestCase( "PT_WORST({../Mp1};{../Mp3};\"X\"" )]
		public void TestInvalidFormula_ThrowsParseException( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.TypeOf<ParserException>() );
		}

		/// <summary>
		/// Test missing characteristic argument
		/// </summary>
		[Test]
		[TestCase( "PT_WORST(\"{../Mp1}\";{../Mp3};\"X\")" )]
		[TestCase( "PT_WORST({../Mp1};{../Mp3};{../Mp2})" )]
		[TestCase( "PT_WORST(1.23;{../Mp3};\"X\")" )]
		[TestCase( "PT_WORST({../Mp1};{../Mp3};1.23)" )]
		public void TestInvalidArguments( string formula )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}


		/// <summary>
		/// Test invalid formula
		/// </summary>
		[TestCaseSource( nameof( CreateInvalidFormulaTestCases ) )]
		public void TestInvalidFormulaNullResult( OprFunctionTestCase testCase )
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();

			testCase.AssertTestCase( characteristics, values );
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
			const string formula = "PT_WORST(\"X\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test invalid direction argument
		/// </summary>
		[Test]
		[TestCase( "PT_WORST({../Mp1};{../Mp2})" )]
		[TestCase( "PT_WORST({../Mp1};{../Mp2};\"F\")" )]
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
		public void TestEmptyDirection_ThrowsException()
		{
			var characteristics = CreateCharacteristics();
			var values = CreateMeasurementValues();
			var operationTestEnvironment = new OperationTestEnvironment( characteristics );
			const string formula = "PT_WORST({../Mp1};{../Mp2};\"\")";

			Assert.That( () => operationTestEnvironment.GetDependentCharacteristics( formula ), Throws.Nothing );
			Assert.That( () => operationTestEnvironment.GetResult( formula, values ), Throws.ArgumentException );
		}

		/// <summary>
		/// Test missing measurement value
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
		/// Test missing characteristic
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
		/// Test only characteristics which are required for documentation
		/// </summary>
		[Test]
		[TestCaseSource( nameof( CreateOnlyDocumentationRequiredCharacteristicsTestCases ) )]
		public void TestOnlyDocumentationRequiredCharacteristics( OprFunctionTestCase testCase )
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