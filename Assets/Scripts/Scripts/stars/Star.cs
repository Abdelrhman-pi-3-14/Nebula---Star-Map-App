using UnityEngine;

public class Star
{
    // Three variables used to define the star in the game.
    public float catalog_number;
    public Vector3 position;
    public Color colour;
    public short magnitude;
    public float size;

    // Keep the original points so we can recalculate based on dates.
    private readonly double right_ascension;
    private readonly double declination;
    private readonly float ra_proper_motion;
    private readonly float dec_proper_motion;

    public double ra { get { return right_ascension; } }
    public double dec { get { return declination; } }

    // Constructor (takes user location and time)
    public Star(float catalog_number, double right_ascension, double declination,
                byte spectral_type, byte spectral_index, short magnitude,
                float ra_proper_motion, float dec_proper_motion)
    {
        this.catalog_number = catalog_number;
        // Save the location parameters.
        this.right_ascension = right_ascension;
        this.declination = declination;
        this.ra_proper_motion = ra_proper_motion;
        this.dec_proper_motion = dec_proper_motion;
        this.magnitude = magnitude;
        // Set the position
        position = Vector3.zero;
        // Set the colour
        colour = SetColour(spectral_type, spectral_index);

        // Set the size
        size = GetSize(magnitude);
    }


    private float GetSize(short magnitude)
    {
        // This isn't factually accurate, but the effect is sufficient
        return 1.0f - Mathf.InverseLerp(-146, 796, magnitude);
    }

    private Color SetColour(byte spectral_type, byte spectral_index)
    {
        Color IntColour(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        // Reference new colors from https://arxiv.org/pdf/2101.06254.pdf
        Color[] col = new Color[8];
        col[0] = IntColour(0x6c, 0x7c, 0xff); // O1
        col[1] = IntColour(0x8a, 0x9e, 0xff); // B0.5
        col[2] = IntColour(0x79, 0x96, 0xff); // A0
        col[3] = IntColour(0xb8, 0xc5, 0xff); // F0
        col[4] = IntColour(0xff, 0xe6, 0xd6); // G1
        col[5] = IntColour(0xff, 0xde, 0xc0); // K0
        col[6] = IntColour(0xff, 0xa2, 0x5a); // M0
        col[7] = IntColour(0xff, 0x7d, 0x24); // M9.5

        int col_idx = -1;
        if (spectral_type == 'O') col_idx = 0;
        else if (spectral_type == 'B') col_idx = 1;
        else if (spectral_type == 'A') col_idx = 2;
        else if (spectral_type == 'F') col_idx = 3;
        else if (spectral_type == 'G') col_idx = 4;
        else if (spectral_type == 'K') col_idx = 5;
        else if (spectral_type == 'M') col_idx = 6;

        if (col_idx == -1) return Color.white;

        // Map second part 0->9 to 0->1.0 over 10 degrees
        float percent = (spectral_index - 0x30) / 10.0f;  // Convert '0'-'9' to 0.0-1.0
        return Color.Lerp(col[col_idx], col[col_idx + 1], percent);
    }
}