#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic
{
	/// <summary>
	/// Represents a constant parsed from a formula.
	/// </summary>
	public class Constant : Number
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="number">The numerical value of the constant.</param>
		/// <param name="constString">The string representation of the constant.</param>
		public Constant( double number, string constString ) : base( 0, 0, number )
		{
			ConstString = constString;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the string representation of the constant.
		/// </summary>
		public string ConstString { get; }

		#endregion
	}
}