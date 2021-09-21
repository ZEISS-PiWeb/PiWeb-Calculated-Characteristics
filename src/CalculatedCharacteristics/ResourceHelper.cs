#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Concurrent;
	using System.Globalization;
	using System.Resources;
	using JetBrains.Annotations;
	using log4net;

	#endregion

	/// <summary>
	/// Helper class to get translated strings from resource files for localization.
	/// </summary>
	internal static class LocalizationHelper
	{
		#region members

		private static readonly ILog Log;
		private static readonly ConcurrentDictionary<(string, string), string> Cache = new ConcurrentDictionary<(string, string), string>();
		private static readonly ConcurrentDictionary<(string, string), string> InvariantCache = new ConcurrentDictionary<(string, string), string>();

		private static CultureInfo _CurrentCacheCulture;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="LocalizationHelper"/>.
		/// </summary>
		static LocalizationHelper()
		{
			Log = LogManager.GetLogger( typeof( LocalizationHelper ) );
		}

		#endregion

		#region methods

		/// <summary>
		/// Ermittelt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, lokalisierten
		/// String, interpretiert diesen als Formatstring und formatiert diesen mittels <see cref="string.Format(string,object)"/> und
		/// den angegebenen Argumenten <paramref name="args"/>.
		/// </summary>
		/// <param name="type">Der Typ, zu dem der String gehört.</param>
		/// <param name="args">Die Argumente die der <see cref="string.Format(string,object)"/>-Routine übergeben werden.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string Format( [NotNull] Type type, [NotNull] string name, params object[] args )
		{
			return string.Format( Get( type, name ), args );
		}

		/// <summary>
		/// Ermittelt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, lokalisierten
		/// String, interpretiert diesen als Formatstring und formatiert diesen mittels <see cref="string.Format(string,object)"/> und
		/// den angegebenen Argumenten <paramref name="args"/>.
		/// </summary>
		/// <typeparam name="T">Der Typ, zu dem der String gehört.</typeparam>
		/// <param name="args">Die Argumente die der <see cref="string.Format(string,object)"/>-Routine übergeben werden.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string Format<T>( [NotNull] string name, params object[] args )
		{
			return Format( typeof( T ), name, args );
		}

		/// <summary>
		/// Ermittelt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, invarianten
		/// String, interpretiert diesen als Formatstring und formatiert diesen mittels <see cref="string.Format(string,object)"/> und
		/// den angegebenen Argumenten <paramref name="args"/>.
		/// </summary>
		/// <param name="type">Der Typ, zu dem der String gehört.</param>
		/// <param name="args">Die Argumente die der <see cref="string.Format(string,object)"/>-Routine übergeben werden.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string FormatInvariant( [NotNull] Type type, [NotNull] string name, params object[] args )
		{
			return string.Format( GetInvariant( type, name ), args );
		}

		/// <summary>
		/// Ermittelt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, invarianten
		/// String, interpretiert diesen als Formatstring und formatiert diesen mittels <see cref="string.Format(string,object)"/> und
		/// den angegebenen Argumenten <paramref name="args"/>.
		/// </summary>
		/// <typeparam name="T">Der Typ, zu dem der String gehört.</typeparam>
		/// <param name="args">Die Argumente die der <see cref="string.Format(string,object)"/>-Routine übergeben werden.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string FormatInvariant<T>( [NotNull] string name, params object[] args )
		{
			return FormatInvariant( typeof( T ), name, args );
		}

		/// <summary>
		/// Ermittelt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, lokalisierten
		/// String, interpretiert diesen als Formatstring und formatiert diesen mittels <see cref="string.Format(string,object)"/> und
		/// den angegebenen Argumenten <paramref name="args"/>.
		/// </summary>
		/// <param name="type">Der Typ, zu dem der String gehört.</param>
		/// <param name="culture">Die Kultur, für die die Lokalisierung angefordert wird.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <param name="args">Die Argumente die der <see cref="string.Format(string,object)"/>-Routine übergeben werden.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string Format( [NotNull] Type type, [NotNull] CultureInfo culture, [NotNull] string name, params object[] args )
		{
			return string.Format( Get( type, name, culture ), args );
		}

		/// <summary>
		/// Ermittelt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, lokalisierten
		/// String, interpretiert diesen als Formatstring und formatiert diesen mittels <see cref="string.Format(string,object)"/> und
		/// den angegebenen Argumenten <paramref name="args"/>.
		/// </summary>
		/// <typeparam name="T">Der Typ, zu dem der String gehört.</typeparam>
		/// <param name="culture">Die Kultur, für die die Lokalisierung angefordert wird.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <param name="args">Die Argumente die der <see cref="string.Format(string,object)"/>-Routine übergeben werden.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string Format<T>( [NotNull] CultureInfo culture, [NotNull] string name, params object[] args )
		{
			return string.Format( Get( typeof( T ), name, culture ), args );
		}

		/// <summary>
		/// Gibt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, lokalisierten
		/// String zurück.
		/// </summary>
		/// <param name="type">Der Typ, zu dem der String gehört.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string Get( [NotNull] Type type, [NotNull] string name )
		{
			return InnerGet( type, name, Cache, CultureInfo.CurrentUICulture, !Equals( _CurrentCacheCulture, CultureInfo.CurrentUICulture ) );
		}

		/// <summary>
		/// Gibt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, lokalisierten
		/// String zurück.
		/// </summary>
		/// <param name="type">Der Typ, zu dem der String gehört.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <param name="culture">Die Kultur, für die die Lokalisierung angefordert wird.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string Get( [NotNull] Type type, [NotNull] string name, [NotNull] CultureInfo culture )
		{
			if( culture == null ) throw new ArgumentNullException( nameof( culture ) );

			return InnerGet( type, name, Cache, culture, !Equals( _CurrentCacheCulture, culture ) );
		}

		/// <summary>
		/// Gibt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, lokalisierten
		/// String zurück.
		/// </summary>
		/// <typeparam name="T">Der Typ, zu dem der String gehört.</typeparam>
		/// <param name="name">Der Name des Strings.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string Get<T>( [NotNull] string name )
		{
			return Get( typeof( T ), name );
		}

		/// <summary>
		/// Gibt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, lokalisierten
		/// String zurück.
		/// </summary>
		/// <typeparam name="T">Der Typ, zu dem der String gehört.</typeparam>
		/// <param name="name">Der Name des Strings.</param>
		/// <param name="culture">Die Kultur, für die die Lokalisierung angefordert wird.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string Get<T>( [NotNull] string name, [NotNull] CultureInfo culture )
		{
			if( culture == null ) throw new ArgumentNullException( nameof( culture ) );

			return Get( typeof( T ), name, culture );
		}

		/// <summary>
		/// Gibt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, invarianten
		/// String zurück.
		/// </summary>
		/// <param name="type">Der Typ, zu dem der String gehört.</param>
		/// <param name="name">Der Name des Strings.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string GetInvariant( [NotNull] Type type, [NotNull] string name )
		{
			return InnerGet( type, name, InvariantCache, CultureInfo.InvariantCulture, false );
		}

		/// <summary>
		/// Gibt den zu einem Typ zugeordneten und unter dem angegebenen Namen gespeicherten, invarianten
		/// String zurück.
		/// </summary>
		/// <typeparam name="T">Der Typ, zu dem der String gehört.</typeparam>
		/// <param name="name">Der Name des Strings.</param>
		/// <returns>Den gefundenen String oder einen leeren String.</returns>
		public static string GetInvariant<T>( [NotNull] string name )
		{
			return GetInvariant( typeof( T ), name );
		}

		private static string InnerGet(
			[NotNull] Type type,
			[NotNull] string name,
			ConcurrentDictionary<(string, string), string> cache,
			CultureInfo ci,
			bool clearCacheAndSaveCurrentCulture )
		{
			var typeName = GetNonGenericTypeName( type );

			if( type == null || typeName == null )
				throw new ArgumentNullException( nameof( type ), string.Intern( "Type must be supplied for retrieving a resource string!" ) );
			if( string.IsNullOrWhiteSpace( name ) )
				throw new ArgumentNullException( nameof( name ), string.Intern( "Name must be supplied for retrieving a resource string!" ) );

			if( clearCacheAndSaveCurrentCulture )
			{
				cache.Clear();
				_CurrentCacheCulture = ci;
			}

			var cacheName = ( typeName, name );

			if( cache.TryGetValue( cacheName, out var cachedString ) )
				return cachedString;

			cachedString = ReadResource( type, name, ci, typeName );
			cache.TryAdd( cacheName, cachedString );

			return cachedString;
		}

		private static string ReadResource( Type type, string name, CultureInfo ci, string typeName )
		{
			var rm = new ResourceManager( typeName, type.Assembly );

			string str;
			try
			{
				str = rm.GetString( name, ci );
			}
			catch( Exception ex ) when( ex is MissingManifestResourceException || ex is MissingSatelliteAssemblyException )
			{
				Log.Warn( $"Could not find resources \"{name}\" for type \"{typeName}\" and culture \"{ci.Name}\"" );
				str = $"#{name}";
			}

			return str != null ? string.Intern( str ) : "";
		}

		/// <summary>
		/// Provides fullname of type with removed generic postfix.
		/// </summary>
		private static string GetNonGenericTypeName( Type type )
		{
			return type.IsGenericType ? type.FullName?.Substring( 0, type.FullName.IndexOf( '`' ) ) : type.FullName;
		}

		#endregion
	}
}