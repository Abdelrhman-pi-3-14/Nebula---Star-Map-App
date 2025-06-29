using Newtonsoft.Json;
using UnityEngine;

public class ConstellationData
{
    [JsonProperty("constellation_area_in_degrees_etendue_de_la_constellation_en_degres_2")]
    public double AreaInDegrees { get; set; }

    [JsonProperty("constellation_area_in_of_the_celestial_sphere_etendue_de_la_constellation_en_de_la_sphere_celeste")]
    public string CelestialSphereArea { get; set; }

    [JsonProperty("constellation_zone_celestial_equator_zone_de_la_constellation_equateur_celeste")]
    public string CelestialEquatorZone { get; set; }

    [JsonProperty("constellation_zone_ecliptic_zone_de_la_constellation_ecliptique")]
    public string EclipticZone { get; set; }

    [JsonProperty("constellation_zone_milky_way_zone_de_la_constellation_voie_lactee")]
    public string MilkyWayZone { get; set; }

    [JsonProperty("dec_declinaison")]
    public string Declination { get; set; }

    [JsonProperty("english_name_nom_en_anglais")]
    public string EnglishName { get; set; }

    [JsonProperty("french_name_nom_francais")]
    public string FrenchName { get; set; }

    [JsonProperty("iau_code")]
    public string IAUCode { get; set; }

    [JsonProperty("image")]
    public ConstellationImage Image { get; set; }

    [JsonProperty("latin_name_nom_latin")]
    public string LatinName { get; set; }

    [JsonProperty("name_origin_origine_de_l_apellation")]
    public string NameOrigin { get; set; }

    [JsonProperty("principal_star_etoile_principale")]
    public string PrincipalStar { get; set; }

    [JsonProperty("quad_repere_de_l_hemisphere_et_du_quadrant")]
    public string Quadrant { get; set; }

    [JsonProperty("season_saison")]
    public string Season { get; set; }

    [JsonProperty("test")]
    public string Test { get; set; }
}
