#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics.Tests.OprFunctions
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.Shared.CalculatedCharacteristics.Tests.Misc;

	#endregion

	public record OperationTestEnvironment
	{
		#region members

		private readonly MathInterpreter _MathInterpreter;

		private readonly EntityAttributeValueHandler _EntityAttributeValueHandler;

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of <see cref="OperationTestEnvironment"/>.
		/// </summary>
		public OperationTestEnvironment( InspectionPlanCollection characteristics )
		{
			_EntityAttributeValueHandler = CreateEntityAttributeHandler( characteristics );
			_MathInterpreter = GetMathInterpreter( characteristics, _EntityAttributeValueHandler );
		}

		#endregion

		#region methods

		private static EntityAttributeValueHandler CreateEntityAttributeHandler( InspectionPlanCollection characteristics )
		{
			var config = GetOprFunctionsConfiguration();
			var catalogs = GetOprFunctionsCatalogs( config );

			return ( path, k, _ ) => characteristics.GetCharacteristicAttribute( config, catalogs, path, k );
		}

		private static MathInterpreter GetMathInterpreter( InspectionPlanCollection characteristics, EntityAttributeValueHandler handler )
		{
			var attributeBasedMathInterpreterFactory = new AttributeBasedMathInterpreterFactory(
				( path, k ) => handler( path, k, null ),
				p => characteristics.GetDirectChildren( p ).Select( ch => ch.Path ) );

			return attributeBasedMathInterpreterFactory.GetInterpreter();
		}

		public double? GetResult( string formula, Dictionary<PathInformationDto, double> values )
		{
			var calculator = _MathInterpreter.Parse( formula, OprFunctionsTestHelper.GetDirectionPath( "MpResult", "X", true ) );
			return calculator.GetResult( v => values.ContainsKey( v ) ? values[ v ] : null, _EntityAttributeValueHandler );
		}

		public IEnumerable<PathInformationDto> GetDependentCharacteristics( string formula )
		{
			var calculator = _MathInterpreter.Parse( formula, OprFunctionsTestHelper.GetDirectionPath( "MpResult", "X", true ) );
			return calculator.GetDependentCharacteristics( _EntityAttributeValueHandler ).Keys;
		}

		private static ConfigurationDto GetOprFunctionsConfiguration()
		{
			return new ConfigurationDto
			{
				PartAttributes = new AbstractAttributeDefinitionDto[] { },
				CharacteristicAttributes = new AbstractAttributeDefinitionDto[]
				{
					new AttributeDefinitionDto { Key = WellKnownKeys.Characteristic.DesiredValue, Type = AttributeTypeDto.Float, Description = "Sollwert" },
					new AttributeDefinitionDto { Key = WellKnownKeys.Characteristic.LowerSpecificationLimit, Type = AttributeTypeDto.Float, Description = "Untere Toleranz" },
					new AttributeDefinitionDto { Key = WellKnownKeys.Characteristic.UpperSpecificationLimit, Type = AttributeTypeDto.Float, Description = "Obere Toleranz" },

					new CatalogAttributeDefinitionDto
					{
						Catalog = Guid.NewGuid(),
						Key = WellKnownKeys.Characteristic.ControlItem,
						Description = "Dokumentationspflicht"
					}
				},
				MeasurementAttributes = new AbstractAttributeDefinitionDto[] { },

				ValueAttributes = new AbstractAttributeDefinitionDto[] { },
				CatalogAttributes = new[] { new AttributeDefinitionDto( 4527, "Ja/Nein", AttributeTypeDto.AlphaNumeric, 255 ) }
			};
		}

		private static CatalogCollectionDto GetOprFunctionsCatalogs( [NotNull] ConfigurationDto config )
		{
			if( config == null ) throw new ArgumentNullException( nameof( config ) );
			var catalogDefinition = config.CharacteristicAttributes.OfType<CatalogAttributeDefinitionDto>().FirstOrDefault( att => att.Key == WellKnownKeys.Characteristic.ControlItem );
			if( catalogDefinition == null )
				throw new ArgumentNullException( nameof( config ), @$"Missing catalog attribute with key {nameof( WellKnownKeys.Characteristic.ControlItem )}" );

			return new CatalogCollectionDto( new[]
			{
				new CatalogDto
				{
					Uuid = catalogDefinition.Catalog,
					Name = "Dokumentationspflicht",
					ValidAttributes = new ushort[] { 4527 },
					CatalogEntries = new[]
					{
						new CatalogEntryDto
						{
							Key = 0,
							Attributes = new[]
							{
								new AttributeDto( 4527, "n.def." )
							}
						},
						new CatalogEntryDto
						{
							Key = 1,
							Attributes = new[]
							{
								new AttributeDto( 4527, "Ja" )
							}
						},
						new CatalogEntryDto
						{
							Key = 2,
							Attributes = new[]
							{
								new AttributeDto( 4527, "Nein" )
							}
						}
					}
				}
			} );
		}

		#endregion
	}
}