using Figgle;

string directoryPath;
Vector3 coordinates = new();


Initialize();

while (true)
{
    Console.Clear();
    ShowLogo();
    Console.WriteLine("Выберите действие:");
    Console.WriteLine("1 - Показать имена всех сундуков");
    Console.WriteLine("2 - Просмотр содержимого файла по имени");
    Console.WriteLine("3 - Поиск предмета в .chest файлах");
    Console.WriteLine("4-9 - (Зарезервировано)");
    Console.WriteLine("E - Выход");

    var key = Console.ReadKey(true).Key;

    if (key == ConsoleKey.E)
        break;

    switch (key)
    {
        case ConsoleKey.D1:
        case ConsoleKey.NumPad1:
            ShowAllChestFileNames();
            break;

        case ConsoleKey.D2:
        case ConsoleKey.NumPad2:
            ViewFileContentsByName();
            break;

        case ConsoleKey.D3:
        case ConsoleKey.NumPad3:
            SearchInChestFiles();
            break;

        default:
            Console.WriteLine("Функция пока не реализована.");
            WaitForKey();
            break;
    }
}


void Initialize()
{
    while (true)
    {
        Console.Clear();
        Console.Write("Введите путь к директории: ");
        directoryPath = Console.ReadLine();

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("Директория не существует. Попробуйте снова.");
            WaitForKey();
            continue;
        }

        Console.Write("Введите координаты (x/z/y): ");
        string input = Console.ReadLine();

        // Использование координат по-умолчанию, если координаты не вводились
        if (string.IsNullOrEmpty(input)) break;

        // Обработка ввода координат
        var parts = input.Split(new char[] { ' ', '/', '.' });
        if (parts.Length != 3 ||
            !int.TryParse(parts[0], out int x) ||
            !int.TryParse(parts[2], out int y) ||
            !int.TryParse(parts[1], out int z))
        {
            Console.WriteLine("Неверный формат координат. Пример: 10/20/5");
            WaitForKey();
            continue;
        }

        coordinates = new Vector3(x, y, z);
        break;
    }
}

// Поиск подстроки в .chest файлах
void SearchInChestFiles()
{
    Console.Clear();
    Console.Write("Введите подстроку для поиска: ");
    string substring = Console.ReadLine();

    Console.Clear();
    Console.WriteLine($"Координаты области: {coordinates}\n");
    Console.WriteLine("Сканирование файлов...\n");

    List<string> chestFiles;

    try
    {
        chestFiles = new List<string>(
            Directory.EnumerateFiles(directoryPath, "*.chest", SearchOption.AllDirectories)
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при получении списка файлов: {ex.Message}");
        WaitForKey();
        return;
    }

    int total = chestFiles.Count;
    int processed = 0;

    var results = new List<(double Distance, string Result)>();

    foreach (var file in chestFiles)
    {
        processed++;
        Console.SetCursorPosition(0, 3);
        Console.WriteLine($"Всего файлов: {total}");
        Console.WriteLine($"Обработано: {processed}/{total}");
        Console.WriteLine($"Файл: {file}");
        Console.WriteLine(new string('-', 60));

        try
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(substring, StringComparison.OrdinalIgnoreCase))
                {
                    var chestPos = ParseCoordinatesFromFilename(file);
                    double distance = Vector3.Distance(coordinates, chestPos);

                    var result = $"Файл: {file}\nКоординаты: {chestPos}\nСтрока {i + 1}: {lines[i]}\n";
                    results.Add((distance, result));
                }
            }
        }
        catch (Exception ex)
        {
            results.Add((double.MaxValue, $"Ошибка в {file}: {ex.Message}"));
        }
    }

    Console.SetCursorPosition(0, 7);
    Console.WriteLine($"\nПоиск завершён. Совпадений: {results.Count}\n");

    foreach (var result in results.OrderBy(r => r.Distance))
    {
        Console.WriteLine(result.Result);
    }

    Console.WriteLine("\nНажмите любую клавишу, чтобы вернуться...");
    Console.ReadKey(true);
}

// Вывод всех сгенерированных сундуков
void ShowAllChestFileNames()
{
    Console.Clear();
    Console.WriteLine("Поиск сундуков...\n");

    HashSet<string> chestNames = new();

    try
    {
        var files = Directory.EnumerateFiles(directoryPath, "*.chest", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            chestNames.Add(name);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при поиске сундуков: {ex.Message}");
        WaitForKey();
        return;
    }

    if (chestNames.Count == 0)
    {
        Console.WriteLine("Сундуки не найдены.");
    }
    else
    {
        var sortedNames = chestNames.OrderBy(name => name).ToList();
        Console.WriteLine($"Найдено сундуков: {sortedNames.Count}\n");
        foreach (var name in sortedNames)
        {
            Console.WriteLine(name);
        }
    }

    WaitForKey();
}

// Просмотр содержимого файла по имени
void ViewFileContentsByName()
{
    Console.Clear();
    Console.Write("Введите имя файла (без расширения): ");
    string targetName = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(targetName))
    {
        Console.WriteLine("Имя файла не может быть пустым.");
        WaitForKey();
        return;
    }

    List<string> matchingFiles;
    try
    {
        matchingFiles = new List<string>(
            Directory.EnumerateFiles(directoryPath, "*.chest", SearchOption.AllDirectories)
            .Where(path => Path.GetFileNameWithoutExtension(path).Equals(targetName, StringComparison.OrdinalIgnoreCase))
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при поиске файлов: {ex.Message}");
        WaitForKey();
        return;
    }

    if (matchingFiles.Count == 0)
    {
        Console.WriteLine("Файлы с таким именем не найдены.");
        WaitForKey();
        return;
    }

    for (int i = 0; i < matchingFiles.Count; i++)
    {
        Console.Clear();
        string file = matchingFiles[i];
        Console.WriteLine($"[{i + 1}/{matchingFiles.Count}] Файл: {file}\n");
        try
        {
            string[] lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении: {ex.Message}");
        }

        if (i < matchingFiles.Count - 1)
        {
            Console.WriteLine("\nНажмите любую клавишу для следующего файла...");
            Console.ReadKey(true);
        }
    }

    Console.WriteLine("\nКонец просмотра. Нажмите любую клавишу...");
    Console.ReadKey(true);
}


void ShowLogo()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(FiggleFonts.Standard.Render("Chest Searcher"));
    Console.ForegroundColor = ConsoleColor.White;
}

void WaitForKey()
{
    Console.WriteLine("\nНажмите любую клавишу...");
    Console.ReadKey(true);
}

Vector3 ParseCoordinatesFromFilename(string filePath)
{
    string fileName = Path.GetFileNameWithoutExtension(filePath);
    var coords = fileName.Split('_');

    if (coords.Length == 3 &&
        int.TryParse(coords[0], out int x) &&
        int.TryParse(coords[2], out int y) &&
        int.TryParse(coords[1], out int z))
    {
        return new Vector3(x, y, z);
    }

    return new Vector3(); // default (0, 0, 0) if parsing fails
}
