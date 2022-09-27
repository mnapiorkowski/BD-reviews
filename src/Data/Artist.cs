namespace TRreviews.Data;

public class Artist
{
    public int Id { get; init; }

    public string Name { get; init; }

    public int ReviewsCount { get; set; }

    public List<Album> Albums { get; set; }

    public string DisplayName => $"{Name}";

    public bool IsRowExpanded { get; set; } = false;

    public bool IsExpandableSet { get; set; } = false;

    public Artist(int artistId, string name) =>
        (Id, Name, Albums) = (artistId, name, new List<Album>());
}