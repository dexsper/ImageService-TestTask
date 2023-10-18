﻿using System.ComponentModel.DataAnnotations;

namespace ImageService.Extensions;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }
    
    protected override ValidationResult? IsValid(
        object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file) 
            return ValidationResult.Success;
        
        var extension = Path.GetExtension(file.FileName);
        
        if (!_extensions.Contains(extension.ToLower()))
        {
            return new ValidationResult("This photo extension is not allowed!");
        }

        return ValidationResult.Success;
    }
}