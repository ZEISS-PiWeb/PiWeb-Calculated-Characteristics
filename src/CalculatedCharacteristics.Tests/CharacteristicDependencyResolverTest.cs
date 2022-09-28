#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * *
 * Carl Zeiss IMT                                  *
 * Softwaresystem PiWeb                            *
 * (c) Carl Zeiss 2020                             *
 * * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Contracts;
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;

	#endregion

	[TestFixture]
	public class CharacteristicDependencyResolverTest
	{
		#region members

		private static readonly ConfigurationDto Configuration = CreateBasicConfiguration();
		private static readonly CatalogCollectionDto CatalogCollection = new CatalogCollectionDto();

		#endregion

		#region methods

		[Test]
		[TestCaseSource( nameof( CreateCharacteristicsInFormulaTestCases ) )]
		public void Test_ValidateFormula( CharacteristicsInFormulaTestCase testCase )
		{
			// Given
			object GetAttributeValue( PathInformation path, ushort key )
			{
				return GetCharacteristicAttribute( testCase.InspectionPlan, path, key );
			}

			var characteristicAttributeHandler = new EntityAttributeValueHandler(
				( path, key, _ ) => GetAttributeValue( path, key ) );
			var childPathsHandler = new ChildPathsHandler(
				path => testCase.InspectionPlan.GetDirectChildren( path ).Select( ch => ch.Path ) );

			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				GetAttributeValue,
				childPathsHandler );

			// When/Then
			if( testCase.FormulaValidationException == null )
				Assert.That( () => CharacteristicDependencyResolver.ValidateDependencies( testCase.CharacteristicsPath, attributeBasedMathInterpreterFactory.GetCharacteristicCalculator, characteristicAttributeHandler ), Throws.Nothing );
			else
				Assert.That( () => CharacteristicDependencyResolver.ValidateDependencies( testCase.CharacteristicsPath, attributeBasedMathInterpreterFactory.GetCharacteristicCalculator, characteristicAttributeHandler ), Throws.TypeOf( testCase.FormulaValidationException ) );
		}

		[Test]
		[TestCaseSource( nameof( CreateCharacteristicsInFormulaTestCases ) )]
		public void Test_GetDependentCharacteristics( CharacteristicsInFormulaTestCase testCase )
		{
			// Given
			var characteristicAttributeHandler = new EntityAttributeValueHandler(
				( path, key, _ ) => GetCharacteristicAttribute( testCase.InspectionPlan, path, key ) );
			var childPathsHandler = new ChildPathsHandler(
				path => testCase.InspectionPlan.GetDirectChildren( path ).Select( ch => ch.Path ) );

			var infoResolver = new CharacteristicDependencyResolver(
				childPathsHandler,
				characteristicAttributeHandler );

			// When
			var dependentChars = infoResolver.GetDependentCharacteristics( testCase.CharacteristicsPath ).ToArray();

			// Then
			Assert.That( dependentChars, Is.EquivalentTo( testCase.ExpectedCharacteristicsPaths ) );
		}

		private static IEnumerable<CharacteristicsInFormulaTestCase> CreateCharacteristicsInFormulaTestCases()
		{
			var part = CreatePart( "NestedCalculation" );
			var ch1 = CreateCharacteristic( "Ch1", part.Path );
			var ch2 = CreateCharacteristic( "Ch2", part.Path );
			var ch3 = CreateCharacteristic( "Ch3", part.Path );

			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, ch1 },
				null,
				typeof( ArgumentNullException ),
				Array.Empty<PathInformation>(),
				"Missing path" );

			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, ch1 },
				ch1.Path,
				null,
				Array.Empty<PathInformation>(),
				"Characteristic without formula" );

			var calc1 = CreateCharacteristic( "Calc1", part.Path, "1 + 2" );
			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, ch1, calc1 },
				calc1.Path,
				null,
				Array.Empty<PathInformation>(),
				"Formula without dependent characteristics" );

			calc1 = CreateCharacteristic( "Calc1", part.Path, "{Ch1} + {Ch2}" );
			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, ch1, ch2, calc1 },
				calc1.Path,
				null,
				new[] { ch1.Path, ch2.Path },
				"Formula with dependent characteristics" );

			var calc2 = CreateCharacteristic( "Calc2", part.Path, "{Calc1} + {Ch3}" );
			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, ch1, ch2, ch3, calc1, calc2 },
				calc2.Path,
				null,
				new[] { ch1.Path, ch2.Path, ch3.Path, calc1.Path },
				"Formula with nested dependent characteristics" );

			var calc3 = CreateCharacteristic( "Calc3", part.Path, "{Calc1} + {Calc3}" );
			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, ch1, ch2, calc1, calc3 },
				calc3.Path,
				typeof( CircularReferenceException ),
				Array.Empty<PathInformation>(),
				"Formula with direct circular reference" );

			var calc4 = CreateCharacteristic( "Calc4", part.Path, "{Calc5} + {Calc6}" );
			var calc5 = CreateCharacteristic( "Calc5", part.Path, "{Calc6} + 1" );
			var calc6 = CreateCharacteristic( "Calc6", part.Path, "{Calc5} - 1" );
			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, calc4, calc5, calc6 },
				calc4.Path,
				typeof( CircularReferenceException ),
				Array.Empty<PathInformation>(),
				"Formula with indirect circular reference" );

			var calc7 = CreateCharacteristic( "Calc7", part.Path, "{Ch1(2101)}" );
			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, calc7 },
				calc7.Path,
				null,
				new[] { ch1.Path },
				"Formula with reference to attribute" );

			var calc8 = CreateCharacteristic( "Calc8", part.Path, "{Calc10(2101)}" );
			var calc9 = CreateCharacteristic( "Calc9", part.Path, "{Ch1} + {Ch2}" );
			var calc10 = CreateCharacteristic( "Calc10", part.Path, "{Calc8} - {Calc9}" );
			yield return new CharacteristicsInFormulaTestCase(
				new InspectionPlanDtoBase[] { part, calc8, calc9, calc10 },
				calc10.Path,
				null,
				new[] { ch1.Path, ch2.Path, calc8.Path, calc9.Path, calc10.Path },
				"Complex formula with reference to attribute" );
		}

		private static ConfigurationDto CreateBasicConfiguration()
		{
			return new ConfigurationDto
			{
				PartAttributes = new AbstractAttributeDefinitionDto[] { },
				CharacteristicAttributes = new AbstractAttributeDefinitionDto[]
				{
					new AttributeDefinitionDto
					{
						Key = WellKnownKeys.Characteristic.LogicalOperationString,
						Type = AttributeTypeDto.AlphaNumeric,
						Description = "Formula"
					}
				},
				MeasurementAttributes = new AbstractAttributeDefinitionDto[] { },
				ValueAttributes = new AbstractAttributeDefinitionDto[] { },
				CatalogAttributes = new AttributeDefinitionDto[] { }
			};
		}

		private static InspectionPlanPartDto CreatePart( string name )
		{
			return CreatePart( new PathInformation( PathElement.Part( name ) ) );
		}

		private static InspectionPlanPartDto CreatePart( PathInformation path )
		{
			return new InspectionPlanPartDto
			{
				Uuid = Guid.NewGuid(),
				Path = path
			};
		}

		private static InspectionPlanCharacteristicDto CreateCharacteristic( string name, PathInformation parentPath, string formula = null )
		{
			return CreateCharacteristic( parentPath + new PathElement( InspectionPlanEntity.Characteristic, name ), formula );
		}

		private static InspectionPlanCharacteristicDto CreateCharacteristic( PathInformation path, string formula = null )
		{
			var characteristic = new InspectionPlanCharacteristicDto
			{
				Uuid = Guid.NewGuid(),
				Path = path
			};

			if( !string.IsNullOrEmpty( formula ) )
				characteristic.SetAttributeValue( WellKnownKeys.Characteristic.LogicalOperationString, formula );

			return characteristic;
		}

		private static object GetCharacteristicAttribute( InspectionPlanCollection characteristics, PathInformation path, ushort key )
		{
			return GetCharacteristicAttribute( Configuration, CatalogCollection, characteristics, path, key );
		}

		private static object GetCharacteristicAttribute(
			ConfigurationDto configuration,
			CatalogCollectionDto catalogCollection,
			InspectionPlanCollection characteristics,
			PathInformation path,
			ushort key )
		{
			if( characteristics[ path ] is InspectionPlanCharacteristicDto characteristic )
				return characteristic.GetRawAttributeValue( key, configuration, catalogCollection );

			return null;
		}

		#endregion

		#region struct CharacteristicsInFormulaTestCase

		public readonly struct CharacteristicsInFormulaTestCase
		{
			#region constructors

			public CharacteristicsInFormulaTestCase(
				InspectionPlanDtoBase[] inspectionPlanEntities,
				PathInformation characteristicsPath,
				Type formulaValidationException,
				PathInformation[] expectedCharacteristicsPaths,
				string testName )
			{
				InspectionPlan = new InspectionPlanCollection( inspectionPlanEntities ?? Enumerable.Empty<InspectionPlanDtoBase>() );
				CharacteristicsPath = characteristicsPath;
				FormulaValidationException = formulaValidationException;
				ExpectedCharacteristicsPaths = expectedCharacteristicsPaths;
				TestName = testName;
			}

			#endregion

			#region properties

			public string TestName { get; }

			public InspectionPlanCollection InspectionPlan { get; }
			public PathInformation CharacteristicsPath { get; }
			public Type FormulaValidationException { get; }
			public PathInformation[] ExpectedCharacteristicsPaths { get; }

			#endregion

			#region methods

			public override string ToString()
			{
				return TestName;
			}

			#endregion
		}

		#endregion
	}
}