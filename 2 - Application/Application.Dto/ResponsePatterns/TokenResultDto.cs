using System.Collections.Generic;
using System;
using API.Application.Dto;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace API.Application.Dto.Response
{
    public class TokenResultDto
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public bool Authenticated { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Expiration { get; set; }
        public List<string> Roles { get; set; }
        public UserDto User { get; set; }

        [JsonIgnore]
        public ClaimsIdentity Identity { get; set; }
    }
}