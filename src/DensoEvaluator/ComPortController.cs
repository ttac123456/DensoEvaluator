using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace DensoEvaluator
{
    /// <summary>
    /// ComPort通信コントローラクラス
    /// </summary>
    class ComPortController
    {
        // 定数
        private const byte ETX = 0x03;
        private const int REMAINING_SIZE_ETX_TO_EOF = 3;    ///< ETX受信後、EOFが出現するまでの残りバイト数

        // メンバ変数
        private static SerialPort serialPort;   ///< シリアルポートインスタンス
        //private static SerialPort serialPort2;  ///< テスト用シリアルポートインスタンス
        private byte[] rcvCommandBuf = new byte[1024];  ///< コマンドデータ受信バッファ (コマンド組み立て用)
        private int rcvCommandSize = 0;                 ///< コマンドデータ受信サイズ
        private int rcvRemainingSize = int.MaxValue;    ///< ETX受信後、EOFが出現するまでの残りバイト数格納エリア

        // イベントコールバック
        public Action<byte[]> OnReceive;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComPortController()
        {
            serialPort = null;
            //serialPort2 = null;
        }

        /// <summary>
        /// 利用可能シリアルポート番号リストを取得する
        /// </summary>
        /// <returns>シリアルポート番号リスト</returns>
        public string[] GetComPortList()
        {
            // シリアルポート番号の列挙
            string[] comPortList = SerialPort.GetPortNames();

            // シリアルポート番号リストを確認表示
            foreach (string comPort in comPortList)
            {
                Console.WriteLine("detected ComPort: " + comPort);
            }

            // シリアルポート番号リストを返却
            return comPortList;
        }

        /// <summary>
        /// ComPort通信を開始する
        /// </summary>
        /// <param name="comPort">ComPort番号</param>
        /// <param name="baudrate">ボーレート</param>
        /// <param name="parity">パリティ</param>
        /// <param name="dataBits">データビット長</param>
        /// <param name="stopBits">ストップビット</param>
        /// <param name="handshake">フロー制御</param>
        public void Start(
            string comPort,
            int baudrate,
            Parity parity,
            int dataBits,
            StopBits stopBits,
            Handshake handshake
            )
        {
            // まだポートに繋がっていない場合
            if (serialPort == null)
            {
                // serialPortの設定
                serialPort = new SerialPort();
                serialPort.PortName = comPort;
                serialPort.BaudRate = baudrate;
                serialPort.Parity = parity;
                serialPort.DataBits = dataBits;
                serialPort.StopBits = stopBits;
                serialPort.Handshake = handshake;
                serialPort.Encoding = Encoding.UTF8;
                serialPort.WriteTimeout = 100000;

                // テスト用ポートの設定
                //serialPort2 = new SerialPort();
                //serialPort2.PortName = "Com12";
                //serialPort2.BaudRate = baudrate;
                //serialPort2.Parity = parity;
                //serialPort2.DataBits = dataBits;
                //serialPort2.StopBits = stopBits;
                //serialPort2.Handshake = handshake;
                //serialPort2.Encoding = Encoding.UTF8;
                //serialPort2.WriteTimeout = 100000;

                // シリアルポートに接続
                try
                {
                    // ポートオープン
                    serialPort.Open();
                    serialPort.DataReceived += serialPort_DataReceived;

                    // テスト用ポートオープン
                    //serialPort2.Open();
                    //serialPort2.DataReceived += serialPort_DataReceived;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// ComPort通信を終了する
        /// </summary>
        public void Stop()
        {
            if (serialPort != null)
            {
                serialPort.Close();
                serialPort = null;
                //serialPort2.Close();
                //serialPort2 = null;
            }
        }

        /// <summary>
        /// ComPortへデータを送信する
        /// </summary>
        /// <param name="sendCommand">送信データ</param>
        public void Send(byte[] sendCommand)
        {
            // 繋がっていない場合は処理しない
            if (serialPort == null) return;
            if (serialPort.IsOpen == false) return;

            // 送信するテキストを取得
            String data = Encoding.ASCII.GetString(sendCommand);

            try
            {
                // シリアルポートからテキストを送信する.
                serialPort.Write(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while ((serialPort != null) && (serialPort.ReadBufferSize > 0))
            {
                try
                {
                    byte rcvData = (byte)serialPort.ReadByte();
                    rcvCommandBuf[rcvCommandSize++] = rcvData;
                    rcvRemainingSize--;
                    if (rcvData == ETX)
                    {
                        rcvRemainingSize = REMAINING_SIZE_ETX_TO_EOF;
                    }
                    if (rcvRemainingSize == 0)
                    {
                        // 受信コマンドを新規byte配列にコピー
                        byte[] receivedCommand = new byte[rcvCommandSize];
                        Array.Copy(rcvCommandBuf, 0, receivedCommand, 0, rcvCommandSize);
                        Console.WriteLine("serialPort_DataReceived: " + rcvCommandSize.ToString());
                        // 受信コマンドをコールバック
                        OnReceive(receivedCommand);
                        // 受信コマンドサイズと残り受信数をリセット
                        rcvCommandSize = 0;
                        rcvRemainingSize = int.MaxValue;
                    }
                }
                catch { }
            }
        }

    }
}
