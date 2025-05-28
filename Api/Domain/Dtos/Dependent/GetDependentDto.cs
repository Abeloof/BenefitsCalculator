using Api.Domain.Models;

namespace Api.Domain.Dtos.Dependent;

public class GetDependentDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; } = default;
    public string Relationship { get; set; } = string.Empty;
}
