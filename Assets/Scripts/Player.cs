using UnityEngine;
using System.Collections.Generic;

public class Player
{
	public string Name
	{
		get { return _name; }
	}
	private string _name = "";

	public Player(string name)
	{
		_name = name;
	}

	public Dictionary<string, float> ResourceAmounts
	{
		get { return _resourceAmounts; }
	}
	private Dictionary<string, float> _resourceAmounts = new Dictionary<string, float>();

	public Color OwnerColor 
	{
		get { return _color; }
	}
	private Color _color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
	HexTile _capital = null;

	public void AddResource (string resourceName, float amount)
	{
		float a;
		if(_resourceAmounts.TryGetValue(resourceName, out a) == false)
			_resourceAmounts[resourceName] = 0;
		_resourceAmounts[resourceName] += amount;
	}

	public void SetCapital(HexTile tile)
	{
		tile.SetPlayerOwner(_name);
		_capital = tile;
	}
}
