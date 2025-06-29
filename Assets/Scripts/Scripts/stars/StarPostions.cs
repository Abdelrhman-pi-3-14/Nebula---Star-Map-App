using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


    public static class StarPositions
    {
    public static Vector3 CalculatePosition(double rightAscension, double declination, UserData user)
    {
        float skyRadius = 20f;

        // Get Local Sidereal Time (LST) from user data (LST is in degrees)
        double LST_rad = user.GetLST() * Mathf.Deg2Rad;

        // Calculate Hour Angle (HA) in radians and normalize
        double hourAngle = LST_rad - rightAscension;
        hourAngle = (hourAngle + 2 * Math.PI) % (2 * Math.PI);  // Ensure positive value

        // Convert observer latitude to radians
        double latRad = user.latitude * Mathf.Deg2Rad;
        double decRad = declination;

        // Calculate altitude
        double sinAlt = Math.Sin(decRad) * Math.Sin(latRad) +
                       Math.Cos(decRad) * Math.Cos(latRad) * Math.Cos(hourAngle);
        // Clamp to avoid NaN due to floating point errors
        sinAlt = Math.Clamp(sinAlt, -1.0, 1.0);
        double altitude = Math.Asin(sinAlt);

        // CORRECT AZIMUTH CALCULATION
        double cosAz = (Math.Sin(decRad) - Math.Sin(altitude) * Math.Sin(latRad)) /
                       (Math.Cos(altitude) * Math.Cos(latRad));
        // Clamp to avoid domain errors
        cosAz = Math.Clamp(cosAz, -1.0, 1.0);
        double azimuth = Math.Acos(cosAz);

        // Adjust azimuth based on hour angle quadrant
        if (Math.Sin(hourAngle) > 0)
        {
            azimuth = 2 * Math.PI - azimuth;
        }

        // Convert to Cartesian coordinates (Unity space)
        // Astronomical convention: +x = east, +y = up, +z = south
        // Unity convention: +x = east, +y = up, +z = north
        // Solution: Invert z-axis to match Unity
        double x = -Math.Cos(altitude) * Math.Sin(azimuth);    // East
        double y = Math.Sin(altitude);                        // Up
        double z = -Math.Cos(altitude) * Math.Cos(azimuth);   // North (inverted from astro convention)

        return new Vector3((float)x, (float)y, (float)z) * skyRadius;
    }









    public static void UpdateStarPositions(List<Star> stars, UserData user)
        {
            foreach (var star in stars)
            {
                star.position = CalculatePosition(star.ra, star.dec, user);

                if (float.IsNaN(star.position.x) || float.IsNaN(star.position.y) || float.IsNaN(star.position.z))
                {
                    Debug.LogError($"Star {star.catalog_number} has NaN position! RA: {star.ra}, Dec: {star.dec}");
                }
            }
        }
        public static List<Star> FilterByDeclination(List<Star> starList, UserData user)
        {
            List<Star> filteredStars = new List<Star>();

            // Convert user's latitude from degrees to radians
            double userLatRad = user.latitude * Mathf.Deg2Rad;

            foreach (Star star in starList)
            {
                // star.dec is assumed to be already in radians, so no conversion here
                double starDecRad = star.dec;

                // Keep the star if its declination is within ±90° of the user's latitude.
                // That is: [userLatRad - ?/2, userLatRad + ?/2]
                if (starDecRad >= (userLatRad - Mathf.PI / 2) && starDecRad <= (userLatRad + Mathf.PI / 2))
                {
                    filteredStars.Add(star);
                }
            }

            Debug.Log($"? Stars after declination filtering: {filteredStars.Count}");
            if (filteredStars.Count == 0)
            {
                Debug.LogError("?? No stars left after declination filtering! Check declination values.");
            }

            return filteredStars;
        }


        public static List<Star> FilterByRA(List<Star> starList, UserData user)
        {
            List<Star> filteredStars = new List<Star>();

            // Get Local Sidereal Time in degrees and convert to radians
            double lstDeg = user.GetLST();
            double lstRad = lstDeg * Mathf.Deg2Rad;

            foreach (Star star in starList)
            {
                // Use star.ra directly because it is already in radians
                double starRARad = star.ra;

                // Compute Hour Angle (HA)
                double HA = lstRad - starRARad;

                // Normalize HA to [-?, ?]
                HA = Mathf.Repeat((float)(HA + Mathf.PI), 2 * Mathf.PI) - Mathf.PI;



                // Keep the star if its Hour Angle is within ±90° (converted to radians)
                if (Mathf.Abs((float)HA) <= 90 * Mathf.Deg2Rad)
                {
                    filteredStars.Add(star);
                }
            }

            Debug.Log($"? Stars after RA filtering: {filteredStars.Count}");
            if (filteredStars.Count == 0)
            {
                Debug.LogError("?? No stars left after RA filtering! Check LST and RA values.");
            }

            return filteredStars;
        }

        //filtering the stars based on the altitude 
        public static List<Star> AltFiltration(List<Star> stars, UserData user)
        {
            List<Star> filteredStars = new List<Star>();

            foreach (Star s in stars)
            {
                // Recalculate the star's position (this should update using RA, Dec, and user data)
                s.position = CalculatePosition(s.ra, s.dec, user);

                // Assuming the sphere is oriented so that y >= 0 means above the horizon
                if (s.position.y >= 0)
                {
                    filteredStars.Add(s);
                }
            }

            return filteredStars;
        }
        public static List<Star> FilterByMagnitude(List<Star> allStars, float magnitudeThreshold)
        {
            List<Star> filteredStars = new List<Star>();

            foreach (Star star in allStars)
            {
                // Convert stored magnitude to actual V magnitude
                float realMagnitude = star.magnitude / 100.0f;

                // Only keep stars brighter than the threshold
                if (realMagnitude <= magnitudeThreshold)
                {
                    filteredStars.Add(star);
                }
            }

            Debug.Log($"Filtered {filteredStars.Count} stars with magnitude ? {magnitudeThreshold}");
            return filteredStars;
        }


    }




