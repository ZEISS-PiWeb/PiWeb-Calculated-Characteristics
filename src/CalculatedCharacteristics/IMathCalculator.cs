#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Implement this interface to provide a math calculation.
	/// </summary>
	public interface IMathCalculator
	{
		#region properties

		/// <summary>
		/// Gets the root of the calculation tree.
		/// </summary>
		MathElement MathTreeRoot { get; }

		#endregion

		#region methods

		/// <summary>
		/// Calculates the value of a formula.
		/// </summary>
		/// <param name="measurementValueHandler">The delegate that defines how measurement values are resolved.</param>
		/// <param name="entityAttributeValueHandler">The delegate that defines how entity attribute values are resolved.</param>
		/// <param name="measurementTime">The measurement time.</param>
		/// <returns>The calculated value or <code>null</code> if the value could not be calculated.</returns>
		double? GetResult(
			[NotNull] MeasurementValueHandler measurementValueHandler,
			[NotNull] EntityAttributeValueHandler entityAttributeValueHandler,
			DateTime? measurementTime = null );

		/// <summary>
		/// Provides a readonly dictionary of dependent characteristic paths. The value provides occurence information within the formula.
		/// </summary>
		/// <param name="entityAttributeValueHandler">The delegate that defines how entity attribute values are resolved.</param>
		/// <returns>Readonly dictionary of dependent characteristic paths.</returns>
		IReadOnlyDictionary<PathInformation, MathDependencyInformation[]> GetDependentCharacteristics( [NotNull] EntityAttributeValueHandler entityAttributeValueHandler );

		#endregion
	}
}