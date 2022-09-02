#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	[TestFixture]
	public class ValueCalculatorTest
	{
		#region members

		private static readonly CharacteristicCalculatorFactory EmptyCharacteristicCalculatorFactory = _ => null;
		private static readonly ChildPathsHandler EmptyChildPathsHandler = _ => Enumerable.Empty<PathInformationDto>();
		private static readonly EntityAttributeValueHandler EmptyEntityAttributeValueHandler = ( _, _, _ ) => null;
		private static readonly ValueCalculator.MeasurementValueHandler EmptyMeasurementValueHandler = ( _, _ ) => null;

		#endregion

		#region methods

		[Test]
		public void Test_ParseWhenFormulaIsNull()
		{
			//Given
			const string formula = null;

			var mathInterpreter = new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
			var valueCalculator = new ValueCalculator( mathInterpreter, EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler );

			//When/Then
			Assert.That( () => valueCalculator.Parse( formula!, null ), Throws.ArgumentNullException );
		}

		[Test]
		public void Test_ParseWhenFormulaIsEmpty()
		{
			//Given
			const string formula = "";

			var mathInterpreter = new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
			var valueCalculator = new ValueCalculator( mathInterpreter, EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler );

			//When/Then
			Assert.That( () => valueCalculator.Parse( formula, null ), Throws.TypeOf<ParserException>() );
		}

		[Test]
		public void Test_ParseWhenFormulaIsInvalid()
		{
			//Given
			const string formula = "1{{/";

			var mathInterpreter = new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
			var valueCalculator = new ValueCalculator( mathInterpreter, EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler );

			//When/Then
			Assert.That( () => valueCalculator.Parse( formula, null ), Throws.TypeOf<ParserException>() );
		}

		[Test]
		public void Test_Parse()
		{
			//Given
			const string formula = "1 + 1";

			var mathInterpreter = new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
			var valueCalculator = new ValueCalculator( mathInterpreter, EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler );

			//When
			var mathCalculator = valueCalculator.Parse( formula, null );

			//Then
			Assert.That( mathCalculator, Is.Not.Null );
		}

		[Test]
		public void Test_CalculateValueWhenAnyParameterIsNull()
		{
			//Given
			var characteristic = new InspectionPlanCharacteristicDto { Path = new PathInformationDto( PathElementDto.Char( "Char" ) ) };
			const string formula = "1 + 1";

			var mathInterpreter = new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
			var mathCalculator = mathInterpreter.Parse( formula, null );
			var valueCalculator = new ValueCalculator( mathInterpreter, EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler );

			//When
			var result1 = valueCalculator.CalculateValue( null, new DataMeasurementDto(), mathCalculator );
			var result2 = valueCalculator.CalculateValue( characteristic, null, mathCalculator );
			var result3 = valueCalculator.CalculateValue( characteristic, new DataMeasurementDto(), null );

			//Then
			Assert.That( result1, Is.Null );
			Assert.That( result2, Is.Null );
			Assert.That( result3, Is.Null );
		}

		[Test]
		public void Test_CalculateValueWhenCharacteristicBelongsToDifferentPartThanMeasurement()
		{
			//Given
			var characteristic = new InspectionPlanCharacteristicDto { Path = new PathInformationDto( PathElementDto.Char( "Char" ) ) };
			const string formula = "1 + 1";

			var mathInterpreter = new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
			var mathCalculator = mathInterpreter.Parse( formula, null );
			var valueCalculator = new ValueCalculator(
				mathInterpreter,
				EmptyMeasurementValueHandler,
				EmptyEntityAttributeValueHandler,
				( _, _ ) => false ); // delegate checking whether the characteristic 'ch' belongs to the part with the given 'uuid'

			//When
			var result = valueCalculator.CalculateValue( characteristic, new DataMeasurementDto(), mathCalculator );

			//Then
			Assert.That( result, Is.Null );
		}

		[Test]
		public void Test_CalculateValue()
		{
			//Given
			var char1 = CreateCharacteristicWithoutFormula( "char1" );
			var calcChar1 = CreateCharacteristicWithFormula( "calcChar1", "1 + 2" );
			var calcChar2 = CreateCharacteristicWithFormula( "calcChar2", "{calcChar1} + {char1}" );
			var characteristics = new[] { char1, calcChar1, calcChar2 };

			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, key ) => characteristics.FirstOrDefault( ch => ch.Path == path ).GetAttributeValue( key ),
				_ => ArraySegment<PathInformationDto>.Empty );
			var mathInterpreter = attributeBasedMathInterpreterFactory.GetInterpreter();

			var valueCalculator = new ValueCalculator( mathInterpreter, GetMeasurementValue, EmptyEntityAttributeValueHandler );

			var measurement = CreateEmptyMeasurement();
			measurement.Characteristics = new[]
			{
				CreateDataCharacteristic( char1, CreateDataValue( 4 ) ),
				CreateDataCharacteristic( calcChar1, CreateDataValueWithAttributes( null ) )
			};

			var expectedValue1 = CreateDataValueWithAttributes( 3 );
			var expectedValue2 = CreateDataValue( 7 );

			//When
			var mathCalculator1 = mathInterpreter.Parse( GetFormula( calcChar1 ), calcChar1.Path );
			var calculatedValue1 = valueCalculator.CalculateValue( calcChar1, measurement, mathCalculator1 );
			var mathCalculator2 = mathInterpreter.Parse( GetFormula( calcChar2 ), calcChar2.Path );
			var calculatedValue2 = valueCalculator.CalculateValue( calcChar2, measurement, mathCalculator2 );

			//Then
			Assert.That( calculatedValue1, Is.EqualTo( expectedValue1 ).Using( new Func<DataCharacteristicDto, DataValueDto, bool>( CompareDataValues ) ) );
			Assert.That( calculatedValue2, Is.EqualTo( expectedValue2 ).Using( new Func<DataCharacteristicDto, DataValueDto, bool>( CompareDataValues ) ) );
		}

		[Test]
		public void Test_CalculateValuesForCalculatedCharacteristicsWhenAnyParameterIsNull()
		{
			//Given
			var mathInterpreter = new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
			var valueCalculator = new ValueCalculator( mathInterpreter, EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler );

			//When/Then
			Assert.That( () => valueCalculator.CalculateValuesForCalculatedCharacteristics( null!, Array.Empty<DataMeasurementDto>(), false ),
				Throws.ArgumentNullException );
			Assert.That( () => valueCalculator.CalculateValuesForCalculatedCharacteristics( Array.Empty<InspectionPlanDtoBase>(), null!, false ),
				Throws.ArgumentNullException );
		}

		[Test]
		public void Test_CalculateValuesForCalculatedCharacteristicsWhenAnyParameterIsEmpty()
		{
			//Given
			var measurements = new[] { new DataMeasurementDto() };
			var emptyMeasurements = Array.Empty<DataMeasurementDto>();
			var characteristics = new InspectionPlanDtoBase[] { new InspectionPlanCharacteristicDto() };
			var emptyCharacteristics = Array.Empty<InspectionPlanDtoBase>();

			var mathInterpreter = new MathInterpreter( EmptyCharacteristicCalculatorFactory, EmptyChildPathsHandler );
			var valueCalculator = new ValueCalculator( mathInterpreter, EmptyMeasurementValueHandler, EmptyEntityAttributeValueHandler );

			//When
			var calculationResult1 = valueCalculator.CalculateValuesForCalculatedCharacteristics( emptyCharacteristics, measurements, false );
			var calculationResult2 = valueCalculator.CalculateValuesForCalculatedCharacteristics( characteristics, emptyMeasurements, false );

			// Then
			Assert.That( calculationResult1, Is.Not.Null );
			Assert.That( calculationResult1.HasUpdatedCharacteristics, Is.False );
			Assert.That( calculationResult1.Exceptions, Is.Empty );
			Assert.That( calculationResult2, Is.Not.Null );
			Assert.That( calculationResult2.HasUpdatedCharacteristics, Is.False );
			Assert.That( calculationResult2.Exceptions, Is.Empty );
		}

		[Test]
		public void Test_CalculateValuesForCalculatedCharacteristicsWhenFormulaIsInvalidAndErrorIsThrownImmediately()
		{
			//Given
			var char1 = CreateCharacteristicWithoutFormula( "char1" );
			var calcChar1 = CreateCharacteristicWithFormula( "calcChar1", "}1 + 2" );
			var calcChar2 = CreateCharacteristicWithFormula( "calcChar2", "{calcChar1} + {char1}" );
			var calcChar3 = CreateCharacteristicWithFormula( "calcChar3", "1 + 2 + {char1}" );
			var characteristics = new InspectionPlanDtoBase[] { char1, calcChar1, calcChar2, calcChar3 };

			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, key ) => characteristics.FirstOrDefault( ch => ch.Path == path ).GetAttributeValue( key ),
				_ => ArraySegment<PathInformationDto>.Empty );
			var mathInterpreter = attributeBasedMathInterpreterFactory.GetInterpreter();
			var valueCalculator = new ValueCalculator( mathInterpreter, GetMeasurementValue, EmptyEntityAttributeValueHandler );

			var measurement = CreateEmptyMeasurement();
			measurement.Characteristics = new[]
			{
				CreateDataCharacteristic( char1, CreateDataValue( 4 ) ),
				CreateDataCharacteristic( calcChar1, CreateDataValueWithAttributes( null ) )
			};
			var measurements = new[] { measurement };

			//When/Then
			Assert.That( () => valueCalculator.CalculateValuesForCalculatedCharacteristics( characteristics, measurements, true ), Throws.Exception );
		}

		[Test]
		public void Test_CalculateValuesForCalculatedCharacteristicsWhenFormulaIsInvalidAndErrorsAreCollected()
		{
			//Given
			var char1 = CreateCharacteristicWithoutFormula( "char1" );
			var calcChar1 = CreateCharacteristicWithFormula( "calcChar1", "}1 + 2" );
			var calcChar2 = CreateCharacteristicWithFormula( "calcChar2", "{calcChar1} + {char1}" );
			var calcChar3 = CreateCharacteristicWithFormula( "calcChar3", "1 + 2 + {char1}" );
			var characteristics = new InspectionPlanDtoBase[] { char1, calcChar1, calcChar2, calcChar3 };

			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, key ) => characteristics.FirstOrDefault( ch => ch.Path == path ).GetAttributeValue( key ),
				_ => ArraySegment<PathInformationDto>.Empty );
			var mathInterpreter = attributeBasedMathInterpreterFactory.GetInterpreter();
			var valueCalculator = new ValueCalculator( mathInterpreter, GetMeasurementValue, EmptyEntityAttributeValueHandler );

			var measurement = CreateEmptyMeasurement();
			measurement.Characteristics = new[]
			{
				CreateDataCharacteristic( char1, CreateDataValue( 4 ) ),
				CreateDataCharacteristic( calcChar1, CreateDataValueWithAttributes( null ) )
			};
			var measurements = new[] { measurement };

			var expectedCharacteristics = new[]
			{
				CreateDataCharacteristic( char1, CreateDataValue( 4 ) ),
				CreateDataCharacteristic( calcChar1, CreateDataValueWithAttributes( null ) ),
				CreateDataCharacteristic( calcChar3, CreateDataValue( 7 ) )
			};

			//When
			var result = valueCalculator.CalculateValuesForCalculatedCharacteristics( characteristics, measurements, false );

			//Then
			Assert.That( result, Is.Not.Null );
			Assert.That( result.Exceptions, Has.Exactly( 1 ).Items );
			Assert.That( result.GetUpdatedCharacteristics( measurement.Uuid ),
				Is.EquivalentTo( expectedCharacteristics )
					.Using( new Func<DataCharacteristicDto, DataCharacteristicDto, bool>( CompareDataCharacteristics ) ) );
		}

		[Test]
		public void Test_CalculateValuesForCalculatedCharacteristics()
		{
			//Given
			var char1 = CreateCharacteristicWithoutFormula( "char1" );
			var calcChar1 = CreateCharacteristicWithFormula( "calcChar1", "1 + 2" );
			var calcChar2 = CreateCharacteristicWithFormula( "calcChar2", "{calcChar1} + {char1}" );
			var characteristics = new InspectionPlanDtoBase[] { char1, calcChar1, calcChar2 };

			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, key ) => characteristics.FirstOrDefault( ch => ch.Path == path ).GetAttributeValue( key ),
				_ => ArraySegment<PathInformationDto>.Empty );
			var mathInterpreter = attributeBasedMathInterpreterFactory.GetInterpreter();

			var valueCalculator = new ValueCalculator( mathInterpreter, GetMeasurementValue, EmptyEntityAttributeValueHandler );

			var measurement1 = CreateEmptyMeasurement();
			measurement1.Characteristics = new[]
			{
				CreateDataCharacteristic( char1, CreateDataValue( 4 ) ),
				CreateDataCharacteristic( calcChar1, CreateDataValueWithAttributes( null ) )
			};
			var measurement2 = CreateEmptyMeasurement();
			measurement2.Characteristics = new[]
			{
				CreateDataCharacteristic( char1, CreateDataValue( 12 ) ),
				CreateDataCharacteristic( calcChar1, CreateDataValueWithAttributes( null ) )
			};
			var measurements = new[] { measurement1, measurement2 };

			var expectedCharacteristicsForMeasurement1 = new[]
			{
				CreateDataCharacteristic( char1, CreateDataValue( 4 ) ),
				CreateDataCharacteristic( calcChar1, CreateDataValueWithAttributes( 3 ) ),
				CreateDataCharacteristic( calcChar2, CreateDataValue( 7 ) )
			};

			var expectedCharacteristicsForMeasurement2 = new[]
			{
				CreateDataCharacteristic( char1, CreateDataValue( 12 ) ),
				CreateDataCharacteristic( calcChar1, CreateDataValueWithAttributes( 3 ) ),
				CreateDataCharacteristic( calcChar2, CreateDataValue( 15 ) )
			};

			//When
			var result = valueCalculator.CalculateValuesForCalculatedCharacteristics( characteristics, measurements, false );

			//Then
			Assert.That( result, Is.Not.Null );
			Assert.That( result.Exceptions, Is.Empty );
			Assert.That( result.GetUpdatedCharacteristics( measurement1.Uuid ),
				Is.EquivalentTo( expectedCharacteristicsForMeasurement1 )
					.Using( new Func<DataCharacteristicDto, DataCharacteristicDto, bool>( CompareDataCharacteristics ) ) );
			Assert.That( result.GetUpdatedCharacteristics( measurement2.Uuid ),
				Is.EquivalentTo( expectedCharacteristicsForMeasurement2 )
					.Using( new Func<DataCharacteristicDto, DataCharacteristicDto, bool>( CompareDataCharacteristics ) ) );
		}

		private static string GetFormula( InspectionPlanCharacteristicDto characteristic )
		{
			var formula = characteristic.GetAttributeValue( WellKnownKeys.Characteristic.LogicalOperationString );
			return formula ?? string.Empty;
		}

		private static double? GetMeasurementValue( DataMeasurementDto measurement, PathInformationDto path )
		{
			return measurement.Characteristics.FirstOrDefault( ch => ch.Path == path )?.Value.MeasuredValue;
		}

		private static InspectionPlanCharacteristicDto CreateCharacteristicWithoutFormula( string name )
		{
			return new InspectionPlanCharacteristicDto
			{
				Uuid = Guid.NewGuid(),
				Path = new PathInformationDto( new PathElementDto( InspectionPlanEntityDto.Characteristic, name ) ),
			};
		}

		private static InspectionPlanCharacteristicDto CreateCharacteristicWithFormula( string name, string formula )
		{
			return new InspectionPlanCharacteristicDto
			{
				Uuid = Guid.NewGuid(),
				Path = new PathInformationDto( new PathElementDto( InspectionPlanEntityDto.Characteristic, name ) ),
				Attributes = new[] { new AttributeDto( WellKnownKeys.Characteristic.LogicalOperationString, formula ) }
			};
		}

		private static DataMeasurementDto CreateEmptyMeasurement()
		{
			return new DataMeasurementDto
			{
				Uuid = Guid.NewGuid(),
				PartUuid = Guid.Empty,
				Created = new DateTime( 2018, 11, 11, 11, 11, 11, DateTimeKind.Local ),
				LastModified = new DateTime( 2018, 11, 11, 11, 11, 11, DateTimeKind.Local ),
				Time = new DateTime( 2018, 11, 11, 11, 11, 11, DateTimeKind.Local ),
			};
		}

		private static DataValueDto CreateDataValue( double measuredValue )
		{
			return new DataValueDto( new[] { new AttributeDto( WellKnownKeys.Value.MeasuredValue, measuredValue ) } );
		}

		private static DataValueDto CreateDataValueWithAttributes( double? measuredValue )
		{
			var attributes = new List<AttributeDto>
			{
				new AttributeDto( WellKnownKeys.Value.AggregatedValueCount, 1 ),
				new AttributeDto( WellKnownKeys.Value.AggregatedRange, 0 )
			};

			if( measuredValue.HasValue )
				attributes.Add( new AttributeDto( WellKnownKeys.Value.MeasuredValue, measuredValue ) );

			return new DataValueDto( attributes.ToArray() );
		}

		private static DataCharacteristicDto CreateDataCharacteristic( InspectionPlanCharacteristicDto characteristic, DataValueDto value )
		{
			return new DataCharacteristicDto
			{
				Uuid = characteristic.Uuid,
				Path = characteristic.Path,
				Attributes = characteristic.Attributes,
				Comment = characteristic.Comment,
				Version = characteristic.Version,
				Timestamp = characteristic.Timestamp,
				Value = value
			};
		}

		private static bool CompareDataValues( DataCharacteristicDto x, DataValueDto y )
		{
			if( x == null )
				return false;

			if( x.Value == null || y == null )
				return ReferenceEquals( x.Value, y );

			return CompareAttributes( x.Value.Attributes, y.Attributes );
		}

		private static bool CompareDataCharacteristics( DataCharacteristicDto x, DataCharacteristicDto y )
		{
			if( x == null || y == null )
				return ReferenceEquals( x, y );

			return x.Uuid == y.Uuid
				&& CompareAttributes( x.Attributes, y.Attributes )
				&& CompareAttributes( x.Value.Attributes, y.Value.Attributes );
		}

		private static bool CompareAttributes( IReadOnlyList<AttributeDto> first, IReadOnlyList<AttributeDto> second )
		{
			if( first == null )
				throw new ArgumentNullException( nameof( first ) );

			if( second == null )
				throw new ArgumentNullException( nameof( second ) );

			if( ReferenceEquals( first, second ) )
				return true;

			if( first.Count != second.Count )
				return false;

			var hashSet = first.ToHashSet( AttributeComparer.Default );
			return hashSet.SetEquals( second );
		}

		#endregion
	}
}