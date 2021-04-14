using Grammars.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;
using static Grammars.Contexts.GrammarsContext;





namespace Grammars.Models.Person
{
  /// <summary>
  /// Грамматика, содержащая правила склонения любых имен указанной гендерной принадлежности,
  /// неизменяемая часть которых оканчивается указанным суффиксом.
  /// </summary>
  public class Grammar
  {
    public Grammar()
    {
      GrammarCases = new List<GrammarCase>();
      Names = new List<PersonName>();
    }
    public Grammar(XElement xelem) : this()
    {
      if(xelem != null)
      {
        var attrFor = xelem.Attribute("for")?.Value;
        var ending = xelem.Attribute("ending")?.Value;
        var nomCase = xelem.Element(G("Nom"));
        var nomLength = nomCase.Attribute("m")?.Value?.Length ?? nomCase.Attribute("f")?.Value?.Length ?? 0;

        Suffix = ending.Substring(0, ending.Length - nomLength);
        BaseEnding = ending.Substring(ending.Length - nomLength);

        switch (attrFor)
        {
          case "name": For = NamePart.FIRST; break;
          case "midname": For = NamePart.MID; break;
          case "surname": For = NamePart.LAST; break;

          default: For = NamePart.LAST; break;
        };


        GrammarCases = xelem.Elements().Select(s => new GrammarCase(this, s)).ToList();

      };
    }









    public void AddDefaultCases()
    {
      if(GrammarCases.Count == 0)
      {
        GrammarCases.Add(new GrammarCase { Case = Case.NOM, MForm = "", FForm = "", Grammar = this });
        GrammarCases.Add(new GrammarCase { Case = Case.GEN, MForm = "", FForm = "", Grammar = this });
        GrammarCases.Add(new GrammarCase { Case = Case.DAT, MForm = "", FForm = "", Grammar = this });
        GrammarCases.Add(new GrammarCase { Case = Case.ACC, MForm = "", FForm = "", Grammar = this });
        GrammarCases.Add(new GrammarCase { Case = Case.LOC, MForm = "", FForm = "", Grammar = this });
        GrammarCases.Add(new GrammarCase { Case = Case.INS, MForm = "", FForm = "", Grammar = this });
      };
    }










    public GrammarCase this[Case @case] => GrammarCases.FirstOrDefault(f=>f.Case == @case);
    public string this[string name, Gender gender, Case @case]
    {
      get
      {
        var nomcase = this[Case.NOM];
        var _case = this[@case];
        var baseending = gender.Equals(Gender.M) ? nomcase.MForm : nomcase.FForm;
        var ending = gender.Equals(Gender.M) ? _case.MForm : _case.FForm;
        return $"{name.Substring(0, name.Length - baseending.Length)}{ending}";
      }
    }









    /// <summary>
    /// Identity.
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Часть имени, правила склонения которой описывает эта грамматика.
    /// </summary>
    [Required, MaxLength(10)]
    public NamePart For { get; set; }

    /// <summary>
    /// Неизменяемая по падежам часть имени (суффикс),
    /// стоящая непосредственно перед изменяемым окончанием.
    /// Определяется в виде регулярного выражения.
    /// Вместе с окончанием формирует уникальный набор правил
    /// для склонения этой части имени по родам и падежам.
    /// </summary>
    [MaxLength(50)]
    public string Suffix { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)]
    public string BaseEnding { get; set; }









    /// <summary>
    /// 
    /// </summary>
    [NotMapped]
    public string NameEnding => @$"{Suffix}{BaseEnding}";

    /// <summary>
    /// Окончание для именительного падежа мужской гендерной формы.
    /// </summary>
    [NotMapped]
    public string MForm => GrammarCases.FirstOrDefault(f => f.Case.Equals(Case.NOM)).MForm;

    /// <summary>
    /// Окончание для именительного падежа женской гендерной формы.
    /// </summary>
    [NotMapped]
    public string FForm => GrammarCases.FirstOrDefault(f => f.Case.Equals(Case.NOM)).FForm;










    /// <summary>
    /// Список правил склонения для каждого падежа.
    /// </summary>
    public virtual List<GrammarCase> GrammarCases { get; set; }

    /// <summary>
    /// Список частей имен, соответствующих правилам этой грамматики.
    /// </summary>
    public virtual List<PersonName> Names { get; set; }


  }
}
