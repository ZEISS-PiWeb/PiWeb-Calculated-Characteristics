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

	using System.Collections.Generic;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Implementation of syntax parser for calculated characteristics.
	/// </summary>
	internal class MathSyntaxParser : IMathVisitor
	{
		#region members

		private SyntaxNode _CurrentNode;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="MathInterpreterVisitor"/>.
		/// </summary>
		/// <remarks>Creating a new instance will prevent threading issues.</remarks>
		private MathSyntaxParser()
		{ }

		#endregion

		#region methods

		/// <summary>
		/// Creates the syntax tree from sequential list of tokens.
		/// </summary>
		/// <param name="tokens">List of sequential tokens.</param>
		/// <param name="pathResolver"><see cref="StringToPathResolver"/> to use for path resolving.</param>
		/// <returns>The root <see cref="MathElement"/> of the path math tree.</returns>
		public static MathElement CreateSyntaxTree( IEnumerable<Token> tokens, IStringToPathResolver pathResolver )
		{
			var visitor = new MathSyntaxParser();
			var rootSyntaxNode = visitor.CreateSyntaxTreeInternal( tokens );
			return rootSyntaxNode?.CreateMathElement( pathResolver );
		}

		/// <summary>
		/// Creates the syntax tree
		/// </summary>
		private SyntaxNode CreateSyntaxTreeInternal( IEnumerable<Token> tokens )
		{
			var expression = new ExpressionNode( null );
			_CurrentNode = expression;
			ParseTokens( tokens );
			return _CurrentNode;
		}

		private void ParseTokens( IEnumerable<Token> tokens )
		{
			foreach( var token in tokens )
			{
				token.Accept( this );
			}
		}

		#endregion

		#region interface IMathVisitor

		/// <inheritdoc/>
		public void VisitFunction( FunctionToken token )
		{
			SyntaxNode.SyntaxNodeResult result;
			do
			{
				result = _CurrentNode.HandleFunction( token );
				_CurrentNode = result.NextNode;
			} while( _CurrentNode != null && !result.IsHandled );
		}

		/// <inheritdoc/>
		public void VisitNumber( NumberToken token )
		{
			SyntaxNode.SyntaxNodeResult result;
			do
			{
				result = _CurrentNode.HandleNumber( token );
				_CurrentNode = result.NextNode;
			} while( _CurrentNode != null && !result.IsHandled );
		}

		/// <inheritdoc/>
		public void VisitTerminal( TerminalToken token )
		{
			SyntaxNode.SyntaxNodeResult result;
			do
			{
				result = _CurrentNode.HandleTerminal( token );
				_CurrentNode = result.NextNode;
			} while( _CurrentNode != null && !result.IsHandled );
		}

		/// <inheritdoc/>
		public void VisitIdent( IdentToken token )
		{
			SyntaxNode.SyntaxNodeResult result;
			do
			{
				result = _CurrentNode.HandleIdent( token );
				_CurrentNode = result.NextNode;
			} while( _CurrentNode != null && !result.IsHandled );
		}

		/// <inheritdoc/>
		public void VisitFinal( FinalToken token )
		{
			SyntaxNode rootNode;

			if( _CurrentNode == null )
			{
				throw new ParserException( "Missing current syntax node", token.Position );
			}

			do
			{
				rootNode = _CurrentNode;
				_CurrentNode = _CurrentNode.HandleFinal( token ).NextNode;
			} while( _CurrentNode != null );

			// set current node back to root
			_CurrentNode = rootNode;
		}

		#endregion
	}
}