using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
	public enum NodeType
	{
		Input,Output
	}
	public NodeType nodeType;
	public List<Node> connections = new List<Node>();
	public CurveRenderer curveRenderer;
	public SpriteRenderer sr;
	public InputModule inMod;
	public Chip gate;
	public bool state;
	public bool preventHide;
	public void Init()
	{
		state = false;
		gate = GetComponentInParent<Chip>();
		sr = GetComponent<SpriteRenderer>();
		GameObject a = Instantiate(gate.player.inputModulePrefab, transform);
		inMod = a.GetComponent<InputModule>();
		inMod.node = this;
		if (preventHide)
			inMod.GetComponent<Collider>().enabled = false;
	}

	public void SetNodeState(bool s, Chip or)
	{
		if (or == gate)
			return;
		state = s;
		SetNodeCurveColor();
		if (nodeType == NodeType.Output)
		{
			if (connections.Count > 0)
			{
				foreach(Node c in connections)
					c.SetNodeState(s, gate);
			}
		}
		else if(nodeType == NodeType.Input)
		{
			gate.ShellInputs[0].state = s;
			gate.UpdateChipStates();
		}
	}

	public void SetNodeCurveColor()
	{
		if (!preventHide && !gate.interactable)
			return;
		sr.color = state ? gate.player.onStateColour : gate.player.offStateColour;
		curveRenderer.SetCurveColour(state ? gate.player.onStateColour : gate.player.offStateColour);
	}


	public void ToggleNodeState()
	{
		if(nodeType == NodeType.Output && gate.chipType == Chip.ChipType.Input)
			SetNodeState(!state, null);
	}

	public void SetLineToPoint(Vector2 point)
	{
		curveRenderer.RenderCurve(transform.position, point);
	}

	public void ConnectNode(Node other)
	{
		if(nodeType == NodeType.Input)
		{
			foreach(Node n in connections)
			{
				n.connections.Remove(this);
				curveRenderer.ClearCurve();
			}
			connections.Add(other);
			curveRenderer.RenderCurve(transform.position, other.transform.position);
		}
		else
		{
			connections.Add(other);
		}
	}

	public void UpdateConnectedCurves()
	{
		if (nodeType == NodeType.Input)
		{
			if(connections.Count>0 && connections[0])
				curveRenderer.RenderCurve(transform.position, connections[0].transform.position);
		}
		else
		{
			foreach (Node node in connections)
			{
				if (node.curveRenderer.isActive)
				{
					if(node)
						node.curveRenderer.RenderCurve(transform.position, node.transform.position);
				}
			}
		}
	}

	public void ClearConnections()
	{
		if (nodeType == NodeType.Input)
		{
			if (connections.Count > 0 && connections[0])
				curveRenderer.ClearCurve();
		}
		else
		{
			foreach (Node node in connections)
			{
				if (node.curveRenderer.isActive)
				{
					if (node)
						node.curveRenderer.ClearCurve();
				}
			}
		}
		foreach (Node n in connections)
		{
			if(n)
				n.connections.Remove(this);
		}
		connections.Clear();
	}
}
