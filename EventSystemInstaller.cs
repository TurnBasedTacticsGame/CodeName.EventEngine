using CodeName.EventSystem.State.Serialization;
using UniDi;
using UnityEngine;

namespace CodeName.EventSystem
{
    public class EventSystemInstaller : MonoInstaller
    {
        [SerializeField] private GameResources resources;

        public override void InstallBindings()
        {
            Container.Bind<GameResources>().FromInstance(resources).AsSingle().OnInstantiated((_, _) =>
            {
                resources.Initialize();
            }).NonLazy();

            Container.Bind<GameStateSerializer>().AsSingle();
        }
    }
}
