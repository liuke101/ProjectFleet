using System;
using System.Collections;
using UnityEngine;
using Best.WebSockets;
using JsonStruct;
using LitJson;
using UnityEngine.Events;

public class WebSocketConsumer : MonoSingleton<WebSocketConsumer>
{
    [Header("WebSocket")]
    public string URL = "ws://ditto:ditto@10.151.1.109:8080/ws/2";
    public string SubscribeMessage = "START-SEND-EVENTS?filter=like(thingId,\"edu.whut.cs.dw.sail:ship*\")";
    public string SubscribeACK = "START-SEND-EVENTS:ACK";
    private WebSocket WebSocket;
    
    
    
    // 连接协程
    private Coroutine SubscriptionCoroutine;
    
    [Header("事件")]
    // 接收到船只数据时进行广播
    public UnityEvent<ShipWebSocketData> OnShipWebSocketMessageReceived;
    
    private void Start()
    {
        WebSocket = new WebSocket(new Uri(URL));
        
        // 绑定 WebSocket 委托
        if (WebSocket != null)
        {
            WebSocket.OnOpen += OnWebSocketOpen;
            WebSocket.OnMessage += OnMessageReceived; //当接收到消息时调用
            WebSocket.OnClosed += OnWebSocketClosed;
            WebSocket.Open();
        }

        Test();
    }

    private void Test()
    {
        // 本地测试
        //读取Resources文件夹下的TestData.json文件
        // string jsonStr = Resources.Load<TextAsset>("Data/TestData").text;
        // ShipWebSocketData data = JsonParser(jsonStr);
        // if(data != null)
        // {
        //     OnShipWebSocketMessageReceived?.Invoke(data);
        //     MessageBox.Instance.PrintShipWebSocketData(data);
        // }
    }

    private void OnDestroy()
    {
        WebSocket.Close();
    }

    //开关UI
    public void SwitchState(bool isOpen)
    {
        if (isOpen)
        {
            WebSocket.Open();
        }
        else
        {
            WebSocket.Close();
        }
    }
    
    private void OnWebSocketOpen(WebSocket webSocket) 
    {
        MessageBox.Instance.PrintMessage("WebSocket 开启成功");
        // 开启订阅协程
        SubscriptionCoroutine = StartCoroutine(Subscription());
    }
    
    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        Debug.Log(message);
        
        // 如果接收到ACK，停止协程
        if(message == SubscribeACK) 
        {
            MessageBox.Instance.PrintMessage("WebSocket 订阅成功, 关闭协程");
            StopCoroutine(SubscriptionCoroutine);
        }
        //如果是json 船只数据
        else if(message.Contains("ship")) 
        {
            //将json解析为项目所用数据结构
            ShipWebSocketData data = JsonParser(message);
             
            //广播有效数据
             if(data != null)
             {
                 OnShipWebSocketMessageReceived?.Invoke(data);
                 MessageBox.Instance.PrintShipWebSocketData(data);
             }
        }
    }
    
    private void OnWebSocketClosed(WebSocket webSocket, WebSocketStatusCodes code, string message)
    {
        MessageBox.Instance.PrintMessage("WebSocket 正在关闭");
    
        if (code == WebSocketStatusCodes.NormalClosure)
        {
            MessageBox.Instance.PrintMessage("WebSocket 关闭成功");
        }
        else 
        {
            // Error
            MessageBox.Instance.PrintMessage("WebSocket 发生了错误" + code);
        }
    }

    //将json解析为项目所用格式
    private ShipWebSocketData JsonParser(string message)
    {
        MessageBox.Instance.PrintMessage("解析Json数据");
        
        // 解析 JSON 数据
        JsonData jsonData = JsonMapper.ToObject(message);

        // 提取特定字段
        ShipWebSocketData WebSocketData = new ShipWebSocketData
        {
            speed = (double)jsonData["value"]["features"]["speed"]["properties"]["value"],
            heading = (double)jsonData["value"]["features"]["heading"]["properties"]["value"],
            x_coordinate = (double)jsonData["value"]["features"]["x_coordinate"]["properties"]["value"],
            y_coordinate = (double)jsonData["value"]["features"]["y_coordinate"]["properties"]["value"],
            ship_id = (int)jsonData["value"]["features"]["ship_id"]["properties"]["value"],
            des_x_coordinate = (double)jsonData["value"]["features"]["des_x_coordinate"]["properties"]["value"],
            des_y_coordinate = (double)jsonData["value"]["features"]["des_y_coordinate"]["properties"]["value"],
        };
        
        // if(WebSocketData != null)
        // {
        //     MessageBox.Instance.PrintMessage("Json解析成功");
        // }
        // else
        // {
        //     MessageBox.Instance.PrintMessage("Json解析失败");
        // }
        
        return WebSocketData;
    }
    
    //Send发送订阅等待connecting
    IEnumerator Subscription()
    {
        while (true)
        {
            //如果未连接，持续发送订阅消息
            if (WebSocket.IsOpen)
            {
                WebSocket.Send(SubscribeMessage);
                MessageBox.Instance.PrintMessage("发送 WebSocket 订阅消息");
            }
            
            yield return new WaitForSeconds(1.0f);
        }
    }
}
