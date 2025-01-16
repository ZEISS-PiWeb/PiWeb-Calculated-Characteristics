#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Syntax
{
	/// <summary>
	/// Base token implementation.
	/// </summary>
	internal abstract class Token
	{
		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="Token"/>.
		/// </summary>
		protected Token( int position, string tokenString )
		{
			Position = position;
			TokenString = tokenString;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the position of the token.
		/// </summary>
		public int Position { get; }

		/// <summary>
		/// Gets the string associated with the token.
		/// </summary>
		public string TokenString { get; }

		#endregion

		#region methods

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{TokenString} <{GetType().Name}|{Position}>";
		}

		/// <summary>
		/// Method required for visitor pattern to call type specific visitor method.
		/// </summary>
		/// <param name="visitor">The visitor that wants to visit the token.</param>
		public abstract void Accept( IMathVisitor visitor );

		#endregion
	}
}