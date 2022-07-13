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

	using System.Collections.Generic;
	using System.Linq;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// <see cref="IEqualityComparer{T}"/> zum Vergleich zweier <see cref="CatalogEntryDto"/>-Objekte.
	/// </summary>
	internal class CatalogEntryComparer : IEqualityComparer<CatalogEntryDto>
	{
		#region members

		/// <summary>
		/// Eine Instanz dieses Objekts.
		/// </summary>
		public static readonly CatalogEntryComparer Default = new CatalogEntryComparer();

		#endregion

		#region methods

		/// <summary>
		/// Bestimmt, ob die angegebenen Objekte gleich sind.
		/// </summary>
		private static bool Equals( CatalogEntryDto a, CatalogEntryDto b, bool ignoreKey )
		{
			if( ReferenceEquals( a, b ) ) return true;

			if( a != null && b != null && ( ignoreKey || a.Key == b.Key ) && a.Attributes.Count == b.Attributes.Count )
			{
				return a.Attributes.SequenceEqual( b.Attributes, AttributeComparer.Default );
			}

			return false;
		}

		#endregion

		#region interface IEqualityComparer<CatalogEntryDto>

		/// <summary>
		/// Bestimmt, ob die angegebenen Objekte gleich sind.
		/// </summary>
		public bool Equals( CatalogEntryDto a, CatalogEntryDto b )
		{
			return Equals( a, b, false );
		}

		/// <summary>
		/// Gibt einen Hashcode für das angegebene Objekt zurück.
		/// </summary>
		public int GetHashCode( CatalogEntryDto e )
		{
			return e.Key;
		}

		#endregion
	}
}