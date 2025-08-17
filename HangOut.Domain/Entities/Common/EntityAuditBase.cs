using HangOut.Domain.Entities.Common.Interface;

namespace HangOut.Domain.Entities.Common;

public abstract class EntityAuditBase<T> : EntityBase<T>, IAuditable
{
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}