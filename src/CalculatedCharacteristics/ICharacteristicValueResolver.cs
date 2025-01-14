#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using Zeiss.PiWeb.Api.Core;

	#endregion

	/// <summary>
	/// Responsible for resolving the value of characteristics and attributes when calculate the formula value.
	/// </summary>
	public interface ICharacteristicValueResolver : ICharacteristicInfoResolver
	{
		#region methods

		/// <summary>
		/// Gets the measurement value for a characteristic.
		/// </summary>
		/// <param name="path">The path of the characteristic.</param>
		/// <returns>The measured or calculated value.</returns>
		double? GetMeasurementValue( PathInformation path );

		#endregion
	}
}