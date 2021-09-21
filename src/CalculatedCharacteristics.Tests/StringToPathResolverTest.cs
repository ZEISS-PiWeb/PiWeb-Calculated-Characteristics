#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics.Tests
{
	#region usings

	using System.Collections.Generic;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Rest.Dtos;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;

	#endregion

	[TestFixture]
	public class StringToPathResolverTest
	{
		#region methods

		[Test]
		[TestCaseSource( nameof( CreateResolvePathTestData ) )]
		public void Test_ResolvePath( ResolvePathTestCase testCase )
		{
			// Given

			// When
			var result = new StringToPathResolver( testCase.ParentPath ).ResolvePath( testCase.PathString );

			// Then
			Assert.That( result, Is.EqualTo( testCase.ExpectedPath ), () => CreateErrorMessage( result, testCase.ExpectedPath ) );
		}

		private static string CreateErrorMessage( PathInformationDto result, PathInformationDto expected )
		{
			var resultString = result != null ? PathHelper.PathInformation2RoundtripString( result ) : "<null>";
			var expectedString = expected != null ? PathHelper.PathInformation2RoundtripString( expected ) : "<null>";

			return $"Got path '{resultString}', but expected '{expectedString}'.";
		}

		private static IEnumerable<ResolvePathTestCase> CreateResolvePathTestData()
		{
			yield return new ResolvePathTestCase
			{
				PathString = null,
				ParentPath = PathInformationDto.Root,
				ExpectedPath = null
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = null,
				ExpectedPath = PathInformationDto.Root + PathElementDto.Char( "foo" ) + PathElementDto.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "foo/bar",
				ParentPath = null,
				ExpectedPath = PathInformationDto.Root + PathElementDto.Char( "foo" ) + PathElementDto.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "",
				ParentPath = PathInformationDto.Root,
				ExpectedPath = null
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/",
				ParentPath = PathInformationDto.Root,
				ExpectedPath = PathInformationDto.Root
			};

			// Absolute paths
			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = PathInformationDto.Root,
				ExpectedPath = PathInformationDto.Root + PathElementDto.Char( "foo" ) + PathElementDto.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Part( "foo" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Part( "foo" ) + PathElementDto.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Part( "foo" ) + PathElementDto.Part( "bar" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Part( "foo" ) + PathElementDto.Part( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Part( "foo" ) + PathElementDto.Part( "foo" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Part( "foo" ) + PathElementDto.Char( "bar" )
			};

			// Relative paths
			yield return new ResolvePathTestCase
			{
				PathString = "../foobar",
				ParentPath = PathInformationDto.Root + PathElementDto.Char( "foobar" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Char( "foobar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foobar",
				ParentPath = PathInformationDto.Root + PathElementDto.Part( "foobar" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Part( "foobar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Part( "part" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Char( "foo" ) + PathElementDto.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Part( "foo" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Part( "foo" ) + PathElementDto.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Char( "char" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Char( "foo" ) + PathElementDto.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Char( "foo" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Char( "foo" ) + PathElementDto.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../../foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Part( "part" ),
				ExpectedPath = null
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../../foo/bar",
				ParentPath = PathInformationDto.Root + PathElementDto.Part( "part1" ) + PathElementDto.Part( "part2" ),
				ExpectedPath = PathInformationDto.Root + PathElementDto.Char( "foo" ) + PathElementDto.Char( "bar" )
			};
		}

		#endregion

		#region struct ResolvePathTestCase

		public struct ResolvePathTestCase
		{
			#region members

			public string PathString;
			public PathInformationDto ParentPath;
			public PathInformationDto ExpectedPath;

			#endregion

			#region methods

			public override string ToString()
			{
				var parentPathText = ParentPath != null
					? PathHelper.PathInformation2RoundtripString( ParentPath )
					: "<null>";

				var expectedPathText = ExpectedPath != null
					? PathHelper.PathInformation2RoundtripString( ExpectedPath )
					: "<null>";

				return $"'{PathString ?? "<null>"}'"
					+ $" (parent: '{parentPathText}')"
					+ $" => '{expectedPathText}'";
			}

			#endregion
		}

		#endregion
	}
}