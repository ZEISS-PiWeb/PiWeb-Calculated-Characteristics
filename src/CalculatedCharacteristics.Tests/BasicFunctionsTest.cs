#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests
{
	#region usings

	using System;
	using System.Linq;
	using Moq;
	using NUnit.Framework;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;
	using Zeiss.PiWeb.CalculatedCharacteristics.Functions;

	#endregion

	[TestFixture]
	public class BasicFunctionsTest
	{
		#region members

		private static readonly ICharacteristicValueResolver Resolver = Mock.Of<ICharacteristicValueResolver>( MockBehavior.Strict );

		private static readonly Function NullReturningFunction =
			new Function(
				0,
				0,
				null,
				"ReturnNull",
				new MathOperation(
					( _, _ ) => null,
					( _, _ ) => Enumerable.Empty<MathDependencyInformation>() ) );

		#endregion

		#region methods

		[TestCase( 1.2, 3.45, 4.65 )]
		[TestCase( 1.2, null, null )]
		[TestCase( null, 3.45, null )]
		[TestCase( null, null, null )]
		[Test]
		public void Test_Add( double? argument1, double? argument2, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1, argument2 );

			//When
			var result = BasicFunctions.Add( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1 } )]
		[TestCase( new[] { 1.1, 2.2, 3.3 } )]
		[Test]
		public void Test_AddWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Add( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( new[] { 1.2 }, 1.2 )]
		[TestCase( new[] { 1.2, 3.45 }, 4.65 )]
		[TestCase( new[] { 1.2, 3.45, 7.8 }, 12.45 )]
		[Test]
		public void Test_Sum( double[] argumentValues, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When
			var result = BasicFunctions.Sum( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ], null )]
		[TestCase( new[] { 1.2 }, null )]
		[TestCase( new[] { 1.2, 3.45 }, null )]
		[TestCase( new[] { 1.2, 3.45, 7.8 }, null )]
		[Test]
		public void Test_SumWithArgumentWithNullValue( double[] argumentValues, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argumentValues ).Append( CreateArgument( null ) ).ToArray();

			//When
			var result = BasicFunctions.Sum( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[Test]
		public void Test_SumWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Sum( arguments, Resolver ), Throws.ArgumentException );
		}


		[TestCase( 4.65, 3.45, 1.2 )]
		[TestCase( 4.65, null, null )]
		[TestCase( null, 3.45, null )]
		[TestCase( null, null, null )]
		[Test]
		public void Test_Sub( double? argument1, double? argument2, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1, argument2 );

			//When
			var result = BasicFunctions.Sub( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1 } )]
		[TestCase( new[] { 1.1, 2.2, 3.3 } )]
		[Test]
		public void Test_SubWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Sub( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 1.2, 3.45, 4.14 )]
		[TestCase( 1.2, null, null )]
		[TestCase( null, 3.45, null )]
		[TestCase( null, null, null )]
		[Test]
		public void Test_Mul( double? argument1, double? argument2, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1, argument2 );

			//When
			var result = BasicFunctions.Mul( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1 } )]
		[TestCase( new[] { 1.1, 2.2, 3.3 } )]
		[Test]
		public void Test_MulWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Mul( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 4.14, 3.45, 1.2 )]
		[TestCase( 4.14, null, null )]
		[TestCase( null, 3.45, null )]
		[TestCase( null, null, null )]
		[Test]
		public void Test_Div( double? argument1, double? argument2, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1, argument2 );

			//When
			var result = BasicFunctions.Div( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1 } )]
		[TestCase( new[] { 1.1, 2.2, 3.3 } )]
		[Test]
		public void Test_DivWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Div( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( Math.PI, 0 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Sin( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Sin( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_SinWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Sin( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( Math.PI, -1 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Cos( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Cos( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_CosWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Cos( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( Math.PI, 0 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Tan( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Tan( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_TanWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Tan( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( Math.PI / 2, 0 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Cot( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Cot( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_CotWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Cot( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 180 / Math.PI, 1 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Rad( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Rad( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_RadWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Rad( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( Math.PI / 180, 1 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Deg( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Deg( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_DegWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Deg( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 1, Math.PI / 2 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Asin( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Asin( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_AsinWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Asin( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 1, 0 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Acos( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Acos( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_AcosWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Acos( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 1, Math.PI / 4 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Atan( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Atan( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_AtanWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Atan( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 2, 4 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Sqr( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Sqr( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_SqrWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Sqr( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 4, 2 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Sqrt( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Sqrt( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_SqrtWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Sqrt( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 2, Math.E * Math.E )]
		[TestCase( null, null )]
		[Test]
		public void Test_Exp( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Exp( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_ExpWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Exp( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( Math.E * Math.E, 2 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Ln( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Ln( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_LnWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Ln( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 2, 2 )]
		[TestCase( -3, 3 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Abs( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Abs( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_AbsWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Abs( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 2, 1 )]
		[TestCase( -3, -1 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Sgn( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Sgn( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_SgnWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Sgn( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 1.2, 1.2 )]
		[TestCase( 0, 0 )]
		[TestCase( -3.45, 0 )]
		[TestCase( null, null )]
		[Test]
		public void Test_Gz( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Gz( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_GzWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Gz( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 1.75, 0.77109389834622566d )]
		[TestCase( -3.4, 0.29351921253535862d )]
		[TestCase( null, null )]
		[Test]
		public void Test_Rnd( double? argument1, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1 );

			//When
			var result = BasicFunctions.Rnd( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1, 2.2 } )]
		[Test]
		public void Test_RndWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Rnd( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 1.2, 3, 1.2 )]
		[TestCase( 1.2, null, 1 )]
		[TestCase( 1.5, null, 2 )]
		[TestCase( 2.5, null, 2 )]
		[TestCase( 1.23456, 3, 1.235000 )]
		[Test]
		public void Test_Round( double? argument1, double? argument2, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1, argument2 );

			//When
			var result = BasicFunctions.Round( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[] { 0 } )]
		[TestCase( new double[] { 0, 1 } )]
		[TestCase( new double[] { 0, 0 } )]
		[Test]
		public void Test_RoundWithValidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Round( arguments, Resolver ), Throws.Nothing );
		}

		[TestCase( new double[] {} )]
		[TestCase( new double[] { 0, 1, 2 } )]
		[TestCase( new double[] { 0, -1 } )]
		[Test]
		public void Test_RoundWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Round( arguments, Resolver ), Throws.Exception );
		}

		[TestCase( new[] { 1.2 }, 1.2 )]
		[TestCase( new[] { 1.2, -3.45 }, -3.45 )]
		[TestCase( new[] { 1.2, -3.45, 7.8 }, -3.45 )]
		[Test]
		public void Test_Min( double[] argumentValues, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When
			var result = BasicFunctions.Min( arguments, Resolver );

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
			var result = BasicFunctions.Min( arguments, Resolver );

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
			Assert.That( () => BasicFunctions.Min( arguments, Resolver ), Throws.ArgumentException );
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
			var result = BasicFunctions.Max( arguments, Resolver );

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
			var result = BasicFunctions.Max( arguments, Resolver );

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
			Assert.That( () => BasicFunctions.Max( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 1.2, 3.45, 1.2 )]
		[TestCase( 1.2, null, 1.2 )]
		[TestCase( null, 3.45, 3.45 )]
		[TestCase( null, null, null )]
		[Test]
		public void Test_IfNotValue( double? argument1, double? argument2, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1, argument2 );

			//When
			var result = BasicFunctions.IfNotValue( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1 } )]
		[TestCase( new[] { 1.1, 2.2, 3.3 } )]
		[Test]
		public void Test_IfNotValueWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.IfNotValue( arguments, Resolver ), Throws.ArgumentException );
		}

		[TestCase( 2, 3, 8 )]
		[TestCase( 2, null, null )]
		[TestCase( null, 3, null )]
		[TestCase( null, null, null )]
		[Test]
		public void Test_Pow( double? argument1, double? argument2, double? expectedResult )
		{
			//Given
			var arguments = CreateArguments( argument1, argument2 );

			//When
			var result = BasicFunctions.Pow( arguments, Resolver );

			//Then
			Assert.That( result, Is.EqualTo( expectedResult ).Using<double?, double?>( CompareDoubles ) );
		}

		[TestCase( new double[ 0 ] )]
		[TestCase( new[] { 1.1 } )]
		[TestCase( new[] { 1.1, 2.2, 3.3 } )]
		[Test]
		public void Test_PowWithInvalidArguments( double[] argumentValues )
		{
			//Given
			var arguments = CreateArguments( argumentValues );

			//When/Then
			Assert.That( () => BasicFunctions.Pow( arguments, Resolver ), Throws.ArgumentException );
		}

		// ----------------------------------------------------

		private static bool CompareDoubles( double? x, double? y )
		{
			const double defaultPrecision = 1e-15;

			if( x.HasValue != y.HasValue )
				return false;

			if( !x.HasValue )
				return true;

			return Math.Abs( x.Value - y.Value ) <= defaultPrecision;
		}

		private static MathElement[] CreateArguments( params double[] argumentValues )
		{
			if( argumentValues == null )
				return null;

			return CreateArguments( argumentValues.Select( v => v as double? ).ToArray() );
		}

		private static MathElement[] CreateArguments( params double?[] argumentValues )
		{
			if( argumentValues == null )
				return null;

			if( argumentValues.Length == 0 )
				return Array.Empty<MathElement>();

			return argumentValues.Select( CreateArgument ).ToArray();
		}

		private static MathElement CreateArgument( double? value )
		{
			if( value.HasValue )
				return new Number( 0, 0, value.Value );

			return NullReturningFunction;
		}

		#endregion
	}
}