using System;
using System.Collections.Generic;

namespace DemoWebApplication.Models;

public partial class questions
{
    public int question_id { get; set; }

    public string question_text { get; set; } = null!;

    public string category { get; set; } = null!;

    public string? difficulty_level { get; set; }

    public int? created_by { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual ICollection<answers> answers { get; set; } = new List<answers>();

    public virtual users? created_byNavigation { get; set; }
}