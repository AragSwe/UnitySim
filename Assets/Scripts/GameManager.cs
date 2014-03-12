using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance
	{
		get { return _instance; }
	}
	private static GameManager _instance = null;

	// Use this for initialization
	void Start ()
	{
		if(_instance == null)
			_instance = this;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void LoadGame(string filePath)
	{
		try
		{
			GridManager gm = GameObject.Find("GridManager").GetComponent(typeof(GridManager)) as GridManager;
			Dictionary<Vector3, HexTile> hexes = new Dictionary<Vector3, HexTile>();
			HexTile tempHex = null;

			foreach(string s in File.ReadAllLines(filePath))
			{
				if(s.Contains(";"))
				{
					tempHex = new HexTile();
					tempHex.tileType = (HexTile.TileType)int.Parse(s.Substring(s.IndexOf(";") + 1));
					hexes.Add(stringToVector3(s.Substring(0, s.IndexOf(";"))), tempHex);
				}
				else
				{
					gm.gridWidthInHexes = int.Parse(s.Substring(0, s.IndexOf(",")));
					gm.gridHeightInHexes = int.Parse(s.Substring(s.IndexOf(",") + 1));
				}
			}

			gm.RecreateWorld(hexes);
		}
		catch(IOException ex)
		{
			Debug.LogError("Error loading game: " + ex.Message);
		}
	}

	Vector3 stringToVector3(string s)
	{
		Vector3 v3 = new Vector3();
		int curPos = 0;
		float[] temp = new float[3];

		// ex. string = (1, 2, 3)
		for(int i = 0; i < 3; i++)
		{
			s = s.Substring(curPos + 1); //remove (
			curPos = s.IndexOfAny(new char[]{ ',', ')' });
			temp[i] = float.Parse(s.Substring(0, curPos));
		}

		v3.x = temp[0];
		v3.y = temp[1];
		v3.z = temp[2];
		return v3;
	}
}
