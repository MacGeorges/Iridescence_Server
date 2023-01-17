using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class NetworkAvatar : MonoBehaviour
{
    public ClientHandler clientHandlerRef;

    private SphereCollider sphereCollider;

    public bool shouldInit = false;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        shouldInit = true;
    }

    private void Update()
    {
        if(shouldInit)
        {
            shouldInit = false;
            StartCoroutine(InitSphere());
        }
    }

    IEnumerator InitSphere()
    {
        sphereCollider.radius = 0;
        while (sphereCollider.radius < 10)
        {
            sphereCollider.radius += 0.01f;
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Avatar detected : " + other);

        EnvironmentPart environmentPart = other.GetComponentInParent<EnvironmentPart>();

        if (environmentPart)
        {
            NetworkRequest request = new NetworkRequest();
            request.sender = ServerManager.instance.server;
            request.requestType = RequestType.objectUpdate;

            ObjectRequest objectRequest = new ObjectRequest();
            objectRequest.requestType = ObjectRequestType.add;
            objectRequest.element = environmentPart.partData;

            request.serializedRequest = JsonUtility.ToJson(objectRequest);

            clientHandlerRef.Send(request);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Avatar leaving : " + other);

        EnvironmentPart environmentPart = other.GetComponentInParent<EnvironmentPart>();

        if (environmentPart)
        {
            NetworkRequest request = new NetworkRequest();
            request.sender = ServerManager.instance.server;
            request.requestType = RequestType.objectUpdate;

            ObjectRequest objectRequest = new ObjectRequest();
            objectRequest.requestType = ObjectRequestType.remove;
            objectRequest.element = environmentPart.partData;

            request.serializedRequest = JsonUtility.ToJson(objectRequest);

            clientHandlerRef.Send(request);
        }
    }
}
