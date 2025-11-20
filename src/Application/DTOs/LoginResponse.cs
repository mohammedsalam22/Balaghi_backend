using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
  namespace Application.DTOs
{
  public record LoginResponse(
    string AccessToken,
    List<string> Roles,
     string RefreshToken,
      DateTime AccessTokenExpiresAt = default
);
}