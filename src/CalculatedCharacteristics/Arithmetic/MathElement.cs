#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic
{
	#region usings

	using System.Collections.Generic;
	using System.Linq;

	#endregion

	/// <summary>
	/// Base class that represents a mathematical element that was parsed from a formula (constant, function, number,...).
	/// </summary>
	public abstract class MathElement
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		protected MathElement( int startPosition, int length, IReadOnlyCollection<MathElement>? arguments = null )
		{
			TokenStartPosition = startPosition;
			TokenLength = length;
			Arguments = arguments ?? [];
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the position of the element in the formula.
		/// </summary>
		public int TokenStartPosition { get; }

		/// <summary>
		/// Gets the length of the element in the formula.
		/// </summary>
		public int TokenLength { get; }

		/// <summary>
		/// Gets the children of the element (e.g. arguments of a function).
		/// </summary>
		public IReadOnlyCollection<MathElement> Arguments { get; }

		#endregion

		#region methods

		/// <summary>
		/// Returns the value of the mathematical element.
		/// For the calculation the given <paramref name="resolver"/> is used.
		/// </summary>
		public abstract double? GetResult( ICharacteristicValueResolver resolver );

		/// <summary>
		/// Provides an enumeration of <see cref="MathDependencyInformation"/> the math element depends on.
		/// </summary>
		internal IEnumerable<MathDependencyInformation> CheckForDependentCharacteristics( ICharacteristicInfoResolver resolver )
		{
			return OnCheckForDependentCharacteristics( resolver );
		}

		/// <summary>
		/// Provides an enumeration of <see cref="MathDependencyInformation"/> the math element depends on.
		/// </summary>
		protected virtual IEnumerable<MathDependencyInformation> OnCheckForDependentCharacteristics( ICharacteristicInfoResolver resolver )
		{
			var enumeration = Enumerable.Empty<MathDependencyInformation>();
			foreach( var child in Arguments )
				enumeration = enumeration.Concat( child.OnCheckForDependentCharacteristics( resolver ) );

			return enumeration;
		}

		#endregion
	}
}