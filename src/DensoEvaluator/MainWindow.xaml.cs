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

namespace DensoEvaluator
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //http://oita.oika.me/2014/11/03/wpf-datagrid-binding/
        //https://social.msdn.microsoft.com/Forums/ja-JP/38e6ae57-4a3c-4ddd-8df5-c3926a473e93/datagridesc?forum=wpfja

        private PersetPositionReader presetPositionReader = new PersetPositionReader();     ///< 移動位置リーダー

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            // 画面コンポーネント初期化
            InitializeComponent();

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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // アプリケーション設定インスタンスを取得
            AppSettings appSettings = AppSettings.GetInstance();

            // アプリケーション設定を読み込む
            appSettings.Load();

            // 通信設定
            comboBox_ComPortSetting.SelectedValue = appSettings.ComPort;
            comboBox_BaudrateSetting.SelectedValue = appSettings.Baudrate;
            comboBox_ParitySetting.SelectedValue = appSettings.Parity;
            comboBox_DataBitSetting.SelectedValue = appSettings.DataBit;
            comboBox_StopBitSetting.SelectedValue = appSettings.StopBit;
            comboBox_FlowControlSetting.SelectedValue = appSettings.FlowControl;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // アプリケーション設定インスタンスを取得
            AppSettings appSettings = AppSettings.GetInstance();

            // 通信設定
            appSettings.ComPort = (ComPortEnum)comboBox_ComPortSetting.SelectedValue;
            appSettings.Baudrate = (BaudrateEnum)comboBox_BaudrateSetting.SelectedValue;
            appSettings.Parity = (ParityEnum)comboBox_ParitySetting.SelectedValue;
            appSettings.DataBit = (DataBitEnum)comboBox_DataBitSetting.SelectedValue;
            appSettings.StopBit = (StopBitEnum)comboBox_StopBitSetting.SelectedValue;
            appSettings.FlowControl = (FlowControlEnum)comboBox_FlowControlSetting.SelectedValue;

            // アプリケーション設定を保存する
            appSettings.Save();
        }

        // マウスクリックイベント取得
        private void button_MoveUp_Click(object sender, RoutedEventArgs e)
        {
        }

        private void button_MoveUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void button_MoveRear_Click(object sender, RoutedEventArgs e)
        {
        }

        private void button_PersetPosition_Click(object sender, RoutedEventArgs e)
        {
            // 押されたボタンのindexを取得
            var buttonContent = ((System.Windows.Controls.Button)sender).Content.ToString();
            // indexをキーにプリセット位置を取得する
            var position = presetPositionReader.GetPosition(buttonContent);

            // forDebug:デバッグ出力
            if (position.Count > 0)
                Console.WriteLine(buttonContent + ":" +
                    position[0].ToString() + "," +
                    position[1].ToString() + "," +
                    position[2].ToString());
            else
                Console.WriteLine(buttonContent);
        }
    }
}
