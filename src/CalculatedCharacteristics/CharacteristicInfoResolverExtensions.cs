﻿#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2021                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

namespace Zeiss.PiWeb.CalculatedCharacteristics
{
	#region usings

	using Zeiss.PiWeb.Api.Core;

	#endregion

	/// <summary>
	/// Provides extension methods for <see cref="ICharacteristicInfoResolver"/>.
	/// </summary>
	public static class CharacteristicInfoResolverExtensions
	{
		#region methods

		/// <summary>
		/// Gets the typed attribute value for a characteristic.
		/// </summary>
		/// <typeparam name="T">The expected type of value.</typeparam>
		/// <param name="resolver">The <see cref="ICharacteristicInfoResolver"/> instance.</param>
		/// <param name="path">The path of the characteristic.</param>
		/// <param name="key">The attribute key.</param>
		/// <param name="defaultValue">The optional default value if no attribute value available ot type does not match.</param>
		/// <returns>The measured or calculated value.</returns>
		public static T? GetEntityAttributeValue<T>(
			this ICharacteristicInfoResolver resolver,
			PathInformation path,
			ushort key,
			T? defaultValue = default )
		{
			return resolver.GetEntityAttributeValue( path, key ) is T typedValue ? typedValue : defaultValue;
		}

		#endregion
	}
}