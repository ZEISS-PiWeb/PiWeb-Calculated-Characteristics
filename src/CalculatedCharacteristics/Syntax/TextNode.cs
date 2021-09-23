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

	using System.Text;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Represents a text syntax node.
	/// </summary>
	internal class TextNode : SyntaxNode
	{
		#region members

		private readonly int _Position;
		private readonly StringBuilder _TextStringBuilder;
		private readonly StringBuilder _TokenStringBuilder;
		private bool _IsEscaping;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="TextNode"/>.
		/// </summary>
		public TextNode( SyntaxNode parent, int position ) : base( parent )
		{
			_Position = position;
			_TextStringBuilder = new StringBuilder();
			_TokenStringBuilder = new StringBuilder();
		}

		#endregion

		#region methods

		/// <inheritdoc/>
		public override MathElement CreateMathElement( IStringToPathResolver pathResolver )
		{
			if( _IsEscaping )
				throw new ParserException( "The text ends with escaping character", _Position + _TextStringBuilder.Length );

			return new Literal( _Position, _TokenStringBuilder.Length, _TextStringBuilder.ToString() );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleFunction( FunctionToken token )
		{
			return Append( token );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleNumber( NumberToken token )
		{
			return Append( token );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleTerminal( TerminalToken token )
		{
			if( !_IsEscaping )
			{
				switch( token.IsSingleToken( "\"\\" ) )
				{
					case 0:
						return GotoParent( false );
					case 1:
						_TokenStringBuilder.Append( token.TokenString );
						_IsEscaping = true;
						return IsHandled();
				}
			}

			return Append( token );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleIdent( IdentToken token )
		{
			return Append( token );
		}

		/// <inheritdoc/>
		protected override void ValidateFinal( int position )
		{
			throw new ParserException( "Invalid end of text", position );
		}

		private SyntaxNodeResult Append( Token token )
		{
			_IsEscaping = false;
			_TextStringBuilder.Append( token.TokenString );
			_TokenStringBuilder.Append( token.TokenString );
			return IsHandled();
		}

		#endregion
	}
}