using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ivanenko01._03
{
    internal class Program
    {
        private static GameLibrary library;
        private const string DataFileName = "gamelibrary.xml";
        private const string JsonFileName = "gamelibrary.json";
        private const string TextFileName = "gamelibrary.txt";

        static void Main(string[] args)
        {
            // Настройка кодировки для корректного отображения русского языка.
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.InputEncoding = Encoding.UTF8;
            }
            catch
            {
                // Если не удалось установить UTF-8, используем кодировку по умолчанию
                try
                {
                    Console.OutputEncoding = Encoding.GetEncoding(1251); // Windows-1251 для русского
                    Console.InputEncoding = Encoding.GetEncoding(1251);
                }
                catch
                {
                    // Используем кодировку по умолчанию системы
                }
            }
            
            // Установка заголовка консоли
            Console.Title = "Управление библиотекой видеоигр";
            
            // Установка локали для русского языка
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = 
                new System.Globalization.CultureInfo("ru-RU");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = 
                new System.Globalization.CultureInfo("ru-RU");

            library = new GameLibrary();
            InitializeDefaultData();

            // Попытка загрузить данные из файла (сначала JSON, потом XML)
            try
            {
                if (File.Exists(JsonFileName))
                {
                    library.LoadFromJsonFile(JsonFileName);
                    Console.WriteLine("Данные успешно загружены из JSON файла.");
                }
                else if (File.Exists(DataFileName))
                {
                    library.LoadFromXmlFile(DataFileName);
                    Console.WriteLine("Данные успешно загружены из XML файла.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Предупреждение: Не удалось загрузить данные: {ex.Message}");
                Console.WriteLine("Будет использована пустая библиотека.");
            }
            finally
            {
                Console.WriteLine();
            }

            bool exit = false;
            while (!exit)
            {
                ShowMainMenu();
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddGame();
                            break;
                        case "2":
                            ViewAllGames();
                            break;
                        case "3":
                            SearchGames();
                            break;
                        case "4":
                            DeleteGame();
                            break;
                        case "5":
                            AddRating();
                            break;
                        case "6":
                            ViewGamesByGenre();
                            break;
                        case "7":
                            ViewGamesByPlatform();
                            break;
                        case "8":
                            ViewTopRatedGames();
                            break;
                        case "9":
                            ManageGenres();
                            break;
                        case "10":
                            ManagePlatforms();
                            break;
                        case "11":
                            ViewOperationHistory();
                            break;
                        case "12":
                            RestoreDeletedGame();
                            break;
                        case "13":
                            SaveToXmlFile();
                            break;
                        case "14":
                            LoadFromXmlFile();
                            break;
                        case "15":
                            SaveToJsonFile();
                            break;
                        case "16":
                            LoadFromJsonFile();
                            break;
                        case "17":
                            ExportToTextFile();
                            break;
                        case "0":
                            exit = true;
                            SaveBeforeExit();
                            break;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                if (!exit)
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void ShowMainMenu()
        {
            const int boxWidth = 50;
            string topBorder = "╔" + new string('═', boxWidth) + "╗";
            string bottomBorder = "╚" + new string('═', boxWidth) + "╝";
            string separator = "╠" + new string('═', boxWidth) + "╣";
            
            Console.WriteLine(topBorder);
            Console.WriteLine(CenterTextInBox("УПРАВЛЕНИЕ БИБЛИОТЕКОЙ ВИДЕОИГР", boxWidth));
            Console.WriteLine(separator);
            Console.WriteLine(FormatMenuItem(1, "Добавить игру", boxWidth));
            Console.WriteLine(FormatMenuItem(2, "Просмотреть все игры", boxWidth));
            Console.WriteLine(FormatMenuItem(3, "Поиск игр", boxWidth));
            Console.WriteLine(FormatMenuItem(4, "Удалить игру", boxWidth));
            Console.WriteLine(FormatMenuItem(5, "Добавить рейтинг к игре", boxWidth));
            Console.WriteLine(FormatMenuItem(6, "Игры по жанру", boxWidth));
            Console.WriteLine(FormatMenuItem(7, "Игры по платформе", boxWidth));
            Console.WriteLine(FormatMenuItem(8, "Топ игр по рейтингу", boxWidth));
            Console.WriteLine(FormatMenuItem(9, "Управление жанрами", boxWidth));
            Console.WriteLine(FormatMenuItem(10, "Управление платформами", boxWidth));
            Console.WriteLine(FormatMenuItem(11, "История операций", boxWidth));
            Console.WriteLine(FormatMenuItem(12, "Восстановить удаленную игру", boxWidth));
            Console.WriteLine(FormatMenuItem(13, "Сохранить в XML", boxWidth));
            Console.WriteLine(FormatMenuItem(14, "Загрузить из XML", boxWidth));
            Console.WriteLine(FormatMenuItem(15, "Сохранить в JSON", boxWidth));
            Console.WriteLine(FormatMenuItem(16, "Загрузить из JSON", boxWidth));
            Console.WriteLine(FormatMenuItem(17, "Экспорт в текстовый файл", boxWidth));
            Console.WriteLine(FormatMenuItem(0, "Выход", boxWidth));
            Console.WriteLine(bottomBorder);
            Console.Write("Выберите действие: ");
        }

        static string FormatMenuItem(int number, string text, int boxWidth)
        {
            // Фиксируем ширину номера в 4 символа для всех пунктов
            // Формат: "1.  ", "10. ", "0.  "
            string numberStr = (number.ToString() + ".").PadRight(4);
            string item = numberStr + text;
            int padding = boxWidth - item.Length;
            if (padding < 0) padding = 0;
            return $"║ {item}{new string(' ', padding)}║";
        }

        static string CenterTextInBox(string text, int boxWidth)
        {
            int padding = (boxWidth - text.Length) / 2;
            int leftPadding = padding;
            int rightPadding = boxWidth - text.Length - leftPadding;
            return $"║{new string(' ', leftPadding)}{text}{new string(' ', rightPadding)}║";
        }

        static void AddGame()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОЙ ИГРЫ ===");
            try
            {
                Console.Write("Название игры: ");
                string title = Console.ReadLine();

                Console.Write("Разработчик: ");
                string developer = Console.ReadLine();

                Console.Write("Год выпуска: ");
                if (!int.TryParse(Console.ReadLine(), out int year))
                {
                    throw new ArgumentException("Неверный формат года");
                }

                Console.WriteLine("\nДоступные жанры:");
                ShowGenres();
                Console.Write("Введите название жанра (или новое): ");
                string genreName = Console.ReadLine();
                Genre genre = library.GetGenre(genreName);
                if (genre == null && !string.IsNullOrWhiteSpace(genreName))
                {
                    Console.Write("Описание жанра: ");
                    string description = Console.ReadLine();
                    genre = new Genre(genreName, description);
                    library.AddGenre(genre);
                }

                Console.WriteLine("\nДоступные платформы:");
                ShowPlatforms();
                Console.Write("Введите название платформы (или новое): ");
                string platformName = Console.ReadLine();
                Platform platform = library.GetPlatform(platformName);
                if (platform == null && !string.IsNullOrWhiteSpace(platformName))
                {
                    Console.Write("Производитель: ");
                    string manufacturer = Console.ReadLine();
                    platform = new Platform(platformName, manufacturer);
                    library.AddPlatform(platform);
                }

                VideoGame game = new VideoGame(title, developer, year, genre, platform);
                library.AddGame(game);
                Console.WriteLine($"\nИгра '{title}' успешно добавлена! ID: {game.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении игры: {ex.Message}");
                throw;
            }
        }

        static void ViewAllGames()
        {
            Console.WriteLine("\n=== ВСЕ ИГРЫ В БИБЛИОТЕКЕ ===");
            var games = library.Games;
            if (games.Count == 0)
            {
                Console.WriteLine("Библиотека пуста.");
                return;
            }

            foreach (var game in games)
            {
                Console.WriteLine("\n" + game.GetInfo());
                if (game.Ratings.Count > 0)
                {
                    Console.WriteLine("\nРейтинги:");
                    foreach (var rating in game.Ratings)
                    {
                        Console.WriteLine($"  - {rating}");
                    }
                }
                Console.WriteLine(new string('-', 50));
            }
            Console.WriteLine($"\nВсего игр: {games.Count}");
        }

        static void SearchGames()
        {
            Console.WriteLine("\n=== ПОИСК ИГР ===");
            Console.Write("Введите поисковый запрос: ");
            string searchTerm = Console.ReadLine();

            var results = library.SearchGames(searchTerm);
            if (results.Count == 0)
            {
                Console.WriteLine("Игры не найдены.");
                return;
            }

            Console.WriteLine($"\nНайдено игр: {results.Count}");
            foreach (var game in results)
            {
                Console.WriteLine($"\n{game.GetInfo()}");
                Console.WriteLine(new string('-', 50));
            }
        }

        static void DeleteGame()
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ ИГРЫ ===");
            ViewAllGames();
            Console.Write("\nВведите ID игры для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (library.RemoveGame(id))
                {
                    Console.WriteLine("Игра успешно удалена.");
                }
                else
                {
                    Console.WriteLine("Игра с таким ID не найдена.");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID.");
            }
        }

        static void AddRating()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ РЕЙТИНГА ===");
            ViewAllGames();
            Console.Write("\nВведите ID игры: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Неверный формат ID.");
                return;
            }

            var game = library.FindGameById(id);
            if (game == null)
            {
                Console.WriteLine("Игра не найдена.");
                return;
            }

            try
            {
                Console.Write("Имя игрока: ");
                string playerName = Console.ReadLine();

                Console.Write("Оценка (0-10): ");
                if (!double.TryParse(Console.ReadLine(), out double score))
                {
                    throw new ArgumentException("Неверный формат оценки");
                }

                Console.Write("Комментарий (необязательно): ");
                string comment = Console.ReadLine();

                Rating rating = new Rating(playerName, score, comment);
                game.AddRating(rating);
                Console.WriteLine($"Рейтинг успешно добавлен! Средний рейтинг игры: {game.GetAverageRating():F2}/10");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ViewGamesByGenre()
        {
            Console.WriteLine("\n=== ИГРЫ ПО ЖАНРУ ===");
            ShowGenres();
            Console.Write("\nВведите название жанра: ");
            string genreName = Console.ReadLine();

            var games = library.GetGamesByGenre(genreName);
            if (games.Count == 0)
            {
                Console.WriteLine("Игры не найдены.");
                return;
            }

            Console.WriteLine($"\nНайдено игр: {games.Count}");
            foreach (var game in games)
            {
                Console.WriteLine($"\n{game.GetInfo()}");
                Console.WriteLine(new string('-', 50));
            }
        }

        static void ViewGamesByPlatform()
        {
            Console.WriteLine("\n=== ИГРЫ ПО ПЛАТФОРМЕ ===");
            ShowPlatforms();
            Console.Write("\nВведите название платформы: ");
            string platformName = Console.ReadLine();

            var games = library.GetGamesByPlatform(platformName);
            if (games.Count == 0)
            {
                Console.WriteLine("Игры не найдены.");
                return;
            }

            Console.WriteLine($"\nНайдено игр: {games.Count}");
            foreach (var game in games)
            {
                Console.WriteLine($"\n{game.GetInfo()}");
                Console.WriteLine(new string('-', 50));
            }
        }

        static void ViewTopRatedGames()
        {
            Console.WriteLine("\n=== ТОП ИГР ПО РЕЙТИНГУ ===");
            var topGames = library.Games
                .Where(g => g.Ratings.Count > 0)
                .OrderByDescending(g => g.GetAverageRating())
                .Take(10)
                .ToList();

            if (topGames.Count == 0)
            {
                Console.WriteLine("Нет игр с рейтингами.");
                return;
            }

            int position = 1;
            foreach (var game in topGames)
            {
                Console.WriteLine($"{position}. {game.Title} - {game.GetAverageRating():F2}/10 ({game.Ratings.Count} оценок)");
                position++;
            }
        }

        static void ManageGenres()
        {
            Console.WriteLine("\n=== УПРАВЛЕНИЕ ЖАНРАМИ ===");
            ShowGenres();
            Console.WriteLine("\n1. Добавить жанр");
            Console.WriteLine("2. Назад");
            Console.Write("Выберите действие: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                try
                {
                    Console.Write("Название жанра: ");
                    string name = Console.ReadLine();
                    Console.Write("Описание: ");
                    string description = Console.ReadLine();
                    Genre genre = new Genre(name, description);
                    library.AddGenre(genre);
                    Console.WriteLine("Жанр успешно добавлен!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        static void ManagePlatforms()
        {
            Console.WriteLine("\n=== УПРАВЛЕНИЕ ПЛАТФОРМАМИ ===");
            ShowPlatforms();
            Console.WriteLine("\n1. Добавить платформу");
            Console.WriteLine("2. Назад");
            Console.Write("Выберите действие: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                try
                {
                    Console.Write("Название платформы: ");
                    string name = Console.ReadLine();
                    Console.Write("Производитель: ");
                    string manufacturer = Console.ReadLine();
                    Platform platform = new Platform(name, manufacturer);
                    library.AddPlatform(platform);
                    Console.WriteLine("Платформа успешно добавлена!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        static void ShowGenres()
        {
            var genres = library.Genres.Values;
            if (genres.Count == 0)
            {
                Console.WriteLine("Жанры не добавлены.");
                return;
            }
            foreach (var genre in genres)
            {
                Console.WriteLine($"  - {genre}");
            }
        }

        static void ShowPlatforms()
        {
            var platforms = library.Platforms.Values;
            if (platforms.Count == 0)
            {
                Console.WriteLine("Платформы не добавлены.");
                return;
            }
            foreach (var platform in platforms)
            {
                Console.WriteLine($"  - {platform}");
            }
        }

        static void ViewOperationHistory()
        {
            library.PrintOperationHistory();
        }

        static void RestoreDeletedGame()
        {
            Console.WriteLine("\n=== ВОССТАНОВЛЕНИЕ УДАЛЕННОЙ ИГРЫ ===");
            var game = library.RestoreLastDeleted();
            if (game != null)
            {
                Console.WriteLine($"Игра '{game.Title}' успешно восстановлена!");
            }
            else
            {
                Console.WriteLine("Нет удаленных игр для восстановления.");
            }
        }

        static void SaveToXmlFile()
        {
            Console.WriteLine("\n=== СОХРАНЕНИЕ В XML ФАЙЛ ===");
            Console.Write($"Имя файла (по умолчанию: {DataFileName}): ");
            string fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = DataFileName;
            }

            try
            {
                library.SaveToXmlFile(fileName);
                Console.WriteLine($"Данные успешно сохранены в XML файл '{fileName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                throw;
            }
        }

        static void LoadFromXmlFile()
        {
            Console.WriteLine("\n=== ЗАГРУЗКА ИЗ XML ФАЙЛА ===");
            Console.Write($"Имя файла (по умолчанию: {DataFileName}): ");
            string fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = DataFileName;
            }

            try
            {
                library.LoadFromXmlFile(fileName);
                Console.WriteLine($"Данные успешно загружены из XML файла '{fileName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке: {ex.Message}");
                throw;
            }
        }

        static void SaveToJsonFile()
        {
            Console.WriteLine("\n=== СОХРАНЕНИЕ В JSON ФАЙЛ ===");
            Console.Write($"Имя файла (по умолчанию: {JsonFileName}): ");
            string fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = JsonFileName;
            }

            try
            {
                library.SaveToJsonFile(fileName);
                Console.WriteLine($"Данные успешно сохранены в JSON файл '{fileName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                throw;
            }
        }

        static void LoadFromJsonFile()
        {
            Console.WriteLine("\n=== ЗАГРУЗКА ИЗ JSON ФАЙЛА ===");
            Console.Write($"Имя файла (по умолчанию: {JsonFileName}): ");
            string fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = JsonFileName;
            }

            try
            {
                library.LoadFromJsonFile(fileName);
                Console.WriteLine($"Данные успешно загружены из JSON файла '{fileName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке: {ex.Message}");
                throw;
            }
        }

        static void ExportToTextFile()
        {
            Console.WriteLine("\n=== ЭКСПОРТ В ТЕКСТОВЫЙ ФАЙЛ ===");
            Console.Write($"Имя файла (по умолчанию: {TextFileName}): ");
            string fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = TextFileName;
            }

            try
            {
                library.SaveToTextFile(fileName);
                Console.WriteLine($"Данные успешно экспортированы в файл '{fileName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при экспорте: {ex.Message}");
                throw;
            }
        }

        static void SaveBeforeExit()
        {
            try
            {
                // Сохраняем в оба формата при выходе
                library.SaveToXmlFile(DataFileName);
                library.SaveToJsonFile(JsonFileName);
                Console.WriteLine("\nДанные автоматически сохранены в XML и JSON форматах.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПредупреждение: Не удалось сохранить данные: {ex.Message}");
            }
        }

        static void InitializeDefaultData()
        {
            try
            {
                // Добавляем несколько жанров по умолчанию
                library.AddGenre(new Genre("Action", "Экшн игры с динамичным геймплеем"));
                library.AddGenre(new Genre("RPG", "Ролевые игры"));
                library.AddGenre(new Genre("Strategy", "Стратегические игры"));
                library.AddGenre(new Genre("Adventure", "Приключенческие игры"));
                library.AddGenre(new Genre("Simulation", "Симуляторы"));

                // Добавляем несколько платформ по умолчанию
                library.AddPlatform(new Platform("PC", "Различные"));
                library.AddPlatform(new Platform("PlayStation 5", "Sony"));
                library.AddPlatform(new Platform("Xbox Series X", "Microsoft"));
                library.AddPlatform(new Platform("Nintendo Switch", "Nintendo"));
            }
            catch
            {
                // Игнорируем ошибки при инициализации
            }
        }
    }
}
