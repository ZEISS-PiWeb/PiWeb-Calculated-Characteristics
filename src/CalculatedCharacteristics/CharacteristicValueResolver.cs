#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2016                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using Zeiss.PiWeb.Api.Core;

	#endregion

	/// <inheritdoc />
	internal class CharacteristicValueResolver : ICharacteristicValueResolver
	{
		#region members

		private readonly CharacteristicCalculatorFactory _CharacteristicCalculatorFactory;
		private readonly ChildPathsHandler _ChildPathsHandler;
		private readonly MeasurementValueHandler _MeasurementValueHandler;
		private readonly EntityAttributeValueHandler _EntityAttributeValueHandler;
		private readonly DateTime? _Timestamp;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="CharacteristicValueResolver"/>.
		/// </summary>
		/// <param name="characteristicCalculatorFactory">The delegate to provide the calculator for a calculated characteristic.</param>
		/// <param name="childPathsHandler">The delegate to get the child entities of an entity.</param>
		/// <param name="measurementValueHandler">The delegate to get a value for a characteristic.</param>
		/// <param name="entityAttributeValueHandler">The delegate to get a value for a characteristic attribute.</param>
		/// <param name="sourcePath">The path of the formula owner.</param>
		/// <param name="timestamp">The time of the measurement to get values for.</param>
		public CharacteristicValueResolver(
			CharacteristicCalculatorFactory characteristicCalculatorFactory,
			ChildPathsHandler childPathsHandler,
			MeasurementValueHandler measurementValueHandler,
			EntityAttributeValueHandler entityAttributeValueHandler,
			PathInformation? sourcePath,
			DateTime? timestamp = null )
		{
			_CharacteristicCalculatorFactory = characteristicCalculatorFactory;
			_ChildPathsHandler = childPathsHandler;
			_MeasurementValueHandler = measurementValueHandler;
			_EntityAttributeValueHandler = entityAttributeValueHandler;
			SourcePath = sourcePath;
			_Timestamp = timestamp;
		}

		#endregion

		#region interface ICharacteristicValueResolver

		/// <inheritdoc />
		public PathInformation? SourcePath { get; }

		/// <inheritdoc />
		public object? GetEntityAttributeValue( PathInformation path, ushort attrKey )
		{
			return _EntityAttributeValueHandler( path, attrKey, _Timestamp );
		}

		/// <inheritdoc />
		public IEnumerable<PathInformation> GetChildPaths( PathInformation parent )
		{
			return _ChildPathsHandler( parent );
		}

		/// <inheritdoc />
		public double? GetMeasurementValue( PathInformation path )
		{
			try
			{
				var calculator = _CharacteristicCalculatorFactory( path );
				if( calculator != null )
				{
					// characteristic is a calculated characteristic
					CharacteristicDependencyResolver.ValidateDependencies( path, _CharacteristicCalculatorFactory, _EntityAttributeValueHandler );
					return calculator.GetResult( _MeasurementValueHandler, _EntityAttributeValueHandler, _Timestamp );
				}
			}
			catch
			{
				// Exceptions while parsing the formula or calculating the formula value can be ignored.
				return null;
			}

			// normal (measured) characteristic
			return _MeasurementValueHandler( path );
		}

		#endregion
	}
}