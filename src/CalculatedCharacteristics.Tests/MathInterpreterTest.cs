#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2022                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests
{
	#region usings

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	/// <summary>
	/// Klasse zum Testen des Interpreters
	/// </summary>
	[TestFixture]
	public class MathInterpreterTest
	{
		#region members

		private static readonly ConfigurationDto Configuration = CreateBasicConfiguration();
		private static readonly CatalogCollectionDto CatalogCollection = new CatalogCollectionDto();

		private static readonly CharacteristicCalculatorFactory EmptyCharacteristicCalculatorFactory = ( _ => null );
		private static readonly ChildPathsHandler EmptyChildPathsHandler = ( _ => [] );
		private static readonly MeasurementValueHandler EmptyMeasurementValueHandler = ( _ => null );
		private static readonly EntityAttributeValueHandler EmptyEntityAttributeValueHandler = ( ( _, _, _ ) => null );

		#endregion

		#region methods

		private static MathInterpreter CreateMathInterpreter()
		{
			return new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
		}

		private static double? GetFormulaResult( string formula )
		{
			var mathInterpreter = CreateMathInterpreter();
			var calculator = mathInterpreter.Parse( formula, null );
			return calculator.GetResult( EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler ).GetValueOrDefault( double.NaN );
		}

		/// <summary>
		/// Testet, ob die Konstanten funktionieren.
		/// </summary>
		[Test]
		[TestCase( "PI", Math.PI )]
		[TestCase( "E", Math.E )]
		[TestCase( "1.23", 1.23 )]
		[TestCase( "-1.23", -1.23 )]
		public void Test_BasicConstants( string formula, double expectedResult )
		{
			// arrange

			// act
			var result = GetFormulaResult( formula );

			// assert
			Assert.That( result, Is.EqualTo( expectedResult ) );
		}

		/// <summary>
		/// Testet, ob die Funktionen funktionieren.
		/// </summary>
		[Test]
		[TestCase( "sum(1)", 1.0, 0.0 )]
		[TestCase( "sum(1,2)", 3.0, 0.0 )]
		[TestCase( "sum(1,2,3,-4)", 2.0, 0.0 )]
		[TestCase( "cos(PI)", -1.0, 1.0e-10 )]
		[TestCase( "sin(PI)", 0.0, 1.0e-10 )]
		[TestCase( "tan(PI)", 0.0, 1.0e-10 )]
		[TestCase( "sqr(3.0)", 9.0, 0.0 )]
		[TestCase( "sqrt(9.0)", 3.0, 0.0 )]
		[TestCase( "abs(PI)", Math.PI, 0.0 )]
		[TestCase( "abs(-PI)", Math.PI, 0.0 )]
		[TestCase( "max(1;0)", 1.0, 0.0 )]
		[TestCase( "max(-1;0)", 0.0, 0.0 )]
		[TestCase( "max(0;-1)", 0.0, 0.0 )]
		[TestCase( "min(1;0)", 0.0, 0.0 )]
		[TestCase( "min(-1;0)", -1.0, 0.0 )]
		[TestCase( "min(0;-1)", -1.0, 0.0 )]
		[TestCase( "pow(5;3)", 125.0, 0.0 )]
		[TestCase( "pow(125;1/3)", 5.0, 1.0e-15 )]
		public void Test_BasicFunctions( string formula, double expectedResult, double delta )
		{
			// arrange

			// act
			var result = GetFormulaResult( formula );

			// assert
			Assert.That( result, Is.EqualTo( expectedResult ).Within( delta ) );
		}

		/// <summary>
		/// Testet, ob die Klammerung funktioniert.
		/// </summary>
		[Test]
		[TestCase( "(((((cos((((((PI)))))))))))", -1.0, 1.0e-10 )]
		[TestCase( "1.0 + ( 1.0 + ( 1.0 + ( -3 )) )", 0.0, 0.0 )]
		[TestCase( "( -2 ) - 5", -7.0, 0.0 )]
		[TestCase( "- ( 2 - 5 )", 3.0, 0.0 )]
		[TestCase( "( 1 + 2 ) * 3", 9, 0.0 )]
		[TestCase( "((( 1 + (2) )) * (3))", 9, 0.0 )]
		[TestCase( "1 / ( 10 * 3 / 6 )", 0.2, 0.0 )]
		[TestCase( "( 1 + 3 ) * 3 / ( 2 * 3 )", 2, 0.0 )]
		public void Test_GroupingByBraces( string formula, double expectedResult, double delta )
		{
			// arrange

			// act
			var result = GetFormulaResult( formula );

			// assert
			Assert.That( result, Is.EqualTo( expectedResult ).Within( delta ) );
		}

		/// <summary>
		/// Testet, ob ungültige Formeln erkannt werden.
		/// </summary>
		[Test]
		[TestCase( "" )]
		[TestCase( "1 + ( 2" )]
		[TestCase( "( 1 + 2" )]
		[TestCase( "1 ) + 2" )]
		[TestCase( "1 + 2 )" )]
		public void Test_InvalidFormulas( string formula )
		{
			var interpreter = CreateMathInterpreter();
			Assert.That( () => interpreter.Parse( formula, null ), Throws.TypeOf<ParserException>() );
		}

		/// <summary>
		/// Testet, ob das Parsen eine fehlende Formel erkennt.
		/// </summary>
		[Test]
		public void Test_NullFormula()
		{
			var interpreter = CreateMathInterpreter();
			Assert.That( () => interpreter.Parse( null!, null ), Throws.ArgumentNullException );
			Assert.That( () => interpreter.Parse( null!, PathInformation.Root ), Throws.ArgumentNullException );
		}

		/// <summary>
		/// Testet, ob die elementaren Rechenoperationen funktionieren.
		/// </summary>
		[Test]
		[TestCase( "2 + 5", 7.0, 0.0 )]
		[TestCase( "-2 + 5", 3.0, 0.0 )]
		[TestCase( "2 + -5", -3.0, 0.0 )]
		[TestCase( "2 - 5", -3.0, 0.0 )]
		[TestCase( "-2 - 5", -7.0, 0.0 )]
		[TestCase( "2 - -5", 7.0, 0.0 )]
		[TestCase( "-2 + -5", -7.0, 0.0 )]
		[TestCase( "2 * 5", 10.0, 0.0 )]
		[TestCase( "-2 * 5", -10.0, 0.0 )]
		[TestCase( "2 * -5", -10.0, 0.0 )]
		[TestCase( "2 / 5", 0.4, 0.0 )]
		[TestCase( "-2 / 5", -0.4, 0.0 )]
		[TestCase( "2 / -5", -0.4, 0.0 )]
		[TestCase( "1 + 2 - 3", 0, 0.0 )]
		[TestCase( "1 / 2 * 3", 1.5, 0.0 )]
		[TestCase( "3 * 1 / 2", 1.5, 0.0 )]
		[TestCase( "1 + 2 * 3", 7, 0.0 )]
		[TestCase( "1 * 2 + 3", 5, 0.0 )]
		[TestCase( "1 + 2 * 3 + 4 * 5", 27, 0.0 )]
		[TestCase( "-1 / +2 + 3 * 4 / 5", 1.9, 1.0e-10 )]
		[TestCase( "3 - 2 - 1", 0, 0 )]
		[TestCase( "10 + 9 - 8 + 7 - 6 + 5 - 4 + 3 - 2 + 1", 15, 0 )]
		public void Test_BasicOperators( string formula, double expectedResult, double delta )
		{
			// arrange

			// act
			var result = GetFormulaResult( formula );

			// assert
			Assert.That( result, Is.EqualTo( expectedResult ).Within( delta ) );
		}

		/// <summary>
		/// Testet die Kombination von Operationen.
		/// </summary>
		[Test]
		[TestCase( "max(1;2;3) + min(5;6;7)", 8.0, 0.0 )]
		[TestCase( "max(1,2,sqrt(9)) + min(5,6,-5+3*4)", 8.0, 0.0 )]
		public void Test_NestedFormulas( string formula, double expectedResult, double delta )
		{
			// arrange

			// act
			var result = GetFormulaResult( formula );

			// assert
			Assert.That( result, Is.EqualTo( expectedResult ).Within( delta ) );
		}

		[Test( Description = "Test for nested calculation." )]
		public void Test_NestedMeasuredValueCalculation()
		{
			// Given
			var inspectionPlan = new InspectionPlanCollection();
			var part = CreatePart( "NestedCalculation" );
			inspectionPlan.Add( part );

			var ch1 = CreateCharacteristic( "Ch1", part.Path );
			var ch2 = CreateCharacteristic( "Ch2", part.Path );
			var ch3 = CreateCharacteristic( "Ch3", part.Path );
			inspectionPlan.AddRange( [ch1, ch2, ch3] );

			var characteristicValues = new Dictionary<PathInformation, double>
			{
				{ ch1.Path, 1.2 },
				{ ch2.Path, 1.5 },
				{ ch3.Path, -0.2 }
			};

			// geschachtelte Berechnung
			inspectionPlan.Add( CreateCharacteristic( "Calc1", part.Path, "{Ch1} + {Ch2}" ) );
			var calc2 = CreateCharacteristic( "Calc2", part.Path, "{Calc1} + {Ch3}" );
			inspectionPlan.Add( calc2 );

			// direkte Ringreferenz
			var calc3 = CreateCharacteristic( "Calc3", part.Path, "{Calc1} + {Calc3}" );
			inspectionPlan.Add( calc3 );

			// indirekte Ringreferenz
			var calc4 = CreateCharacteristic( "Calc4", part.Path, "{Calc5} + {Calc6}" );
			inspectionPlan.Add( calc4 );
			inspectionPlan.Add( CreateCharacteristic( "Calc5", part.Path, "{Calc6} + 1" ) );
			inspectionPlan.Add( CreateCharacteristic( "Calc6", part.Path, "{Calc5} - 1" ) );

			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, k ) => GetCharacteristicAttribute( inspectionPlan, path, k ),
				p => inspectionPlan.GetDirectChildren( p ).Select( ch => ch.Path ) );
			var interpreter = attributeBasedMathInterpreterFactory.GetInterpreter();

			// When/Then
			var calculator = interpreter.Parse( GetFormula( calc2 ), calc2.Path );
			var measurementValueHandler = new MeasurementValueHandler( path => characteristicValues[ path ] );

			Assert.That( calculator.GetResult( measurementValueHandler, EmptyEntityAttributeValueHandler ), Is.EqualTo( 2.5 ) );

			// bei Ringreferenz kann es kein Ergebnis geben
			calculator = interpreter.Parse( GetFormula( calc3 ), calc3.Path );
			Assert.That( calculator.GetResult( measurementValueHandler, EmptyEntityAttributeValueHandler ), Is.Null );
			calculator = interpreter.Parse( GetFormula( calc4 ), calc4.Path );
			Assert.That( calculator.GetResult( measurementValueHandler, EmptyEntityAttributeValueHandler ), Is.Null );
		}

		[Test( Description = "Test if calculation works with unusual characters in inspection plan paths as well." )]
		public void Test_UnusualCharactersInPaths()
		{
			// Given
			var inspectionPlan = new InspectionPlanCollection();
			var part = CreatePart( "UnusualCharactersInPaths" );
			inspectionPlan.Add( part );

			var ch1 = CreateCharacteristic( "D. 18.97 +0.05_-0.08 oben min IMG", part.Path );
			var ch2 = CreateCharacteristic( "D. 18.97 +0.05_-0.08 oben max IMG", part.Path );
			var ch3 = CreateCharacteristic( "characteristic(23)", part.Path );
			var ch4 = CreateCharacteristic( "characteristic/comment", part.Path );
			inspectionPlan.AddRange( [ch1, ch2, ch3, ch4] );

			var characteristicValues = new Dictionary<PathInformation, double>
			{
				{ ch1.Path, 1.2 },
				{ ch2.Path, 1.5 },
				{ ch3.Path, -0.2 },
				{ ch4.Path, 0.2 }
			};

			// Merkmalsnamen mit '+' UND Leerzeichen (siehe MathInterpreter.TERMINALS)
			var calc1 = CreateCharacteristic(
				"D. 18.97 +0.05/-0.08 oben IMG",
				part.Path,
				"{D. 18.97 +0.05_-0.08 oben min IMG} + {D. 18.97 +0.05_-0.08 oben max IMG}" );
			inspectionPlan.Add( calc1 );

			// Merkmalsnamen mit runden Klammern bzw. Slash (GEHT NICHT - keine Maskierung)
			var calc2 = CreateCharacteristic( "calc2", part.Path, "{characteristic(23)} + {characteristic/comment}" );
			inspectionPlan.Add( calc2 );

			var entityAttributeValueHandler = new EntityAttributeValueHandler( ( path, key, _ ) => GetCharacteristicAttribute( inspectionPlan, path, key ) );
			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, key ) => entityAttributeValueHandler( path, key, null ),
				p => inspectionPlan.GetDirectChildren( p ).Select( ch => ch.Path ) );

			var interpreter = attributeBasedMathInterpreterFactory.GetInterpreter();
			var measurementValueHandler = new MeasurementValueHandler( path => characteristicValues[ path ] );

			// When/Then
			var calculator = interpreter.Parse( GetFormula( calc1 ), calc1.Path );
			Assert.That( calculator.GetResult( measurementValueHandler, entityAttributeValueHandler ), Is.EqualTo( 2.7 ) );

			calculator = interpreter.Parse( GetFormula( calc2 ), calc2.Path );
			Assert.That( () => calculator.GetResult( measurementValueHandler, entityAttributeValueHandler ), Throws.TypeOf<KeyNotFoundException>() );
		}

		[Test]
		[TestCaseSource( nameof( CreateEntityPathResolverTests ) )]
		public void Test_ValueCalculationForPaths(
			InspectionPlanCollection inspectionPlan,
			MeasurementValueHandler measurementValueHandler,
			InspectionPlanCharacteristicDto calcEntity,
			double expectedResult )
		{
			// arrange

			// create required handlers for MathInterpreter
			var characteristicAttributeHandler = new EntityAttributeValueHandler(
				( path, key, _ ) => GetCharacteristicAttribute( inspectionPlan, path, key ) );
			var childPathHandler = new ChildPathsHandler(
				path => inspectionPlan.GetDirectChildren( path ).Select( ch => ch.Path ) );

			// create the MathInterpreter creator delegate
			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, key ) => characteristicAttributeHandler( path, key, null ),
				childPathHandler );

			var calculator = attributeBasedMathInterpreterFactory.GetCharacteristicCalculator( calcEntity.Path );

			// act
			double? result;
			try
			{
				result = calculator!.GetResult( measurementValueHandler, EmptyEntityAttributeValueHandler );
			}
			finally
			{
				TestContext.WriteLine( "Done" );
			}

			// assert
			Assert.That( result, Is.EqualTo( expectedResult ) );
		}

		[Test]
		public void Test_CallDelegateForGetResultWithNestedCalculatedCharacteristic()
		{
			var expectedPaths = new[]
			{
				PathInformation.Combine( PathInformation.Root, PathElement.Char( "M1" ) ),
				PathInformation.Combine( PathInformation.Root, PathElement.Char( "M2" ) )
			};

			var requestedPaths = new List<PathInformation>();
			var calculatorProvider = new CharacteristicCalculatorFactory( path =>
			{
				requestedPaths.Add( path );
				return null;
			} );

			var sut = new MathInterpreter( calculatorProvider, EmptyChildPathsHandler );
			var calculator = sut.Parse( "{M1}+{M2}", null );

			calculator.GetResult( EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler );

			Assert.That( requestedPaths, Has.Count.EqualTo( 2 ) );
			Assert.That( requestedPaths, Is.EquivalentTo( expectedPaths ) );
		}

		[Test]
		public void Test_DoNotCallDelegateForGetDependenciesWithNestedCalculatedCharacteristic()
		{
			var expectedPaths = new[]
			{
				PathInformation.Combine( PathInformation.Root, PathElement.Char( "M1" ) ),
				PathInformation.Combine( PathInformation.Root, PathElement.Char( "M2" ) )
			};

			var requestedPaths = new List<PathInformation>();
			var calculatorProvider = new CharacteristicCalculatorFactory( path =>
			{
				requestedPaths.Add( path );
				return null;
			} );

			var sut = new MathInterpreter( calculatorProvider, EmptyChildPathsHandler );
			var calculator = sut.Parse( "{M1}+{M2}", null );

			var result = calculator.GetDependentCharacteristics( EmptyEntityAttributeValueHandler );

			Assert.That( requestedPaths, Has.Count.EqualTo( 0 ) );
			Assert.That( result.Keys, Is.EquivalentTo( expectedPaths ) );
		}

		[Test]
		public void Test_PathResolver_IsCalledForAllPaths()
		{
			/*
			 * This test verifies, that the path resolver is called for every path string. We need this behavior
			 * to collect the paths strings within a formula without fetching anything. The calculator is not async,
			 * and we need to make sure that all paths are fetched before it's called.
			 */
			var pathResolver = new EmptyTrackingStringToPathResolver();
			var pathResolverFactory = new PathResolverFactory( _ => pathResolver );

			var sut = new MathInterpreter(
				EmptyCharacteristicCalculatorFactory,
				EmptyChildPathsHandler,
				pathResolverFactory );

			var calculator = sut.Parse( "{M1}+{../M2}+{P1/M3}+{../P2/M4}+{M5(1234)}", null );
			calculator.GetDependentCharacteristics( EmptyEntityAttributeValueHandler );

			Assert.That( pathResolver.RequestedPaths, Has.Count.EqualTo( 5 ) );
			Assert.That( pathResolver.RequestedPaths[0], Is.EqualTo( "M1" ) );
			Assert.That( pathResolver.RequestedPaths[1], Is.EqualTo( "../M2" ) );
			Assert.That( pathResolver.RequestedPaths[2], Is.EqualTo( "P1/M3" ) );
			Assert.That( pathResolver.RequestedPaths[3], Is.EqualTo( "../P2/M4" ) );
			Assert.That( pathResolver.RequestedPaths[4], Is.EqualTo( "M5" ) );
		}

		private class EmptyTrackingStringToPathResolver : IStringToPathResolver
		{
			public List<string?> RequestedPaths { get; } = [];

			public PathInformation? ResolvePath( string? path )
			{
				RequestedPaths.Add( path );
				return PathInformation.Root;
			}
		}

		private static IEnumerable CreateEntityPathResolverTests()
		{
			// define some parts and characteristics
			var partPath = new PathInformation( PathElement.Part( "Calculation" ) );
			var ch1Path = partPath + PathElement.Char( "D. 18.97 +0.05_-0.08 oben min IMG" );
			var ch2Path = partPath + PathElement.Char( "D. 18.97 +0.05_-0.08 oben max IMG" );
			var ch3Path = partPath + PathElement.Char( "characteristic(23)" );
			var ch4Path = partPath + PathElement.Char( "characteristic/comment" );
			var ch5Path = partPath + PathElement.Char( "SimpleNameCharacteristic" );
			var ch6Path = partPath + PathElement.Char( "char with curved braces {_}" );
			var ch7Path = partPath + PathElement.Char( "char with percent %" );
			var ch8Path = partPath + PathElement.Char( @"char with quotes """"" );
			var ch9Path = partPath + PathElement.Char( @"char with escape char \\" );
			var ch10Path = partPath + PathElement.Char( @"char with escape char \" );

			// set measurement values
			var characteristicValues = new Dictionary<PathInformation, double>
			{
				{ ch1Path, 1.2 },
				{ ch2Path, 1.5 },
				{ ch3Path, -0.2 },
				{ ch4Path, 0.2 },
				{ ch5Path, 3.4 },
				{ ch6Path, -3.4 },
				{ ch7Path, 8.7 },
				{ ch8Path, -8.7 },
				{ ch9Path, 12.4 },
				{ ch10Path, -12.4 }
			};

			// fields that can be used in all test cases and will not change
			var measurementValueProvider = new MeasurementValueHandler( path => characteristicValues[ path ] );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				@"{""char with quotes \""\""""}",
				-8.7,
				ch8Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				@"{""char with escape char \\\\""}",
				12.4,
				ch9Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				@"{""char with escape char \\""}",
				-12.4,
				ch10Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				"{SimpleNameCharacteristic}",
				3.4,
				ch5Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				"{D. 18.97 +0.05_-0.08 oben min IMG}",
				1.2,
				ch1Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				"{\"D. 18.97 +0.05_-0.08 oben min IMG\"}",
				1.2,
				ch1Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				"{\"characteristic(23)\"}",
				-0.2,
				ch3Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				"{\"characteristic/comment\"}",
				0.2,
				ch4Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				"{\"char with curved braces {_}\"}",
				-3.4,
				ch6Path );

			yield return CreateTestCase(
				measurementValueProvider,
				partPath,
				"{D. 18.97 +0.05_-0.08 oben min IMG} + {D. 18.97 +0.05_-0.08 oben max IMG}",
				2.7,
				ch1Path, ch2Path );
		}

		private static TestCaseData CreateTestCase(
			MeasurementValueHandler measurementValueHandler,
			PathInformation partPath,
			string formula,
			double expectedResult,
			params PathInformation[] knownPaths )
		{
			if( partPath.Type != InspectionPlanEntity.Part )
				throw new ArgumentException( "Part path must be of type 'Part'!" );

			if( knownPaths.Any( p => p.Type != InspectionPlanEntity.Characteristic ) )
				throw new ArgumentException( "Known paths must be of type 'Characteristic'!" );

			// create calculated characteristic entity with formula
			var calcEntity = CreateCharacteristic( "Calculation", partPath, formula );

			// create dummy inspection plan with root part and required characteristics
			var entitiesCreateQuery = Enumerable.Empty<InspectionPlanDtoBase>()
				.Append( CreatePart( partPath ) )
				.Concat( knownPaths.Select( path => CreateCharacteristic( path ) ) )
				.Append( calcEntity );

			var inspectionPlan = new InspectionPlanCollection( entitiesCreateQuery );

			// create test data container
			return new TestCaseData(
				inspectionPlan,
				measurementValueHandler,
				calcEntity,
				expectedResult )
			{
				TestName = formula
			};
		}

		private static ConfigurationDto CreateBasicConfiguration()
		{
			return new ConfigurationDto
			{
				PartAttributes = [],
				CharacteristicAttributes =
				[
					new AttributeDefinitionDto
					{
						Key = WellKnownKeys.Characteristic.LogicalOperationString,
						Type = AttributeTypeDto.AlphaNumeric,
						Description = "Formel"
					}
				],
				MeasurementAttributes = [],
				ValueAttributes = [],
				CatalogAttributes = []
			};
		}

		private static InspectionPlanPartDto CreatePart( string name )
		{
			return CreatePart( new PathInformation( PathElement.Part( name ) ) );
		}

		private static InspectionPlanPartDto CreatePart( PathInformation path )
		{
			return new InspectionPlanPartDto
			{
				Uuid = Guid.NewGuid(),
				Path = path
			};
		}

		private static InspectionPlanCharacteristicDto CreateCharacteristic( string name, PathInformation parentPath, string? formula = null )
		{
			return CreateCharacteristic( parentPath + new PathElement( InspectionPlanEntity.Characteristic, name ), formula );
		}

		private static InspectionPlanCharacteristicDto CreateCharacteristic( PathInformation path, string? formula = null )
		{
			var characteristic = new InspectionPlanCharacteristicDto
			{
				Uuid = Guid.NewGuid(),
				Path = path
			};

			if( !string.IsNullOrEmpty( formula ) )
				characteristic.SetAttributeValue( WellKnownKeys.Characteristic.LogicalOperationString, formula );

			return characteristic;
		}

		private static string GetFormula( InspectionPlanCharacteristicDto characteristic )
		{
			return characteristic.GetAttributeValue( WellKnownKeys.Characteristic.LogicalOperationString );
		}

		private static object? GetCharacteristicAttribute( InspectionPlanCollection characteristics, PathInformation path, ushort key )
		{
			return characteristics.GetCharacteristicAttribute( Configuration, CatalogCollection, path, key );
		}

		#endregion
	}
}