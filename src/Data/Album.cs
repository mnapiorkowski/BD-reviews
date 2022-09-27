namespace TRreviews.Data;

public class Album
{
    public int Id { get; init; }

    public string Title { get; init; }

    public Artist? Artist { get; set; }

    public List<Review> Reviews { get; set; }

    public int ReviewsCount { get; set; }

    public decimal AvgMark { get; set; }

    public string DisplayName => $"{Title}";

    public bool IsRowExpanded { get; set; } = false;

    public bool IsExpandableSet { get; set; } = false;

    public Album(int albumId, string title) =>
        (Id, Title, Reviews) = (albumId, title, new List<Review>());
}