//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
	public static TValue RandomValue<TKey, TValue>(IDictionary<TKey, TValue> dict)
	{
		return RandomValues(dict).Take(1).First();
	}

	public static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
	{
		List<TValue> values = Enumerable.ToList(dict.Values);
		int size = dict.Count;
		
		while(true)
		{
			yield return values[UnityEngine.Random.Range(0, size)];
		}
	}

	public static string GeneratePlayerName()
	{
		return "player" + UnityEngine.Random.Range(1, 1000);
	}

	public static Color GeneratePlayerColor()
	{
		Color c = new Color(UnityEngine.Random.Range(0, 255)/255f, UnityEngine.Random.Range(0, 255)/255f, UnityEngine.Random.Range(0, 255)/255f);
		return c;
	}
}

