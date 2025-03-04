#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2025                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests;

#region usings

using System.Linq;
using Moq;
using NUnit.Framework;
using Zeiss.PiWeb.CalculatedCharacteristics.Functions;
using static ArgumentBuilder;
using static SpecializedComparer;

#endregion

public class StatisticalFunctionsTest
{
	#region members

	private static readonly ICharacteristicValueResolver Resolver = Mock.Of<ICharacteristicValueResolver>( MockBehavior.Strict );

	#endregion

	#region methods

	[TestCase( new[] { 1.2 }, 1.2 )]
	[TestCase( new[] { 1.2, -3.45 }, -3.45 )]
	[TestCase( new[] { 1.2, -3.45, 7.8 }, -3.45 )]
	[Test]
	public void Test_Min( double[] argumentValues, double? expectedResult )
	{
		//Given
		var arguments = CreateArguments( argumentValues );

		//When
		var result = StatisticalFunctions.Min( arguments, Resolver );

		//Then
		Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
	}

	[TestCase( new double[ 0 ], null )]
	[TestCase( new[] { 1.2 }, 1.2 )]
	[TestCase( new[] { 1.2, -3.45 }, -3.45 )]
	[TestCase( new[] { 1.2, -3.45, 7.8 }, -3.45 )]
	[Test]
	public void Test_MinWithArgumentWithNullValue( double[] argumentValues, double? expectedResult )
	{
		//Given
		var arguments = CreateArguments( argumentValues ).Append( CreateArgument( null ) ).ToArray();

		//When
		var result = StatisticalFunctions.Min( arguments, Resolver );

		//Then
		Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
	}

	[TestCase( new double[ 0 ] )]
	[Test]
	public void Test_MinWithInvalidArguments( double[] argumentValues )
	{
		//Given
		var arguments = CreateArguments( argumentValues );

		//When/Then
		Assert.That( () => StatisticalFunctions.Min( arguments, Resolver ), Throws.ArgumentException );
	}

	[TestCase( new[] { 1.2 }, 1.2 )]
	[TestCase( new[] { 1.2, -3.45 }, 1.2 )]
	[TestCase( new[] { 1.2, -3.45, 7.8 }, 7.8 )]
	[Test]
	public void Test_Max( double[] argumentValues, double? expectedResult )
	{
		//Given
		var arguments = CreateArguments( argumentValues );

		//When
		var result = StatisticalFunctions.Max( arguments, Resolver );

		//Then
		Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
	}

	[TestCase( new double[ 0 ], null )]
	[TestCase( new[] { 1.2 }, 1.2 )]
	[TestCase( new[] { 1.2, -3.45 }, 1.2 )]
	[TestCase( new[] { 1.2, -3.45, 7.8 }, 7.8 )]
	[Test]
	public void Test_MaxWithArgumentWithNullValue( double[] argumentValues, double? expectedResult )
	{
		//Given
		var arguments = CreateArguments( argumentValues ).Append( CreateArgument( null ) ).ToArray();

		//When
		var result = StatisticalFunctions.Max( arguments, Resolver );

		//Then
		Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
	}

	[TestCase( new double[ 0 ] )]
	[Test]
	public void Test_MaxWithInvalidArguments( double[] argumentValues )
	{
		//Given
		var arguments = CreateArguments( argumentValues );

		//When/Then
		Assert.That( () => StatisticalFunctions.Max( arguments, Resolver ), Throws.ArgumentException );
	}

	[TestCase( 1.2, 1.2 )]
	[TestCase( 4.65 / 2, 1.2, 3.45 )]
	[TestCase( 12.45 / 3, 1.2, 3.45, 7.8 )]
	[TestCase( 12.45 / 3, null, 1.2, 3.45, 7.8 )]
	[TestCase( null, null )]
	[TestCase( null, null, null )]
	[Test]
	public void Test_Mean( double? expectedResult, params double?[] argumentValues )
	{
		//Given
		var arguments = CreateArguments( argumentValues );

		//When
		var result = StatisticalFunctions.Mean( arguments, Resolver );

		//Then
		Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
	}

	[TestCase( 1.2, 1.2 )]
	[TestCase( 4.65 / 2, 1.2, 3.45 )]
	[TestCase( 3.45, 1.2, 3.45, 7.8 )]
	[TestCase( 3.45, null, 1.2, 3.45, 7.8 )]
	[TestCase( 6.05 / 2, 1.2, 2.6, 8.3, 9.2, 1.3, null, 3.45 )]
	[TestCase( 3.45, 1.2, 2.6, 8.3, 9.2, 1.3, 7.8, 3.45 )]
	[TestCase( null, null )]
	[TestCase( null, null, null )]
	[Test]
	public void Test_Median( double? expectedResult, params double?[] argumentValues )
	{
		//Given
		var arguments = CreateArguments( argumentValues );

		//When
		var result = StatisticalFunctions.Median( arguments, Resolver );

		//Then
		Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
	}

	#endregion
}