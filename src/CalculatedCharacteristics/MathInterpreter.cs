#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;
	using Zeiss.PiWeb.CalculatedCharacteristics.Syntax;

	#endregion

	/// <summary>
	/// Represents a parser responsible for parsing the formula of a characteristic and calculating its value.
	/// The parser can handle constants and functions.
	/// The underlying grammar is defined as follows:
	/// <code>
	/// <b>expression</b>: expression '+' term | expression '-' term | term<br/>
	/// <b>term</b>: term '*' factor | term '/' factor | factor<br/>
	/// <b>factor</b>: [+-] (NUMBER | function | '(' expression ')' | '{' path '}')<br/>
	/// <b>argumentList</b>: expression ';' argumentList | expression ',' argumentList | expression<br/>
	/// <b>function</b>: IDENT '(' [argumentList] ')'<br/>
	/// <b>path</b>: pathsegment ['(' KEY ')'] | pathsegment '/' path<br/>
	/// <b>pathsegment</b>: IDENT | '"' IDENT '"'<br/>
	/// <br/>
	/// <b>NUMBER</b>: floating-point number in invariant format<br/>
	/// <b>IDENT</b>: arbitrary string<br/>
	/// <b>KEY</b>: integer key for a characteristic attribute
	/// </code>
	/// </summary>
	public sealed partial class MathInterpreter
	{
		#region constants

		/// <summary>The terminals.</summary>
		public const string Terminals = @"+-*/(){};,""\";

		#endregion

		#region members

		[NotNull] private readonly CharacteristicCalculatorFactory _CharacteristicCalculatorFactory;
		[NotNull] private readonly ChildPathsHandler _ChildPathsHandler;
		[NotNull] private readonly PathResolverFactory _PathResolverFactory;
		[NotNull] private readonly OperationCatalog _OperationCatalog;

		private IEnumerable<Token> _Tokens;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="MathInterpreter"/> that support formulas with characteristics.
		/// <param name="characteristicCalculatorFactory">The delegate to get the calculator for a calculated characteristic.</param>
		/// <param name="childPathsHandler">The delegate to get the paths of the children for a path.</param>
		/// <param name="pathResolverFactory">An optional delegate to provide custom <see cref="IStringToPathResolver"/>.</param>
		/// </summary>
		public MathInterpreter(
			[NotNull] CharacteristicCalculatorFactory characteristicCalculatorFactory,
			[NotNull] ChildPathsHandler childPathsHandler,
			[CanBeNull] PathResolverFactory pathResolverFactory = null )
		{
			_CharacteristicCalculatorFactory = characteristicCalculatorFactory;
			_ChildPathsHandler = childPathsHandler;
			_PathResolverFactory = pathResolverFactory ?? DefaultPathResolverFactory;
			_OperationCatalog = OperationCatalog.Default;
		}

		#endregion

		#region methods

		private static IStringToPathResolver DefaultPathResolverFactory( PathInformation parent )
		{
			return new StringToPathResolver( parent );
		}

		/// <summary>
		/// Parses the given formula and internally builds a formula tree.
		/// The value of the formula is still NOT calculated.
		/// </summary>
		/// <param name="formula">The formula to parse.</param>
		/// <param name="parentPath">The path of the characteristic the formula belongs to. If not provided all paths will be handled relative to root.</param>
		/// <exception cref="ParserException">If the formula contains an error.</exception>
		/// <returns>The <see cref="IMathCalculator"/> that contains the parsed formula tree
		/// and can be used to calculated the formula value.</returns>
		public IMathCalculator Parse( [NotNull] string formula, [CanBeNull] PathInformation parentPath )
		{
			if( formula == null ) throw new ArgumentNullException( nameof( formula ) );

			return ParseInternal( formula, parentPath );
		}

		private IMathCalculator ParseInternal( string formula, PathInformation parentPath )
		{
			_Tokens = CreateTokens( formula );
			try
			{
				var parsedTree = MathSyntaxParser.CreateSyntaxTree( _Tokens, _PathResolverFactory( parentPath?.ParentPath ) );
				return new MathCalculator( parsedTree, formula,  _ChildPathsHandler, _CharacteristicCalculatorFactory, parentPath );
			}
			catch( NullReferenceException )
			{
				throw new ParserException( "Unexpected end of formula!", formula.Length );
			}
		}

		private IEnumerable<Token> CreateTokens( string formula )
		{
			var culture = CultureInfo.InvariantCulture;
			var pos = 0;

			var stringTokenizer = new StringTokenizer( formula, Terminals, true );
			foreach( var tokenString in stringTokenizer )
			{
				var tokenPosition = pos;
				var trimmedTokenString = tokenString.Trim();

				if( double.TryParse( trimmedTokenString, NumberStyles.Float, culture, out var doubleValue ) )
				{
					yield return new NumberToken(
						new Number( tokenPosition, trimmedTokenString.Length, doubleValue ),
						tokenPosition,
						tokenString );
				}
				else if( _OperationCatalog.TryGetOperation( trimmedTokenString, out var operation ) )
				{
					yield return new FunctionToken(
						operation,
						tokenPosition,
						tokenString );
				}
				else if( _OperationCatalog.TryGetConstant( trimmedTokenString, out var constant ) )
				{
					yield return new NumberToken( constant, tokenPosition, tokenString );
				}
				else if( trimmedTokenString.Length == 1 && Terminals.IndexOf( trimmedTokenString, StringComparison.InvariantCulture ) != -1 )
				{
					yield return new TerminalToken( tokenPosition, tokenString );
				}
				else
				{
					yield return new IdentToken( tokenPosition, tokenString );
				}

				pos += tokenString.Length;
			}

			yield return new FinalToken( pos );
		}

		#endregion
	}
}