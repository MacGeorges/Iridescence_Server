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
}
