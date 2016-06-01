using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Lessons.SampleData
{
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
}
