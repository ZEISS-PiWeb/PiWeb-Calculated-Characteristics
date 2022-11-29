#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2015                             */
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
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Functional dimensions from the car body sector for use in the <see cref="MathInterpreter"/>.
	/// </summary>
	public static partial class OprFunctions
	{
		#region constants

		/// <summary>
		/// Name of function <see cref="Pt_Min"/>.
		/// </summary>
		public const string PtMin = "PT_MIN";

		/// <summary>
		/// Name of function <see cref="Pt_Max"/>.
		/// </summary>
		public const string PtMax = "PT_MAX";

		/// <summary>
		/// Name of function <see cref="Pt_Sym"/>.
		/// </summary>
		public const string PtSym = "PT_SYM";

		/// <summary>
		/// Name of function <see cref="Pt_Dist"/>.
		/// </summary>
		public const string PtDist = "PT_DIST";

		/// <summary>
		/// Name of function <see cref="Pt_Ref"/>.
		/// </summary>
		public const string PtRef = "PT_REF";

		/// <summary>
		/// Name of function <see cref="Pt_Pos_Square"/>.
		/// </summary>
		public const string PtPosSquare = "PT_POS_SQUARE";

		/// <summary>
		/// Name of function <see cref="Pt_Profile"/>.
		/// </summary>
		public const string PtProfile = "PT_PROFILE";

		/// <summary>
		/// Name of function <see cref="Pt_Worst"/>.
		/// </summary>
		public const string PtWorst = "PT_WORST";

		/// <summary>
		/// Name of function <see cref="Pt_Worst_Target"/>.
		/// </summary>
		public const string PtWorstTarget = "PT_WORST_TARGET";

		/// <summary>
		/// Name of function <see cref="Pt_Dist_Pt_2Pt"/>.
		/// </summary>
		public const string PtDistPoint2Line = "PT_DIST_PT_2PT";

		/// <summary>
		/// Name of function <see cref="Pt_Dist_Pt_3Pt"/>.
		/// </summary>
		public const string PtDistPoint2Plane = "PT_DIST_PT_3PT";

		private static readonly string[] DirectionsXy = { "X", "Y" };
		private static readonly string[] DirectionsXz = { "X", "Z" };
		private static readonly string[] DirectionsYz = { "Y", "Z" };
		private static readonly string[] DirectionsXyz = { "X", "Y", "Z" };
		private static readonly string[] DirectionsXyzn = { "X", "Y", "Z", "N" };
		private static readonly string[] DirectionsXyze = { "X", "Y", "Z", "E" };
		private static readonly string[] DirectionsXyznp = { "X", "Y", "Z", "N", "P" };
		private static readonly string[] DirectionsAllAxisAndN = { "X", "Y", "Z", "N", "XY", "XZ", "YZ", "XYZ" };

		#endregion

		#region methods

		/// <summary>
		/// Calculates the minimum from a list of characteristics.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// * [optional] literal "true" to calculate a result only if all characteristics have a value
		/// </summary>
		[OperationTemplate( PtMin+"($PATHS;$DIRECTION;$CHECK)", OperationTemplateTypes.PtMin )]
		public static double? Pt_Min( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtMin, 1, true );

			var direction = GetDirection( args );
			var values = GetCharacteristics( args ).Select( ch => ch.GetValue( resolver, direction ) ).ToArray();

			if( AllValuesRequired( args ) && values.Any( v => !v.HasValue ) )
				return null;

			return values.Min();
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Min"/>.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Min_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtMin, 1, true );

				var direction = GetDirection( args );
				var characteristics = GetCharacteristics( args );
				return GetDirectionDependencies( resolver, characteristics, direction );
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		/// <summary>
		/// Calculates the minimum from a list of characteristics.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// * [optional] literal "true" to calculate a result only if all characteristics have a value
		/// </summary>
		[OperationTemplate( PtMax + "($PATHS;$DIRECTION;$CHECK)", OperationTemplateTypes.PtMax )]
		public static double? Pt_Max( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtMax, 1, true );

			var direction = GetDirection( args );
			var values = GetCharacteristics( args ).Select( ch => ch.GetValue( resolver, direction ) ).ToArray();

			if( AllValuesRequired( args ) && values.Any( v => !v.HasValue ) )
				return null;

			return values.Max();
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Max"/>.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Max_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtMax, 1, true );

				var direction = GetDirection( args );
				var characteristics = GetCharacteristics( args );
				return GetDirectionDependencies( resolver, characteristics, direction );
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		private static bool TryCreateDirectionDependency(
			[NotNull] ICharacteristicInfoResolver resolver,
			Characteristic characteristic,
			string direction,
			out MathDependencyInformation dependencyInformation )
		{
			var childPath = GetDirectionChild( resolver, characteristic.Path, direction );
			if( childPath == null )
			{
				dependencyInformation = MathDependencyInformation.Empty;
				return false;
			}

			dependencyInformation = new MathDependencyInformation( childPath, characteristic.TokenStartPosition, characteristic.TokenLength, characteristic.Text, characteristic.AttributeKey );
			return true;
		}

		private static IEnumerable<MathDependencyInformation> GetDirectionDependencies(
			[NotNull] ICharacteristicInfoResolver resolver,
			Characteristic characteristic,
			string direction )
		{
			if( TryCreateDirectionDependency( resolver, characteristic, direction, out var mathDependencyInformation ) )
				return new[] { mathDependencyInformation };

			return Enumerable.Empty<MathDependencyInformation>();
		}

		private static IEnumerable<MathDependencyInformation> GetDirectionDependencies(
			[NotNull] ICharacteristicInfoResolver resolver,
			Characteristic characteristic,
			IReadOnlyCollection<string> directions )
		{
			var result = new List<MathDependencyInformation>( directions.Count );
			foreach( var direction in directions )
			{
				if( TryCreateDirectionDependency( resolver, characteristic, direction, out var mathDependencyInformation ) )
					result.Add( mathDependencyInformation );
			}

			return result;
		}

		private static IEnumerable<MathDependencyInformation> GetDirectionDependencies(
			[NotNull] ICharacteristicInfoResolver resolver,
			IReadOnlyCollection<Characteristic> characteristics,
			string direction )
		{
			var result = new List<MathDependencyInformation>( characteristics.Count );
			foreach( var characteristic in characteristics )
			{
				if( TryCreateDirectionDependency( resolver, characteristic, direction, out var mathDependencyInformation ) )
					result.Add( mathDependencyInformation );
			}

			return result;
		}

		private static IEnumerable<MathDependencyInformation> GetDirectionDependencies(
			[NotNull] ICharacteristicInfoResolver resolver,
			IReadOnlyCollection<Characteristic> characteristics,
			string[] directions )
		{
			var result = new List<MathDependencyInformation>( characteristics.Count * directions.Length );
			foreach( var characteristic in characteristics )
			{
				foreach( var direction in directions )
				{
					if( TryCreateDirectionDependency( resolver, characteristic, direction, out var mathDependencyInformation ) )
						result.Add( mathDependencyInformation );
				}
			}

			return result;
		}

		/// <summary>
		/// Provides the path information for a measurement point direction.
		/// </summary>
		/// <remarks>
		/// The direction matching prefer the extended naming format by design.
		/// </remarks>
		[CanBeNull]
		private static PathInformation GetDirectionChild( [NotNull] ICharacteristicInfoResolver resolver, PathInformation characteristic, string direction )
		{
			var extendedDirectionName = GetCharacteristicByDirectionExtendedName( characteristic, direction ).Name;
			var shortDirectionName = GetCharacteristicByDirectionShortName( characteristic, direction ).Name;

			PathInformation foundDirectionPath = null;

			foreach( var childPath in resolver.GetChildPaths( characteristic ) )
			{
				var childName = childPath.Name;
				if( childName.Equals( extendedDirectionName ) )
					return childPath;

				if( childName.Equals( shortDirectionName ) )
					foundDirectionPath = childPath;
			}

			return foundDirectionPath;
		}

		/// <summary>
		/// Calculates the symmetry point from two characteristics.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// * [optional] literal "true" to calculate a result only if all characteristics have a value
		/// </summary>
		[OperationTemplate( PtSym + "($PATHS;$DIRECTION;$CHECK)", OperationTemplateTypes.PtSym )]
		public static double? Pt_Sym( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtSym, 1, true );

			var direction = GetDirection( args );
			var characteristics = GetCharacteristics( args ).Select( ch => ch.GetValue( resolver, direction ) ).ToArray();

			if( AllValuesRequired( args ) && characteristics.Any( v => !v.HasValue ) )
				return null;

			var values = characteristics.Where( v => v.HasValue ).ToArray();
			if( values.Length == 0 )
				return null;

			var result = values.Sum() / values.Length;
			if( result.HasValue && values.Length > 1 )
				result = Math.Round( result.Value, 3 );

			return result;
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Sym"/>.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Sym_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				var (characteristics, direction) = AnalyzeArguments( args, PtSym, 1, true );
				return GetDirectionDependencies( resolver, characteristics, direction );
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		/// <summary>
		/// Calculates the distance between two characteristics.
		/// Expected arguments:
		/// * 2 characteristics
		/// * 1 direction literal (X,Y,Z,E)
		/// </summary>
		[OperationTemplate( PtDist + "($PATH0;$PATH1;$DIRECTION)", OperationTemplateTypes.PtDist )]
		public static double? Pt_Dist( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			var (characteristics, direction) =AnalyzeArguments( args, PtDist, 2, false, DirectionsXyze );

			var ch1 = characteristics[ 0 ];
			var ch2 = characteristics[ 1 ];

			const ushort nominalValueKey = WellKnownKeys.Characteristic.DesiredValue;

			var p1Nominal = new[]
			{
				ch1.GetAttribute( resolver, nominalValueKey, "X" ),
				ch1.GetAttribute( resolver, nominalValueKey, "Y" ),
				ch1.GetAttribute( resolver, nominalValueKey, "Z" )
			};
			var p2Nominal = new[]
			{
				ch2.GetAttribute( resolver, nominalValueKey, "X" ),
				ch2.GetAttribute( resolver, nominalValueKey, "Y" ),
				ch2.GetAttribute( resolver, nominalValueKey, "Z" )
			};
			var p1Diff = new[] { ch1.GetValue( resolver, "X" ), ch1.GetValue( resolver, "Y" ), ch1.GetValue( resolver, "Z" ) };
			var p2Diff = new[] { ch2.GetValue( resolver, "X" ), ch2.GetValue( resolver, "Y" ), ch2.GetValue( resolver, "Z" ) };

			return Calc_Pt_Dist( direction, p1Diff, p1Nominal, p2Diff, p2Nominal );
		}

		/// <summary>
		/// Calculates the distance between two points (PT_DIST).
		/// </summary>
		/// <param name="direction">Direction coordinate</param>
		/// <param name="values1">Measured values for point1 [XYZ]</param>
		/// <param name="nominalValues1">Nominal values for point1 [XYZ]</param>
		/// <param name="values2">Measured values for point2 [XYZ]</param>
		/// <param name="nominalValues2">Nominal values for point2 [XYZ]</param>
		/// <returns>The calculated distance or <code>null</code> if the distance could not be calculated.</returns>
		public static double? Calc_Pt_Dist( string direction, double?[] values1, double?[] nominalValues1, double?[] values2, double?[] nominalValues2 )
		{
			ValidateForThreePointVector( values1, nameof( values1 ) );
			ValidateForThreePointVector( values2, nameof( values2 ) );
			ValidateForThreePointVector( nominalValues1, nameof( nominalValues1 ) );
			ValidateForThreePointVector( nominalValues2, nameof( nominalValues2 ) );

			double? result = null;
			if( direction is "X" or "Y" || direction == "Z" )
			{
				var i = Array.IndexOf( DirectionsXyze, direction );
				if( nominalValues1[ i ].HasValue && nominalValues2[ i ].HasValue && values1[ i ].HasValue && values2[ i ].HasValue )
				{
					result = Math.Abs( nominalValues2[ i ].Value + values2[ i ].Value - nominalValues1[ i ].Value - values1[ i ].Value ) -
						Math.Abs( nominalValues2[ i ].Value - nominalValues1[ i ].Value );
				}
			}
			else if( direction == "E" )
			{
				var t1 = nominalValues2[ 0 ] + values2[ 0 ] - nominalValues1[ 0 ] - values1[ 0 ];
				var t2 = nominalValues2[ 1 ] + values2[ 1 ] - nominalValues1[ 1 ] - values1[ 1 ];
				var t3 = nominalValues2[ 2 ] + values2[ 2 ] - nominalValues1[ 2 ] - values1[ 2 ];

				var t4 = nominalValues2[ 0 ] - nominalValues1[ 0 ];
				var t5 = nominalValues2[ 1 ] - nominalValues1[ 1 ];
				var t6 = nominalValues2[ 2 ] - nominalValues1[ 2 ];

				if( t1.HasValue && t2.HasValue && t3.HasValue && t4.HasValue && t5.HasValue && t6.HasValue )
				{
					result = Math.Sqrt( t1.Value * t1.Value + t2.Value * t2.Value + t3.Value * t3.Value ) -
						Math.Sqrt( t4.Value * t4.Value + t5.Value * t5.Value + t6.Value * t6.Value );
				}
			}

			if( result.HasValue )
				result = Math.Round( result.Value, 3 );

			return result;
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Dist"/>.
		/// Expected arguments:
		/// * 2 characteristics
		/// * 1 direction literal (X,Y,Z,E)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Dist_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtDist, 2, false, DirectionsXyze );

				var direction = GetDirection( args );
				var characteristics = GetCharacteristics( args );

				switch( direction )
				{
					case "X":
					case "Y":
					case "Z":
						return GetDirectionDependencies( resolver, characteristics, direction );

					case "E":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXyz );
				}
			}
			catch
			{
				/**/
			}

			return Enumerable.Empty<MathDependencyInformation>();
		}

		/// <summary>
		/// Calculates the delta between two characteristics taking the nominal values into account.
		/// (Synonyms are "Bezugspunkt", "Deltaberechnung", "Versatz".)
		/// Expected arguments:
		/// * 2 characteristics
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		[OperationTemplate( PtRef + "($PATH0;$PATH1;$DIRECTION)", OperationTemplateTypes.PtRef )]
		public static double? Pt_Ref( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtRef, 2, false );

			var direction = GetDirection( args );
			var characteristics = GetCharacteristics( args );
			var p1Value = characteristics[ 0 ].GetValue( resolver, direction );
			var p2Value = characteristics[ 1 ].GetValue( resolver, direction );

			if( !p1Value.HasValue || !p2Value.HasValue )
				return null;

			var result = p1Value.Value - p2Value.Value;
			return Math.Round( result, 3 );
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Ref"/>.
		/// Expected arguments:
		/// * 2 characteristics
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Ref_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				var (characteristics, direction) = AnalyzeArguments( args, PtRef, 2, false );
				return GetDirectionDependencies( resolver, characteristics, direction );
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		/// <summary>
		/// Calculates the position of a characteristic.
		/// Expected arguments:
		/// * 1 characteristic
		/// * 1 direction literal (X,Y,Z,N,XY,XZ,YZ,XYZ)
		/// </summary>
		[OperationTemplate( PtPosSquare + "($PATH0;$DIRECTION)", OperationTemplateTypes.PtPosSquare )]
		public static double? Pt_Pos_Square( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			var (characteristics,direction) = AnalyzeArguments( args, PtPosSquare, 1, false, DirectionsAllAxisAndN );

			var ch = characteristics[ 0 ];

			if( direction.Length == 1 )
			{
				// 1-dimensional position
				switch( direction )
				{
					case "X":
					case "Y":
					case "Z":
					case "N":
						return ch.GetValue( resolver, direction );
					default:
						return null;
				}
			}

			var pX = ch.GetValue( resolver, "X" );
			var pY = ch.GetValue( resolver, "Y" );
			var pZ = ch.GetValue( resolver, "Z" );

			// multidimensional position

			var path = resolver.SourcePath?.ParentPath ?? ch.Path;

			var posX = GetAbsoluteDistanceFromToleranceMiddle( pX, GetTolerance( GetCharacteristicByDirectionExtendedName( path, "X" ), resolver ) );
			var posY = GetAbsoluteDistanceFromToleranceMiddle( pY, GetTolerance( GetCharacteristicByDirectionExtendedName( path, "Y" ), resolver ) );
			var posZ = GetAbsoluteDistanceFromToleranceMiddle( pZ, GetTolerance( GetCharacteristicByDirectionExtendedName( path, "Z" ), resolver ) );

			switch( direction )
			{
				case "XY" when posX.HasValue && posY.HasValue: return Math.Max( posX.Value, posY.Value );
				case "XZ" when posX.HasValue && posZ.HasValue: return Math.Max( posX.Value, posZ.Value );
				case "YZ" when posY.HasValue && posZ.HasValue: return Math.Max( posY.Value, posZ.Value );
				case "XYZ" when posX.HasValue && posY.HasValue && posZ.HasValue: return Math.Max( Math.Max( posX.Value, posY.Value ), posZ.Value );
				default: return null;
			}
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Pos_Square"/>.
		/// Expected arguments:
		/// * 1 characteristic
		/// * 1 direction literal (X,Y,Z,N,XY,XZ,YZ,XYZ)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Pos_Square_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtPosSquare, 1, false, DirectionsAllAxisAndN );

				var pos = ( (Literal)args.ElementAt( 1 ) ).Text.ToUpper();
				var ch = (Characteristic)args.ElementAt( 0 );

				switch( pos )
				{
					case "X":
					case "Y":
					case "Z":
					case "N":
						return GetDirectionDependencies( resolver, ch, pos );

					case "XY":
						return GetDirectionDependencies( resolver, ch, DirectionsXy );
					case "XZ":
						return GetDirectionDependencies( resolver, ch, DirectionsXz );
					case "YZ":
						return GetDirectionDependencies( resolver, ch, DirectionsYz );
					case "XYZ":
						return GetDirectionDependencies( resolver, ch, DirectionsXyz );
				}
			}
			catch
			{
				/**/
			}

			return Enumerable.Empty<MathDependencyInformation>();
		}

		/// <summary>
		/// Calculates the profile tolerance from a list of characteristics.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		[OperationTemplate( PtProfile + "($PATHS;$DIRECTION)", OperationTemplateTypes.PtProfile )]
		public static double? Pt_Profile( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtProfile, 1, true );

			var characteristics = GetCharacteristics( args );
			var direction = GetDirection( args );
			var toleratedValues = GetToleratedValues( characteristics, resolver, direction );
			if( toleratedValues.Length == 0 )
				return null;

			var values = toleratedValues.Select( v => GetDistanceFromToleranceMiddle( v.Value, v.Tolerance ) ).ToArray();
			return values.Max() - values.Min();
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Profile"/>.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Profile_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtProfile, 1, true );

				var direction = GetDirection( args );
				var characteristics = GetCharacteristics( args );
				return GetDirectionDependencies( resolver, characteristics, direction );
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		/// <summary>
		/// Calculates the worst value from a list of characteristics.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * 1 direction literal (X,Y,Z,N,P)
		/// </summary>
		[OperationTemplate( PtWorst + "($PATHS;$DIRECTION)", OperationTemplateTypes.PtWorst )]
		public static double? Pt_Worst( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtWorst, 1, true, DirectionsXyznp );

			var direction = GetDirection( args );
			var characteristics = GetCharacteristics( args );
			if( characteristics.Count == 0 )
				return null;

			var toleratedValues = Array.Empty<ToleratedValue>();

			if( direction == "X" || direction == "Y" || direction == "Z" || direction == "N" )
			{
				toleratedValues = GetToleratedValues( characteristics, resolver, direction );
			}
			else if( direction == "P" )
			{
				// The worst value should be calculated from the characteristics that must be documented (attribute "Dokumentationspflicht" set)
				var documentedCharacteristics = characteristics
					.SelectMany( ch => DirectionsXyzn.Select( d => ch.Path.GetCharacteristicByDirectionExtendedName( d ) ) )
					.Where( p => resolver.GetEntityAttributeValue<CatalogEntryDto>( p, WellKnownKeys.Characteristic.ControlItem )?.Key == 1 );

				toleratedValues = GetToleratedValues( documentedCharacteristics, resolver );
			}

			if( toleratedValues.Length == 0 )
				return null;

			double? result = null;
			double maxDist = 0;
			foreach( var tv in toleratedValues )
			{
				var dist = GetDistanceFromToleranceMiddle( tv.Value, tv.Tolerance );
				if( dist == null )
					continue;

				var absDist = Math.Abs( dist.Value );
				if( absDist > maxDist || ( absDist == maxDist && dist.Value > 0 ) )
				{
					maxDist = absDist;
					result = tv.Value;
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Worst"/>.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * 1 direction literal (X,Y,Z,N,P)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Worst_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtWorst, 1, true, DirectionsXyznp );
				return Pt_Worst_Dependent_Characteristics_All_Internal( args, resolver );
			}
			catch
			{
				//
			}

			return Enumerable.Empty<MathDependencyInformation>();
		}

		private static IEnumerable<MathDependencyInformation> Pt_Worst_Dependent_Characteristics_All_Internal( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			var direction = GetDirection( args );
			var characteristics = GetCharacteristics( args );

			switch( direction )
			{
				case "X":
				case "Y":
				case "Z":
				case "N":
					return GetDirectionDependencies( resolver, characteristics, direction );

				case "P":
				{
					var dependencies = GetDirectionDependencies( resolver, characteristics, DirectionsXyzn );
					return dependencies
						.Where( dep => resolver.GetEntityAttributeValue<CatalogEntryDto>( dep.Path, WellKnownKeys.Characteristic.ControlItem )?.Key == 1 )
						.ToArray();
				}
			}

			return Enumerable.Empty<MathDependencyInformation>();
		}

		/// <summary>
		/// Calculates the worst value from a list of characteristics evaluated using the tolerance definition of the target characteritic.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * 1 direction literal (X,Y,Z,N,P)
		/// </summary>
		[OperationTemplate( PtWorstTarget + "($PATHS;$DIRECTION)", OperationTemplateTypes.PtWorstTarget )]
		public static double? Pt_Worst_Target( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtWorstTarget, 1, true, DirectionsXyznp );

			if ( resolver.SourcePath is null)
				throw new ArgumentException( "Function '" + PtWorstTarget + "' requires path of the target characteristic!" );

			var direction = GetDirection( args );
			var characteristics = GetCharacteristics( args );
			if( characteristics.Count == 0 )
				return null;

			var toleranceMiddle = GetTolerance( resolver.SourcePath, resolver ).Middle;
			if( toleranceMiddle == null )
				return null;

			IEnumerable<PathInformation> paths;
			switch(direction)
			{
				case "X":
					case "Y":
					case "Z":
					case "N":
					paths = characteristics.Select( ch => GetDirectionChild( resolver, ch.Path, direction ) );
					break;
				case "P":
					paths = characteristics
						.SelectMany( ch => DirectionsXyzn.Select( d => ch.Path.GetCharacteristicByDirectionExtendedName( d ) ) )
						.Where( p => resolver.GetEntityAttributeValue<CatalogEntryDto>( p, WellKnownKeys.Characteristic.ControlItem )?.Key == 1 );
					break;
				default:
					return null;
			}

			double? result = null;
			double maxDist = 0;
			foreach( var path in paths )
			{
				if( path == null )
					return null;

				var value = resolver.GetMeasurementValue( path );
				var dist = value - toleranceMiddle;
				if( dist == null )
					return null;

				var absDist = Math.Abs( dist.Value );
				if( absDist > maxDist || ( absDist == maxDist && dist.Value > 0 ) )
				{
					maxDist = absDist;
					result = value;
				}
			}

			return result;
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Worst_Target"/>.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * 1 direction literal (X,Y,Z,N,P)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Worst_Target_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtWorstTarget, 1, true, DirectionsXyznp );
				return Pt_Worst_Dependent_Characteristics_All_Internal( args, resolver );
			}
			catch
			{
				//
			}

			return Enumerable.Empty<MathDependencyInformation>();
		}

		/// <summary>
		/// Calculates the shortest distance between a point and a line.
		/// Expected arguments:
		/// * 3 characteristics (point + 2 line points)
		/// * 1 direction literal (X,Y,Z,E)
		/// </summary>
		[OperationTemplate( PtDistPoint2Line + "($PATH0;$PATH1;$PATH2;$DIRECTION)", OperationTemplateTypes.PtDistPt2Pt )]
		public static double? Pt_Dist_Pt_2Pt( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtDistPoint2Line, 3, false, DirectionsXyze );

			var characteristics = GetCharacteristics( args );
			var ch = characteristics[ 0 ];
			var ch1 = characteristics[ 1 ];
			var ch2 = characteristics[ 2 ];

			var direction = GetDirection( args );
			const ushort nominalValueKey = WellKnownKeys.Characteristic.DesiredValue;

			var pNominal = new[] { ch.GetAttribute( resolver, nominalValueKey, "X" ), ch.GetAttribute( resolver, nominalValueKey, "Y" ), ch.GetAttribute( resolver, nominalValueKey, "Z" ) };
			var p1Nominal = new[] { ch1.GetAttribute( resolver, nominalValueKey, "X" ), ch1.GetAttribute( resolver, nominalValueKey, "Y" ), ch1.GetAttribute( resolver, nominalValueKey, "Z" ) };
			var p2Nominal = new[] { ch2.GetAttribute( resolver, nominalValueKey, "X" ), ch2.GetAttribute( resolver, nominalValueKey, "Y" ), ch2.GetAttribute( resolver, nominalValueKey, "Z" ) };

			var pCurrent = new[] { pNominal[ 0 ] + ch.GetValue( resolver, "X" ), pNominal[ 1 ] + ch.GetValue( resolver, "Y" ), pNominal[ 2 ] + ch.GetValue( resolver, "Z" ) };
			var p1Current = new[] { p1Nominal[ 0 ] + ch1.GetValue( resolver, "X" ), p1Nominal[ 1 ] + ch1.GetValue( resolver, "Y" ), p1Nominal[ 2 ] + ch1.GetValue( resolver, "Z" ) };
			var p2Current = new[] { p2Nominal[ 0 ] + ch2.GetValue( resolver, "X" ), p2Nominal[ 1 ] + ch2.GetValue( resolver, "Y" ), p2Nominal[ 2 ] + ch2.GetValue( resolver, "Z" ) };

			if( pNominal.Any( v => !v.HasValue )
				|| p1Nominal.Any( v => !v.HasValue )
				|| p2Nominal.Any( v => !v.HasValue ) )
				return null;

			if( pCurrent.Any( v => !v.HasValue )
				|| p1Current.Any( v => !v.HasValue )
				|| p2Current.Any( v => !v.HasValue ) )
				return null;

			var current = Calc_Pt_Dist_Pt_2Pt( direction,
				pCurrent.Select( v => v.Value ).ToArray(),
				p1Current.Select( v => v.Value ).ToArray(),
				p2Current.Select( v => v.Value ).ToArray() );
			var nominal = Calc_Pt_Dist_Pt_2Pt( direction,
				pNominal.Select( v => v.Value ).ToArray(),
				p1Nominal.Select( v => v.Value ).ToArray(),
				p2Nominal.Select( v => v.Value ).ToArray() );

			var result = current - nominal;
			if( result.HasValue )
				result = Math.Round( result.Value, 3 );

			return result;
		}

		/// <summary>
		/// Calculates the perpendicular distance between a point and a line (PT_DIST_PT_2PT).
		/// </summary>
		/// <param name="direction">Direction coordinate</param>
		/// <param name="valuesP">Values for the point [XYZ]</param>
		/// <param name="valuesP1">Values for the first line point [XYZ]</param>
		/// <param name="valuesP2">Values for the second line point [XYZ]</param>
		/// <returns>The calculated distance or <code>null</code> if the distance could not be calculated.</returns>
		public static double? Calc_Pt_Dist_Pt_2Pt( string direction, double[] valuesP, double[] valuesP1, double[] valuesP2 )
		{
			ValidateForThreePointVector( valuesP, nameof( valuesP ) );
			ValidateForThreePointVector( valuesP1, nameof( valuesP1 ) );
			ValidateForThreePointVector( valuesP2, nameof( valuesP2 ) );

			var pl = CalculateLinePerpendicularPoint( valuesP, valuesP1, valuesP2 );
			if( pl == null )
				return null;

			double? result = null;
			if( direction == "X" || direction == "Y" || direction == "Z" )
			{
				var i = Array.IndexOf( DirectionsXyz, direction );
				result = Math.Abs( valuesP[ i ] - pl[ i ] );
			}
			else if( direction == "E" )
			{
				result = Math.Sqrt( ( ( valuesP[ 0 ] - pl[ 0 ] ) * ( valuesP[ 0 ] - pl[ 0 ] ) ) +
					( ( valuesP[ 1 ] - pl[ 1 ] ) * ( valuesP[ 1 ] - pl[ 1 ] ) ) +
					( ( valuesP[ 2 ] - pl[ 2 ] ) * ( valuesP[ 2 ] - pl[ 2 ] ) ) );
			}

			if( !result.HasValue || double.IsNaN( result.Value ) )
				return null;

			return Math.Round( result.Value, 3 );
		}

		private static void ValidateForThreePointVector<T>( T[] values, string parameterName )
		{
			if( values == null || values.Length != 3 )
				throw new ArgumentException( @"Values must have a length of 3", parameterName );
		}

		/// <summary>
		/// Calculates the intersection point of a perpendicular from a point to a line.
		/// </summary>
		/// <param name="valuesP">Values for the point [XYZ]</param>
		/// <param name="valuesP1">Values for the first line point [XYZ]</param>
		/// <param name="valuesP2">Values for the second line point [XYZ]</param>
		/// <returns>The calculated point [XYZ] or <code>null</code> if the point could not be calculated.</returns>
		private static double[] CalculateLinePerpendicularPoint( double[] valuesP, double[] valuesP1, double[] valuesP2 )
		{
			// Direction X,Y,Z
			var nx = valuesP2[ 0 ] - valuesP1[ 0 ];
			var ny = valuesP2[ 1 ] - valuesP1[ 1 ];
			var nz = valuesP2[ 2 ] - valuesP1[ 2 ];

			// Absolute value of the perpendicular
			var nAbs = Math.Sqrt( nx * nx + ny * ny + nz * nz );

			// Normalized perpendicular in X,Y,Z
			var lu = nx / nAbs;
			var lv = ny / nAbs;
			var lw = nz / nAbs;

			var lambda = ( valuesP[ 0 ] - valuesP1[ 0 ] ) * lu + ( valuesP[ 1 ] - valuesP1[ 1 ] ) * lv + ( valuesP[ 2 ] - valuesP1[ 2 ] ) * lw;
			var result = new[] { valuesP1[ 0 ] + lambda * lu, valuesP1[ 1 ] + lambda * lv, valuesP1[ 2 ] + lambda * lw };

			return result.Any( double.IsNaN ) ? null : result;
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Dist_Pt_2Pt"/>.
		/// Expected arguments:
		/// * 3 characteristics (point + 2 line points)
		/// * 1 direction literal (X,Y,Z,E)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Dist_Pt_2Pt_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtDistPoint2Line, 3, false, DirectionsXyze );

				var characteristics = GetCharacteristics( args );
				return GetDirectionDependencies( resolver, characteristics, DirectionsXyz );
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		/// <summary>
		/// Calculates the shortest distance between a point and a plane.
		/// Expected arguments:
		/// * 4 characteristics (point + 3 plane points)
		/// * 1 direction literal (X,Y,Z,E)
		/// </summary>
		[OperationTemplate( PtDistPoint2Plane + "($PATH0;$PATH1;$PATH2;$PATH3;$DIRECTION)", OperationTemplateTypes.PtDistPt3Pt )]
		public static double? Pt_Dist_Pt_3Pt( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			AnalyzeArguments( args, PtDistPoint2Plane, 4, false, DirectionsXyze );

			var characteristics = GetCharacteristics( args );
			var ch = characteristics[ 0 ];
			var ch1 = characteristics[ 1 ];
			var ch2 = characteristics[ 2 ];
			var ch3 = characteristics[ 3 ];

			const ushort nominalValueKey = WellKnownKeys.Characteristic.DesiredValue;
			var direction = GetDirection( args );

			var pNominal = new[] { ch.GetAttribute( resolver, nominalValueKey, "X" ), ch.GetAttribute( resolver, nominalValueKey, "Y" ), ch.GetAttribute( resolver, nominalValueKey, "Z" ) };
			var p1Nominal = new[] { ch1.GetAttribute( resolver, nominalValueKey, "X" ), ch1.GetAttribute( resolver, nominalValueKey, "Y" ), ch1.GetAttribute( resolver, nominalValueKey, "Z" ) };
			var p2Nominal = new[] { ch2.GetAttribute( resolver, nominalValueKey, "X" ), ch2.GetAttribute( resolver, nominalValueKey, "Y" ), ch2.GetAttribute( resolver, nominalValueKey, "Z" ) };
			var p3Nominal = new[] { ch3.GetAttribute( resolver, nominalValueKey, "X" ), ch3.GetAttribute( resolver, nominalValueKey, "Y" ), ch3.GetAttribute( resolver, nominalValueKey, "Z" ) };

			var pCurrent = new[] { pNominal[ 0 ] + ch.GetValue( resolver, "X" ), pNominal[ 1 ] + ch.GetValue( resolver, "Y" ), pNominal[ 2 ] + ch.GetValue( resolver, "Z" ) };
			var p1Current = new[] { p1Nominal[ 0 ] + ch1.GetValue( resolver, "X" ), p1Nominal[ 1 ] + ch1.GetValue( resolver, "Y" ), p1Nominal[ 2 ] + ch1.GetValue( resolver, "Z" ) };
			var p2Current = new[] { p2Nominal[ 0 ] + ch2.GetValue( resolver, "X" ), p2Nominal[ 1 ] + ch2.GetValue( resolver, "Y" ), p2Nominal[ 2 ] + ch2.GetValue( resolver, "Z" ) };
			var p3Current = new[] { p3Nominal[ 0 ] + ch3.GetValue( resolver, "X" ), p3Nominal[ 1 ] + ch3.GetValue( resolver, "Y" ), p3Nominal[ 2 ] + ch3.GetValue( resolver, "Z" ) };

			if( pNominal.Any( v => !v.HasValue )
				|| p1Nominal.Any( v => !v.HasValue )
				|| p2Nominal.Any( v => !v.HasValue )
				|| p3Nominal.Any( v => !v.HasValue ) )
				return null;

			if( pCurrent.Any( v => !v.HasValue )
				|| p1Current.Any( v => !v.HasValue )
				|| p2Current.Any( v => !v.HasValue )
				|| p3Current.Any( v => !v.HasValue ) )
				return null;

			var current = Calc_Pt_Dist_Pt_3Pt( direction,
				pCurrent.Select( v => v.Value ).ToArray(),
				p1Current.Select( v => v.Value ).ToArray(),
				p2Current.Select( v => v.Value ).ToArray(),
				p3Current.Select( v => v.Value ).ToArray() );
			var nominal = Calc_Pt_Dist_Pt_3Pt( direction,
				pNominal.Select( v => v.Value ).ToArray(),
				p1Nominal.Select( v => v.Value ).ToArray(),
				p2Nominal.Select( v => v.Value ).ToArray(),
				p3Nominal.Select( v => v.Value ).ToArray() );

			var result = current - nominal;
			if( !result.HasValue )
				return null;

			return Math.Round( result.Value, 3 );
		}

		/// <summary>
		/// Calculates the perpendicular distance between a point and a plane (PT_DIST_PT_3PT).
		/// </summary>
		/// <param name="direction">Direction coordinate</param>
		/// <param name="valuesP">Values for the point [XYZ]</param>
		/// <param name="valuesP1">Values for the first plane point [XYZ]</param>
		/// <param name="valuesP2">Values for the second plane point [XYZ]</param>
		/// <param name="valuesP3">Values for the third plane point [XYZ]</param>
		/// <returns>The calculated distance or <code>null</code> if the distance could not be calculated.</returns>
		public static double? Calc_Pt_Dist_Pt_3Pt( string direction, double[] valuesP, double[] valuesP1, double[] valuesP2, double[] valuesP3 )
		{
			ValidateForThreePointVector( valuesP, nameof( valuesP ) );
			ValidateForThreePointVector( valuesP1, nameof( valuesP1 ) );
			ValidateForThreePointVector( valuesP2, nameof( valuesP2 ) );
			ValidateForThreePointVector( valuesP3, nameof( valuesP3 ) );

			double? result = null;

			// Perpendicular in X,Y,Z
			var nx = ( ( valuesP2[ 1 ] - valuesP1[ 1 ] ) * ( valuesP3[ 2 ] - valuesP1[ 2 ] ) ) - ( ( valuesP2[ 2 ] - valuesP1[ 2 ] ) * ( valuesP3[ 1 ] - valuesP1[ 1 ] ) );
			var ny = ( ( valuesP2[ 2 ] - valuesP1[ 2 ] ) * ( valuesP3[ 0 ] - valuesP1[ 0 ] ) ) - ( ( valuesP2[ 0 ] - valuesP1[ 0 ] ) * ( valuesP3[ 2 ] - valuesP1[ 2 ] ) );
			var nz = ( ( valuesP2[ 0 ] - valuesP1[ 0 ] ) * ( valuesP3[ 1 ] - valuesP1[ 1 ] ) ) - ( ( valuesP2[ 1 ] - valuesP1[ 1 ] ) * ( valuesP3[ 0 ] - valuesP1[ 0 ] ) );

			// Absolute value of the perpendicular
			var nAbs = Math.Sqrt( nx * nx + ny * ny + nz * nz );

			// Normalized perpendicular in X,Y,Z
			var eu = nx / nAbs;
			var ev = ny / nAbs;
			var ew = nz / nAbs;

			switch( direction )
			{
				case "X":
					result = Math.Abs( valuesP[ 0 ] - ( valuesP1[ 0 ] + ev / eu * ( valuesP1[ 1 ] - valuesP[ 1 ] ) + ew / eu * ( valuesP1[ 2 ] - valuesP[ 2 ] ) ) );
					break;
				case "Y":
					result = Math.Abs( valuesP[ 1 ] - ( valuesP1[ 1 ] + eu / ev * ( valuesP1[ 0 ] - valuesP[ 0 ] ) + ew / ev * ( valuesP1[ 2 ] - valuesP[ 2 ] ) ) );
					break;
				case "Z":
					result = Math.Abs( valuesP[ 2 ] - ( valuesP1[ 2 ] + eu / ew * ( valuesP1[ 0 ] - valuesP[ 0 ] ) + ev / ew * ( valuesP1[ 1 ] - valuesP[ 1 ] ) ) );
					break;
				case "E":
					result = Math.Abs( ( valuesP[ 0 ] - valuesP1[ 0 ] ) * eu + ( valuesP[ 1 ] - valuesP1[ 1 ] ) * ev + ( valuesP[ 2 ] - valuesP1[ 2 ] ) * ew );
					break;
			}

			if( !result.HasValue || double.IsNaN( result.Value ) )
				return null;

			return Math.Round( result.Value, 3 );
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Dist_Pt_3Pt"/>.
		/// Expected arguments:
		/// * 4 characteristics (point + 3 plane points)
		/// * 1 direction literal (X,Y,Z,E)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Dist_Pt_3Pt_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				AnalyzeArguments( args, PtDistPoint2Plane, 4, false, DirectionsXyze );

				var directions = new[] { "X", "Y", "Z" };
				var characteristics = GetCharacteristics( args );
				return GetDirectionDependencies( resolver, characteristics, directions );
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		private static IReadOnlyList<Characteristic> GetCharacteristics( [NotNull] IEnumerable<MathElement> args )
		{
			// The first block of arguments define characteristics, characteristics in a later block are ignored/invalid
			return args.TakeWhile( arg => arg is Characteristic ).Cast<Characteristic>().ToArray();
		}

		private static string GetDirection( [NotNull] IEnumerable<MathElement> args )
		{
			// The second block of arguments defines a direction in form of a literal
			return ( args.SkipWhile( arg => arg is Characteristic ).FirstOrDefault() as Literal )?.Text.ToUpper();
		}

		private static bool AllValuesRequired( [NotNull] IEnumerable<MathElement> args )
		{
			// The last block of arguments can define the optional boolean flag to calculate a result only if all characteristics have a value
			return ( args.Last() as Literal )?.Text.Equals( "true", StringComparison.InvariantCultureIgnoreCase ) ?? false;
		}

		/// <summary>
		/// Provides the path of the direction characteristic usually used by Audi/Daimler.
		/// </summary>
		private static PathInformation GetCharacteristicByDirectionExtendedName( this PathInformation parent, string direction )
		{
			return PathInformation.Combine( parent, PathElement.Char( parent.Name + "." + direction ) );
		}

		/// <summary>
		/// Provides the path of the direction characteristic usually used by Calypso.
		/// </summary>
		private static PathInformation GetCharacteristicByDirectionShortName( this PathInformation parent, string direction )
		{
			return PathInformation.Combine( parent, PathElement.Char( direction ) );
		}

		private static double? GetValue( this Characteristic characteristic, [NotNull] ICharacteristicValueResolver resolver, string direction )
		{
			return resolver.GetMeasurementValue( GetCharacteristicByDirectionExtendedName( characteristic.Path, direction ) )
				?? resolver.GetMeasurementValue( GetCharacteristicByDirectionShortName( characteristic.Path, direction ) );
		}

		private static double? GetAttribute( this Characteristic characteristic, [NotNull] ICharacteristicInfoResolver resolver, ushort attribute, string direction )
		{
			return characteristic.Path.GetAttribute( resolver, attribute, direction );
		}

		private static double? GetAttribute( this PathInformation path, [NotNull] ICharacteristicInfoResolver resolver, ushort attribute, string direction )
		{
			return resolver.GetEntityAttributeValue<double?>( GetCharacteristicByDirectionExtendedName( path, direction ), attribute )
				?? resolver.GetEntityAttributeValue<double?>( GetCharacteristicByDirectionShortName( path, direction ), attribute );
		}

		private static double? GetAbsoluteDistanceFromToleranceMiddle( double? value, Tolerance tolerance )
		{
			var distanceFromToleranceMiddle = GetDistanceFromToleranceMiddle( value, tolerance );
			if( distanceFromToleranceMiddle == null )
				return null;

			return Math.Abs( distanceFromToleranceMiddle.Value );
		}

		private static double? GetDistanceFromToleranceMiddle( double? value, Tolerance tolerance )
		{
			if( !value.HasValue || tolerance.IsPartiallyUnbounded )
				return null;

			return value.Value - tolerance.Middle;
		}

		private static Tolerance GetTolerance( PathInformation path, [NotNull] ICharacteristicInfoResolver resolver )
		{
			var attributeHandler = new ToleranceProvider.AttributeHandler( key => resolver.GetEntityAttributeValue( path, key ) );
			return ToleranceProvider.GetTolerance( attributeHandler );
		}

		[NotNull]
		private static ToleratedValue[] GetToleratedValues( IEnumerable<Characteristic> characteristics, [NotNull] ICharacteristicValueResolver resolver, string direction )
		{
			var paths = characteristics.Select( ch => GetDirectionChild( resolver, ch.Path, direction ) );
			return GetToleratedValues( paths, resolver );
		}

		/// <summary>
		/// Gets the measured values and the tolerances for the given characteristics.
		/// Returns <code>null</code> if the measured value or a tolerance value for a characteristic could not be found.
		/// </summary>
		[NotNull]
		private static ToleratedValue[] GetToleratedValues( IEnumerable<PathInformation> paths, [NotNull] ICharacteristicValueResolver resolver )
		{
			var valueList = new List<ToleratedValue>();
			foreach( var path in paths )
			{
				if( path == null )
					return Array.Empty<ToleratedValue>();

				var value = resolver.GetMeasurementValue( path );
				if( !value.HasValue )
					return Array.Empty<ToleratedValue>();

				var tolerance = GetTolerance( path, resolver );
				if( tolerance.IsPartiallyUnbounded )
					return Array.Empty<ToleratedValue>();

				valueList.Add( new ToleratedValue( value.Value, tolerance ) );
			}

			return valueList.ToArray();
		}

		#endregion

		#region struct ToleratedValue

		private readonly struct ToleratedValue
		{
			#region constructors

			public ToleratedValue( double value, Tolerance tolerance )
			{
				Value = value;
				Tolerance = tolerance;
			}

			#endregion

			#region properties

			public double Value { get; }
			public Tolerance Tolerance { get; }

			#endregion
		}

		#endregion
	}
}
