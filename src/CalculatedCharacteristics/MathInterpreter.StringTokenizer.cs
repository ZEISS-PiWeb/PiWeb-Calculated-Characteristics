#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using JetBrains.Annotations;

	#endregion

	public sealed partial class MathInterpreter
	{
		#region class StringTokenizer

		/// <summary>
		/// Responsible for splitting a text into tokens.
		/// The splitting takes place at the given delimiters.
		/// The difference to <see cref="string.Split(char[])"/> is that the delimiters can also be returned as tokens.
		/// That is particularly needed for the parsing of files, mathematical terms and so on.
		/// </summary>
		private class StringTokenizer : IEnumerable<string>
		{
			#region members

			/// <summary>
			/// The length of the text to split.
			/// </summary>
			private readonly int _MaxPosition;

			/// <summary>
			/// The text to split.
			/// </summary>
			private readonly string _Text;

			/// <summary>
			/// The delimiters to split the text at.
			/// </summary>
			private readonly string _Delimiters;

			/// <summary>
			/// A flag to determine whether the delimiters should be returned as tokens.
			/// </summary>
			private readonly bool _ReturnDelimiterTokens;

			/// <summary>
			/// The highest and the lowest ASCII value of the delimiters.
			/// This is used to speed up the search for delimiters.
			/// </summary>
			private readonly (char Min, char Max) _DelimiterCharRange;

			/// <summary>
			/// Current position in the text to split.
			/// </summary>
			private int _CurrentPosition;

			private string _Current;

			#endregion

			#region constructors

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="text">The text to be split into tokens.</param>
			/// <param name="delimiter">The delimiters to split the text at.</param>
			/// <param name="returnDelimiterTokens">A flag whether the delimiters should be returned as tokens.</param>
			public StringTokenizer( string text, [NotNull] string delimiter, bool returnDelimiterTokens )
			{
				if( string.IsNullOrEmpty( delimiter ) )
					throw new ArgumentNullException( nameof( delimiter ) );

				_CurrentPosition = 0;
				_Text = text;
				_MaxPosition = text.Length;
				_Delimiters = delimiter;
				_ReturnDelimiterTokens = returnDelimiterTokens;
				_DelimiterCharRange = GetCharRange( _Delimiters );
			}

			#endregion

			#region methods

			/// <summary>
			/// Moves to the next token.
			/// </summary>
			/// <returns><code>True</code> if the next token exists, otherwise <code>false</code>.</returns>
			private bool MoveNext()
			{
				if( !_ReturnDelimiterTokens )
					_CurrentPosition = SkipDelimiters( _CurrentPosition );

				if( _CurrentPosition >= _MaxPosition )
					return false;

				var start = _CurrentPosition;
				_CurrentPosition = ScanToken( _CurrentPosition );
				_Current = _Text.Substring( start, _CurrentPosition - start );
				return true;
			}

			/// <summary>
			/// Determines the highest and the lowest ASCII value of the delimiters.
			/// </summary>
			private static (char, char) GetCharRange( string anyString )
			{
				var maxChar = char.MinValue;
				var minChar = char.MaxValue;
				foreach( var c in anyString )
				{
					maxChar = c > maxChar ? c : maxChar;
					minChar = c < minChar ? c : minChar;
				}

				return ( minChar, maxChar );
			}

			/// <summary>
			/// Checks whether a character is a possible delimiter.
			/// </summary>
			private static bool IsPossibleDelimiter( (char Min, char Max) range, char character )
			{
				return !( character > range.Max || character < range.Min );
			}

			/// <summary>
			/// Skips delimiters in the text to split starting at the start position <paramref name="startPos"/>.
			/// </summary>
			/// <returns>The position of the first character that is no delimiter or
			/// the end of the text if there are no more characters that are no delimiters.</returns>
			private int SkipDelimiters( int startPos )
			{
				var position = startPos;
				while( position < _MaxPosition )
				{
					var c = _Text[ position ];
					if( !IsPossibleDelimiter( _DelimiterCharRange, c ) || _Delimiters.IndexOf( c ) < 0 )
						break;

					position++;
				}

				return position;
			}

			/// <summary>
			/// Scans the text for the next delimiter starting a the start position <paramref name="startPos"/>
			/// </summary>
			/// <returns>The position of the next delimiter or the end of the text if there are no more delimiters.</returns>
			private int ScanToken( int startPos )
			{
				var position = startPos;
				while( position < _MaxPosition )
				{
					var c = _Text[ position ];
					if( IsPossibleDelimiter( _DelimiterCharRange, c ) && _Delimiters.IndexOf( c ) >= 0 )
					{
						if( _ReturnDelimiterTokens && startPos == position )
							position++;
						break;
					}

					position++;
				}

				return position;
			}

			#endregion

			#region interface IEnumerable<string>

			/// <summary>
			/// Returns an enumerator that iterates through the tokens.
			/// </summary>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			/// <summary>
			/// Returns an enumerator that iterates through the tokens.
			/// </summary>
			public IEnumerator<string> GetEnumerator()
			{
				while( MoveNext() )
				{
					yield return _Current;
				}
			}

			#endregion
		}

		#endregion
	}
}