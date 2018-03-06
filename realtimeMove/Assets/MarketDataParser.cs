using System;
using System.Threading;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using MydataType = UnityEngine.Vector4;

public class MarketDataParser {

    private static string logname = "parserlog.txt";
    private static void log(string fun, string content) {
        //System.IO.File.AppendAllText(logname, fun + "," + content + "\r\n");
    }

    private static string line = null;
    private static int currentLineNo = 0;   // each line is counted
    public static int currentSnapShotMax;
    public volatile static int NStocks;   // 
    private static TextReader strReader = null;
    private static DateTime end1;
    private const bool useFile = false;

    public static void Load3ds() {// now needs to load faster, just read first list
        //string fname = @"..\..\..\..\Shanghai matlab data\" + @"data4\short_mid.csv";
        log("Load3ds", "just enter function");
        TextAsset txtData = Resources.Load("mds_3d") as TextAsset;
        string txt = txtData.text;
        strReader = new StringReader(txt);
        NStocks = 501;
        string line1 = strReader.ReadLine();

        var values = line1.Split(',');
        log("3ds", "values count = " + values.Count());
        int colX = -1, colY = -1, colZ = -1, colSize = -1, colR = -1, colG = -1, colB = -1;
        for (int i = 0; i < values.Count(); i++) {
            switch (values[i].Replace(" ", "") ){
                case "X": colX = i; break;
                case "Y": colY = i; break;
                case "Z": colZ = i; break;
                case "Size": colSize = i; break;
                case "R": colR = i; break;
                case "G": colG = i; break;
                case "B": colB = i; break;
                default:
                    break;
            }
        }
        log("load3DS", "X" + colX + "Y" + colY + "Z" + colZ + "S"+colSize+"R" + colR + "G" + colG + "B" + colB);
        while ( ( line = strReader.ReadLine() ) != null )
        {
            values = line.Split(',');
            float scaler = 1.0F;
            if( UnityEngine.XR.XRSettings.loadedDeviceName.Equals("HoloLens") )
                scaler = 0.001F;
            float x = (float)Convert.ToSingle(values[colX]) * scaler;
            float y = (float)Convert.ToSingle(values[colY]) * scaler;
            float z = (float)Convert.ToSingle(values[colZ]) * scaler;
            float w = (float)Convert.ToSingle(values[colSize]) * scaler;
            float r = (float)Convert.ToSingle(values[colR]);
            float g = (float)Convert.ToSingle(values[colG]);
            float b = (float)Convert.ToSingle(values[colB]);
            runningColorList.Add(new Color(r, g, b));
            runningList.Add(new MydataType(x, y, z, w));
        }
    }

    public volatile static List<MydataType> runningList = new List<MydataType>();
    public volatile static List<Color> runningColorList = new List<Color>();

}


namespace utilities
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
