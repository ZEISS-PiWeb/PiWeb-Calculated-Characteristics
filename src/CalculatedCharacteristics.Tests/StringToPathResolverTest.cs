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

	using System.Collections.Generic;
	using NUnit.Framework;
	using Zeiss.PiWeb.Api.Core;

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

		private static string CreateErrorMessage( PathInformation? result, PathInformation? expected )
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
				ParentPath = PathInformation.Root,
				ExpectedPath = null
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = null,
				ExpectedPath = PathInformation.Root + PathElement.Char( "foo" ) + PathElement.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "foo/bar",
				ParentPath = null,
				ExpectedPath = PathInformation.Root + PathElement.Char( "foo" ) + PathElement.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "",
				ParentPath = PathInformation.Root,
				ExpectedPath = null
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/",
				ParentPath = PathInformation.Root,
				ExpectedPath = PathInformation.Root
			};

			// Absolute paths
			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = PathInformation.Root,
				ExpectedPath = PathInformation.Root + PathElement.Char( "foo" ) + PathElement.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = PathInformation.Root + PathElement.Part( "foo" ),
				ExpectedPath = PathInformation.Root + PathElement.Part( "foo" ) + PathElement.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = PathInformation.Root + PathElement.Part( "foo" ) + PathElement.Part( "bar" ),
				ExpectedPath = PathInformation.Root + PathElement.Part( "foo" ) + PathElement.Part( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "/foo/bar",
				ParentPath = PathInformation.Root + PathElement.Part( "foo" ) + PathElement.Part( "foo" ),
				ExpectedPath = PathInformation.Root + PathElement.Part( "foo" ) + PathElement.Char( "bar" )
			};

			// Relative paths
			yield return new ResolvePathTestCase
			{
				PathString = "../foobar",
				ParentPath = PathInformation.Root + PathElement.Char( "foobar" ),
				ExpectedPath = PathInformation.Root + PathElement.Char( "foobar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foobar",
				ParentPath = PathInformation.Root + PathElement.Part( "foobar" ),
				ExpectedPath = PathInformation.Root + PathElement.Part( "foobar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foo/bar",
				ParentPath = PathInformation.Root + PathElement.Part( "part" ),
				ExpectedPath = PathInformation.Root + PathElement.Char( "foo" ) + PathElement.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foo/bar",
				ParentPath = PathInformation.Root + PathElement.Part( "foo" ),
				ExpectedPath = PathInformation.Root + PathElement.Part( "foo" ) + PathElement.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foo/bar",
				ParentPath = PathInformation.Root + PathElement.Char( "char" ),
				ExpectedPath = PathInformation.Root + PathElement.Char( "foo" ) + PathElement.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../foo/bar",
				ParentPath = PathInformation.Root + PathElement.Char( "foo" ),
				ExpectedPath = PathInformation.Root + PathElement.Char( "foo" ) + PathElement.Char( "bar" )
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../../foo/bar",
				ParentPath = PathInformation.Root + PathElement.Part( "part" ),
				ExpectedPath = null
			};

			yield return new ResolvePathTestCase
			{
				PathString = "../../foo/bar",
				ParentPath = PathInformation.Root + PathElement.Part( "part1" ) + PathElement.Part( "part2" ),
				ExpectedPath = PathInformation.Root + PathElement.Char( "foo" ) + PathElement.Char( "bar" )
			};
		}

		#endregion

		#region struct ResolvePathTestCase

		public struct ResolvePathTestCase
		{
			#region members

			public string? PathString;
			public PathInformation? ParentPath;
			public PathInformation? ExpectedPath;

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