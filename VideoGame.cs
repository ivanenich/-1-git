using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ivanenko01._03
{
    /// <summary>
    /// Класс для представления видеоигры
    /// </summary>
    [Serializable]
    [DataContract]
    public class VideoGame : IGame
    {
        private string _title;
        private string _developer;
        private int _releaseYear;
        private List<Rating> _ratings;
        private Genre _genre;
        private Platform _platform;

        [DataMember]
        public string Title
        {
            get { return _title; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название игры не может быть пустым");
                _title = value;
            }
        }

        [DataMember]
        public string Developer
        {
            get { return _developer; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Разработчик не может быть пустым");
                _developer = value;
            }
        }

        [DataMember]
        public int ReleaseYear
        {
            get { return _releaseYear; }
            set
            {
                if (value < 1970 || value > DateTime.Now.Year + 1)
                    throw new ArgumentException($"Год выпуска должен быть между 1970 и {DateTime.Now.Year + 1}");
                _releaseYear = value;
            }
        }

        [DataMember]
        public Genre Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }

        [DataMember]
        public Platform Platform
        {
            get { return _platform; }
            set { _platform = value; }
        }

        [DataMember]
        public List<Rating> Ratings
        {
            get { return _ratings; }
            set { _ratings = value ?? new List<Rating>(); }
        }

        [DataMember]
        public int Id { get; set; }

        public VideoGame()
        {
            _ratings = new List<Rating>();
        }

        public VideoGame(string title, string developer, int releaseYear, Genre genre, Platform platform)
        {
            Title = title;
            Developer = developer;
            ReleaseYear = releaseYear;
            Genre = genre;
            Platform = platform;
            _ratings = new List<Rating>();
        }

        public void AddRating(Rating rating)
        {
            if (rating == null)
                throw new ArgumentNullException(nameof(rating));
            _ratings.Add(rating);
        }

        public double GetAverageRating()
        {
            if (_ratings == null || _ratings.Count == 0)
                return 0;
            return _ratings.Average(r => r.Score);
        }

        public string GetInfo()
        {
            return $"ID: {Id}\n" +
                   $"Название: {Title}\n" +
                   $"Разработчик: {Developer}\n" +
                   $"Год выпуска: {ReleaseYear}\n" +
                   $"Жанр: {Genre?.Name ?? "Не указан"}\n" +
                   $"Платформа: {Platform?.Name ?? "Не указана"}\n" +
                   $"Средний рейтинг: {GetAverageRating():F2}/10\n" +
                   $"Количество оценок: {_ratings.Count}";
        }

        public override string ToString()
        {
            return $"{Title} ({ReleaseYear}) - {Developer}";
        }
    }
}

