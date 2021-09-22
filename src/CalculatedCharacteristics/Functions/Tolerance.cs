#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	#region usings

	using System;

	#endregion

	/// <summary>
	/// Represents an interval with upper and lower limit bound.
	/// </summary>
	internal readonly struct Tolerance
	{
		#region members

		/// <summary>
		/// Creates an unbounded interval, which means upper and lower bound are undefined.
		/// </summary>
		public static readonly Tolerance Unbounded = new Tolerance();

		#endregion

		#region constructors

		/// <summary>
		/// Constructor. Initializes new interval with given upper and lower bound.
		/// </summary>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="lowerBound">The lower bound.</param>
		/// <remarks>If lower limit is bigger than upper bound, they are swapped.</remarks>
		public Tolerance( double? lowerBound, double? upperBound )
		{
			UpperBound = upperBound;
			LowerBound = lowerBound;

			if( lowerBound.HasValue && upperBound.HasValue && lowerBound.Value > upperBound.Value )
			{
				UpperBound = lowerBound;
				LowerBound = upperBound;
			}
		}

		#endregion

		#region properties

		/// <summary>
		/// Returns the upper bound.
		/// </summary>
		public double? UpperBound { get; }

		/// <summary>
		/// Returns the lower bound.
		/// </summary>
		public double? LowerBound { get; }

		/// <summary>
		/// Returns the middle of the interval. If the interval is not bounded <see langword="null"/> is returned.
		/// </summary>
		public double? Middle => IsPartiallyUnbounded ? null : 0.5 * ( LowerBound!.Value + UpperBound!.Value );

		/// <summary>
		/// Returns the intervals length.
		/// </summary>
		public double? Length => UpperBound.HasValue && LowerBound.HasValue ? UpperBound.Value - LowerBound.Value : null;

		/// <summary>
		/// Returns <see langword="true"/> if the interval is empty, which means upper and lower bound are the same.
		/// </summary>
		public bool IsEmpty => IsClose( Length, 0 );

		/// <summary>
		/// Returns <see langword="true"/> if the interval is unbounded, which means upper and lower bound are undefined.
		/// </summary>
		public bool IsUnbounded => !LowerBound.HasValue && !UpperBound.HasValue;

		/// <summary>
		/// Returns <see langword="true"/> if at least one bound is undefined.
		/// </summary>
		public bool IsPartiallyUnbounded => !UpperBound.HasValue || !LowerBound.HasValue;

		#endregion

		#region methods

		/// <summary>
		/// Returns <see langword="true"/> if the interval contains the given value; otherwise <see langword="false"/>.
		/// </summary>
		/// <param name="value">The value in question</param>
		public bool Contains( double value )
		{
			if( UpperBound.HasValue && LowerBound.HasValue )
				return IsGreaterThanOrClose( UpperBound.Value, value ) && IsLessThanOrClose( LowerBound.Value, value );

			if( UpperBound.HasValue )
				return IsGreaterThanOrClose( UpperBound.Value, value );

			if( LowerBound.HasValue )
				return IsLessThanOrClose( LowerBound.Value, value );

			return true;
		}

		/// <summary>
		/// Equality operator. The boundaries are compared using a tolerance.
		/// </summary>
		public static bool operator ==( Tolerance v1, Tolerance v2 )
		{
			return IsClose( v1.LowerBound, v2.LowerBound ) && IsClose( v1.UpperBound, v2.UpperBound );
		}

		/// <summary>
		/// Inequality operator. The boundaries are compared using a tolerance.
		/// </summary>
		public static bool operator !=( Tolerance v1, Tolerance v2 )
		{
			return !( v1 == v2 );
		}

		/// <inheritdoc />
		public override bool Equals( object obj )
		{
			return obj is Tolerance tolerance && this == tolerance;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCode.Combine( UpperBound, LowerBound );
		}

		private static bool IsClose( double? x, double? y )
		{
			const double defaultPrecision = 1e-15;

			if( x.HasValue != y.HasValue )
				return false;

			if( !x.HasValue )
				return true;

			return Math.Abs( x.Value - y.Value ) <= defaultPrecision;
		}

		private static bool IsGreaterThanOrClose( double x, double y )
		{
			return x >= y || IsClose( x, y );
		}

		private static bool IsLessThanOrClose( double x, double y )
		{
			return x <= y || IsClose( x, y );
		}

		#endregion
	}
}