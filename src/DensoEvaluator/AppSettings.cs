using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DensoEvaluator
{
    /// <summary>
    /// アプリケーション設定
    /// </summary>
    public sealed class AppSettings
    {
        private static AppSettings _singleInstance = new AppSettings();

        public static AppSettings GetInstance()
        {
            return _singleInstance;
        }

        // 設定ファイル名
        private const string _filename = "AppSettings.xml";

        // 通信設定
        private ComPortEnum _comPort;
        private BaudrateEnum _baudrate;
        private ParityEnum _parity;
        private DataBitEnum _dataBit;
        private StopBitEnum _stopBit;
        private FlowControlEnum _flowControl;

        private string _text;
        private int _number;

        public ComPortEnum ComPort
        {
            get { return _comPort; }
            set { _comPort = value; }
        }

        public BaudrateEnum Baudrate
        {
            get { return _baudrate; }
            set { _baudrate = value; }
        }

        public ParityEnum Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        public DataBitEnum DataBit
        {
            get { return _dataBit; }
            set { _dataBit = value; }
        }

        public StopBitEnum StopBit
        {
            get { return _stopBit; }
            set { _stopBit = value; }
        }

        public FlowControlEnum FlowControl
        {
            get { return _flowControl; }
            set { _flowControl = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        private AppSettings()
        {
            // デフォルト通信設定
            SetDefaultCommnicationSetting();
        }

        /// <summary>
        /// デフォルト通信設定
        /// </summary>
        public void SetDefaultCommnicationSetting()
        {
            _comPort = CommunicationSettingData.DEFAULT_COM_PORT;
            _baudrate = CommunicationSettingData.DEFAULT_BAUDRATE;
            _parity = CommunicationSettingData.DEFAULT_PARITY;
            _dataBit = CommunicationSettingData.DEFAULT_DATA_BIT;
            _stopBit = CommunicationSettingData.DEFAULT_STOP_BIT;
            _flowControl = CommunicationSettingData.DEFAULT_FLOW_CONTROL;
        }

        /// <summary>
        /// XMLファイルから読み込む
        /// </summary>
        public void Load()
        {
            //XmlSerializerオブジェクトの作成
            System.Xml.Serialization.XmlSerializer serializer2 =
                new System.Xml.Serialization.XmlSerializer(typeof(AppSettings));
            try
            {
                //ファイルを開く
                System.IO.StreamReader sr = new System.IO.StreamReader(
                    _filename, new System.Text.UTF8Encoding(false));
                //XMLファイルから読み込み、逆シリアル化する
                _singleInstance.Copy((AppSettings)serializer2.Deserialize(sr));
                //閉じる
                sr.Close();
            }
            catch { }
        }

        /// <summary>
        /// XMLファイルに書き込む
        /// </summary>
        public void Save()
        {
            //XmlSerializerオブジェクトを作成
            //書き込むオブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer serializer1 =
                new System.Xml.Serialization.XmlSerializer(typeof(AppSettings));
            try
            {
                //ファイルを開く（UTF-8 BOM無し）
                System.IO.StreamWriter sw = new System.IO.StreamWriter(
                _filename, false, new System.Text.UTF8Encoding(false));
                //シリアル化し、XMLファイルに保存する
                serializer1.Serialize(sw, _singleInstance);
                //閉じる
                sw.Close();
            }
            catch { }
        }

        /// <summary>
        /// 値をコピー
        /// </summary>
        /// <param name="appSettings">コピー元のインスタンス</param>
        public void Copy(AppSettings appSettings)
        {
            ComPort = appSettings._comPort;
            Baudrate = appSettings._baudrate;
            Parity = appSettings._parity;
            DataBit = appSettings._dataBit;
            StopBit = appSettings._stopBit;
            FlowControl = appSettings._flowControl;
        }
    }
}
