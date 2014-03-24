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
		LoadGame,
		LoadingGame
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

	public GUISkin guiSkin = null;

	public bool IsMouseOverGUI { get { return lastToolTip != ""; } }
	private string lastToolTip = "";

	void Start()
	{
		if(_instance == null)
			_instance = this;

		ScreenCenter = new Rect(Screen.width / 2, Screen.height / 2, 0, 0);
	}

	void OnGUI()
	{
		GUI.skin = guiSkin;

		DrawOwnPlayerInfo();

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
		case Menu.LoadingGame:
			ShowLoadingMenu();
			break;
		default:
			break;
		}

		if(GridManager.Instance.CurrentSelectedHex != null)
		{
			ShowHexInfo((HexTile)GridManager.Instance.CurrentSelectedHex.GetComponent(typeof(HexTile)));
		}

		if(Input.GetMouseButtonDown(0) == false)
			lastToolTip = GUI.tooltip;
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
		GUIStyle s = new GUIStyle();
		GUI.BeginGroup(CreateCenterRect(100, 200));
		if(CreateButton(new Rect(0, 0, 80, 20), "New Game"))
		{
			GridManager.Instance.RecreateWorld();
			CloseAllMenus();
		}
		if(CreateButton(new Rect(0, 25, 80, 20), "Save"))
			ToggleSaveGameMenu();
		if(CreateButton(new Rect(0, 50, 80, 20), "Load"))
			ToggleLoadGameMenu();
		if(CreateButton(new Rect(0, 75, 80, 20), "Exit"))
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

	public void ToggleLoadingMenu()
	{
		CloseAllMenus();
		_activeMenu = Menu.LoadingGame;
	}

	void DrawOwnPlayerInfo ()
	{
		GUI.BeginGroup(new Rect(10, 10, Screen.width, 120));

		GUI.Label(new Rect(0, 0, 500, 20), "You are: " + GameManager.Instance.CurrentPlayer.Name);
		if(GUI.Button(new Rect(0, 25, 80, 20), "Capital"))
			GridManager.Instance.ZoomToCapital(GameManager.Instance.CurrentPlayer);
		GUI.EndGroup();
	}

	void ShowSaveGameMenu()
	{
		GUI.BeginGroup(CreateCenterRect(100, 200));

		GUI.Label(new Rect(0, 0, 200, 20), "Enter save name:");
		saveGameFileName = GUI.TextField(new Rect(0, 25, 200, 20), saveGameFileName);
		if(CreateButton(new Rect(0, 50, 80, 20), "Save"))
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
			if(CreateButton(new Rect(0, buttonTop, 80, buttonHeight), Path.GetFileName(filePath)))
			{
				GameManager.Instance.LoadGame(filePath);
				CloseAllMenus();
			}
		}

		GUI.EndGroup();
	}

	void ShowLoadingMenu()
	{
		GUI.HorizontalSlider (CreateCenterRect(200, 20), 40f, 0.0f, 10.0f);
	}

	void ShowHexInfo (HexTile currentSelectedHex)
	{
		GUI.BeginGroup(new Rect(Screen.width - 305, 20, 300, 500));

		GUI.Label(new Rect(0, 0, 300, 20), "Type: " + currentSelectedHex.terrainType.FriendlyName);

		if(currentSelectedHex.PlayerOwner != null)
			GUI.Label(new Rect(0, 25, 300, 20), "Owned by: " + currentSelectedHex.PlayerOwner.Name);
		else
			GUI.Label(new Rect(0, 25, 300, 20), "Owned by: noone");

		int top = 50;
		foreach(IAction action in currentSelectedHex.Actions)
		{
			if(CreateButton(new Rect(0, top, 80, 20), action.ActionName))
			{
				action.Execute(GameManager.Instance.CurrentPlayer.Name, currentSelectedHex);
			}
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

	bool CreateButton(Rect rect, string text)
	{
		return GUI.Button(rect, new GUIContent(text, "aa"));
	}
}
