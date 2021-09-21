#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics.Syntax
{
	using Zeiss.PiWeb.Shared.CalculatedCharacteristics.Arithmetic;

	/// <summary>
	/// Represents a factor syntax node.
	/// </summary>
	internal class FactorNode : SyntaxNode
	{
		#region members

		private SyntaxNode _Child;
		private MathElement _MathElement;
		private bool _ChangeSign;
		private int _ChangeSignPosition = -1;
		private FactorType _FactorType;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="FactorNode"/>.
		/// </summary>
		public FactorNode( SyntaxNode parent ) : base( parent )
		{
			_FactorType = FactorType.Normal;
		}

		#endregion

		#region methods

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleFunction( FunctionToken token )
		{
			if( !HasContent( this ) )
			{
				switch( token.IsSingleToken( "+-" ) )
				{
					case 0:
						return IsHandled();
					case 1:
						_ChangeSign = !_ChangeSign;
						_ChangeSignPosition = token.Position;
						return IsHandled();
				}

				var function = AddNewFunction();
				return GotoNext( function, false );
			}

			// all other function tokens can not be handled by function node -> max parent can handle it
			return GotoParent( false );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleNumber( NumberToken token )
		{
			if( !HasContent( this ) )
			{
				_MathElement = token.Value;
				return GotoParent( true );
			}

			return GotoParent( false );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleTerminal( TerminalToken token )
		{
			if( !HasContent( this ) )
			{
				switch( token.IsSingleToken( "{(\"" ) )
				{
					case 0:
						// start characteristic node
						_FactorType = FactorType.Path;
						var path = AddNewPath( token.Position + 1 );
						return GotoNext( path, true );

					case 1:
						// start expression node
						_FactorType = FactorType.Expression;
						var expression = AddNewExpression();
						return GotoNext( expression, true );

					case 2:
						// start text node
						_FactorType = FactorType.Text;
						var text = AddNewText( token.Position + 1 );
						return GotoNext( text, true );
				}
			}
			else
			{
				switch( token.IsSingleToken( "})\"" ) )
				{
					case 0 when _FactorType == FactorType.Path:
					case 1 when _FactorType == FactorType.Expression:
					case 2 when _FactorType == FactorType.Text:
						// close expression|characteristic|text node
						return GotoParent( true );
				}
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

			throw new ParserException( "Factor node does not support handling text", token.Position );
		}

		/// <inheritdoc/>
		protected override void ValidateFinal( int position )
		{
			if( !HasContent( this ) )
			{
				throw new ParserException( "Invalid end of factor", position );
			}

			switch( _FactorType )
			{
				case FactorType.Expression:
					throw new ParserException( "Missing brace for end of expression", position );

				case FactorType.Path:
					throw new ParserException( "Missing brace for end of characteristic", position );
			}
		}

		/// <inheritdoc/>
		public override MathElement CreateMathElement( IStringToPathResolver pathResolver )
		{
			var mathElement = !( _Child is null ) ? _Child.CreateMathElement( pathResolver ) : _MathElement;

			if( _ChangeSign )
				mathElement = new Negate( mathElement, _ChangeSignPosition );
			return mathElement;
		}

		private ExpressionNode AddNewExpression()
		{
			var expression = new ExpressionNode( this );
			_Child = expression;
			return expression;
		}

		private FunctionNode AddNewFunction()
		{
			var function = new FunctionNode( this );
			_Child = function;
			return function;
		}

		private PathNode AddNewPath( int position )
		{
			var path = new PathNode( this, position );
			_Child = path;
			return path;
		}

		private TextNode AddNewText( int position )
		{
			var text = new TextNode( this, position );
			_Child = text;
			return text;
		}

		private static bool HasContent( FactorNode node )
		{
			return !( node._MathElement is null && node._Child is null );
		}

		#endregion

		#region class FactorType

		private enum FactorType
		{
			Normal,
			Expression,
			Path,
			Text
		}

		#endregion
	}
}