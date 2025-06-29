using System;
using UnityEngine;

public class UserData
{
    public double longitude = 0.000;
    public double latitude = 0.000;
    public DateTime time;

  

    public UserData(double longitude, double latitude, DateTime time)
    {
        this.longitude = longitude;
        this.latitude = latitude;
        this.time = time;
    }

    public double GetLST()
    {
        // Convert current UTC time to Julian Date
        double JD = GetJulianDate(time);
        double T = (JD - 2451545.0) / 36525.0; // Julian centuries since J2000

        // Calculate Greenwich Sidereal Time (GST) in degrees
        double GST = 280.46061837 + 360.98564736629 * (JD - 2451545.0) +
                     0.000387933 * T * T - (T * T * T) / 38710000.0;

        GST = GST % 360.0; // Keep it in the range 0-360
        if (GST < 0) GST += 360.0;

        // Convert GST to Local Sidereal Time (LST)
        double LST = GST + longitude;
        LST = LST % 360.0;
        if (LST < 0) LST += 360.0;

        return LST; // LST in degrees
    }


    private double GetJulianDate(DateTime dateTime)
    {
        int year = dateTime.Year;
        int month = dateTime.Month;
        int day = dateTime.Day;
        double hour = dateTime.Hour + dateTime.Minute / 60.0 + dateTime.Second / 3600.0;

        if (month <= 2)
        {
            year--;
            month += 12;
        }
        int A = year / 100;
        int B = 2 - A + (A / 4);
        double JD = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + hour / 24.0 + B - 1524.5;
        return JD;
    }
}

