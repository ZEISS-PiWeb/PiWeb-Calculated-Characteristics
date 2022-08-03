#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.OprFunctions
{
	#region usings

	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	/// <summary>
	/// A test case for an calculated characteristic given by a formula
	/// </summary>
	public record OprFunctionTestCase
	{
		#region members

		/// <summary>
		/// The formula to test
		/// </summary>
		public string GivenFormula { get; init; }

		/// <summary>
		/// The expected result
		/// </summary>
		public object ExpectedResult { get; init; }

		/// <summary>
		/// All ordered expected characteristics
		/// </summary>
		public ExpectedMeasurementPoint[] ExpectedDependentCharacteristics { get; init; }

		/// <summary>
		/// Optional tolerance for result value comparison.
		/// </summary>
		public double? Tolerance { get; init; }

		#endregion

		/// <summary>
		/// Creates a new instance of <see cref="OprFunctionTestCase"/>.
		/// </summary>
		public OprFunctionTestCase()
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="OprFunctionTestCase"/>.
		/// </summary>
		public OprFunctionTestCase( string formula, object expectedResult, params ExpectedMeasurementPoint[] expectedDependentCharacteristics )
		{
			GivenFormula = formula;
			ExpectedResult = expectedResult;
			ExpectedDependentCharacteristics = expectedDependentCharacteristics;
		}

		#region methods

		/// <summary>
		/// Asserts the given formula with a list of characteristics and measurement values
		/// </summary>
		/// <param name="actualCharacteristics">Characteristics</param>
		/// <param name="actualMeasurementValues">Measurement values</param>
		public void AssertTestCase( InspectionPlanCollection actualCharacteristics, Dictionary<PathInformationDto, double> actualMeasurementValues )
		{
			var operationTestEnvironment = new OperationTestEnvironment( actualCharacteristics );

			AssertDependentCharacteristics( operationTestEnvironment );

			var result = operationTestEnvironment.GetResult( GivenFormula, actualMeasurementValues );
			if( Tolerance.HasValue )
				Assert.That( result, Is.EqualTo( ExpectedResult ).Within( Tolerance.Value ) );
			else
				Assert.That( result, Is.EqualTo( ExpectedResult ) );
		}

		/// <summary>
		/// Asserts the dependent characteristics
		/// </summary>
		private void AssertDependentCharacteristics( OperationTestEnvironment operationTestEnvironment )
		{
			var dependentCharacteristics = operationTestEnvironment.GetDependentCharacteristics( GivenFormula ).ToArray();

			var expectedCharacteristics = ExpectedDependentCharacteristics.SelectMany( point => point.Directions.Select( dir => OprFunctionsTestHelper.GetDirectionPath( point.Name, dir, point.HasExtendedName ) ) ).ToArray();

			// Check number of items
			Assert.That( dependentCharacteristics, Has.Length.EqualTo( expectedCharacteristics.Length ) );

			// Check the expected characteristic with the expected direction
			Assert.That( dependentCharacteristics, Is.EquivalentTo( expectedCharacteristics ) );
		}

		public override string ToString()
		{
			return GivenFormula ?? base.ToString();
		}

		#endregion

		#region struct ExpectedMeasurementPoint

		public readonly struct ExpectedMeasurementPoint
		{
			#region constructors

			/// <summary>
			/// Creates a new instance of <see cref="ExpectedMeasurementPoint"/>.
			/// </summary>
			public ExpectedMeasurementPoint( string name, bool hasExtendedName, params string[] directions )
			{
				Name = name;
				HasExtendedName = hasExtendedName;
				Directions = directions;
			}

			#endregion

			#region properties

			public string Name { get; }
			public bool HasExtendedName { get; }
			public string[] Directions { get; }

			#endregion
		}

		#endregion
	}
}