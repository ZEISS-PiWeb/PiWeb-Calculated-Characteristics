#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Syntax
{
	/// <summary>
	/// Implement this interface to provide a math token that can be visited.
	/// </summary>
	internal interface IMathToken
	{
		#region methods

		/// <summary>
		/// Method required for visitor pattern to call type specific visitor method.
		/// </summary>
		/// <param name="visitor">The visitor that wants to visit the token.</param>
		void Accept( IMathVisitor visitor );

		#endregion
	}
}