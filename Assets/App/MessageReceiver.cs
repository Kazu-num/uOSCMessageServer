using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uOSC;

[RequireComponent(typeof(uOscServer))]
public class MessageReceiver : MonoBehaviour
{
    uOscServer server;

    [SerializeField]
    bool messageLog = false;

    [SerializeField]
    int messagePoolLength = 100;

    LinkedList<Message> messageList = new LinkedList<Message>();

    bool stop = false;

    void Awake()
    {
        server = GetComponent<uOscServer>();
        server.onDataReceived.AddListener(OnDataReceived);

        messageList.Clear();
    }

    void OnDataReceived(Message message)
    {
        if (stop) { return; }

        if (messageList.Count >= messagePoolLength)
        {
            messageList.RemoveFirst();
        }

        messageList.AddLast(message);

        if (messageLog)
        {
            Debug.Log(message.ToString());
        }
    }

    public void SaveMessagesAsTxt()
    {
        stop = true;

        string name = "Messages_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        string path = System.IO.Path.Combine(Application.dataPath, name);
        Debug.Log(path);

        var sb = new System.Text.StringBuilder();
        foreach(var msg in messageList)
        {
            sb.AppendLine(msg.ToString());
        }

        using (var sw = new System.IO.StreamWriter(path,false,System.Text.Encoding.UTF8))
        {
            sw.Write(sb.ToString());
        }

        messageList.Clear();
        stop = false;

    }

    private void OnGUI()
    {
        if (GUILayout.Button( "save messages"))
        {
            try
            {
                SaveMessagesAsTxt();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
            finally
            {
                Debug.Log("save tried.");
            }
        }
    }
}
