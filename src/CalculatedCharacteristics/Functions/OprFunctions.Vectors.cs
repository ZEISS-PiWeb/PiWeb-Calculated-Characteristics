#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2022                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	using System;
	using CalculatedCharacteristics.Arithmetic;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;

	public static partial class OprFunctions
	{
		internal const string MethodNamePtLen = "PT_LEN";

		/// <summary>
		/// Calculates length of an vector.
		/// Expected arguments:
		/// * at least 1 characteristic
		/// * any direction literal (e.g. X,Y,Z,...)
		/// </summary>
		[OperationTemplate( MethodNamePtLen + "($PATH0;\"XYZ\")", OperationTemplateTypes.PtLen )]
		[OperationTemplate( MethodNamePtLen + "($PATH0;$PATH1;\"XYZ\")", OperationTemplateTypes.PtLen2 )]
		public static double? Pt_Len( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicValueResolver resolver )
		{
			var (characteristics, direction) = AnalyzeArguments( args, MethodNamePtLen, 1, 2, "[X,Y,Z,XY,XZ,YZ,XYZ]" );

			switch( direction )
			{
				case "X":
				case "Y":
				case "Z":
					return GetSingleDimensionLength( characteristics, direction, resolver );

				case "XY":
				case "XZ":
				case "YZ":
					return GetTwoDimensionLength( characteristics, direction, resolver );

				case "XYZ":
					return GetThreeDimensionLength( characteristics, resolver );
			}

			return null;
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

			var dir1 = new string( direction.AsSpan( 0, 1 ));
			var dir2 = new string( direction.AsSpan( 1, 1 ));

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

			if( !d1.HasValue || !d2.HasValue || !d3.HasValue)
				return null;

			return Math.Sqrt( d1.Value * d1.Value + d2.Value * d2.Value + d3.Value * d3.Value );
		}

		/// <summary>
		/// Gets the paths that are referenced by <see cref="Pt_Len"/>.
		/// Expected arguments:
		/// * at least 1 or 2 characteristics
		/// * any direction literal (e.g. X,Y,Z,XY,XZ,YZ,XYZ)
		/// </summary>
		public static IEnumerable<MathDependencyInformation> Pt_Len_DependentCharacteristics( [NotNull] IReadOnlyCollection<MathElement> args, [NotNull] ICharacteristicInfoResolver resolver )
		{
			try
			{
				var (characteristics, direction) = AnalyzeArguments( args, MethodNamePtLen, 1, 2 );
				switch( direction )
				{
					case "X":
					case "Y":
					case "Z":
						return GetDirectionDependencies( resolver, characteristics, direction );
					case "XY":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXY );
					case "XZ":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXZ );
					case "YZ":
						return GetDirectionDependencies( resolver, characteristics, DirectionsYZ );
					case "XYZ":
						return GetDirectionDependencies( resolver, characteristics, DirectionsXYZ );
				}
			}
			catch
			{
				/**/
			}

			return Enumerable.Empty<MathDependencyInformation>();
		}
	}
}