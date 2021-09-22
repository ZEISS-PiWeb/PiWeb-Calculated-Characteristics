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
	/// Implement this interface to provide a syntax node.
	/// </summary>
	internal abstract class SyntaxNode
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="SyntaxNode"/>.
		/// </summary>
		protected SyntaxNode( SyntaxNode parent )
		{
			Parent = parent;
		}

		#endregion

		#region properties

		/// <summary>
		/// The parent of the node.
		/// </summary>
		protected SyntaxNode Parent { get; }

		#endregion

		#region methods

		/// <summary>
		/// Creates a <see cref="MathElement"/> from the syntax node.
		/// </summary>
		/// <param name="pathResolver"><see cref="StringToPathResolver"/> to use for path resolving.</param>
		/// <returns>The math element represented by the syntax node.</returns>
		public abstract MathElement CreateMathElement( IStringToPathResolver pathResolver );

		/// <summary>
		/// Handle token <see cref="FunctionToken"/>.
		/// </summary>
		public abstract SyntaxNodeResult HandleFunction( FunctionToken token );

		/// <summary>
		/// Handle token <see cref="NumberToken"/>.
		/// </summary>
		public abstract SyntaxNodeResult HandleNumber( NumberToken token );

		/// <summary>
		/// Handle token <see cref="TerminalToken"/>.
		/// </summary>
		public abstract SyntaxNodeResult HandleTerminal( TerminalToken token );

		/// <summary>
		/// Handle token <see cref="IdentToken"/>.
		/// </summary>
		public abstract SyntaxNodeResult HandleIdent( IdentToken token );

		/// <summary>
		/// Handle token <see cref="FinalToken"/>.
		/// </summary>
		public SyntaxNodeResult HandleFinal( FinalToken token )
		{
			ValidateFinal( token.Position );
			return GotoParent( false );
		}

		/// <summary>
		/// Validate node on token stream ended.
		/// </summary>
		protected abstract void ValidateFinal( int position );

		/// <summary>
		/// Go to <paramref name="nextNode"/>.
		/// </summary>
		protected static SyntaxNodeResult GotoNext( SyntaxNode nextNode, bool isHandled )
		{
			return new SyntaxNodeResult( nextNode, isHandled );
		}

		/// <summary>
		/// Creates result to indicate that everything is ok for final token.
		/// </summary>
		protected SyntaxNodeResult IsFinalized()
		{
			return GotoParent( false );
		}

		/// <summary>
		/// Go to parent node.
		/// </summary>
		protected SyntaxNodeResult GotoParent( bool isHandled )
		{
			return GotoNext( Parent, isHandled );
		}

		/// <summary>
		/// Mark token as handled and stay in current node.
		/// </summary>
		protected SyntaxNodeResult IsHandled()
		{
			return GotoNext( this, true );
		}

		#endregion

		#region class SyntaxNodeResult

		/// <summary>
		/// The result of token handling by syntax node.
		/// </summary>
		public readonly struct SyntaxNodeResult
		{
			/// <summary>
			/// Creates a new instance of <see ref="SyntaxNodeVisitResult"/>.
			/// </summary>
			public SyntaxNodeResult( SyntaxNode node, bool isHandled )
			{
				IsHandled = isHandled;
				NextNode = node;
			}

			/// <summary>
			/// Gets a value indicating if the syntax node handled the token.
			/// </summary>
			public bool IsHandled { get; }

			/// <summary>
			/// Gets a value indicting the token to handle current or next token.
			/// </summary>
			public SyntaxNode NextNode { get; }
		}

		#endregion
	}
}