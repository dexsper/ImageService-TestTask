using System.ComponentModel.DataAnnotations;

namespace ImageService.Extensions;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;
    
    public MaxFileSizeAttribute(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }

    protected override ValidationResult? IsValid(
        object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        
        if (file == null) 
            return ValidationResult.Success;
        
        return file.Length > _maxFileSize ? new ValidationResult($"Maximum allowed file size is { _maxFileSize} bytes.") : ValidationResult.Success;
    }
}