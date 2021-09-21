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

	using System;
	using JetBrains.Annotations;

	#endregion

	/// <summary>
	/// Extesnion methods for <see cref="Token"/>.
	/// </summary>
	public static class TokenExtensions
	{
		#region methods

		/// <summary>
		/// Checks if the token text is a single character in <paramref name="validTokens"/>.
		/// </summary>
		/// <param name="token">The token to check.</param>
		/// <param name="validTokens">String with valid token characters.</param>
		/// <returns>True if the token is any token of <paramref name="validTokens"/>, else false.</returns>
		public static bool IsAnyToken( this Token token, [NotNull] string validTokens )
		{
			return token.TokenString.Length == 1 && validTokens.IndexOf( token.TokenString, StringComparison.InvariantCulture ) > -1;
		}

		/// <summary>
		/// Checks if the token text is a single character in <paramref name="validTokens"/> and provides the match index.
		/// </summary>
		/// <param name="token">The token to check.</param>
		/// <param name="validTokens">String with valid token characters.</param>
		/// <returns>The index of the matched token in <paramref name="validTokens"/>. If no match <c>-1</c> is returned.</returns>
		public static int IsSingleToken( this Token token, [NotNull] string validTokens )
		{
			return token.TokenString.Length == 1 ? validTokens.IndexOf( token.TokenString, StringComparison.InvariantCulture ) : -1;
		}

		#endregion
	}
}