using UnityEngine;
using System.Collections;

[System.Serializable]
public class VoxelMeshFile {
	public string Name;
	public Vector3 VoxelSize;
	public int[] VoxelData;
	public Color[] Palette;
	public int MaterialID;
}
