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
        return Results.BadRequest("���� �� ������ ��� ����");

    string fileName = file.FileName;
    if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest("�������� ������ �����. ��������� PDF.");

    var uploadPath = Path.Combine("wwwroot");
    Directory.CreateDirectory(uploadPath);

    var filePath = Path.Combine(uploadPath, fileName);


    using var stream = File.Create(filePath);
    await file.CopyToAsync(stream);

    return Results.Ok("���� ������� ��������");
});


app.MapGet("/ReadListFiles", async () =>
{
    var uploadPath = Path.Combine("wwwroot");
    if (!Directory.Exists(uploadPath))
        return Results.NotFound("������� �� ������");

    var files = Directory.GetFiles(uploadPath)
        .Select(Path.GetFileName)
        .ToList();

    if (files.Count == 0)
        return Results.NotFound("��� ������ � ��������");
    return Results.Ok(files);
});

app.MapGet("/ReadFilePDF/{fileName}", async (string fileName) =>
{
    if (string.IsNullOrEmpty(fileName))
        return Results.BadRequest("��� ����� �� �������");
    var uploadPath = Path.Combine("wwwroot", fileName);

    if (!File.Exists(uploadPath))
        return Results.NotFound("���� �� ������");

    var fileBytes = File.ReadAllBytes(uploadPath);
    return Results.File(fileBytes, "application/pdf", fileName);
});


app.MapPut("/UpdateFilePDF/{fileName}", async (string fileName, [FromForm] IFormFile file) =>
{
    if (string.IsNullOrEmpty(fileName) || file == null || file.Length == 0)
        return Results.BadRequest("��� ����� �� ������� ��� ���� �� ������");

    fileName = Path.GetFileName(fileName);               // ������ �� path traversal
    if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest("�������� ������ �����. ��������� PDF.");

    var filePath = Path.Combine("wwwroot", fileName);
    if (!File.Exists(filePath))
        return Results.NotFound("���� �� ������");

    using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
    await file.CopyToAsync(stream);

    return Results.Ok("���� ������� ��������");
});



app.MapDelete("/DeleteFilePDF/{fileName}", async (string fileName) =>
{
    if (string.IsNullOrEmpty(fileName))
        return Results.BadRequest("��� ����� �� �������");
    var uploadPath = Path.Combine("wwwroot", fileName);
    if (!File.Exists(uploadPath))
        return Results.NotFound("���� �� ������");
    File.Delete(uploadPath);
    return Results.Ok("���� ������� ������");
});





app.Run();
