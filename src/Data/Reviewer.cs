namespace TRreviews.Data;

public class Reviewer
{
    public int Id { get; init; }

    public string Name { get; init; }

    public int AlbumsReviewed { get; set; }

    public decimal AvgMark { get; set; }

    public List<int> Marks { get; set; }

    public string DisplayName => $"{Name}";

    public bool IsRowExpanded { get; set; } = false;

    public bool IsExpandableSet { get; set; } = false;

    public Reviewer(int reviewerId, string name) =>
        (Id, Name, Marks) = (reviewerId, name, new List<int>());

}