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
    SQLiteConnection DB;

    //
    public class Animal
    {
      [PrimaryKey]
      public int ID { get; set; }
      public string CommonName { get; set; }

      public override string ToString()
      {
        return String.Format("{0,-6}{1}", ID, CommonName);
      }
    }


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
      DB.InsertAll(CoListBox.Items);
      // load DB into listbox
      LoadDBList();
      // update DB button states
      UpdateDatabaseButtons();
    }

    private void Col_Reset_Click(object sender, RoutedEventArgs e)
    {
      // toss all items from collection
      CoListBox.Items.Clear();
      // add items from static source
      CoListBox.Items.Add(new Animal() { ID = 0, CommonName = "Dog" });
      CoListBox.Items.Add(new Animal() { ID = 1, CommonName = "Cat" });
      CoListBox.Items.Add(new Animal() { ID = 2, CommonName = "Horse" });
      // update collection button states
      UpdateCollectionButtons();
    }

    private void Col_LoadFromDb_Click(object sender, RoutedEventArgs e)
    {
      // toss all items from collection
      CoListBox.Items.Clear();
      // copy over items from DB list
      foreach (Animal a in DbListBox.Items)
      {
        CoListBox.Items.Add(a);
      }
      // update collection button states
      UpdateCollectionButtons();
    }

    private void Col_ClearList_Click(object sender, RoutedEventArgs e)
    {
      // clear the collection
      CoListBox.Items.Clear();
      // update collection button states
      UpdateCollectionButtons();
    }

    private void Col_DeleteSelected_Click(object sender, RoutedEventArgs e)
    {
      // remove the selected item from collection
      CoListBox.Items.Remove(CoListBox.SelectedItem);
      CoListBox.SelectedItem = null;
      // update the collection buttons
      UpdateCollectionButtons();
    }

    private void Item_SaveToCol_Click(object sender, RoutedEventArgs e)
    {
      int id = int.Parse(IdBox.Text);
      // attempt to find item with same ID in collection
      var a = (Animal)(CoListBox.Items.Where(p => (p as Animal).ID == id).SingleOrDefault());
      // if it exists in the collection, just update it
      if (a != null)
      {
        a.CommonName = NameBox.Text;
      }
      else
      {       
        CoListBox.Items.Add(new Animal() { ID = id, CommonName = NameBox.Text });
        var ol = CoListBox.Items.OrderBy(p => (p as Animal).ID).ToList();
        CoListBox.Items.Clear();
        foreach (Animal oa in ol)
        {
          CoListBox.Items.Add(oa);
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
      // set selected item information in the item box
      Animal selected = ((sender as ListBox).SelectedItem as Animal);
      if (selected != null)
      {
        NameBox.Text = selected.CommonName;
        IdBox.Text = selected.ID.ToString();
        UpdateItemButtons();
      }
      UpdateCollectionButtons();
    }

    private void DbListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Animal selected = ((sender as ListBox).SelectedItem as Animal);
      if (selected != null)
      {
        NameBox.Text = selected.CommonName;
        IdBox.Text = selected.ID.ToString();
        UpdateItemButtons();
      }
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
      CoDeleteSelectedButton.IsEnabled = (CoListBox.SelectedItem != null);
      CoClearListButton.IsEnabled = (CoListBox.Items.Count > 0);
      DbCreateTableButton.IsEnabled = (CoListBox.Items.Count > 0);
    }

    private void UpdateItemButtons()
    {
      if (NameBox.Text.Length > 0 && IdBox.Text.Length > 0)
      {
        ItemToColButton.IsEnabled = true;
        ItemCreateButton.IsEnabled = true;
        ItemUpdateButton.IsEnabled = true;
      }
      else
      {
        ItemToColButton.IsEnabled = false;
        ItemCreateButton.IsEnabled = false;
        ItemUpdateButton.IsEnabled = false;
      }
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
  }
}
