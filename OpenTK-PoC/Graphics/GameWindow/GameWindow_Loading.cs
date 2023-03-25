using Mechs_Vs_Minions_Abstractions;
using Mechs_Vs_Minions_Abstractions.GameStates;
using Mechs_Vs_Minions_Graphics.Graphics.Abstractions;
using Mechs_Vs_Minions_Graphics.Graphics.MeshProcessing;
using Mechs_Vs_Minions_Graphics.Graphics.Models;
using Mechs_Vs_Minions_Graphics.Graphics.StateManagement;
using Mechs_Vs_Minions_Graphics.Graphics.Uniforms;
using OpenTK.Mathematics;
using System.Collections.Immutable;
using System.Drawing;
using Utilities;

namespace Mechs_Vs_Minions_Graphics.Graphics.GameWindow;

internal partial class GameWindow
{
    private static void LoadAudio(IAudioStore audioStore)
    {
        audioStore.Load(AudioHandle.FireBurst);
        audioStore.Load(AudioHandle.Swoosh);
        audioStore.Load(AudioHandle.StoneSlide);
        audioStore.Load(AudioHandle.Slash);
    }

    private void EnqueueLoadOverlayModels(
        GraphicsModelStore graphicsModelStore,
        ITexturesStore texturesStore,
        OverlayUniformSet overlayUniformSet
    )
    {
        _loadingTasks.Enqueue(
            new Task(() => {
                _titlePrefix = "Loading Overlays";
            })
        );
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<CommandLineModel, OverlayInstance>(
                    new CommandLineModel(texturesStore, overlayUniformSet)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<RipsawCardModel, OverlayInstance>(
                    new RipsawCardModel(texturesStore, overlayUniformSet)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<BlazeCardModel, OverlayInstance>(
                    new BlazeCardModel(texturesStore, overlayUniformSet)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<OmnistompCardModel, OverlayInstance>(
                    new OmnistompCardModel(texturesStore, overlayUniformSet)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<SpeedCardModel, OverlayInstance>(
                    new SpeedCardModel(texturesStore, overlayUniformSet)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<AimBotCardModel, OverlayInstance>(
                    new AimBotCardModel(texturesStore, overlayUniformSet)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<ScytheCardModel, OverlayInstance>(
                    new ScytheCardModel(texturesStore, overlayUniformSet)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<DamageCardModel, OverlayInstance>(
                    new DamageCardModel(texturesStore, overlayUniformSet)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<RightArrowModel, OverlayInstance>(
                    new RightArrowModel(texturesStore, overlayUniformSet)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<TopArrowModel, OverlayInstance>(
                    new TopArrowModel(texturesStore, overlayUniformSet)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<LeftArrowModel, OverlayInstance>(
                    new LeftArrowModel(texturesStore, overlayUniformSet)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<DownArrowModel, OverlayInstance>(
                    new DownArrowModel(texturesStore, overlayUniformSet)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<VictoryOverlayModel, OverlayInstance>(
                    new VictoryOverlayModel(texturesStore, overlayUniformSet)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<ResetGameMenuModel, OverlayInstance>(
                    new ResetGameMenuModel(texturesStore, overlayUniformSet)
                );
            })
        );
    }

    private void EnqueueLoadGameModels(
        GraphicsModelStore graphicsModelStore,
        ITexturesStore texturesStore,
        IObjStore objStore,
        ICameraUniformData cameraUniformData,
        ParticleUniformSet particleUniformSet
    )
    {
        _loadingTasks.Enqueue(
            new Task(() => {
                _titlePrefix = "Loading Models";
            })
        );
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<CoordinateSystemModel, PositionalInstance>(
                    new CoordinateSystemModel(cameraUniformData)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<TristanaModel, MechInstance>(
                    new TristanaModel(texturesStore, objStore, cameraUniformData)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<HeimerdingerModel, MechInstance>(
                    new HeimerdingerModel(texturesStore, objStore, cameraUniformData)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<CorkiModel, MechInstance>(
                    new CorkiModel(texturesStore, objStore, cameraUniformData)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<ZiggsModel, MechInstance>(
                    new ZiggsModel(texturesStore, objStore, cameraUniformData)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<LightModel, MinionInstance>(
                    new LightModel(texturesStore, objStore, cameraUniformData)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<BlueMinionModel, MinionInstance>(
                    new BlueMinionModel(texturesStore, objStore, cameraUniformData)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<RedMinionModel, MinionInstance>(
                    new RedMinionModel(texturesStore, objStore, cameraUniformData)
                );
            })
        );
        
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<ParticleModel, ParticleInstance>(
                    new ParticleModel(cameraUniformData, particleUniformSet)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<RipsawModel, RipSawInstance>(
                    new RipsawModel(texturesStore, cameraUniformData)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<TransparentOverlayModel, OverlayInstance>(
                    new TransparentOverlayModel()
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<SkyBoxModel,PositionalInstance>(
                    new SkyBoxModel(texturesStore, cameraUniformData)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<ClickDummySquareModel, ClickableInstance>(
                    new ClickDummySquareModel(cameraUniformData)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<SquareOutlineModel, BoardTileInstance>(
                    new SquareOutlineModel(cameraUniformData)
                );
            })
        );

        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<PolylineModel, PositionalInstance>(
                    new PolylineModel(cameraUniformData)
                );
            })
        );
    }


    private void EnqueueLoadGameStateAssets(
        GraphicsModelStore graphicsModelStore,
        ITexturesStore texturesStore,
        ICameraUniformData cameraUniformData,
        GameState gameState
    )
    {
        _loadingTasks.Enqueue(
            new Task(() => {
                graphicsModelStore.AddRenderModel<GameBoardModel, PositionalInstance>(
                    GameBoardModel.Generate(texturesStore, cameraUniformData, gameState.Board)
                );
            })
        );

        _loadingTasks.Enqueue(
           new Task(() => {
               graphicsModelStore.AddRenderModel<GameBoardRockModel, PositionalInstance>(
                   GameBoardRockModel.Generate(texturesStore, cameraUniformData)
               );
           })
       );

        var obstacleGraph = GetObstacleGraph(gameState);
        var obstacleGroups = obstacleGraph.GetConnectedComponents();
        
        foreach (var obsGroup in obstacleGroups)
        {
            var tiles = obsGroup.Select(p => new Vector2i(p.X, p.Y));
            _worldGraphicsState.AddStones(CircleDistributor.GenerateRandomCirclesInTiles(tiles.ToImmutableList()));
        }

        _loadingTasks.Enqueue(
            new Task(() => {
                _titlePrefix = "Loading Assets";
                _toBeLoadedItemCount = _worldGraphicsState.StonePositions.Count;
            })
        );
        
        for(var i = 0; i < _worldGraphicsState.StonePositions.Count; i++)
        {
            _loadingTasks.Enqueue(
                new Task(() => {
                    graphicsModelStore.AddProceduralRenderModel<RockModel, PositionalInstance>(
                        RockModel.Generate(cameraUniformData)
                    );
                })
            );
        }
    }

    private void ReLoadGameStateAssets(
        GraphicsModelStore graphicsModelStore,
        ITexturesStore texturesStore,
        ICameraUniformData cameraUniformData,
        GameState gameState
    )
    {
        
        graphicsModelStore.Destroy<GameBoardModel>();
        graphicsModelStore.AddRenderModel<GameBoardModel, PositionalInstance>(
            GameBoardModel.Generate(texturesStore, cameraUniformData, gameState.Board)
        );
            

        var obstacleGraph = GetObstacleGraph(gameState);
        var obstacleGroups = obstacleGraph.GetConnectedComponents();

        _worldGraphicsState.ClearStones();
        foreach (var obsGroup in obstacleGroups)
        {
            var tiles = obsGroup.Select(p => new Vector2i(p.X, p.Y));
            _worldGraphicsState.AddStones(CircleDistributor.GenerateRandomCirclesInTiles(tiles.ToImmutableList()));
        }

        graphicsModelStore.Destroy<RockModel>();
        for(var i = 0; i < _worldGraphicsState.StonePositions.Count; i++)
        {
            graphicsModelStore.AddProceduralRenderModel<RockModel, PositionalInstance>(
                RockModel.Generate(cameraUniformData)
            );
        }
    }

    private static Graph<Point> GetObstacleGraph(GameState gameState)
    {
        var graph = new Graph<Point>();

        var vertices = gameState.Board.BoardTiles
            .Select(entry => entry.Key)
            .Where(tile => !gameState.Board.IsPassable(tile))
            .ToImmutableList();

        graph.AddVertices(vertices);

        foreach (var v in vertices)
        {
            var neighbors = new List<Point>()
            {
                v with { X = v.X-1 },
                v with { Y = v.Y+1 },
                v with { X = v.X+1 },
                v with { Y = v.Y-1 }
            };
            neighbors.ForEach(n => graph.TryCreateEdge(v, n));
        }

        return graph;
    }
}
