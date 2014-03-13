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
	private const string SAVEFILEFOLDER = "/saves/";

	// Use this for initialization
	void Start ()
	{
		if(_instance == null)
			_instance = this;

		if(Directory.Exists(GameManager.GetApplicationPath() + SAVEFILEFOLDER) == false)
			Directory.CreateDirectory(GameManager.GetApplicationPath() + SAVEFILEFOLDER);
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
			Dictionary<Vector3, string> hexes = new Dictionary<Vector3, string>();
			HexTile tempHex = null;

			foreach(string s in File.ReadAllLines(filePath))
			{
				if(s.Contains(";"))
				{
					hexes.Add(stringToVector3(s.Substring(0, s.IndexOf(";"))), s.Substring(s.IndexOf(";") + 1));
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
	
	public void SaveGame (string saveGameName)
	{
		try
		{
			GridManager gm = GridManager.Instance;
			Transform HexGrid = GameObject.Find("HexGrid").transform;
			using(StreamWriter sw = new StreamWriter(GetApplicationPath() + SAVEFILEFOLDER + saveGameName, false))
			{
				sw.WriteLine("{0}, {1}", gm.gridWidthInHexes, gm.gridHeightInHexes);
				Transform child = null;
				HexTile tileInfo = null;
				for(int i = 0; i < HexGrid.childCount; i++)
				{
					child = HexGrid.GetChild(i);
					tileInfo = (HexTile)child.GetComponent(typeof(HexTile));
					sw.WriteLine("{0};{1}", child.position, tileInfo.terrainType.UniqueName);
				}
			}
		}
		catch (IOException ex)
		{
			Debug.LogError("Error while saving file: " + ex.Message);
		}
	}
	
	public static string[] GetSaveFiles()
	{
		string saveFileDirectoryPath = GetApplicationPath() + SAVEFILEFOLDER;
		string[] saveFiles = new string[]{};
		
		try
		{
			if(Directory.Exists(saveFileDirectoryPath) == false)
				Directory.CreateDirectory(saveFileDirectoryPath);
			
			saveFiles = Directory.GetFiles(saveFileDirectoryPath);
		}
		catch (IOException ex)
		{
			//Handle error lol
			Debug.LogError("Error while loading savefiles: " + ex.Message);
		}
		
		return saveFiles;
	}
	
	public static string GetApplicationPath()
	{
		string path = Application.dataPath;
		if (Application.platform == RuntimePlatform.OSXPlayer) {
			path += "/../../";
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer) {
			path += "/../";
		}
		return path;
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
