using Grammars.Contexts;
using Grammars.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;




namespace GrammarsConsole
{
  class Program
  {
    static void Main(string[] args)
    {


      using(var db = new GrammarsContext(true))
      {

        //var pname = db.FindPersonName("Михаил");
        //var pname1 = db.FindPersonName("Анастасия");
        //var pname2 = $"{pname1[Case.DAT]} {pname[pname1.Gender, Case.DAT]}";


        //var grammars = db.ReadGrammarsFromXML(@"D:\DEVELOP\VS\Projects\Grammars\Data.xml");
        //var pnames = db.ReadPersonNamesFromXML(@"D:\DEVELOP\VS\Projects\Grammars\Data.xml");

        //db.LoadGrammarsFromXML(@"D:\DEVELOP\VS\Projects\Grammars\Data.xml");
        //db.LoadPersonNamesFromXML(@"D:\DEVELOP\VS\Projects\Grammars\Data.xml");

        //db.CalculatePersonNamesGrammars();

        //db.Grammars.Include(i => i.GrammarCases).Load();

        //var pname = db.FindPersonName("Михаил");

        //var grammar = db.Grammars.AsEnumerable().FirstOrDefault(f => Regex.IsMatch(pname.Name, $".?{f.Suffix}{(pname.Gender == Gender.M ? f.MForm : f.FForm)}$") && f.BaseEnding == (pname.Gender == Gender.M ? f.MForm : f.FForm) && f.For == pname.Part);


        //var pname = "Куликова";
        //var gender = Gender.F;
        //var grammar = db.Grammars.AsEnumerable().FirstOrDefault(f => f.For == NamePart.LAST && Regex.IsMatch(pname, @$".?{(gender.Equals(Gender.M) ? f.MForm : f.FForm)}$"));


        var res = db.ParsePersonData("иван");

        var rrr = res[Case.LOC];




      };






      














      Console.WriteLine("Hello World!");

    }
  }
}
