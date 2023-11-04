﻿namespace midis.muchik.market.application.dto
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string RoleId { get; set; } = null!;
        public RoleDto Role { get; set; } = null!;
    }
}