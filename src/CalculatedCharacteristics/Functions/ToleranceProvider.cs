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

	using Zeiss.PiWeb.Api.Definitions;

	#endregion

	/// <summary>
	/// Provides access to the attribute values by a delegate.
	/// </summary>
	internal static class ToleranceProvider
	{
		#region methods

		public static Tolerance GetTolerance( AttributeHandler attributeHandler )
		{
			var lower = AttributeReader.GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.LowerSpecificationLimit );
			var upper = AttributeReader.GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.UpperSpecificationLimit );

			if( !lower.HasValue && !upper.HasValue )
			{
				var nominal = AttributeReader.GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.NominalValue );

				lower = nominal + AttributeReader.GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.LowerTolerance );
				upper = nominal + AttributeReader.GetDoubleAttributeValue( attributeHandler, WellKnownKeys.Characteristic.UpperTolerance );
			}

			return new Tolerance( lower, upper );
		}

		#endregion
	}
}