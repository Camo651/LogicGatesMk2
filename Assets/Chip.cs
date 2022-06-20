using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour
{
	public Player player;

	public List<Node> ShellInputs = new List<Node>();
	public List<Node> ShellOutputs = new List<Node>();

	public List<Chip> InternalChips = new List<Chip>();
	public Chip parentChip;

	public GameObject ChipObject;
	public GameObject HighlightObject;

	public ChipType chipType;
	public Color chipColour;
	public string chipName, chipInfo;

	public Vector2? tempAnchorPos;

	public bool interactable;

	public void InitNodes()
	{
		BroadcastMessage("Init");
	}

	public void UpdateChipStates()
	{
		switch (chipType)
		{
			case ChipType.Input:
				ShellOutputs[0].SetNodeState(ShellInputs[0].state, null);
				break;
			case ChipType.Output:
				ShellOutputs[0].SetNodeState(ShellInputs[0].state, null);
				break;
			case ChipType.And:
				ShellOutputs[0].SetNodeState(ShellInputs[0].state && ShellInputs[1].state, null);
				break;
			case ChipType.Or:
				ShellOutputs[0].SetNodeState(ShellInputs[0].state || ShellInputs[1].state, null);
				break;
			case ChipType.Not:
				ShellOutputs[0].SetNodeState(!ShellInputs[0].state, null);
				break;
			case ChipType.Chip:

				break;
		}
	}

	public void DeleteChip()
	{
		UpdateNodeCurves(true);
		player.allChips.Remove(this);
		Destroy(gameObject);
	}

	public void UpdateNodeCurves(bool deleteCurves)
	{
		foreach (Node n in ShellInputs)
			if (n)
				if (deleteCurves)
					n.ClearConnections();
				else
					n.UpdateConnectedCurves();
		foreach (Node n in ShellOutputs)
			if (n)
				if (deleteCurves)
					n.ClearConnections();
				else
					n.UpdateConnectedCurves();
	}

	public void SetHighlightState(bool s)
	{

	}

	public void HideChip()
	{
		foreach (Node n in ShellInputs)
			if (n && !n.preventHide)
			{
				n.sr.color = Color.clear;
				n.curveRenderer.SetCurveColour(Color.clear);
				n.curveRenderer.enabled = false;
			}
		foreach (Node n in ShellOutputs)
			if (n && !n.preventHide)
			{
				n.sr.color = Color.clear;
				n.curveRenderer.SetCurveColour(Color.clear);
				n.curveRenderer.enabled = false;
			}
		interactable = false;
		if (GetComponent<SpriteRenderer>())
		{
			GetComponent<SpriteRenderer>().color = Color.clear;
			GetComponent<SpriteRenderer>().enabled = false;
		}
		else
		{
			transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.clear;
			transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	public void SetChipAsInternal()
	{
		interactable = false;
		foreach (Node node in ShellInputs)
		{
			if (node)
			{
				node.sr.enabled = false;
				node.curveRenderer.enabled = false;
				if (node.inMod)
					node.inMod.enabled = false;
			}
		}
		foreach (Node node in ShellOutputs)
		{
			if (node)
			{
				node.sr.enabled = false;
				node.curveRenderer.enabled = false;
				if (node.inMod)
					node.inMod.enabled = false;
			}
		}
	}
	public enum ChipType
	{
		None,
		Input,
		Output,
		And,
		Or,
		Not,
		Chip
	}
}
