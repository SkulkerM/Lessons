using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lessons.SampleData
{
  public class SampleAnimalCollection 
  {
    public AnimalCollection Animals = new AnimalCollection();

    public SampleAnimalCollection()
    {
      Animals.Add(new Animal() { ID = 0, CommonName = "Cat" });
      Animals.Add(new Animal() { ID = 1, CommonName = "Dog" });
      Animals.Add(new Animal() { ID = 2, CommonName = "Horse" });
    }
  }
}
