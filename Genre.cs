using System;
using System.Runtime.Serialization;

namespace Ivanenko01._03
{
    /// <summary>
    /// Класс для представления жанра игры
    /// </summary>
    [Serializable]
    [DataContract]
    public class Genre
    {
        private string _name;
        private string _description;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название жанра не может быть пустым");
                _name = value;
            }
        }

        [DataMember]
        public string Description
        {
            get { return _description; }
            set { _description = value ?? string.Empty; }
        }

        public Genre() { }

        public Genre(string name, string description = "")
        {
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Name} - {Description}";
        }
    }
}

