﻿namespace ImageService.Models;

public class Image
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }

    public User User { get; set; } = null!;
}