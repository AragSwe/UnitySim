using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class HexTile : MonoBehaviour
{
	public TerrainType terrainType;
	public IList<IAction> Actions
	{
		get { return GetActions(); }
	}

	public string PlayerOwner
	{
		get { return _playerOwner; }
	}
	private string _playerOwner = "";
	
	// Use this for initialization
	void Start () {
		if(terrainType == null)
			SetTileType(Utility.RandomValue(ConfigurationManager.Instance.TerrainTypes).UniqueName);

		GameManager.Instance.TimerTick += new GameManager.TimerTickHandler(TimerTick);
	}

	void TimerTick ()
	{
		GenerateResources();
	}
	
	// Update is called once per frame
	void Update () {
		// Use TimerTick instead
	}

	public void SetTileType(string newType)
	{
		terrainType = ConfigurationManager.Instance.TerrainTypes[newType];
		
		Color c = Color.black;
		switch(newType)
		{
		case "mountain": c = Color.grey; break;
		case "plains": c = Color.green; break;
		case "woods": c = Color.yellow; break;
		case "water": c = Color.blue; break;
		}
		renderer.material.SetColor("_Color", c);
	}

	public void SetPlayerOwner(string playerName)
	{
		_playerOwner = playerName;
	}

	IList<IAction> GetActions ()
	{
		if(terrainType.UniqueName != "water")
			return new List<IAction> { new StakeClaim() };
		return new List<IAction>();
	}

	void GenerateResources()
	{
		Player p = null;
		if(_playerOwner != "")
			p = GameManager.Instance.GetPlayerByName(_playerOwner);

		if(p != null)
		{
			terrainType.Resources.ForEach( res => p.AddResource(res.ResourceName, res.BaseModifier) );
		}
	}
}
