
using HangOut.Domain.Entities.Common.Interface;

namespace HangOut.Domain.Entities.Common;

public class EntityBase<TKey> : IEntityBase<TKey>
{
    public TKey Id { get; set; }
}