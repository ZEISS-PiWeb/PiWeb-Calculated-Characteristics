#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	#endregion

	/// <summary>
	/// <see cref="IComparer{T}"/> zum sicheren Vergleich zweier DateTime-Objekte anhand ihrer UTC-Repräsentationen.
	/// </summary>
	internal sealed class DateTimeComparer : IEqualityComparer<DateTime>, IComparer<DateTime>
	{
		#region members

		public static readonly DateTimeComparer Default = new DateTimeComparer();

		#endregion

		#region constructors

		private DateTimeComparer()
		{ }

		#endregion

		#region methods

		private static int Comparison( DateTime x, DateTime y )
		{
			var compareTimeX = ToCompareTime( x );
			var compareTimeY = ToCompareTime( y );

			return DateTime.Compare( compareTimeX, compareTimeY );
		}

		private static bool Equal( DateTime x, DateTime y )
		{
			var compareTimeX = ToCompareTime( x );
			var compareTimeY = ToCompareTime( y );

			return DateTime.Equals( compareTimeX, compareTimeY );
		}

		private static DateTime ToCompareTime( DateTime dateTime )
		{
			Debug.Assert( dateTime.Kind != DateTimeKind.Unspecified, "DateTimeKind of the comparison parameter can't be \'Unspecified\'" );

			return dateTime.ToUniversalTime();
		}

		#endregion

		#region interface IComparer<DateTime>

		public int Compare( DateTime x, DateTime y )
		{
			return Comparison( x, y );
		}

		#endregion

		#region interface IEqualityComparer<DateTime>

		public bool Equals( DateTime x, DateTime y )
		{
			return Equal( x, y );
		}

		public int GetHashCode( DateTime obj )
		{
			return ToCompareTime( obj ).GetHashCode();
		}

		#endregion
	}
}