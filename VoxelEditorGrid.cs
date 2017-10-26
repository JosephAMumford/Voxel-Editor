using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelEditorGrid {

	public GameObject Create2DGrid(Vector3 position, Vector2 size, Color color, Material material){
		GameObject NewGameOject = new GameObject("Grid");
		Mesh NewMesh = new Mesh();


		List<Vector3> NewVertices = new List<Vector3>();
		List<int> NewTriangles = new List<int>();
		List<Vector2> NewUVs = new List<Vector2>();
		List<Color32> NewColors = new List<Color32>();
		int Index;
		Color32 NewColor = color;

		for(float x = -(size.x/2); x < (size.x/2); x++){
			for(float z = -(size.y/2); z < (size.y/2); z++){
				NewVertices.Add(new Vector3(x, 0, z));
				NewVertices.Add(new Vector3(x+1, 0, z));
				NewVertices.Add(new Vector3(x, 0, z+1));
				NewVertices.Add(new Vector3(x+1, 0, z+1));
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				Index = NewVertices.Count - 4;		
				NewTriangles.Add(Index);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+1);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+3);
				NewTriangles.Add(Index+1);
				NewUVs.Add (new Vector2(0,0));
				NewUVs.Add (new Vector2(1,0));
				NewUVs.Add (new Vector2(0,1));
				NewUVs.Add (new Vector2(1,1));
			}
		}

		NewMesh.vertices = NewVertices.ToArray();
		NewMesh.colors32 = NewColors.ToArray();
		NewMesh.triangles = NewTriangles.ToArray();
		NewMesh.uv = NewUVs.ToArray();
		NewMesh.RecalculateNormals();
		NewMesh.Optimize();

		NewGameOject.AddComponent<MeshRenderer>();
		NewGameOject.AddComponent<MeshFilter>().sharedMesh = NewMesh;
		NewGameOject.AddComponent<MeshCollider>().sharedMesh = NewMesh;
		NewGameOject.transform.position = position;
		NewGameOject.GetComponent<Renderer>().material = material;

		return NewGameOject;
	}

	public GameObject Create3DGrid(Vector3 position, Vector3 size, Color color, Material material){
		GameObject NewGameOject = new GameObject("Grid");
		Mesh NewMesh = new Mesh();


		List<Vector3> NewVertices = new List<Vector3>();
		List<int> NewTriangles = new List<int>();
		List<Vector2> NewUVs = new List<Vector2>();
		List<Color32> NewColors = new List<Color32>();
		int Index;
		Color32 NewColor = color;

		float x,y,z;

		//XZ Plane
		for(x = -(size.x/2); x < (size.x/2); x++){
			for(z = -(size.z/2); z < (size.z/2); z++){
				NewVertices.Add(new Vector3(x, 0, z));
				NewVertices.Add(new Vector3(x+1, 0, z));
				NewVertices.Add(new Vector3(x, 0, z+1));
				NewVertices.Add(new Vector3(x+1, 0, z+1));
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				Index = NewVertices.Count - 4;		
				NewTriangles.Add(Index);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+1);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+3);
				NewTriangles.Add(Index+1);
				NewUVs.Add (new Vector2(0,0));
				NewUVs.Add (new Vector2(1,0));
				NewUVs.Add (new Vector2(0,1));
				NewUVs.Add (new Vector2(1,1));
			}
		}

		//YZ Plane
		for(y = 0; y < size.y; y++){
			for(z = -(size.z/2); z < (size.z/2); z++){
				NewVertices.Add(new Vector3(-(size.x/2), y, z));
				NewVertices.Add(new Vector3(-(size.x/2), y, z+1));
				NewVertices.Add(new Vector3(-(size.x/2), y+1, z));
				NewVertices.Add(new Vector3(-(size.x/2), y+1, z+1));
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				Index = NewVertices.Count - 4;		
				NewTriangles.Add(Index);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+1);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+3);
				NewTriangles.Add(Index+1);
				NewUVs.Add (new Vector2(0,0));
				NewUVs.Add (new Vector2(1,0));
				NewUVs.Add (new Vector2(0,1));
				NewUVs.Add (new Vector2(1,1));
			}
		}

		//XY Plane
		for(y = 0; y < size.y; y++){
			for(x = -(size.x/2); x < (size.x/2); x++){
				NewVertices.Add(new Vector3(x, y, (size.z/2)));
				NewVertices.Add(new Vector3(x+1, y, (size.z/2)));
				NewVertices.Add(new Vector3(x, y+1, (size.z/2)));
				NewVertices.Add(new Vector3(x+1, y+1, (size.z/2)));
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				NewColors.Add (NewColor);
				Index = NewVertices.Count - 4;		
				NewTriangles.Add(Index);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+1);
				NewTriangles.Add(Index+2);
				NewTriangles.Add(Index+3);
				NewTriangles.Add(Index+1);
				NewUVs.Add (new Vector2(0,0));
				NewUVs.Add (new Vector2(1,0));
				NewUVs.Add (new Vector2(0,1));
				NewUVs.Add (new Vector2(1,1));
			}
		}
		
		NewMesh.vertices = NewVertices.ToArray();
		NewMesh.colors32 = NewColors.ToArray();
		NewMesh.triangles = NewTriangles.ToArray();
		NewMesh.uv = NewUVs.ToArray();
		NewMesh.RecalculateNormals();
		NewMesh.Optimize();
		
		NewGameOject.AddComponent<MeshRenderer>();
		NewGameOject.AddComponent<MeshFilter>().sharedMesh = NewMesh;
		NewGameOject.AddComponent<MeshCollider>().sharedMesh = NewMesh;
		NewGameOject.transform.position = position;
		
		return NewGameOject;
	}
}
