using UnityEngine;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    public sealed class SceneRuntimeBundle
    {
        public SceneRuntimeBundle(Camera mainCamera, Board3D board3D, CardFactory cardFactory, Hand3D hand3D)
        {
            MainCamera = mainCamera;
            Board3D = board3D;
            CardFactory = cardFactory;
            Hand3D = hand3D;
        }

        public Camera MainCamera { get; }
        public Board3D Board3D { get; }
        public CardFactory CardFactory { get; }
        public Hand3D Hand3D { get; }
    }
}
