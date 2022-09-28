#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Contracts;

	#endregion

	/// <summary>
	/// Implement this interface to provide an resolver that creates the entity path from a textual representation.
	/// </summary>
	public interface IStringToPathResolver
	{
		#region methods

		/// <summary>
		/// Creates a <see cref="PathInformation"/> from a path string based on the relative path.
		/// </summary>
		/// <remarks>A path from a formula can contain ".." allowing to navigate 'upwards' in the path tree.</remarks>
		/// <param name="path">The path of characteristic.</param>
		/// <returns>The created characteristic path or <code>null</code> if the path could not be resolved.</returns>
		[CanBeNull]
		PathInformation ResolvePath( [CanBeNull] string path );

		#endregion
	}
}