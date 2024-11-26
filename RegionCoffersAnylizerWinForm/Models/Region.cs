using System;
using System.Collections.Generic;

namespace RegionCoffersAnylizerWinForm.Models;

public partial class Region
{
    public string? Inn { get; set; }

    public string? Okpo { get; set; }

    public string? Naimobj { get; set; }

    public string? Adresf { get; set; }

    public string? Okatof { get; set; }

    public string? Oktmof { get; set; }

    public string? TypeObj { get; set; }

    public string? TypeMsp { get; set; }

    public string? Typeermsp { get; set; }

    public string? OkvedOsn { get; set; }

    public string? OkvedNeosn { get; set; }

    public string? FactOkvedOsn { get; set; }

    public string? FactOkvedNeosn { get; set; }

    public decimal? SchrFns { get; set; }

    public DateTime? SchrActualDate { get; set; }

    public decimal? Schr { get; set; }

    public decimal? Viruchka { get; set; }

    public string? Systemnalog { get; set; }

    public string? License { get; set; }

    public string? kpp { get; set; }

    public string? actualType { get; set; }

    public DateTime? actualDate { get; set; }

    public string? ul_okpo { get; set; }

    public DateTime? lifeEndDate { get; set; }

    public string? regAdress { get; set; }

    public string? factoryType { get; set; }

    public decimal? countTosp { get; set; }

    public string? usingUsn { get; set; }

    public decimal? oborot { get; set; }

    public string? SposobLikvid { get; set; }


    public override bool Equals(object obj)
    {
        if (obj is Models.Region other)
        {
            return this.Inn == other.Inn;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Inn);
    }

}
