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

	public Player PlayerOwner
	{
		get { return _playerOwner; }
	}
	private Player _playerOwner = null;
	
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
		case "mountains": c = Color.gray; break;
		case "plains": c = Color.green; break;
		case "woods": c = Color.yellow; break;
		case "water": c = Color.blue; break;
		}
		renderer.materials[1].SetColor("_Color", c);
	}

	public void SetPlayerOwner(string playerName)
	{
		Player player = GameManager.Instance.GetPlayerByName(playerName);
		_playerOwner = player;
		renderer.materials[0].SetColor("_Color", player.OwnerColor);
	}

	public void ResetColors()
	{
		SetTileType(terrainType.UniqueName);
		if(_playerOwner != null)
			renderer.materials[0].SetColor("_Color", _playerOwner.OwnerColor);
		else
			renderer.materials[0].SetColor("_Color", Color.black);
	}

	IList<IAction> GetActions ()
	{
		if(terrainType.UniqueName != "water")
			return new List<IAction> { new StakeClaim() };
		return new List<IAction>();
	}

	void GenerateResources()
	{
		if(_playerOwner != null)
		{
			terrainType.Resources.ForEach( res => _playerOwner.AddResource(res.ResourceName, res.BaseModifier) );
		}
	}
}
