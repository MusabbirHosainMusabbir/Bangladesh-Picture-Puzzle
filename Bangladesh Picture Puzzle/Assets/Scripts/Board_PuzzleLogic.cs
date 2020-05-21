using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Board_PuzzleLogic : MonoBehaviour
{
	public Transform rootCells;		// root for the puzzle pieces
	public Transform pictBack;		// mesh for substrate image
	public Transform pictOver;		// mesh for the final image
	public CellGroup prefGroup;		// prefab puzzle piece
	public Camera camDraw;			// camera for the preparation of the puzzle pieces
	public Dictionary<int, CellGroup> groups;	// list of puzzle pieces
	
	private bool guiOver;			// click in the interface element
	private CellGroup curGroup;		// the current piece
	private Vector3 mousePos;		// the current position of the mouse
	private int sizeCellX;			// chunk-size X in pixels
	private int sizeCellY;			// chunk-size Y in pixels
	private RenderTexture rTex;		// RenderTexture to generate pieces
	private Texture2D texPuzzle;	// texture-generated puzzle pieces
	private float dockValue;		// distance to the correct position in pixels

	private Vector3 startPos;		// piece original position ready for installation
	private CellGroup nextGroup;	// the next piece group

	private bool gameover;
	private int sizeBoardX;
	private int sizeBoardY;
	
	void Awake()
	{
		pictOver.gameObject.SetActive(false);
		prefGroup.gameObject.SetActive(false);
	}

	public void SetData()
	{
		transform.Translate(-128, 0, 0);

		sizeBoardX = Game.piecesX;
		sizeBoardY = Game.piecesY;
		
		pictBack.localScale = new Vector3(Game.image.texture.width, Game.image.texture.height, 1);
		pictOver.localScale = pictBack.localScale;
		
		sizeCellX = (int)pictBack.localScale.x / sizeBoardX;
		sizeCellY = (int)pictBack.localScale.y / sizeBoardY;
		dockValue = Mathf.Min(sizeCellX, sizeCellY) / 4;

		startPos = new Vector3(430, -40, 0);
		
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

		// add the image to the substrate
		pictBack.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = Game.image.texture;
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

		// joints are generated for each piece
		for (int y=0; y<sizeBoardY; y++)
		{
			for (int x=0; x<sizeBoardX; x++)
			{
				groups [x + y * sizeBoardX].cells[0].draw.dataDraw = SetJoints(x, y);
				groups [x + y * sizeBoardX].cells[0].draw.UpdateJoints();
			}
		}

		// generates texture with pieces of puzzle and setted joints
		StartCoroutine(DrawingCells());
	}

	// generation of joints for a piece
	public DC_CellDraw SetJoints(int posX, int posY)
	{
		DC_CellDraw data = new DC_CellDraw();
//		data.posX = posX;
//		data.posY = posY;
		data.joints = new List<int>();
		data.invs = new List<bool>();
		
		// Side 1
		if (posY == 0)
		{
			data.joints.Add(0);
			data.invs.Add(false);
		}
		else
		{
			data.joints.Add(groups [posX + (posY - 1) * sizeBoardX].cells[0].draw.dataDraw.joints [2]);
			data.invs.Add(!groups [posX + (posY - 1) * sizeBoardX].cells[0].draw.dataDraw.invs [2]);
		}
		
		// Side 2
		if (posX == sizeBoardX - 1)
		{
			data.joints.Add(0);
			data.invs.Add(false);
		}
		else
		{
			data.joints.Add(Random.Range(1, prefGroup.cells[0].draw.jointMasks.Count));
			data.invs.Add(Random.Range(0, 2) == 0 ? false : true);
		}
		
		// Side 3
		if (posY == sizeBoardY - 1)
		{
			data.joints.Add(0);
			data.invs.Add(false);
		}
		else
		{
			data.joints.Add(Random.Range(1,  prefGroup.cells[0].draw.jointMasks.Count));
			data.invs.Add(Random.Range(0, 2) == 0 ? false : true);
		}
		
		// Side 4
		if (posX == 0)
		{
			data.joints.Add(0);
			data.invs.Add(false);
		}
		else
		{
			data.joints.Add(groups [(posX - 1) + posY * sizeBoardX].cells[0].draw.dataDraw.joints [1]);
			data.invs.Add(!groups [(posX - 1) + posY * sizeBoardX].cells[0].draw.dataDraw.invs [1]);
		}
		
		return data;
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
		
		// Wait position
		foreach (CellGroup group in groups.Values)
		{
			group.transform.localPosition = new Vector3(0, -800, 0);
		}
		
		// Random
		for (int i=0; i<100; i++)
		{
			curGroup = groups[Random.Range(0, groups.Count)];
			SortgroupsUp();
		}
		
		curGroup = null;
		nextGroup = groups [0];
	}

	void Update()
	{
		// check if the cursor over an interface element
		guiOver = EventSystem.current.IsPointerOverGameObject();

		if (guiOver)
			return;

		if (Input.GetMouseButtonDown(0) && !gameover)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
		
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.name.Contains("Cell"))
				{
					// was pressing on a piece of
					Cell cell = hit.collider.transform.parent.GetComponent<Cell>();
					curGroup = cell.transform.parent.GetComponent<CellGroup>();
					// move it to the upper layer
					SortgroupsUp();
					mousePos = Input.mousePosition;
				}
			}
		}

		if (Input.GetMouseButton(0) && curGroup != null && !gameover)
		{
			// Move the current piece
			Vector3 delta = Input.mousePosition - mousePos;
			delta.z = 0;

			// UIRoot.size - correct move distance
			curGroup.transform.Translate(delta / UIRoot.size);
			mousePos = Input.mousePosition;
		}
		else
		{
			// if the current piece is not set, then move to the starting position
			if (curGroup == null && nextGroup != null && !nextGroup.setted)
			{
				startPos.z = -nextGroup.index * 2;
				nextGroup.transform.localPosition = Vector3.Lerp(nextGroup.transform.localPosition,
				                                                 startPos,
				                                                 Time.deltaTime * 10);
			}
		}

		if (Input.GetMouseButtonUp(0) && curGroup != null && !gameover)
		{
			// check the correct position on the board
			Vector3 pos = curGroup.transform.localPosition;
			pos.z = 0;
			Vector3 vDelta = curGroup.cells[0].rightPos - pos;
			if (vDelta.magnitude < dockValue)
			{
				// right position, put in place
				curGroup.SetToRightPosition(vDelta);
				// Move the current piece of the lower layer
				SortgroupsDown();
				if(curGroup == nextGroup)
					nextGroup = null;
				curGroup = null;
				// check at the end of the game
				CheckGameOver();
				return;
			}

			curGroup = null;
		}

	}

	// check the end of the game
	void CheckGameOver()
	{
		foreach(CellGroup group in groups.Values)
		{
			if(group.setted ||
			   group == nextGroup)
				continue;

			if(nextGroup == null)
			{
				nextGroup = group;
				break;
			}
		}

		if (nextGroup != null)
			return;

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

	// Move the selected piece to the lowest layer (set to the desired position)
	void SortgroupsDown()
	{
		int index = curGroup.index;
		int max = groups.Count - 1;

		groups [max] = groups [0];
		groups [max].index = max;
		groups [max].UpdateIndex();

		groups [0] = curGroup;
		groups [0].index = 0;
		groups [0].UpdateIndex();
	}

	void OnDestroy()
	{
		Destroy(texPuzzle);
		Destroy(rTex);
	}


}
