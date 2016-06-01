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
using System.ComponentModel;
using System.Runtime.CompilerServices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238



namespace Lessons
{
  public class IntStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return ((int)value).ToString();
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
  public class LengthBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      return ((int)value) != 0;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }

  public class Animal : INotifyPropertyChanged
  {
    private string name;

    [PrimaryKey]
    public int ID { get; set; }
    public string CommonName
    {
      get { return name; }
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
      return String.Format("{0,-6}{1}", ID, CommonName);
    }
  }

  public class AnimalCollection : ObservableCollection<Animal> {}

  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class Case5 : Page
  {
    SQLiteConnection DB;
    public AnimalCollection Animals = new AnimalCollection();

    public Case5()
    {
      this.InitializeComponent();
      DB = new SQLiteConnection("Animals.sqlite");

      DB.CreateTable<Animal>();
      // if we have items in the database, load 'em up
      if (DB.Table<Animal>().Count() > 0)
      {
        // load DB into listbox
        LoadDBList();
      }
      // Update the state buttons
      UpdateCollectionButtons();
      UpdateDatabaseButtons();
      UpdateItemButtons();
    }

 
    private void Db_DeleteAll_Click(object sender, RoutedEventArgs e)
    {
      // remove all items from the database
      DB.DeleteAll<Animal>();
      // remove all listbox items
      DbListBox.Items.Clear();
      // Update database button states
      UpdateDatabaseButtons();
    }

    private void Db_DeleteSelected_Click(object sender, RoutedEventArgs e)
    {
      var id = (DbListBox.SelectedItem as Animal).ID;
      // search the database for that item
      Animal sel = DB.Table<Animal>().Where<Animal>(p => p.ID == id).Single<Animal>();
      // remove that item from the database
      DB.Delete(sel);
      // remove the item from database list
      DbListBox.Items.Remove(DbListBox.SelectedItem);
      DbListBox.SelectedItem = null;
      UpdateDatabaseButtons();
    }

    private void Db_CreateTable_Click(object sender, RoutedEventArgs e)
    {
      // delete all items in the DB
      DB.DeleteAll<Animal>();
      DbListBox.Items.Clear();
      // run through items in collection and add them to DB
      DB.InsertAll(Animals);
      // load DB into listbox
      LoadDBList();
      // update DB button states
      UpdateDatabaseButtons();
    }

    private void Col_Reset_Click(object sender, RoutedEventArgs e)
    {
      // toss all items from collection
      Animals.Clear();
      // add items from static source
      Animals.Add(new Animal() { ID = 0, CommonName = "Dog" });
      Animals.Add(new Animal() { ID = 1, CommonName = "Cat" });
      Animals.Add(new Animal() { ID = 2, CommonName = "Horse" });
      // update collection button states
      UpdateCollectionButtons();
    }

    private void Col_LoadFromDb_Click(object sender, RoutedEventArgs e)
    {
      // toss all items from collection
      Animals.Clear();
      // copy over items from DB list
      foreach (Animal a in DbListBox.Items)
      {
        Animals.Add(a);
      }
      // update collection button states
      UpdateCollectionButtons();
    }

    private void Col_ClearList_Click(object sender, RoutedEventArgs e)
    {
      // clear the collection
      Animals.Clear();
      // update collection button states
      UpdateCollectionButtons();
    }

    private void Col_DeleteSelected_Click(object sender, RoutedEventArgs e)
    {
      // remove the selected item from collection
      Animals.Remove(CoListView.SelectedItem as Animal);
      CoListView.SelectedItem = null;
      // update the collection buttons
      UpdateCollectionButtons();
    }

    private void Item_SaveToCol_Click(object sender, RoutedEventArgs e)
    {
      int id = int.Parse(IdBox.Text);
      // attempt to find item with same ID in collection
      var a = (Animal)(Animals.Where(p => (p as Animal).ID == id).SingleOrDefault());
      // if it exists in the collection, just update it
      if (a != null)
      {
        a.CommonName = NameBox.Text;
      }
      else
      {       
        Animals.Add(new Animal() { ID = id, CommonName = NameBox.Text });
        var ol = Animals.OrderBy(p => (p as Animal).ID).ToList();
        Animals.Clear();
        foreach (Animal oa in ol)
        {
          Animals.Add(oa);
        }
      }
      ClearItem();
      // update collection buttons
      UpdateCollectionButtons();
    }

    private void Item_Create_Click(object sender, RoutedEventArgs e)
    {
      int id = int.Parse(IdBox.Text);
      // if item doesn't exist in database
      var a = DB.Table<Animal>().Where<Animal>(p => p.ID == id).SingleOrDefault<Animal>();
      if (a == null)
      {
        Animal n = new Animal() { ID = id, CommonName = NameBox.Text };
        DB.Insert(n);
        // cheat a bit here... just clear and reload
        DbListBox.Items.Clear();
        LoadDBList();
        UpdateDatabaseButtons();
        ClearItem();
      }
    }

    private void Item_Update_Click(object sender, RoutedEventArgs e)
    {
      int id = int.Parse(IdBox.Text);
      // if item doesn't exist in database
      var a = DB.Table<Animal>().Where<Animal>(p => p.ID == id).SingleOrDefault<Animal>();
      if (a != null)
      {
        a.CommonName = NameBox.Text;
        DB.Update(a);
        DbListBox.Items.Clear();
        LoadDBList();
        UpdateDatabaseButtons();
        ClearItem();
      }
    }

    private void CoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      //// set selected item information in the item box
      //Animal selected = ((sender as ListView).SelectedItem as Animal);
      //if (selected != null)
      //{
      //  NameBox.Text = selected.CommonName;
      //  IdBox.Text = selected.ID.ToString();
      //  UpdateItemButtons();
      //}
      UpdateCollectionButtons();
    }

    private void DbListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      //Animal selected = ((sender as ListView).SelectedItem as Animal);
      //if (selected != null)
      //{
      //  NameBox.Text = selected.CommonName;
      //  IdBox.Text = selected.ID.ToString();
      //  UpdateItemButtons();
      //}
      UpdateDatabaseButtons();
    }
    private void UpdateDatabaseButtons()
    {
      DbDeleteAllButton.IsEnabled = (DbListBox.Items.Count > 0);
      DbDeleteSelectedButton.IsEnabled = (DbListBox.SelectedItem != null);
      CoLoadFromDbButton.IsEnabled = (DbListBox.Items.Count > 0);
    }

    private void UpdateCollectionButtons()
    {
      CoDeleteSelectedButton.IsEnabled = (CoListView.SelectedItem != null);
      CoClearListButton.IsEnabled = (Animals.Count > 0);
      DbCreateTableButton.IsEnabled = (Animals.Count > 0);
    }

    private void UpdateItemButtons()
    {
      //if (NameBox.Text.Length > 0 && IdBox.Text.Length > 0)
      //{
      //  ItemToColButton.IsEnabled = true;
      //  ItemCreateButton.IsEnabled = true;
      //  ItemUpdateButton.IsEnabled = true;
      //}
      //else
      //{
      //  ItemToColButton.IsEnabled = false;
      //  ItemCreateButton.IsEnabled = false;
      //  ItemUpdateButton.IsEnabled = false;
      //}
      ItemToColButton.IsEnabled = false;
      ItemCreateButton.IsEnabled = false;
      ItemUpdateButton.IsEnabled = false;
    }

    private void LoadDBList()
    {
      // get a list of all the people in the DB
      foreach (Animal a in DB.Table<Animal>())
      {
        DbListBox.Items.Add(a);
      }
    }

    private void IdBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateItemButtons();
    }

    private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateItemButtons();
    }

    private void ClearItem()
    {
      NameBox.Text = "";
      IdBox.Text = "";
    }

    private void ChangeColItem_Click(object sender, RoutedEventArgs e)
    {
      int id = int.Parse(ColItemID.Text);
      // attempt to find item with same ID in collection
      var a = (Animal)(Animals.Where(p => (p as Animal).ID == id).SingleOrDefault());
      // if it exists in the collection, just update it
      if (a != null)
      {
        a.CommonName = ColItemName.Text;
      }
      else
      {
        Animals.Add(new Animal() { ID = id, CommonName = ColItemName.Text });

        var ol = Animals.ToList<Animal>();
        ol.Sort((x, y) => { return x.ID - y.ID; });
        

//        var ol = Animals.OrderBy(p => p.ID).ToList<Animal>();
        Animals.Clear();
        foreach (Animal oa in ol) Animals.Add(oa);
      }
      ClearItem();
      // update collection buttons
      UpdateCollectionButtons();
    }
  }
}
