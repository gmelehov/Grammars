using Grammars.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Xml.Linq;




namespace Grammars.Models.Person
{
  /// <summary>
  /// Имя.
  /// </summary>
  public class PersonName
  {
    public PersonName()
    {
      Derived = new List<PersonName>();
    }
    public PersonName(XElement xelem) : this()
    {
      if(xelem != null)
      {
        Name = xelem.Attribute("val").Value;
        Gender = xelem.Name.LocalName == "M.Name" ? Gender.M : xelem.Name.LocalName == "F.Name" ? Gender.F : Gender.N;
        Part = NamePart.FIRST;

        if(xelem.Name.LocalName == "M.Name")
        {
          var mmid = xelem.Attribute("m.mid").Value;
          var fmid = xelem.Attribute("f.mid").Value;

          Derived.Add(new PersonName { Name = mmid, Gender = Gender.M, Part = NamePart.MID, BaseName = this });
          Derived.Add(new PersonName { Name = fmid, Gender = Gender.F, Part = NamePart.MID, BaseName = this });
        };
      };
    }
    






    public void AddDefaultDerivedNames()
    {
      if(Gender == Gender.M && Derived.Count == 0)
      {
        Derived.Add(new PersonName { Name = "", Gender = Gender.M, Part = NamePart.MID, BaseName = this });
        Derived.Add(new PersonName { Name = "", Gender = Gender.F, Part = NamePart.MID, BaseName = this });
      };
    }








    public string this[Case @case]
    {
      get
      {
        var baseending = Grammar?.BaseEnding;
        var _case = Grammar?.GrammarCases?.FirstOrDefault(f => f.Case == @case);
        var ending = Gender == Gender.M ? _case.MForm : _case.FForm;
        return $"{Name.Substring(0, Name.Length - baseending.Length)}{ending}";
      }
    }
    public string this[Gender gender, Case @case] => gender == Gender.M ? GetMaleMidname()?[@case] : gender == Gender.F ? GetFemaleMidname()?[@case] : null;
    public PersonName this[Gender gender] => Derived.FirstOrDefault(f => f.Gender == gender);






    public PersonName GetMaleMidname() => Gender == Gender.M ? Derived?.FirstOrDefault(f => f.Gender == Gender.M && f.Part == NamePart.MID) : null;
    public PersonName GetFemaleMidname() => Gender == Gender.M ? Derived?.FirstOrDefault(f => f.Gender == Gender.F && f.Part == NamePart.MID) : null;








    /// <summary>
    /// Identity.
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Имя, в единственном числе, в именительном падеже.
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Гендерное соответствие.
    /// </summary>
    [Required, MaxLength(3)]
    public Gender Gender { get; set; }

    /// <summary>
    /// Часть имени.
    /// </summary>
    [Required, MaxLength(5)]
    public NamePart Part { get; set; }








    

    [NotMapped]
    public string RegExpr => @$".?{Grammar.Suffix}{Grammar.BaseEnding}$";

    //public string RegExpr => (Grammar.BaseEnding == (Gender == Gender.M ? Grammar.MForm : Grammar.FForm) && Grammar.For == Part) ? @$".?{Grammar.Suffix}{Grammar.BaseEnding}$" : null;


    [NotMapped]
    public string NameEnding => @$"{Grammar.Suffix}{Grammar.BaseEnding}";

    //public string NameEnding => (Grammar.For.Equals(Part) && Grammar.BaseEnding == (Gender.Equals(Gender.M) ? Grammar.MForm : Grammar.FForm)) ? @$"{Grammar.Suffix}{Grammar.BaseEnding}" : null;









    /// <summary>
    /// Список частей имени, производных от этой части имени.
    /// </summary>
    public virtual List<PersonName> Derived { get; set; }










    /// <summary>
    /// Внешний ключ.
    /// </summary>
    public int? BaseId { get; set; }

    /// <summary>
    /// Ссылка на базовое имя, производным от которого является эта часть имени.
    /// </summary>
    [ForeignKey("BaseId")]
    public virtual PersonName BaseName { get; set; }








    /// <summary>
    /// Внешний ключ.
    /// </summary>
    public int? GrammarId { get; set; }

    /// <summary>
    /// Ссылка на грамматику, правилам которой соответствует эта часть имени.
    /// </summary>
    [ForeignKey("GrammarId")]
    public virtual Grammar Grammar { get; set; }









    public override string ToString() => $"{Name}";

  }
}
