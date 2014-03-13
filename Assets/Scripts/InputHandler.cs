using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour
{
	private bool isMouseOverGUI = false;

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

		if(isMouseOverGUI == false)
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

	void OnGUI()
	{
		if(Event.current.type == EventType.Repaint && 
		   GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition ))
		{
			isMouseOverGUI = true;
		}
		else
		{
			isMouseOverGUI = false;
		}
	}
}
