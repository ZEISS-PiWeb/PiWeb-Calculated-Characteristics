#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Contracts;

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
		double? GetMeasurementValue( [NotNull] PathInformation path );

		#endregion
	}
}