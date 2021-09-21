#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics.Arithmetic
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;

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
		protected MathElement( int startPosition, int length, IReadOnlyCollection<MathElement> arguments = null )
		{
			TokenStartPosition = startPosition;
			TokenLength = length;
			Arguments = arguments ?? Array.Empty<MathElement>();
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
		[NotNull]
		public IReadOnlyCollection<MathElement> Arguments { get; }

		#endregion

		#region methods

		/// <summary>
		/// Returns the value of the mathematical element.
		/// For the calculation the given <paramref name="resolver"/> is used.
		/// </summary>
		public abstract double? GetResult( [NotNull] ICharacteristicValueResolver resolver );

		/// <summary>
		/// Provides an enumeration of <see cref="MathDependencyInformation"/> the math element depends on.
		/// </summary>
		internal IEnumerable<MathDependencyInformation> CheckForDependentCharacteristics( [NotNull] ICharacteristicInfoResolver resolver )
		{
			return OnCheckForDependentCharacteristics( resolver );
		}

		/// <summary>
		/// Provides an enumeration of <see cref="MathDependencyInformation"/> the math element depends on.
		/// </summary>
		protected virtual IEnumerable<MathDependencyInformation> OnCheckForDependentCharacteristics( [NotNull] ICharacteristicInfoResolver resolver )
		{
			var enumeration = Enumerable.Empty<MathDependencyInformation>();
			foreach( var child in Arguments )
				enumeration = enumeration.Concat( child.OnCheckForDependentCharacteristics( resolver ) );

			return enumeration;
		}

		#endregion
	}
}