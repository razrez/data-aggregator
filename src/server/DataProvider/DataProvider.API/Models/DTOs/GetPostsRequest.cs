using Microsoft.AspNetCore.Mvc;

namespace DataProvider.API.Models.DTOs;

public record GetPostsRequest(
  [property: FromQuery] int Offset,
  [property: FromQuery] int Limit
);
