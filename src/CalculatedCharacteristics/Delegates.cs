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
	using Zeiss.PiWeb.Api.Contracts;

	/// <summary>
	/// Delegate used to get the value for an entity.
	/// </summary>
	public delegate double? MeasurementValueHandler( [NotNull] PathInformation characteristicPath );

	/// <summary>
	/// Delegate to get the value for a characteristic.
	/// </summary>
	[CanBeNull]
	public delegate object EntityAttributeValueHandler( [NotNull] PathInformation path, ushort key, DateTime? timestamp );

	/// <summary>
	/// Delegate to get the children paths for a path.
	/// </summary>
	[NotNull]
	public delegate IEnumerable<PathInformation> ChildPathsHandler( [NotNull] PathInformation parent );

	/// <summary>
	/// Delegate to create a handler that creates <see cref="PathElement"/> from it's textual representation.
	/// </summary>
	[NotNull]
	public delegate IStringToPathResolver PathResolverFactory( [CanBeNull] PathInformation parentPath );

	/// <summary>
	/// Delegate to provide an <see cref="IMathCalculator"/> for an characteristic.
	/// </summary>
	/// <param name="path">Path of the characteristic.</param>
	/// <returns>If <paramref name="path"/> results in a calculated characteristic the math calculator for this characteristic ist returned, otherwise <c>null</c>.</returns>
	[CanBeNull]
	public delegate IMathCalculator CharacteristicCalculatorFactory( [NotNull] PathInformation path );
}