using System.Collections.Generic;

namespace Opswat.Metadefender.Core.Client
{
	public static class DictionaryValueGetter
	{
		public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : class
		{
			try
			{
				return dictionary[key];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}
	}
}
