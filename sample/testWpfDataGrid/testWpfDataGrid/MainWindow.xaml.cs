using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace testWpfDataGrid
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly List<Person> baseList;


        //コンストラクタ
        public MainWindow()
        {
            InitializeComponent();

            baseList = new List<Person> {
            new Person(1, "Tanaka", new DateTime(2000, 1, 1)),
            new Person(2, "Yamada", new DateTime(1990, 5, 5)),
            new Person(3, "Sato",   new DateTime(2001, 12, 31)),
        };
            UpdateDispList();
        }

        //表示用リストを再設定
        private void UpdateDispList()
        {
            this.dataGrid.ItemsSource = new ReadOnlyCollection<Person>(baseList);
        }

        //追加ボタンクリック時
        private void OnAddButtonClick(object sender, RoutedEventArgs e)
        {
            baseList.Add(new Person(4, "Yoshida", new DateTime(2002, 2, 2)));
            UpdateDispList();
        }

        //名前変更ボタンクリック時
        private void OnRenameButtonClick(object sender, RoutedEventArgs e)
        {
            var person = this.dataGrid.SelectedItem as Person;
            if (person == null) return;

            var idx = baseList.IndexOf(person);

            //名前だけ変えた新しいインスタンスを作って入れ替える
            var newPerson = new Person(person.No, "hayashi", person.BirthDay);
            baseList[idx] = newPerson;

            UpdateDispList();

            //選択行を再現
            this.dataGrid.SelectedIndex = idx;
        }
    }
}
