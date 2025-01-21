using System;
using System.Collections.Generic;

namespace DemoWebApplication.Models;

public partial class evaluations
{
    public int evaluation_id { get; set; }

    public int? user_id { get; set; }

    public int total_score { get; set; }

    public string? recommendation { get; set; }

    public DateTime? evaluated_at { get; set; }

    public virtual users? user { get; set; }
}
