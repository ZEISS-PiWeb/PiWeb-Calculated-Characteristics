#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2019                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Functions
{
	/// <summary>
	/// Defines the types of available operation templates.
	/// </summary>
	public enum OperationTemplateTypes
	{
		/// <summary>
		/// A custom operation.
		/// </summary>
		Custom,
		/// <summary>
		/// The PT_MIN operation.
		/// </summary>
		PtMin,
		/// <summary>
		/// The PT_MAX operation.
		/// </summary>
		PtMax,
		/// <summary>
		/// The PT_SYM operation.
		/// </summary>
		PtSym,
		/// <summary>
		/// The PT_DIST operation.
		/// </summary>
		PtDist,
		/// <summary>
		/// The PT_REF operation.
		/// </summary>
		PtRef,
		/// <summary>
		/// The PT_POS_SQUARE operation.
		/// </summary>
		PtPosSquare,
		/// <summary>
		/// The PT_PROFILE operation.
		/// </summary>
		PtProfile,
		/// <summary>
		/// The PT_DIST_PT_2PT operation.
		/// </summary>
		PtDistPt2Pt,
		/// <summary>
		/// The PT_DIST_PT_3PT operation.
		/// </summary>
		PtDistPt3Pt,
		/// <summary>
		/// The PT_WORST operation.
		/// </summary>
		PtWorst,
		/// <summary>
		/// The PT_WORST_TARGET operation.
		/// </summary>
		PtWorstTarget,
		/// <summary>
		/// The PT_LEN operation for single position.
		/// </summary>
		PtPosLength,
		/// <summary>
		/// The PT_LEN operation for vector between two positions.
		/// </summary>
		PtVectorLength
	}
}