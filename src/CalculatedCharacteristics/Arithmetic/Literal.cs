﻿#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Innovationszentrum für Messtechnik   */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2012                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.Shared.CalculatedCharacteristics.Arithmetic
{
	/// <summary>
	/// Represents a literal parsed from a formula.
	/// </summary>
	public class Literal : MathElement
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		public Literal( int startPosition, int length, string originText, string text ) : base( startPosition, length )
		{
			Text = text;
			OriginText = originText;
		}

		#endregion

		#region properties

		/// <summary>
		/// Gets the parsed text of the literal.
		/// </summary>
		public string Text { get; }

		// FIXME: Needed?
		/// <summary>
		/// Gets the original text of the literal (with escaping).
		/// </summary>
		public string OriginText { get; }

		#endregion

		#region methods

		/// <inheritdoc />
		/// <exception cref="ParserException">A literal cannot calculate a value.</exception>
		public override double? GetResult( ICharacteristicValueResolver context )
		{
			throw new ParserException( "A literal can not calculate a value [" + Text + "]", 0 );
		}

		#endregion
	}
}