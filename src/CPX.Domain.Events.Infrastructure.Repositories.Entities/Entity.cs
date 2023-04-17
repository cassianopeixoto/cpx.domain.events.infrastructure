namespace CPX.Domain.Events.Infrastructure.Repositories.Entities;

public abstract class Entity
{
    public Guid Uuid { get; set;}
    
    public DateTimeOffset CreatedAt { get; set; }
}