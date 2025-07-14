# LibraryMinimalAPI

Простой **Minimal API на .NET 9** для управления PDF-файлами. Проект позволяет загружать, просматривать, скачивать, обновлять и удалять PDF-документы, хранящиеся в папке `wwwroot`.

## Возможности

- **Загрузка** новых PDF-файлов
    
- **Получение списка** сохранённых PDF
    
- **Скачивание** конкретного PDF
    
- **Обновление** (перезапись) существующих PDF
    
- **Удаление** PDF
    
- Раздача статических файлов из `wwwroot`
    

## Требования

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
    
- Git
    
- Postman или любой HTTP‑клиент для тестирования
    

## Быстрый старт

1. **Клонировать репозиторий**
    
    ```bash
    git clone https://github.com/Neroimor/LibraryMinimalAPI.git
    cd LibraryMinimalAPI
    ```
    
2. **Собрать проект**
    
    ```bash
    dotnet build
    ```
    
3. **Запустить API**
    
    ```bash
    dotnet run
    ```
    
    По умолчанию приложение слушает по адресу `https://localhost:7182`.
    

## Эндпоинты API

|Метод|Маршрут|Описание|
|---|---|---|
|**POST**|`/AddedFilePDF`|Загрузить новый PDF. Параметр формы: `file`|
|**GET**|`/ReadListFiles`|Получить список имён всех PDF-файлов|
|**GET**|`/ReadFilePDF/{fileName}`|Скачивание указанного PDF|
|**PUT**|`/UpdateFilePDF/{fileName}`|Перезаписать существующий PDF. Параметр формы: `file`|
|**DELETE**|`/DeleteFilePDF/{fileName}`|Удалить указанный PDF-файл|

> **Важно:** имя файла во всех маршрутах очищается через `Path.GetFileName` для защиты от path traversal. Допускаются только файлы с расширением `.pdf`.

## Конфигурация

Дополнительной настройки не требуется. Используются стандартные middleware для HTTPS и статических файлов.

### Отключение CSRF-проверки

Для удобного тестирования из Postman у маршрутов загрузки и обновления файлов отключена проверка CSRF/anti-forgery:

```csharp
app.MapPost("/AddedFilePDF", …).DisableAntiforgery();
app.MapPut("/UpdateFilePDF/{fileName}", …).DisableAntiforgery();
```

## Скриншоты

