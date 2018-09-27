using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testWpfDataGrid
{
    public class Person
    {
        //publicなsetterを作らず、コンストラクタで受け取る
        public int No { get; private set; }
        public string Name { get; private set; }
        public DateTime BirthDay { get; private set; }

        public Person(int no, string name, DateTime birthDay)
        {
            this.No = no;
            this.Name = name;
            this.BirthDay = birthDay;
        }
    }
}
