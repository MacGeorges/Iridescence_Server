using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SampleSerializer : MonoBehaviour
{
    public Part samplePart;
    public RegionElements sampleRegionElements;

    private string sampleFolderPath;

    // Start is called before the first frame update
    void Start()
    {
        sampleFolderPath = Application.dataPath + "/SerializedDataSamples/";
        ClearSampleFolder();
        SerializePart();
        SerializeRegionElements();
    }

    private void ClearSampleFolder()
    {
        foreach (string file in Directory.GetFiles(sampleFolderPath))
        {
            File.Delete(file);
        }
    }

    private void SerializePart()
    {
        StreamWriter writer = new StreamWriter(sampleFolderPath + "SerializedpartSample.json", true);
        writer.WriteLine(JsonUtility.ToJson(samplePart));
        writer.Close();
    }

    private void SerializeRegionElements()
    {
        StreamWriter writer = new StreamWriter(sampleFolderPath + "/SerializedRegionElementsSample.json", true);
        writer.WriteLine(JsonUtility.ToJson(sampleRegionElements));
        writer.Close();
    }
}
