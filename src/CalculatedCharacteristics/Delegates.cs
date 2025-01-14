#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	using System;
	using System.Collections.Generic;
	using Zeiss.PiWeb.Api.Core;

	/// <summary>
	/// Delegate used to get the value for an entity.
	/// </summary>
	public delegate double? MeasurementValueHandler( PathInformation characteristicPath );

	/// <summary>
	/// Delegate to get the value for a characteristic.
	/// </summary>
	public delegate object? EntityAttributeValueHandler( PathInformation path, ushort key, DateTime? timestamp );

	/// <summary>
	/// Delegate to get the children paths for a path.
	/// </summary>
	public delegate IEnumerable<PathInformation> ChildPathsHandler( PathInformation parent );

	/// <summary>
	/// Delegate to create a handler that creates <see cref="PathElement"/> from it's textual representation.
	/// </summary>
	public delegate IStringToPathResolver PathResolverFactory( PathInformation? parentPath );

	/// <summary>
	/// Delegate to provide an <see cref="IMathCalculator"/> for a characteristic.
	/// </summary>
	/// <param name="path">Path of the characteristic.</param>
	/// <returns>If <paramref name="path"/> results in a calculated characteristic the math calculator for this characteristic is returned, otherwise <c>null</c>.</returns>
	public delegate IMathCalculator? CharacteristicCalculatorFactory( PathInformation path );
}