﻿#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Syntax
{
	/// <summary>
	/// Token indicating the end of the token stream.
	/// </summary>
	internal class FinalToken : Token
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="FinalToken"/>.
		/// </summary>
		public FinalToken( int position ) : base( position, string.Empty )
		{ }

		#endregion

		#region methods

		/// <inheritdoc/>
		public override void Accept( IMathVisitor visitor )
		{
			visitor.VisitFinal( this );
		}

		#endregion
	}
}