using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

[System.Serializable]
public class Message
{
    public float left_x;
    public float left_y;
    public bool left_vis;
    
    public float right_x;
    public float right_y;
    public bool right_vis;

    public Vector2 LWristPos => NormalizeToPos(new Vector2(left_x, left_y));
    public Vector2 RWristPos => NormalizeToPos(new Vector2(right_x, right_y));
    
    public Vector2 LWristPosPixel => NormalizeToPixel(new Vector2(left_x, left_y));
    public Vector2 RWristPosPixel => NormalizeToPixel(new Vector2(right_x, right_y));
    
    Vector2 NormalizeToPos(Vector2 pos)
    {
        float xPos = pos.x * GameManager.Instance.sensitivity * GameManager.Instance.ScreenRatio * -1;
        float yPos = pos.y * GameManager.Instance.sensitivity * -1;

        return new Vector2(xPos, yPos);
    }

    Vector2 NormalizeToPixel(Vector2 pos)
    {
        float pixelX = pos.x * GameManager.Instance.ScreenSize.x;
        float pixelY = pos.y * GameManager.Instance.ScreenSize.y;

        return new Vector2(pixelX, pixelY);
    }
}

public class PythonReceiverManager : MonoBehaviour
{
    public static PythonReceiverManager Instance { get; private set; }

    public int port = 5052;

    public Message LastMessage { get; private set; } = new Message();

    private Thread thread;
    private UdpClient client;
    
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        client = new UdpClient(port);
        
        thread = new Thread(ListenThread);
        thread.IsBackground = true;
        thread.Start();

        Debug.Log($"PythonReceiver listening on port " + port);
    }

    void ListenThread()
    {
        IPEndPoint senderEP = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref senderEP);
                string msg = Encoding.UTF8.GetString(data).Trim();
                messageQueue.Enqueue(msg);
            }
            catch { break; }
        }
    }

    void Update()
    {
        // Main thread - safe to modify Unity and deserialize
        while (messageQueue.TryDequeue(out string json))
        {
            try
            {
                LastMessage = JsonUtility.FromJson<Message>(json);
                // currentMessage.UpdateMessage(lastMessage);
            }
            catch
            {
                Debug.LogWarning("Failed to parse JSON: " + json);
            }
        }
    }

    void OnApplicationQuit()
    {
        client?.Close();
    }
}