#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
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
	internal class FunctionToken : Token
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="FunctionToken"/>.
		/// </summary>
		public FunctionToken( MathOperation operation, int position, string tokenString ) : base( position, tokenString )
		{
			Value = operation;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the <see cref="MathElement"/> associated with the token.
		/// </summary>
		public MathOperation Value { get; }

		#endregion

		#region methods

		/// <inheritdoc/>
		public override void Accept( IMathVisitor visitor )
		{
			visitor.VisitFunction( this );
		}

		#endregion
	}
}