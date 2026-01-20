#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Reflection;
	using Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic;
	using Zeiss.PiWeb.CalculatedCharacteristics.Functions;

	#endregion

	public sealed partial class MathInterpreter
	{
		#region class OperationCatalog

		/// <summary>
		/// Represents a catalog of operations and constants that should be supported by <see cref="MathInterpreter"/>.
		/// </summary>
		private sealed class OperationCatalog
		{
			#region members

			private static readonly Lazy<OperationCatalog> LazyDefault = new Lazy<OperationCatalog>();

			private readonly Dictionary<string, MathOperation> _Operations =
				new Dictionary<string, MathOperation>( StringComparer.OrdinalIgnoreCase );

			private readonly Dictionary<string, Constant> _Constants = new Dictionary<string, Constant>( StringComparer.OrdinalIgnoreCase );

			#endregion

			#region constructors

			/// <summary>
			/// Creates a new instance of <see ref="OperationCatalog"/>.
			/// </summary>
			public OperationCatalog()
			{
				AddBasicConstants( this );
				AddBasicFunctions( this );
				AddSheetMetalFunctions( this );
				AddAuditFunctions( this );
			}

			#endregion

			#region properties

			/// <summary>
			/// Gets the default catalog.
			/// </summary>
			public static OperationCatalog Default => LazyDefault.Value;

			#endregion

			#region methods

			/// <summary>
			/// Tries to get an operation by the given <paramref name="name"/>.
			/// </summary>
			/// <returns><code>true</code> if the operation could be found, otherwise <code>false</code>.</returns>
			public bool TryGetOperation( string name, out MathOperation mathOperation )
			{
				return _Operations.TryGetValue( name, out mathOperation );
			}

			/// <summary>
			/// Tries to get a constant by the given <paramref name="name"/>.
			/// </summary>
			/// <returns><code>true</code> if the constant could be found, otherwise <code>false</code>.</returns>
			public bool TryGetConstant( string name, [NotNullWhen( true )] out Constant? constant )
			{
				return _Constants.TryGetValue( name, out constant );
			}

			/// <summary>
			/// Registers an operation.
			/// </summary>
			private void AddOperation(
				string name,
				CalculateValueDelegate calculationHandler,
				GetDependentCharacteristicsDelegate? dependentCharacteristicsHandler = null )
			{
				_Operations.Add( name, new MathOperation( calculationHandler, dependentCharacteristicsHandler ) );
			}

			/// <summary>
			/// Registers a constant.
			/// </summary>
			private void AddConstant( string name, double value )
			{
				_Constants.Add( name, new Constant( value, name ) );
			}

			/// <summary>
			/// Registers operations for calculations from the car body sector.
			/// </summary>
			private static void AddSheetMetalFunctions( OperationCatalog catalog )
			{
				catalog.AddOperation( OprFunctions.PtMin, OprFunctions.Pt_Min, OprFunctions.Pt_Min_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtMax, OprFunctions.Pt_Max, OprFunctions.Pt_Max_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtSym, OprFunctions.Pt_Sym, OprFunctions.Pt_Sym_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtDist, OprFunctions.Pt_Dist, OprFunctions.Pt_Dist_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtRef, OprFunctions.Pt_Ref, OprFunctions.Pt_Ref_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtPosSquare, OprFunctions.Pt_Pos_Square, OprFunctions.Pt_Pos_Square_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtProfile, OprFunctions.Pt_Profile, OprFunctions.Pt_Profile_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtWorst, OprFunctions.Pt_Worst, OprFunctions.Pt_Worst_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtWorstTarget, OprFunctions.Pt_Worst_Target, OprFunctions.Pt_Worst_Target_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtDistPoint2Line, OprFunctions.Pt_Dist_Pt_2Pt, OprFunctions.Pt_Dist_Pt_2Pt_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtDistPoint2Plane, OprFunctions.Pt_Dist_Pt_3Pt, OprFunctions.Pt_Dist_Pt_3Pt_DependentCharacteristics );
				catalog.AddOperation( OprFunctions.PtLen, OprFunctions.Pt_Len, OprFunctions.Pt_Len_DependentCharacteristics );
			}

			/// <summary>
			/// Registers operations for audit grade calculation.
			/// </summary>
			private static void AddAuditFunctions( OperationCatalog catalog )
			{
				catalog.AddOperation( "QZ", AuditFunctions.AuditGrade, AuditFunctions.AuditGrade_DependentCharacteristics );
				catalog.AddOperation( "AverageQZ", AuditFunctions.AverageAuditGrade, AuditFunctions.AverageOrGroupedAuditGrade_DependentCharacteristics );
				catalog.AddOperation( "GroupedQZ", AuditFunctions.GroupedAuditGrade, AuditFunctions.AverageOrGroupedAuditGrade_DependentCharacteristics );
			}

			/// <summary>
			/// Registers basic mathematical operations:<br/>
			/// +,-,*,/,sin,cos,tan,sqr,sqrt,abs,max,min
			/// </summary>
			private static void AddBasicFunctions( OperationCatalog catalog )
			{
				var methodInfos = typeof( BasicFunctions ).GetMethods( BindingFlags.Static | BindingFlags.Public );
				foreach( var methodInfo in methodInfos )
				{
					var attribute = methodInfo.GetCustomAttribute<BasicFunctionAttribute>();
					if( attribute is null )
						continue;

					var function = Delegate.CreateDelegate(typeof(CalculateValueDelegate), methodInfo) as CalculateValueDelegate;
					if( function is null )
						continue;

					catalog.AddOperation( attribute.Name, function );
				}
			}

			/// <summary>
			/// Registers basic constants:<br/>
			/// Pi, e
			/// </summary>
			private static void AddBasicConstants( OperationCatalog catalog )
			{
				catalog.AddConstant( "E", Math.E );
				catalog.AddConstant( "PI", Math.PI );
			}

			#endregion
		}

		#endregion
	}
}