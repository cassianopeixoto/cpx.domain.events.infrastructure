using CPX.Domain.Abstract.Identifiers;

namespace CPX.Domain.Events.Infrastructure.Test.Mocks;
public sealed class FooId : Identifier
{
    public FooId(Guid value) : base(value)
    {
    }
}
