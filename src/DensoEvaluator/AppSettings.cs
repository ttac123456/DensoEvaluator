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
    class AppSettings
    {
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

        public AppSettings()
        {
            // デフォルト通信設定
            _comPort = CommunicationSettingData.DEFAULT_COM_PORT;
            _baudrate = CommunicationSettingData.DEFAULT_BAUDRATE;
            _parity = CommunicationSettingData.DEFAULT_PARITY;
            _dataBit = CommunicationSettingData.DEFAULT_DATA_BIT;
            _stopBit = CommunicationSettingData.DEFAULT_STOP_BIT;
            _flowControl = CommunicationSettingData.DEFAULT_FLOW_CONTROL;
        }
    }
}
