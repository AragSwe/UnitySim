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

	public HexTile Capital
	{
		get
		{ return _capital;}
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
	private Color _color = Utility.GeneratePlayerColor();

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
