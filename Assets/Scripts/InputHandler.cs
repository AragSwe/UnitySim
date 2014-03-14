using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			if(GridManager.Instance.CurrentSelectedHex != null)
				GridManager.Instance.DeselectHex();
			else
				Menus.Instance.ToggleMainMenu();
		}

		if(Input.GetKeyDown(KeyCode.F2))
		{
			ConfigurationManager.Instance.SaveDefaultConfigurations();
		}

		if(Input.GetKeyDown(KeyCode.F3))
		{
			ConfigurationManager.Instance.ReloadConfigurations();
		}

		if(Menus.Instance.IsMouseOverGUI)
			GridManager.Instance.HoverHex(null); //stop hovering if we are above UI elements
		else
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo, float.MaxValue))
			{
				GridManager.Instance.HoverHex(hitInfo.transform);
				if(Input.GetButton("Select"))
				{
					GridManager.Instance.SelectHex(hitInfo.transform);
				}
			}
		}
	}
}
