using System;

namespace CodeName.EventSystem.State
{
    public abstract class EntityInstance
    {
        public EntityId Id { get; set; }
        public abstract EntityAsset UntypedAsset { get; }

        public InstanceDataCollection Data { get; protected set; }
    }

    public class EntityInstance<T> : EntityInstance where T : EntityAsset
    {
        public EntityInstance(T asset, Func<EntityId> generateId)
        {
            Asset = asset;
            Data = CreateInstanceData(generateId);

            Id = generateId();
        }

        public EntityInstance(T asset, GameState state) : this(asset, state.GenerateId) {}

        public T Asset { get; }
        public override EntityAsset UntypedAsset => Asset;

        private InstanceDataCollection CreateInstanceData(Func<EntityId> generateId)
        {
            var data = new InstanceDataCollection();
            UntypedAsset.DefineInstanceData(data, generateId);

            return data;
        }
    }
}
