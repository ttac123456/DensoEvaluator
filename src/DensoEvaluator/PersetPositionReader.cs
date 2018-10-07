using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace DensoEvaluator
{
    class PersetPositionReader
    {
        private Dictionary<string, List<double>> dictPresetPosition = new Dictionary<string, List<double>>();

        public PersetPositionReader()
        {
            dictPresetPosition.Clear();
        }

        public bool Load(string filePath)
        {
            bool loadResult = false;
            StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("Shift_JIS"));
            try
            {
                Dictionary<string, List<double>> tempDictPresetPosition = new Dictionary<string, List<double>>();

                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    string[] fields = line.Split(',');
                    //string[] fields = line.Split('\t'); //TSVファイルの場合

                    string indexText = fields[0];
                    int index;
                    bool isNumber = int.TryParse(indexText, out index);
                    if (isNumber)
                    {
                        List<double> listPosition = new List<double>();

                        for (int i = 1; i < fields.Length; i++)
                        {
                            Console.Write(fields[i].ToString() + ",");
                            double position;
                            double.TryParse(fields[i], out position);
                            listPosition.Add(position);
                        }
                        Console.WriteLine();
                        if (listPosition.Count > 0)
                        {
                            tempDictPresetPosition.Add(index.ToString("00"), listPosition);
                        }
                    }
                }
                if (tempDictPresetPosition.Count > 0)
                {
                    dictPresetPosition = tempDictPresetPosition;
                    loadResult = true;
                }
            }
            finally
            {
                sr.Close();
            }
            return loadResult;
        }

        public List<double> GetPosition(string indexText)
        {
            if (dictPresetPosition.Count > 0)
                return dictPresetPosition[indexText];
            else
                return null;
        }
    }
}
