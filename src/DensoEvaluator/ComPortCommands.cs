using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DensoEvaluator
{
    /// <summary>
    /// ComPort通信コマンドクラス
    /// </summary>
    public class ComPortCommands
    {
        // 型定義
        /// 移動方向     
        public enum MoveDirection
        {
            X = 0,
            Y,
            Z,
        }

        /// 絶対位置
        public class AbsolutePosition
        {
            public MoveDirection direction;
            public Int32 value;
        }

        // 定数
        private readonly byte[] SOH = new byte[1] { 0x01 };                         ///< 送信開始: SOH     
        private readonly byte[] CMD = new byte[4] { 0x43, 0x4D, 0x44, 0x20 };       ///< 識別: "CMD "
        private readonly byte[] ID_POSITION_DATA_UPLOAD = new byte[1] { 0x48 };     ///< 電文ID: "H" (ポジションデータアップロード)
        private readonly byte[] ID_MOVE_TO_POSITION = new byte[1] { 0x47 };         ///< 電文ID: "G" (ポジション移動)
        private readonly byte[] ID_MOVE_RELATIVE = new byte[1] { 0x4C };            ///< 電文ID: "L" (軸移動(相対アドレス))
        private readonly byte[] ID_REFER_POSITION = new byte[1] { 0x4E };           ///< 電文ID: "N" (軸のポジション(絶対アドレス取得))
        private readonly byte[] TYPE_REQUEST = new byte[1] { 0x30 };                ///< 種別: "0" (ターゲットへの要求)
        private readonly byte[] TYPE_RESPONSE = new byte[1] { 0x31 };               ///< 種別: "1" (ターゲットからの結果応答)
        private readonly byte[] TYPE_INDICATE = new byte[1] { 0x32 };               ///< 種別: "2" (ターゲットからの通知) :(未使用)
        private readonly byte[] TYPE_ALIVE = new byte[1] { 0x33 };                  ///< 種別: "3" (ヘルスチェック)       :(未使用)
        private readonly byte[] RESULT_NOT_RESPONSE = new byte[1] { 0x30 };         ///< 処理結果: "0" (応答電文以外)         :(未使用)
        private readonly byte[] RESULT_SUCCESS = new byte[1] { 0x31 };              ///< 処理結果: "1" (正常(終了))           :(未使用)
        private readonly byte[] RESULT_BUSY = new byte[1] { 0x32 };                 ///< 処理結果: "2" (要求処理中)           :(未使用)
        private readonly byte[] RESULT_INVALID = new byte[1] { 0x33 };              ///< 処理結果: "3" (電文不正)             :(未使用)
        private readonly byte[] RESULT_NOT_ACCEPT = new byte[1] { 0x34 };           ///< 処理結果: "4" (受付不可)             :(未使用)
        private readonly byte[] RESULT_OPERATION_ERROR = new byte[1] { 0x35 };      ///< 処理結果: "5" (運用異常発生)         :(未使用)
        private readonly byte[] STX = new byte[1] { 0x02 };                         ///< テキスト開始: STX     
        private readonly byte[] ETX = new byte[1] { 0x03 };                         ///< テキスト終了: ETX     
        public readonly byte[] EOT = new byte[1] { 0x04 };                         ///< 送信終了: EOT     

        private const int POSITION_ID = 5;          ///< 電文IDの格納位置
        private const int POSITION_TYPE = 6;        ///< 種別の格納位置
        private const int POSITION_RESULT = 7;      ///< 処理結果の格納位置 
        private const int POSITION_DATASIZE = 8;    ///< データサイズの格納位置
        private const int POSITION_DATATEXT = 13;   ///< テキストの格納位置 (STXの次の位置)


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComPortCommands()
        {
            var yyy = makeCommandPositionDataUpload("abc");
            yyy = makeMoveToPosition(7);
            yyy = makeMoveRelative(MoveDirection.X, -123);
            yyy = makeReferPosition(MoveDirection.Z);
        }

        /// <summary>
        /// ポジションデータアップロードコマンド作成
        /// </summary>
        /// <param name="positionDataText"></param>
        /// <returns></returns>
        public byte[] makeCommandPositionDataUpload(string positionDataText)
        {
            // コマンドデータサイズを確定して、byte配列を確保する
            UInt32 commandDataSize = getCommandDataSize(ID_POSITION_DATA_UPLOAD, positionDataText);
            byte[] commandData = new byte[commandDataSize];

            // コマンド作成
            int wrPos = 0;
            wrPos += appendSOH(commandData, wrPos);
            wrPos += appendCMD(commandData, wrPos);
            wrPos += appendID(commandData, wrPos, ID_POSITION_DATA_UPLOAD);
            wrPos += appendType(commandData, wrPos, TYPE_REQUEST);
            wrPos += appendResult(commandData, wrPos, RESULT_NOT_RESPONSE);
            wrPos += appendDataSize(commandData, wrPos, positionDataText);
            wrPos += appendSTX(commandData, wrPos);
            wrPos += appendDataText(commandData, wrPos, positionDataText);
            wrPos += appendETX(commandData, wrPos);
            wrPos += appendCheckSum(commandData, wrPos);
            wrPos += appendEOT(commandData, wrPos);

            return commandData;
        }

        /// <summary>
        /// ポジション移動コマンド作成
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public byte[] makeMoveToPosition(int position)
        {
            // ポジション数値を2バイト配列経由でstringに変換する
            string positionText = position.ToString();

            // コマンドデータサイズを確定して、byte配列を確保する
            UInt32 commandDataSize = getCommandDataSize(ID_MOVE_TO_POSITION, positionText);
            byte[] commandData = new byte[commandDataSize];

            // コマンド作成
            int wrPos = 0;
            wrPos += appendSOH(commandData, wrPos);
            wrPos += appendCMD(commandData, wrPos);
            wrPos += appendID(commandData, wrPos, ID_MOVE_TO_POSITION);
            wrPos += appendType(commandData, wrPos, TYPE_REQUEST);
            wrPos += appendResult(commandData, wrPos, RESULT_NOT_RESPONSE);
            wrPos += appendDataSize(commandData, wrPos, positionText);
            wrPos += appendSTX(commandData, wrPos);
            wrPos += appendDataText(commandData, wrPos, positionText);
            wrPos += appendETX(commandData, wrPos);
            wrPos += appendCheckSum(commandData, wrPos);
            wrPos += appendEOT(commandData, wrPos);

            return commandData;
        }

        /// <summary>
        /// 軸移動(相対アドレス)コマンド作成
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] makeMoveRelative(MoveDirection direction, int value)
        {
            // 方向と移動量をstringに変換する
            string directionText = direction == MoveDirection.X ? "X" : direction == MoveDirection.Y ? "Y" : "Z";
            string valueText = value.ToString();
            string moveParamText = directionText + "," + valueText;

            // コマンドデータサイズを確定して、byte配列を確保する
            UInt32 commandDataSize = getCommandDataSize(ID_MOVE_RELATIVE, moveParamText);
            byte[] commandData = new byte[commandDataSize];

            // コマンド作成
            int wrPos = 0;
            wrPos += appendSOH(commandData, wrPos);
            wrPos += appendCMD(commandData, wrPos);
            wrPos += appendID(commandData, wrPos, ID_MOVE_RELATIVE);
            wrPos += appendType(commandData, wrPos, TYPE_REQUEST);
            wrPos += appendResult(commandData, wrPos, RESULT_NOT_RESPONSE);
            wrPos += appendDataSize(commandData, wrPos, moveParamText);
            wrPos += appendSTX(commandData, wrPos);
            wrPos += appendDataText(commandData, wrPos, moveParamText);
            wrPos += appendETX(commandData, wrPos);
            wrPos += appendCheckSum(commandData, wrPos);
            wrPos += appendEOT(commandData, wrPos);

            return commandData;
        }

        /// <summary>
        /// 軸のポジション(絶対アドレス取得)コマンド作成
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] makeReferPosition(MoveDirection direction)
        {
            // 方向をstringに変換する
            string directionText = direction == MoveDirection.X ? "X" : direction == MoveDirection.Y ? "Y" : "Z";

            // コマンドデータサイズを確定して、byte配列を確保する
            UInt32 commandDataSize = getCommandDataSize(ID_REFER_POSITION, directionText);
            byte[] commandData = new byte[commandDataSize];

            // コマンド作成
            int wrPos = 0;
            wrPos += appendSOH(commandData, wrPos);
            wrPos += appendCMD(commandData, wrPos);
            wrPos += appendID(commandData, wrPos, ID_REFER_POSITION);
            wrPos += appendType(commandData, wrPos, TYPE_REQUEST);
            wrPos += appendResult(commandData, wrPos, RESULT_NOT_RESPONSE);
            wrPos += appendDataSize(commandData, wrPos, directionText);
            wrPos += appendSTX(commandData, wrPos);
            wrPos += appendDataText(commandData, wrPos, directionText);
            wrPos += appendETX(commandData, wrPos);
            wrPos += appendCheckSum(commandData, wrPos);
            wrPos += appendEOT(commandData, wrPos);

            return commandData;
        }

        private UInt32 getCommandDataSize(byte[] id, string dataText = "")
        {
            UInt32 commandDataSize = 0;
            commandDataSize += (UInt32)SOH.Length;
            commandDataSize += (UInt32)CMD.Length;
            commandDataSize += (UInt32)id.Length;       // 電文IDサイズ
            commandDataSize += (UInt32)TYPE_REQUEST.Length;
            commandDataSize += (UInt32)RESULT_NOT_RESPONSE.Length;
            commandDataSize += (UInt32)sizeof(UInt32);  // データサイズ
            commandDataSize += (UInt32)STX.Length;
            commandDataSize += (UInt32)dataText.Length; // テキストサイズ
            commandDataSize += (UInt32)ETX.Length;
            commandDataSize += (UInt32)sizeof(UInt16);  // チェックサム
            commandDataSize += (UInt32)EOT.Length;
            return commandDataSize;
        }

        private int appendSOH(byte[] commandData, int wrPos)
        {
            Array.Copy(SOH, 0, commandData, wrPos, SOH.Length);
            return SOH.Length;
        }

        private int appendCMD(byte[] commandData, int wrPos)
        {
            Array.Copy(CMD, 0, commandData, wrPos, CMD.Length);
            return CMD.Length;
        }

        private int appendID(byte[] commandData, int wrPos, byte[] id)
        {
            Array.Copy(id, 0, commandData, wrPos, id.Length);
            return id.Length;
        }

        private int appendType(byte[] commandData, int wrPos, byte[] type)
        {
            Array.Copy(type, 0, commandData, wrPos, type.Length);
            return type.Length;
        }

        private int appendResult(byte[] commandData, int wrPos, byte[] result)
        {
            Array.Copy(result, 0, commandData, wrPos, result.Length);
            return result.Length;
        }

        private int appendDataSize(byte[] commandData, int wrPos, string dataText)
        {
            int dataSizeValue = STX.Length + dataText.Length + SOH.Length;
            byte[] dataSizeArray = new Byte[4];
            dataSizeArray[3] = (byte)(0x30 + (dataSizeValue % 10));
            dataSizeArray[2] = (byte)(0x30 + (dataSizeValue / 10 % 10));
            dataSizeArray[1] = (byte)(0x30 + (dataSizeValue / 10 / 10 % 10));
            dataSizeArray[0] = (byte)(0x30 + (dataSizeValue / 10 / 10 / 10 % 10));
            Array.Copy(dataSizeArray, 0, commandData, wrPos, sizeof(UInt32));
            return sizeof(UInt32);          // データサイズ格納サイズ
        }

        private int appendSTX(byte[] commandData, int wrPos)
        {
            Array.Copy(STX, 0, commandData, wrPos, STX.Length);
            return STX.Length;
        }

        private int appendDataText(byte[] commandData, int wrPos, string dataText)
        {
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(dataText), 0, commandData, wrPos, dataText.Length);
            return dataText.Length;
        }


        private int appendETX(byte[] commandData, int wrPos)
        {
            Array.Copy(ETX, 0, commandData, wrPos, ETX.Length);
            return ETX.Length;
        }

        private int appendCheckSum(byte[] commandData, int wrPos)
        {
            int sumPos, checkSum = 0;
            for (sumPos = 0; sumPos < wrPos; sumPos++)
            {
                checkSum ^= commandData[sumPos];
            }
            commandData[wrPos++] = (byte)(checkSum >> 4);
            commandData[wrPos++] = (byte)(checkSum & 0x0f);
            return sizeof(UInt16);          // チェックサム格納サイズ
        }

        private int appendEOT(byte[] commandData, int wrPos)
        {
            Array.Copy(EOT, 0, commandData, wrPos, EOT.Length);
            return EOT.Length;
        }

        /// <summary>
        /// 軸のポジション(絶対アドレス取得)取得応答をパースする
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public AbsolutePosition parseReferPosition(byte[] command)
        {
            AbsolutePosition parsedPos = null;

            // コマンドデータサイズを確定して、byte配列を確保する
            if ((command[POSITION_ID] == ID_REFER_POSITION[0]) &&       // 電文IDの格納位置
                (command[POSITION_TYPE] == TYPE_RESPONSE[0]) &&         // 種別の格納位置
                (command[POSITION_RESULT] == RESULT_SUCCESS[0]))        // 処理結果の格納位置
            {
                // 絶対位置格納エリアを確保
                parsedPos = new AbsolutePosition();

                // テキストサイズを算出
                int textSize = 0;
                textSize += (command[POSITION_DATASIZE + 0] - 0x30) * 1000;
                textSize += (command[POSITION_DATASIZE + 1] - 0x30) * 100;
                textSize += (command[POSITION_DATASIZE + 2] - 0x30) * 10;
                textSize += (command[POSITION_DATASIZE + 3] - 0x30);
                textSize -= 2;      // STXとETXを除いて、テキストのみのサイズに調整

                // テキストを切り出す
                byte[] textArray = new byte[textSize];
                Array.Copy(command, POSITION_DATATEXT, textArray, 0, textSize);

                // テキストから、移動方向と移動量を抜き出す
                string AbsPosParamText = System.Text.Encoding.ASCII.GetString(textArray);
                string[] splitText = AbsPosParamText.Split(',');
                string directionText = splitText[0];
                string valueText = splitText[1];

                // 絶対位置格納エリアに移動方向と移動量を格納
                parsedPos.direction = directionText == "X" ? MoveDirection.X : directionText == "Y" ? MoveDirection.Y : MoveDirection.Z;
                parsedPos.value = Int32.Parse(valueText);
            }

            return parsedPos;
        }

    }
}
