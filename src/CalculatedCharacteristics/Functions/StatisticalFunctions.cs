#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2025                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions;

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

#endregion

/// <summary>
/// Simple statistical functions for use in <see cref="MathInterpreter"/>.
/// </summary>
public static class StatisticalFunctions
{
	#region methods

	/// <summary>
	/// Minimum.
	/// Expects at least 1 argument to get the smallest value from.
	/// </summary>
	[BasicFunction( "min", "min( value1, value2, ... )" )]
	public static double? Min( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
	{
		if( args.Count == 0 )
			throw new ArgumentException( "Function 'min' requires at least 1 argument!" );

		return args.Min( arg => arg.GetResult( resolver ) );
	}

	/// <summary>
	/// Maximum.
	/// Expects at least 1 argument to get the biggest value from.
	/// </summary>
	[BasicFunction( "max", "max( value1, value2, ... )" )]
	public static double? Max( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
	{
		if( args.Count == 0 )
			throw new ArgumentException( "Function 'max' requires at least 1 argument!" );

		return args.Max( arg => arg.GetResult( resolver ) );
	}

	/// <summary>
	/// Mean.
	/// Expects a variable number of values to be summed up.
	/// Expects at least 1 argument to calculate a mean from.
	/// Ignores 'null' values.
	/// </summary>
	[BasicFunction( "mean", "mean( value1, value2, ... )" )]
	public static double? Mean( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
	{
		switch( args.Count )
		{
			case 0:
				throw new ArgumentException( "Function 'mean' requires at least 1 argument!" );
			case 1:
				return args.ElementAt( 0 ).GetResult( resolver );
			default:
				var result = args.Aggregate( ( (double)0, 0 ), ( sum, m ) =>
				{
					var value = m.GetResult( resolver );
					return value.HasValue ? ( sum.Item1 + value.Value, sum.Item2 + 1 ) : sum;
				} );

				if( result.Item2 == 0 )
					return null;

				return result.Item1 / result.Item2;
		}
	}

	/// <summary>
	/// Median.
	/// Expects a variable number of values.
	/// Expects at least 1 argument to calculate a median from.
	/// Ignores 'null' values.
	/// </summary>
	[BasicFunction( "median", "median( value1, value2, ... )" )]
	public static double? Median( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
	{
		switch( args.Count )
		{
			case 0:
				throw new ArgumentException( "Function 'median' requires at least 1 argument!" );
			case 1:
				return args.ElementAt( 0 ).GetResult( resolver );
			default:
				var values = args
					.Select( m => m.GetResult( resolver ) )
					.Where( v => v.HasValue )
					.OrderBy( v => v!.Value )
					.ToList();

				if( values.Count == 0 )
					return null;

				if( ( values.Count & 1 ) == 1 )
					return values[ ( values.Count - 1 ) / 2 ];

				var idx = values.Count / 2;
				return ( values[ idx ] + values[ idx - 1 ] ) / 2;
		}
	}

	/// <summary>
	/// Count of values that are not "null".
	/// Expects a variable number of values.
	/// Expects at least 1 argument to calculate a count from.
	/// Ignores 'null' values.
	/// </summary>
	[BasicFunction( "count", "count( value1, value2, ... )" )]
	public static double? Count( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
	{
		switch( args.Count )
		{
			case 0:
				throw new ArgumentException( "Function 'count' requires at least 1 argument!" );
			case 1:
				return args.ElementAt( 0 ).GetResult( resolver ) is not null ? 1 : 0;
			default:
				return args
					.Select( m => m.GetResult( resolver ) )
					.Count( v => v.HasValue );
		}
	}

	#endregion
}