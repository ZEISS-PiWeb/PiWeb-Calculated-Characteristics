#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2026                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions;

using System;
using System.Globalization;

/// <summary>
/// Delegate for retrieving attribute values.
/// </summary>
internal delegate object? AttributeHandler( ushort key );

internal static class AttributeReader
{
	/// <summary>
	/// Retrieves the value of an attribute as a double, if possible.
	/// The method attempts to convert the attribute value to a double,
	/// supporting values of type double, string (parsed as a double),
	/// or other numeric types (converted to double).
	/// </summary>
	/// <param name="attributeHandler">A delegate used to retrieve the attribute value by its key.</param>
	/// <param name="attributeKey">The key of the attribute to retrieve.</param>
	/// <returns>
	/// The attribute value as a double, or null if the value is not convertible to a double
	/// or if the attribute does not exist.
	/// </returns>
	public static double? GetDoubleAttributeValue( AttributeHandler attributeHandler, ushort attributeKey )
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
}