#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Syntax
{
	#region usings

	using System;

	#endregion

	/// <summary>
	/// Token indicating a terminal.
	/// </summary>
	public class TerminalToken : Token
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="TerminalToken"/>.
		/// </summary>
		public TerminalToken( int position, string tokenString ) : base( position, tokenString )
		{
			if( tokenString?.Length != 1 )
				throw new ArgumentException( "Terminal must be single character", nameof( tokenString ) );
		}

		#endregion

		#region methods

		/// <inheritdoc/>
		public override void Accept( IMathVisitor visitor )
		{
			visitor.VisitTerminal( this );
		}

		#endregion
	}
}