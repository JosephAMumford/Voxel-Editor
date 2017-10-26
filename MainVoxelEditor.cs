using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

//use mode to decide pallete texture atlas or single texture

public class MainVoxelEditor : MonoBehaviour {
	
	//Menu Variables
	bool DebugOn;
	bool MenuOn;
	bool GridOn;
	bool CurrentColorOn;
	bool ColorPaletteOn;
	bool SaveOn;
	bool LoadOn;

	public GameObject EditorCamera;
	public Material[] EditorMaterials;
	public GameObject[] EditorPrefabs;
	public GameObject[] EditorObjects;
	public int GridMode;
	public Vector3 GridSize;
	public Color GridColor;
	public Color BackgroundColor;
	public int Mode;
	public Atlas[] TextureAtlas;
	public Vector2 SelectedTile;

	//Color Palette
	Color[] Palette;
	public int CurrentColor;
	Texture2D CurrentColorTexture;
	public int CurrentTexture;
	//public Texture2D PaletteTexture;
	public Texture2D[] PaletteTextures = new Texture2D[4];

	public	VoxelMeshFile VoxelFile = new VoxelMeshFile();
	VoxelMesh NewVoxelMesh = new VoxelMesh();
	private string[] DirectoryFiles;
	Vector2 scrollPosition = Vector2.zero;
	string SaveFileName = "";
	string LoadFileName = "";

	public GameObject[] Highlighters;
	RaycastHit Hit;

	VoxelEditorGrid NewVoxelEditorGrid = new VoxelEditorGrid();
	
	//Input
	bool LockAxisX;
	bool LockAxisY;
	bool LockAxisZ;
	Vector3 CurrentAxis;

	void Start(){
	
		//Setup format

		if(GridSize.x > 32 || GridSize.y > 32 || GridSize.z > 32){
			print("No axis can be greater than 32, now setting axis to default");
			GridSize = new Vector3(8,8,8);
		}

		//Create Voxel File
		VoxelFile.Name = "object1";
		VoxelFile.MaterialID = 0;
		VoxelFile.VoxelSize = GridSize;
		VoxelFile.VoxelData = new int[(int)GridSize.x*(int)GridSize.y*(int)GridSize.z];

		ClearVoxels();

		//Setup Workspace

		//Setup Axis
		EditorObjects = new GameObject[4];
		EditorObjects[0] = (GameObject)Instantiate(EditorPrefabs[4], new Vector3(0.0f,-0.05f,-(GridSize.z/2.0f)-0.05f), Quaternion.identity);
		EditorObjects[0].transform.localScale = new Vector3(GridSize.x, 0.1f, 0.1f);
		EditorObjects[0].transform.parent = gameObject.transform;
		EditorObjects[1] = (GameObject)Instantiate(EditorPrefabs[5], new Vector3(-(GridSize.x/2.0f)-0.05f,-0.05f,0.0f), Quaternion.identity);
		EditorObjects[1].transform.localScale = new Vector3(0.1f, 0.1f, GridSize.z);
		EditorObjects[1].transform.parent = gameObject.transform;

		//Create Grid and Grid Highlighters
		GridOn = true;
		if(GridMode == 0){
			EditorObjects[2] = NewVoxelEditorGrid.Create2DGrid(new Vector3(0,0,0), new Vector2(GridSize.x,GridSize.z), GridColor, EditorMaterials[0]);
			EditorObjects[2].GetComponent<Renderer>().material = EditorMaterials[0];
			EditorObjects[2].transform.parent = gameObject.transform;
			Highlighters = new GameObject[2];
			Highlighters[0] = (GameObject)Instantiate(EditorPrefabs[3]);
			Highlighters[0].transform.parent = gameObject.transform;
			Highlighters[1] = (GameObject)Instantiate(EditorPrefabs[0]);
			Highlighters[1].transform.parent = gameObject.transform;
			UpdateHighlighter(new Vector3(0.0f,0.0f,0.0f));
		}
		if(GridMode == 1){
			EditorObjects[2] = NewVoxelEditorGrid.Create3DGrid(new Vector3(0,0,0), new Vector3(GridSize.x,GridSize.y,GridSize.z), GridColor, EditorMaterials[0]);
			EditorObjects[2].GetComponent<Renderer>().material = EditorMaterials[0];
			EditorObjects[2].transform.parent = gameObject.transform;
			Highlighters = new GameObject[4];
			Highlighters[0] = (GameObject)Instantiate(EditorPrefabs[3]);
			Highlighters[0].transform.parent = gameObject.transform;
			Highlighters[1] = (GameObject)Instantiate(EditorPrefabs[0]);
			Highlighters[1].transform.parent = gameObject.transform;
			Highlighters[2] = (GameObject)Instantiate(EditorPrefabs[1]);
			Highlighters[2].transform.parent = gameObject.transform;
			Highlighters[3] = (GameObject)Instantiate(EditorPrefabs[2]);
			Highlighters[3].transform.parent = gameObject.transform;
			UpdateHighlighter(new Vector3(0.0f,0.0f,0.0f));
		}

		//Set Camera
		EditorCamera.GetComponent<SphericalCamera>().Theta = Mathf.PI/3.0f;
		EditorCamera.GetComponent<SphericalCamera>().Phi = -Mathf.PI/4.0f;
		EditorCamera.GetComponent<SphericalCamera>().Radius = 16.0f;
		EditorCamera.GetComponent<SphericalCamera>().Target = new Vector3(0,(GridSize.y/2.0f),0);

		//Set Background Color
		Camera.main.backgroundColor = BackgroundColor;

		//CreateDefaultPalette();
		ColorPaletteOn = true;
		CurrentColorOn = true;
		MenuOn = true;
		CurrentColor = 0;
		CurrentColorTexture = new Texture2D(16,16);
		CurrentColorTexture.filterMode = FilterMode.Point;
		UpdateCurrentColorTexture();
	}

	void Update(){
		//Check Input
		EditorInput();
	}

	void EditorInput(){
		if(SaveOn == false && LoadOn == false){
			//Update Hightlighter Position
			Ray Ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (Ray, out Hit, 100.0f)){
				UpdateHighlighter(Hit.point);

				//Mouse Input
				if(Input.GetMouseButtonUp(1)){	
					DeleteVoxel();	
				}
				if(Input.GetMouseButtonUp(0)){	
					AddVoxel();
				}
				if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
					if(Input.GetMouseButton(1)){	
						DeleteVoxel();	
					}
					if(Input.GetMouseButton(0)){	
						AddVoxel();	
					}
				}
			}
			//Lock Axis Control
			if(Input.GetKeyUp(KeyCode.X)){
				if(LockAxisX == false){	LockAxisX = true;	}	else {	LockAxisX = false;	}
			}
			if(Input.GetKeyUp(KeyCode.Y)){
				if(LockAxisY == false){	LockAxisY = true;	}	else {	LockAxisY = false;	}
			}
			if(Input.GetKeyUp(KeyCode.Z)){
				if(LockAxisZ == false){	LockAxisZ = true;	}	else {	LockAxisZ = false;	}
			}

			//Toggle Menu
			if(Input.GetKeyDown(KeyCode.M)){
				if(MenuOn == false){
					MenuOn = true;
				}
				else{
					MenuOn = false;
				}
			}

			//Toggle Debug
			if(Input.GetKeyDown (KeyCode.F1)){
				if(DebugOn == false){
					DebugOn = true;
				}
				else{
					DebugOn = false;
				}
			}

			//Toggle Grid
			if(Input.GetKeyDown(KeyCode.G)){
				if(GridOn == true){
					GridOn = false;
					EditorObjects[0].SetActive(false);
					EditorObjects[1].SetActive(false);
					EditorObjects[2].SetActive(false);
					if(GridMode == 0){
						Highlighters[0].SetActive(false);
						Highlighters[1].SetActive(false);
					}
					else{
						Highlighters[0].SetActive(false);
						Highlighters[1].SetActive(false);
						Highlighters[2].SetActive(false);
						Highlighters[3].SetActive(false);
					}
				}
				else {
					GridOn = true;
					EditorObjects[0].SetActive(true);
					EditorObjects[1].SetActive(true);
					EditorObjects[2].SetActive(true);
					if(GridMode == 0){
						Highlighters[0].SetActive(true);
						Highlighters[1].SetActive(true);
					}
					else{
						Highlighters[0].SetActive(true);
						Highlighters[1].SetActive(true);
						Highlighters[2].SetActive(true);
						Highlighters[3].SetActive(true);
					}
				}
			}

			//Toggle Color Menu
			if(Input.GetKeyDown(KeyCode.P)){
				if(ColorPaletteOn == false){
					ColorPaletteOn = true;
				}
				else{
					ColorPaletteOn = false;
				}
			}

			//Toggle Color Menu
			if(Input.GetKeyDown(KeyCode.C)){
				if(CurrentColorOn == false){
					CurrentColorOn = true;
				}
				else{
					CurrentColorOn = false;
				}
			}

			//Color Selection
			if(Input.GetKeyDown(KeyCode.D)){
				if(CurrentColor+1 < 255){
					CurrentColor++;
				}
				UpdateCurrentColorTexture();
			}
			if(Input.GetKeyDown(KeyCode.A)){
				if(CurrentColor-1 > 0){
					CurrentColor--;
				}
				UpdateCurrentColorTexture();
			}
			if(Input.GetKeyDown(KeyCode.S)){
				if(CurrentColor-16 > 0){
					CurrentColor -= 16;
				}
				UpdateCurrentColorTexture();
			}
			if(Input.GetKeyDown(KeyCode.W)){
				if(CurrentColor+16 < 255){
					CurrentColor += 16;
				}
				UpdateCurrentColorTexture();
			}
		}
		if(SaveOn == true){
			if(Input.GetKeyDown(KeyCode.Escape)){
				SaveOn = false;
			}
		}
		if(LoadOn == true){
			if(Input.GetKeyDown(KeyCode.Escape)){
				LoadOn = false;
			}
		}
	}

	void OnGUI(){

		if(CurrentColorOn == true){
			GUI.Box(new Rect(Screen.width-96, Screen.height-112,80,96), "Current");
			GUI.DrawTexture(new Rect(Screen.width-88, Screen.height-88,64,64), CurrentColorTexture,ScaleMode.ScaleToFit);
		}

		if(DebugOn == true){
			//Draw Debug information
			GUI.Label(new Rect(16,16,256,32), "Normal: " + Hit.normal.ToString());
			GUI.Label(new Rect(16,48,256,32), "Point: " + Hit.point.ToString());
		}

		if(MenuOn == true){
			//Draw Menu
			GUI.BeginGroup( new Rect(0,Screen.height-72,Screen.width,80));
			GUI.Box(new Rect(8,8,432,48),"");
			if(GUI.Button (new Rect(16,16,64,32), "Load")){
				LoadOn = true;
				GetFilesInDirectory();
				//LoadCurrentVoxelObject("Object1");
			}
			if(GUI.Button (new Rect(96,16,64,32), "Save")){
				SaveOn = true;
				//SaveCurrentVoxelObject(VoxelFile);
			}
			if(GUI.Button (new Rect(176,16,64,32), "Clear")){
				ClearVoxels();
			}

			if(GUI.Button (new Rect(256,16,64,32), "Fill")){
				FillVoxels();
			}

			LockAxisX = GUI.Toggle (new Rect(336,20,32,24), LockAxisX, " X");
			LockAxisY = GUI.Toggle (new Rect(368,20,32,24), LockAxisY, " Y");
			LockAxisZ = GUI.Toggle (new Rect(400,20,32,24), LockAxisZ, " Z");

			GUI.EndGroup();
		}

		if(ColorPaletteOn == true){
			//Color Menu
			GUI.BeginGroup(new Rect(16,16,272,552));
			GUI.Box(new Rect(0,0,256,352), "Color Menu");
			if(GUI.Button(new Rect(0,32,256,256), PaletteTextures[0])){
				Vector2 PickPosition = Event.current.mousePosition;
				//int a = (int)PickPosition.x;
				//int b = 288-(int)PickPosition.y;
				int a = (int)(PickPosition.x/16);
				int b = (int)((288-PickPosition.y)/16);
				if(a < 0){ a = 0;	}
				if(a > 15){ a = 15;	}
				if(b < 0){	b = 0;	}
				if(b > 15){	b = 15;	}
				CurrentColor = (b * 16) + a;
				//CurrentColor = ((b/15)*16+(a/15));
				//Palette[CurrentColor] = PaletteTexture.GetPixel(a,278-b);
				SelectedTile = new Vector2(a,b);
				UpdateCurrentColorTexture();
			}
			GUI.EndGroup();
		}

		if(LoadOn == true){
			GUI.Box (new Rect(Screen.width/2-112,Screen.height/2-128,224,256), "Load Menu");
			LoadFileName = GUI.TextField(new Rect(Screen.width/2-96,Screen.height/2-104,128,24), LoadFileName, 16);
			if(GUI.Button(new Rect(Screen.width/2+48,Screen.height/2-104,48,16), "Load")) {
				LoadCurrentVoxelObject(LoadFileName);
				LoadOn = false;
				LoadFileName = "";
			}
			
			int p = DirectoryFiles.Length;
			//Display list of files in save directory
			scrollPosition = GUI.BeginScrollView(new Rect(Screen.width/2-96, Screen.height/2-48, 196, 160), scrollPosition, 
			                                     new Rect(Screen.width/2-112, Screen.height/2-64, 180, 0 + (p*24) ));
			for(int x = 0; x < p; x++){
				GUI.Label(new Rect (Screen.width/2-96,Screen.height/2-64+(x*24),80,20), Path.GetFileName(DirectoryFiles[x]) );
			}
			GUI.EndScrollView();
		}

		if(SaveOn == true){
			GUI.Box (new Rect(Screen.width/2-112, Screen.height/2-32, 224, 64 ), "Save Menu");
			SaveFileName = GUI.TextField(new Rect(Screen.width/2-96, Screen.height/2,128,24), SaveFileName, 16);
			if(GUI.Button(new Rect(Screen.width/2+48, Screen.height/2,48,16), "Save")) {
				VoxelFile.Name = SaveFileName;
				SaveMesh(SaveFileName);
				//SaveCurrentVoxelObject(VoxelFile);
				SaveOn = false;
				SaveFileName = "";
			}
		}
	}

	//Save Menu Functions///////////////////////////////////////////////////////////////////////////////////////////////
	public void SaveCurrentVoxelObject(VoxelMeshFile v){
		BinaryWriter bw;
		
		//Create the file
		try {
			FileStream filestream = File.OpenWrite("Assets/VoxelObjects/" + v.Name + ".vox");
			bw = new BinaryWriter(filestream);
		}
		catch(IOException e){
			Debug.Log(e.Message + " Cannot create file");
			return;
		}
		
		//Write the file
		try{
			bw.Write (v.Name);
			bw.Write(v.MaterialID);
			bw.Write((int)v.VoxelSize.x);
			bw.Write((int)v.VoxelSize.y);
			bw.Write((int)v.VoxelSize.z);
			int p = (int)v.VoxelSize.x*(int)v.VoxelSize.y*(int)v.VoxelSize.z;
			for(int x = 0; x < p; x++){
				bw.Write(v.VoxelData[x]);
			}
		}
		catch (IOException e){
			Debug.Log (e.Message + " Cannot write to file.");
			return;
		}
		bw.Close();
		Debug.Log ("File written");
	}
	
	//Load Menu Functions///////////////////////////////////////////////////////////////////////////////////////////////
	public void LoadCurrentVoxelObject(string file){
		BinaryReader br;
		
		try {
			FileStream filestream = File.OpenRead("Assets/VoxelObjects/" + file + ".vox");
			br = new BinaryReader(filestream);
		}
		catch (IOException e){
			Debug.Log (e.Message + " Cannot open file.");
			return;
		}
		try{
			//Load stuff
			VoxelFile.Name = br.ReadString();
			VoxelFile.MaterialID = br.ReadInt32();
			VoxelFile.VoxelSize.x = (float)br.ReadInt32();
			VoxelFile.VoxelSize.y = (float)br.ReadInt32();
			VoxelFile.VoxelSize.z = (float)br.ReadInt32();
			int p = (int)VoxelFile.VoxelSize.x*(int)VoxelFile.VoxelSize.y*(int)VoxelFile.VoxelSize.z;
			for(int x = 0; x < p; x++){
				VoxelFile.VoxelData[x] = br.ReadInt32();
			}
			Debug.Log(VoxelFile.Name + " loaded");
			
		}
		catch (IOException e){
			Debug.Log(e.Message + " Cannot read from file.");
		}
		br.Close ();
		UpdateVoxelMesh();
	}
	
	public void GetFilesInDirectory(){
		DirectoryFiles = Directory.GetFiles("Assets/VoxelObjects/","*.vox");
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

	void UpdateCurrentColorTexture(){
		//CurrentColorTexture.SetPixel(1,1,Palette[CurrentColor]);
		//CurrentColorTexture.Apply();

		Color[] ColorMap = new Color[256];
		int a = (int)(SelectedTile.x*16);
		int b = (int)(SelectedTile.y*16);

		for(int x = 0; x < 16; x++){
			for(int y = 0; y < 16; y++){
				ColorMap[y * 16 + x] = PaletteTextures[0].GetPixel(a+x,b+y);			
			}
		}

		CurrentColorTexture.SetPixels(ColorMap);
		CurrentColorTexture.Apply();
	}

	void AddVoxel(){
		Vector3 p = Highlighters[0].transform.position;
		p.x += (GridSize.x/2)-0.5f;
		p.z += (GridSize.z/2)-0.5f;

		//VoxelObject[(int)p.x + (int)GridSize.y * ((int)p.y + (int)GridSize.z * (int)p.z)] = CurrentColor;
		VoxelFile.VoxelData[((int)p.y * (int)GridSize.x + (int)p.x) +((int)GridSize.x * (int)GridSize.y * (int)p.z)] = CurrentColor;
		if(GameObject.Find("Voxel Mesh") != null){
			DestroyImmediate(GameObject.Find("Voxel Mesh"));
			NewVoxelMesh.BuildVoxelMesh(VoxelFile.VoxelData, GridSize, new Vector3(-(GridSize.x/2),0,-(GridSize.z/2)), TextureAtlas, EditorMaterials[1], Mode);
		}
		else{
			NewVoxelMesh.BuildVoxelMesh(VoxelFile.VoxelData, GridSize, new Vector3(-(GridSize.x/2),0,-(GridSize.z/2)), TextureAtlas, EditorMaterials[1], Mode);
		}
		Resources.UnloadUnusedAssets();
	}

	void DeleteVoxel(){
		Vector3 p = Highlighters[0].transform.position;
		p.x += (GridSize.x/2)-0.5f;
		p.z += (GridSize.z/2)-0.5f;

		//VoxelObject[(int)p.x + (int)GridSize.y * ((int)p.y + (int)GridSize.z * (int)p.z)] = -1;
		VoxelFile.VoxelData[((int)p.y * (int)GridSize.x + (int)p.x) +((int)GridSize.x * (int)GridSize.y * (int)p.z)] = 0;
		if(GameObject.Find("Voxel Mesh") != null){
			DestroyImmediate(GameObject.Find("Voxel Mesh"));
			NewVoxelMesh.BuildVoxelMesh(VoxelFile.VoxelData, GridSize, new Vector3(-(GridSize.x/2),0,-(GridSize.z/2)), TextureAtlas, EditorMaterials[1], Mode);
		}
		else{
			NewVoxelMesh.BuildVoxelMesh(VoxelFile.VoxelData, GridSize, new Vector3(-(GridSize.x/2),0,-(GridSize.z/2)), TextureAtlas, EditorMaterials[1], Mode);
		}
		Resources.UnloadUnusedAssets();
	}

	void FillVoxels(){
		for(int x = 0; x < VoxelFile.VoxelData.Length; x++){
			VoxelFile.VoxelData[x] = CurrentColor;
		}
		UpdateVoxelMesh();
	}

	void ClearVoxels(){
		for(int x = 0; x < VoxelFile.VoxelData.Length; x++){
			VoxelFile.VoxelData[x] = 0;
		}
		UpdateVoxelMesh();
	}

	void UpdateVoxelMesh(){
		if(GameObject.Find("Voxel Mesh") != null){
			DestroyImmediate(GameObject.Find("Voxel Mesh"));
			NewVoxelMesh.BuildVoxelMesh(VoxelFile.VoxelData, GridSize, new Vector3(-(GridSize.x/2),0,-(GridSize.z/2)), TextureAtlas, EditorMaterials[1], Mode);
		}
		else{
			NewVoxelMesh.BuildVoxelMesh(VoxelFile.VoxelData, GridSize, new Vector3(-(GridSize.x/2),0,-(GridSize.z/2)), TextureAtlas, EditorMaterials[1], Mode);
		}
		Resources.UnloadUnusedAssets();
	}

	void SaveMesh(string name){
		if(Mode == 1){
			GameObject SaveObject = NewVoxelMesh.BuildVoxelMesh(VoxelFile.VoxelData, GridSize, new Vector3(-(GridSize.x/2),0,-(GridSize.z/2)), TextureAtlas, EditorMaterials[1], 2);

			Mesh voxelMesh = SaveObject.GetComponent<MeshFilter>().mesh;
			AssetDatabase.CreateAsset(voxelMesh, "Assets/" + name + ".asset"); // saves to "assets/"
			DestroyImmediate(SaveObject);
		}
		else {
			Mesh voxelMesh = GameObject.Find("Voxel Mesh").GetComponent<MeshFilter>().mesh;
			AssetDatabase.CreateAsset(voxelMesh, "Assets/" + name + ".asset"); // saves to "assets/"
			//AssetDatabase.SaveAssets(); // not needed?
		}
	}

	void UpdateHighlighter(Vector3 Position){
		if(LockAxisX == true){
			Position.x = CurrentAxis.x;
		}
		else{
			Position.x = Mathf.Round (Position.x);
		}
		if(LockAxisY == true){
			Position.y = CurrentAxis.y;
		}
		else{
			Position.y = Mathf.Round (Position.y);
		}
		if(LockAxisZ == true){
			Position.z = CurrentAxis.z;
		}
		else{
			Position.z = Mathf.Round (Position.z);
		}

		if(Position.x > (GridSize.x/2)-1) {	Position.x = (GridSize.x/2)-1;	}
		if(Position.x < -(GridSize.x/2)) {	Position.x = (GridSize.x/2);	}
		if(Position.y > GridSize.y-1) {	Position.y = GridSize.y-1;	}
		if(Position.y < 0) {	Position.y = 0;	}
		if(Position.z > (GridSize.z/2)-1) {	Position.z = (GridSize.z/2)-1;	}
		if(Position.z < -(GridSize.z/2)) {	Position.z = (GridSize.z/2);	}
		
		//2D Grid
		if(GridMode == 0){
			Vector3 p = new Vector3(Position.x+0.5f,Position.y+0.5f,Position.z+0.5f);
			Highlighters[0].transform.position = p;
			p = new Vector3(Position.x+0.5f, 0.0f, Position.z+0.5f);
			Highlighters[1].transform.position = p;
		}
		
		//3D Grid
		if(GridMode == 1){
			Vector3 p = new Vector3(Position.x+0.5f,Position.y+0.5f,Position.z+0.5f);
			Highlighters[0].transform.position = p;
			
			p = new Vector3(Position.x+0.5f, 0.0f, Position.z+0.5f);
			Highlighters[1].transform.position = p;
			
			p = new Vector3(Position.x+0.5f, Position.y+0.5f, (GridSize.z/2)-0.1f);
			Highlighters[2].transform.position = p;
			
			p = new Vector3(-(GridSize.x/2)+0.1f, Position.y+0.5f, Position.z+0.5f);
			Highlighters[3].transform.position = p;
		}
		if(LockAxisX == false){	CurrentAxis.x = Position.x;	}
		if(LockAxisY == false){	CurrentAxis.y = Position.y;	}
		if(LockAxisZ == false){	CurrentAxis.z = Position.z;	}
	}
}
