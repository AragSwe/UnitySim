using UnityEngine;
using System.Collections;

public class HexTile : MonoBehaviour
{
	public enum TileType
	{
		None,
		Plains,
		Woods,
		Mountain,
		Water
	}
	
	public TileType tileType;
	
	// Use this for initialization
	void Start () {
		if(tileType == TileType.None)
			SetTileType((TileType)Random.Range(2, 5));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTileType(HexTile.TileType newType)
	{
		tileType = newType;
		
		Color c = Color.black;
		switch(tileType)
		{
		case TileType.Mountain: c = Color.grey; break;
		case TileType.Plains: c = Color.green; break;
		case TileType.Woods: c = Color.green; break;
		case TileType.Water: c = Color.blue; break;
		}
		renderer.material.SetColor("_Color", c);
	}
}
