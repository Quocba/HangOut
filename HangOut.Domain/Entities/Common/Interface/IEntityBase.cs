namespace HangOut.Domain.Entities.Common.Interface;

public interface IEntityBase<T>
{
    T Id { get; set; }
}