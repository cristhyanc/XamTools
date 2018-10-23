using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamTools.AutoComplete;

namespace XamTools
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new AutoCompleteViewModel();
        }
    }


    public class AutoCompleteViewModel: INotifyPropertyChanged
    {
        public class Person
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }

            public string FullName
            {
                get
                {
                    return (Firstname + " " + Lastname);
                }
            }
            
            public Guid ID { get; set; }

            public override string ToString()
            {
                return  (Firstname + " " + Lastname);
            }
           
        }

        List<Person> _listPerson;

        public List<Person> ListPerson
        {
            get
            {
                return _listPerson;
            }
            set
            {
                _listPerson = value;
                OnPropertyChanged("ListPerson");
            }
        }

        Person _personSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public Person PersonSelected
        {
            get
            {
                return _personSelected;
            }
            set
            {
                _personSelected = value;
                OnPropertyChanged("PersonSelected");
            }
        }

        public void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public AutoCompleteViewModel()
        {
            ListPerson = new List<Person>();
            ListPerson.Add(new Person { Firstname = "Cristhyan", Lastname = "Cardona", ID = Guid.NewGuid() });
            ListPerson.Add(new Person { Firstname = "Sophie", Lastname = "Chung", ID = Guid.NewGuid() });
            ListPerson.Add(new Person { Firstname = "Leo", Lastname = "Shown", ID = Guid.NewGuid() });
        }
    }
}
