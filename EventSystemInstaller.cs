using CodeName.EventSystem.State.Serialization;
using UniDi;

namespace CodeName.EventSystem
{
    public class EventSystemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateSerializer>().AsSingle();
        }
    }
}
