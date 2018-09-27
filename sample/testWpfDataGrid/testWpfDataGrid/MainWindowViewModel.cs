using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testWpfDataGrid
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Person[] _persons;

        public Person[] Persons
        {
            get
            {
                return _persons;
            }
            private set
            {
                _persons = value;

                //更新通知
                var h = PropertyChanged;
                if (h != null) h(this, new PropertyChangedEventArgs("Persons"));
            }
        }


        public MainWindowViewModel()
        {
            Persons = new[]
            {
                new Person( 1, "Tanaka", new DateTime(2000, 1, 1) ),
                new Person( 2, "Yamada", new DateTime(1990, 5, 5) ),
                new Person( 3, "Sato"  , new DateTime(2001, 12, 31) ),
            };
        }
    }
}
