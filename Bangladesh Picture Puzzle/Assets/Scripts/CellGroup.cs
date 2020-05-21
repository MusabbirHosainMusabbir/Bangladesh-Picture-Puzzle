using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGroup : MonoBehaviour
{
	public int index;			// group index
	public List<Cell> cells;	// pieces in the group list
	public bool setted;			// group is set to the correct position

	// add a piece to the group
	public void AddCell(Cell cell)
	{
		cells.Add(cell);
		cell.transform.SetParent(transform);
		Vector3 pos = cell.transform.localPosition;
		pos.z = 0;
		cell.transform.localPosition = pos;
	}

	// adding a few pieces of the group
	public void AddCells(List<Cell> addCells)
	{
		foreach (Cell cell in addCells)
		{
			AddCell(cell);
		}
	}

	// moving groups according to the index layer
	public void UpdateIndex()
	{
		if(setted)
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -1);
		else
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -index * 2);
	}

	public void UpdateCellPos()
	{
		foreach (Cell cell in cells)
		{
			cell.curPos = cell.transform.localPosition + transform.localPosition;
			cell.curPos.z = 0;
		}
	}

	// attaching groups to the correct position
	public void SetToRightPosition(Vector3 delta)
	{
		setted = true;
		transform.localPosition += delta;
		foreach (Cell cell in cells)
		{
			cell.mesh.GetComponent<BoxCollider>().enabled = false;
		}
	}

	// setting group in a position to link to another group
	public void SetToLinkPosition(Vector3 delta)
	{
		transform.localPosition += delta;
		foreach (Cell cell in cells)
		{
			cell.curPos += delta;
		}
	}

}
