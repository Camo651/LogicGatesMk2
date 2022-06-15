using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveRenderer : MonoBehaviour
{
	public LineRenderer lr;
	public Node node;
	public bool isActive;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
		node = GetComponent<Node>();
	}
	public void RenderCurve(Vector2 a, Vector2 b)
	{
		isActive = true;
		float distance = Vector2.Distance(a, b);
		Vector2 midpoint = Vector2.Lerp(a, b, .5f);
		int halfRes = (int)(distance * 5f);
		if (halfRes == 0)
			return;
		Vector3[] points = new Vector3[(halfRes*2)+1];
		Vector2 x1 = a;
		Vector2 x2 = new Vector2(midpoint.x, a.y);
		Vector2 y1 = x2;
		Vector2 y2 = midpoint;

		for (int i = 0; i < halfRes; i++)
		{
			float t = i / (float)halfRes;
			points[i] = Vector2.Lerp(Vector2.Lerp(x1, x2, t), Vector2.Lerp(y1, y2, t), t);
		}

		x1 = new Vector2(midpoint.x, b.y);
		x2 = b;
		y1 = midpoint;
		y2 = x1;

		for (int i = 0; i <= halfRes; i++)
		{
			float t = i / (float)halfRes;
			points[i+halfRes] = Vector2.Lerp(Vector2.Lerp(y1, y2, t), Vector2.Lerp(x1, x2, t), t);
		}

		lr.positionCount = points.Length;
		lr.SetPositions(points);
		lr.startWidth = node.gate.player.lineWidth;
		lr.endWidth = node.gate.player.lineWidth;
	}

	public void SetCurveColour(Color col)
	{
		lr.startColor = col;
		lr.endColor = col;
	}

	public void ClearCurve()
	{
		lr.positionCount = 0;
		isActive = false;
	}
}
