using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.Windows.Threading;

namespace DensoEvaluator
{
    /// <summary>
    /// ファイルパス編集用のTextBoxクラス
    /// </summary>
    internal class PathTextBox : System.Windows.Controls.TextBox
    {
        public event EventHandler InputTextChanged;
        public PathTextBox()
        {
            var oldVal = string.Empty;
            this.GotFocus += (sender, e) => { oldVal = this.Text; };
            this.LostFocus += (sender, e) => { if (oldVal != this.Text && InputTextChanged != null) { InputTextChanged(sender, e); } };
        }
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //http://oita.oika.me/2014/11/03/wpf-datagrid-binding/
        //https://social.msdn.microsoft.com/Forums/ja-JP/38e6ae57-4a3c-4ddd-8df5-c3926a473e93/datagridesc?forum=wpfja

        // メンバ変数
        private ComPortCommands comPortCommands = new ComPortCommands();                    ///< ComPort通信コマンド          
        private ComPortController comPortController = new ComPortController();              ///< ComPort通信コントローラ
        private PersetPositionReader presetPositionReader = new PersetPositionReader();     ///< 移動位置リーダー
        private DispatcherTimer referPositionTimer = new DispatcherTimer();                 ///< 絶対アドレス取得要求タイマ
        private DispatcherTimer pressedMoveButtonTimer = new DispatcherTimer();             ///< 移動ボタン長押しタイマ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            // 画面コンポーネント初期化
            InitializeComponent();

            // DataContextに入力データ検証クラスを用いることを追加
            this.DataContext = new InputDataValidater();

            // デフォルト設定
            setDefaultSettings();

            //ComPortController yyy = new ComPortController();
            comPortController.OnReceive += ComPortCommandReceived;
        }

        private void ComPortCommandReceived(byte[] receivedCommand)
        {
            //Console.WriteLine("ComPortCommandReceived: " + receivedCommand.Length.ToString());
            var absPos = comPortCommands.parseReferPosition(receivedCommand);
            if (absPos != null)
            {
                //Console.WriteLine("recv ReferPosition: direction=" + absPos.direction.ToString() + ", value=" + absPos.value.ToString());
                switch (absPos.direction)
                {
                case ComPortCommands.MoveDirection.X:
                    // X方向の現在位置表示を更新
                    textBox_CurrentPosX.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        textBox_CurrentPosX.Text = absPos.value.ToString();
                    }));
                    break;
                case ComPortCommands.MoveDirection.Y:
                    // Y方向の現在位置表示を更新
                    textBox_CurrentPosY.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        textBox_CurrentPosY.Text = absPos.value.ToString();
                    }));
                    break;
                case ComPortCommands.MoveDirection.Z:
                    // Z方向の現在位置表示を更新
                    textBox_CurrentPosZ.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        textBox_CurrentPosZ.Text = absPos.value.ToString();
                    }));
                    break;
                }
            }
        }

        /// <summary>
        /// デフォルト設定
        /// </summary>
        private void setDefaultSettings()
        {
            // アプリケーション設定を取得
            AppSettings appSettings = AppSettings.GetInstance();

            // デフォルト通信設定
            comboBox_ComPortSetting.SelectedValue = appSettings.ComPort;
            comboBox_BaudrateSetting.SelectedValue = appSettings.Baudrate;
            comboBox_ParitySetting.SelectedValue = appSettings.Parity;
            comboBox_DataBitSetting.SelectedValue = appSettings.DataBit;
            comboBox_StopBitSetting.SelectedValue = appSettings.StopBit;
            comboBox_FlowControlSetting.SelectedValue = appSettings.FlowControl;

            // 現在位置確認周期起動設定
            referPositionTimer.Interval = TimeSpan.FromMilliseconds(appSettings.ReferPositionIntgerval);
            this.referPositionTimer.Tick += (sender, e) =>
            {
                Console.WriteLine("referPositionTimer.Tick");
                // 周期起動時の処理
                // X軸の絶対位置取得コマンド送信
                var commandReferPosX = comPortCommands.makeReferPosition(ComPortCommands.MoveDirection.X);
                comPortController.Send(commandReferPosX);
                // Y軸の絶対位置取得コマンド送信
                var commandReferPosY = comPortCommands.makeReferPosition(ComPortCommands.MoveDirection.Y);
                comPortController.Send(commandReferPosY);
                // Z軸の絶対位置取得コマンド送信
                var commandReferPosZ = comPortCommands.makeReferPosition(ComPortCommands.MoveDirection.Z);
                comPortController.Send(commandReferPosZ);
            };

            // 移動ボタン長押し設定
            pressedMoveButtonTimer.Interval = TimeSpan.FromMilliseconds(appSettings.MoveIntervalCyclic);
            this.pressedMoveButtonTimer.Tick += (sender, e) =>
            {
                Console.WriteLine("pressedMoveButtonTimer.Tick");
                // 周期起動時の処理
                // 絶対位置移動コマンド周期送信
                sendMoveRelativeCyclic();
            };
        }

        // 絶対位置移動コマンド周期送信
        private void sendMoveRelativeCyclic()
        {
            byte[] command = null;

            // 絶対位置移動コマンド作成
            if (button_MoveFront.IsPressed)
            {
                // Y(+)方向に移動
                command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.Y, AppSettings.GetInstance().MoveStepCyclic);
            }
            else if (button_MoveRear.IsPressed)
            {
                // Y(-)方向に移動
                command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.Y, (-1) * AppSettings.GetInstance().MoveStepCyclic);
            }
            else if (button_MoveLeft.IsPressed)
            {
                // X(+)方向に移動
                command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.X, AppSettings.GetInstance().MoveStepCyclic);
            }
            else if (button_MoveRight.IsPressed)
            {
                // X(-)方向に移動
                command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.X, (-1) * AppSettings.GetInstance().MoveStepCyclic);
            }
            else if (button_MoveUp.IsPressed)
            {
                // Z(-)方向に移動
                command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.Z, (-1) * AppSettings.GetInstance().MoveStepCyclic);
            }
            else if (button_MoveDown.IsPressed)
            {
                // Z(+)方向に移動
                command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.Z, AppSettings.GetInstance().MoveStepCyclic);
            }

            // 絶対位置移動コマンド送信
            if (command != null)
            {
                comPortController.Send(command);
            }
        }

        /// <summary>
        /// ウィンドウ初期化完了イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            // アプリケーション設定インスタンスを取得
            AppSettings appSettings = AppSettings.GetInstance();

            // アプリケーション設定を読み込む
            appSettings.Load();

            // ウィンドウ位置
            Left = appSettings.WindowPosX;
            Top = appSettings.WindowPosY;
        }

        /// <summary>
        /// ウィンドウロード完了イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // アプリケーション設定インスタンスを取得
            AppSettings appSettings = AppSettings.GetInstance();

            // 通信設定
            comboBox_ComPortSetting.SelectedValue = appSettings.ComPort;
            comboBox_BaudrateSetting.SelectedValue = appSettings.Baudrate;
            comboBox_ParitySetting.SelectedValue = appSettings.Parity;
            comboBox_DataBitSetting.SelectedValue = appSettings.DataBit;
            comboBox_StopBitSetting.SelectedValue = appSettings.StopBit;
            comboBox_FlowControlSetting.SelectedValue = appSettings.FlowControl;

            // 移動位置設定
            textBox_SettingCsvPath.Text = appSettings.PositionSettingCsvPath;

            // 速度設定
            textBox_SettingSpeedLowX.Text = appSettings.SpeedXLow.ToString();
            textBox_SettingSpeedHighX.Text = appSettings.SpeedXHigh.ToString();
            textBox_SettingSpeedLowY.Text = appSettings.SpeedYLow.ToString();
            textBox_SettingSpeedHighY.Text = appSettings.SpeedYHigh.ToString();
            textBox_SettingSpeedLowZ.Text = appSettings.SpeedZLow.ToString();
            textBox_SettingSpeedHighZ.Text = appSettings.SpeedZHigh.ToString();

            // 移動位置設定をロード
            loadPresetPositionSetting(textBox_SettingCsvPath.Text);
        }

        /// <summary>
        /// ウィンドウクローズ処理中イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // ComPort通信終了
            comPortController.Stop();

            // アプリケーション設定インスタンスを取得
            AppSettings appSettings = AppSettings.GetInstance();

            // 速度設定
            UInt32 speed;
            if (UInt32.TryParse(textBox_SettingSpeedLowX.Text, out speed)) appSettings.SpeedXLow = speed;
            if (UInt32.TryParse(textBox_SettingSpeedHighX.Text, out speed)) appSettings.SpeedXHigh = speed;
            if (UInt32.TryParse(textBox_SettingSpeedLowY.Text, out speed)) appSettings.SpeedYLow = speed;
            if (UInt32.TryParse(textBox_SettingSpeedHighY.Text, out speed)) appSettings.SpeedYHigh = speed;
            if (UInt32.TryParse(textBox_SettingSpeedLowZ.Text, out speed)) appSettings.SpeedZLow = speed;
            if (UInt32.TryParse(textBox_SettingSpeedHighZ.Text, out speed)) appSettings.SpeedZHigh = speed;

            // 移動位置設定
            appSettings.PositionSettingCsvPath = textBox_SettingCsvPath.Text;

            // 通信設定
            appSettings.ComPort = (ComPortNumEnum)comboBox_ComPortSetting.SelectedValue;
            appSettings.Baudrate = (BaudrateEnum)comboBox_BaudrateSetting.SelectedValue;
            appSettings.Parity = (ParityEnum)comboBox_ParitySetting.SelectedValue;
            appSettings.DataBit = (DataBitEnum)comboBox_DataBitSetting.SelectedValue;
            appSettings.StopBit = (StopBitEnum)comboBox_StopBitSetting.SelectedValue;
            appSettings.FlowControl = (FlowControlEnum)comboBox_FlowControlSetting.SelectedValue;

            // ウィンドウ位置
            appSettings.WindowPosX = Left;
            appSettings.WindowPosY = Top;

            // アプリケーション設定を保存する
            appSettings.Save();
        }

        /// <summary>
        /// プリセット位置移動ToolTip表示開始イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_PersetPosition_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            // 押されたボタンのindexを取得
            var buttonContent = ((System.Windows.Controls.Button)sender).Content.ToString();
            // indexをキーにプリセット位置を取得する
            var position = presetPositionReader.GetPosition(buttonContent);

            // ToolTip表示テキストを作成する
            string selectedPosition;
            if ((position != null) && (position.Count > 0))
            {
                selectedPosition =
                    "Pos" + buttonContent + "[x,y,z]=[" +
                    position[0].ToString() + "," +
                    position[1].ToString() + "," +
                    position[2].ToString() + "]";
            }
            else
            {
                selectedPosition = buttonContent + "=none";
            }

            // ToolTip表示テキストを変更
            ((System.Windows.Controls.Button)sender).ToolTip = selectedPosition;
        }

        /// <summary>
        /// プリセット位置移動ボタンクリックイベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_PersetPosition_Click(object sender, RoutedEventArgs e)
        {
            // 押されたボタンのindexを取得
            var buttonContent = ((System.Windows.Controls.Button)sender).Content.ToString();
            // indexをキーにプリセット位置を取得する
            var position = presetPositionReader.GetPosition(buttonContent);

            if (position != null)
            {
                var destinationPosition = int.Parse(buttonContent);
                // ポジション移動コマンド送信
                var command = comPortCommands.makeMoveToPosition(destinationPosition);
                comPortController.Send(command);
            }
        }

        /// <summary>
        /// 速度設定テキスト編集イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void textBox_SettingSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            // BindingExpressionを使って強制的にバインディングソースへ反映させる
            var be = (sender as System.Windows.Controls.TextBox).GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
            be.UpdateSource();
        }

        /// <summary>
        /// プリセット位置設定CSVファイル参照ボタンクリックイベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_SettingCsvPath_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new Microsoft.Win32.OpenFileDialog();
            // ファイルの種類を設定
            dialog.Filter = "CSVファイル (*.csv)|*.csv|全てのファイル (*.*)|*.*";
            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイルパスを取得する
                Uri uriSelectedFile = new Uri(dialog.FileName);
                // カレントディレクトリを取得する
                Uri uriCurrentDir = new Uri(Directory.GetCurrentDirectory() + "\\");
                // 選択されたファイルの相対パスを取得する
                string strRelativePath = uriCurrentDir.MakeRelativeUri(uriSelectedFile).ToString();

                // 移動位置設定をロード
                if (loadPresetPositionSetting(strRelativePath))
                {
                    // 選択されたファイルの相対パスをTextBoxに表示
                    textBox_SettingCsvPath.Text = strRelativePath;
                }
            }
        }

        /// <summary>
        /// ファイルパスTextBox編集完了イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void PathTextBox_InputTextChanged(object sender, EventArgs e)
        {
            // 移動位置設定をロード
            loadPresetPositionSetting(textBox_SettingCsvPath.Text);
        }

        /// <summary>
        /// 移動位置設定をロードする
        /// </summary>
        /// <param name="csvFilePath">ロード対象のCSVファイルパス</param>
        private bool loadPresetPositionSetting(string csvFilePath)
        {
            bool loadResult = false;
            if (File.Exists(csvFilePath))
            {
                // 移動位置設定をロード
                loadResult = presetPositionReader.Load(csvFilePath);
                if (loadResult)
                {
                    string positionDataText = presetPositionReader.GetPositionDataText();
                    int len = positionDataText.Length;
                    // ポジションデータアップロードコマンド送信
                    var command = comPortCommands.makeCommandPositionDataUpload(positionDataText);
                    comPortController.Send(command);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("ファイル「" + csvFilePath + "」は存在しません。");
            }
            return loadResult;
        }

        /// <summary>
        /// ComPort通信開始/停止ボタン押下イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_ComPortConnect_Click(object sender, RoutedEventArgs e)
        {
            string buttonContent = button_ComPortConnect.Content.ToString();
            if (buttonContent == "接続")
            {
                string comPort = comboBox_ComPortSetting.Text;
                int baudrate = int.Parse(comboBox_BaudrateSetting.Text);
                Parity parity = Parity.None;
                switch ((ParityEnum)comboBox_ParitySetting.SelectedValue)
                {
                case ParityEnum.ParityEven:
                    parity = Parity.Even;
                    break;
                case ParityEnum.ParityOdd:
                    parity = Parity.Odd;
                    break;
                }
                int dataBits = int.Parse(comboBox_DataBitSetting.Text);
                StopBits stopBits = StopBits.None;
                switch ((StopBitEnum)comboBox_StopBitSetting.SelectedValue)
                {
                case StopBitEnum.StopBit1:
                    stopBits = StopBits.One;
                    break;
                case StopBitEnum.StopBit1_Half:
                    stopBits = StopBits.OnePointFive;
                    break;
                case StopBitEnum.StopBit2:
                    stopBits = StopBits.Two;
                    break;
                }
                Handshake handshake = Handshake.None;
                switch ((FlowControlEnum)comboBox_FlowControlSetting.SelectedValue)
                {
                case FlowControlEnum.FlowRtsCts:
                    handshake = Handshake.RequestToSend;
                    break;
                case FlowControlEnum.FlowDtrDsr:
                    handshake = Handshake.RequestToSendXOnXOff;
                    break;
                case FlowControlEnum.FlowXonXoff:
                    handshake = Handshake.XOnXOff;
                    break;
                }

                // ComPort通信開始
                comPortController.Start(comPort, baudrate, parity, dataBits, stopBits, handshake);

                // 現在位置確認開始
                this.referPositionTimer.Start();

                // 移動位置設定をロード
                loadPresetPositionSetting(textBox_SettingCsvPath.Text);

                // ボタン表示を「切断」に変更
                button_ComPortConnect.Content = "切断";
            }
            else
            {
                // 現在位置確認終了
                this.referPositionTimer.Stop();

                // ComPort通信終了
                comPortController.Stop();

                // ボタン表示を「接続」に変更
                button_ComPortConnect.Content = "接続";
            }
        }

        /// <summary>
        /// Y(+)方向移動ボタン押下イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_MoveFront_Click(object sender, RoutedEventArgs e)
        {
            // Y(+)方向に移動
            var command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.Y, AppSettings.GetInstance().MoveStepOneshot);
            comPortController.Send(command);
        }

        /// <summary>
        /// Y(-)方向移動ボタン押下イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_MoveRear_Click(object sender, RoutedEventArgs e)
        {
            // Y(-)方向に移動
            var command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.Y, (-1) * AppSettings.GetInstance().MoveStepOneshot);
            comPortController.Send(command);
        }

        /// <summary>
        /// X(+)方向移動ボタン押下イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_MoveLeft_Click(object sender, RoutedEventArgs e)
        {
            // X(+)方向に移動
            var command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.X, AppSettings.GetInstance().MoveStepOneshot);
            comPortController.Send(command);
        }

        /// <summary>
        /// X(-)方向移動ボタン押下イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_MoveRight_Click(object sender, RoutedEventArgs e)
        {
            // X(-)方向に移動
            var command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.X, (-1) * AppSettings.GetInstance().MoveStepOneshot);
            comPortController.Send(command);
        }

        /// <summary>
        /// Z(-)方向移動ボタン押下イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            // Z(-)方向に移動
            var command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.Z, (-1) * AppSettings.GetInstance().MoveStepOneshot);
            comPortController.Send(command);
        }

        /// <summary>
        /// Z(+)方向移動ボタン押下イベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            // Z(+)方向に移動
            var command = comPortCommands.makeMoveRelative(ComPortCommands.MoveDirection.Z, AppSettings.GetInstance().MoveStepOneshot);
            comPortController.Send(command);
        }

        /// <summary>
        /// 移動ボタンMouseDownイベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_Move_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 移動ボタン長押しタイマ開始
            this.pressedMoveButtonTimer.Start();
        }

        /// <summary>
        /// 移動ボタンMouseUpイベントハンドラ
        /// </summary>
        /// <param name="sender">呼び出し元</param>
        /// <param name="e">イベントパラメータ</param>
        private void button_Move_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // 移動ボタン長押しタイマ終了
            this.pressedMoveButtonTimer.Stop();
        }
    }
}
