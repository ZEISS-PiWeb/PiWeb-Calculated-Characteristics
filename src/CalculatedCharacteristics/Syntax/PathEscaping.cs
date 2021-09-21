#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Syntax
{
	#region usings

	using System.Collections.Generic;
	using System.Text;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	/// <summary>
	/// Provides methods to create an escaped path that can be used in formula supported by <see cref="MathInterpreter"/>.
	/// </summary>
	public static class PathEscaping
	{
		#region members

		private static readonly char[] EscapeTerminals = new[] { '(', ')', '{', '}', '\\', '/', '"' };

		#endregion

		#region methods

		/// <summary>
		/// Creates a string of a relative path with escaping.
		/// </summary>
		/// <param name="path">Path of characteristic.</param>
		/// <param name="relativeTo">Path of relative characteristic.</param>
		/// <returns>Relative path as escaped string.</returns>
		public static string ToRelativeMathInspectionPath( PathInformationDto path, PathInformationDto relativeTo  )
		{
			var relativePath = path.RelativeTo( relativeTo );
			var sharedPathElementCount = path.Count - relativePath.Count;
			var requiredBackSteps = relativeTo.Count - sharedPathElementCount - 1;

			// internal enumeration of string
			IEnumerable<string> CreatePathStrings( int stepBacks, PathInformationDto finalPath )
			{
				for( var i = 0; i < stepBacks; i++ )
				{
					yield return "..";
				}

				foreach( var pathElement in finalPath )
				{
					yield return CheckAndApplyNameEscaping( pathElement.Value );
				}
			}

			return string.Join( "/", CreatePathStrings( requiredBackSteps, relativePath ) );
		}

		/// <summary>
		/// Creates a string of an absolute path with escaping.
		/// </summary>
		/// <param name="absolutePath">Absolute path to characteristic.</param>
		/// <returns>Absolute path as escaped string.</returns>
		public static string ToAbsoluteMathInspectionPath( PathInformationDto absolutePath )
		{
			var stringBuilder = new StringBuilder();
			foreach( var pathElement in absolutePath )
			{
				stringBuilder.Append( "/" );
				stringBuilder.Append( CheckAndApplyNameEscaping( pathElement.Value ) );
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Checks whether a path element name needs escaping and applies it if required.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string CheckAndApplyNameEscaping( string path )
		{
			if( path.IndexOfAny( EscapeTerminals ) == -1 )
				return path;

			return '"' + path.Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" ) + '"';
		}

		#endregion
	}
}