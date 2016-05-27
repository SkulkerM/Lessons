using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using SQLite;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238



namespace Lessons
{

  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Case5 : Page
  {
    ObservableCollection<Animal> Animals = new ObservableCollection<Animal>();

    //
    public class Animal
    {
      [PrimaryKey]
      public int ID { get; set; }
      public string CommonName { get; set; }
    }

    //
    private void initAnimalCollection()
    {
      Animals.Clear();

      Animals.Add(new Animal() { ID = 0, CommonName = "Dog" });
      Animals.Add(new Animal() { ID = 1, CommonName = "Cat" });
      Animals.Add(new Animal() { ID = 2, CommonName = "Horse" });
    }

    public Case5()
    {
      this.InitializeComponent();
      SQLiteConnection DB = new SQLiteConnection("Animals.sqlite");

      DB.CreateTable<Animal>();

      // if the DB has information, start app with that data
      if (DB.Table<Animal>().Count() >= 1)
      {
        // load data from database into collection
//        DBtoList();
//        DisplayDB();
      }
      // if the DB is empty, initialize the collection and load that info
      else
      {
        initAnimalCollection();
        // store collection into database?
//        loadCollection();
      }
    }
  }
}
