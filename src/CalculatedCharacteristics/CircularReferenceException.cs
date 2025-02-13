﻿#region Copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2020                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using System;

	#endregion

	/// <summary>
	/// Represents an exception that occurred while validating a formula.
	/// </summary>
	public class CircularReferenceException : Exception
	{
		#region constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">The message of the exception.</param>
		public CircularReferenceException( string message )
			: base( message )
		{ }

		#endregion
	}
}