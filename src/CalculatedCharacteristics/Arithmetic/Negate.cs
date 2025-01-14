#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic
{
	#region usings

	using System.Collections.Generic;
	using System.Linq;

	#endregion

	/// <summary>
	/// Represents the negation of a given <see cref="MathElement"/>.
	/// </summary>
	public class Negate : MathElement
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		internal Negate( MathElement inner, int position ) : base( position, 1, new List<MathElement> { inner } )
		{ }

		#endregion

		#region methods

		/// <inheritdoc/>
		public override double? GetResult( ICharacteristicValueResolver resolver )
		{
			return -1 * Arguments.First().GetResult( resolver );
		}

		#endregion
	}
}