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
	
	// Use this for initialization
	void Start () {
		if(terrainType == null)
			SetTileType(Utility.RandomValue(ConfigurationManager.Instance.TerrainTypes).UniqueName);
	}
	
	// Update is called once per frame
	void Update () {
	
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

	IList<IAction> GetActions ()
	{
		return new List<IAction> { new StakeClaim() };
	}

	IDictionary<ResourceType, float> GenerateResources()
	{
		Dictionary<ResourceType, float> di = new Dictionary<ResourceType, float>();
		terrainType.Resources.ForEach( res => di.Add(ConfigurationManager.Instance.ResourceTypes[res.ResourceName], res.BaseModifier));

		return di;
	}
}
