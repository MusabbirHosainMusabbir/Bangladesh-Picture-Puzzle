using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDraw : MonoBehaviour
{
	public MeshRenderer mesh;			// mesh quad with texture of puzzle pieces
	public DC_Cell data;				// a piece of data
	public DC_CellDraw dataDraw;		// a piece draw of data
	public List<Texture> jointMasks;	// to the list of masks for joints
	public float outline;				// offset outline

	public void SetData(DC_Cell data)
	{
		this.data = data;
		dataDraw = new DC_CellDraw();

		foreach (Transform child in transform)
		{
			child.localPosition = child.localPosition * outline;
			child.localScale = new Vector3(data.sizeCellX * 2, data.sizeCellY * 2, 1);
			MeshRenderer meshChild = child.GetComponent<MeshRenderer>();
			meshChild.sharedMaterial = new Material(meshChild.sharedMaterial);
		}

		mesh.sharedMaterial.SetTextureScale("_MainTex", new Vector2(data.sizeCellX / data.sizePictX * 2, data.sizeCellY / (float)data.sizePictY * 2));
		mesh.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(data.posX * 1f / data.sizeBoardX - 1f / data.sizeBoardX / 2f,
		                                                             (data.posY + 1) * -1f / data.sizeBoardY - 1f / data.sizeBoardY / 2f));

	}

	public void UpdateJoints()
	{
		foreach (Transform child in transform)
		{
			MeshRenderer meshChild = child.GetComponent<MeshRenderer>();
			for (int i=1; i<=4; i++)
			{
				meshChild.sharedMaterial.SetTexture("_Side" + i, jointMasks[dataDraw.joints[i - 1]]);
				meshChild.sharedMaterial.SetFloat("_Invert" + i, System.Convert.ToInt32(dataDraw.invs[i - 1]));
			}
		}
	}

	void OnDestroy()
	{
		foreach (Transform child in transform)
		{
			MeshRenderer meshChild = child.GetComponent<MeshRenderer>();
			Destroy(meshChild.sharedMaterial);
		}
	}

}

[System.Serializable]
public class DC_CellDraw
{
//	public int posX;
//	public int posY;
	public List<int> joints;
	public List<bool> invs;
}

