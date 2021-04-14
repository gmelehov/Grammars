using EFCore.BulkExtensions;
using Grammars.Enums;
using Grammars.Models.Person;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;





namespace Grammars.Contexts
{
  public partial class GrammarsContext
  {





    /// <summary>
    /// Читает определения частей имен из файла по указанному пути,
    /// создает и возвращает коллекцию объектов <see cref="PersonName"/>
    /// </summary>
    /// <param name="xmlpath">Путь к xml-файлу, содержащему определения частей имен.</param>
    /// <returns></returns>
    public IList<PersonName> ReadPersonNamesFromXML(string xmlpath)
    {
      var xdoc = XDocument.Load(xmlpath);
      return xdoc.Root.Element(G("Names")).Elements().Select(s => new PersonName(s)).ToList();
    }

    /// <summary>
    /// Загружает коллекцию объектов <see cref="PersonName"/>, созданную 
    /// из определений частей имен, найденных в xml-файле по указанному пути,
    /// и сохраняет ее в базу данных.
    /// </summary>
    /// <param name="xmlpath">Путь к xml-файлу, содержащему определения частей имен.</param>
    public void LoadPersonNamesFromXML(string xmlpath)
    {
      var pnames = ReadPersonNamesFromXML(xmlpath);
      PersonNames.AddRange(pnames);
      SaveChanges();
      //this.BulkInsert<PersonName>(pnames);
    }





    /// <summary>
    /// Читает определения грамматик из файла по указанному пути,
    /// создает и возвращает коллекцию объектов <see cref="Grammar"/>
    /// </summary>
    /// <param name="xmlpath">Путь к xml-файлу, содержащему определения грамматик.</param>
    /// <returns></returns>
    public IList<Grammar> ReadGrammarsFromXML(string xmlpath)
    {
      var xdoc = XDocument.Load(xmlpath);
      return xdoc.Root.Element(G("Grammars")).Elements(G("Grammar")).Select(s => new Grammar(s)).ToList();
    }

    /// <summary>
    /// Загружает коллекцию объектов <see cref="Grammar"/>, созданную 
    /// из определений грамматик, найденных в xml-файле по указанному пути,
    /// и сохраняет ее в базу данных.
    /// </summary>
    /// <param name="xmlpath">Путь к xml-файлу, содержащему определения грамматик.</param>
    public void LoadGrammarsFromXML(string xmlpath)
    {
      var grammars = ReadGrammarsFromXML(xmlpath);
      Grammars.AddRange(grammars);
      SaveChanges();
      //this.BulkInsert<Grammar>(grammars);
    }





    public void CalculatePersonNamesGrammars()
    {
      Grammars.Include(i => i.Names).Include(i => i.GrammarCases).Load();
      Grammars.ToList().ForEach(gr =>
      {
        var nomcase = gr.GrammarCases.FirstOrDefault(f => f.Case == Case.NOM);
        var names = PersonNames.AsEnumerable().Where(w => Regex.IsMatch(w.Name, $".?{gr.Suffix}{(w.Gender == Gender.M ? nomcase.MForm : nomcase.FForm)}$") && gr.BaseEnding == (w.Gender == Gender.M ? nomcase.MForm : nomcase.FForm) && gr.For == w.Part);
        foreach (var n in names)
        {
          n.Grammar = gr;
          n.GrammarId = gr.Id;
        };
      });

      SaveChanges();
    }
    public PersonName CalculatePersonNameGrammar(PersonName pname)
    {
      if (pname != null)
      {
        Grammars.Include(i => i.GrammarCases).Load();
        var grammar = Grammars.AsEnumerable().FirstOrDefault(f => Regex.IsMatch(pname.Name, $".?{f.Suffix}{(pname.Gender == Gender.M ? f.MForm : f.FForm)}$") && f.BaseEnding == (pname.Gender == Gender.M ? f.MForm : f.FForm) && f.For == pname.Part);
        if (grammar != null)
        {
          pname.Grammar = grammar;
          pname.GrammarId = grammar.Id;
        };
      };

      return pname;
    }






    public void ProcessXmlData()
    {
      LoadGrammarsFromXML(@"D:\DEVELOP\VS\Projects\Grammars\Data.xml");
      LoadPersonNamesFromXML(@"D:\DEVELOP\VS\Projects\Grammars\Data.xml");

      CalculatePersonNamesGrammars();
    }









    public PersonName FindPersonName(string name) => PersonNames.FirstOrDefault(f => f.Name == name);

    public bool PersonNameExists(string name) => PersonNames.Any(a => a.Name == name);







    public PersonName CreatePersonName(string name, Gender gender, string malemidname = null, string femalemidname = null)
    {
      if (!string.IsNullOrWhiteSpace(name) && !PersonNameExists(name))
      {
        PersonName result = new PersonName
        {
          Name = name,
          Gender = gender,
          Part = NamePart.FIRST
        };
        if (gender == Gender.M && !string.IsNullOrWhiteSpace(malemidname) && !string.IsNullOrWhiteSpace(femalemidname))
        {
          result.Derived.Add(new PersonName { Name = malemidname, Gender = Gender.M, Part = NamePart.MID, BaseName = result });
          result.Derived.Add(new PersonName { Name = femalemidname, Gender = Gender.F, Part = NamePart.MID, BaseName = result });
        };

        return CalculatePersonNameGrammar(result);
      }
      else
      {
        return null;
      }
    }





    public PersonData ParsePersonData(string text)
    {
      Grammars.Include(i => i.Names).Include(i => i.GrammarCases).Load();

      var result = new PersonData();
      string firstName = "";
      string midName = "";
      string lastName = "";
      Gender gender = Gender.N;

      var list = text.Trim(' ').Split(' ').ToList();

      string foundName = PersonNames.AsEnumerable().Select(s => s.Name).Intersect(list).FirstOrDefault();
      PersonName foundPersonName = FindPersonName(foundName);


      if (foundPersonName != null)
      {
        switch (foundPersonName.Part)
        {
          case NamePart.FIRST:
            firstName = foundName;
            result.FirstName = foundName;
            result.FirstNameGrammar = foundPersonName.Grammar;
            break;
          case NamePart.MID:
            midName = foundName;
            result.MidName = foundName;
            result.MidNameGrammar = foundPersonName.Grammar;
            break;

          default:
            break;
        };

        gender = foundPersonName.Gender;
        result.Gender = foundPersonName.Gender;
        list.Remove(foundName);
      };


      foundName = PersonNames.AsEnumerable().Select(s => s.Name).Intersect(list).FirstOrDefault();
      foundPersonName = FindPersonName(foundName);


      if (foundPersonName != null)
      {
        switch (foundPersonName.Part)
        {
          case NamePart.FIRST:
            firstName = foundName;
            result.FirstName = foundName;
            result.FirstNameGrammar = foundPersonName.Grammar;
            break;
          case NamePart.MID:
            midName = foundName;
            result.MidName = foundName;
            result.MidNameGrammar = foundPersonName.Grammar;
            break;

          default:
            break;
        };

        list.Remove(foundName);
      };


      if(list.Count > 0)
      {
        for(var i = 0; i < list.Count; i++)
        {
          var found = Grammars.AsEnumerable().FirstOrDefault(f => f.For == NamePart.LAST && Regex.IsMatch(list[i], @$".?{(gender.Equals(Gender.M) ? f.MForm : f.FForm)}$"));
          if (found != null)
          {
            lastName = list[i];
            result.LastName = list[i];
            result.LastNameGrammar = found;
          }
        };
      };


      if (string.IsNullOrWhiteSpace(lastName))
      {
        lastName = String.Join(" ", list).Trim(' ');
        var found = Grammars.AsEnumerable().FirstOrDefault(f => f.For == NamePart.LAST && Regex.IsMatch(lastName, @$".?{(gender.Equals(Gender.M) ? f.MForm : f.FForm)}$"));
        if (found != null)
        {
          result.LastName = lastName;
          result.LastNameGrammar = found;
        }
      };


      return result;
    }









    public static XName G(string name) => XName.Get(name, "Grammars");

  }
}
