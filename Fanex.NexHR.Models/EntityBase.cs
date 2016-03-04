namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Entity base.
    /// </summary>
    /// <typeparam name="TId">The entity type.</typeparam>
    public class EntityBase<TId> : IEntity<TId>
    {
        public TId Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}