#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss IMT (IZfM Dresden)                   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2018                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.OprFunctions
{
	public class CharacteristicInfo
	{
		#region constructors

		public CharacteristicInfo( string direction, double? desired = null, Tolerance? tolerance = null, bool? isControlItem = null )
		{
			Direction = direction;
			Desired = desired;
			Tolerance = tolerance ?? Tolerance.Unbounded;
			IsControlItem = isControlItem;
		}

		#endregion

		#region properties

		public string Direction { get; }
		public double? Desired { get; }
		public Tolerance Tolerance { get; }
		public bool? IsControlItem { get; }

		#endregion
	}
}