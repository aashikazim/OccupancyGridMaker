using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(UnityMainThreadDispatcher))]
public class UDPListener : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;

    public int port = 8080;

    //WorldOffsetController controller;

    string receivedData = "";

    public void Start()
    {
        //controller = GetComponent<WorldOffsetController>();
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                receivedData = Encoding.UTF8.GetString(data);
                //Debug.Log(receivedData);
                RobotPose robotPose = JsonUtility.FromJson<RobotPose>(receivedData);
                //Debug.Log(robotPose);

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    //controller.UpdateRobotPose(robotPose.x, robotPose.y, robotPose.theta)
                    UtilityFunctions.UpdateRobotPose(robotPose)
                ) ;
            }
            catch (Exception err)
            {
                Debug.Log(err.ToString());
            }
        }
    }

    void OnDestroy()
    {
        if (receiveThread != null)
            receiveThread.Abort();
        client.Close();
    }
}