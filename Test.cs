using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	LoadVoxelObject Loader = new LoadVoxelObject();
	Color[] Palette;
	public Texture2D[] PaletteTextures = new Texture2D[4];
	public Material[] ObjectMaterials;

	// Use this for initialization
	void Start () {
		CreateDefaultPalette();

		CreateMap ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateMap(){
		GameObject[] Map = new GameObject[16*16];

//		for(int x = 0; x < 16; x++){
//			for(int y = 0; y < 16; y++){
//				Map[y*16+x] = Loader.LoadObject("grass2", new Vector3(x*1.0f+1.0f,0.0f,y*1.0f+1.0f), Palette, ObjectMaterials[0], Y);
//				Map[y*16+x].transform.localScale = new Vector3(0.125f,0.125f,0.125f);
//			}
//		}
	}

	void CreateDefaultPalette(){
		Palette = new Color[256];
		
		//Palette = PaletteTextures[1].GetPixels();
		for(int x = 0; x < 16; x++){
			for(int y = 0; y < 16; y++){
				Palette[y * 16 + x] = PaletteTextures[0].GetPixel(x*16,y*16);
			}
		}
	}
}
