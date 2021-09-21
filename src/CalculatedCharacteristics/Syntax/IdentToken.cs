#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Syntax
{
	/// <summary>
	/// Token indicating a text.
	/// </summary>
	public class IdentToken : Token
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="IdentToken"/>.
		/// </summary>
		public IdentToken( int position, string tokenString ) : base( position, tokenString )
		{ }

		#endregion

		#region methods

		/// <inheritdoc/>
		public override void Accept( IMathVisitor visitor )
		{
			visitor.VisitIdent( this );
		}

		#endregion
	}
}