namespace TRreviews.Data;

public class Review
{
    public int Id { get; init; }

    public decimal Mark { get; init; }

    public string Issue { get; init; }

    public string Section { get; init; }

    public int Page { get; init; }

    public Reviewer? Reviewer { get; set; }

    public Album? Album { get; set; }

    public Review(int reviewId, decimal mark, string issue, string sect, int page) =>
        (Id, Mark, Issue, Section, Page) = (reviewId, mark, issue, sect, page);
}