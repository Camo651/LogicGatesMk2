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



	public void InitNodes()
	{
		BroadcastMessage("Init");
	}

	public void UpdateChipStates()
	{
		switch (chipType)
		{
			case ChipType.Input:

				break;
			case ChipType.Output:

				break;
			case ChipType.And:
				ShellOutputs[0].SetNodeState(ShellInputs[0].state && ShellInputs[1].state);
				break;
			case ChipType.Or:
				ShellOutputs[0].SetNodeState(ShellInputs[0].state || ShellInputs[1].state);
				break;
			case ChipType.Not:
				ShellOutputs[0].SetNodeState(!ShellInputs[0].state);
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
