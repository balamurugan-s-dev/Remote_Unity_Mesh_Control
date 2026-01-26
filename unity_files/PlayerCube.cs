using UnityEngine;
using NativeWebSocket;
using System;
using System.Collections;
using System.Threading.Tasks;

public class PlayerCube : MonoBehaviour
{
    WebSocket ws;
    public string serverIP = "10.120.5.124";
    public int port = 3000;
    bool isConnecting = false;
    Vector3 cachedRotation;

    // actual rotation target
    Quaternion targetRotation;
    bool hasTargetRotation = false;
    public float rotationSpeed = 360f;

    [Serializable]
    public class CommandData
    {
        public string target;
        public string type;
        public string property;

        public string axis;       // x y z
        public float axisValue;   // angle
    }

    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        cachedRotation = Vector3.zero;
        targetRotation = Quaternion.identity;

        await Connect();
    }

    async Task Connect()
    {
        if (isConnecting) return;
        isConnecting = true;

        ws = new WebSocket($"ws://{serverIP}:{port}");

        ws.OnOpen += async () =>
        {
            Debug.Log("WebSocket Connected");
            await Task.Delay(100);
            await ws.SendText("Unity connected");
        };

        ws.OnError += (e) =>
        {
            Debug.Log("WebSocket Error: " + e);
        };

        ws.OnClose += (e) =>
        {
            Debug.Log("Disconnected — reconnecting...");
            isConnecting = false;
            StartCoroutine(Reconnect());
        };

        ws.OnMessage += (bytes) =>
        {
            string msg = System.Text.Encoding.UTF8.GetString(bytes);

            try
            {
                CommandData data =
                    JsonUtility.FromJson<CommandData>(msg);

                ProcessCommand(data);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("JSON error: " + ex.Message);
            }
        };

        try
        {
            await ws.Connect();
        }
        catch
        {
            isConnecting = false;
            StartCoroutine(Reconnect());
        }
    }

    void ProcessCommand(CommandData data)
    {
        if (data.type != "transform") return;
        if (data.property != "rotation") return;

        // update cached axis
        switch (data.axis)
        {
            case "x":
                cachedRotation.x = data.axisValue;
                break;

            case "y":
                cachedRotation.y = data.axisValue;
                break;

            case "z":
                cachedRotation.z = data.axisValue;
                break;
        }

        // reset detection
        if (
            Mathf.Abs(cachedRotation.x) < 0.001f &&
            Mathf.Abs(cachedRotation.y) < 0.001f &&
            Mathf.Abs(cachedRotation.z) < 0.001f
        )
        {
            targetRotation = Quaternion.identity;
        }
        else
        {
            targetRotation = Quaternion.Euler(cachedRotation);
        }

        hasTargetRotation = true;
    }

    void Update()
    {
        ws?.DispatchMessageQueue();

        if (!hasTargetRotation) return;

        transform.localRotation =
            Quaternion.RotateTowards(
                transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(2f);
        ReconnectAsync();
    }

    async void ReconnectAsync()
    {
        await Connect();
    }

    async void OnApplicationQuit()
    {
        if (ws != null)
            await ws.Close();
    }
}
