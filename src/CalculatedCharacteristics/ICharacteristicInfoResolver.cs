#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System.Collections.Generic;
	using Zeiss.PiWeb.Api.Core;

	#endregion

	/// <summary>
	/// Provides information for entities required to resolve dependencies in formulas of calculated characteristics.
	/// </summary>
	public interface ICharacteristicInfoResolver
	{
		#region properties

		/// <summary>
		/// The path of the characteristic that owns the formula.
		/// </summary>
		PathInformation? SourcePath { get; }

		#endregion

		#region methods

		/// <summary>
		/// Gets the paths of the children for the given <paramref name="parent"/> path.
		/// </summary>
		/// <param name="parent">The path to get the children for.</param>
		/// <returns>The list of children.</returns>
		IEnumerable<PathInformation> GetChildPaths( PathInformation parent );

		/// <summary>
		/// Gets the attribute value for an entity.
		/// </summary>
		/// <param name="path">The path of the characteristic.</param>
		/// <param name="key">The attribute key.</param>
		/// <returns>The measured or calculated value.</returns>
		object? GetEntityAttributeValue( PathInformation path, ushort key );

		#endregion
	}
}