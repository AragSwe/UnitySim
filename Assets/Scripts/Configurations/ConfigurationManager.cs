using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System;

public class ConfigurationManager
{
	public static ConfigurationManager Instance
	{ get { if(_instance == null) _instance = new ConfigurationManager(); return _instance; } }
	private static ConfigurationManager _instance;
	private const string CONFIGURATIONFOLDER = "/Configurations/";

	public Dictionary<string, ResourceType> ResourceTypes = new Dictionary<string, ResourceType>();
	public Dictionary<string, TerrainType> TerrainTypes = new Dictionary<string, TerrainType>();

	private ConfigurationManager()
	{
		if(Directory.Exists(GameManager.GetApplicationPath() + CONFIGURATIONFOLDER) == false)
			Directory.CreateDirectory(GameManager.GetApplicationPath() + CONFIGURATIONFOLDER);
		ReloadConfigurations();
	}

	public void ReloadConfigurations()
	{
		LoadResources();
		LoadTerrainTypes();
	}

	void LoadResources ()
	{
		ResourceTypes.Clear();
		List<ResourceType> lr = null;
		try
		{
			using(FileStream fs = new FileStream(GameManager.GetApplicationPath() + CONFIGURATIONFOLDER + "Resources.xml", FileMode.Open))
			{
				XmlSerializer xml = new XmlSerializer(typeof(List<ResourceType>));
				lr = (List<ResourceType>)xml.Deserialize(fs);
			}
			if(lr != null)
			{
				foreach(ResourceType r in lr)
					ResourceTypes[r.UniqueName] = r;
			}
		}
		catch(IOException ex)
		{
			Debug.LogError("Could not load Resources.xml. Reason: " + ex.Message);
		}
		catch(Exception ex)
		{
			Debug.LogError("Could not create resources. Reason: " + ex.Message);
		}
	}

	void LoadTerrainTypes ()
	{
		TerrainTypes.Clear();
		List<TerrainType> lr = null;
		try
		{
			using(FileStream fs = new FileStream(GameManager.GetApplicationPath() + CONFIGURATIONFOLDER + "TerrainTypes.xml", FileMode.Open))
			{
				XmlSerializer xml = new XmlSerializer(typeof(List<TerrainType>));
				lr = (List<TerrainType>)xml.Deserialize(fs);
			}
			if(lr != null)
			{
				foreach(TerrainType r in lr)
					TerrainTypes[r.UniqueName] = r;
			}
		}
		catch(IOException ex)
		{
			Debug.LogError("Could not load TerrainTypes.xml. Reason: " + ex.Message);
		}
		catch(Exception ex)
		{
			Debug.LogError("Could not create terrain types. Reason: " + ex.Message);
		}
	}

	public void SaveDefaultConfigurations()
	{
		List<ResourceType> Resources = new List<ResourceType>();
		Resources.Add(new ResourceType("wood", "Chopped wood"));
		Resources.Add(new ResourceType("water", "Fresh water"));
		Resources.Add(new ResourceType("fish", "Food (fish)"));
		Resources.Add(new ResourceType("grain", "Food (grain)"));
		Resources.Add(new ResourceType("berry", "Food (berries)"));
		Resources.Add(new ResourceType("stone", "Cut stone"));
		
		using(TextWriter tw = new StreamWriter(GameManager.GetApplicationPath() + CONFIGURATIONFOLDER + "Resources.xml", false))
		{
			XmlSerializer xml = new XmlSerializer(typeof(List<ResourceType>));
			xml.Serialize(tw, Resources);
		}

		List<TerrainType> TerrainTypes = new List<TerrainType>();
		TerrainTypes.Add(new TerrainType("woods", "Forest", 
		                                 new List<TerrainType.ResourceInfo>() { new TerrainType.ResourceInfo("wood", 1.0f) }
		));

		TerrainTypes.Add(new TerrainType("plains", "Plains", 
		                                 new List<TerrainType.ResourceInfo>() { new TerrainType.ResourceInfo("berry", 0.3f) }
		));
		TerrainTypes.Add(new TerrainType("mountains", "Mountains", 
		                                 new List<TerrainType.ResourceInfo>() { new TerrainType.ResourceInfo("stone", 0.8f) }
		));
		TerrainTypes.Add(new TerrainType("water", "Rivers", 
			                                 new List<TerrainType.ResourceInfo>() { 
												new TerrainType.ResourceInfo("water", 1.0f),
												new TerrainType.ResourceInfo("fish", 0.4f) }
		));
		
		using(TextWriter tw = new StreamWriter(GameManager.GetApplicationPath() + CONFIGURATIONFOLDER + "TerrainTypes.xml", false))
		{
			XmlSerializer xml = new XmlSerializer(typeof(List<TerrainType>));
			xml.Serialize(tw, TerrainTypes);
		}
	}
}
