using BrowserService.Interfaces;
using BrowserService.Services;
using Infrastructure.Database.FileSystem;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IFileBrowserService, FileBrowserService>();
builder.Services.AddDbContext<FileSystemDbContext>(options =>
{
    options.UseInMemoryDatabase("FileSystemDb");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
