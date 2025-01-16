#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics.Arithmetic
{
	#region usings

	using System.Collections.Generic;

	#endregion

	/// <summary>
	/// Represents a function parsed from a formula.
	/// </summary>
	public class Function : MathElement
	{
		#region members

		private readonly MathOperation _Operation;

		#endregion

		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		internal Function(
			int startPosition,
			int length,
			IReadOnlyCollection<MathElement>? arguments,
			string name,
			MathOperation operation )
			: base( startPosition, length, arguments )
		{
			Name = name;
			_Operation = operation;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the name of the function.
		/// </summary>
		public string Name { get; }

		#endregion

		#region methods

		/// <inheritdoc />
		public override double? GetResult( ICharacteristicValueResolver resolver )
		{
			return _Operation.CalculateValue( Arguments, resolver );
		}

		/// <inheritdoc/>
		protected override IEnumerable<MathDependencyInformation> OnCheckForDependentCharacteristics( ICharacteristicInfoResolver resolver )
		{
			if( _Operation.GetDependentCharacteristics == null )
				return base.OnCheckForDependentCharacteristics( resolver );

			return _Operation.GetDependentCharacteristics( Arguments, resolver );
		}

		#endregion
	}
}