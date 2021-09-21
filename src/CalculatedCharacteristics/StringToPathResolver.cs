#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Rest.Dtos;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Defines the default implementation to provide an entity path from string.
	/// </summary>
	public class StringToPathResolver : IStringToPathResolver
	{
		#region members

		[NotNull] private readonly PathInformationDto _ReferencePath;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="StringToPathResolver"/>.
		/// </summary>
		/// <param name="referencePath">The path of the entity that is used as reference for relative path strings.</param>
		public StringToPathResolver( PathInformationDto referencePath )
		{
			_ReferencePath = referencePath ?? PathInformationDto.Root;
		}

		#endregion

		#region methods

		/// <inheritdoc/>
		public virtual PathInformationDto ResolvePath( string path )
		{
			return ResolvePathInternal( path, _ReferencePath );
		}

		/// <summary>
		/// Creates a <see cref="PathInformationDto"/> from a formula path based on the given <paramref name="parentPath"/>.
		/// </summary>
		/// <remarks>A path from a formula can contain ".." allowing to navigate 'upwards' in the path tree.</remarks>
		/// <param name="path">The path of characteristic.</param>
		/// <param name="parentPath">The path of the current inspection plan node parent.</param>
		/// <returns>The created characteristic path or <code>null</code> if the path could not be resolved.</returns>
		[CanBeNull]
		private static PathInformationDto ResolvePathInternal(
			[CanBeNull] string path,
			[NotNull] PathInformationDto parentPath )
		{
			if( path == null )
				return null;

			var result = ParsePath( path, parentPath );
			if( result == null )
				return null;

			return CorrectPathElementTypes( result, parentPath );
		}

		[CanBeNull]
		private static PathInformationDto ParsePath( [NotNull] string path, [NotNull] PathInformationDto parentPath )
		{
			path = path.Trim();
			if( string.IsNullOrEmpty( path ) )
				return null;

			if( path == PathInformationDto.Root.ToString() )
				return PathInformationDto.Root;

			var parsedPath = PathHelper.String2CharPathInformation( path );

			// path is relative => absolute path is determined based on the given parent path
			if( !path.StartsWith( PathInformationDto.Root.ToString(), StringComparison.Ordinal ) )
				parsedPath = ToAbsolutePath( parsedPath, parentPath );

			return parsedPath;
		}

		[CanBeNull]
		private static PathInformationDto ToAbsolutePath( [NotNull] PathInformationDto relativePath, [NotNull] PathInformationDto parentPath )
		{
			var pathElements = new List<PathElementDto>( parentPath );

			foreach( var pathElement in relativePath )
			{
				if( !pathElement.Value.Equals( ".." ) )
				{
					pathElements.Add( pathElement );
					continue;
				}

				if( pathElements.Count == 0 )
					return null;

				pathElements.RemoveAt( pathElements.Count - 1 );
			}

			return new PathInformationDto( pathElements );
		}

		[NotNull]
		private static PathInformationDto CorrectPathElementTypes( [NotNull] PathInformationDto path, [NotNull] PathInformationDto parentPath )
		{
			var numberOfMatchingPathElements = 0;
			var maxPathElementsToCheck = Math.Min( parentPath.Count, path.Count );
			for( var i = 0; i < maxPathElementsToCheck; i++ )
			{
				var parentPartPathElement = parentPath[ i ];

				var pathElement = path[ i ];
				if( pathElement == parentPartPathElement )
				{
					numberOfMatchingPathElements++;
					continue;
				}

				// Try the other type
				pathElement = pathElement.Type == InspectionPlanEntityDto.Part
					? PathElementDto.Char( pathElement.Value )
					: PathElementDto.Part( pathElement.Value );
				if( pathElement == parentPartPathElement )
				{
					numberOfMatchingPathElements++;
					continue;
				}

				// No match found, stop here
				break;
			}

			if( numberOfMatchingPathElements == 0 )
				return path;

			return parentPath.SubPath( 0, numberOfMatchingPathElements ) + path.SubPath( numberOfMatchingPathElements );
		}

		#endregion
	}


}