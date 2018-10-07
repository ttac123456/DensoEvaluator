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
        private const string APP_SETTING_FILENAME = "AppSettings.xml";                  ///< アプリ設定ファイル名
        private const string POSITION_SETTING_FILENAME = "PresetPositionSetting.csv";   ///< 移動位置設定ファイル名
        private const UInt32 SPEED_SETTING_HIGH = 123456789;    ///< 移動速度設定HIGH
        private const UInt32 SPEED_SETTING_LOW = 23456789;      ///< 移動速度設定LOW

        // ウィンドウ位置
        private double _windowPosX;
        private double _windowPosY;

        // 通信設定
        private ComPortEnum _comPort;
        private BaudrateEnum _baudrate;
        private ParityEnum _parity;
        private DataBitEnum _dataBit;
        private StopBitEnum _stopBit;
        private FlowControlEnum _flowControl;

        // 移動位置設定
        private string _positionSettingCsvPath;

        // 速度設定
        private UInt32 _speedXHigh;
        private UInt32 _speedXLow;
        private UInt32 _speedYHigh;
        private UInt32 _speedYLow;
        private UInt32 _speedZHigh;
        private UInt32 _speedZLow;

        public double WindowPosX
        {
            get { return _windowPosX; }
            set { _windowPosX = value; }
        }

        public double WindowPosY
        {
            get { return _windowPosY; }
            set { _windowPosY = value; }
        }

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

        public string PositionSettingCsvPath
        {
            get { return _positionSettingCsvPath; }
            set { _positionSettingCsvPath = value; }
        }

        public UInt32 SpeedXHigh
        {
            get { return _speedXHigh; }
            set { _speedXHigh = value; }
        }

        public UInt32 SpeedXLow
        {
            get { return _speedXLow; }
            set { _speedXLow = value; }
        }

        public UInt32 SpeedYHigh
        {
            get { return _speedYHigh; }
            set { _speedYHigh = value; }
        }
        public UInt32 SpeedYLow
        {
            get { return _speedYLow; }
            set { _speedYLow = value; }
        }

        public UInt32 SpeedZHigh
        {
            get { return _speedZHigh; }
            set { _speedZHigh = value; }
        }
        public UInt32 SpeedZLow
        {
            get { return _speedZLow; }
            set { _speedZLow = value; }
        }

        private AppSettings()
        {
            // デフォルトウィンドウ位置設定
            SetDefaultWindowPosition();
            // デフォルト通信設定
            SetDefaultCommnicationSetting();
            // デフォルト移動位置設定
            SetDefaultPositionSetting();
            // デフォルト速度設定
            SetDefaultSpeedSetting();
        }

        /// <summary>
        /// デフォルトウィンドウ位置設定
        /// </summary>
        public void SetDefaultWindowPosition()
        {
            _windowPosX = 0;
            _windowPosY = 0;
        }

        /// <summary>
        /// デフォルト移動位置設定
        /// </summary>
        public void SetDefaultPositionSetting()
        {
            PositionSettingCsvPath = POSITION_SETTING_FILENAME;
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
        /// デフォルト速度設定
        /// </summary>
        public void SetDefaultSpeedSetting()
        {
            _speedXHigh = SPEED_SETTING_HIGH;
            _speedYHigh = SPEED_SETTING_HIGH;
            _speedZHigh = SPEED_SETTING_HIGH;
            _speedXLow = SPEED_SETTING_LOW;
            _speedYLow = SPEED_SETTING_LOW;
            _speedZLow = SPEED_SETTING_LOW;
        }

        /// <summary>
        /// XMLファイルから読み込む
        /// </summary>
        public void Load()
        {
            //XmlSerializerオブジェクトの作成
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(AppSettings));
            try
            {
                //ファイルを開く
                System.IO.StreamReader sr = new System.IO.StreamReader(
                    APP_SETTING_FILENAME, new System.Text.UTF8Encoding(false));
                //XMLファイルから読み込み、逆シリアル化する
                _singleInstance.Copy((AppSettings)serializer.Deserialize(sr));
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
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(AppSettings));
            try
            {
                //ファイルを開く（UTF-8 BOM無し）
                System.IO.StreamWriter sw = new System.IO.StreamWriter(
                APP_SETTING_FILENAME, false, new System.Text.UTF8Encoding(false));
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(sw, _singleInstance);
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
            // ウィンドウ位置
            _windowPosX = appSettings._windowPosX;
            _windowPosY = appSettings._windowPosY;
            // 通信設定
            ComPort = appSettings._comPort;
            Baudrate = appSettings._baudrate;
            Parity = appSettings._parity;
            DataBit = appSettings._dataBit;
            StopBit = appSettings._stopBit;
            FlowControl = appSettings._flowControl;
            // 移動位置設定
            PositionSettingCsvPath = appSettings._positionSettingCsvPath;
            // デフォルト速度設定
            SpeedXHigh = appSettings._speedXHigh;
            SpeedYHigh = appSettings._speedYHigh;
            SpeedZHigh = appSettings._speedZHigh;
            SpeedXLow = appSettings._speedXLow;
            SpeedYLow = appSettings._speedYLow;
            SpeedZLow = appSettings._speedZLow;
        }
    }
}
