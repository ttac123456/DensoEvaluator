using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 参考：
// ・シンプル通信（Rs-232c）のリファレンス
//      https://aoyama.posttips.net/rs232c/ref.htm
// ・C#でシリアル通信を行う
//      https://kana-soft.com/tech/sample_0007.htm

namespace DensoEvaluator
{
    /// <summary>
    /// 通信ポート番号設定値定義
    /// </summary>
    public enum ComPortNumEnum
    {
        Com1 = 1,
        Com2,
        Com3,
        Com4,
        Com5,
        Com6,
        Com7,
        Com8,
        Com9,
        Com10,
        Com11,
        Com12,
        Com13,
        Com14,
        Com15,
        Com16,
    }

    /// <summary>
    /// 通信ポート番号設定値クラス
    /// </summary>
    public class ComPortNumSettingData : BindableBase
    {
        // 画面とバインドしたい列挙型のプロパティ
        private ComPortNumEnum _enumValueComPortNum;
        public ComPortNumEnum EnumValueComPortNum
        {
            get { return _enumValueComPortNum; }
            set { SetProperty(ref _enumValueComPortNum, value); }
        }                                               ///< 通信ポート番号設定値プロパティ
    }

    /// <summary>
    /// ボーレート設定値定義
    /// </summary>
    public enum BaudrateEnum
    {
        Baud_110 = 110,
        Baud_300 = 300,
        Baud_600 = 600,
        Baud_1200 = 1200,
        Baud_2400 = 2400,
        Baud_4800 = 4800,
        Baud_9600 = 9600,
        Baud_14400 = 14400,
        Baud_19200 = 19200,
        Baud_38400 = 38400,
        Baud_56000 = 56000,
        Baud_57600 = 57600,
        Baud_115200 = 115200,
        Baud_128000 = 128000,   // 128kbps:未使用
        Baud_256000 = 256000,   // 256kbps:未使用
    }

    /// <summary>
    /// ボーレート設定値クラス
    /// </summary>
    public class BaudrateSettingData : BindableBase
    {
        // 画面とバインドしたい列挙型のプロパティ
        private BaudrateEnum _enumValueBaudrate;
        public BaudrateEnum EnumValueBaudrate
        {
            get { return _enumValueBaudrate; }
            set { SetProperty(ref _enumValueBaudrate, value); }
        }                                               ///< ボーレート設定値プロパティ
    }

    /// <summary>
    /// パリティ設定値定義
    /// </summary>
    public enum ParityEnum
    {
        ParityNone = 0,
        ParityOdd = 1,
        ParityEven = 2,
        ParityMark = 3,         // マーク:未使用
        ParitySpace = 4,        // スペース:未使用
    }

    /// <summary>
    /// パリティ設定値クラス
    /// </summary>
    public class ParitySettingData : BindableBase
    {
        // 画面とバインドしたい列挙型のプロパティ
        private ParityEnum _enumValueBParity;
        public ParityEnum EnumValueParity
        {
            get { return _enumValueBParity; }
            set { SetProperty(ref _enumValueBParity, value); }
        }                                               ///< パリティ設定値プロパティ
    }

    /// <summary>
    /// データビット長設定値定義
    /// </summary>
    public enum DataBitEnum
    {
        DataBit5 = 5,           // 5ビット:未使用
        DataBit6 = 6,           // 6ビット:未使用
        DataBit7 = 7,
        DataBit8 = 8,
    }

    /// <summary>
    /// データビット長設定値クラス
    /// </summary>
    public class DataBitSettingData : BindableBase
    {
        // 画面とバインドしたい列挙型のプロパティ
        private DataBitEnum _enumValueDataBit;
        public DataBitEnum EnumValueDataBit
        {
            get { return _enumValueDataBit; }
            set { SetProperty(ref _enumValueDataBit, value); }
        }                                               ///< データビット長設定値プロパティ
    }

    /// <summary>
    /// ストップビット設定値定義
    /// </summary>
    public enum StopBitEnum
    {
        StopBit1 = 0,
        StopBit1_Half = 1,       // 1.5ビット:未使用
        StopBit2 = 2,
    }

    /// <summary>
    /// ストップビット設定値クラス
    /// </summary>
    public class StopBitSettingData : BindableBase
    {
        // 画面とバインドしたい列挙型のプロパティ
        private StopBitEnum _enumValueStopBit;
        public StopBitEnum EnumValueStopBit
        {
            get { return _enumValueStopBit; }
            set { SetProperty(ref _enumValueStopBit, value); }
        }                                               ///< ストップビット設定値プロパティ
    }

    /// <summary>
    /// フロー制御設定値定義
    /// </summary>
    public enum FlowControlEnum
    {
        FlowNone = 0,
        FlowDtrDsr = 1,
        FlowRtsCts = 2,
        FlowXonXoff = 4,
    }

    /// <summary>
    /// フロー制御設定値クラス
    /// </summary>
    public class FlowControlSettingData : BindableBase
    {
        // 画面とバインドしたい列挙型のプロパティ
        private FlowControlEnum _enumValueFlowControl;
        public FlowControlEnum EnumValueFlowControl
        {
            get { return _enumValueFlowControl; }
            set { SetProperty(ref _enumValueFlowControl, value); }
        }                                               ///< フロー制御設定値プロパティ
    }

    /// <summary>
    /// ComPort通信設定データクラス
    /// </summary>
    public class ComPortSettingDatas
    {
        // デフォルト設定
        public const ComPortNumEnum DEFAULT_COM_PORT_NUM = ComPortNumEnum.Com1;
        public const BaudrateEnum DEFAULT_BAUDRATE = BaudrateEnum.Baud_19200;
        public const ParityEnum DEFAULT_PARITY = ParityEnum.ParityNone;
        public const DataBitEnum DEFAULT_DATA_BIT = DataBitEnum.DataBit8;
        public const StopBitEnum DEFAULT_STOP_BIT = StopBitEnum.StopBit1;
        public const FlowControlEnum DEFAULT_FLOW_CONTROL = FlowControlEnum.FlowNone;

        // ComboBoxの一覧に表示するデータ
        public Dictionary<ComPortNumEnum, string> ComPortNumEnumNameDictionary { get; }
          = new Dictionary<ComPortNumEnum, string>();       ///< 通信ポート設定値一覧データ
        public Dictionary<BaudrateEnum, string> BaudrateEnumNameDictionary { get; }
          = new Dictionary<BaudrateEnum, string>();         ///< ボーレート設定値一覧データ
        public Dictionary<ParityEnum, string> ParityEnumNameDictionary { get; }
          = new Dictionary<ParityEnum, string>();           ///< パリティ設定値一覧データ
        public Dictionary<DataBitEnum, string> DataBitEnumNameDictionary { get; }
          = new Dictionary<DataBitEnum, string>();          ///< データビット長設定値一覧データ
        public Dictionary<StopBitEnum, string> StopBitEnumNameDictionary { get; }
          = new Dictionary<StopBitEnum, string>();          ///< ストップビット設定値一覧データ
        public Dictionary<FlowControlEnum, string> FlowControlEnumNameDictionary { get; }
          = new Dictionary<FlowControlEnum, string>();      ///< フロー制御設定値一覧データ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComPortSettingDatas()
        {
            // 通信ポート設定値とその表示文字列のDictionaryを作る
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com1, "COM1");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com2, "COM2");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com3, "COM3");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com4, "COM4");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com5, "COM5");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com6, "COM6");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com7, "COM7");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com8, "COM8");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com9, "COM9");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com10, "COM10");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com11, "COM11");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com12, "COM12");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com13, "COM13");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com14, "COM14");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com15, "COM15");
            ComPortNumEnumNameDictionary.Add(ComPortNumEnum.Com16, "COM16");

            // ボーレート設定値とその表示文字列のDictionaryを作る
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_110, "110");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_300, "300");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_600, "600");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_1200, "1200");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_2400, "2400");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_4800, "4800");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_9600, "9600");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_14400, "14400");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_19200, "19200");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_38400, "38400");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_56000, "56000");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_57600, "57600");
            BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_115200, "115200");
            //BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_128000, "128000");
            //BaudrateEnumNameDictionary.Add(BaudrateEnum.Baud_256000, "256000");

            // パリティ設定値とその表示文字列のDictionaryを作る
            ParityEnumNameDictionary.Add(ParityEnum.ParityNone, "None");
            ParityEnumNameDictionary.Add(ParityEnum.ParityOdd, "Odd");
            ParityEnumNameDictionary.Add(ParityEnum.ParityEven, "Even");
            //ParityEnumNameDictionary.Add(ParityEnum.ParityMark, "Mark");
            //ParityEnumNameDictionary.Add(ParityEnum.ParitySpace, "Space");

            // データビット長設定値とその表示文字列のDictionaryを作る
            //DataBitEnumNameDictionary.Add(DataBitEnum.DataBit5, "5");
            //DataBitEnumNameDictionary.Add(DataBitEnum.DataBit6, "6");
            DataBitEnumNameDictionary.Add(DataBitEnum.DataBit7, "7");
            DataBitEnumNameDictionary.Add(DataBitEnum.DataBit8, "8");

            // ストップビット設定値とその表示文字列のDictionaryを作る
            StopBitEnumNameDictionary.Add(StopBitEnum.StopBit1, "1");
            //StopBitEnumNameDictionary.Add(StopBitEnum.StopBit1_Half, "1.5");
            StopBitEnumNameDictionary.Add(StopBitEnum.StopBit2, "2");

            // フロー制御設定値とその表示文字列のDictionaryを作る
            FlowControlEnumNameDictionary.Add(FlowControlEnum.FlowNone, "None");
            FlowControlEnumNameDictionary.Add(FlowControlEnum.FlowDtrDsr, "DTR/DSR");
            FlowControlEnumNameDictionary.Add(FlowControlEnum.FlowRtsCts, "RTS/CTS");
            FlowControlEnumNameDictionary.Add(FlowControlEnum.FlowXonXoff, "Xon/Xoff");
        }

    }
}
