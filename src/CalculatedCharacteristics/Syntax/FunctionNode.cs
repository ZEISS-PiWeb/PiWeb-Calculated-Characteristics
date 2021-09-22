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
	/// Represents a function syntax node.
	/// </summary>
	internal class FunctionNode : SyntaxNode
	{
		#region members

		private readonly SyntaxNode _ArgumentList;
		private bool _HasFunction;
		private bool _IsArgumentListOpened;
		private bool _IsArgumentListClosed;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="FunctionNode"/>.
		/// </summary>
		public FunctionNode( SyntaxNode parent ) : base( parent )
		{
			_ArgumentList = new ArgumentListNode( this );
		}

		#endregion

		#region methods

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleFunction( FunctionToken token )
		{
			if( !_HasFunction )
			{
				_HasFunction = true;
				_ArgumentList.HandleFunction( token );
				return IsHandled();
			}

			return GotoParent( false );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleNumber( NumberToken token )
		{
			throw new ParserException( "Function node does not support handling number", token.Position );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleTerminal( TerminalToken token )
		{
			if( !_HasFunction )
				throw new ParserException( "Function node does not support handling terminal without operation", token.Position );

			if( !_IsArgumentListOpened && token.IsAnyToken( "(" ) )
			{
				_IsArgumentListOpened = true;
				return GotoNext( _ArgumentList, true );
			}

			if( _IsArgumentListOpened && !_IsArgumentListClosed && token.IsAnyToken( ")" ) )
			{
				_IsArgumentListClosed = true;
				return GotoParent( true );
			}

			return GotoParent( false );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleIdent( IdentToken token )
		{
			if( string.IsNullOrEmpty( token.TokenString.Trim() ) )
			{
				return IsHandled();
			}

			throw new ParserException( "Function node does not support handling text", token.Position );
		}

		/// <inheritdoc/>
		protected override void ValidateFinal( int position )
		{
			if( _HasFunction && _IsArgumentListOpened && _IsArgumentListClosed )
				return;

			throw new ParserException( "Invalid end of function", position );
		}

		/// <inheritdoc/>
		public override MathElement CreateMathElement( IStringToPathResolver pathResolver )
		{
			return _ArgumentList.CreateMathElement( pathResolver );
		}

		#endregion
	}
}