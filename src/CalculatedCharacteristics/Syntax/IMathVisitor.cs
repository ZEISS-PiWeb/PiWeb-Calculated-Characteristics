#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Syntax
{
	/// <summary>
	/// Implement this interface to provide a visitor for <see cref="Token"/>.
	/// </summary>
	internal interface IMathVisitor
	{
		#region methods

		/// <summary>
		/// Handle token <see cref="FunctionToken"/>.
		/// </summary>
		void VisitFunction( FunctionToken token );

		/// <summary>
		/// Handle token <see cref="NumberToken"/>.
		/// </summary>
		void VisitNumber( NumberToken token );

		/// <summary>
		/// Handle token <see cref="TerminalToken"/>.
		/// </summary>
		void VisitTerminal( TerminalToken token );

		/// <summary>
		/// Handle token <see cref="IdentToken"/>.
		/// </summary>
		void VisitIdent( IdentToken token );

		/// <summary>
		/// Handle token <see cref="FinalToken"/>.
		/// </summary>
		void VisitFinal( FinalToken token );

		#endregion
	}
}