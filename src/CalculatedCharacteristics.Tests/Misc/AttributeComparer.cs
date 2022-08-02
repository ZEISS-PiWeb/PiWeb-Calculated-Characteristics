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
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// <see cref="IEqualityComparer{T}"/> zum Vergleich zweier <see cref="AttributeDto"/>-Objekte.
	/// If both attribute values are of type <see cref="System.DateTime"/> then they are converted into UTC for comparison.
	/// If both attribute values are of type <see cref="double"/> then they are compared using a precision tolerance of 1e-15 />.
	/// If both attribute values are of type <see cref="CatalogEntryDto"/> then they are compared using the <see cref="CatalogEntryComparer"/>.
	/// </summary>
	internal class AttributeComparer : IEqualityComparer<AttributeDto>
	{
		#region members

		/// <summary>
		/// Eine Instanz dieses Objekts.
		/// </summary>
		public static readonly AttributeComparer Default = new AttributeComparer();

		#endregion

		#region constructors

		private AttributeComparer()
		{ }

		#endregion

		#region methods

		private static bool RawValueEquals( [NotNull] object valueX, [NotNull] object valueY )
		{
			if( ReferenceEquals( valueX, valueY ) )
				return true;

			if( valueX.GetType() != valueY.GetType() )
				return false;

			if( valueX is double doubleX && valueY is double doubleY )
				return IsCloseTo( doubleX, doubleY );

			if( valueX is DateTime dateTimeValueX && valueY is DateTime dateTimeValueY )
				return DateTimeComparer.Default.Equals( dateTimeValueX.ToUniversalTime(), dateTimeValueY.ToUniversalTime() );

			if( valueX is CatalogEntryDto catalogEntryX && valueY is CatalogEntryDto catalogEntryY )
				return CatalogEntryComparer.Default.Equals( catalogEntryX, catalogEntryY );

			return valueX.Equals( valueY );
		}

		private static bool IsCloseTo( double x, double y )
		{
			const double defaultPrecision = 1e-15;

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			return x == y || Math.Abs( x - y ) <= defaultPrecision;
		}

		#endregion

		#region interface IEqualityComparer<AttributeDto>

		/// <summary>
		/// Bestimmt, ob die angegebenen Objekte gleich sind.
		/// </summary>
		public bool Equals( AttributeDto x, AttributeDto y )
		{
			if( x.Key != y.Key ) return false;

			if( x.RawValue != null && y.RawValue != null )
				return RawValueEquals( x.RawValue, y.RawValue );

			return Equals( x.Value, y.Value );
		}

		/// <summary>
		/// Gibt einen Hashcode für das angegebene Objekt zurück.
		/// </summary>
		public int GetHashCode( AttributeDto att )
		{
			// This implementation is good practice. See http://stackoverflow.com/questions/2733541/what-is-the-best-way-to-implement-this-composite-gethashcode.
			unchecked
			{
				var hash = 17;
				hash = hash * 23 + ( att.Key.GetHashCode() );
				hash = hash * 23 + ( att.RawValue?.GetHashCode() ?? 0 );
				hash = hash * 23 + ( att.Value?.GetHashCode() ?? 0 );
				return hash;
			}
		}

		#endregion
	}
}