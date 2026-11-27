using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class QuitApplicationOnLastClientLeave : MonoBehaviour
{

#if UNITY_SERVER
    // The component is not attached in network.unity, so add it to the NetworkManager at startup.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AttachToNetworkManager()
    {
        var nm = NetworkManager.Singleton != null ? NetworkManager.Singleton : FindObjectOfType<NetworkManager>();
        if (nm != null && nm.GetComponent<QuitApplicationOnLastClientLeave>() == null) {
            nm.gameObject.AddComponent<QuitApplicationOnLastClientLeave>();
        }
    }
#endif

    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    private void OnEnable()
    {
#if UNITY_SERVER
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
#endif
    }
    private void OnDisable()
    {
#if UNITY_SERVER
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
#endif
    }

#if UNITY_SERVER
    private void OnClientDisconnect(ulong clientId)
    {
        if (networkManager.ConnectedClientsIds.Count == 1) { // the leaving client is still counted in
            Debug.Log("Last client left the game, shuting down the server");
            //StartCoroutine(StopService());
            Application.Quit(); 
        }
    }

    /*private IEnumerator StopService()
    {
        string url = Environment.GetEnvironmentVariable("ARBITRIUM_SIGTERM_POST_URL");
        string requestId = Environment.GetEnvironmentVariable("ARBITRIUM_REQUEST_ID");

        var request = new UnityWebRequest();
        request.url = url;
        request.method = UnityWebRequest.kHttpVerbPOST;
        //request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes($"{{\"request_id\":\"{requestId}\"}}"));
        //request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 30;
        
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            Debug.Log("Error: " + request.error);
        } else {
            string jsonResponse = Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log(request.responseCode + " " + jsonResponse);
        }
    }*/
#endif
}
