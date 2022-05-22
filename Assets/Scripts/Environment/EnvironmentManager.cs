using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public RegionElements regionElements;

    public static EnvironmentManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadEnvironement();
    }

    private void LoadEnvironement()
    {
        CloudWrapper.GetAllRegionElements(SpawnBaseEnvironment);
    }

    private void SpawnBaseEnvironment(string serializedData)
    {
        if(string.IsNullOrEmpty(serializedData)){ return; }
        regionElements = JsonUtility.FromJson<RegionElements>(serializedData);

        foreach (RegionElement element in regionElements.regionElements)
        {
            GameObject regionElement = new GameObject();
            regionElement.name = "regionElement - " + element.elementID;
            EnvironmentPart environmentPart = regionElement.AddComponent<EnvironmentPart>();
            environmentPart.Init(element);
        }
    }
}
