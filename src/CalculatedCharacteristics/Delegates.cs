#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	using System;
	using System.Collections.Generic;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	/// <summary>
	/// Delegate used to get the value for an entity.
	/// </summary>
	public delegate double? MeasurementValueHandler( [NotNull] PathInformationDto characteristicPath );

	/// <summary>
	/// Delegate to get the value for a characteristic.
	/// </summary>
	[CanBeNull]
	public delegate object EntityAttributeValueHandler( [NotNull] PathInformationDto path, ushort key, DateTime? timestamp );

	/// <summary>
	/// Delegate to get the children paths for a path.
	/// </summary>
	[NotNull]
	public delegate IEnumerable<PathInformationDto> ChildPathsHandler( [NotNull] PathInformationDto parent );

	/// <summary>
	/// Delegate to create a handler that creates <see cref="PathElementDto"/> from it's textual representation.
	/// </summary>
	[NotNull]
	public delegate IStringToPathResolver PathResolverFactory( [CanBeNull] PathInformationDto parentPath );

	/// <summary>
	/// Delegate to provide an <see cref="IMathCalculator"/> for an characteristic.
	/// </summary>
	/// <param name="path">Path of the characteristic.</param>
	/// <returns>If <paramref name="path"/> results in a calculated characteristic the math calculator for this characteristic ist returned, otherwise <c>null</c>.</returns>
	[CanBeNull]
	public delegate IMathCalculator CharacteristicCalculatorFactory( [NotNull] PathInformationDto path );
}