namespace ImageService.Services;

public class ITaskResult<T>
{
    public T? Result { get; init; }
    public required bool Succeeded { get; init; }
    public string Error { get; init; }
}