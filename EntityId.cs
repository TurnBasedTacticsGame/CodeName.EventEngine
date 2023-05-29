using System;

namespace CodeName.EventSystem
{
    public struct EntityId : IEquatable<EntityId>
    {
        public static EntityId InvalidId => new(Guid.Empty);

        private readonly Guid internalId;

        public EntityId(Guid guid)
        {
            internalId = guid;
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
            return internalId.GetHashCode();
        }

        public static explicit operator Guid(EntityId id)
        {
            return id.internalId;
        }

        public static explicit operator EntityId(Guid id)
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

        public static EntityId Create()
        {
            return new EntityId(Guid.NewGuid());
        }
    }
}
