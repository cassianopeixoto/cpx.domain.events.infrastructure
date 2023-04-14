namespace CPX.Events.Infrastructure.Repository.Entities;

public abstract class Entity
{
    public Guid Uuid { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
}