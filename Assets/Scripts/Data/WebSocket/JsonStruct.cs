using System;

namespace JsonStruct
{
    [Serializable]
    public class ShipWebSocketData
    {
        public ShipWebSocketData()
        { }
        
        public double speed;
        public double heading; //转向角度
        public double x_coordinate; //生成位置
        public double y_coordinate; 
        public int ship_id;
        public double des_x_coordinate; //目的地
        public double des_y_coordinate;
    }
}

// {
//     "topic": "edu.whut.cs.dw.sail/ship2/things/twin/events/modified",
//     "headers": {
//         "correlation-id": "36b02124-869c-431b-8668-d78c01ee4294",
//         "authorization": "Basic ZGl0dG86ZGl0dG8=",
//         "x-real-ip": "172.18.0.1",
//         "x-forwarded-user": "ditto",
//         "x-ditto-pre-authenticated": "nginx:ditto",
//         "postman-token": "5759e22c-82c0-4e9f-928d-75e51cf339ae",
//         "host": "10.151.1.109:8080",
//         "x-forwarded-for": "172.18.0.1",
//         "accept": "*/*",
//         "user-agent": "PostmanRuntime/7.39.0",
//         "ditto-originator": "nginx:ditto",
//         "response-required": false,
//         "version": 2,
//         "requested-acks": [],
//         "content-type": "application/json"
//     },
//     "path": "/",
//     "value": {
//         "thingId": "edu.whut.cs.dw.sail:ship2",
//         "policyId": "ditto.default:policy",
//         "features": {
//             "speed": {
//                 "properties": {
//                     "value": 0
//                 }
//             },
//             "heading": {
//                 "properties": {
//                     "value": 90
//                 }
//             },
//             "x_coordinate": {
//                 "properties": {
//                     "value": 0
//                 }
//             },
//             "y_coordinate": {
//                 "properties": {
//                     "value": 0
//                 }
//             },
//             "ship_id": {
//                 "properties": {
//                     "value": 10001
//                 }
//             },
//             "des_x_coordinate": {
//                 "properties": {
//                     "value": 0
//                 }
//             },
//             "des_y_coordinate": {
//                 "properties": {
//                     "value": 0
//                 }
//             }
//         }
//     },
//     "revision": 3,
//     "timestamp": "2024-06-28T07:04:27.306736324Z"
// }