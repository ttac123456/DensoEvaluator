using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace DensoEvaluator
{
    /// <summary>
    /// プリセット位置移動読み込みクラス
    /// </summary>
    class PersetPositionReader
    {
        // メンバ変数
        private Dictionary<string, List<double>> dictPresetPosition = new Dictionary<string, List<double>>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PersetPositionReader()
        {
            dictPresetPosition.Clear();
        }

        /// <summary>
        /// CSVファイル読み込み
        /// </summary>
        /// <param name="filePath">CSVファイルパス</param>
        /// <returns>読み込み成否</returns>
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

        /// <summary>
        /// 指定位置の座標情報を取得する
        /// </summary>
        /// <param name="indexText">指定位置</param>
        /// <returns>指定位置の座標情報(X,Y,Z位置)</returns>
        public List<double> GetPosition(string indexText)
        {
            if (dictPresetPosition.Count > 0)
                return dictPresetPosition[indexText];
            else
                return null;
        }

        /// <summary>
        /// ポジションデータアップロード用のテキストを取得する
        /// </summary>
        /// <returns>ポジションデータアップロード用テキスト</returns>
        public string GetPositionDataText()
        {
            string positionDataText = "";

            for (int i = 0; i < dictPresetPosition.Count; i++)
            {
                string indexText = i.ToString();
                List<double> presetPosition = dictPresetPosition[i.ToString("00")];
                positionDataText += indexText + ","
                    + presetPosition[0].ToString() + ","
                    + presetPosition[1].ToString() + ","
                    + presetPosition[2].ToString() + ",";
            }
            positionDataText = positionDataText.TrimEnd(',');

            return positionDataText;
        }
    }
}
