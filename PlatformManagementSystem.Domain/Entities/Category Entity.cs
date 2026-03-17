using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlatformManagementSystem.Domain.Common;

namespace PlatformManagementSystem.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}

