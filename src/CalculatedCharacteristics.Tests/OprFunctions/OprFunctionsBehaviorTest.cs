#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.OprFunctions
{
	#region usings

	using System.Collections.Generic;
	using Moq;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;
	using Zeiss.PiWeb.CalculatedCharacteristics.Functions;

	#endregion

	[TestFixture]
	public class OprFunctionsBehaviorTest
	{
		#region methods

		[Test]
		public void Should_Prefer_Extended_Direction_Name()
		{
			//***** arrange *****

			// PT_MAX({../Mp1};\"Y\")"
			const string nameMp = "Mp1";
			const string nameDirX = "X";
			const string nameDirY = "Y";

			// define all paths
			var pathPart = PathInformationDto.Combine( PathInformationDto.Root, PathElementDto.Part( "Test" ) );
			var pathMeasurementPoint = PathInformationDto.Combine( pathPart, PathElementDto.Char( nameMp ) );
			var longPathDirectionX = PathInformationDto.Combine( pathMeasurementPoint, PathElementDto.Char( nameMp + "." + nameDirX ) );
			var shortPathDirectionX = PathInformationDto.Combine( pathMeasurementPoint, PathElementDto.Char( nameDirX ) );
			var longPathDirectionY = PathInformationDto.Combine( pathMeasurementPoint, PathElementDto.Char( nameMp + "." + nameDirY ) );
			var shortPathDirectionY = PathInformationDto.Combine( pathMeasurementPoint, PathElementDto.Char( nameDirY ) );

			// define test values for paths
			var values = new Dictionary<PathInformationDto, double>
			{
				{ shortPathDirectionX, 40d },
				{ shortPathDirectionY, 30d },
				{ longPathDirectionX, 20d },
				{ longPathDirectionY, 10d }
			};

			// define input parameters
			var characteristicMathElement = new Characteristic( 0, 6, pathMeasurementPoint.ToString(), pathMeasurementPoint, null );
			var directionMathElement = new Literal( 0, 1, nameDirY );

			// setup ICharacteristicValueResolver
			var characteristicValueResolverMoq = new Mock<ICharacteristicValueResolver>( MockBehavior.Strict );
			characteristicValueResolverMoq.Setup( resolver => resolver.GetChildPaths( pathMeasurementPoint ) ).Returns( values.Keys );
			characteristicValueResolverMoq.Setup( resolver => resolver.GetMeasurementValue( It.IsAny<PathInformationDto>() ) ).Returns<PathInformationDto>( path => values[ path ] );

			//***** act *****
			var result = OprFunctions.Pt_Max( new MathElement[] { characteristicMathElement, directionMathElement }, characteristicValueResolverMoq.Object );

			//***** assert *****
			Assert.That( result, Is.EqualTo( values[ longPathDirectionY ] ) );
		}

		#endregion
	}
}