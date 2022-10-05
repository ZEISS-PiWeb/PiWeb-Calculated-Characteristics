#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2018                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.OprFunctions
{
	#region usings

	using System;
	using System.Collections.Generic;
	using Zeiss.PiWeb.Api.Contracts;
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.CalculatedCharacteristics.Tests.Misc;
	using Attribute = Zeiss.PiWeb.Api.Contracts.Attribute;

	#endregion

	public static class OprFunctionsTestHelper
	{
		#region methods

		public static InspectionPlanCollection SetupInspectionPlanWithOprFunctionPoint( IEnumerable<CharacteristicInfo> chInfo )
		{
			var characteristics = new InspectionPlanCollection();
			characteristics.AddRange( CreateMeasurementPoint( "MpResult", chInfo, true ) );
			return characteristics;
		}

		public static IEnumerable<InspectionPlanCharacteristicDto> CreateMeasurementPoint( string baseName, IEnumerable<CharacteristicInfo> chInfo, bool extendedName )
		{
			yield return new InspectionPlanCharacteristicDto { Uuid = Guid.NewGuid(), Path = new PathInformation( PathElement.Part( "Test" ), PathElement.Char( baseName ) ) };

			foreach( var characteristic in chInfo )
			{
				var ch = new InspectionPlanCharacteristicDto { Uuid = Guid.NewGuid(), Path = GetDirectionPath( baseName, characteristic.Direction, extendedName ) };
				if( characteristic.Desired.HasValue )
					ch.SetAttribute( new Attribute( WellKnownKeys.Characteristic.DesiredValue, characteristic.Desired.Value ) );

				if( characteristic.Tolerance.LowerBound.HasValue )
					ch.SetAttribute( new Attribute( WellKnownKeys.Characteristic.LowerSpecificationLimit, characteristic.Tolerance.LowerBound.Value ) );
				if( characteristic.Tolerance.UpperBound.HasValue )
					ch.SetAttribute( new Attribute( WellKnownKeys.Characteristic.UpperSpecificationLimit, characteristic.Tolerance.UpperBound.Value ) );

				if( characteristic.IsControlItem.HasValue && characteristic.IsControlItem.Value )
					ch.SetAttribute( new Attribute( WellKnownKeys.Characteristic.ControlItem, "1" ) );

				yield return ch;
			}
		}

		public static PathInformation GetDirectionPath( string baseName, string direction, bool isExtended )
		{
			var directionCharacteristicName = isExtended ? baseName + "." + direction : direction;
			return new PathInformation( PathElement.Part( "Test" ), PathElement.Char( baseName ), PathElement.Char( directionCharacteristicName ) );
		}

		#endregion
	}
}