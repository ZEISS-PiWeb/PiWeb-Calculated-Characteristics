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

	using System.Collections.Generic;
	using Zeiss.PiWeb.Api.Core;

	#endregion

	internal class CharacteristicInfoResolver : ICharacteristicInfoResolver
	{
		#region members

		private readonly ChildPathsHandler _ChildPathsHandler;
		private readonly EntityAttributeValueHandler _EntityAttributeValueHandler;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="CharacteristicInfoResolver"/>.
		/// </summary>
		public CharacteristicInfoResolver(
			ChildPathsHandler childPathsHandler,
			EntityAttributeValueHandler entityAttributeValueHandler,
			PathInformation? sourcePath )
		{
			_ChildPathsHandler = childPathsHandler;
			_EntityAttributeValueHandler = entityAttributeValueHandler;
			SourcePath = sourcePath;
		}

		#endregion

		#region interface ICharacteristicInfoResolver

		/// <inheritdoc/>
		public PathInformation? SourcePath { get; }

		/// <inheritdoc/>
		public IEnumerable<PathInformation> GetChildPaths( PathInformation parent )
		{
			return _ChildPathsHandler( parent );
		}

		/// <inheritdoc/>
		public object? GetEntityAttributeValue( PathInformation path, ushort key )
		{
			return _EntityAttributeValueHandler( path, key, null );
		}

		#endregion
	}
}