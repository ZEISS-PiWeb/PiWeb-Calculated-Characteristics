#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	#region usings

	using System;
	using System.Globalization;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Definitions;

	#endregion

	/// <summary>
	/// Provides access to the attribute values by a delegate.
	/// </summary>
	internal static class ToleranceProvider
	{
		#region members

		/// <summary>
		/// Delegate for retrieving attribute values.
		/// </summary>
		[CanBeNull]
		public delegate object AttributeHandler( ushort key );

		#endregion

		#region methods

		private static double? GetDoubleAttributeValue( AttributeHandler attributeHandler, ushort attributeKey )
		{
			var value = attributeHandler( attributeKey );
			if( value == null )
				return null;

			switch( value )
			{
				case double doubleValue:
					return doubleValue;

				case string stringValue:
					if( double.TryParse( stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue ) )
						return parsedValue;

					return null;

				default:
					try
					{
						// Needed to be able to return a double value for a boxed integer value
						return Convert.ToDouble( value, CultureInfo.InvariantCulture );
					}
					catch
					{
						return null;
					}
			}
		}

		public static Tolerance GetTolerance( AttributeHandler attributeHandler )
		{
			var lower = GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.LowerSpecificationLimit );
			var upper = GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.UpperSpecificationLimit );

			if( !lower.HasValue && !upper.HasValue )
			{
				var nominal = GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.NominalValue );

				lower = nominal + GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.LowerTolerance );
				upper = nominal + GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.UpperTolerance );
			}

			return new Tolerance( lower, upper );
		}

		#endregion
	}
}