using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
public class Bootstrap : MonoBehaviour{

	public static EntityArchetype Cube;
	public static EntityManager entityManager;
	public static EntityArchetype CubeSpawner;
	public static Settings Settings;

	public static NativeArray<int> Board;

	public static int halfDistanceHeight;
	public static int halfDistanceWidth;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Initialize(){
		entityManager = World.Active.GetOrCreateManager<EntityManager>();
		Cube = entityManager.CreateArchetype(typeof(Position), typeof(MeshInstanceRenderer), 
            typeof(Rotation), typeof(Color));

		CubeSpawner = entityManager.CreateArchetype(typeof(Spawner));
		

	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitializeWithScene(){
		Debug.Log("InitializeWithScene");
		Settings = GameObject.Find("Settings").GetComponent<Settings>();
		UnityEngine.Color[] col = Settings.tex2dl.GetPixels();
		Settings.width = Settings.tex2dl.width;
		Settings.height = Settings.tex2dl.height;
		Board = new NativeArray<int>(Settings.width * Settings.height, Allocator.Persistent);
		for (int i = 0; i < Settings.tex2dl.width; i++){
			for (int k = 0; k < Settings.tex2dl.height; k++){
				if(col[k + i * Settings.tex2dl.width] == UnityEngine.Color.white){
					Board[k + i * Settings.tex2dl.width] = 1;
				}
				else{
					Board[k + i * Settings.tex2dl.width] = 0;
					// Debug.Log("Black");
				}
			}
		}

		entityManager.CreateEntity(CubeSpawner);
	}

    private void OnApplicationQuit() {
        Board.Dispose();
    }

}
