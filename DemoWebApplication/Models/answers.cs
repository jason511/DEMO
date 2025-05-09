﻿using System;
using System.Collections.Generic;

namespace DemoWebApplication.Models;

public partial class answers
{
    public int answer_id { get; set; }

    public int? user_id { get; set; }

    public int? question_id { get; set; }

    public string? answer { get; set; }

    public int? score { get; set; }

    public DateTime? answered_at { get; set; }
    
    public Guid survey_id { get; set; }
    
}
