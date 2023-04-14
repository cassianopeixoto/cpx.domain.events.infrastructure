﻿namespace CPX.Events.Infrastructure.Repositories.Entities;

public sealed class Metadata : Entity
{
    public string? ClassName { get; set; }

    public string? NamespaceName { get; set; }

    public ICollection<Aggregate>? Aggregates { get; set; }

    public ICollection<Event>? Events { get; set; }
}