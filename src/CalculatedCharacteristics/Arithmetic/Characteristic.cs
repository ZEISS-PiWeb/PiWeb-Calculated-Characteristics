#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic
{
	#region usings

	using System.Collections.Generic;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Represents a characteristic parsed from a formula.
	/// </summary>
	public class Characteristic : MathElement
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Characteristic(
			int startPosition,
			int length,
			string text,
			PathInformationDto path,
			ushort? attrKey ) : base( startPosition, length )
		{
			Text = text;
			Path = path;
			AttributeKey = attrKey;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the text from the formula that represents the characteristic.
		/// </summary>
		public string Text { get; }

		/// <summary>
		/// Gets the path of the characteristic.
		/// </summary>
		public PathInformationDto Path { get; }

		/// <summary>
		/// Gets the key of the referenced characteristic attribute.
		/// </summary>
		public ushort? AttributeKey { get; }

		#endregion

		#region methods

		/// <inheritdoc />
		public override double? GetResult( ICharacteristicValueResolver resolver )
		{
			var result = AttributeKey.HasValue
				? resolver.GetEntityAttributeValue<double?>( Path, AttributeKey.Value )
				: resolver.GetMeasurementValue( Path );

			return result;
		}

		/// <inheritdoc/>
		protected override IEnumerable<MathDependencyInformation> OnCheckForDependentCharacteristics( ICharacteristicInfoResolver resolver )
		{
			yield return new MathDependencyInformation( Path, TokenStartPosition, TokenLength, Text, AttributeKey );
		}

		#endregion
	}
}