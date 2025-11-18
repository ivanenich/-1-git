using System;
using System.Runtime.Serialization;

namespace Ivanenko01._03
{
    /// <summary>
    /// Класс для представления рейтинга игры от игрока
    /// </summary>
    [Serializable]
    [DataContract]
    public class Rating
    {
        private string _playerName;
        private double _score;
        private string _comment;

        [DataMember]
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя игрока не может быть пустым");
                _playerName = value;
            }
        }

        [DataMember]
        public double Score
        {
            get { return _score; }
            set
            {
                if (value < 0 || value > 10)
                    throw new ArgumentException("Оценка должна быть от 0 до 10");
                _score = value;
            }
        }

        [DataMember]
        public string Comment
        {
            get { return _comment; }
            set { _comment = value ?? string.Empty; }
        }

        [DataMember]
        public DateTime RatingDate { get; set; }

        public Rating() 
        {
            RatingDate = DateTime.Now;
        }

        public Rating(string playerName, double score, string comment = "")
        {
            PlayerName = playerName;
            Score = score;
            Comment = comment;
            RatingDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{PlayerName}: {Score}/10 - {Comment} ({RatingDate:dd.MM.yyyy})";
        }
    }
}

