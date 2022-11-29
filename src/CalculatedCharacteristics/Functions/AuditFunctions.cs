#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;
	using Zeiss.PiWeb.Api.Core;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;
	using Characteristic = Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic.Characteristic;

	#endregion

	/// <summary>
	/// Audit functions for use in the <see cref="MathInterpreter"/>.
	/// </summary>
	public static class AuditFunctions
	{
		#region constants

		private const string Measured = "Measured";
		private const string OutOfTolerance = "OutOfTolerance";
		private const string Missing = "Missing";
		private const string MissingAsOutOfTolerance = "MissingAsOOT";

		#endregion

		#region methods

		/// <summary>
		/// Audit grade ("Qualitätszahl").
		/// Calculates the audit grade for the given (audit function) characteristic.
		/// If the first argument is the literal "MissingAsOOT" then missing measured values
		/// for characteristics with an audit function will be counted as out of tolerance.
		/// If no characteristic is given, the audit grade is calculated for the parent characteristic
		/// because then it is assumed that the characteristic with the formula is located directly
		/// beneath the (audit function) characteristic and next to "Measured", "OutOfTolerance" (and "Missing").
		/// Possible calls:
		/// QZ()
		/// QZ( {Auditfunction} )
		/// QZ( "MissingAsOOT" )
		/// QZ( "MissingAsOOT", {Auditfunction})
		/// </summary>
		public static double? AuditGrade( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			if( args.Count > 2 ) throw new ArgumentException( "Function 'QZ' accepts at most 2 parameters!" );

			var node = GetAuditFunctionCharacteristic( args, resolver.SourcePath, out var countMissingAsOutOfTolerance );
			if( node.Path == null )
				return null;

			return CalculateAverageAuditGrade( new[] { node.Path }, resolver, countMissingAsOutOfTolerance );
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="AuditGrade"/>.
		/// </summary>
		public static IEnumerable<MathDependencyInformation> AuditGrade_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			if( args.Count > 2 )
				return Enumerable.Empty<MathDependencyInformation>();

			try
			{
				var node = GetAuditFunctionCharacteristic( args, resolver.SourcePath, out var countMissingAsOutOfTolerance );
				if( node.Path == null )
					return Enumerable.Empty<MathDependencyInformation>();

				var startPosition = node.MathElement?.TokenStartPosition ?? -1;
				var length = node.MathElement?.TokenLength ?? -1;
				var text = node.MathElement?.Text;
				var attributeKey = node.MathElement?.AttributeKey;

				var result = new List<MathDependencyInformation>
				{
					new MathDependencyInformation( PathInformation.Combine( node.Path, PathElement.Char( Measured ) ), startPosition, length, text, attributeKey ),
					new MathDependencyInformation( PathInformation.Combine( node.Path, PathElement.Char( OutOfTolerance ) ), startPosition, length, text, attributeKey )
				};

				if( countMissingAsOutOfTolerance )
					result.Add( new MathDependencyInformation( PathInformation.Combine( node.Path, PathElement.Char( Missing ) ), startPosition, length, text, attributeKey ) );

				return result;
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		private static (PathInformation Path, Characteristic MathElement) GetAuditFunctionCharacteristic(
			[NotNull] IReadOnlyCollection<MathElement> args,
			PathInformation parent,
			out bool countMissingAsOutOfTolerance )
		{
			countMissingAsOutOfTolerance = false;

			if( args.Count == 0 )
				return ( parent?.ParentPath, null );

			if( args.Count == 1 )
			{
				// only 1 argument: literal "MissingAsOOT" or characteristic
				if( args.First() is Literal literal && literal.Text.Equals( MissingAsOutOfTolerance, StringComparison.OrdinalIgnoreCase ) )
				{
					countMissingAsOutOfTolerance = true;
					return (parent?.ParentPath, null);
				}

				if( args.First() is Characteristic characteristic )
					return ( characteristic.Path, characteristic );

				throw new ArgumentException( "Function 'QZ' accepts only a literal [MissingAsOOT] or a characteristic as its first parameter if only 1 parameter is given!" );
			}

			// at least 2 arguments - 1. argument: literal "MissingAsOOT"; 2. argument: characteristic
			if( args.ElementAt( 0 ) is Literal lit && lit.Text.Equals( MissingAsOutOfTolerance, StringComparison.OrdinalIgnoreCase ) )
			{
				countMissingAsOutOfTolerance = true;
			}
			else
				throw new ArgumentException( "Function 'QZ' accepts only a literal [MissingAsOOT] as its first parameter if 2 parameters are given!" );

			if( args.ElementAt( 1 ) is Characteristic ch )
				return ( ch.Path, ch );

			throw new ArgumentException( "Function 'QZ' accepts only a characteristic as its second parameter!" );
		}

		/// <summary>
		/// Average audit grade.
		/// Calculates the average audit grade for the given (audit function) characteristics.
		/// If the first argument is the literal "MissingAsOOT" then missing measured values
		/// for characteristics with an audit function will be counted as out of tolerance.
		/// If no characteristic is given, the average audit grade is calculated for the neighbouring characteristics
		/// because then it is assumed that the characteristic with the formula is located
		/// next to the (audit function) characteristics (e.g. next to "auditrelevant" or "FO li Dichtkante")
		/// which contain the child characteristics "Measured", "OutOfTolerance" (and "Missing").
		/// Possible calls:
		/// AverageQZ()
		/// AverageQZ( {Auditfunction1} )
		/// AverageQZ( {Auditfunction1}, {Auditfunction2} )
		/// ...
		/// AverageQZ( "MissingAsOOT" )
		/// AverageQZ( "MissingAsOOT", {Auditfunction1} )
		/// AverageQZ( "MissingAsOOT", {Auditfunction1}, {Auditfunction2} )
		/// ...
		/// </summary>
		public static double? AverageAuditGrade( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			return CalculateAuditGrade( args, resolver, false );
		}

		/// <summary>
		/// Grouped audit grade.
		/// Calculates the grouped audit grade for the given (audit function) characteristics.
		/// If the first argument is the literal "MissingAsOOT" then missing measured values
		/// for characteristics with an audit function will be counted as out of tolerance.
		/// If no characteristic is given, the grouped audit grade is calculated for the neighbouring characteristics
		/// because then it is assumed that the characteristic with the formula is located
		/// next to the (audit function) characteristics (e.g. next to "auditrelevant" or "FO li Dichtkante")
		/// which contain the child characteristics "Measured", "OutOfTolerance" (and "Missing").
		/// Possible calls:
		/// GroupedQZ()
		/// GroupedQZ( {Auditfunction1} )
		/// GroupedQZ( {Auditfunction1}, {Auditfunction2} )
		/// ...
		/// GroupedQZ( "MissingAsOOT" )
		/// GroupedQZ( "MissingAsOOT", {Auditfunction1} )
		/// GroupedQZ( "MissingAsOOT", {Auditfunction1}, {Auditfunction2} )
		/// ...
		/// </summary>
		public static double? GroupedAuditGrade( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			return CalculateAuditGrade( args, resolver, true );
		}

		private static double? CalculateAuditGrade( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver, bool grouped )
		{
			var paths = GetAuditFunctionCharacteristics( args, resolver, out var countMissingAsOutOfTolerance )
				.Select( node => node.Path );

			return grouped
				? CalcGroupedAuditGrade( paths, resolver, countMissingAsOutOfTolerance )
				: CalculateAverageAuditGrade( paths, resolver, countMissingAsOutOfTolerance );
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="AverageAuditGrade"/> or <see cref="GroupedAuditGrade"/>.
		/// </summary>
		public static IEnumerable<MathDependencyInformation> AverageOrGroupedAuditGrade_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				var nodes = GetAuditFunctionCharacteristics( args, resolver, out var countMissingAsOutOfTolerance );
				if( nodes == null )
					return Enumerable.Empty<MathDependencyInformation>();

				var result = new List<MathDependencyInformation>();
				foreach( var node in nodes )
				{
					var startPosition = node.MathElement?.TokenStartPosition ?? -1;
					var length = node.MathElement?.TokenLength ?? -1;
					var text = node.MathElement?.Text;
					var attributeKey = node.MathElement?.AttributeKey;

					result.Add( new MathDependencyInformation( PathInformation.Combine( node.Path, PathElement.Char( Measured ) ), startPosition,length,text,attributeKey ) );
					result.Add( new MathDependencyInformation( PathInformation.Combine( node.Path, PathElement.Char( OutOfTolerance ) ), startPosition,length,text,attributeKey ) );

					if( countMissingAsOutOfTolerance )
						result.Add( new MathDependencyInformation( PathInformation.Combine( node.Path, PathElement.Char( Missing ) ), startPosition,length,text,attributeKey ) );
				}

				return result;
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		private static IEnumerable<(PathInformation Path, Characteristic MathElement)> GetAuditFunctionCharacteristics(
			[NotNull] IReadOnlyCollection<MathElement> args,
			[NotNull] ICharacteristicInfoResolver resolver,
			out bool countMissingAsOutOfTolerance )
		{
			countMissingAsOutOfTolerance = false;

			// no arguments => return all existing audit function characteristics next to the source path
			if( args.Count == 0 )
			{
				return GetPathsFromSourcePathSiblings( resolver )
					.Select<PathInformation,(PathInformation Path, Characteristic MathElement)>( path => ( path, null ) );
			}

			switch( args.First() )
			{
				case Literal literal when literal.Text.Equals( MissingAsOutOfTolerance, StringComparison.OrdinalIgnoreCase ):
				{
					countMissingAsOutOfTolerance = true;

					// no further arguments => return all existing audit function characteristics next to the source path
					if( args.Count == 1 )
					{
						return GetPathsFromSourcePathSiblings( resolver )
							.Select<PathInformation,(PathInformation Path, Characteristic MathElement)>( path => ( path, null ) );
					}

					// further arguments after literal "MissingAsOOT" must be characteristics
					return GetPathsFromCharacteristics( args.Skip(1) );
				}

				case Characteristic _:
					return GetPathsFromCharacteristics( args );

				default:
					throw new ArgumentException( "Function accepts only a literal [MissingAsOOT] or a characteristic as its first parameter!" );
			}
		}

		private static IEnumerable<(PathInformation,Characteristic)> GetPathsFromCharacteristics( [NotNull] IEnumerable<MathElement> args )
		{
			foreach( var mathElement in args )
			{
				if( !( mathElement is Characteristic characteristic ) )
					throw new ArgumentException( "Parameter at position " + ( mathElement.TokenStartPosition + 1 ) + " is not a characteristic!" );

				if( characteristic.Path != null )
					yield return ( characteristic.Path, characteristic );
			}
		}

		private static IEnumerable<PathInformation> GetPathsFromSourcePathSiblings( [NotNull] ICharacteristicInfoResolver resolver )
		{
			var parentPath = resolver.SourcePath?.ParentPath;
			if( parentPath == null )
				return Enumerable.Empty<PathInformation>();

			var childPaths = new HashSet<PathInformation>( resolver.GetChildPaths( parentPath ) );
			childPaths.Remove( resolver.SourcePath );
			return childPaths;
		}

		/// <summary>
		/// Calculates the audit grade for each audit function characteristic and returns the average for those audit grades.
		/// </summary>
		private static double? CalculateAverageAuditGrade(
			[NotNull] IEnumerable<PathInformation> paths,
			[NotNull] ICharacteristicValueResolver resolver,
			bool countMissingAsOutOfTolerance )
		{
			var dict = new Dictionary<string, double>();
			foreach( var path in paths )
			{
				var measured = (int?)resolver.GetMeasurementValue( PathInformation.Combine( path, PathElement.Char( Measured ) ) );
				if( !measured.HasValue )
					continue;

				var outOfTolerance = (int?)resolver.GetMeasurementValue( PathInformation.Combine( path, PathElement.Char( OutOfTolerance ) ) );
				var missing = (int?)resolver.GetMeasurementValue( PathInformation.Combine( path, PathElement.Char( Missing ) ) );

				dict[ path.Name ] = CalcQuantitativeAuditValue( measured.Value, outOfTolerance ?? 0, countMissingAsOutOfTolerance ? missing : null );
			}

			if( dict.Any() )
				return dict.Values.Average();

			return null;
		}

		/// <summary>
		/// Groups the audit information (number of characteristics measured/out of tolerance/missing) of all audit function characteristics
		/// together and calculates the audit grade based on the grouped audit information.
		/// </summary>
		private static double? CalcGroupedAuditGrade(
			[NotNull] IEnumerable<PathInformation> paths,
			[NotNull] ICharacteristicValueResolver resolver,
			bool countMissingAsOutOfTolerance )
		{
			var allMeasured = 0;
			var allOutOfTolerance = 0;
			var allMissing = 0;

			foreach( var path in paths )
			{
				var measured = (int?)resolver.GetMeasurementValue( PathInformation.Combine( path, PathElement.Char( Measured ) ) );
				if( !measured.HasValue )
					continue;

				var outOfTolerance = (int?)resolver.GetMeasurementValue( PathInformation.Combine( path, PathElement.Char( OutOfTolerance ) ) );
				var missing = (int?)resolver.GetMeasurementValue( PathInformation.Combine( path, PathElement.Char( Missing ) ) );

				allMeasured += measured.Value;
				allOutOfTolerance += outOfTolerance ?? 0;
				allMissing += missing ?? 0;
			}

			if( allMeasured > 0 )
				return CalcQuantitativeAuditValue( allMeasured, allOutOfTolerance, countMissingAsOutOfTolerance ? allMissing : 0 );

			return null;
		}

		private static double CalcQuantitativeAuditValue( int measured, int outOfTolerance, int? missing = null )
		{
			if( missing.HasValue )
			{
				outOfTolerance += missing.Value;
				measured += missing.Value;
			}

			return (double)outOfTolerance / measured * 10.0 + 1.0;
		}

		#endregion
	}
}