using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lessons.SampleData
{
  public class AnimalCollection : ObservableCollection<Animal>
  {
    public AnimalCollection()
    {
      Add(new Animal() { ID = 0, CommonName = "Cat" });
      Add(new Animal() { ID = 1, CommonName = "Dog" });
      Add(new Animal() { ID = 2, CommonName = "Horse" });
    }
  }
}
