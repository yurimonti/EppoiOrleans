namespace EppoiBackend.Dtos
{
    public interface IStateToDtoConverter<T,R> where T : IGrain
    {
        Task<R> ConvertToDto(T grain);
    }
}
