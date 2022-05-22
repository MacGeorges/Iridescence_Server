using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class NetworkAvatar : MonoBehaviour
{
    public ClientHandler clientHandlerRef;

    private SphereCollider sphereCollider;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        StartCoroutine(InitSphere());
    }

    IEnumerator InitSphere()
    {
        while(sphereCollider.radius < 0.1)
        {
            sphereCollider.radius += 0.001f;
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Avatar detected : " + other);

        EnvironmentPart environmentPart = other.GetComponentInParent<EnvironmentPart>();

        if (environmentPart)
        {
            clientHandlerRef.Send(RequestType.objectUpdate, JsonUtility.ToJson(environmentPart.partData));
        }
    }
}
