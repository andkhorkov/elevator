using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

//public class DebugDisplay : MonoBehaviour
//{
//    public TextMeshProUGUI currentDir;
//    public TextMeshProUGUI opposite;
//    public TextMeshProUGUI delayed;
//    public TextMeshProUGUI current;

//    public ElevatorController elevator;


//    void Update()
//    {
//        PrintRequests();

//    }

//    private void PrintRequests()
//    {
//        var root = elevator.AlongCabinDirectionRequests.First;
//        var sb = new StringBuilder();

//        while (root != null)
//        {
//            var request = root.Value;
//            sb.Append($"{request.FloorNum}:{request.DesiredDirection},  ");

//            root = root.Next;
//        }

//        currentDir.text = sb.ToString();
//        sb.Clear();

//        root = elevator.OppositeRequests.First;
//        while (root != null)
//        {
//            var request = root.Value;
//            sb.Append($"{request.FloorNum}:{request.DesiredDirection},  ");

//            root = root.Next;
//        }

//        opposite.text = sb.ToString();
//        sb.Clear();

//        root = elevator.DelayedRequests.First;
//        while (root != null)
//        {
//            var request = root.Value;
//            sb.Append($"{request.FloorNum}:{request.DesiredDirection},  ");

//            root = root.Next;
//        }

//        delayed.text = sb.ToString();
//        sb.Clear();

//        root = elevator.CurrentRequests.First;
//        while (root != null)
//        {
//            var request = root.Value;
//            sb.Append($"{request.FloorNum}:{request.DesiredDirection},  ");

//            root = root.Next;
//        }

//        current.text = sb.ToString();
//        sb.Clear();
//    }
//}
