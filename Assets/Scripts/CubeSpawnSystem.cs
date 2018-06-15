using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;



public class CubeSpawnSystem : ComponentSystem {
	int width;
	int height;

	public struct Data{
		[ReadOnly] public ComponentDataArray<Spawner> Spawner;
	}

	[Inject] Data _data;

	public class SpawnBarrierSystem : BarrierSystem
	{}

	protected override void OnUpdate(){
		width = Bootstrap.Settings.width;
		height = Bootstrap.Settings.height;
		
		int halfDistanceHeight;
		int halfDistanceWidth;

		if(width % 2 == 0)
			halfDistanceWidth = width/2;
		else
			halfDistanceWidth = (width - 1)/2;

		if(width % 2 == 0)
			halfDistanceHeight = height/2;
		else
			halfDistanceHeight = (height - 1)/2;

		Bootstrap.halfDistanceHeight = halfDistanceHeight;
		Bootstrap.halfDistanceWidth = halfDistanceWidth;
		
		MeshInstanceRenderer MSI = Bootstrap.Settings.MSI.Value;
		MSI.material = Bootstrap.Settings.mat;
		
		MSI.mesh = Bootstrap.Settings.mesh;

		MeshInstanceRenderer MSIW = Bootstrap.Settings.MSI.Value;
		MSIW.material = Bootstrap.Settings.matW;
		
		MSIW.mesh = Bootstrap.Settings.mesh;
		int tempHalf;
		int tempHalfW = -halfDistanceWidth;

		for (int i = 0; i < width; i++){
			 tempHalf= -halfDistanceHeight;
			for (int k = 0; k < height; k++){
				PostUpdateCommands.CreateEntity(Bootstrap.Cube);
				PostUpdateCommands.SetComponent<Position>(new Position {Value = new float3(tempHalfW, 0, tempHalf)});
				PostUpdateCommands.SetComponent<Rotation>(new Rotation { Value = quaternion.identity});
				if(Bootstrap.Board[(i * (width)) + k] == 1)
					PostUpdateCommands.SetSharedComponent<MeshInstanceRenderer>(MSIW);
				else
					PostUpdateCommands.SetSharedComponent<MeshInstanceRenderer>(MSI);

				PostUpdateCommands.SetComponent<Color>(new Color { Value = Bootstrap.Board[(i * (width)) + k]});
				tempHalf++;
			}
			tempHalfW++;
		}

		this.Enabled = false;
	}
}
