using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class RouterDevice
{
    public string Id;
    public string IpAddress;
    public string MacAddress;
    public string PortId;
    public string Status;
    public string Duration;
    public string DeviceType;
    public string Hostname;
    public string OsInfo;
    public string ConnectionType;
    public string LinkState;

    public override string ToString()
    {
        return $"[{Id}] {Hostname} ({DeviceType}) - {IpAddress} - {MacAddress} - Port: {PortId}, Status: {Status}, Connected: {Duration}, OS: {OsInfo}, Band: {ConnectionType}, Link: {LinkState}";
    }
}

public static class RouterParser
{
    private static string DecodeHexEscapes(string input)
    {
        return Regex.Replace(input, @"\\x([0-9A-Fa-f]{2})", m =>
        {
            int value = Convert.ToInt32(m.Groups[1].Value, 16);
            return ((char)value).ToString();
        });
    }

    public static List<RouterDevice> ParseDevices(string rawJs)
    {
        var devices = new List<RouterDevice>();

        // Remove BOM + cleanup
        rawJs = rawJs.Trim('\uFEFF', ' ', '\n', '\r');

        Regex regex = new Regex(@"new USERDevice\((.*?)\)");
        MatchCollection matches = regex.Matches(rawJs);

        foreach (Match match in matches)
        {
            string[] fields = match.Groups[1].Value
                .Split(',')
                .Select(f => DecodeHexEscapes(f.Trim().Trim('"')))
                .ToArray();

            RouterDevice dev = new RouterDevice
            {
                Id = GetField(fields, 0),
                IpAddress = GetField(fields, 1),
                MacAddress = GetField(fields, 2),
                PortId = GetField(fields, 3),
                Status = GetField(fields, 4),
                Duration = GetField(fields, 5),
                DeviceType = GetField(fields, 6),
                Hostname = GetField(fields, 7),
                OsInfo = GetField(fields, 8),
                ConnectionType = GetField(fields, 9),
                LinkState = GetField(fields, 10)
            };

            devices.Add(dev);
        }

        return devices;
    }

    private static string GetField(string[] arr, int index)
    {
        return index < arr.Length ? arr[index] : "";
    }
}
