using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Board_Mosaic : MonoBehaviour
{
	public Transform rootCells;		// root for the puzzle pieces
	public Transform pictBack;		// mesh for substrate image
	public Transform pictOver;		// mesh for the final image
	public CellGroup prefGroup;		// prefab puzzle piece
	public Camera camDraw;			// camera for the preparation of the puzzle pieces
	public Dictionary<int, CellGroup> groups;	// list of puzzle pieces
	
	private int sizeBoardX;			// the number of pieces on the side X
	private int sizeBoardY;			// the number of pieces on the side Y
	private bool guiOver;			// click in the interface element
	private CellGroup curGroup;		// the current piece
	private Vector3 mousePos;		// the current position of the mouse
	private int sizeCellX;			// chunk-size X in pixels
	private int sizeCellY;			// chunk-size Y in pixels
	private RenderTexture rTex;		// RenderTexture to generate pieces
	private Texture2D texPuzzle;	// texture-generated puzzle pieces
	private float dockValue;		// distance to the correct position in pixels

	private CellGroup curGroup2;
	private Vector3 targetPos;
	private Vector3 targetPos2;

	private bool moveMouse;
	private bool moveFree;
	private bool gameover;

	void Awake()
	{
		pictOver.gameObject.SetActive(false);
		prefGroup.gameObject.SetActive(false);
	}

	public void SetData()
	{
		sizeBoardX = Game.piecesX;
		sizeBoardY = Game.piecesY;
		
		pictBack.localScale = new Vector3(Game.image.texture.width, Game.image.texture.height, 1);
		pictOver.localScale = pictBack.localScale;
		
		sizeCellX = (int)pictBack.localScale.x / sizeBoardX;
		sizeCellY = (int)pictBack.localScale.y / sizeBoardY;
		dockValue = Mathf.Min(sizeCellX, sizeCellY) / 2;

		// load the picture into a piece that will be cloned
#if UNITY_WEBGL
		Texture2D tex = new Texture2D(Game.image.texture.width, Game.image.texture.height, TextureFormat.ARGB32, false);
		tex.SetPixels(Game.image.texture.GetPixels());
		tex.Apply();
		TextureScale.Bilinear(tex, 1024, 1024);
		tex.wrapMode = TextureWrapMode.Repeat;
		prefGroup.cells [0].draw.mesh.sharedMaterial.mainTexture = tex;
#else
		prefGroup.cells [0].draw.mesh.sharedMaterial.mainTexture = Game.image.texture;
#endif

		// expose the camera settings and RenderTexture to generate the texture of puzzle pieces
		camDraw.orthographicSize = (int)pictBack.localScale.y;
		rTex = new RenderTexture((int)pictBack.localScale.x * 2, (int)pictBack.localScale.y * 2, 0, RenderTextureFormat.ARGB32);
//		rTex.antiAliasing = 1;
		camDraw.targetTexture = rTex;

		// remove a picture from the substrate, if it was there
		pictBack.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = null;
		// add the final image
		pictOver.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Game.image.texture;

		// creating pieces of the puzzle
		BoardCreate();
	}

	void BoardCreate()
	{
		groups = new Dictionary<int, CellGroup>();
		Lib.RemoveObjects(rootCells);

		// create groups for each piece and places them in the starting position
		for (int y=0; y<sizeBoardY; y++)
		{
			for (int x=0; x<sizeBoardX; x++)
			{
				CellGroup group = Lib.AddObject<CellGroup>(prefGroup, rootCells, true);
				group.index = x + y * sizeBoardX;
				Cell cell = group.cells[0];
				
				DC_Cell data = new DC_Cell();
				data.posX = x;
				data.posY = y;
				data.sizeBoardX = sizeBoardX;
				data.sizeBoardY = sizeBoardY;
				data.sizePictX = pictBack.localScale.x;
				data.sizePictY = pictBack.localScale.y;
				data.sizeCellX = sizeCellX;
				data.sizeCellY = sizeCellY;
				
				cell.SetData(data);
				cell.draw.SetData(data);
				group.UpdateIndex();
				groups.Add(group.index, group);
				cell.mesh.gameObject.SetActive(false);
			}
		}

		// generates texture with pieces of puzzle
		StartCoroutine(DrawingCells());
	}

	// texture generation of puzzle pieces
	IEnumerator DrawingCells()
	{
		camDraw.enabled = true;
		
		// remove the texture, if was generated earlier
		if(texPuzzle != null)
			DestroyImmediate(texPuzzle);
		
		// exhibiting the pieces in the right position for generating camera
		int camPosX = (int)camDraw.transform.localPosition.x;
		foreach (CellGroup group in groups.Values)
		{
			DC_Cell data = group.cells[0].data;
			float posX = camPosX + data.posX * data.sizeCellX * 2 - (data.sizeBoardX - 1) * data.sizeCellX;
			float posY = -data.posY * data.sizeCellY * 2 + (data.sizeBoardY - 1) * data.sizeCellY;
			group.transform.localPosition = new Vector3(posX, posY, 0);
		}
		
		yield return new WaitForSeconds(0.1f);
		
		// save the resulting image in the texture
		RenderTexture.active = rTex;
		texPuzzle = new Texture2D(sizeBoardX * sizeCellX * 2, sizeBoardY * sizeCellY * 2, TextureFormat.ARGB32, false);
		texPuzzle.ReadPixels(new Rect(0, 0, sizeBoardX * sizeCellX * 2, sizeBoardY * sizeCellY * 2), 0, 0);
		texPuzzle.Apply();
		RenderTexture.active = null;
		
		camDraw.enabled = false;
		
		// We remove the helper objects and assign-generated texture
		foreach (CellGroup group in groups.Values)
		{
			Destroy(group.cells[0].draw.gameObject);
			group.cells[0].mesh.gameObject.SetActive(true);
			group.cells[0].mesh.sharedMaterial.mainTexture = texPuzzle;
		}
		
		// mix pieces
		for (int i=0; i<100; i++)
		{
			curGroup = groups[Random.Range(0, groups.Count)];
			SortgroupsUp();
		}
		
		curGroup = null;
		
		// set in the received position
		for (int y=0; y<sizeBoardY; y++)
		{
			for (int x=0; x<sizeBoardX; x++)
			{
				groups [x + y * sizeBoardX].transform.localPosition = new Vector3(
					x * sizeCellX - (sizeBoardX - 0) * sizeCellX / 2 + sizeCellX / 2,
					y * sizeCellY - (sizeBoardY - 0) * sizeCellY / 2 + sizeCellY / 2, 0);
			}
		}
	}

	void Update()
	{
		// check if the cursor over an interface element
		guiOver = EventSystem.current.IsPointerOverGameObject();
		
		if (guiOver)
			return;

		if (Input.GetMouseButtonDown(0) && !gameover && !moveFree)
		{
			moveMouse = true;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
		
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.name.Contains("Cell"))
				{
					// was pressing on a piece of
					Cell cell = hit.collider.transform.parent.GetComponent<Cell>();
					curGroup = cell.transform.parent.GetComponent<CellGroup>();
					// memorize the point of no return
					targetPos = curGroup.transform.localPosition;
					// move it to the upper layer
					SortgroupsUp();
					mousePos = Input.mousePosition;
				}
			}
		}

		if (Input.GetMouseButton(0) && curGroup != null && !gameover && moveMouse)
		{
			// Move the current piece
			Vector3 delta = Input.mousePosition - mousePos;
			delta.z = 0;

			// UIRoot.size - correct move distance
			curGroup.transform.Translate(delta / UIRoot.size);
			mousePos = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0) && curGroup != null && !gameover && moveMouse)
		{
			moveMouse = false;
			moveFree = true;

			// check the correct position on the board
			Vector3 pos = curGroup.transform.localPosition;
			pos.z = 0;

			// check what changed
			foreach(CellGroup group in groups.Values)
			{
				if(group == curGroup)
					continue;

				Vector3 pos1 = group.transform.localPosition;
				pos1.z = 0;

				if ((pos - pos1).magnitude < dockValue)
				{
					// We identified a piece for the exchange, change
					targetPos2 = targetPos;
					curGroup2 = group;

					pos1.z = curGroup.transform.localPosition.z;
					targetPos = pos1;

					targetPos2.z = curGroup2.transform.localPosition.z;;
					return;
				}

			}

		}

		if (!moveMouse && moveFree)
		{
			// no moving pieces of the mouse, complete the movement of the pieces
			float dist1 = 0;
			float dist2 = 0;

			if (curGroup != null)
			{
				curGroup.transform.localPosition = Vector3.Lerp(curGroup.transform.localPosition,
					targetPos,
					Time.deltaTime * 10);

				dist1 = (curGroup.transform.localPosition - targetPos).magnitude;
			}

			if (curGroup2 != null)
			{
				curGroup2.transform.localPosition = Vector3.Lerp(curGroup2.transform.localPosition,
					targetPos2,
					Time.deltaTime * 10);

				dist2 = (curGroup2.transform.localPosition - targetPos2).magnitude;
			}

			if(dist1 < 1 && dist2 < 1)
			{
				moveFree = false;
				if (curGroup != null)
					curGroup.transform.localPosition = targetPos;
				if (curGroup2 != null)
					curGroup2.transform.localPosition = targetPos2;

				// check at the end of the game
				if(!gameover)
				{
					CheckGameOver();
					curGroup = null;
				}
			}

		}

	}

	// check the end of the game
	void CheckGameOver()
	{
		foreach(CellGroup group in groups.Values)
		{
			if((group.transform.localPosition.x - group.cells[0].rightPos.x) > 2 ||
			   (group.transform.localPosition.y - group.cells[0].rightPos.y) > 2)
				return;
		}

		// all the pieces in place
		gameover = true;
		pictOver.gameObject.SetActive(true);
		StartCoroutine(GameOver());
	}

	IEnumerator GameOver()
	{
		yield return new WaitForSeconds(0.5f);

		EventDispatcher.SendEvent(EventName.GameOver);
	}

	// Move the selected piece in the uppermost layer (dragged)
	void SortgroupsUp()
	{
		int index = curGroup.index;
		int max = groups.Count - 1;

		for (int i=index; i<max; i++)
		{
			groups [i] = groups [i + 1];
			groups [i].index = i;
			groups [i].UpdateIndex();
		}

		groups [max] = curGroup;
		groups [max].index = max;
		groups [max].UpdateIndex();
	}

	void OnDestroy()
	{
		Destroy(texPuzzle);
		Destroy(rTex);
	}

}
