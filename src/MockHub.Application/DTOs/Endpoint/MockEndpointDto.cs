using MockHub.Domain.Enums;

namespace MockHub.Application.DTOs.Endpoint;

public class MockEndpointDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Route { get; set; } = string.Empty;
    public HttpMethodType Method { get; set; }
    public bool IsActive { get; set; }
    public int Order { get; set; }
    public bool IsWildcard { get; set; }
    public string? RegexPattern { get; set; }
    public ResponseSelectionMode ResponseMode { get; set; }
    public int? DelayMin { get; set; }
    public int? DelayMax { get; set; }
    public int ResponseCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MockResponseDto>? Responses { get; set; }
}

public class MockResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string? Body { get; set; }
    public string ContentType { get; set; } = "application/json";
    public string? Headers { get; set; }
    public bool IsDefault { get; set; }
}

public class CreateMockEndpointDto
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Route { get; set; } = string.Empty;
    public HttpMethodType Method { get; set; } = HttpMethodType.GET;
    public bool IsWildcard { get; set; } = false;
    public string? RegexPattern { get; set; }
    public ResponseSelectionMode ResponseMode { get; set; } = ResponseSelectionMode.Sequential;
    public int? DelayMin { get; set; }
    public int? DelayMax { get; set; }
}

public class UpdateMockEndpointDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Route { get; set; } = string.Empty;
    public HttpMethodType Method { get; set; }
    public bool IsActive { get; set; } = true;
    public int Order { get; set; }
    public bool IsWildcard { get; set; }
    public string? RegexPattern { get; set; }
    public ResponseSelectionMode ResponseMode { get; set; }
    public int? DelayMin { get; set; }
    public int? DelayMax { get; set; }
    public string? CallbackUrl { get; set; }
    public bool EnableCallback { get; set; }
}


