namespace GroceryTracker.Domain;

public abstract class Entity
{
    public Guid Id { get; set; }
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.Now;
}
