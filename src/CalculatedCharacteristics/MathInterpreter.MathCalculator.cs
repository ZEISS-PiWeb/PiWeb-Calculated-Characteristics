#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Shared.CalculatedCharacteristics.Arithmetic;

	#endregion

	public sealed partial class MathInterpreter
	{
		#region class MathCalculator

		/// <summary>
		/// Responsible calculating the formula value.
		/// </summary>
		private class MathCalculator : IMathCalculator
		{
			#region members

			private readonly string _FormulaString;
			[NotNull] private readonly ChildPathsHandler _ChildPathsHandler;
			[NotNull] private readonly CharacteristicCalculatorFactory _CharacteristicCalculatorFactory;
			[CanBeNull] private readonly PathInformationDto _SourcePath;

			#endregion

			#region constructors

			/// <summary>
			/// Creates a new instance of <see ref="MathCalculation"/>.
			/// </summary>
			public MathCalculator(
				MathElement calculationTreeRoot,
				string formula,
				[NotNull] ChildPathsHandler childPathsHandler,
				[NotNull] CharacteristicCalculatorFactory characteristicCalculatorFactory,
				[CanBeNull] PathInformationDto sourcePath )
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
			public MathElement MathTreeRoot { get; }

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
				return MathTreeRoot.GetResult( characteristicValueResolver );
			}

			/// <inheritdoc/>
			public IReadOnlyDictionary<PathInformationDto, MathDependencyInformation[]> GetDependentCharacteristics( EntityAttributeValueHandler entityAttributeValueHandler )
			{
				if( MathTreeRoot == null )
					return new Dictionary<PathInformationDto, MathDependencyInformation[]>();

				var characteristicInfoResolver = new CharacteristicInfoResolver(
					_ChildPathsHandler,
					entityAttributeValueHandler,
					_SourcePath );
				return MathTreeRoot.CheckForDependentCharacteristics( characteristicInfoResolver )
					.GroupBy( mdi => mdi.Path )
					.ToDictionary( g => g.Key, grouping => grouping.Select( mdi => mdi ).ToArray() );
			}

			#endregion
		}

		#endregion
	}
}