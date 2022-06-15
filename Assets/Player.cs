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
	public GameObject selectionBox;
	public List<Chip> selectedChips = new List<Chip>();
	public bool draggingSelectionBox;
	public bool itemsAreaSelected;

	private void Update()
	{

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
					}
				}
				dragging = null;
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			hoveredChip.DeleteChip();
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (!hoveredChip && !connection)
			{
				draggingSelectionBox = true;
				selectionBoxDragStart = mousePos;
				selectionBox.SetActive(true);
			}
			if (itemsAreaSelected && !PointIsInBounds(mousePos))
			{
				itemsAreaSelected = false;
				selectionBox.SetActive(false);
				selectedChips.Clear();
			}
		}
		if (Input.GetMouseButton(0))
		{
			if (draggingSelectionBox)
			{
				selectionBoxDragEnd = mousePos;
				SetSelectionBox();
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
	}

	public void CreateChip(int gateIndex)
	{
		Chip gate = Instantiate(mainGatePrefabs[gateIndex]).GetComponent<Chip>();
		gate.transform.position = ((Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition));
		gate.player = this;
		gate.InitNodes();
		gate.UpdateChipStates();
		allChips.Add(gate);
	}

	public void SetSelectionBox()
	{
		Vector2 centre = Vector2.Lerp(selectionBoxDragStart, selectionBoxDragEnd, .5f);
		Vector2 size = selectionBoxDragEnd - selectionBoxDragStart;
		selectionBox.transform.position = centre;
		selectionBox.transform.localScale = size;

		selectedChips.Clear();
		foreach(Chip gate in allChips)
		{
			if(gate && PointIsInBounds(gate.transform.position))
			{
				selectedChips.Add(gate);
			}
		}
		itemsAreaSelected = selectedChips.Count > 0;
	}

	public bool PointIsInBounds(Vector2 point)
	{
		Vector2 a = selectionBoxDragStart;
		Vector2 b = selectionBoxDragEnd;
		bool x = point.x >= Mathf.Min(a.x, b.x) && point.x <= Mathf.Max(a.x, b.x);
		bool y = point.y >= Mathf.Min(a.y, b.y) && point.y <= Mathf.Max(a.y, b.y);
		return x && y;
	}
}
