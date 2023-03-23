using GezginRobotProjesi.Abstractions;
using GezginRobotProjesi.Entity;
using GezginRobotProjesi.Helpers;
using GezginRobotProjesi.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GezginRobotProjesi
{
    public class Application
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly GameMenu _menu;

        public Application(ServiceProvider serviceProvider){
            Console.TreatControlCAsInput = true;
            _serviceProvider = serviceProvider;
            _menu = _serviceProvider.GetRequiredService<GameMenu>();
        }

        public async Task GameLoop(){
            while (true) {
                _menu.Draw();
                if(_menu.GetTakenAction() == 0){
                    Console.WriteLine("Bay bay");
                    break;
                }
                if(_menu.GetTakenAction() == 1){
                    Response<GameMap> gameMap = await _menu.CreateMapFromUrl(_serviceProvider);
                    StartGame(gameMap);
                }
                if(_menu.GetTakenAction() == 2){
                    Response<MapSize> mapSize = _menu.AskMapSize();
                    if(mapSize.IsSuccess){
                        Response<GameMap> gameMap = _menu.CreateLabyrinth(_serviceProvider, mapSize.Result.Height, mapSize.Result.Width);
                        StartGame(gameMap);
                    }
                }

                if(_menu.GetTakenAction() == 3){
                    _menu.SwitchMapUrl();
                }
            }
        }

        public void StartGame(Response<GameMap> gameMap){
            if(gameMap.IsSuccess){
                bool isGameOver = false;
                PlayerRobot player = _serviceProvider.GetRequiredService<IPlayerRobotFactory>().CreateInstance(gameMap.Result.StartingPosition);
                player.SetIsGameOver(isGameOver);
                WallFollower wallFollower = new WallFollower(gameMap.Result, player);
                while(!player.ShouldFinishGame()){
                    wallFollower.GameMap.Draw(player.VisitedCoordinates, player.CurrentPosition);
                    while(player.GetAction() == -1){
                        player.WaitForAction();
                    }
                    int playerAction = player.GetAction();
                    if(playerAction == 1 || playerAction == 2){
                        bool shouldWaitAnotherAction = playerAction == 1;
                        Coordinate nextPosition = wallFollower.NextPosition();
                        player.Move(nextPosition, shouldWaitAnotherAction);
                        if(wallFollower.GameMap.EndingPosition.IsEqual(nextPosition)){
                            wallFollower.Player.SetIsGameOver(true);
                        }
                    }
                    if(playerAction == 3) {
                        player.ShowGameEndingMessage();
                        player.SetIsGameOver(true);
                    }
                }
                if(player.GetAction() != 3)
                {
                    wallFollower.GameMap.Draw(player.VisitedCoordinates, player.CurrentPosition);
                    Console.ReadKey();
                }
            }else{
                _menu.ShowError(gameMap.ErrorMessage);
            }
        }

    }
}