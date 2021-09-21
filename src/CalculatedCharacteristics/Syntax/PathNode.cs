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

	using System;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Represents a path syntax node.
	/// </summary>
	internal class PathNode : SyntaxNode
	{
		#region constants

		private const string EscapeRegex = "[\x00\x01\x02\x03\x04\x05\x06]";
		private const char EscapedSlash = (char)0; // /
		private const char EscapedBackslash = (char)1; // \
		private const char EscapedQuotes = (char)2; // "
		private const char EscapedOpenBracket = (char)3; // (
		private const char EscapedCloseBracket = (char)4; // )
		private const char EscapedOpenBrace = (char)5; // {
		private const char EscapedCloseBrace = (char)6; // }

		#endregion

		#region members

		private readonly StringBuilder _PathStringBuilder;
		private readonly StringBuilder _TokenStringBuilder;
		private readonly int _Position;
		private bool _IsEscaping;
		private bool _EscapeNext;
		private bool _HasEscapes;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="PathNode"/>.
		/// </summary>
		public PathNode( SyntaxNode parent, int position ) : base( parent )
		{
			_Position = position;
			_PathStringBuilder = new StringBuilder();
			_TokenStringBuilder = new StringBuilder();
		}

		#endregion

		#region methods

		/// <inheritdoc/>
		public override MathElement CreateMathElement( IStringToPathResolver pathResolver )
		{
			// create path information
			var path = _PathStringBuilder.ToString();
			if( string.IsNullOrEmpty( path ) )
			{
				throw new ParserException( "Characteristic path is empty", _Position );
			}

			// get optional key
			ushort? key = null;
			var characteristicPath = path;
			var trimmedPath = path.TrimEnd();
			var lastOpenBrace = trimmedPath.LastIndexOf( "(", StringComparison.InvariantCulture );
			if( lastOpenBrace > 0 )
			{
				var nextCloseBrace = trimmedPath.IndexOf( ")", lastOpenBrace, StringComparison.InvariantCulture );
				if( nextCloseBrace == trimmedPath.Length - 1 )
				{
					var braceContent = trimmedPath.Substring( lastOpenBrace + 1, nextCloseBrace - lastOpenBrace - 1 );
					if( ushort.TryParse( braceContent, out var value ) )
					{
						key = value;
						characteristicPath = path.Substring( 0, lastOpenBrace );
					}
				}
			}

			if( _HasEscapes )
			{
				// do this afterwards hence API-Method in "ResolvePath" will split on '/'
				var escapeReplace = new Regex( EscapeRegex );
				var evaluator = new MatchEvaluator( match =>
				{
					switch( match.Value[0] )
					{
						case EscapedSlash:
							return "\\/";
						case EscapedBackslash:
							return "\\\\";
						case EscapedQuotes:
							return "\"";
						case EscapedOpenBracket:
							return "(";
						case EscapedCloseBracket:
							return ")";
						case EscapedOpenBrace:
							return "{";
						case EscapedCloseBrace:
							return "}";
					}

					return match.Value;
				});
				characteristicPath = escapeReplace.Replace( characteristicPath, evaluator );
			}

			// create path information
			var pathInformation = pathResolver.ResolvePath( characteristicPath );

			// create MathElement
			var mathElement = new Characteristic( _Position, _TokenStringBuilder.Length, _TokenStringBuilder.ToString(), pathInformation, key );
			return mathElement;
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleFunction( FunctionToken token )
		{
			if( (_IsEscaping || _EscapeNext)  && token.IsAnyToken( "/" ) )
			{
				_HasEscapes = true;
				_TokenStringBuilder.Append( token.TokenString );
				Append( EscapedSlash );
				return IsHandled();
			}

			Append( token.TokenString, token.TokenString );
			return IsHandled();
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleNumber( NumberToken token )
		{
			Append( token.TokenString, token.TokenString );
			return IsHandled();
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleTerminal( TerminalToken token )
		{
			if( !_EscapeNext )
			{
				switch( token.IsSingleToken( @"""\}" ) )
				{
					case 0:
						_TokenStringBuilder.Append( token.TokenString );
						_IsEscaping = !_IsEscaping;
						return IsHandled();

					case 1:
						_TokenStringBuilder.Append( token.TokenString );
						_EscapeNext = true;
						return IsHandled();

					case 2 when !_IsEscaping:
						return GotoParent( false );
				}
			}

			if ( (_IsEscaping || _EscapeNext) && token.IsAnyToken(@"(){}""\"))
			{
				_HasEscapes = true;
				_TokenStringBuilder.Append( token.TokenString );
				Append( GetEscapeCharacter( token.TokenString ) );
				return IsHandled();
			}

			Append( token.TokenString, token.TokenString );
			return IsHandled();
		}

		private static char GetEscapeCharacter( string terminal )
		{
			switch( terminal )
			{
				case "/":
					return EscapedSlash;
				case "\\":
					return EscapedBackslash;
				case "\"":
					return EscapedQuotes;
				case "(":
					return EscapedOpenBracket;
				case ")":
					return EscapedCloseBracket;
				case "{":
					return EscapedOpenBrace;
				case "}":
					return EscapedCloseBrace;
				default:
					// ReSharper disable once AssignNullToNotNullAttribute
					return terminal.First();
			}
		}

		/// <inheritdoc/>
		public override SyntaxNodeResult HandleIdent( IdentToken token )
		{
			Append( token.TokenString, token.TokenString );
			return IsHandled();
		}

		/// <inheritdoc/>
		protected override void ValidateFinal( int position )
		{
			if( _IsEscaping )
				throw new ParserException( "Missing closing escape character in path", position );

			if( _PathStringBuilder.Length == 0 )
				throw new ParserException( "Invalid path - the path is empty", position );
		}

		private void Append( char character )
		{
			_EscapeNext = false;
			_PathStringBuilder.Append( character );
		}

		private void Append( string pathString, string tokenString )
		{
			_EscapeNext = false;
			_TokenStringBuilder.Append(tokenString);
			_PathStringBuilder.Append( pathString );
		}

		#endregion
	}
}