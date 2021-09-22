﻿#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2016                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <inheritdoc />
	internal class CharacteristicValueResolver : ICharacteristicValueResolver
	{
		#region members

		[NotNull] private readonly CharacteristicCalculatorFactory _CharacteristicCalculatorFactory;
		[NotNull] private readonly ChildPathsHandler _ChildPathsHandler;
		[NotNull] private readonly MeasurementValueHandler _MeasurementValueHandler;
		[NotNull] private readonly EntityAttributeValueHandler _EntityAttributeValueHandler;
		[CanBeNull] private readonly DateTime? _Timestamp;

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
			[NotNull] CharacteristicCalculatorFactory characteristicCalculatorFactory,
			[NotNull] ChildPathsHandler childPathsHandler,
			[NotNull] MeasurementValueHandler measurementValueHandler,
			[NotNull] EntityAttributeValueHandler entityAttributeValueHandler,
			[CanBeNull] PathInformationDto sourcePath,
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
		public PathInformationDto SourcePath { get; }

		/// <inheritdoc />
		public object GetEntityAttributeValue( PathInformationDto path, ushort attrKey )
		{
			return _EntityAttributeValueHandler( path, attrKey, _Timestamp );
		}

		/// <inheritdoc />
		public IEnumerable<PathInformationDto> GetChildPaths( PathInformationDto parent )
		{
			return _ChildPathsHandler( parent );
		}

		/// <inheritdoc />
		public double? GetMeasurementValue( PathInformationDto path )
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