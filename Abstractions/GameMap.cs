using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GezginRobotProjesi.Entity;
using GezginRobotProjesi.Entity.Enums;
using GezginRobotProjesi.Helpers;

namespace GezginRobotProjesi.Abstractions
{
    public abstract class GameMap
    {
        public List<List<Block>> Playground {get; set;}
        public Coordinate StartingPosition {get; set;} = new Coordinate(0, 0);
        public Coordinate EndingPosition {get; set;} = new Coordinate(0, 0);
        public Robot Player {get; set;}

        public GameMap() { 
            this.Playground = new List<List<Block>>();
            this.StartingPosition = new Coordinate(0, 0);
            this.EndingPosition = new Coordinate(0, 0);
            this.Player = new Robot(0, 0);
        }

        public void SetGameMap(List<List<Block>> map){
            this.Playground = map;
            int height = map.Count;
            int width = height > 0 ? map[0].Count : 0;
            for(int i=0; i<width; i++){
                if(map[1][i].Type == BlockType.Path){
                    this.StartingPosition = new Coordinate(0, i);
                    LabyrinthHelper.SetBlockAsPath(map, this.StartingPosition);
                    break;
                }
            }
            for(int j=width-1; j>-1; j--){
                if(map[height-2][j].Type == BlockType.Path){
                    this.EndingPosition = new Coordinate(height-1, j);
                    LabyrinthHelper.SetBlockAsPath(map, this.EndingPosition);
                    break;
                }
            }
            Player = new Robot(StartingPosition.X, StartingPosition.Y);
        }

        public abstract void Draw();

    }
}