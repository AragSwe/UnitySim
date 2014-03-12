using UnityEngine;
using System.Collections;
using System.IO;

public class Menus : MonoBehaviour
{
	public enum Menu
	{
		None,
		Main,
		SaveGame,
		LoadGame
	}

	public static Menus Instance
	{
		get
		{
			return _instance;
		}
	}
	private static Menus _instance = null;

	private Menu _activeMenu = Menu.None;
	private Rect ScreenCenter;
	private string[] saveFilesList = new string[]{};
	private const string SAVEFILEFOLDER = "/saves/";
	private string saveGameFileName = "";

	void Start()
	{
		if(_instance == null)
			_instance = this;

		ScreenCenter = new Rect(Screen.width / 2, Screen.height / 2, 0, 0);
	}

	void OnGUI()
	{
		switch(_activeMenu)
		{
		case Menu.Main:
			ShowMainMenu();
			break;
		case Menu.LoadGame:
			ShowLoadGameMenu();
			break;
		case Menu.SaveGame:
			ShowSaveGameMenu();
			break;
		default:
			break;
		}

		if(GridManager.Instance.CurrentSelectedHex != null)
		{
			ShowHexInfo((HexTile)GridManager.Instance.CurrentSelectedHex.GetComponent(typeof(HexTile)));
		}
	}

	public void ToggleMainMenu()
	{
		ToggleMainMenu(false);
	}

	public void ToggleMainMenu(bool forceShow)
	{
		if(_activeMenu != Menu.None && forceShow == false)
			CloseAllMenus();
		else
		{
			_activeMenu = Menu.Main;
		}
	}

	void ShowMainMenu ()
	{
		GUI.BeginGroup(CreateCenterRect(100, 200));
		if(GUI.Button(new Rect(0, 0, 80, 20), "New Game"))
		{
			GridManager.Instance.RecreateWorld();
			CloseAllMenus();
		}
		if(GUI.Button(new Rect(0, 25, 80, 20), "Save"))
			ToggleSaveGameMenu();
		if(GUI.Button(new Rect(0, 50, 80, 20), "Load"))
			ToggleLoadGameMenu();
		if(GUI.Button(new Rect(0, 75, 80, 20), "Exit"))
			Application.Quit();
		GUI.EndGroup();
	}

	void ToggleSaveGameMenu ()
	{
		if(_activeMenu == Menu.SaveGame)
			_activeMenu = Menu.None;
		else
			_activeMenu = Menu.SaveGame;
	}

	public void ToggleLoadGameMenu()
	{
		if(_activeMenu == Menu.LoadGame)
			_activeMenu = Menu.None;
		else
		{
			saveFilesList = GetSaveFiles();
			_activeMenu = Menu.LoadGame;
		}
	}

	void ShowSaveGameMenu()
	{
		GUI.BeginGroup(CreateCenterRect(100, 200));

		GUI.Label(new Rect(0, 0, 200, 20), "Enter save name:");
		saveGameFileName = GUI.TextField(new Rect(0, 25, 200, 20), saveGameFileName);
		if(GUI.Button(new Rect(0, 50, 80, 20), "Save"))
			SaveGame(saveGameFileName);
		GUI.EndGroup();
	}

	void SaveGame (string saveGameName)
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
					sw.WriteLine("{0};{1}", child.position, (int)tileInfo.tileType);
				}
			}
		}
		catch (IOException ex)
		{
			Debug.LogError("Error while saving file: " + ex.Message);
		}

		CloseAllMenus();
	}

	void ShowLoadGameMenu()
	{
		int buttonTop = 0;
		int buttonHeight = 20;

		GUI.BeginGroup(CreateCenterRect(100, 200));

		GUI.Label(new Rect(0, buttonTop, 200, buttonHeight), "List of save files:");

		foreach(string filePath in saveFilesList)
		{
			buttonTop += buttonHeight + 5;
			if(GUI.Button(new Rect(0, buttonTop, 80, buttonHeight), Path.GetFileName(filePath)))
				GameManager.Instance.LoadGame(filePath);
		}

		GUI.EndGroup();
	}

	void ShowHexInfo (HexTile currentSelectedHex)
	{
		GUI.BeginGroup(new Rect(Screen.width - 305, 20, 300, 500));

		GUI.Label(new Rect(0, 0, 300, 20), "Type: " + currentSelectedHex.tileType);

		GUI.EndGroup();
	}

	private Rect CreateCenterRect(int width, int height)
	{
		Rect newRect = ScreenCenter;
		newRect.x -= width / 2;
		newRect.y -= height / 2;
		newRect.width = width;
		newRect.height = height;

		return newRect;
	}

	private static string[] GetSaveFiles()
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

	void CloseAllMenus ()
	{
		_activeMenu = Menu.None;
	}
}
