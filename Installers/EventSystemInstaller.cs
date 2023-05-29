using UniDi;

namespace CodeName.EventSystem.Installers
{
    public class EventSystemInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateSerializer>().AsSingle();
        }
    }
}
