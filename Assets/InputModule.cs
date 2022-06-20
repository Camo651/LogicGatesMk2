using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputModule : MonoBehaviour
{
	public Node node;
	public Chip gate;


	private void Awake()
	{
		gate = GetComponent<Chip>();
	}
	public void OnMouseDown()
	{
		if (node)
		{
			node.ToggleNodeState();
			if (node.nodeType == Node.NodeType.Input)
			{
				node.ClearConnections();
			}
		}
	}
	public void OnMouseDrag()
	{
		if(node)
			node.gate.player.dragging = node;
		if (gate)
			gate.player.draggingChip = gate;
	}

	public void OnMouseEnter()
	{
		if(node)
			node.gate.player.connection = node;
		if (gate)
			gate.player.hoveredChip = gate;
	}

	public void OnMouseExit()
	{
		if (node)
			node.gate.player.connection = null;
		if (gate)
			gate.player.hoveredChip = null;
	}
}
