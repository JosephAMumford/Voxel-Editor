using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//Need to remove color information, work on face reduction, and add scale effect.
//Scale effect could be a multiplier where you can still use a texture atlas and share a material for
//zone and detail objects.  zone object will be normal scale 1.0f with texture offset at 1/16; Detail object
//will have scale 1/16 with texture offset at 1/256.  each data set will need to have a number to figure out
//what sub-texture from atlas to use.

public class VoxelMesh {

	public GameObject BuildVoxelMesh(int[] data, Vector3 size, Vector3 position, Atlas[] atlas, Material material, int mode){
		float ox = 1.0f/16.0f;
		float oy = 1.0f/16.0f;
		GameObject NewGameObject = new GameObject("Voxel Mesh");
		int a, b ,c;
		
		a = (int)size.x;
		b = (int)size.y;
		c = (int)size.z;

		Vector3 s = new Vector3(1.0f,1.0f,1.0f);
		if(mode == 2){
			s.x = 1.0f/size.x;
			s.y = 1.0f/size.y;
			s.z = 1.0f/size.z;
		}
		Mesh NewMesh = new Mesh();
		Color NewColor = new Color();
		List<Vector3> NewVertices = new List<Vector3>();
		List<int> NewTriangles = new List<int>();
		List<Vector2> NewUVs = new List<Vector2>();
		List<Color> NewColors = new List<Color>();
		int Index;
		int[] n = new int[6];
		for(int x = 0; x < a; x++){
			for(int y = 0; y < b; y++){
				for(int z = 0; z < c; z++){

					//If voxel exists
					//if(data[x + b * (y + c * z)] != -1){
					int t = data[(y * a + x)+(z * a * b)];
					if(data[(y * a + x)+(z * a * b)] != 0){	

						//NewColor = Palette[t];
						NewColor = Color.white;

						if(y < (b-1)){	n[0] = data[((y+1) * a + x)+(z * a * b)];	}
						if(y > 0){		n[1] = data[((y-1) * a + x)+(z * a * b)];	}
						if(x > 0){		n[2] = data[(y * a + (x-1))+(z * a * b)];	}
						if(x < (a-1)){	n[3] = data[(y * a + (x+1))+(z * a * b)];	}
						if(z > 0){		n[4] = data[(y * a + x)+((z-1) * a * b)];	}
						if(z < (c-1)){	n[5] = data[(y * a + x)+((z+1) * a * b)];	}

						if(n[0] == 0 || y == b-1){
							//Top Face
							NewVertices.Add(new Vector3(s.x*x, s.y*y+s.y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y+s.y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y+s.y, s.z*z+s.z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y+s.y, s.z*z+s.z));
							Index = NewVertices.Count - 4;		
							NewTriangles.Add(Index);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+1);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+3);
							NewTriangles.Add(Index+1);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewUVs.Add(new Vector2(atlas[t].Tile[0].x*ox, atlas[t].Tile[0].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[0].x*ox+ox, atlas[t].Tile[0].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[0].x*ox, atlas[t].Tile[0].y*oy+oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[0].x*ox+ox, atlas[t].Tile[0].y*oy+oy)); 
						}

						if(n[1] == 0 || y == 0){
							//Bottom Face
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y, s.z*z+s.z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y, s.z*z+s.z));		
							Index = NewVertices.Count - 4;		
							NewTriangles.Add(Index);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+1);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+3);
							NewTriangles.Add(Index+1);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewUVs.Add(new Vector2(atlas[t].Tile[1].x*ox, atlas[t].Tile[1].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[1].x*ox+ox, atlas[t].Tile[1].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[1].x*ox, atlas[t].Tile[1].y*oy+oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[1].x*ox+ox, atlas[t].Tile[1].y*oy+oy)); 
						}

						if(n[2] == 0 || x == 0){
							//Left Face
							NewVertices.Add(new Vector3(s.x*x, s.y*y, s.z*z+s.z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y+s.y, s.z*z+s.z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y+s.y, s.z*z));	
							Index = NewVertices.Count - 4;		
							NewTriangles.Add(Index);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+1);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+3);
							NewTriangles.Add(Index+1);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewUVs.Add(new Vector2(atlas[t].Tile[2].x*ox, atlas[t].Tile[2].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[2].x*ox+ox, atlas[t].Tile[2].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[2].x*ox, atlas[t].Tile[2].y*oy+oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[2].x*ox+ox, atlas[t].Tile[2].y*oy+oy)); 
						}

						if(n[3] == 0 || x == a-1){
							//Right Face
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y, s.z*z+s.z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y+s.y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y+s.y, s.z*z+s.z));
							Index = NewVertices.Count - 4;		
							NewTriangles.Add(Index);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+1);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+3);
							NewTriangles.Add(Index+1);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewUVs.Add(new Vector2(atlas[t].Tile[3].x*ox, atlas[t].Tile[3].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[3].x*ox+ox, atlas[t].Tile[3].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[3].x*ox, atlas[t].Tile[3].y*oy+oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[3].x*ox+ox, atlas[t].Tile[3].y*oy+oy)); 
						}

						if(n[4] == 0 || z == 0){
							//Front Face
							NewVertices.Add(new Vector3(s.x*x, s.y*y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y+s.y, s.z*z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y+s.y, s.z*z));
							Index = NewVertices.Count - 4;		
							NewTriangles.Add(Index);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+1);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+3);
							NewTriangles.Add(Index+1);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewUVs.Add(new Vector2(atlas[t].Tile[4].x*ox, atlas[t].Tile[4].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[4].x*ox+ox, atlas[t].Tile[4].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[4].x*ox, atlas[t].Tile[4].y*oy+oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[4].x*ox+ox, atlas[t].Tile[4].y*oy+oy)); 
						}

						if(n[5] == 0 || z == c-1){
							//Back Face
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y, s.z*z+s.z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y, s.z*z+s.z));
							NewVertices.Add(new Vector3(s.x*x+s.x, s.y*y+s.y, s.z*z+s.z));
							NewVertices.Add(new Vector3(s.x*x, s.y*y+s.y, s.z*z+s.z));
							Index = NewVertices.Count - 4;		
							NewTriangles.Add(Index);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+1);
							NewTriangles.Add(Index+2);
							NewTriangles.Add(Index+3);
							NewTriangles.Add(Index+1);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewColors.Add(NewColor);
							NewUVs.Add(new Vector2(atlas[t].Tile[5].x*ox, atlas[t].Tile[5].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[5].x*ox+ox, atlas[t].Tile[5].y*oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[5].x*ox, atlas[t].Tile[5].y*oy+oy)); 
							NewUVs.Add(new Vector2(atlas[t].Tile[5].x*ox+ox, atlas[t].Tile[5].y*oy+oy)); 
						}
					}
				}
			}
		}
		
		NewMesh.vertices = NewVertices.ToArray();
		NewMesh.triangles = NewTriangles.ToArray();
		NewMesh.uv = NewUVs.ToArray();
		NewMesh.colors = NewColors.ToArray();
		NewMesh.RecalculateNormals();
		NewMesh.Optimize();
		
		NewGameObject.AddComponent<MeshRenderer>();
		NewGameObject.AddComponent<MeshFilter>().sharedMesh = NewMesh;
		NewGameObject.AddComponent<MeshCollider>().sharedMesh = NewMesh;
		NewGameObject.transform.position = position;
		NewGameObject.GetComponent<Renderer>().material = material;
		
		return NewGameObject;
	}
}
