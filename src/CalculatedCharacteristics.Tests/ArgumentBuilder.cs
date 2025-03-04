#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2025                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests;

#region usings

using System.Linq;
using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

#endregion

/// <summary>
/// Provides methods to create math elements for argument lists.
/// </summary>
internal static class ArgumentBuilder
{
	#region members

	private static readonly Function NullReturningFunction =
		new Function(
			0,
			0,
			null,
			"ReturnNull",
			new MathOperation(
				( _, _ ) => null,
				( _, _ ) => [] ) );

	#endregion

	#region methods

	public static MathElement[] CreateArguments( params double[] argumentValues )
	{
		return CreateArguments( argumentValues.Select( v => v as double? ).ToArray() );
	}

	public static MathElement[] CreateArguments( params double?[] argumentValues )
	{
		if( argumentValues.Length == 0 )
			return [];

		return argumentValues.Select( CreateArgument ).ToArray();
	}

	public static MathElement CreateArgument( double? value )
	{
		if( value.HasValue )
			return new Number( 0, 0, value.Value );

		return NullReturningFunction;
	}

	#endregion
}