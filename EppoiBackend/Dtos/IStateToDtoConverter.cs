namespace EppoiBackend.Dtos
{
    //TODO: change implementation of that interface and its classes
    public interface IStateToDtoConverter<T,R> where T : IGrain
    {
        Task<R> ConvertToDto(T grain);
    }
}
