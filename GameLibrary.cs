using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace Ivanenko01._03
{
    /// <summary>
    /// Класс для управления библиотекой игр
    /// </summary>
    public class GameLibrary
    {
        private List<VideoGame> _games;
        private Dictionary<string, Genre> _genres;
        private Dictionary<string, Platform> _platforms;
        private Queue<string> _operationHistory;
        private Stack<VideoGame> _recentlyDeleted;
        private int _nextId;

        public List<VideoGame> Games
        {
            get { return _games; }
            private set { _games = value; }
        }

        public Dictionary<string, Genre> Genres
        {
            get { return _genres; }
            private set { _genres = value; }
        }

        public Dictionary<string, Platform> Platforms
        {
            get { return _platforms; }
            private set { _platforms = value; }
        }

        public GameLibrary()
        {
            _games = new List<VideoGame>();
            _genres = new Dictionary<string, Genre>();
            _platforms = new Dictionary<string, Platform>();
            _operationHistory = new Queue<string>();
            _recentlyDeleted = new Stack<VideoGame>();
            _nextId = 1;
        }

        public void AddGame(VideoGame game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            game.Id = _nextId++;
            _games.Add(game);
            _operationHistory.Enqueue($"Добавлена игра: {game.Title} ({DateTime.Now:dd.MM.yyyy HH:mm:ss})");
            
            // Ограничиваем историю последними 50 операциями
            while (_operationHistory.Count > 50)
            {
                _operationHistory.Dequeue();
            }
        }

        public bool RemoveGame(int id)
        {
            var game = _games.FirstOrDefault(g => g.Id == id);
            if (game != null)
            {
                _games.Remove(game);
                _recentlyDeleted.Push(game);
                _operationHistory.Enqueue($"Удалена игра: {game.Title} ({DateTime.Now:dd.MM.yyyy HH:mm:ss})");
                
                // Ограничиваем стек последними 10 удаленными играми
                if (_recentlyDeleted.Count > 10)
                {
                    var temp = new Stack<VideoGame>();
                    int count = 0;
                    while (_recentlyDeleted.Count > 0 && count < 10)
                    {
                        temp.Push(_recentlyDeleted.Pop());
                        count++;
                    }
                    _recentlyDeleted = temp;
                }
                return true;
            }
            return false;
        }

        public VideoGame FindGameById(int id)
        {
            return _games.FirstOrDefault(g => g.Id == id);
        }

        public List<VideoGame> SearchGames(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<VideoGame>();

            // Использование StringComparison для корректного поиска с учетом русских символов
            StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            return _games.Where(g =>
                g.Title.IndexOf(searchTerm, comparison) >= 0 ||
                g.Developer.IndexOf(searchTerm, comparison) >= 0 ||
                (g.Genre != null && g.Genre.Name.IndexOf(searchTerm, comparison) >= 0) ||
                (g.Platform != null && g.Platform.Name.IndexOf(searchTerm, comparison) >= 0)
            ).ToList();
        }

        public List<VideoGame> GetGamesByGenre(string genreName)
        {
            // Использование CurrentCultureIgnoreCase для корректного сравнения русских строк
            return _games.Where(g => g.Genre != null && g.Genre.Name.Equals(genreName, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        public List<VideoGame> GetGamesByPlatform(string platformName)
        {
            // Использование CurrentCultureIgnoreCase для корректного сравнения русских строк
            return _games.Where(g => g.Platform != null && g.Platform.Name.Equals(platformName, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        public void AddGenre(Genre genre)
        {
            if (genre == null)
                throw new ArgumentNullException(nameof(genre));
            _genres[genre.Name] = genre;
        }

        public void AddPlatform(Platform platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));
            _platforms[platform.Name] = platform;
        }

        public Genre GetGenre(string name)
        {
            return _genres.ContainsKey(name) ? _genres[name] : null;
        }

        public Platform GetPlatform(string name)
        {
            return _platforms.ContainsKey(name) ? _platforms[name] : null;
        }

        public void SaveToXmlFile(string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(GameLibraryData));
                var data = new GameLibraryData
                {
                    Games = _games,
                    Genres = _genres.Values.ToList(),
                    Platforms = _platforms.Values.ToList(),
                    NextId = _nextId
                };

                // Использование UTF-8 кодировки для корректного сохранения русских символов
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    serializer.Serialize(writer, data);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении XML файла: {ex.Message}", ex);
            }
        }

        public void LoadFromXmlFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Файл не найден: {filePath}");
                }

                var serializer = new XmlSerializer(typeof(GameLibraryData));
                GameLibraryData data;

                // Использование UTF-8 кодировки для корректного чтения русских символов
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    data = (GameLibraryData)serializer.Deserialize(reader);
                }

                LoadDataFromGameLibraryData(data);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке XML файла: {ex.Message}", ex);
            }
        }

        public void SaveToJsonFile(string filePath)
        {
            try
            {
                var data = new GameLibraryData
                {
                    Games = _games,
                    Genres = _genres.Values.ToList(),
                    Platforms = _platforms.Values.ToList(),
                    NextId = _nextId
                };

                var serializer = new DataContractJsonSerializer(typeof(GameLibraryData));
                
                // Использование UTF-8 кодировки для корректного сохранения русских символов
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    serializer.WriteObject(stream, data);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении JSON файла: {ex.Message}", ex);
            }
        }

        public void LoadFromJsonFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Файл не найден: {filePath}");
                }

                var serializer = new DataContractJsonSerializer(typeof(GameLibraryData));
                GameLibraryData data;

                // Использование UTF-8 кодировки для корректного чтения русских символов
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    data = (GameLibraryData)serializer.ReadObject(stream);
                }

                LoadDataFromGameLibraryData(data);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке JSON файла: {ex.Message}", ex);
            }
        }

        private void LoadDataFromGameLibraryData(GameLibraryData data)
        {
            _games = data.Games ?? new List<VideoGame>();
            _nextId = data.NextId;

            _genres.Clear();
            if (data.Genres != null)
            {
                foreach (var genre in data.Genres)
                {
                    _genres[genre.Name] = genre;
                }
            }

            _platforms.Clear();
            if (data.Platforms != null)
            {
                foreach (var platform in data.Platforms)
                {
                    _platforms[platform.Name] = platform;
                }
            }
        }

        public void SaveToTextFile(string filePath)
        {
            try
            {
                // Использование UTF-8 кодировки для корректного сохранения русских символов
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("=== БИБЛИОТЕКА ВИДЕОИГР ===");
                    writer.WriteLine($"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                    writer.WriteLine($"Всего игр: {_games.Count}");
                    writer.WriteLine();

                    foreach (var game in _games)
                    {
                        writer.WriteLine(game.GetInfo());
                        writer.WriteLine("---");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении текстового файла: {ex.Message}", ex);
            }
        }

        public void PrintOperationHistory()
        {
            Console.WriteLine("\n=== История операций ===");
            if (_operationHistory.Count == 0)
            {
                Console.WriteLine("История пуста.");
                return;
            }

            foreach (var operation in _operationHistory)
            {
                Console.WriteLine(operation);
            }
        }

        public VideoGame RestoreLastDeleted()
        {
            if (_recentlyDeleted.Count > 0)
            {
                var game = _recentlyDeleted.Pop();
                _games.Add(game);
                _operationHistory.Enqueue($"Восстановлена игра: {game.Title} ({DateTime.Now:dd.MM.yyyy HH:mm:ss})");
                return game;
            }
            return null;
        }
    }

    /// <summary>
    /// Вспомогательный класс для сериализации
    /// </summary>
    [Serializable]
    [DataContract]
    [XmlRoot("GameLibraryData")]
    public class GameLibraryData
    {
        [DataMember]
        [XmlArray("Games")]
        [XmlArrayItem("VideoGame")]
        public List<VideoGame> Games { get; set; }

        [DataMember]
        [XmlArray("Genres")]
        [XmlArrayItem("Genre")]
        public List<Genre> Genres { get; set; }

        [DataMember]
        [XmlArray("Platforms")]
        [XmlArrayItem("Platform")]
        public List<Platform> Platforms { get; set; }

        [DataMember]
        public int NextId { get; set; }
    }
}

