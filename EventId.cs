using System;

namespace CodeName.EventEngine
{
    public struct EventId : IEquatable<EventId>
    {
        public static EventId InvalidId => new(Guid.Empty);

        private readonly Guid internalId;

        public EventId(Guid guid)
        {
            internalId = guid;
        }

        public bool IsValid()
        {
            return this != InvalidId;
        }

        public bool Equals(EventId other)
        {
            return internalId == other.internalId;
        }

        public override bool Equals(object obj)
        {
            return obj is EventId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return internalId.GetHashCode();
        }

        public static explicit operator Guid(EventId id)
        {
            return id.internalId;
        }

        public static explicit operator EventId(Guid id)
        {
            return new EventId(id);
        }

        public static bool operator ==(EventId left, EventId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EventId left, EventId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return internalId.ToString();
        }

        public static EventId Generate()
        {
            return new EventId(Guid.NewGuid());
        }
    }
}
