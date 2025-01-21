using System;
using System.Collections.Generic;

namespace DemoWebApplication.Models;

public partial class users
{
    public int user_id { get; set; }

    public string username { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<answers> answers { get; set; } = new List<answers>();

    public virtual ICollection<evaluations> evaluations { get; set; } = new List<evaluations>();

    public virtual ICollection<questions> questions { get; set; } = new List<questions>();
}
