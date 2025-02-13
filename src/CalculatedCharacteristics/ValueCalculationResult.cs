﻿#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
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
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Responsible for collecting the results of the value calculation for calculated characteristics for multiple measurements
	/// and for applying the values to the measurements.
	/// </summary>
	public class ValueCalculationResult
	{
		#region members

		private readonly IReadOnlyCollection<DataMeasurementDto>? _Measurements;

		// Key: measurementUuid
		private readonly Dictionary<Guid, CharacteristicsChangeSet> _ChangeSets = new Dictionary<Guid, CharacteristicsChangeSet>();
		private readonly List<Exception> _Exceptions = new List<Exception>();

		#endregion

		#region constructors

		/// <summary>
		/// Creates an empty <see cref="ValueCalculationResult"/>.
		/// </summary>
		public ValueCalculationResult()
		{ }

		/// <summary>
		/// Creates a <see cref="ValueCalculationResult"/> for the given measurements.
		/// </summary>
		/// <param name="measurements"></param>
		public ValueCalculationResult( IReadOnlyCollection<DataMeasurementDto> measurements )
		{
			_Measurements = measurements;

			foreach( var measurement in _Measurements )
				_ChangeSets[ measurement.Uuid ] = new CharacteristicsChangeSet( measurement.Characteristics );
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the exceptions that occurred while calculating values for calculated characteristics.
		/// </summary>
		public IReadOnlyList<Exception> Exceptions => _Exceptions;

		/// <summary>
		/// Gets whether the <see cref="ValueCalculationResult"/> contains updated values for calculated characteristics.
		/// </summary>
		public bool HasUpdatedCharacteristics => _ChangeSets.Count > 0;

		#endregion

		#region methods

		/// <summary>
		/// Assigns a calculated value to a characteristic and measurement.
		/// </summary>
		/// <param name="measurementUuid">The id of the measurement the value was calculated for.</param>
		/// <param name="characteristicUuid">The id of the calculated characteristic the value was calculated for.</param>
		/// <param name="newValue">The calculated value.</param>
		internal void SetUpdatedCharacteristic( Guid measurementUuid, Guid characteristicUuid, DataValueDto? newValue )
		{
			if( !_ChangeSets.TryGetValue( measurementUuid, out var changeSet ) )
			{
				throw new ArgumentException( @$"Measurement with id '{measurementUuid}' could not be found.", nameof( measurementUuid ) );
			}

			changeSet.SetValue( characteristicUuid, newValue );
		}

		/// <summary>
		/// Gets the measured and calculated values for a measurement.
		/// </summary>
		/// <param name="measurementUuid">The id of the measurement to get the values for.</param>
		/// <returns>The array of values.</returns>
		public IReadOnlyDictionary<Guid, DataValueDto> GetUpdatedCharacteristics( Guid measurementUuid )
		{
			if( !_ChangeSets.TryGetValue( measurementUuid, out var changeSet ) )
				return new Dictionary<Guid, DataValueDto>();

			return changeSet.GetUpdatedCharacteristics();
		}

		/// <summary>
		/// Adds an exception to the list of exceptions that occurred while calculating values for calculated characteristics.
		/// </summary>
		/// <param name="exception">The exception to add.</param>
		internal void AddException( Exception exception )
		{
			_Exceptions.Add( exception );
		}

		/// <summary>
		/// Applies the updated list of measured and calculated values to the measurements.
		/// </summary>
		public void ApplyUpdatedCharacteristicsToMeasurements()
		{
			if( _Measurements == null )
				return;

			foreach( var measurement in _Measurements )
			{
				if( _ChangeSets.TryGetValue( measurement.Uuid, out var changeSet ) && changeSet.HasChanges )
					measurement.Characteristics = changeSet.GetUpdatedCharacteristics();
			}
		}

		#endregion

		#region class CharacteristicsChangeSet

		private sealed class CharacteristicsChangeSet
		{
			#region members

			private readonly IReadOnlyDictionary<Guid, DataValueDto> _OriginalCharacteristics;
			private Dictionary<Guid, DataValueDto>? _ChangedCharacteristics;

			#endregion

			#region constructors

			public CharacteristicsChangeSet( IReadOnlyDictionary<Guid, DataValueDto>? characteristics = null )
			{
				_OriginalCharacteristics = characteristics ?? new Dictionary<Guid, DataValueDto>();
			}

			#endregion

			#region properties

			public bool HasChanges => _ChangedCharacteristics != null;

			#endregion

			#region methods

			public void SetValue( Guid characteristicUuid, DataValueDto? newValue )
			{
				_ChangedCharacteristics ??= _OriginalCharacteristics.ToDictionary( c => c.Key, c => c.Value );

				if( newValue is not null )
					_ChangedCharacteristics[ characteristicUuid ] = newValue;
				else
					_ChangedCharacteristics.Remove( characteristicUuid );
			}

			public IReadOnlyDictionary<Guid, DataValueDto> GetUpdatedCharacteristics()
			{
				return _ChangedCharacteristics ?? _OriginalCharacteristics;
			}

			#endregion
		}

		#endregion
	}
}