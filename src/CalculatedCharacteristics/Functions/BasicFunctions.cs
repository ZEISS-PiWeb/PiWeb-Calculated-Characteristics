#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Basic functions for use in <see cref="MathInterpreter"/>.
	/// </summary>
	public static class BasicFunctions
	{
		#region methods

		// Basic mathematical operations: +,-,*,/

		/// <summary>
		/// Addition.
		/// Expects 2 arguments to be added up.
		/// </summary>
		[BasicFunction( "+", "value1 + value2" )]
		public static double? Add( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 2 )
				throw new ArgumentException( "Operation '+' requires 2 arguments!" );

			return args.ElementAt( 0 ).GetResult( resolver ) + args.ElementAt( 1 ).GetResult( resolver );
		}

		/// <summary>
		/// Subtraction.
		/// Expects 2 arguments, the second will be subtracted from the first.
		/// </summary>
		[BasicFunction( "-", "value1 - value2" )]
		public static double? Sub( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 2 )
				throw new ArgumentException( "Operation '-' requires 2 arguments!" );

			return args.ElementAt( 0 ).GetResult( resolver ) - args.ElementAt( 1 ).GetResult( resolver );
		}

		/// <summary>
		/// Multiplication.
		/// Expects 2 arguments to be multiplied.
		/// </summary>
		[BasicFunction( "*", "value1 * value2" )]
		public static double? Mul( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 2 )
				throw new ArgumentException( "Operation '*' requires 2 arguments!" );

			return args.ElementAt( 0 ).GetResult( resolver ) * args.ElementAt( 1 ).GetResult( resolver );
		}

		/// <summary>
		/// Division.
		/// Expects 2 arguments, the first will be divided by the second.
		/// </summary>
		[BasicFunction( "/", "value1 / value2" )]
		public static double? Div( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 2 )
				throw new ArgumentException( "Operation '/' requires 2 arguments!" );

			return args.ElementAt( 0 ).GetResult( resolver ) / args.ElementAt( 1 ).GetResult( resolver );
		}

		/// <summary>
		/// Sum.
		/// Expects a variable number of values to be summed up.
		/// Expects at least 1 argument to calculate a sum from.
		/// Returns 'null' if the value cannot be determined for one of the arguments.
		/// </summary>
		[BasicFunction( "sum", "sum( value1, value2, ... )" )]
		public static double? Sum( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			switch( args.Count )
			{
				case 0:
					throw new ArgumentException( "Function 'sum' requires at least 1 argument!" );
				case 1:
					return args.ElementAt( 0 ).GetResult( resolver );
				default:
					return args.Aggregate( (double?)0, ( sum, m ) => sum + m.GetResult( resolver ) );
			}
		}

		// Trigonometric functions

		/// <summary>
		/// Sine.
		/// Expects 1 argument to calculate the sine value for.
		/// </summary>
		[BasicFunction( "sin", "sin( value )" )]
		public static double? Sin( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'sin' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Sin( result.Value );
		}

		/// <summary>
		/// Cosine.
		/// Expects 1 argument to calculate the cosine value for.
		/// </summary>
		[BasicFunction( "cos", "cos( value )" )]
		public static double? Cos( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'cos' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Cos( result.Value );
		}

		/// <summary>
		/// Tangent.
		/// Expects 1 argument to calculate the tangent value for.
		/// </summary>
		[BasicFunction( "tan", "tan( value )" )]
		public static double? Tan( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'Tan' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Tan( result.Value );
		}

		/// <summary>
		/// Cotangent [ 1.0 / tan(x) ].
		/// Expects 1 argument to calculate the cotangent value for.
		/// </summary>
		[BasicFunction( "cot", "cot( value )" )]
		public static double? Cot( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'cot' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return 1.0 / Math.Tan( result.Value );
		}

		/// <summary>
		/// Radian.
		/// Expects 1 argument (degree value) to be converted into a radian value.
		/// </summary>
		[BasicFunction( "rad", "rad( value )" )]
		public static double? Rad( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'rad' requires 1 argument!" );

			return Math.PI * args.First().GetResult( resolver ) / 180.0;
		}

		/// <summary>
		/// Degree.
		/// Expects 1 argument (radian value) to be converted into a degree value.
		/// </summary>
		[BasicFunction( "deg", "deg( value )" )]
		public static double? Deg( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'deg' requires 1 argument!" );

			return 180.0 / Math.PI * args.First().GetResult( resolver );
		}

		/// <summary>
		/// Arc sine.
		/// Expects 1 argument to calculate the arc sine value for.
		/// </summary>
		[BasicFunction( "asin", "asin( value )" )]
		public static double? Asin( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'asin' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Asin( result.Value );
		}

		/// <summary>
		/// Arc cosine.
		/// Expects 1 argument to calculate the arc cosine value for.
		/// </summary>
		[BasicFunction( "acos", "acos( value )" )]
		public static double? Acos( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'acos' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Acos( result.Value );
		}

		/// <summary>
		/// Arc tangent.
		/// Expects 1 argument to calculate the arc tangent value for.
		/// </summary>
		[BasicFunction( "atan", "atan( value )" )]
		public static double? Atan( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'atan' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Atan( result.Value );
		}

		// Further functions

		/// <summary>
		/// Square.
		/// Expects 1 argument to calculate the square value for.
		/// </summary>
		[BasicFunction( "sqr", "sqr( value )" )]
		public static double? Sqr( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'sqr' requires 1 argument!" );

			var tmp = args.First().GetResult( resolver );
			return tmp * tmp;
		}

		/// <summary>
		/// Square root.
		/// Expects 1 argument to calculate the square root value for.
		/// </summary>
		[BasicFunction( "sqrt", "sqrt( value )" )]
		public static double? Sqrt( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'sqrt' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Sqrt( result.Value );
		}

		/// <summary>
		/// Exponential function [ e^x ].
		/// Expects 1 argument to calculate e^x for.
		/// </summary>
		[BasicFunction( "exp", "exp( value )" )]
		public static double? Exp( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'exp' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Exp( result.Value );
		}

		/// <summary>
		/// Natural logarithm.
		/// Expects 1 argument to calculate the natural logarithm value for.
		/// </summary>
		[BasicFunction( "ln", "ln( value )" )]
		public static double? Ln( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'ln' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Log( result.Value );
		}

		/// <summary>
		/// Absolute value.
		/// Expects 1 argument to get the absolute value for.
		/// </summary>
		[BasicFunction( "abs", "abs( value )" )]
		public static double? Abs( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'abs' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Abs( result.Value );
		}

		/// <summary>
		/// Sign.
		/// Expects 1 argument to get the sign for.
		/// </summary>
		[BasicFunction( "sgn", "sgn( value )" )]
		public static double? Sgn( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'sgn' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			return Math.Sign( result.Value );
		}

		/// <summary>
		/// Greater Zero.
		/// Expects 1 argument to calculate the value greater zero for.
		/// Returns '0' if the argument is less or equal to '0',
		/// else the argument is returned.
		/// </summary>
		[BasicFunction( "gz", "gz( value )" )]
		public static double? Gz( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'gz' requires 1 argument!" );

			var tmp = args.First().GetResult( resolver );
			if( !tmp.HasValue )
				return null;

			return tmp < 0 ? 0 : tmp;
		}

		/// <summary>
		/// Random number.
		/// Expects 1 argument to be rounded to an integer and then used as seed for the random generator.
		/// </summary>
		[BasicFunction( "rnd", "rnd( value )" )]
		public static double? Rnd( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 1 )
				throw new ArgumentException( "Function 'rnd' requires 1 argument!" );

			var result = args.First().GetResult( resolver );
			if( !result.HasValue )
				return null;

			var r = new Random( (int)Math.Round( Math.Abs( result.Value ) ) );
			return r.NextDouble();
		}

		/// <summary>
		/// Rounding.
		/// First argument is the value, second argument is the precision (optional).
		/// </summary>
		[BasicFunction( "round", "round( value, [ decimals ] )" )]
		public static double? Round( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count is 0 or > 2 )
				throw new ArgumentException( "Function 'round' requires 1 or 2 arguments!" );

			var value = args.ElementAt( 0 ).GetResult( resolver );
			if( !value.HasValue )
				return null;

			var precision = 0;

			if( args.Count == 2 )
			{
				var precisionValue = args.ElementAt( 1 ).GetResult( resolver );
				if( precisionValue.HasValue )
					precision = (int)precisionValue.Value;
			}

			if( precision < 0 )
				throw new ArgumentException( "Decimal places in 'round' must be greater than 0!" );

			// .NET-Core 2.1 and higher shall use MidpointRounding.ToEven when rounding
			var result = Math.Round( value.Value, precision, MidpointRounding.ToEven );

			// prevent negative 0
			return result == 0 ? 0 : result;
		}

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
		/// IfNV ("if no value") a la Excel.
		/// Expects 2 arguments.
		/// Returns the first argument if it has a value, else the second argument is returned.
		/// </summary>
		[BasicFunction( "ifnv", "ifnv( value, thenvalue )" )]
		public static double? IfNotValue( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 2 )
				throw new ArgumentException( "Function 'ifnv' requires 2 arguments!" );

			return args.ElementAt( 0 ).GetResult( resolver ) ?? args.ElementAt( 1 ).GetResult( resolver );
		}

		/// <summary>
		/// Power [ x^y ].
		/// Raises the first argument (base, x) to the power specified by the second argument (exponent, y).
		/// </summary>
		[BasicFunction( "pow", "pow( base, exponent )" )]
		public static double? Pow( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			if( args.Count != 2 )
				throw new ArgumentException( "Function 'pow' requires 2 arguments!" );

			var value = args.ElementAt( 0 ).GetResult( resolver );
			var exponent = args.ElementAt( 1 ).GetResult( resolver );

			if( !value.HasValue || !exponent.HasValue )
				return null;

			return Math.Pow( value.Value, exponent.Value );
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

		#endregion
	}
}