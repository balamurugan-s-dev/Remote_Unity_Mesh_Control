using UnityEngine;
using NativeWebSocket;

public class WebSocketColor : MonoBehaviour
{
    WebSocket ws;
    Renderer r;

    async void Start()
    {
        r = GetComponent<Renderer>();

        ws = new WebSocket("ws://10.120.5.124:3000");

        ws.OnMessage += (bytes) =>
        {
            string msg = System.Text.Encoding.UTF8.GetString(bytes);

            if (msg == "red") r.material.color = Color.red;
            if (msg == "green") r.material.color = Color.green;
            if (msg == "blue") r.material.color = Color.blue;
        };

        await ws.Connect();
    }

    void Update()
    {
        ws.DispatchMessageQueue();
    }

    async void OnApplicationQuit()
    {
        await ws.Close();
    }
}
