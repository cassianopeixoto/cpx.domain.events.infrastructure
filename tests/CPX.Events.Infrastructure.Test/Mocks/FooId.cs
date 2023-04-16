using CPX.Domain.Abstract.Identifiers;

namespace CPX.Events.Infrastructure.Test.Mocks;
public sealed class FooId : Identifier
{
    public FooId(Guid value) : base(value)
    {
    }
}
