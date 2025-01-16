#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;
	using Zeiss.PiWeb.CalculatedCharacteristics.Functions;

	#endregion

	[TestFixture]
	public class AuditFunctionsTest
	{
		#region methods

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesForAuditGrade ) )]
		[Test]
		public void Test_AuditGrade( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var valueResolver = testCase.GetCharacteristicValueResolver();

			//When
			var result = AuditFunctions.AuditGrade( arguments, valueResolver );

			//Then
			Assert.That( result, Is.EqualTo( testCase.ExpectedResult ) );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesWithInvalidArgumentsForAuditGrade ) )]
		[Test]
		public void Test_AuditGradeWithInvalidArguments( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var valueResolver = testCase.GetCharacteristicValueResolver();

			//When/Then
			Assert.That( () => AuditFunctions.AuditGrade( arguments, valueResolver ), Throws.TypeOf( testCase.ExpectedExceptionType! ) );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesForAuditGrade ) )]
		[Test]
		public void Test_AuditGrade_DependentCharacteristics( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var infoResolver = testCase.GetCharacteristicValueResolver();

			//When
			var result = AuditFunctions.AuditGrade_DependentCharacteristics( arguments, infoResolver );

			//Then
			Assert.That( result.Select( dependency => dependency.Path ).ToArray(), Is.EqualTo( testCase.ExpectedDependentChars ) );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesWithInvalidArgumentsForAuditGrade ) )]
		[Test]
		public void Test_AuditGrade_DependentCharacteristicsWithInvalidArguments( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var infoResolver = testCase.GetCharacteristicValueResolver();

			//When
			var result = AuditFunctions.AuditGrade_DependentCharacteristics( arguments, infoResolver );

			//Then
			Assert.That( result, Is.Empty );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesForAverageAuditGrade ) )]
		[Test]
		public void Test_AverageAuditGrade( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var valueResolver = testCase.GetCharacteristicValueResolver();

			//When
			var result = AuditFunctions.AverageAuditGrade( arguments, valueResolver );

			//Then
			Assert.That( result, Is.EqualTo( testCase.ExpectedResult ) );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesWithInvalidArgumentsForAverageOrGroupedAuditGrade ) )]
		[Test]
		public void Test_AverageAuditGradeWithInvalidArguments( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var valueResolver = testCase.GetCharacteristicValueResolver();

			//When/Then
			Assert.That( () => AuditFunctions.AverageAuditGrade( arguments, valueResolver ), Throws.TypeOf( testCase.ExpectedExceptionType! ) );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesForGroupedAuditGrade ) )]
		[Test]
		public void Test_GroupedAuditGrade( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var valueResolver = testCase.GetCharacteristicValueResolver();

			//When
			var result = AuditFunctions.GroupedAuditGrade( arguments, valueResolver );

			//Then
			Assert.That( result, Is.EqualTo( testCase.ExpectedResult ) );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesWithInvalidArgumentsForAverageOrGroupedAuditGrade ) )]
		[Test]
		public void Test_GroupedAuditGradeWithInvalidArguments( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var valueResolver = testCase.GetCharacteristicValueResolver();

			//When/Then
			Assert.That( () => AuditFunctions.GroupedAuditGrade( arguments, valueResolver ), Throws.TypeOf( testCase.ExpectedExceptionType! ) );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesForAverageAuditGrade ) )]
		[Test]
		public void Test_AverageOrGroupedQZ_DependentCharacteristics( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var infoResolver = testCase.GetCharacteristicValueResolver();

			//When
			var result = AuditFunctions.AverageOrGroupedAuditGrade_DependentCharacteristics( arguments, infoResolver );

			//Then
			Assert.That( result.Select( dependency => dependency.Path ).ToArray(), Is.EqualTo( testCase.ExpectedDependentChars ) );
		}

		[TestCaseSource( typeof( AuditFunctionsTestCase ), nameof( AuditFunctionsTestCase.CreateTestCasesWithInvalidArgumentsForAverageOrGroupedAuditGrade ) )]
		[Test]
		public void Test_AverageOrGroupedQZ_DependentCharacteristicsWithInvalidArguments( AuditFunctionsTestCase testCase )
		{
			//Given
			var arguments = testCase.Arguments;
			var infoResolver = testCase.GetCharacteristicInfoResolver();

			//When
			var result = AuditFunctions.AverageOrGroupedAuditGrade_DependentCharacteristics( arguments, infoResolver );

			//Then
			Assert.That( result, Is.Empty );
		}

		#endregion

		#region class AuditFunctionsTestCase

		public class AuditFunctionsTestCase
		{
			private const string Measured = "Measured";
			private const string OutOfTolerance = "OutOfTolerance";
			private const string Missing = "Missing";
			private const string MissingAsOutOfTolerance = "MissingAsOOT";

			#region members

			private static readonly PathInformation ParentPart = new PathInformation( PathElement.Part( "Audit" ) );
			private static readonly PathInformation AuditChar1 = PathInformation.Combine( ParentPart, PathElement.Char( "AuditChar1" ) );
			private static readonly PathInformation AuditChar2 = PathInformation.Combine( ParentPart, PathElement.Char( "AuditChar2" ) );
			private static readonly PathInformation AuditChar3 = PathInformation.Combine( ParentPart, PathElement.Char( "AuditChar3" ) );

			private static readonly Dictionary<PathInformation, MeasuredValue> MeasuredValues;

			private readonly CharacteristicCalculatorFactory _CharacteristicCalculatorFactory = _ => null;
			private readonly MeasurementValueHandler _MeasurementValueHandler = path => MeasuredValues.TryGetValue( path, out var data ) ? data.Value : null;
			private readonly EntityAttributeValueHandler _EntityAttributeValueHandler = ( _, _, _ ) => null;
			private readonly ChildPathsHandler _ChildPathsHandler = parent => MeasuredValues.Keys.Where( p => p.ParentPath == parent );

			#endregion

			#region constructors

			static AuditFunctionsTestCase()
			{
				var values = new List<MeasuredValue>();
				values.AddRange( CreateAuditValues( AuditChar1, 12, 3, 0 ) );
				values.AddRange( CreateAuditValues( AuditChar2, 10, 1, 2 ) );
				values.AddRange( CreateAuditValues( AuditChar3, 10, 5, 2 ) );
				MeasuredValues = values.ToDictionary( c => c.Path );
			}

			private AuditFunctionsTestCase( string functionName, PathInformation sourcePath, MathElement[] arguments, double? expectedResult, PathInformation[] expectedDependentChars )
			{
				FunctionName = functionName;
				SourcePath = sourcePath;
				Arguments = arguments;
				ExpectedResult = expectedResult;
				ExpectedDependentChars = expectedDependentChars;
				ExpectedExceptionType = null;
			}

			private AuditFunctionsTestCase( string functionName, PathInformation sourcePath, MathElement[] arguments, Type expectedExceptionType )
			{
				FunctionName = functionName;
				SourcePath = sourcePath;
				Arguments = arguments;
				ExpectedResult = null;
				ExpectedDependentChars = [];
				ExpectedExceptionType = expectedExceptionType;
			}

			#endregion

			#region properties

			private string FunctionName { get; }
			private PathInformation SourcePath { get; }
			public MathElement[] Arguments { get; }
			public double? ExpectedResult { get; }
			public PathInformation[] ExpectedDependentChars { get; }
			public Type? ExpectedExceptionType { get; }

			#endregion

			#region methods

			public ICharacteristicValueResolver GetCharacteristicValueResolver()
			{
				return new CharacteristicValueResolver(
					_CharacteristicCalculatorFactory,
					_ChildPathsHandler,
					_MeasurementValueHandler,
					_EntityAttributeValueHandler,
					SourcePath );
			}

			public ICharacteristicInfoResolver GetCharacteristicInfoResolver()
			{
				return new CharacteristicInfoResolver( _ChildPathsHandler, _EntityAttributeValueHandler, SourcePath );
			}

			public override string ToString()
			{
				var arguments = Arguments.Length > 0 ? string.Join( ", ", Arguments.Select( ToString ) ) : "null";

				return $"{SourcePath} : {FunctionName}({arguments})";
			}

			private static string? ToString( MathElement mathElement )
			{
				if( mathElement is Literal literal )
					return literal.Text;

				if( mathElement is Characteristic characteristic )
				{
					return characteristic.Path.Name;
				}

				return mathElement.ToString();
			}

			public static IEnumerable<AuditFunctionsTestCase> CreateTestCasesForAuditGrade()
			{
				yield return Create( "QZ", ParentPart, [], null, CreateDependentChars( ParentPart ) );

				yield return Create( "QZ", ParentPart, [AuditChar1.Name], 3.5, CreateDependentChars( ParentPart, charName: AuditChar1.Name ) );
				yield return Create( "QZ", ParentPart, [AuditChar2.Name], 2.0, CreateDependentChars( ParentPart, charName: AuditChar2.Name ) );

				yield return Create( "QZ", ParentPart, [MissingAsOutOfTolerance], null,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance ) );
				yield return Create( "QZ", ParentPart, [MissingAsOutOfTolerance, AuditChar1.Name], 3.5,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance, AuditChar1.Name ) );
				yield return Create( "QZ", ParentPart, [MissingAsOutOfTolerance, AuditChar2.Name], 3.5,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance, AuditChar2.Name ) );

				yield return Create( "QZ", AuditChar1, [], 3.5, CreateDependentChars( AuditChar1 ) );
				yield return Create( "QZ", AuditChar1, [AuditChar1.Name], null, CreateDependentChars( AuditChar1, charName: AuditChar1.Name ) );

				yield return Create( "QZ", AuditChar1, [MissingAsOutOfTolerance], 3.5,
					CreateDependentChars( AuditChar1, MissingStrategy.CountAsOutOfTolerance ) );
				yield return Create( "QZ", AuditChar1, [MissingAsOutOfTolerance, AuditChar1.Name], null,
					CreateDependentChars( AuditChar1, MissingStrategy.CountAsOutOfTolerance, AuditChar1.Name ) );
			}

			public static IEnumerable<AuditFunctionsTestCase> CreateTestCasesWithInvalidArgumentsForAuditGrade()
			{
				yield return Create( "QZ", ParentPart, ["foobar"], typeof( ArgumentException ) );
				yield return Create( "QZ", ParentPart, [MissingAsOutOfTolerance, "foobar"], typeof( ArgumentException ) );
				yield return Create( "QZ", ParentPart, [MissingAsOutOfTolerance, MissingAsOutOfTolerance], typeof( ArgumentException ) );
				yield return Create( "QZ", ParentPart, [AuditChar1.Name, MissingAsOutOfTolerance], typeof( ArgumentException ) );
				yield return Create( "QZ", ParentPart, [AuditChar1.Name, AuditChar2.Name], typeof( ArgumentException ) );
				yield return Create( "QZ", ParentPart, [MissingAsOutOfTolerance, AuditChar1.Name, AuditChar2.Name], typeof( ArgumentException ) );
			}

			public static IEnumerable<AuditFunctionsTestCase> CreateTestCasesForAverageAuditGrade()
			{
				yield return Create( "AverageQZ", ParentPart, [], 3.8333333333333335d,
					CreateDependentChars( ParentPart, charNames: [AuditChar1.Name, AuditChar2.Name, AuditChar3.Name] ) );

				yield return Create( "AverageQZ", ParentPart, [AuditChar1.Name], 3.5,
					CreateDependentChars( ParentPart, charName: AuditChar1.Name ) );
				yield return Create( "AverageQZ", ParentPart, [AuditChar2.Name], 2.0,
					CreateDependentChars( ParentPart, charName: AuditChar2.Name ) );
				yield return Create( "AverageQZ", ParentPart, [AuditChar1.Name, AuditChar2.Name], 2.75,
					CreateDependentChars( ParentPart, charNames: [AuditChar1.Name, AuditChar2.Name] ) );
				yield return Create( "AverageQZ", ParentPart, [AuditChar1.Name, AuditChar2.Name, AuditChar3.Name], 3.8333333333333335d,
					CreateDependentChars( ParentPart, charNames: [AuditChar1.Name, AuditChar2.Name, AuditChar3.Name] ) );

				yield return Create( "AverageQZ", ParentPart, [MissingAsOutOfTolerance], 4.6111111111111116d,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance, AuditChar1.Name, AuditChar2.Name, AuditChar3.Name ) );
				yield return Create( "AverageQZ", ParentPart, [MissingAsOutOfTolerance, AuditChar1.Name], 3.5,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance, AuditChar1.Name ) );
				yield return Create( "AverageQZ", ParentPart, [MissingAsOutOfTolerance, AuditChar2.Name], 3.5,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance, AuditChar2.Name ) );

				yield return Create( "AverageQZ", AuditChar1, [], null,
					CreateDependentChars( AuditChar1, charNames: [Measured, OutOfTolerance, Missing] ) );
				yield return Create( "AverageQZ", AuditChar1, [AuditChar1.Name], null,
					CreateDependentChars( AuditChar1, charName: AuditChar1.Name ) );
				yield return Create( "AverageQZ", AuditChar1, [MissingAsOutOfTolerance], null,
					CreateDependentChars( AuditChar1, MissingStrategy.CountAsOutOfTolerance, Measured, OutOfTolerance, Missing ) );
				yield return Create( "AverageQZ", AuditChar1, [MissingAsOutOfTolerance, AuditChar1.Name], null,
					CreateDependentChars( AuditChar1, MissingStrategy.CountAsOutOfTolerance, AuditChar1.Name ) );
			}

			public static IEnumerable<AuditFunctionsTestCase> CreateTestCasesForGroupedAuditGrade()
			{
				yield return Create( "GroupedQZ", ParentPart, [], 3.8125,
					CreateDependentChars( ParentPart, charNames: [AuditChar1.Name, AuditChar2.Name, AuditChar3.Name] ) );

				yield return Create( "GroupedQZ", ParentPart, [AuditChar1.Name], 3.5,
					CreateDependentChars( ParentPart, charName: AuditChar1.Name ) );
				yield return Create( "GroupedQZ", ParentPart, [AuditChar2.Name], 2.0,
					CreateDependentChars( ParentPart, charName: AuditChar2.Name ) );
				yield return Create( "GroupedQZ", ParentPart, [AuditChar1.Name, AuditChar2.Name], 2.8181818181818183d,
					CreateDependentChars( ParentPart, charNames: [AuditChar1.Name, AuditChar2.Name] ) );
				yield return Create( "GroupedQZ", ParentPart, [AuditChar1.Name, AuditChar2.Name, AuditChar3.Name], 3.8125,
					CreateDependentChars( ParentPart, charNames: [AuditChar1.Name, AuditChar2.Name, AuditChar3.Name] ) );

				yield return Create( "GroupedQZ", ParentPart, [MissingAsOutOfTolerance], 4.6111111111111107d,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance, AuditChar1.Name, AuditChar2.Name, AuditChar3.Name ) );
				yield return Create( "GroupedQZ", ParentPart, [MissingAsOutOfTolerance, AuditChar1.Name], 3.5,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance, AuditChar1.Name ) );
				yield return Create( "GroupedQZ", ParentPart, [MissingAsOutOfTolerance, AuditChar2.Name], 3.5,
					CreateDependentChars( ParentPart, MissingStrategy.CountAsOutOfTolerance, AuditChar2.Name ) );

				yield return Create( "GroupedQZ", AuditChar1, [], null,
					CreateDependentChars( AuditChar1, charNames: [Measured, OutOfTolerance, Missing] ) );
				yield return Create( "GroupedQZ", AuditChar1, [AuditChar1.Name], null,
					CreateDependentChars( AuditChar1, charName: AuditChar1.Name ) );
				yield return Create( "GroupedQZ", AuditChar1, [MissingAsOutOfTolerance], null,
					CreateDependentChars( AuditChar1, MissingStrategy.CountAsOutOfTolerance, Measured, OutOfTolerance, Missing ) );
				yield return Create( "GroupedQZ", AuditChar1, [MissingAsOutOfTolerance, AuditChar1.Name], null,
					CreateDependentChars( AuditChar1, MissingStrategy.CountAsOutOfTolerance, AuditChar1.Name ) );
			}

			public static IEnumerable<AuditFunctionsTestCase> CreateTestCasesWithInvalidArgumentsForAverageOrGroupedAuditGrade()
			{
				yield return Create( "AverageOrGroupedQZ", ParentPart, ["foobar"], typeof( ArgumentException ) );
				yield return Create( "AverageOrGroupedQZ", ParentPart, [MissingAsOutOfTolerance, "foobar"], typeof( ArgumentException ) );
				yield return Create( "AverageOrGroupedQZ", ParentPart, [MissingAsOutOfTolerance, MissingAsOutOfTolerance], typeof( ArgumentException ) );
				yield return Create( "AverageOrGroupedQZ", ParentPart, [AuditChar1.Name, MissingAsOutOfTolerance], typeof( ArgumentException ) );
			}

			private static AuditFunctionsTestCase Create( string functionName, PathInformation parentPath, string[] argumentValues,
				double? expectedResult, PathInformation[] expectedDependentChars )
			{
				var arguments = CreateArguments( argumentValues, parentPath );
				var calculatedCharPath = PathInformation.Combine( parentPath, PathElement.Char( "QZ" ) );

				return new AuditFunctionsTestCase( functionName, calculatedCharPath, arguments, expectedResult, expectedDependentChars );
			}

			private static AuditFunctionsTestCase Create( string functionName, PathInformation parentPath, string[] argumentValues, Type expectedExceptionType )
			{
				var arguments = CreateArguments( argumentValues, parentPath );
				var calculatedCharPath = PathInformation.Combine( parentPath, PathElement.Char( "QZ" ) );

				return new AuditFunctionsTestCase( functionName, calculatedCharPath, arguments, expectedExceptionType );
			}

			private static MathElement[] CreateArguments( string[] argumentValues, PathInformation parentPath )
			{
				if( argumentValues.Length == 0 )
					return [];

				var arguments = new List<MathElement>();
				foreach( var argumentValue in argumentValues )
				{
					if( argumentValue == AuditChar1.Name || argumentValue == AuditChar2.Name || argumentValue == AuditChar3.Name )
					{
						var charPath = PathInformation.Combine( parentPath, PathElement.Char( argumentValue ) );
						arguments.Add( new Characteristic( 0, 0, argumentValue, charPath, null ) );
					}
					else
					{
						arguments.Add( new Literal( 0, 0, argumentValue ) );
					}
				}

				return arguments.ToArray();
			}

			private static PathInformation[] CreateDependentChars(
				PathInformation parent, MissingStrategy missingStrategy = MissingStrategy.Ignore, params string[] charNames )
			{
				var paths = new List<PathInformation>();
				foreach( var name in charNames )
					paths.AddRange( CreateDependentChars( parent, missingStrategy, name ) );

				return paths.ToArray();
			}

			private static PathInformation[] CreateDependentChars(
				PathInformation parent, MissingStrategy missingStrategy = MissingStrategy.Ignore, string? charName = null )
			{
				var parentPath = string.IsNullOrEmpty( charName ) ? parent : PathInformation.Combine( parent, PathElement.Char( charName ) );

				var paths = new List<PathInformation>
				{
					PathInformation.Combine( parentPath, PathElement.Char( Measured ) ),
					PathInformation.Combine( parentPath, PathElement.Char( OutOfTolerance ) )
				};
				if( missingStrategy == MissingStrategy.CountAsOutOfTolerance )
					paths.Add( PathInformation.Combine( parentPath, PathElement.Char( Missing ) ) );

				return paths.ToArray();
			}

			private static IEnumerable<MeasuredValue> CreateAuditValues( PathInformation auditCharacteristic, double? measured, double? outOfTolerance, double? missing )
			{
				yield return new MeasuredValue { Path = auditCharacteristic, Value = null };

				yield return new MeasuredValue
				{
					Path = PathInformation.Combine( auditCharacteristic, PathElement.Char( Measured ) ),
					Value = measured
				};

				yield return new MeasuredValue
				{
					Path = PathInformation.Combine( auditCharacteristic, PathElement.Char( OutOfTolerance ) ),
					Value = outOfTolerance
				};

				yield return new MeasuredValue
				{
					Path = PathInformation.Combine( auditCharacteristic, PathElement.Char( Missing ) ),
					Value = missing
				};
			}

			#endregion

			private enum MissingStrategy
			{
				Ignore,
				CountAsOutOfTolerance
			}

			#region struct MeasuredValue

			private struct MeasuredValue
			{
				#region members

				public PathInformation Path;
				public double? Value;

				#endregion
			}

			#endregion
		}

		#endregion
	}
}