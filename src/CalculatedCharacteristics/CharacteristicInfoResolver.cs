#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System.Collections.Generic;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	internal class CharacteristicInfoResolver : ICharacteristicInfoResolver
	{
		#region members

		[NotNull] private readonly ChildPathsHandler _ChildPathsHandler;
		[NotNull] private readonly EntityAttributeValueHandler _EntityAttributeValueHandler;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="CharacteristicInfoResolver"/>.
		/// </summary>
		public CharacteristicInfoResolver(
			[NotNull] ChildPathsHandler childPathsHandler,
			[NotNull] EntityAttributeValueHandler entityAttributeValueHandler,
			[CanBeNull] PathInformationDto sourcePath )
		{
			_ChildPathsHandler = childPathsHandler;
			_EntityAttributeValueHandler = entityAttributeValueHandler;
			SourcePath = sourcePath;
		}

		#endregion

		#region interface ICharacteristicInfoResolver

		/// <inheritdoc/>
		public PathInformationDto SourcePath { get; }

		/// <inheritdoc/>
		public IEnumerable<PathInformationDto> GetChildPaths( PathInformationDto parent )
		{
			return _ChildPathsHandler( parent );
		}

		/// <inheritdoc/>
		public object GetEntityAttributeValue( PathInformationDto path, ushort key )
		{
			return _EntityAttributeValueHandler( path, key, null );
		}

		#endregion
	}
}