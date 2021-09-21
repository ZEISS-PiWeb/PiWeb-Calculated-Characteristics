#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics.Syntax
{
	#region usings

	using System.Collections.Generic;
	using System.Linq;
	using Zeiss.PiWeb.Shared.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Represents an argument list syntax node.
	/// </summary>
	internal class ArgumentListNode : SyntaxNode
	{
		#region members

		private MathOperation _MathOperation;
		private List<SyntaxNode> _Arguments;
		private int _TokenPosition = -1;
		private string _TokenString;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="ArgumentListNode"/>.
		/// </summary>
		public ArgumentListNode( SyntaxNode parent ) : base( parent )
		{ }

		#endregion

		#region methods

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleFunction( FunctionToken token )
		{
			if( _MathOperation.IsEmpty )
			{
				_MathOperation = token.Value;
				_TokenPosition = token.Position;
				_TokenString = token.TokenString;
				return IsHandled();
			}

			if( _Arguments is null )
			{
				_Arguments = new List<SyntaxNode>( 1 );
				var expression = AddNewExpression();
				return GotoNext( expression, false );
			}

			throw new ParserException( "Function arguments can only by associated to single operation", token.Position );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleNumber( NumberToken token )
		{
			if( _Arguments is null )
			{
				_Arguments = new List<SyntaxNode>( 1 );
				var expression = AddNewExpression();
				return GotoNext( expression, false );
			}

			throw new ParserException( "Argument list does not expects number", token.Position );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleTerminal( TerminalToken token )
		{
			if( _Arguments is null && !token.IsAnyToken( ");," ) )
			{
				_Arguments = new List<SyntaxNode>( 1 );
				var expression = AddNewExpression();
				return GotoNext( expression, false );
			}

			if( token.IsAnyToken( ")" ) )
			{
				return GotoParent( false );
			}

			if( _Arguments?.Count > 0 && token.IsAnyToken( ";," ) )
			{
				var expression = AddNewExpression();
				return GotoNext( expression, true );
			}

			throw new ParserException( $"Argument list does not expects terminal {token.TokenString}", token.Position );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleIdent( IdentToken token )
		{
			if( string.IsNullOrEmpty( token.TokenString.Trim() ) )
			{
				return IsHandled();
			}

			if( _Arguments is null )
			{
				_Arguments = new List<SyntaxNode>( 1 );
				var expression = AddNewExpression();
				return GotoNext( expression, false );
			}

			throw new ParserException( "Argument list does not expects number", token.Position );
		}

		/// <inheritdoc/>
		protected override void ValidateFinal( int position )
		{
			throw new ParserException( "Invalid end of function", position );
		}

		/// <inheritdoc/>
		public override MathElement CreateMathElement( IStringToPathResolver pathResolver )
		{
			if( _MathOperation.IsEmpty )
				throw new ParserException( "Argument list is not associated to operation", -1 );

			var mathElement = new Function(
				_TokenPosition,
				_TokenString.Length,
				_Arguments?.Select( a => a.CreateMathElement( pathResolver ) ).ToArray(),
				_TokenString,
				_MathOperation );

			return mathElement;
		}

		private ExpressionNode AddNewExpression()
		{
			var expression = new ExpressionNode( this );
			_Arguments.Add( expression );
			return expression;
		}

		#endregion
	}
}