using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class UpdateStateSystem : ComponentSystem {
	struct Data{
		[ReadOnly] public ComponentDataArray<Position> pos;
		public ComponentDataArray<Color> col;
		[ReadOnly] public SharedComponentDataArray<MeshInstanceRenderer> msi;
		[ReadOnly] public EntityArray entity;
	}

	[Inject] Data _data;

	[Unity.Burst.BurstCompile]
	struct updateJob : IJobParallelFor{
		[ReadOnly] public ComponentDataArray<Position> pos;
		[ReadOnly] public NativeArray<int> Board;
		public ComponentDataArray<Color> col;
		[ReadOnly] public int halfHeight;
		[ReadOnly] public int halfWidth, width;
        //public int births, deaths, stables;

        int x, y, liveNeighbours, newI;
		public void Execute(int i){
			x = (int)pos[i].Value.x + halfHeight;
			y = (int)pos[i].Value.z + halfWidth;
            liveNeighbours = 0;
			// Debug.Log(x + "  " + y);
			if(x < 2*halfHeight - 2 && x > 0 && y > 0 && y < 2* halfWidth - 2){
				newI = (x-1) + (y-1) * width;
				if(Board[newI] != 1){
					liveNeighbours++;
				}

				newI = (x-1) + (y) * width;
				if(Board[newI] != 1){
					liveNeighbours++;
				}

				newI = (x-1) + (y+1) * width;
				if(Board[newI] != 1){
					liveNeighbours++;
				}

				newI = (x) + (y-1) * width;
				if(Board[newI] != 1){
					liveNeighbours++;
				}

				newI = (x) + (y+1) * width;
				if(Board[newI] != 1){
					liveNeighbours++;
				}

				newI = (x+1) + (y-1) * width;
				if(Board[newI] != 1){
					liveNeighbours++;
				}

				newI = (x+1) + (y) * width;
				if(Board[newI] != 1){
					liveNeighbours++;
				}

				newI = (x+1) + (y+1) * width;
				if(Board[newI] != 1){
					liveNeighbours++;
				}
			}

            //if(liveNeighbours != 0) {
            //    Debug.Log(liveNeighbours);
            //}
				
			if(col[i].Value == 1 && liveNeighbours == 3){
				col[i] = new Color { Value = 0};
                //births++;
                // msi[i] = MSIB;
            }
			else if(col[i].Value != 1){
                if (liveNeighbours > 3) {
                    col[i] = new Color { Value = 1 };
                    //deaths++;
                }
                else if (liveNeighbours < 2) {
                    col[i] = new Color { Value = 0 };
                    // births++;
                }
			}
            
		}
	}

	struct updateBoard : IJob{
		[ReadOnly] public ComponentDataArray<Color> col;
        [ReadOnly] public int halfHeight;
        [ReadOnly] public int halfWidth, width;
        [ReadOnly] public ComponentDataArray<Position> pos;

        public NativeArray<int> Board;
        int x, y, index;

		public void Execute(){
			for(int i = 0; i < col.Length; i++){
                x = (int)pos[i].Value.x + halfHeight;
                y = (int)pos[i].Value.z + halfWidth;
                index = x + y * width;
                Board[index] = col[i].Value;
			}
		}
	}

	protected override void OnUpdate(){
		MeshInstanceRenderer MSI = Bootstrap.Settings.MSI.Value;
		MSI.material = Bootstrap.Settings.mat;
		
		MSI.mesh = Bootstrap.Settings.mesh;

		MeshInstanceRenderer MSIW = Bootstrap.Settings.MSI.Value;
		MSIW.material = Bootstrap.Settings.matW;
		
		MSIW.mesh = Bootstrap.Settings.mesh;
        int births = 0, deaths = 0, stables = 0;
        var job = new updateJob() {
            pos = _data.pos,
            col = _data.col,
            Board = Bootstrap.Board,
            halfHeight = Bootstrap.halfDistanceHeight,
            halfWidth = Bootstrap.halfDistanceWidth,
            width = Bootstrap.Settings.width,
		};
		JobHandle jobHandle = job.Schedule(Bootstrap.Board.Length, Bootstrap.Board.Length/SystemInfo.processorCount);

        JobHandle uBoard = new updateBoard(){ col = _data.col,
            Board = Bootstrap.Board,
            halfHeight = Bootstrap.halfDistanceHeight,
            halfWidth = Bootstrap.halfDistanceWidth,
            pos = _data.pos,
            width = Bootstrap.Settings.width
        }.Schedule(jobHandle);
        //Debug.Log("stables: " + stables + " deaths: " + deaths + " and births: " + births);

        uBoard.Complete();


        int changesTemp = 0;
        
		for(int i = 0; i < _data.col.Length; i++){
			if(_data.msi[i].material.color.b != _data.col[i].Value){
                changesTemp++;
				
				if(_data.msi[i].material.color.b == 1)
					PostUpdateCommands.SetSharedComponent<MeshInstanceRenderer>(_data.entity[i], MSI);
				else
					PostUpdateCommands.SetSharedComponent<MeshInstanceRenderer>(_data.entity[i], MSIW);
			}
		}
        
        changesTemp = 0;
        //this.Enabled = false;
    }

    
}
