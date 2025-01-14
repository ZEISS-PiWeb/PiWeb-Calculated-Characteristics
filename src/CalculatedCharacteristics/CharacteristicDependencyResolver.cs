#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2016                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System.Collections.Generic;
	using System.Linq;
	using Zeiss.PiWeb.Api.Core;

	#endregion

	/// <summary>
	/// Resolves characteristic dependencies.
	/// </summary>
	public class CharacteristicDependencyResolver
	{
		#region members

		private readonly EntityAttributeValueHandler _EntityAttributeValueHandler;
		private readonly CharacteristicCalculatorFactory _CalculatorFactory;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="CharacteristicDependencyResolver"/>.
		/// </summary>
		public CharacteristicDependencyResolver(
			ChildPathsHandler childPathsHandler,
			EntityAttributeValueHandler entityAttributeValueHandler,
			PathResolverFactory? resolvePathHandler = null )
		{
			_EntityAttributeValueHandler = entityAttributeValueHandler;

			var mathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, key ) => entityAttributeValueHandler( path, key, null ),
				childPathsHandler,
				resolvePathHandler );
			_CalculatorFactory = path => mathInterpreterFactory.GetCharacteristicCalculator( path );
		}

		#endregion

		#region methods

		/// <summary>
		/// Gets the paths of all characteristics directly or indirectly referenced in the characteristic's formula.
		/// Returns an empty list if the formula contains a syntax error or a circular characteristics reference.
		/// </summary>
		/// <param name="path">The path of the characteristic.</param>
		/// <returns>The paths of the referenced characteristics.</returns>
		public IEnumerable<PathInformation> GetDependentCharacteristics( PathInformation path )
		{
			try
			{
				return GetDependentCharacteristics( path, _CalculatorFactory, _EntityAttributeValueHandler );
			}
			catch
			{
				// Exceptions while parsing the formula or
				// traversing the parsed formula tree for dependent characteristics can be ignored
				return [];
			}
		}

		/// <summary>
		/// Checks whether dependencies required by a calculated characteristic are valid.
		/// </summary>
		/// <param name="path">The path of the characteristic.</param>
		/// <param name="characteristicCalculatorFactory">The delegate to get the calculator for a characteristic.</param>
		/// <param name="entityAttributeValueHandler">A handler to get attribute values for an inspection plan entity.</param>
		/// <exception cref="CircularReferenceException">If the formula contains a circular characteristics reference.</exception>
		internal static void ValidateDependencies(
			PathInformation path,
			CharacteristicCalculatorFactory characteristicCalculatorFactory,
			EntityAttributeValueHandler entityAttributeValueHandler )
		{
			GetDependentCharacteristics( path, characteristicCalculatorFactory, entityAttributeValueHandler );
		}

		/// <summary>
		/// Gets the dependencies required by a calculated characteristic.
		/// </summary>
		/// <param name="path">The path of the characteristic.</param>
		/// <param name="characteristicCalculatorFactory">The delegate to get the calculator for a characteristic.</param>
		/// <param name="entityAttributeValueHandler">A handler to get attribute values for an inspection plan entity.</param>
		/// <exception cref="CircularReferenceException">Resolving the characteristic formula detected a circular characteristic reference.</exception>
		private static IEnumerable<PathInformation> GetDependentCharacteristics(
			PathInformation path,
			CharacteristicCalculatorFactory characteristicCalculatorFactory,
			EntityAttributeValueHandler entityAttributeValueHandler )
		{
			return EnumerateDependentCharacteristics(
					path,
					characteristicCalculatorFactory,
					entityAttributeValueHandler,
					new Stack<PathInformation>( 4 ) )
				.Distinct();
		}

		/// <summary>
		/// Get all paths of dependent characteristics used in formula of a calculated characteristic.
		/// </summary>
		/// <remarks>
		/// This method also provides characteristic paths in formulas of used characteristics.
		/// </remarks>
		private static IEnumerable<PathInformation> EnumerateDependentCharacteristics(
			PathInformation path,
			CharacteristicCalculatorFactory characteristicCalculatorFactory,
			EntityAttributeValueHandler entityAttributeValueHandler,
			Stack<PathInformation> stackTrace )
		{
			var calculator = characteristicCalculatorFactory( path );
			if( calculator == null )
				return [];

			var dependentCharsFromFormula = calculator.GetDependentCharacteristics( entityAttributeValueHandler )
				.Select( p => ( p.Key, p.Value.All( mdi => mdi.AttributeKey.HasValue ) ) )
				.ToArray();

			stackTrace.Push( path );

			var resultingDependentChars = dependentCharsFromFormula.Select( p => p.Key );
			foreach( var (dependentPath, isAttributeOnly) in dependentCharsFromFormula )
			{
				if( isAttributeOnly )
					continue;

				if( stackTrace.Contains( dependentPath ) )
					throw new CircularReferenceException( $"The characteristic '{path}' directly or indirectly references itself in its formula." );

				resultingDependentChars = resultingDependentChars.Concat(
					EnumerateDependentCharacteristics(
						dependentPath,
						characteristicCalculatorFactory,
						entityAttributeValueHandler,
						stackTrace ) );
			}

			stackTrace.Pop();

			return resultingDependentChars;
		}

		#endregion
	}
}