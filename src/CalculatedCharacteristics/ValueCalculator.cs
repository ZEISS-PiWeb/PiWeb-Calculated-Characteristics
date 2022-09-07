#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Responsible for calculating the values for calculated characteristics (aka characteristics with formula).
	/// </summary>
	public class ValueCalculator
	{
		#region members

		[NotNull] private readonly MathInterpreter _MathInterpreter;
		[NotNull] private readonly MeasurementValueHandler _MeasurementValueHandler;
		[NotNull] private readonly EntityAttributeValueHandler _EntityAttributeValueHandler;
		[CanBeNull] private readonly ParentPartCheckHandler _ParentPartCheckHandler;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="ValueCalculator"/>.
		/// </summary>
		/// <param name="mathInterpreter">The math interpreter that parses the characteristics formula.</param>
		/// <param name="measurementValueHandler">A handler to get the measurement value for a characteristic.</param>
		/// <param name="entityAttributeValueHandler">A handler to get the attribute value for a characteristic.</param>
		/// <param name="parentPartCheckHandler">An optional handler to check whether an characteristic belongs to a part.</param>
		public ValueCalculator(
			[NotNull] MathInterpreter mathInterpreter,
			[NotNull] MeasurementValueHandler measurementValueHandler,
			[NotNull] EntityAttributeValueHandler entityAttributeValueHandler,
			[CanBeNull] ParentPartCheckHandler parentPartCheckHandler = null )
		{
			_MathInterpreter = mathInterpreter ?? throw new ArgumentNullException( nameof( mathInterpreter ) );
			_MeasurementValueHandler = measurementValueHandler ?? throw new ArgumentNullException( nameof( measurementValueHandler ) );
			_EntityAttributeValueHandler = entityAttributeValueHandler ?? throw new ArgumentNullException( nameof( entityAttributeValueHandler ) );

			_ParentPartCheckHandler = parentPartCheckHandler;
		}

		/// <summary>
		/// Creates a new instance of <see cref="ValueCalculator"/>.
		/// </summary>
		/// <param name="childPathsHandler">A handler to resolve child paths of a given path.</param>
		/// <param name="measurementValueHandler">A handler to get the measurement value for a characteristic.</param>
		/// <param name="entityAttributeValueHandler">A handler to get the attribute value for a characteristic.</param>
		/// <param name="parentPartCheckHandler">An optional handler to check whether an characteristic belongs to a part.</param>
		public ValueCalculator(
			[NotNull] ChildPathsHandler childPathsHandler,
			[NotNull] EntityAttributeValueHandler entityAttributeValueHandler,
			[NotNull] MeasurementValueHandler measurementValueHandler,
			[CanBeNull] ParentPartCheckHandler parentPartCheckHandler = null )
			: this(
				CreateFromAttributeHandler( childPathsHandler, entityAttributeValueHandler ),
				measurementValueHandler,
				entityAttributeValueHandler,
				parentPartCheckHandler )
		{
		}

		#endregion

		#region methods

		private static MathInterpreter CreateFromAttributeHandler( ChildPathsHandler childPathsHandler, EntityAttributeValueHandler entityAttributeValueHandler )
		{
			return new AttributeBasedMathInterpreterFactory( ( path, key ) => entityAttributeValueHandler( path, key, null ), childPathsHandler ).GetInterpreter();
		}

		/// <summary>
		/// Parses the given formula.
		/// Does not calculate the value of the formula.
		/// </summary>
		/// <param name="formula">The formula to be parsed.</param>
		/// <param name="characteristicPath">The characteristic the formula belongs to.</param>
		/// <exception cref="ParserException">Thrown if the formula contains an error.</exception>
		/// <returns>A <see cref="IMathCalculator"/> that can be used to calculate the value of the formula.</returns>
		[NotNull]
		public IMathCalculator Parse( [NotNull] string formula, [CanBeNull] PathInformationDto characteristicPath )
		{
			if( formula == null ) throw new ArgumentNullException( nameof( formula ) );

			return _MathInterpreter.Parse( formula, characteristicPath );
		}

		/// <summary>
		/// Calculates the value of the calculated characteristic for a given measurement.
		/// Returns <code>null</code> if the characteristic does not contain a formula.
		/// </summary>
		/// <param name="characteristic">The calculated characteristic.</param>
		/// <param name="measurement">The measurement to calculate the value for.</param>
		/// <param name="calculator">The <see cref="IMathCalculator"/> to calculate the value.</param>
		/// <returns>The calculated value.</returns>
		[CanBeNull]
		public DataValueDto? CalculateValue(
			[CanBeNull] InspectionPlanDtoBase characteristic,
			[CanBeNull] DataMeasurementDto measurement,
			[CanBeNull] IMathCalculator calculator )
		{
			if( calculator == null || characteristic == null || measurement == null )
				return null;

			// Calculate value only if the characteristic belongs to the part of the measurement
			if( _ParentPartCheckHandler != null && !_ParentPartCheckHandler( characteristic, measurement.PartUuid ) )
				return null;

			var calculatedValue = calculator.GetResult(
				charPath => _MeasurementValueHandler( measurement, charPath ),
				_EntityAttributeValueHandler,
				measurement.Time );

			var existingDataValue = measurement.Characteristics.TryGetValue( characteristic.Uuid, out var value )
				? value
				: (DataValueDto?)null;

			return CreateDataValueForCalculatedValue( calculatedValue, existingDataValue );
		}

		/// <summary>
		/// Calculates the values of calculated characteristics for the given measurements.
		/// </summary>
		/// <param name="calculatedCharacteristics">The calculated characteristics.</param>
		/// <param name="measurements">The measurements to calculate the values for.</param>
		/// <param name="throwError">A flag whether to throw the first occurring error.
		/// If the flag is set to <code>false</code>, the occuring errors are collected
		/// and returned as part of the <see cref="ValueCalculationResult"/>.</param>
		/// <returns>A <see cref="ValueCalculationResult"/> containing the calculated values and occurred errors.</returns>
		[NotNull]
		public ValueCalculationResult CalculateValuesForCalculatedCharacteristics(
			[NotNull] IReadOnlyCollection<InspectionPlanDtoBase> calculatedCharacteristics,
			[NotNull] IReadOnlyCollection<DataMeasurementDto> measurements,
			bool throwError )
		{
			if( calculatedCharacteristics == null ) throw new ArgumentNullException( nameof( calculatedCharacteristics ) );
			if( measurements == null ) throw new ArgumentNullException( nameof( measurements ) );

			if( measurements.Count == 0 || calculatedCharacteristics.Count == 0 )
				return new ValueCalculationResult();

			var result = new ValueCalculationResult( measurements );

			foreach( var characteristic in calculatedCharacteristics )
			{
				var formula = characteristic.GetAttributeValue( WellKnownKeys.Characteristic.LogicalOperationString );
				if( string.IsNullOrEmpty( formula ) )
					continue;

				try
				{
					var calculator = Parse( formula, characteristic.Path );

					foreach( var measurement in measurements )
					{
						var newDataValue = CalculateValue( characteristic, measurement, calculator );
						result.SetUpdatedCharacteristic( measurement.Uuid, characteristic.Uuid, newDataValue );
					}
				}
				catch( Exception ex )
				{
					var error = new Exception(
						LocalizationHelper.Format( typeof( ValueCalculator ), "FormulaParsingError", characteristic.Path, ex.Message ), ex );

					if( throwError )
						throw error;

					result.AddException( error );

					// Remove already existing value for the calculated characteristic
					foreach( var measurement in measurements )
					{
						var existingDataValue = measurement.Characteristics.TryGetValue( characteristic.Uuid, out var value )
							? value
							: (DataValueDto?)null;

						// Remove the measured value only and keep the DataCharacteristic if it still contains other attribute values
						var newDataValue = CreateDataValueForCalculatedValue( null, existingDataValue );
						result.SetUpdatedCharacteristic( measurement.Uuid, characteristic.Uuid, newDataValue );
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Creates a DataValueDto for the calculated value of the given characteristic.
		/// If an <paramref name="existingDataValue"/> is available for the characteristic,
		/// its attribute values (besides the measured value) are copied into the new DataValueDto.
		/// </summary>
		[CanBeNull]
		private static DataValueDto? CreateDataValueForCalculatedValue( double? calculatedValue, DataValueDto? existingDataValue )
		{
			// Copy attributes from existing DataCharacteristic
			IEnumerable<AttributeDto> valueAttributes = existingDataValue?.Attributes ?? Array.Empty<AttributeDto>();

			// Remove measured value because it has been recalculated
			valueAttributes = valueAttributes.Where( attr => attr.Key != WellKnownKeys.Value.MeasuredValue );

			// Add calculated value to the attributes
			if( calculatedValue.HasValue )
				valueAttributes = valueAttributes.Prepend( new AttributeDto( WellKnownKeys.Value.MeasuredValue, calculatedValue.Value ) );

			var valueAttributesArray = valueAttributes.ToArray();
			if( valueAttributesArray.Length == 0 )
				return null;

			return new DataValueDto( valueAttributesArray );
		}

		#endregion

		/// <summary>
		/// Delegate to check, whether a characteristic belongs to a part.
		/// </summary>
		public delegate bool ParentPartCheckHandler( InspectionPlanDtoBase characteristic, Guid partUuid );

		/// <summary>
		/// Delegate to get a measured value for a characteristic.
		/// </summary>
		public delegate double? MeasurementValueHandler( DataMeasurementDto measurement, PathInformationDto characteristicPath );
	}
}