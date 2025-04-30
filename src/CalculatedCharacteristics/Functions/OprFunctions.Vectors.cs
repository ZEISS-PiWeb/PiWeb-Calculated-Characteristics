#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2022                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;

	#endregion

	public static partial class OprFunctions
	{
		#region constants

		/// <summary>
		/// Name of function <see cref="Pt_Len"/>.
		/// </summary>
		public const string PtLen = "PT_LEN";

		/// <summary>
		/// Name of function <see cref="Pt_Worst_Axis"/>.
		/// </summary>
		public const string PtWorstAxis = "PT_WORST_AXIS";

		#endregion

		#region members

		private static readonly string[] DirectionsAllAxis = ["X", "Y", "Z", "XY", "YX", "XZ", "ZX", "YZ", "ZY", "XYZ", "XZY", "YXZ", "YZX", "ZXY", "ZYX"];

		#endregion

		#region methods

		/// <summary>
		/// Calculates length of an vector.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		[OperationTemplate( PtLen + "($PATH0;$AXES)", OperationTemplateTypes.PtPosLength )]
		[OperationTemplate( PtLen + "($PATH0;$PATH1;$AXES)", OperationTemplateTypes.PtVectorLength )]
		public static double? Pt_Len( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			var (characteristics, direction) = AnalyzeArguments( args, PtLen, 1, 2, DirectionsAllAxis );

			return direction.Length switch
			{
				1 => GetSingleDimensionLength( characteristics, direction, resolver ),
				2 => GetTwoDimensionLength( characteristics, direction, resolver ),
				3 => GetThreeDimensionLength( characteristics, resolver ),
				_ => null
			};
		}

		private static double? GetSingleDimensionLength( IReadOnlyList<Characteristic> characteristics, string direction, ICharacteristicValueResolver resolver )
		{
			double? result;

			if( characteristics.Count == 1 )
				result = GetValue( characteristics[ 0 ], resolver, direction );
			else
				result = GetValue( characteristics[ 1 ], resolver, direction ) - GetValue( characteristics[ 0 ], resolver, direction );

			return result.HasValue ? Math.Abs( result.Value ) : null;
		}

		private static double? GetTwoDimensionLength( IReadOnlyList<Characteristic> characteristics, string direction, ICharacteristicValueResolver resolver )
		{
			double? d1;
			double? d2;

			var dir1 = new string( direction.AsSpan( 0, 1 ) );
			var dir2 = new string( direction.AsSpan( 1, 1 ) );

			if( characteristics.Count == 1 )
			{
				d1 = GetValue( characteristics[ 0 ], resolver, dir1 );
				d2 = GetValue( characteristics[ 0 ], resolver, dir2 );
			}
			else
			{
				d1 = GetValue( characteristics[ 1 ], resolver, dir1 ) - GetValue( characteristics[ 0 ], resolver, dir1 );
				d2 = GetValue( characteristics[ 1 ], resolver, dir2 ) - GetValue( characteristics[ 0 ], resolver, dir2 );
			}

			if( !d1.HasValue || !d2.HasValue )
				return null;

			return Math.Sqrt( d1.Value * d1.Value + d2.Value * d2.Value );
		}

		private static double? GetThreeDimensionLength( IReadOnlyList<Characteristic> characteristics, ICharacteristicValueResolver resolver )
		{
			double? d1;
			double? d2;
			double? d3;

			if( characteristics.Count == 1 )
			{
				d1 = GetValue( characteristics[ 0 ], resolver, "X" );
				d2 = GetValue( characteristics[ 0 ], resolver, "Y" );
				d3 = GetValue( characteristics[ 0 ], resolver, "Z" );
			}
			else
			{
				d1 = GetValue( characteristics[ 1 ], resolver, "X" ) - GetValue( characteristics[ 0 ], resolver, "X" );
				d2 = GetValue( characteristics[ 1 ], resolver, "Y" ) - GetValue( characteristics[ 0 ], resolver, "Y" );
				d3 = GetValue( characteristics[ 1 ], resolver, "Z" ) - GetValue( characteristics[ 0 ], resolver, "Z" );
			}

			if( !d1.HasValue || !d2.HasValue || !d3.HasValue )
				return null;

			return Math.Sqrt( d1.Value * d1.Value + d2.Value * d2.Value + d3.Value * d3.Value );
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Len"/>.
		/// Expected arguments:
		/// * at least 1 or 2 characteristics
		/// * any direction literal (e.g. X,Y,Z,XY,XZ,YZ,XYZ)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Len_DependentCharacteristics( IReadOnlyCollection<MathElement> args, ICharacteristicInfoResolver resolver )
		{
			try
			{
				var (characteristics, direction) = AnalyzeArguments( args, PtLen, 1, 2 );
				switch( direction )
				{
					case "X":
					case "Y":
					case "Z":
						return GetDirectionDependencies( resolver, characteristics, direction );
					case "XY":
					case "YX":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXy );
					case "XZ":
					case "ZX":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXz );
					case "YZ":
					case "ZY":
						return GetDirectionDependencies( resolver, characteristics, DirectionsYz );
					case "XYZ":
					case "XZY":
					case "YXZ":
					case "ZXY":
					case "YZX":
					case "ZYX":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXyz );
				}
			}
			catch
			{
				// do nothing
			}

			return [];
		}

		/// <summary>
		/// Calculates the worst value from a list of characteristics.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * 1 direction literal (X,Y,Z,N,P)
		/// </summary>
		[OperationTemplate( PtWorstAxis + "($PATH0;$AXIS)", OperationTemplateTypes.PtWorstAxis )]
		public static double? Pt_Worst_Axis( IReadOnlyCollection<MathElement> args, ICharacteristicValueResolver resolver )
		{
			var (characteristics, direction) = AnalyzeArguments( args, PtWorstAxis, 1, false, DirectionsAllAxis );

			var characteristic = characteristics[ 0 ].Path;
			var axisCharacteristicsQuery = direction
				.Select( dir => GetDirectionChild( resolver, characteristic, dir.ToString() ) )
				.Where( p => p is not null );

			var toleratedValues = GetToleratedValues( axisCharacteristicsQuery, resolver );
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
		/// Gets the paths that are referenced by <see cref="Pt_Worst_Axis"/>.
		/// Expected arguments:
		/// * only 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,XY,XZ,YZ,XYZ)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Worst_Axis_DependentCharacteristics( IReadOnlyCollection<MathElement> args, ICharacteristicInfoResolver resolver )
		{
			try
			{
				var (characteristics, direction) = AnalyzeArguments( args, PtWorstAxis, 1, false );
				switch( direction )
				{
					case "X":
					case "Y":
					case "Z":
						return GetDirectionDependencies( resolver, characteristics, direction );
					case "XY":
					case "YX":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXy );
					case "XZ":
					case "ZX":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXz );
					case "YZ":
					case "ZY":
						return GetDirectionDependencies( resolver, characteristics, DirectionsYz );
					case "XYZ":
					case "XZY":
					case "YXZ":
					case "ZXY":
					case "YZX":
					case "ZYX":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXyz );
				}
			}
			catch
			{
				// do nothing
			}

			return [];
		}

		#endregion
	}
}