using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAdmin.Models;

public class MenuItem
{
     public int Id { get; set; }
        public int? ParentId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        public int Order { get; set; }
        public bool IsActive { get; set; }

        public List<MenuItem> Children { get; set; } = new();
}
