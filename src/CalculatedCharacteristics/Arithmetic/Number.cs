#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic
{
	/// <summary>
	/// Represents a number parsed from a formula.
	/// </summary>
	public class Number : MathElement
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		internal Number( int startPosition, int length, double figure ) : base( startPosition, length )
		{
			Figure = figure;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the represented figure.
		/// </summary>
		public double Figure { get; }

		#endregion

		#region methods

		/// <inheritdoc />
		public override double? GetResult( ICharacteristicValueResolver resolver )
		{
			return Figure;
		}

		#endregion
	}
}