using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
	public static GridManager Instance
	{
		get { return _instance; }
	}
	private static GridManager _instance = null;
	//following public variable is used to store the hex model prefab;
    //instantiate it by dragging the prefab on this variable using unity editor
    public GameObject Hex;
    //next two variables can also be instantiated using unity editor
    public int gridWidthInHexes = 10;
    public int gridHeightInHexes = 10;

	 //Hexagon tile width and height in game world
    private float hexWidth;
    private float hexHeight;

	private Transform currentHoverHex = null;
	private GameObject hexGridGO = null;

	public Transform CurrentSelectedHex { get { return currentSelectedHex; } }
	private Transform currentSelectedHex = null;

	//Method to initialise Hexagon width and height
    void setSizes()
    {
        //renderer component attached to the Hex prefab is used to get the current width and height
        hexWidth = Hex.renderer.bounds.size.x;
        hexHeight = Hex.renderer.bounds.size.z;
    }
	
	//Method to calculate the position of the first hexagon tile
    //The center of the hex grid is (0,0,0)
    Vector3 calcInitPos()
    {
        Vector3 initPos;
        //the initial position will be in the left upper corner
        initPos = new Vector3(-hexWidth * gridWidthInHexes / 2f + hexWidth / 2, 0,
            gridHeightInHexes / 2f * hexHeight - hexHeight / 2);

        return initPos;
    }
	
	//method used to convert hex grid coordinates to game world coordinates
    public Vector3 calcWorldCoord(Vector2 gridPos)
    {
        //Position of the first hex tile
        Vector3 initPos = calcInitPos();
        //Every second row is offset by half of the tile width
        float offset = 0;
        if (gridPos.y % 2 != 0)
            offset = hexWidth / 2;

        float x =  initPos.x + offset + gridPos.x * hexWidth;
        //Every new line is offset in z direction by 3/4 of the hexagon height
        float z = initPos.z - gridPos.y * hexHeight * 0.75f;
        return new Vector3(x, 0, z);
    }
	
	//Finally the method which initialises and positions all the tiles
	void createGrid(Dictionary<string, string> hexGridInfo)
    {
        //Game object which is the parent of all the hex tiles
        hexGridGO = new GameObject("HexGrid");

		if(hexGridInfo == null)
			hexGridInfo = new Dictionary<string, string>();

		if(Menus.Instance != null)
			Menus.Instance.ToggleLoadingMenu();

		string hexType = "";
        for (float y = 0; y < gridHeightInHexes; y++)
        {
            for (float x = 0; x < gridWidthInHexes; x++)
            {
                //GameObject assigned to Hex public variable is cloned
                //Current position in grid
                Vector2 gridPos = new Vector2(x, y);
        		GameObject hex = (GameObject)Instantiate(Hex);
				hex.transform.position = calcWorldCoord(gridPos);
				hex.transform.parent = hexGridGO.transform;
				hex.name = "h" + x + "," + y;
				if(hexGridInfo.TryGetValue(hex.name, out hexType))
					((HexTile)hex.GetComponent(typeof(HexTile))).SetTileType(hexType);
            }
		}

		MeshFilter[] meshFilters = hexGridGO.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		for (var i = 0; i < meshFilters.Length; i++){
			combine[i].mesh = meshFilters[i].mesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].gameObject.active = false;
		}
		hexGridGO.AddComponent<MeshFilter>();
		hexGridGO.AddComponent<MeshRenderer>();
		hexGridGO.renderer.sharedMaterials = meshFilters[0].renderer.sharedMaterials;
		hexGridGO.transform.GetComponent<MeshFilter>().mesh = new Mesh();
		hexGridGO.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
		hexGridGO.active = true;
    }

    //The grid should be generated on game start
    void Start()
    {
		if(_instance == null)
			_instance = this;
		RecreateWorld();
    }


	public void RecreateWorld()
	{
		RecreateWorld(null);
	}

	public void RecreateWorld(Dictionary<string, string> hexGridInfo)
	{
		if(hexGridGO != null)
		{
			Destroy(hexGridGO);
			hexGridGO = null;
		}

		setSizes();
		createGrid(hexGridInfo);
	}

	public void HoverHex(Transform hoverHex)
	{
		if(hoverHex != currentHoverHex)
		{
			if(currentHoverHex != null && currentHoverHex != currentSelectedHex)
			{
				ResetHexColors(currentHoverHex);
			}
			if(hoverHex != null)
				hoverHex.renderer.material.SetColor("_Color", Color.white);
			currentHoverHex = hoverHex;
		}
	}

	public void SelectHex(Transform selectedHex)
	{
		if(currentSelectedHex != selectedHex)
		{
			if(currentSelectedHex != null)
				ResetHexColors(currentSelectedHex);
			currentSelectedHex = selectedHex;
			currentSelectedHex.renderer.material.SetColor("_Color", Color.white);

			Debug.Log(currentSelectedHex.renderer.sharedMaterials.Length);
		}
	}

	public void DeselectHex()
	{
		if(currentSelectedHex != null)
			ResetHexColors(currentSelectedHex);
		currentSelectedHex = null;
	}

	private void ResetHexColors(Transform t)
	{
		HexTile ht = (HexTile)t.GetComponent(typeof(HexTile));
		ht.ResetColors();
	}

	public HexTile GetTileByIndex(int x, int y)
	{
		if(hexGridGO == null)
			return null;

		return hexGridGO.transform.FindChild("h" + x + "," + y).GetComponent<HexTile>();
	}

	public void ZoomToCapital (Player p)
	{
		Camera.main.transform.position = p.Capital.transform.position;
		Camera.main.transform.Translate(new Vector3(0, 0, -20), Space.Self);
		SelectHex(p.Capital.transform);
	}
}
