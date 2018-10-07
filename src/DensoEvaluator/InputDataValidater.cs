using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace DensoEvaluator
{
    class InputDataValidater : IDataErrorInfo
    {
        // 定数定義
        private const UInt32 SPEED_VALUE_MIN = 0;           ///< 速度設定最小値
        private const UInt32 SPEED_VALUE_MAX = 999999999;   ///< 速度設定最大値

        // プロパティ定義
        public String SpeedLowX  { get; set; }
        public String SpeedHighX { get; set; }
        public String SpeedLowY  { get; set; }
        public String SpeedHighY { get; set; }
        public String SpeedLowZ  { get; set; }
        public String SpeedHighZ { get; set; }

        // 今回は使わないが、IDataErrorInfo インターフェースでは実装しなければならない
        public string Error { get { return null; } }

        // これも実装必須のプロパティで、各プロパティに対応するエラーメッセージを返す
        public string this[string propertyName]
        {
            get
            {
                string result = null;
                UInt32 speedValue;
                String speedText = "";

                switch (propertyName)
                {
                case "SpeedLowX":
                    if (this.SpeedLowX == null) return null;
                    speedText = this.SpeedLowX;
                    break;
                case "SpeedHighX":
                    if (this.SpeedHighX == null) return null;
                    speedText = this.SpeedHighX;
                    break;
                case "SpeedLowY":
                    if (this.SpeedLowY == null) return null;
                    speedText = this.SpeedLowY;
                    break;
                case "SpeedHighY":
                    if (this.SpeedHighY == null) return null;
                    speedText = this.SpeedHighY;
                    break;
                case "SpeedLowZ":
                    if (this.SpeedLowZ == null) return null;
                    speedText = this.SpeedLowZ;
                    break;
                case "SpeedHighZ":
                    if (this.SpeedHighZ == null) return null;
                    speedText = this.SpeedHighZ;
                    break;
                }

                try
                {
                    speedValue = UInt32.Parse(speedText);
                    if (speedValue < SPEED_VALUE_MIN)
                        result = SPEED_VALUE_MIN.ToString() + "以上の整数値を入力してください。";
                    else if (SPEED_VALUE_MAX < speedValue)
                        result = SPEED_VALUE_MAX.ToString() + "以下の整数値を入力してください。";
                }
                catch (Exception)
                {
                    result = "整数値を入力してください。";
                }

                return result;
            }
        }
    }

}
