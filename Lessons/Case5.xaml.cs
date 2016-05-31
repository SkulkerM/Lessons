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
      if (DB.Table<Animal>().Count() > 1)
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
      // search the database for that item
      // remove that item from the database

      // remove the item from database list
      DbListBox.Items.Remove(DbListBox.SelectedItem);
      DbListBox.SelectedItem = null;
      UpdateDatabaseButtons();
    }

    private void Db_CreateTable_Click(object sender, RoutedEventArgs e)
    {
      // delete all items in the DB
      DB.DeleteAll<Animal>();
      // run through items in collection and add them to DB

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
      // take item data - ID and name, check for matching name in collection
      // on ID match, update the name?
      // otherwise, add to collection
      // update collection buttons
      UpdateCollectionButtons();
    }

    private void Item_Create_Click(object sender, RoutedEventArgs e)
    {
      // if item doesn't exist in database
        // add new item to database
        // add item to list
        // sort the list?
        // Update database buttons

    }

    private void Item_Update_Click(object sender, RoutedEventArgs e)
    {
      // find item in database
        // update item name
        // update the list
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
      DbCreateTableButton.IsEnabled = (CoListBox.Items.Count > 0);
    }

    private void UpdateCollectionButtons()
    {
      CoDeleteSelectedButton.IsEnabled = (CoListBox.SelectedItem != null);
      CoClearListButton.IsEnabled = (CoListBox.Items.Count > 0);
      CoLoadFromDbButton.IsEnabled = (DbListBox.Items.Count > 0);
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

    }

    private void IdBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateItemButtons();
    }

    private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdateItemButtons();
    }
  }
}
