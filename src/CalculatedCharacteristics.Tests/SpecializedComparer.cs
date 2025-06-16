#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2025                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests;

#region usings

using System;

#endregion

internal static class SpecializedComparer
{
	#region methods

	public static bool CompareDoubles( double? x, double? y )
	{
		const double defaultPrecision = 1e-15;

		if( x.HasValue != y.HasValue )
			return false;

		if( !x.HasValue )
			return true;

		return Math.Abs( x.Value - y!.Value ) <= defaultPrecision;
	}

	#endregion
}