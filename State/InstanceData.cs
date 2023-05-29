using Newtonsoft.Json;

namespace CodeName.EventSystem.State
{
    /// <summary>
    ///     Inherit from this class to store data within <see cref="EntityInstance"/>s.
    /// </summary>
    /// <remarks>
    ///     Note to implementers: Ideally the inheriting data classes should be sealed because <see cref="InstanceDataCollection"/> does not allow inheritance.
    ///     If the data class is only used for a specific <see cref="EntityAsset"/>, the class should be marked as private.
    /// </remarks>
    public abstract class InstanceData
    {
        /// <summary>
        ///     Key in the format of "ModId:InstanceDataName". Used for serialization.
        /// </summary>
        [JsonIgnore]
        public abstract string Key { get; }

        protected string CreateKey(string modId, string resourceName)
        {
            return $"{modId}:{resourceName}";
        }

        protected string CreateKey(string modId, string resourceName, string path)
        {
            return $"{modId}:{resourceName}/{path}";
        }
    }
}
