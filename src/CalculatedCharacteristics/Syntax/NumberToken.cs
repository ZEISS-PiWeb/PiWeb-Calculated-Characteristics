#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Syntax
{
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	/// <summary>
	/// Token indicating function.
	/// </summary>
	internal class NumberToken : Token
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="NumberToken"/>.
		/// </summary>
		public NumberToken( MathElement value, int position, string tokenString ) : base( position, tokenString )
		{
			Value = value;
		}

		#endregion

		#region properties

		public MathElement Value { get; }

		#endregion

		#region methods

		/// <inheritdoc/>
		public override void Accept( IMathVisitor visitor )
		{
			visitor.VisitNumber( this );
		}

		#endregion
	}
}