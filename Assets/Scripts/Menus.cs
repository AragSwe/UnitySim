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
	private string saveGameFileName = "";

	public bool IsMouseOverGUI { get { return isMouseOverGUI; } }
	private bool isMouseOverGUI = false;

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
			saveFilesList = GameManager.GetSaveFiles();
			_activeMenu = Menu.LoadGame;
		}
	}

	void ShowSaveGameMenu()
	{
		GUI.BeginGroup(CreateCenterRect(100, 200));

		GUI.Label(new Rect(0, 0, 200, 20), "Enter save name:");
		saveGameFileName = GUI.TextField(new Rect(0, 25, 200, 20), saveGameFileName);
		if(GUI.Button(new Rect(0, 50, 80, 20), "Save") && isMouseOverGUI)
		{
			GameManager.Instance.SaveGame(saveGameFileName);
			CloseAllMenus();
		}
		GUI.EndGroup();
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
			{
				GameManager.Instance.LoadGame(filePath);
				CloseAllMenus();
			}
		}

		GUI.EndGroup();
	}

	void ShowHexInfo (HexTile currentSelectedHex)
	{
		GUI.BeginGroup(new Rect(Screen.width - 305, 20, 300, 500));

		GUI.Label(new Rect(0, 0, 300, 20), "Type: " + currentSelectedHex.tileType);

		int top = 25;
		foreach(IAction action in currentSelectedHex.Actions)
		{
			if(GUI.Button(new Rect(0, top, 80, 20), action.ActionName))
				action.Execute(currentSelectedHex);
			top += 25;
		}
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

	void CloseAllMenus ()
	{
		_activeMenu = Menu.None;
	}
}
