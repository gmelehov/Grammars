using System;
using System.Collections.Generic;
using System.Text;
using Grammars.Enums;





namespace Grammars.Models.Person
{
  /// <summary>
  /// Личные данные (фамилия, имя, отчество, пол).
  /// </summary>
  public class PersonData
  {
    





    public string this[Case @case] => $"{LastNameGrammar?[LastName, Gender, @case]} {FirstNameGrammar?[FirstName, Gender, @case]} {MidNameGrammar?[MidName, Gender, @case]}";





    public string FirstName { get; set; }

    public string MidName { get; set; }

    public string LastName { get; set; }

    public Gender Gender { get; set; }






    public Grammar FirstNameGrammar { get; set; }

    public Grammar MidNameGrammar { get; set; }

    public Grammar LastNameGrammar { get; set; }



  }
}
