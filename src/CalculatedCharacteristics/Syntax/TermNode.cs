#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH                   */
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
	/// Represents a term syntax node.
	/// </summary>
	internal class TermNode : SyntaxNode
	{
		#region constants

		private const string SupportedTerminals = "*/";

		#endregion

		#region members

		private readonly List<SyntaxNode> _Children;
		private MathOperation _MathOperation;
		private int _TokenPosition = -1;
		private string _TokenString = "";

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="ExpressionNode"/>.
		/// </summary>
		public TermNode( SyntaxNode parent ) : base( parent )
		{
			_Children = new List<SyntaxNode>( 2 );
		}

		#endregion

		#region methods

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleFunction( FunctionToken token )
		{
			// no term means first term
			if( _Children.Count == 0 )
			{
				var factor = AddNewFactor();
				return GotoNext( factor, false );
			}

			if( token.IsAnyToken( SupportedTerminals ) )
			{
				// one term already set
				if( _Children.Count == 1 )
				{
					// add a new function
					_MathOperation = token.Value;
					_TokenPosition = token.Position;
					_TokenString = token.TokenString;
					var factor = AddNewFactor();
					return GotoNext( factor, true );
				}

				if( _Children.Count == 2 )
				{
					// term only supports two children -> due to math goes from left to right create a copy from current term and put it as left operator
					var term = new TermNode( this )
					{
						_MathOperation = _MathOperation,
						_TokenPosition = _TokenPosition,
						_TokenString = _TokenString
					};
					term._Children.AddRange( _Children );

					_MathOperation = token.Value;
					_TokenPosition = token.Position;
					_TokenString = token.TokenString;
					_Children.Clear();
					_Children.Add( term );
					var factor = AddNewFactor();
					return GotoNext( factor, true );
				}
			}

			return GotoParent( false );
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleNumber( NumberToken token )
		{
			return UpdateHandlerNode();
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleTerminal( TerminalToken token )
		{
			return UpdateHandlerNode();
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleIdent( IdentToken token )
		{
			if( string.IsNullOrEmpty( token.TokenString.Trim() ) )
			{
				return IsHandled();
			}

			return UpdateHandlerNode();
		}

		/// <inheritdoc/>
		protected override void ValidateFinal( int position )
		{
			if( _Children.Count == 1 || _Children.Count == 2 && !_MathOperation.IsEmpty )
			{
				return;
			}

			throw new ParserException( "Invalid end of term", position );
		}

		/// <inheritdoc/>
		public override MathElement CreateMathElement( IStringToPathResolver pathResolver )
		{
			if( _MathOperation.IsEmpty && _Children.Count == 1 )
				return _Children[ 0 ].CreateMathElement( pathResolver );

			if( !_MathOperation.IsEmpty && _Children.Count == 2 )
			{
				return new Function(
					_TokenPosition,
					_TokenString.Length,
					[_Children[ 0 ].CreateMathElement( pathResolver ), _Children[ 1 ].CreateMathElement( pathResolver )],
					_TokenString,
					_MathOperation );
			}

			throw new ParserException( "Invalid term node detected", _TokenPosition );
		}

		private SyntaxNodeResult UpdateHandlerNode()
		{
			// check if node expects first or second child
			if( _Children.Count == 0 || _Children.Count == 1 && !_MathOperation.IsEmpty )
			{
				var factor = AddNewFactor();
				return GotoNext( factor, false );
			}

			return GotoParent( false );
		}

		private FactorNode AddNewFactor()
		{
			var factor = new FactorNode( this );
			_Children.Add( factor );
			return factor;
		}

		#endregion
	}
}