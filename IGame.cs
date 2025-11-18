using System;

namespace Ivanenko01._03
{
    /// <summary>
    /// Интерфейс для игр
    /// </summary>
    public interface IGame
    {
        string Title { get; set; }
        string Developer { get; set; }
        int ReleaseYear { get; set; }
        double GetAverageRating();
        string GetInfo();
    }
}

