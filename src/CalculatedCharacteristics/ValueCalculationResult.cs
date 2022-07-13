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
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Responsible for collecting the results of the value calculation for calculated characteristics for multiple measurements
	/// and for applying the values to the measurements.
	/// </summary>
	public class ValueCalculationResult
	{
		#region members

		private readonly IReadOnlyCollection<DataMeasurementDto> _Measurements;

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
		public ValueCalculationResult( [NotNull] IReadOnlyCollection<DataMeasurementDto> measurements )
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
		internal void SetUpdatedCharacteristic( Guid measurementUuid, Guid characteristicUuid, [CanBeNull] DataCharacteristicDto newValue )
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
		public IReadOnlyCollection<DataCharacteristicDto> GetUpdatedCharacteristics( Guid measurementUuid )
		{
			if( !_ChangeSets.TryGetValue( measurementUuid, out var changeSet ) )
				return Array.Empty<DataCharacteristicDto>();

			return changeSet.GetUpdatedCharacteristics();
		}

		/// <summary>
		/// Adds an exception to the list of exceptions that occurred while calculating values for calculated characteristics.
		/// </summary>
		/// <param name="exception">The exception to add.</param>
		internal void AddException( [NotNull] Exception exception )
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

		private class CharacteristicsChangeSet
		{
			#region members

			private readonly IReadOnlyCollection<DataCharacteristicDto> _OriginalCharacteristics;
			private Dictionary<Guid, DataCharacteristicDto> _ChangedCharacteristics;

			#endregion

			#region constructors

			public CharacteristicsChangeSet( IReadOnlyCollection<DataCharacteristicDto> characteristics = null )
			{
				_OriginalCharacteristics = characteristics ?? Array.Empty<DataCharacteristicDto>();
			}

			#endregion

			#region properties

			public bool HasChanges => _ChangedCharacteristics != null;

			#endregion

			#region methods

			public void SetValue( Guid characteristicUuid, [CanBeNull] DataCharacteristicDto newValue )
			{
				if( !HasChanges )
					_ChangedCharacteristics = _OriginalCharacteristics.ToDictionary( ch => ch.Uuid );

				_ChangedCharacteristics[ characteristicUuid ] = newValue;
			}

			public IReadOnlyCollection<DataCharacteristicDto> GetUpdatedCharacteristics()
			{
				if( !HasChanges )
					return _OriginalCharacteristics;

				return _ChangedCharacteristics.Values.Where( c => c != null ).ToArray();
			}

			#endregion
		}

		#endregion
	}
}