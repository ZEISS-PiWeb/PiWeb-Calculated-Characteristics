#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2022                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	#region usings

	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;
	using System.Collections.Generic;
	using System.Linq;
	using System;
	using JetBrains.Annotations;

	#endregion

	public static partial class OprFunctions
	{
		private static (IReadOnlyList<Characteristic> CHaracteristics, string Direction) AnalyzeArguments( [NotNull] IReadOnlyCollection<MathElement> args, string name, int requiredCharacteristicsCount, bool allowMultipleCharacteristics, [CanBeNull] IReadOnlyCollection<string> literalPattern = null )
		{
			var characteristics = GetCharacteristics( args );
			CheckArgumentsForRequiredCharacteristicsCount( characteristics, args.Count, name, requiredCharacteristicsCount, allowMultipleCharacteristics );

			if( characteristics.Any( c => c.AttributeKey.HasValue ) )
				throw new ArgumentException( $"Function '{name}' does not support using characteristic attributes!" );

			var direction = ValidateDirectionLiteral( args, name, literalPattern );

			return ( characteristics, direction );
		}

		private static (IReadOnlyList<Characteristic> CHaracteristics, string Direction) AnalyzeArguments(
			[NotNull] IReadOnlyCollection<MathElement> args,
			string name,
			int minRequiredCharacteristicsCount,
			int maxRequiredCharacteristicsCount,
			[CanBeNull] IReadOnlyCollection<string> literalPattern = null )
		{
			var characteristics = GetCharacteristics( args );
			CheckArgumentsForRequiredCharacteristicsCount( characteristics, args.Count, name, minRequiredCharacteristicsCount, maxRequiredCharacteristicsCount );

			if( characteristics.Any( c => c.AttributeKey.HasValue ) )
				throw new ArgumentException( $"Function '{name}' does not support using characteristic attributes!" );

			var direction = ValidateDirectionLiteral( args, name, literalPattern );

			return ( characteristics, direction );
		}

		private static string ValidateDirectionLiteral( [NotNull] IReadOnlyCollection<MathElement> args, string name, [CanBeNull] IReadOnlyCollection<string> literalPattern )
		{
			var direction = GetDirection( args );
			if( string.IsNullOrEmpty( direction ) )
				throw new ArgumentException( $"Function '{name}' requires a literal as parameter after the characteristics parameters!" );

			if( literalPattern != null && !literalPattern.Contains( direction ) )
			{
				throw new ArgumentException( $"Function '{name}' requires a literal {literalPattern} as its last parameter!" );
			}

			return direction;
		}

		private static void CheckArgumentsForRequiredCharacteristicsCount(
			IReadOnlyCollection<Characteristic> characteristics,
			int argsCount,
			string name,
			int requiredCharacteristicsCount,
			bool allowMultipleCharacteristics )
		{
			if( allowMultipleCharacteristics )
			{
				if( argsCount < requiredCharacteristicsCount + 1 )
					throw new ArgumentException( $"Function '{name}' requires at least {requiredCharacteristicsCount + 1} parameters!" );

				if( characteristics.Count < requiredCharacteristicsCount )
					throw new ArgumentException( $"Function '{name}' requires at least {requiredCharacteristicsCount} characteristics as its first parameter!" );
			}
			else
			{
				if( argsCount != requiredCharacteristicsCount + 1 )
					throw new ArgumentException( $"Function 'name' requires {requiredCharacteristicsCount + 1} parameters!" );

				if( characteristics.Count != requiredCharacteristicsCount )
					throw new ArgumentException( $"Function '{name}' requires at least {requiredCharacteristicsCount} characteristics as its first parameter!" );
			}
		}

		private static void CheckArgumentsForRequiredCharacteristicsCount(
			IReadOnlyCollection<Characteristic> characteristics,
			int argsCount,
			string name,
			int minRequiredCharacteristicsCount,
			int maxRequiredCharacteristicsCount )
		{
				if( characteristics.Count < minRequiredCharacteristicsCount )
					throw new ArgumentException( $"Function '{name}' requires at least {minRequiredCharacteristicsCount} characteristics as its first parameter!" );

				if( characteristics.Count > maxRequiredCharacteristicsCount )
					throw new ArgumentException( $"Function '{name}' only supports maximum of{maxRequiredCharacteristicsCount} characteristics as its first parameter!" );

				if( argsCount != characteristics.Count + 1 )
					throw new ArgumentException( $"Function 'name' requires {characteristics.Count + 1} parameters!" );
		}
	}
}