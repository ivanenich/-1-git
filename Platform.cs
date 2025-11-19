using System;
using System.Runtime.Serialization;

namespace Ivanenko01._03
{
    /// <summary>
    /// Класс для представления платформы
    /// </summary>
    [Serializable]
    [DataContract]
    public class Platform
    {
        private string _name;
        private string _manufacturer;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название платформы не может быть пустым.");
                _name = value;
            }
        }

        [DataMember]
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value ?? string.Empty; }
        }

        public Platform() { }

        public Platform(string name, string manufacturer = "")
        {
            Name = name;
            Manufacturer = manufacturer;
        }

        public override string ToString()
        {
            return $"{Name} ({Manufacturer})";
        }
    }
}

