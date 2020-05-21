using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
	public MeshRenderer mesh;	// mesh quad with texture of puzzle pieces
	public CellDraw draw;		// helper object to render the texture of the puzzle
	public DC_Cell data;		// a piece of data
	public Vector3 rightPos;	// the coordinates of the right position piece
	public Vector3 curPos;		// the coordinates of the current position of a piece

	// a piece of data installation
	public void SetData(DC_Cell data)
	{
		this.data = data;

		rightPos = new Vector3(data.posX * data.sizeCellX - data.sizeBoardX * data.sizeCellX / 2 + data.sizeCellX / 2,
		                       -data.posY * data.sizeCellY + data.sizeBoardY * data.sizeCellY / 2 - data.sizeCellY / 2, 0);
		
		// the size and displacement of the textures in the mesh
		mesh.transform.localScale = new Vector3(data.sizeCellX * 2, data.sizeCellY * 2, 1);
		mesh.sharedMaterial = new Material(mesh.sharedMaterial);
		mesh.sharedMaterial.SetTextureScale("_MainTex", new Vector2(1f / data.sizeBoardX, 1f / data.sizeBoardY));
		mesh.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(data.posX * 1f / data.sizeBoardX,
		                                                             1 - (data.posY + 1) * 1f / data.sizeBoardY));
	}

	void OnDestroy()
	{
		Destroy(mesh.sharedMaterial);
	}
}

[System.Serializable]
public class DC_Cell
{
	public int posX;		// position X on board
	public int posY;		// position Y on board
	public int sizeBoardX;	// board size X in pieces
	public int sizeBoardY;	// board size Y in pieces
	public float sizePictX;	// picture size X in pixels
	public float sizePictY;	// picture size Y in pixels
	public int sizeCellX;	// cell size X in pixels
	public int sizeCellY;	// cell size Y in pixels
}
