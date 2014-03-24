using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance
	{
		get { return _instance; }
	}

	public int TimerSpeed = 1; // How many ticks/second
	public int PlayerCount = 10;

	private static GameManager _instance = null;
	private const string SAVEFILEFOLDER = "/saves/";
	private float _timerLength = 0;
	private float _timer = 0;

	public delegate void TimerTickHandler();
	public event TimerTickHandler TimerTick;
	
	public Player CurrentPlayer
	{
		get { return _players[_currentPlayerIndex]; }
	}
	private List<Player> _players = new List<Player>();
	int _currentPlayerIndex = 0;

	public Player GetPlayerByName(string name)
	{
		foreach(Player p in _players)
			if(p.Name == name)
				return p;
		return null;
	}
	// Use this for initialization
	void Start ()
	{
		if(_instance == null)
			_instance = this;

		if(Directory.Exists(GameManager.GetApplicationPath() + SAVEFILEFOLDER) == false)
			Directory.CreateDirectory(GameManager.GetApplicationPath() + SAVEFILEFOLDER);

		// add players
		for(int i = 0; i < PlayerCount; i++)
		{
			_players.Add(new Player(Utility.GeneratePlayerName()));
			_players[i].SetCapital(
				GridManager.Instance.GetTileByIndex(
					Random.Range(1, GridManager.Instance.gridWidthInHexes),
					Random.Range(1, GridManager.Instance.gridHeightInHexes)
				)
			);
			//Debug.Log(string.Format("Name: {0}. Capital: {1}", _players[i].Name, _players[i].Capital.name));
		}
		//_players[0].SetCapital(

		SetTimerSpeed(TimerSpeed);
		TimerTick += new TimerTickHandler(TimerTickMethod);
	}

	void TimerTickMethod ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		_timer -= Time.deltaTime;
		if(_timer <= 0)
		{
			if(TimerTick != null)
				TimerTick();
			_timer = _timerLength;
		}
	}

	public void SetTimerSpeed(int speed)
	{
		_timerLength = 1f/speed;
	}

	public void LoadGame(string filePath)
	{
		try
		{
			GridManager gm = GameObject.Find("GridManager").GetComponent(typeof(GridManager)) as GridManager;
			Dictionary<string, string> hexes = new Dictionary<string, string>();

			foreach(string s in File.ReadAllLines(filePath))
			{
				if(s.Contains(";"))
				{
					hexes.Add(s.Substring(0, s.IndexOf(";")), s.Substring(s.IndexOf(";") + 1));
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
					sw.WriteLine("{0};{1}", child.name, tileInfo.terrainType.UniqueName);
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
