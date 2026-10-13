using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.Application.Profiles.Dtos
{
    public class ProfileDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
    }
}
