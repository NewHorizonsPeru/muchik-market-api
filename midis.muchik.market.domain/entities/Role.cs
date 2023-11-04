﻿namespace midis.muchik.market.domain.entities
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}