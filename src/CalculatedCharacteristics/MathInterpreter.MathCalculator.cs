#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	public sealed partial class MathInterpreter
	{
		#region class MathCalculator

		/// <summary>
		/// Responsible calculating the formula value.
		/// </summary>
		private sealed class MathCalculator : IMathCalculator
		{
			#region members

			private readonly string _FormulaString;
			private readonly ChildPathsHandler _ChildPathsHandler;
			private readonly CharacteristicCalculatorFactory _CharacteristicCalculatorFactory;
			private readonly PathInformation? _SourcePath;

			#endregion

			#region constructors

			/// <summary>
			/// Creates a new instance of <see ref="MathCalculation"/>.
			/// </summary>
			public MathCalculator(
				MathElement calculationTreeRoot,
				string formula,
				ChildPathsHandler childPathsHandler,
				CharacteristicCalculatorFactory characteristicCalculatorFactory,
				PathInformation? sourcePath )
			{
				_FormulaString = formula;
				_ChildPathsHandler = childPathsHandler;
				_CharacteristicCalculatorFactory = characteristicCalculatorFactory;
				_SourcePath = sourcePath;
				MathTreeRoot = calculationTreeRoot;
			}

			#endregion

			#region methods

			/// <inheritdoc/>
			public override string ToString()
			{
				return _FormulaString;
			}

			#endregion

			#region interface IMathCalculator

			/// <inheritdoc/>
			public MathElement? MathTreeRoot { get; }

			/// <inheritdoc/>
			public double? GetResult(
				MeasurementValueHandler measurementValueHandler,
				EntityAttributeValueHandler entityAttributeValueHandler,
				DateTime? measurementTime = null )
			{
				var characteristicValueResolver = new CharacteristicValueResolver(
					_CharacteristicCalculatorFactory,
					_ChildPathsHandler,
					measurementValueHandler,
					entityAttributeValueHandler,
					_SourcePath,
					measurementTime );
				return MathTreeRoot?.GetResult( characteristicValueResolver );
			}

			/// <inheritdoc/>
			public IReadOnlyDictionary<PathInformation, MathDependencyInformation[]> GetDependentCharacteristics( EntityAttributeValueHandler entityAttributeValueHandler )
			{
				if( MathTreeRoot == null )
					return new Dictionary<PathInformation, MathDependencyInformation[]>();

				var characteristicInfoResolver = new CharacteristicInfoResolver(
					_ChildPathsHandler,
					entityAttributeValueHandler,
					_SourcePath );
				return MathTreeRoot.CheckForDependentCharacteristics( characteristicInfoResolver )
					.Where( mdi => mdi.Path is not null )
					.GroupBy( mdi => mdi.Path )
					.ToDictionary( g => g.Key!, grouping => grouping.Select( mdi => mdi ).ToArray() );
			}

			#endregion
		}

		#endregion
	}
}