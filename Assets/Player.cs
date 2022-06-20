using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Node dragging, connection;
	public Chip draggingChip, hoveredChip;
	public Camera mainCam;

	public List<GameObject> mainGatePrefabs;
	public GameObject inputModulePrefab;
	public List<Chip> allChips = new List<Chip>();

	public float lineWidth;
	public Color onStateColour, offStateColour;

	public Vector2 selectionBoxDragStart, selectionBoxDragEnd;
	public Vector2? selectionBoxMovementAnchor;
	public GameObject selectionBox;
	public List<Chip> selectedChips = new List<Chip>();
	public bool draggingSelectionBox;
	public bool itemsAreaSelected;
	public float scrollSpeed;
	public Vector2 scrollScaleLim;
	public GameObject complexChipPrefab, blankNodePrefab;

	private void Update()
	{
		mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize + (-scrollSpeed * Input.mouseScrollDelta.y * Time.deltaTime), scrollScaleLim.x, scrollScaleLim.y);

		Vector2 mousePos = (Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition);
		if (draggingChip)
		{
			draggingChip.transform.position = mousePos;
			draggingChip.UpdateNodeCurves(false);
			if (Input.GetMouseButtonUp(0))
			{
				draggingChip = null;
			}
		}
		if (dragging)
		{
			dragging.SetLineToPoint(mousePos);
			if (Input.GetMouseButtonUp(0))
			{
				dragging.curveRenderer.ClearCurve();
				if (connection)
				{
					connection.curveRenderer.ClearCurve();
					if((dragging.nodeType == Node.NodeType.Input && connection.nodeType == Node.NodeType.Output)|| (dragging.nodeType == Node.NodeType.Output && connection.nodeType == Node.NodeType.Input))
					{
						dragging.ConnectNode(connection);
						connection.ConnectNode(dragging);
						dragging.SetNodeCurveColor();
						connection.SetNodeCurveColor();
					}
				}
				dragging = null;
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			if(hoveredChip)
				hoveredChip.DeleteChip();
			selectionBoxMovementAnchor = null;
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (!hoveredChip && !connection)
			{
				draggingSelectionBox = true;
				selectionBoxDragStart = mousePos;
				selectionBox.SetActive(true);
			}
			if (itemsAreaSelected && !PointIsInBounds(mousePos, selectionBoxDragStart, selectionBoxDragEnd))
			{
				itemsAreaSelected = false;
				selectionBox.SetActive(false);
				selectedChips.Clear();
			}
			if(itemsAreaSelected && PointIsInBounds(mousePos, selectionBoxDragStart, selectionBoxDragEnd))
			{
				selectionBoxMovementAnchor = mousePos;
				foreach (Chip c in selectedChips)
				{
					c.tempAnchorPos = c.transform.position;
				}
			}
		}
		if (Input.GetMouseButton(0))
		{
			if (draggingSelectionBox)
			{
				selectionBoxDragEnd = mousePos;
				SetSelectionBox(selectionBoxDragStart, selectionBoxDragEnd);
			}
			else if (itemsAreaSelected && selectionBoxMovementAnchor != null)
			{
				Vector2 offset = mousePos - ((Vector2)selectionBoxMovementAnchor);
				SetSelectionBox(selectionBoxDragStart + offset, selectionBoxDragEnd + offset);
				foreach(Chip c in selectedChips)
				{
					if (c.tempAnchorPos != null)
					{
						c.transform.position = ((Vector2)c.tempAnchorPos) + offset;
						c.UpdateNodeCurves(false);
					}
				}
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			draggingSelectionBox = false;
			if (!itemsAreaSelected)
			{
				selectionBox.SetActive(false);
				selectedChips.Clear();
			}
			if (selectionBoxMovementAnchor != null)
			{
				Vector2 offset = mousePos - ((Vector2)selectionBoxMovementAnchor);
				selectionBoxDragStart += offset;
				selectionBoxDragEnd += offset;
				selectionBoxMovementAnchor = null;
			}
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
			CreateChip(0);
		if (Input.GetKeyDown(KeyCode.Alpha2))
			CreateChip(1);
		if (Input.GetKeyDown(KeyCode.Alpha3))
			CreateChip(2);
		if (Input.GetKeyDown(KeyCode.Alpha4))
			CreateChip(3);
		if (Input.GetKeyDown(KeyCode.Alpha5))
			CreateChip(4);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (itemsAreaSelected)
			{
				CompileComplexChip();
			}
		}
	}

	public void CompileComplexChip()
	{
		List<Chip> internals = selectedChips;
		List<Chip> inputs = new List<Chip>();
		List<Chip> outputs = new List<Chip>();
		foreach (Chip c in internals)
		{
			if (c.chipType == Chip.ChipType.Input)
				inputs.Add(c);
			else if (c.chipType == Chip.ChipType.Output)
				outputs.Add(c);
		}
		int iCount = inputs.Count;
		int oCount = outputs.Count;
		if (iCount == 0 || oCount == 0)
			return;
		float sideSize = Mathf.Clamp(Mathf.Max(iCount, oCount) / 2f, 1.5f, 100);

		Chip comp = Instantiate(complexChipPrefab).GetComponent<Chip>();
		allChips.Add(comp);
		comp.InternalChips = internals;
		comp.transform.GetChild(0).localScale = new Vector3(1.5f, sideSize, 1f);
		comp.transform.GetComponent<BoxCollider>().size = new Vector3(1.5f, sideSize, 1f);
		comp.player = this;
		comp.interactable = true;
		foreach (Chip c in internals)
		{
			c.parentChip = comp;
			c.HideChip();
			c.transform.SetParent(comp.transform);
			c.transform.localPosition = Vector3.zero;
			c.interactable = false;
			c.GetComponent<Collider>().enabled = false;
		}
		for (int i = 0; i < iCount; i++)
		{
			Node n = inputs[i].ShellInputs[0];
			inputs[i].ShellOutputs[0].inMod.GetComponent<Collider>().enabled = false;
			n.inMod.GetComponent<Collider>().enabled = true;
			n.inMod.GetComponent<Collider>().transform.name = "aa";
			float spacing = (sideSize) / iCount;
			float offset = (spacing * (iCount / 2f));
			float y = (spacing * -i) + offset - .25f;
			n.transform.position = new Vector3(-.75f, y, 0);
			n.sr.color = Color.black;
			n.curveRenderer.SetCurveColour(Color.black);
			comp.ShellInputs.Add(n);
		}
		for (int i = 0; i < oCount; i++)
		{
			Node n = outputs[i].ShellOutputs[0];
			outputs[i].ShellInputs[0].inMod.GetComponent<Collider>().enabled = false;
			n.inMod.GetComponent<Collider>().enabled = true;
			float spacing = (sideSize) / oCount;
			float offset = (spacing * (oCount / 2f));
			float y = (spacing * -i) + offset - .25f;
			n.transform.position = new Vector3(.75f, y, 0);
			n.sr.color = Color.black;
			n.curveRenderer.SetCurveColour(Color.black);
			comp.ShellOutputs.Add(n);
		}
		//comp.InitNodes();
		comp.UpdateChipStates();

		draggingSelectionBox = false;
		selectionBox.SetActive(false);
		selectedChips.Clear();
	}

	public void CreateChip(int gateIndex)
	{
		Chip gate = Instantiate(mainGatePrefabs[gateIndex]).GetComponent<Chip>();
		gate.transform.position = ((Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition));
		gate.player = this;
		gate.InitNodes();
		gate.UpdateChipStates();
		gate.interactable = true;
		allChips.Add(gate);
	}

	public void SetSelectionBox(Vector2 s, Vector2 e)
	{
		Vector2 centre = Vector2.Lerp(s, e, .5f);
		Vector2 size = e - s;
		selectionBox.transform.position = centre;
		selectionBox.transform.localScale = size;

		selectedChips.Clear();
		foreach(Chip gate in allChips)
		{
			if(gate && gate.interactable && PointIsInBounds(gate.transform.position, s, e))
			{
				selectedChips.Add(gate);
			}
		}
		itemsAreaSelected = selectedChips.Count > 0;
	}

	public bool PointIsInBounds(Vector2 point, Vector2 a, Vector2 b)
	{
		bool x = point.x >= Mathf.Min(a.x, b.x) && point.x <= Mathf.Max(a.x, b.x);
		bool y = point.y >= Mathf.Min(a.y, b.y) && point.y <= Mathf.Max(a.y, b.y);
		return x && y;
	}
}
