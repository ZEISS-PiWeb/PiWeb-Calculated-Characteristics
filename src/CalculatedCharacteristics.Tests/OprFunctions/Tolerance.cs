#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Tests.OprFunctions
{
	public readonly struct Tolerance
	{
		#region constructors

		public Tolerance( double? lowerBound, double? upperBound )
		{
			UpperBound = upperBound;
			LowerBound = lowerBound;

			if( lowerBound.HasValue && upperBound.HasValue && lowerBound.Value > upperBound.Value )
			{
				UpperBound = lowerBound;
				LowerBound = upperBound;
			}
		}

		#endregion

		#region properties

		public static Tolerance Unbounded => new Tolerance();

		public double? LowerBound { get; }

		public double? UpperBound { get; }

		#endregion
	}
}