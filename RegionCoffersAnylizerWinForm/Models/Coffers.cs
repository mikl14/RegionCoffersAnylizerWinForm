using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegionCoffersAnylizerWinForm.Models;

public partial class Coffers
{
    public string? Inn { get; set; }


    public decimal Slice { get; set; }
}
