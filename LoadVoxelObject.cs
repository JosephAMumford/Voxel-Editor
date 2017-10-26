using UnityEngine;
using System.Collections;
using System.IO;

public class LoadVoxelObject {

	VoxelMesh NewVoxelMesh = new VoxelMesh();
	VoxelMeshFile VoxelFile = new VoxelMeshFile();

	//Load Menu Functions///////////////////////////////////////////////////////////////////////////////////////////////
	public GameObject LoadObject(string file, Vector3 position, Atlas[] atlas, Material material){
		BinaryReader br;
		
		try {
			FileStream filestream = File.OpenRead("Assets/VoxelObjects/" + file + ".vox");
			br = new BinaryReader(filestream);
		}
		catch (IOException e){
			Debug.Log (e.Message + " Cannot open file.");
			return null;
		}
		try{
			//Load stuff
			VoxelFile.Name = br.ReadString();
			VoxelFile.MaterialID = br.ReadInt32();
			VoxelFile.VoxelSize.x = (float)br.ReadInt32();
			VoxelFile.VoxelSize.y = (float)br.ReadInt32();
			VoxelFile.VoxelSize.z = (float)br.ReadInt32();
			int p = (int)VoxelFile.VoxelSize.x*(int)VoxelFile.VoxelSize.y*(int)VoxelFile.VoxelSize.z;
			VoxelFile.VoxelData = new int[p];
			for(int x = 0; x < p; x++){
				VoxelFile.VoxelData[x] = br.ReadInt32();
			}
			//Debug.Log(VoxelFile.Name + " loaded");
			
		}
		catch (IOException e){
			Debug.Log(e.Message + " Cannot read from file.");
		}
		br.Close ();


		return NewVoxelMesh.BuildVoxelMesh(VoxelFile.VoxelData, VoxelFile.VoxelSize, position, atlas, material, 1);
	}
}
