//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Gate : MonoBehaviour
//{
//	public Player player;
//	public Node input1, input2;
//	public Node output;

//	public enum GateType
//	{
//		Input,
//		Output,
//		And,
//		Or,
//		Not
//	};
//	public GateType gateType;

//	public void InitNodes()
//	{
//		BroadcastMessage("Init");
//	}

//	public void UpdateGateState()
//	{
//		switch (gateType)
//		{
//			case GateType.And:
//				output.SetNodeState(input1.state && input2.state);
//				break;
//			case GateType.Or:
//				output.SetNodeState(input1.state || input2.state);
//				break;
//			case GateType.Not:
//				output.SetNodeState(!input1.state);
//				break;
//		}
//	}


//}
