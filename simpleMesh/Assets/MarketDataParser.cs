using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public struct StockSample
{
    public string date;
    public string time;
    public float open;
    public float high;
    public float low;
    public float close;
    public int volume;

    public override string ToString()
    {
        return "date=" + date
            + ", time=" + time
            + ", open=" + open
            + ", high=" + high
            + ", low=" + low
            + ", close=" + close
            + ", volume=" + volume; 
    }
}

public class MarketDataParser
{
    private const int maxTick = 4800 / 8;
    public static string GetMarketDataPath()
    {
        return @"..\..\..\..\MarketData\";
    }

    public static List<string> LoadSP500()
    {
        List<string> sp500 = new List<string>();
        string fname = GetMarketDataPath() + @"SP500\SP500_wikipedia.csv";
        var reader = new StreamReader(File.OpenRead(fname));
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line.StartsWith("#") || line.StartsWith("Ticker"))
                continue;
            var items = line.Split(new char[] { ',' });
            string ticker = items[0];
            string sector = items[3];
            if (sector.StartsWith("Information Technology"))
            {
                sp500.Add(ticker);
            }
        }
        return sp500;
    }
    public static List<List<float>> LoadStocksMid() {
        //string fname = @"..\..\..\..\Shanghai matlab data\" + @"data4\short_mid.csv";

		TextAsset txtData= Resources.Load("short_mid") as TextAsset;
        string txt = txtData.text;
		//string[] lines = txtData.text.Split ('\n');
		string[] lines = txt.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        //Console.WriteLine( "First line is " + lines[ 0 ] );

        List<List<float>> result = new List<List<float>>();
        foreach( var line in lines ) {
            List<float> listOneTime = new List<float>();
            var values = line.Split(',');
            int counter = 0;
            foreach (var val in values)
            {
                ++counter;
                if (counter > 10)
                    break;
                if (val.Length == 0)
                    continue;
                listOneTime.Add((float)Convert.ToSingle(val));
            }
            result.Add(listOneTime);
        }
        
        return result;
    }
    public static List<List<StockSample>> LoadStocks()
    {
        var stocks = new List<string>(new string[] {
            GetMarketDataPath() + @"Stooq\data\5 min\us\nasdaq etfs\qqq.us.txt" }
                        );
        //foreach(var s in new string[] {  "AAPL", "AMZN", "FB", "NFLX", "MSFT", "GOOG", "CELG" } )
        foreach(var s in LoadSP500())
        {
            for (int k = 0; k < 3; ++k)
            {
                string fname = GetMarketDataPath() + @"Stooq\data\5 min\us\nasdaq stocks\" + k + @"\" + s + ".us.txt";
                if (File.Exists(fname))
                    stocks.Add(fname);
                bool test_mode = false;
                if (test_mode && stocks.Count > 3)
                    break;
            }
        }
        List<List<StockSample>> listAllStocks = new List<List<StockSample>>();
        foreach (var stock in stocks)
        {
            var reader = new StreamReader(File.OpenRead(stock));
            List<StockSample> listOneStock = new List<StockSample>();
            var header = reader.ReadLine();
            //Console.WriteLine(header);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                StockSample sample = new StockSample();
                sample.date = values[0];
                sample.time = values[1];
                sample.open  = (float)Convert.ToSingle(values[2]);
                sample.high  = (float)Convert.ToSingle(values[3]);
                sample.low   = (float)Convert.ToSingle(values[4]);
                sample.close = (float)Convert.ToSingle(values[5]);
                listOneStock.Add(sample);
            }
            // loaded one stock 
            listAllStocks.Add(listOneStock);
        }

        return listAllStocks;
    }

}


namespace utilities
{
    class Program
    {
        static void Main(string[] args)
        {
            var stockMidList = MarketDataParser.LoadStocksMid();

            var stocks = MarketDataParser.LoadStocks();
            foreach( var stock_samples in stocks )
            {
                foreach( var sample in stock_samples )
                {
                    //Console.WriteLine(sample);
                    break;
                }
            }
        }
    }
}
