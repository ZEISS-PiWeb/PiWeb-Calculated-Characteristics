#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Text;

	#endregion

	/// <summary>
	/// Represents an exception that occurred while parsing a formula.
	/// </summary>
	public class ParserException : Exception
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">The message of the exception.</param>
		/// <param name="position">The position in the formula where the exception occurred.</param>
		public ParserException( string message, int position )
			: base( message )
		{
			Position = position;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the position in the formula where the exception occurred.
		/// </summary>
		public int Position { get; }

		#endregion

		#region methods

		/// <inheritdoc />
		public override string ToString()
		{
			return new StringBuilder()
				.Append( "ParserException (pos. " )
				.Append( Position )
				.Append( "): " )
				.Append( Message ).ToString();
		}

		#endregion
	}
}