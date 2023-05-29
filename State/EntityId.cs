using System;

namespace CodeName.EventSystem.State
{
    public struct EntityId : IEquatable<EntityId>
    {
        public static EntityId InvalidId => new(0);

        private readonly int internalId;

        public EntityId(int id)
        {
            internalId = id;
        }

        public bool IsValid()
        {
            return this != InvalidId;
        }

        public bool Equals(EntityId other)
        {
            return internalId == other.internalId;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return internalId;
        }

        public static explicit operator int(EntityId id)
        {
            return id.internalId;
        }

        public static explicit operator EntityId(int id)
        {
            return new EntityId(id);
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return internalId.ToString();
        }
    }
}
