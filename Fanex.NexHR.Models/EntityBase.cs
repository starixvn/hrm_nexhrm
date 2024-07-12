namespace Fanex.NexHR.Models
{
    public class EntityBase<TId> : IEntity<TId>
    {
        public TId Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}