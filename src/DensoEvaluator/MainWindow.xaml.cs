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

namespace NovaEvaluator
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        //http://oita.oika.me/2014/11/03/wpf-datagrid-binding/
        //https://social.msdn.microsoft.com/Forums/ja-JP/38e6ae57-4a3c-4ddd-8df5-c3926a473e93/datagridesc?forum=wpfja

        // 操作方向名称定義
        //string[] src = { "Tokyo", "Osaka", "Nagoya" };

        private string[] DIRECTION_NAME = { "X", "Y", "Z" };

        // クリック回数
        private int count = 0;

        public MainWindow()
        {
            InitializeComponent();

            this.dataGrid_PositionStatus.LoadingRow +=
                 (sender, e) => {
                     e.Row.Header = DIRECTION_NAME[e.Row.GetIndex()];
                 };
        }

        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<Customer> customers = new ObservableCollection<Customer>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var dt = new DataTable();
            dt.Columns.Add("Direction");
            int itemcount = DIRECTION_NAME.Length;
            for (int j = 0; j < itemcount; j++)
            {
                customers.Add(new Customer()
                {
                    Status1 = "stat" + j.ToString(),
                    Status2 = "stat" + j.ToString(),
                    Status3 = "stat" + j.ToString(),
                    Status4 = "stat" + j.ToString(),
                    Status5 = "stat" + j.ToString(),
                    Status6 = "stat" + j.ToString(),
                });
            }

            view.Source = customers;
            this.dataGrid_PositionStatus.DataContext = view;
        }

        class Customer
        {
            public string Status1 { get; set; }
            public string Status2 { get; set; }
            public string Status3 { get; set; }
            public string Status4 { get; set; }
            public string Status5 { get; set; }
            public string Status6 { get; set; }
        }

        // マウスクリックイベント取得
        private void button_MoveUp_Click(object sender, RoutedEventArgs e)
            {
            // sender経由でクリックイベントを発生させたボタンを取得
            var button = (System.Windows.Controls.Button)sender;
            // ボタンの表示を更新
            button.Content = string.Format("{0}回", ++count);

        }

        private void button_MoveUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("MouseDown");
        }

        static int i = 10;
        private void button_MoveRear_Click(object sender, RoutedEventArgs e)
        {
            var dt = new DataTable();
            dt.Columns.Add("Direction");
            int itemcount = DIRECTION_NAME.Length;
            for (int j = 0; j < itemcount; j++)
            {
                i += j;
                customers.Add(new Customer()
                {
                    Status1 = "stat" + i.ToString(),
                    Status2 = "stat" + i.ToString(),
                    Status3 = "stat" + i.ToString(),
                    Status4 = "stat" + i.ToString(),
                    Status5 = "stat" + i.ToString(),
                    Status6 = "stat" + i.ToString(),
                });
            }
            i++;

            this.dataGrid_PositionStatus.DataContext = dt;
        }
    }
}
