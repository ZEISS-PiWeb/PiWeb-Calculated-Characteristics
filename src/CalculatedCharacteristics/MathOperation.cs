﻿#region copyright

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
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Represents an internal used data container for operation logic.
	/// </summary>
	internal readonly struct MathOperation
	{
		#region members

		private readonly bool _IsSet;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see ref="Operation"/>.
		/// </summary>
		public MathOperation( CalculateValueDelegate calculateValueHandler, GetDependentCharacteristicsDelegate? dependentCharacteristicsHandler )
		{
			_IsSet = true;
			CalculateValue = calculateValueHandler ?? throw new ArgumentNullException( nameof( calculateValueHandler ) );
			GetDependentCharacteristics = dependentCharacteristicsHandler;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the handler for the calculation.
		/// </summary>
		public CalculateValueDelegate CalculateValue { get; }

		/// <summary>
		/// Gets the handler to determine dependent characteristics.
		/// </summary>
		public GetDependentCharacteristicsDelegate? GetDependentCharacteristics { get; }

		/// <summary>
		/// Indicates if the math operation is empty.
		/// </summary>
		public bool IsEmpty => !_IsSet;

		#endregion
	}

	internal delegate double? CalculateValueDelegate( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver );

	internal delegate IEnumerable<MathDependencyInformation> GetDependentCharacteristicsDelegate( IReadOnlyCollection<MathElement> args, ICharacteristicInfoResolver resolver );
}