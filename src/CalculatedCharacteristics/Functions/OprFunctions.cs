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
	using Zeiss.PiWeb.Api.Definitions;
	using Zeiss.PiWeb.Api.Rest.Dtos.Data;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	/// <summary>
	/// Functional dimensions from the car body sector for use in the <see cref="MathInterpreter"/>.
	/// </summary>
	public static class OprFunctions
	{
		#region methods

		/// <summary>
		/// Calculates the minimum from a list of characteristics.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// * [optional] literal "true" to calculate a result only if all characteristics have a value
		/// </summary>
		[OperationTemplate( "PT_MIN($PATHS;$DIRECTION;$CHECK)", OperationTemplateTypes.PtMin )]
		public static double? Pt_Min( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_MIN", 1, true );

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
				CheckArguments( args, "PT_MIN", 1, true );

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
		[OperationTemplate( "PT_MAX($PATHS;$DIRECTION;$CHECK)", OperationTemplateTypes.PtMax )]
		public static double? Pt_Max( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_MAX", 1, true );

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
				CheckArguments( args, "PT_MAX", 1, true );

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
			string[] directions )
		{
			var result = new List<MathDependencyInformation>( directions.Length );
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
		private static PathInformationDto GetDirectionChild( [NotNull] ICharacteristicInfoResolver resolver, PathInformationDto characteristic, string direction )
		{
			var extendedDirectionName = GetCharacteristicByDirectionExtendedName( characteristic, direction ).Name;
			var shortDirectionName = GetCharacteristicByDirectionShortName( characteristic, direction ).Name;

			PathInformationDto foundDirectionPath = null;

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
		[OperationTemplate( "PT_SYM($PATHS;$DIRECTION;$CHECK)", OperationTemplateTypes.PtSym )]
		public static double? Pt_Sym( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_SYM", 1, true );

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
				CheckArguments( args, "PT_SYM", 1, true );

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
		/// Calculates the distance between two characteristics.
		/// Expected arguments:
		/// * 2 characteristics
		/// * 1 direction literal (X,Y,Z,E)
		/// </summary>
		[OperationTemplate( "PT_DIST($PATH0;$PATH1;$DIRECTION)", OperationTemplateTypes.PtDist )]
		public static double? Pt_Dist( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_DIST", 2, false, "[X,Y,Z,E]" );

			var ch1 = GetCharacteristics( args )[ 0 ];
			var ch2 = GetCharacteristics( args )[ 1 ];
			var direction = GetDirection( args );

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

			var directions = new[] { "X", "Y", "Z", "E" };
			double? result = null;
			if( direction == "X" || direction == "Y" || direction == "Z" )
			{
				var i = Array.IndexOf( directions, direction );
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
				CheckArguments( args, "PT_DIST", 2, false, "[X,Y,Z,E]" );

				var direction = GetDirection( args );
				var characteristics = GetCharacteristics( args );

				switch( direction )
				{
					case "X":
					case "Y":
					case "Z":
						return GetDirectionDependencies( resolver, characteristics, direction );

					case "E":
					{
						var directions = new[] { "X", "Y", "Z" };
						return GetDirectionDependencies( resolver, characteristics, directions );
					}
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
		[OperationTemplate( "PT_REF($PATH0;$PATH1;$DIRECTION)", OperationTemplateTypes.PtRef )]
		public static double? Pt_Ref( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_REF", 2, false );

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
				CheckArguments( args, "PT_REF", 2, false );

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
		/// Calculates the position of a characteristic.
		/// Expected arguments:
		/// * 1 characteristic
		/// * 1 direction literal (X,Y,Z,N,XY,XZ,YZ,XYZ)
		/// </summary>
		[OperationTemplate( "PT_POS_SQUARE($PATH0;$DIRECTION)", OperationTemplateTypes.PtPosSquare )]
		public static double? Pt_Pos_Square( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_POS_SQUARE", 1, false, "[X,Y,Z,N,XY,XZ,YZ,XYZ]" );

			var direction = GetDirection( args );
			var ch = GetCharacteristics( args )[ 0 ];


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
				case "XYZ" when posX.HasValue && posY.HasValue && posZ.HasValue: return new[] { posX.Value, posY.Value, posZ.Value }.Max();
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
				CheckArguments( args, "PT_POS_SQUARE", 1, false, "[X,Y,Z,N,XY,XZ,YZ,XYZ]" );

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
						return GetDirectionDependencies( resolver, ch, new[] { "X", "Y" } );
					case "XZ":
						return GetDirectionDependencies( resolver, ch, new[] { "X", "Z" } );
					case "YZ":
						return GetDirectionDependencies( resolver, ch, new[] { "Y", "Z" } );
					case "XYZ":
						return GetDirectionDependencies( resolver, ch, new[] { "X", "Y", "Z" } );
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
		[OperationTemplate( "PT_PROFILE($PATHS;$DIRECTION)", OperationTemplateTypes.PtProfile )]
		public static double? Pt_Profile( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_PROFILE", 1, true );

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
				CheckArguments( args, "PT_PROFILE", 1, true );

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
		[OperationTemplate( "PT_WORST($PATHS;$DIRECTION)", OperationTemplateTypes.PtWorst )]
		public static double? Pt_Worst( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_WORST", 1, true, "[X,Y,Z,N,P]" );

			var direction = GetDirection( args );
			var characteristics = GetCharacteristics( args );
			if( characteristics.Length == 0 )
				return null;

			var toleratedValues = Array.Empty<ToleratedValue>();

			if( direction == "X" || direction == "Y" || direction == "Z" || direction == "N" )
			{
				toleratedValues = GetToleratedValues( characteristics, resolver, direction );
			}
			else if( direction == "P" )
			{
				// The worst value should be calculated from the characteristics that must be documented (attribute "Dokumentationspflicht" set)
				var directions = new[] { "X", "Y", "Z", "N" };
				var documentedCharacteristics = characteristics
					.SelectMany( ch => directions.Select( d => ch.Path.GetCharacteristicByDirectionExtendedName( d ) ) )
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
				CheckArguments( args, "PT_WORST", 1, true, "[X,Y,Z,N,P]" );

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
						var directions = new[] { "X", "Y", "Z", "N" };
						var dependencies = GetDirectionDependencies( resolver, characteristics, directions );
						return dependencies
							.Where( dep => resolver.GetEntityAttributeValue<CatalogEntryDto>( dep.Path, WellKnownKeys.Characteristic.ControlItem )?.Key == 1 )
							.ToArray();
					}
				}
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
		[OperationTemplate( "PT_DIST_PT_2PT($PATH0;$PATH1;$PATH2;$DIRECTION)", OperationTemplateTypes.PtDistPt2Pt )]
		public static double? Pt_Dist_Pt_2Pt( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_DIST_PT_2PT", 3, false, "[X,Y,Z,E]" );

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

			var directions = new[] { "X", "Y", "Z" };
			double? result = null;
			if( direction == "X" || direction == "Y" || direction == "Z" )
			{
				var i = Array.IndexOf( directions, direction );
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
				CheckArguments( args, "PT_DIST_PT_2PT", 3, false, "[X,Y,Z,E]" );

				var directions = new[] { "X", "Y", "Z" };
				var characteristics = GetCharacteristics( args );
				return GetDirectionDependencies( resolver, characteristics, directions );
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
		[OperationTemplate( "PT_DIST_PT_3PT($PATH0;$PATH1;$PATH2;$PATH3;$DIRECTION)", OperationTemplateTypes.PtDistPt3Pt )]
		public static double? Pt_Dist_Pt_3Pt( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			CheckArguments( args, "PT_DIST_PT_3PT", 4, false, "[X,Y,Z,E]" );

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
				CheckArguments( args, "PT_DIST_PT_3PT", 4, false, "[X,Y,Z,E]" );

				var directions = new[] { "X", "Y", "Z" };
				var characteristics = GetCharacteristics( args );
				return GetDirectionDependencies( resolver, characteristics, directions );
			}
			catch
			{
				return Enumerable.Empty<MathDependencyInformation>();
			}
		}

		private static void CheckArguments( [NotNull] IReadOnlyCollection<MathElement> args, string name, int requiredCharacteristicsCount, bool allowMultipleCharacteristics, string literalPattern = null )
		{
			var characteristics = GetCharacteristics( args );

			if( allowMultipleCharacteristics )
			{
				if( args.Count < requiredCharacteristicsCount + 1 )
					throw new ArgumentException( $"Function '{name}' requires at least {requiredCharacteristicsCount + 1} parameters!" );

				if( characteristics.Length < requiredCharacteristicsCount )
					throw new ArgumentException( $"Function '{name}' requires at least {requiredCharacteristicsCount} characteristics as its first parameter!" );
			}
			else
			{
				if( args.Count != requiredCharacteristicsCount + 1 )
					throw new ArgumentException( $"Function 'name' requires {requiredCharacteristicsCount + 1} parameters!" );

				if( characteristics.Length != requiredCharacteristicsCount )
					throw new ArgumentException( $"Function '{name}' requires at least {requiredCharacteristicsCount} characteristics as its first parameter!" );
			}

			if( characteristics.Any( c => c.AttributeKey.HasValue ) )
				throw new ArgumentException( $"Function '{name}' does not support using characteristic attributes!" );

			var direction = GetDirection( args );
			if( string.IsNullOrEmpty( direction ) )
				throw new ArgumentException( $"Function '{name}' requires a literal as parameter after the characteristics parameters!" );

			if( literalPattern != null )
			{
				var supportedDirections = literalPattern.Replace( '[', ' ' ).Replace( ']', ' ' ).Trim().Split( ',' );
				if( !supportedDirections.Contains( direction ) )
					throw new ArgumentException( $"Function '{name}' requires a literal {literalPattern} as its last parameter!" );
			}
		}

		private static Characteristic[] GetCharacteristics( [NotNull] IEnumerable<MathElement> args )
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
			return ( args.Last() as Literal )?.Text.ToLower() == "true";
		}

		/// <summary>
		/// Provides the path of the direction characteristic usually used by Audi/Daimler.
		/// </summary>
		private static PathInformationDto GetCharacteristicByDirectionExtendedName( this PathInformationDto parent, string direction )
		{
			return PathInformationDto.Combine( parent, PathElementDto.Char( parent.Name + "." + direction ) );
		}

		/// <summary>
		/// Provides the path of the direction characteristic usually used by Calypso.
		/// </summary>
		private static PathInformationDto GetCharacteristicByDirectionShortName( this PathInformationDto parent, string direction )
		{
			return PathInformationDto.Combine( parent, PathElementDto.Char( direction ) );
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

		private static double? GetAttribute( this PathInformationDto path, [NotNull] ICharacteristicInfoResolver resolver, ushort attribute, string direction )
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

		private static Tolerance GetTolerance( PathInformationDto path, [NotNull] ICharacteristicInfoResolver resolver )
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
		private static ToleratedValue[] GetToleratedValues( IEnumerable<PathInformationDto> paths, [NotNull] ICharacteristicValueResolver resolver )
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
