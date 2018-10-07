﻿using System;
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
        private PersetPositionReader presetPositionReader = new PersetPositionReader();     ///< 移動位置リーダー

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
            // アプリケーション設定インスタンスを取得
            AppSettings appSettings = AppSettings.GetInstance();

            // 速度設定
            UInt32 speed;
            if (UInt32.TryParse(textBox_SettingSpeedLowX.Text,  out speed)) appSettings.SpeedXLow  = speed;
            if (UInt32.TryParse(textBox_SettingSpeedHighX.Text, out speed)) appSettings.SpeedXHigh = speed;
            if (UInt32.TryParse(textBox_SettingSpeedLowY.Text,  out speed)) appSettings.SpeedYLow  = speed;
            if (UInt32.TryParse(textBox_SettingSpeedHighY.Text, out speed)) appSettings.SpeedYHigh = speed;
            if (UInt32.TryParse(textBox_SettingSpeedLowZ.Text,  out speed)) appSettings.SpeedZLow  = speed;
            if (UInt32.TryParse(textBox_SettingSpeedHighZ.Text, out speed)) appSettings.SpeedZHigh = speed;

            // 移動位置設定
            appSettings.PositionSettingCsvPath = textBox_SettingCsvPath.Text;

            // 通信設定
            appSettings.ComPort = (ComPortEnum)comboBox_ComPortSetting.SelectedValue;
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

            // forDebug:デバッグ出力
            if ((position != null) && (position.Count > 0))
                Console.WriteLine(buttonContent + ":" +
                    position[0].ToString() + "," +
                    position[1].ToString() + "," +
                    position[2].ToString());
            else
                Console.WriteLine(buttonContent);
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
                Uri　uriSelectedFile = new Uri(dialog.FileName);
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
            }
            else
            {
                System.Windows.MessageBox.Show("ファイル「" + csvFilePath + "」は存在しません。");
            }
            return loadResult;
        }
    }
}
