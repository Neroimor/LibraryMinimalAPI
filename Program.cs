using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.MapPost("/AddedFilePDF", async ([FromForm] IFormFile file) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest("Файл не выбран или пуст");

    string fileName = file.FileName;
    if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest("Неверный формат файла. Ожидается PDF.");

    var uploadPath = Path.Combine("wwwroot");
    Directory.CreateDirectory(uploadPath);

    var filePath = Path.Combine(uploadPath, fileName);


    using var stream = File.Create(filePath);
    await file.CopyToAsync(stream);

    return Results.Ok("Файл успешно загружен");
});


app.MapGet("/ReadListFiles", async () =>
{
    var uploadPath = Path.Combine("wwwroot");
    if (!Directory.Exists(uploadPath))
        return Results.NotFound("Каталог не найден");

    var files = Directory.GetFiles(uploadPath)
        .Select(Path.GetFileName)
        .ToList();

    if (files.Count == 0)
        return Results.NotFound("Нет файлов в каталоге");
    return Results.Ok(files);
});

app.MapGet("/ReadFilePDF/{fileName}", async (string fileName) =>
{
    if (string.IsNullOrEmpty(fileName))
        return Results.BadRequest("Имя файла не указано");
    var uploadPath = Path.Combine("wwwroot", fileName);

    if (!File.Exists(uploadPath))
        return Results.NotFound("Файл не найден");

    var fileBytes = File.ReadAllBytes(uploadPath);
    return Results.File(fileBytes, "application/pdf", fileName);
});


app.MapPut("/UpdateFilePDF/{fileName}", async (string fileName, [FromForm] IFormFile file) =>
{
    if (string.IsNullOrEmpty(fileName) || file == null || file.Length == 0)
        return Results.BadRequest("Имя файла не указано или файл не выбран");

    fileName = Path.GetFileName(fileName);               // защита от path traversal
    if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest("Неверный формат файла. Ожидается PDF.");

    var filePath = Path.Combine("wwwroot", fileName);
    if (!File.Exists(filePath))
        return Results.NotFound("Файл не найден");

    using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
    await file.CopyToAsync(stream);

    return Results.Ok("Файл успешно обновлен");
});



app.MapDelete("/DeleteFilePDF/{fileName}", async (string fileName) =>
{
    if (string.IsNullOrEmpty(fileName))
        return Results.BadRequest("Имя файла не указано");
    var uploadPath = Path.Combine("wwwroot", fileName);
    if (!File.Exists(uploadPath))
        return Results.NotFound("Файл не найден");
    File.Delete(uploadPath);
    return Results.Ok("Файл успешно удален");
});





app.Run();
